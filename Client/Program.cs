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
            var svcProxy = new HttpServiceProxy<ITestService>();
            var svc = svcProxy.GetService();

            var primitiveResult = svc.TestPrimitive("passed");
            var complexResult = svc.TestComplex(new TestModel { Prop1 = "passed" });
            var voidResult = svc.TestVoid();

            Console.WriteLine($"this should write to the console 5 seconds before the next statement");            
            Console.WriteLine($"test primitive: {await primitiveResult}");            
            Console.WriteLine($"test complex: {(await complexResult).Prop1}");
            await voidResult;
            Console.WriteLine("test void: test passed");

            Console.Read();
        }
    }
}
