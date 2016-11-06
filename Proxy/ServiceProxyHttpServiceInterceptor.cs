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
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Async;
using System.Web.Mvc.Routing;
using System.Web.Routing;

namespace Proxy
{
    public class ServiceProxyHttpServiceInterceptor : IInterceptor
    {
        private const string baseServiceAddress = "http://api/Service";

        public async void Intercept(IInvocation invocation)
        {
            var contractServiceAttribute = Attribute.GetCustomAttribute(invocation.Method.DeclaringType, typeof(BeelineHttpServiceAttribute)) as BeelineHttpServiceAttribute;
            var contractRouteAttribute = Attribute.GetCustomAttribute(invocation.Method, typeof(RouteAttribute)) as RouteAttribute;

            var args = invocation.Method.GetParameters().ToDictionary(p => p.Name, p => invocation.Arguments[p.Position]);
            var message = new HttpRequestMessage
            {
                RequestUri = new Uri($"{baseServiceAddress}/{contractServiceAttribute.Prefix}/{contractRouteAttribute.Template}"),
                Method = HttpMethod.Post,
                Content = new StringContent(JsonConvert.SerializeObject(args), Encoding.UTF8, "application/json")
            };

            using (var client = new HttpClient())
            {
                // todo, make this proper async
                var result = client.SendAsync(message).Result;
                var resultContent = await result.Content.ReadAsStringAsync();
                dynamic actualResult = JsonConvert.DeserializeObject(resultContent, invocation.Method.ReturnType.GetGenericArguments()[0]);
                invocation.ReturnValue = Task.FromResult(actualResult);
            }
        }
    }
}
