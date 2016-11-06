using Contract;
using Proxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            MainAsync().Wait();
        }

        static async Task MainAsync()
        {
            var svcProxy = new ServiceProxy<ITestService>();
            var svc = svcProxy.GetService();

            var primitiveResult = svc.TestPrimitive("passed");
            var complexResult = svc.TestComplex(new TestModel { Prop1 = "passed" });

            Console.WriteLine($"If async works, this line should print before results");

            Console.WriteLine($"test primitive get: {await primitiveResult}");
            Console.WriteLine($"test complex get: {(await complexResult).Prop1}");

            Console.Read();
        }
    }
}
