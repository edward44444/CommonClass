using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Caching;

namespace CommonClass
{
    public abstract class CommonCacheDependency
    {
        public abstract CommonDependencyManager DependencyManager { get; }

        public CacheDependency InnerCacheDependency { get; private set; }

        protected string _depkey;
        public string DependencyKey
        {
            get { return _depkey; }
            set { _depkey = value; }
        }

        public bool HasChanged
        {
            get
            {
                return InnerCacheDependency.HasChanged;
            }
        }

        public DateTime UtcLastModified
        {
            get
            {
                return InnerCacheDependency.UtcLastModified;
            }
        }

        public CommonCacheDependency(string depkey)
            : base()
        {
            this._depkey = depkey;
            DependencyManager.EnsureDependItemIsPooled();
            InnerCacheDependency = new CacheDependency(null, new string[] { DependencyManager.GetMoniterKey(depkey) });
            DependencyManager.InitialPolling();
        }
    }
}
