using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Caching;

namespace CommonClass
{
    public class User
    {
        public string Name { get; set; }

        public DateTime LastCommentTime { get; set; }

        public User(string name, DateTime lastCommentTime)
        {
            this.Name = name;
            this.LastCommentTime = lastCommentTime;
        }
    }

    public class CustomCacheDependencyManager
    {
        private class NotifyState
        {
            public int RefCount { get; set; }

            public string DepKey { get; set; }
        }

        private static System.Threading.Timer _timer;

        private static Dictionary<string, NotifyState> _notifyStates;

        private static object SyncObject = new object();

        private static int _poolTime = 1000 * 10;

        static CustomCacheDependencyManager()
        {
            _notifyStates = new Dictionary<string, NotifyState>();
        }

        public static void Start()
        {
            if (_timer == null)
            {
                lock (SyncObject)
                {
                    if (_timer == null)
                    {
                        InitialStates();
                        _timer = new System.Threading.Timer(PollForChanges, _notifyStates, _poolTime, _poolTime);
                    }
                }
            }
            
        }

        private static void InitialStates()
        {
            _notifyStates.Add("edward", new NotifyState());
            _notifyStates.Add("john", new NotifyState());
            foreach (var key in _notifyStates.Keys)
            {
                HttpRuntime.Cache.Add(GetMoniterKey(key), DateTime.Now, null, Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration, CacheItemPriority.Normal, null);
            }
        }

        public static void AddRef(string depKey)
        {
            if (_notifyStates.ContainsKey(depKey))
            {
                _notifyStates[depKey].RefCount += 1;
            }
        }

        public static void RemoveRef(string depKey)
        {
            if (_notifyStates.ContainsKey(depKey))
            {
                _notifyStates[depKey].RefCount -= 1;
            }
        }

        private static void PollForChanges(object state)
        {
            List<User> lstUser = new List<User>();
            lstUser.Add(new User("edward", DateTime.Now));
            lstUser.Add(new User("john", DateTime.Now));
            foreach (User user in lstUser)
            {
                if (_notifyStates[user.Name].RefCount == 0)
                {
                    continue;
                }
                string moniterKey=GetMoniterKey(user.Name);
                if (HttpRuntime.Cache.Get(moniterKey) == null)
                {
                    HttpRuntime.Cache.Add(moniterKey, user.LastCommentTime, null, Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration, CacheItemPriority.Normal, null);
                    continue;
                }
                if (!((DateTime)HttpRuntime.Cache.Get(moniterKey)).Equals(user.LastCommentTime))
                {
                    HttpRuntime.Cache.Insert(moniterKey, user.LastCommentTime);
                }
            }
        }

        public static string GetMoniterKey(string depKey)
        {
            return "CustomCacheDependency:" + depKey;
        }

        public static void Stop()
        {
            if (_timer != null)
            {
                _timer.Dispose();
                _timer = null;
            }
        }
    }
}
