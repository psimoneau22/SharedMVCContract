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
        private const string baseAddress = "http://api/Service";
        private HttpClient HttpClient { get; }
        private MethodInfo GenericResultMethod { get; }

        public HttpServiceProxyInterceptor()
        {
            this.GenericResultMethod = typeof(HttpServiceProxyInterceptor).GetMethod("GetGenericResultDynamic", BindingFlags.Instance | BindingFlags.NonPublic);
            this.HttpClient = new HttpClient();
        }

        public void Intercept(IInvocation invocation)
        {
            bool isGeneric = invocation.Method.ReturnType.IsGenericType && invocation.Method.ReturnType.GetGenericTypeDefinition() == typeof(Task<>);
            if (invocation.Method.ReturnType != typeof(Task) && !isGeneric)
            {
                throw new InvalidOperationException("Synchronous service operation cannot be called via HttpServiceProxy");
            };

            invocation.ReturnValue = isGeneric ? this.GetGenericResult(invocation) : this.GetResult(invocation);
        }

        private object GetGenericResult(IInvocation invocation)
        {
            return this.GenericResultMethod
                .MakeGenericMethod(invocation.Method.ReturnType.GetGenericArguments()[0])
                .Invoke(this, new object[] { invocation });
        }

        private Task<T> GetGenericResultDynamic<T>(IInvocation invocation)
        {
            return this.SendRequest(invocation)
                .ContinueWith(t => t.Result.Result)
                .ContinueWith(t => JsonConvert.DeserializeObject<T>(t.Result));
        }

        private Task GetResult(IInvocation invocation)
        {
            return this.SendRequest(invocation).ContinueWith(t => { });
        }

        private Task<Task<string>> SendRequest(IInvocation invocation)
        {
            return this.HttpClient.SendAsync(this.GetMessage(invocation))
                .ContinueWith(t => t.Result.Content.ReadAsStringAsync());
        }

        private HttpRequestMessage GetMessage(IInvocation invocation)
        {
            var contractServiceAttribute = Attribute.GetCustomAttribute(invocation.Method.DeclaringType, typeof(BeelineHttpServiceAttribute)) as BeelineHttpServiceAttribute;
            var contractRouteAttribute = Attribute.GetCustomAttribute(invocation.Method, typeof(RouteAttribute)) as RouteAttribute;
            var routePrefix = contractServiceAttribute?.Prefix ?? invocation.Method.DeclaringType.Name;
            var routeAction = contractRouteAttribute?.Template ?? invocation.Method.Name;
            var args = invocation.Method.GetParameters().ToDictionary(p => p.Name, p => invocation.Arguments[p.Position]);

            return new HttpRequestMessage
            {
                RequestUri = new Uri($"{baseAddress}/{routePrefix}/{routeAction}"),
                Method = HttpMethod.Post,
                Content = new StringContent(JsonConvert.SerializeObject(args), Encoding.UTF8, "application/json")
            };
        }
    }
}
