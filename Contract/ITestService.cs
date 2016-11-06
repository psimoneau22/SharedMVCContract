using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Contract
{
    [BeelineHttpService("TestService")]
    public interface ITestService
    {
        [Route("Prim")]
        Task<string> TestPrimitive(string param1);

        [Route("Comp")]
        Task<TestModel> TestComplex(TestModel param1);
    }
    
    public class TestModel
    {
        public string Prop1 { get; set; }
    }
}
