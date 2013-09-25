using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Caching;
using System.Web;
using System.Collections;
using System.Threading;

namespace CommonClass
{
    public class UserConfigCacheDependency : CommonCacheDependency
    {
        private static CommonCacheDependencyManager _cacheDependencyManager = new UserConfigCacheDependencyManager();

        public UserConfigCacheDependency(string depkey)
            : base(depkey)
        {
           
        }

        public override CommonCacheDependencyManager CacheDependencyManager
        {
            get { return _cacheDependencyManager; }
        }
    }
}
