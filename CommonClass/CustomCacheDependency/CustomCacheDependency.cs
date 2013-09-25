using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Caching;

namespace CommonClass
{
    public class CustomCacheDependency : CacheDependency
    {
        private string _depKey;

        public CustomCacheDependency(string depKey)
            : base(null, new string[] { CustomCacheDependencyManager.GetMoniterKey(depKey) })
        {
            _depKey = depKey;
            CustomCacheDependencyManager.AddRef(depKey);
            FinishInit();
        }

        public override string GetUniqueID()
        {
            return Guid.NewGuid().ToString();
        }

        protected override void DependencyDispose()
        {
            CustomCacheDependencyManager.RemoveRef(_depKey);
            base.DependencyDispose();
        }
    }
}
