using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonClass
{
    public class DependencyKeyException:Exception
    {
        public DependencyKeyException(string message)
            : base(message)
        {
        }
    }
}
