using Contract;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;

namespace Service.Controllers
{    
    public class TestServiceController : Controller, ITestService
    {        
        public async Task<string> TestPrimitive(string param1)
        {
            return await Task.FromResult($"test {param1}");
        }

        public async Task<TestModel> TestComplex(TestModel param1)
        {
            return await Task.FromResult(new TestModel { Prop1 = $"test {param1.Prop1}"});
        }
    }    
}