using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonClass
{
    public class UserConfig : IDependencyItem
    {
        public string DependencyKey { get; set; }

        public int Version { get; set; }
    }
}
