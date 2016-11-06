using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Async;
using System.Web.Routing;
using System.Web.SessionState;

namespace Service
{
    public class BeelineServiceControllerFactory : DefaultControllerFactory
    {
        public override IController CreateController(RequestContext requestContext, string controllerName)
        {     
            var controller = base.CreateController(requestContext, controllerName) as Controller;
            controller.ActionInvoker = new ServiceActionInvoker();

            return controller;
        }
    }

    public class ServiceActionInvoker : AsyncControllerActionInvoker
    {        
        protected override ActionResult CreateActionResult(ControllerContext controllerContext, ActionDescriptor actionDescriptor, object actionReturnValue)
        {            
            var actionResult = actionReturnValue as ActionResult;
            if (actionResult == null)
            {
                controllerContext.Controller.ViewData.Model = actionReturnValue;
                var result = new JsonResult
                {
                    Data = actionReturnValue,
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet                    
                };
                return result;
            }
            return base.CreateActionResult(controllerContext, actionDescriptor, actionReturnValue);
        }
    }
}