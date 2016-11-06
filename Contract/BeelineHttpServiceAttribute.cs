using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Contract
{
    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = false, Inherited = true)]
    public class BeelineHttpServiceAttribute : RoutePrefixAttribute
    {
        public BeelineHttpServiceAttribute(string prefix) : base(prefix) { }
    }
}
