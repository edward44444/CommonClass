using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Caching;
using System.Web;

namespace CommonClass
{
    public class CacheHelper
    {
        private static Cache _cache = HttpRuntime.Cache;

        public static void Insert(string key, object value, CommonCacheDependency dependencies)
        {
            if (dependencies.HasChanged)
            {
                CommonCacheDependencyManager.EnsureDependencyItemIsPooled();
                string depkey = dependencies.DependencyKey;
                dependencies = new CommonCacheDependency(depkey);
            }
            _cache.Insert(key, value, dependencies);
        }
    }
}
