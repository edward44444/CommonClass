using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Caching;
using System.Web;
using System.Collections;
using System.Threading;

namespace CommonClass.Backup
{
    public interface IDependencyItem
    {
        string DependencyKey { get; }

        int Version { get;}
    }

    public class CommonCacheDependencyManager
    {
        public static bool Shutdown = true;

        private class DependencyItemState
        {
            public string DependencyKey;

            public int CurrentVersion;

            public int RefCount;
        }

        private static System.Threading.Timer _timer;

        private static int _pollTime = 1000 * 10;

        private static Dictionary<string, DependencyItemState> _depItems;

        private static readonly object SyncObject = new object();

        public static string GetMoniterKey(string depKey)
        {
            return depKey + ":CommonClass.CommonCacheDependency";
        }

        public static void InitialPolling()
        {
            if (_timer == null)
            {
                lock (SyncObject)
                {
                    if (_timer == null)
                    {
                        _timer = new System.Threading.Timer(PollForChanges, null, _pollTime, _pollTime);
                        Shutdown = false;
                    }
                }
            }
        }

        public static void EnsureDependencyItemIsPooled()
        {
            if (_depItems == null)
            {
                lock (SyncObject)
                {
                    if (_depItems == null)
                    {
                        _depItems = new Dictionary<string, DependencyItemState>();
                        foreach (var item in GetDependencyItems())
                        {
                            _depItems.Add(item.DependencyKey, new DependencyItemState { DependencyKey = item.DependencyKey, CurrentVersion = item.Version });
                            string moniterKey = GetMoniterKey(item.DependencyKey);
                            if (HttpRuntime.Cache.Get(moniterKey) == null)
                            {
                                HttpRuntime.Cache.Insert(moniterKey, item.Version);
                            }
                        }
                    }
                }
            }
        }

        public static void AddRef(string depKey)
        {
            try
            {
                DependencyItemState item = _depItems[depKey];
                Interlocked.Increment(ref item.RefCount);
            }
            catch(KeyNotFoundException ex)
            {
                throw new DependencyKeyException("the dependencyKey is not supported");
            }
        }

        public static void RemoveRef(string depKey)
        {
            try
            {
                DependencyItemState item = _depItems[depKey];
                Interlocked.Decrement(ref item.RefCount);
            }
            catch (KeyNotFoundException ex)
            {
            }
        }

        private static List<IDependencyItem> GetDependencyItems()
        {
            List<IDependencyItem> items = new List<IDependencyItem>();
            //items.Add(new UserConfig { DependencyKey = "edward", Version = new Random(Guid.NewGuid().GetHashCode()).Next(1,10) });
            //items.Add(new UserConfig { DependencyKey = "john", Version = 1 });
            return items;
        }

        private static void PollForChanges(object state)
        {
            try
            {
                foreach (var item in GetDependencyItems())
                {
                    if (_depItems[item.DependencyKey].RefCount == 0)
                    {
                        continue;
                    }
                    string moniterKey = GetMoniterKey(item.DependencyKey);
                    if (!((int)HttpRuntime.Cache.Get(moniterKey)).Equals(item.Version))
                    {
                        HttpRuntime.Cache.Insert(moniterKey, item.Version);
                    }
                }
            }
            catch
            {
            }
        }

        public static void Stop()
        {
            if (_timer != null)
            {
                _timer.Dispose();
                _timer = null;
                Shutdown = true;
            }
        }
    }

    public class CommonCacheDependency : CacheDependency
    {
        private string _depkey;
        public string DependencyKey
        {
            get { return _depkey; }
            set { _depkey = value; }
        }

        public CommonCacheDependency(string depkey)
            : base(null, new string[] { CommonCacheDependencyManager.GetMoniterKey(depkey) })
        {
            this._depkey = depkey;
            CommonCacheDependencyManager.EnsureDependencyItemIsPooled();
            CommonCacheDependencyManager.AddRef(depkey);
            if (CommonCacheDependencyManager.Shutdown == true)
            {
                CommonCacheDependencyManager.InitialPolling();
            }
            FinishInit();
        }

        public override string GetUniqueID()
        {
            return Guid.NewGuid().ToString();
        }

        protected override void DependencyDispose()
        {
            CommonCacheDependencyManager.RemoveRef(_depkey);
            base.DependencyDispose();
        }
    }
}
