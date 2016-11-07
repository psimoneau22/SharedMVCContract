using Castle.DynamicProxy;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Proxy
{
    public class HttpServiceProxy<T> where T : class
    {
        private static ProxyGenerator _generator;
        private static ProxyGenerator ServiceGenerator
        {
            get
            {
                if (_generator == null)
                {
                    _generator = new ProxyGenerator();
                }
                return _generator;
            }
        }

        public T GetService()
        {
            
            return ServiceGenerator.CreateInterfaceProxyWithoutTarget<T>(ProxyGenerationOptions.Default, new HttpServiceProxyInterceptor());
        }
    }
}