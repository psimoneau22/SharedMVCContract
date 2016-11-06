using Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Mvc;
using System.Web.Mvc.Routing;

namespace Service
{
    public class BeelineServiceRouteProvider : DefaultDirectRouteProvider
    {
        
        protected override string GetRoutePrefix(ControllerDescriptor controllerDescriptor)
        {            
            var serviceContract = GetBeelineServiceContract(controllerDescriptor.ControllerType);
            var serviceAttribute = Attribute.GetCustomAttribute(serviceContract, typeof(BeelineHttpServiceAttribute)) as BeelineHttpServiceAttribute;
            return serviceAttribute.Prefix;
        }

        protected override IReadOnlyList<IDirectRouteFactory> GetActionRouteFactories(ActionDescriptor actionDescriptor)
        {
            var controller = actionDescriptor.ControllerDescriptor.ControllerType;
            var method = controller.GetMethod(actionDescriptor.ActionName);
            var contract = GetBeelineServiceContract(controller);

            return new List<IDirectRouteFactory>(
                // get routes from controller method
                actionDescriptor.GetCustomAttributes(typeof(IDirectRouteFactory), true).OfType<IDirectRouteFactory>().Union(

                // get routes from interface method implemented by the controller
                GetBeelineServiceContractRoutes(method, controller, contract))
            );
        }

        private IEnumerable<IDirectRouteFactory> GetBeelineServiceContractRoutes(
        MethodInfo actionMethodInfo, Type controllerType, Type interfaceType)
        {
            InterfaceMapping map = controllerType.GetInterfaceMap(interfaceType);

            // locate matching interface method info and extract its DirectRoute custom attribute
            return map.InterfaceMethods
                .Where((t, i) => map.TargetMethods[i] == actionMethodInfo)
                .Select(t => t.GetCustomAttributes(typeof(IDirectRouteFactory), true).OfType<IDirectRouteFactory>())
                .FirstOrDefault();
        }

        private Type GetBeelineServiceContract(Type controllerType)
        {
            return controllerType.GetInterfaces().Single(serviceContract => Attribute.IsDefined(serviceContract, typeof(BeelineHttpServiceAttribute)));
        }
    }
}