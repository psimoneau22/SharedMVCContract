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
        private static ProxyGenerator ServiceGenerator { get; set; }
        private static HttpServiceProxyInterceptor Interceptor { get; set; }

        public HttpServiceProxy(){
            if (ServiceGenerator == null)
            {
                ServiceGenerator = new ProxyGenerator();
            }

            if (Interceptor == null)
            {
                Interceptor = new HttpServiceProxyInterceptor();
            }
        }

        public T GetService()
        {
            return ServiceGenerator.CreateInterfaceProxyWithoutTarget<T>(ProxyGenerationOptions.Default, Interceptor);
        }
    }
}