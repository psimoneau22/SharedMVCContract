using Castle.DynamicProxy;
using Contract;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Async;
using System.Web.Mvc.Routing;
using System.Web.Routing;

namespace Proxy
{
    public class HttpServiceProxyInterceptor : IInterceptor
    {
        private const string baseServiceAddress = "http://api/Service";
        private MethodInfo getGenericResultMethod;

        public HttpServiceProxyInterceptor()
        {
            this.getGenericResultMethod = this.GetType().GetMethod("GetGenericResult", BindingFlags.Instance | BindingFlags.NonPublic);
        }

        public void Intercept(IInvocation invocation)
        {
            bool isGeneric = invocation.Method.ReturnType.IsGenericType && invocation.Method.ReturnType.GetGenericTypeDefinition() == typeof(Task<>);
            if (invocation.Method.ReturnType != typeof(Task) && !isGeneric)
            {
                throw new InvalidOperationException("Synchronous service operation cannot be called via HttpServiceProxy");
            };
            
            if (isGeneric)
            {
                invocation.ReturnValue = getGenericResultMethod
                    .MakeGenericMethod(invocation.Method.ReturnType.GetGenericArguments()[0])
                    .Invoke(this, new object[] { invocation });
            }
            else
            {
                invocation.ReturnValue = GetVoidResult(invocation);
            }            
        }

        private Task<Task<string>> GetResult(IInvocation invocation)
        {
            var message = GetMessage(invocation);
            var client = new HttpClient();
            
            return client.SendAsync(message)
                .ContinueWith(t => t.Result.Content.ReadAsStringAsync());                
        }

        private Task GetVoidResult(IInvocation invocation)
        {
            return GetResult(invocation).ContinueWith(t => { });
        }

        private Task<T> GetGenericResult<T>(IInvocation invocation)
        {
            return GetResult(invocation)
                .ContinueWith(t => t.Result.Result)
                .ContinueWith(t => JsonConvert.DeserializeObject<T>(t.Result));
        }

        private HttpRequestMessage GetMessage(IInvocation invocation)
        {
            var contractServiceAttribute = Attribute.GetCustomAttribute(invocation.Method.DeclaringType, typeof(BeelineHttpServiceAttribute)) as BeelineHttpServiceAttribute;
            var contractRouteAttribute = Attribute.GetCustomAttribute(invocation.Method, typeof(RouteAttribute)) as RouteAttribute;
            var args = invocation.Method.GetParameters().ToDictionary(p => p.Name, p => invocation.Arguments[p.Position]);

            return new HttpRequestMessage
            {
                RequestUri = new Uri($"{baseServiceAddress}/{contractServiceAttribute.Prefix}/{contractRouteAttribute.Template}"),
                Method = HttpMethod.Post,
                Content = new StringContent(JsonConvert.SerializeObject(args), Encoding.UTF8, "application/json")
            };
        }
    }
}
