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
        private static CommonDependencyManager _dependencyManager = new UserConfigCacheDependencyManager();

        public UserConfigCacheDependency(string depkey)
            : base(depkey)
        {
           
        }

        public override CommonDependencyManager DependencyManager
        {
            get { return _dependencyManager; }
        }
    }
}
