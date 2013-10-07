using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace CommonClass
{
    public class UserConfigCacheDependencyManager : CommonCacheDependencyManager<UserConfig>
    {
        private static int _index = 0;

        protected override IList<UserConfig> GetDependItems()
        {
            List<UserConfig> lstUserConfig = new List<UserConfig>();
            lstUserConfig.Add(new UserConfig { Name = "edward", Version = new Random(Guid.NewGuid().GetHashCode()).Next(1, 10) });
            lstUserConfig.Add(new UserConfig { Name = "john", Version = 1 });
            if (_index>0)
            {
                lstUserConfig.Add(new UserConfig { Name = "Jim", Version = new Random(Guid.NewGuid().GetHashCode()).Next(1, 10) });
            }
            _index++;
            return lstUserConfig;
        }

        protected override string GetDependKey(UserConfig instance)
        {
            return instance.Name;
        }

        protected override object GetDependValue(UserConfig instance)
        {
            return instance.Version;
        }
    }
}
