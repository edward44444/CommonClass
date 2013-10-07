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
                _cache.Insert(key, value, null, DateTime.UtcNow.AddMinutes(1), Cache.NoSlidingExpiration);
                return;
            }
            _cache.Insert(key, value, dependencies.InnerCacheDependency);
        }
    }
}
