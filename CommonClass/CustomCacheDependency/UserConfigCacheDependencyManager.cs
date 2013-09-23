using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace CommonClass
{
    public class UserConfigCacheDependencyManager : CommonDependencyManager
    {
        private Dictionary<string, UserConfig> _depItems;

        public override void EnsureDependencyItemIsPooled()
        {
            if (_depItems == null)
            {
                lock (SyncObject)
                {
                    if (_depItems == null)
                    {
                        _depItems = new Dictionary<string, UserConfig>();
                        foreach (var item in GetUserConfigList())
                        {
                            _depItems.Add(item.Name, item);
                            string moniterKey = GetMoniterKey(item.Name);
                            HttpRuntime.Cache.Insert(moniterKey, item.Version);
                        }
                    }
                }
            }
        }

        private List<UserConfig> GetUserConfigList()
        {
            List<UserConfig> lstUserConfig = new List<UserConfig>();
            lstUserConfig.Add(new UserConfig { Name = "edward", Version = new Random(Guid.NewGuid().GetHashCode()).Next(1, 10) });
            lstUserConfig.Add(new UserConfig { Name = "john", Version = 1 });
            return lstUserConfig;
        }

        protected override void PollForChanges(object state)
        {
            foreach (var item in GetUserConfigList())
            {
                string moniterKey = GetMoniterKey(item.Name);
                if (!((int)HttpRuntime.Cache.Get(moniterKey)).Equals(item.Version))
                {
                    HttpRuntime.Cache.Insert(moniterKey, item.Version);
                }
            }
        }
    }
}
