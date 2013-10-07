using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace CommonClass
{
    public abstract class CommonCacheDependencyManager<T> : CommonCacheDependencyManager
    {
        protected Dictionary<string, T> _depItems;

        protected abstract IList<T> GetDependItems();

        protected abstract string GetDependKey(T instance);

        protected abstract object GetDependValue(T instance);

        public override void EnsureDependItemIsPooled()
        {
            try
            {
                if (_depItems == null)
                {
                    lock (SyncObject)
                    {
                        if (_depItems == null)
                        {
                            _depItems = new Dictionary<string, T>();
                            foreach (var item in GetDependItems())
                            {
                                string depKey = GetDependKey(item);
                                object depValue = GetDependValue(item);
                                string moniterKey = GetMoniterKey(depKey);
                                _depItems.Add(depKey, item);
                                HttpRuntime.Cache.Insert(moniterKey, depValue);
                            }
                        }
                    }
                }
            }
            catch
            {
            }
        }

        protected override void PollForChanges(object state)
        {
            try
            {
                foreach (var item in GetDependItems())
                {
                    string depKey = GetDependKey(item);
                    object depValue = GetDependValue(item);
                    string moniterKey = GetMoniterKey(depKey);
                    object cachedItem = HttpRuntime.Cache.Get(moniterKey);
                    if (cachedItem == null)
                    {
                        HttpRuntime.Cache.Insert(moniterKey, depValue);
                        continue;
                    }
                    if (!cachedItem.Equals(depValue))
                    {
                        HttpRuntime.Cache.Insert(moniterKey, depValue);
                    }
                }
            }
            catch
            {
            }
        }
    }

    public abstract class CommonCacheDependencyManager
    {
        private System.Threading.Timer _timer;

        private int _pollTime = 1000 * 10;
        public int PollTime
        {
            get { return _pollTime; }
            set
            {
                _pollTime = value;
                if (_timer != null)
                {
                    _timer.Change(_pollTime, _pollTime);
                }
            }
        }

        protected readonly object SyncObject = new object();

        public void InitialPolling()
        {
            if (_timer == null)
            {
                lock (SyncObject)
                {
                    if (_timer == null)
                    {
                        _timer = new System.Threading.Timer(PollForChanges, null, _pollTime, _pollTime);
                    }
                }
            }
        }

        public abstract void EnsureDependItemIsPooled();
        
        protected abstract void PollForChanges(object state);

        public string GetMoniterKey(string depKey)
        {
            return depKey + ":CacheDependencyManager." + this.GetType().Name;
        }

        public void Stop()
        {
            if (_timer != null)
            {
                _timer.Dispose();
                _timer = null;
            }
        }    
    }
}
