using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Routing;
using System.Web.Routing;

namespace Service
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {            
            ControllerBuilder.Current.SetControllerFactory(typeof(BeelineServiceControllerFactory));
            GlobalFilters.Filters.Add(new HandleErrorAttribute());
            RouteTable.Routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            RouteTable.Routes.MapMvcAttributeRoutes(new BeelineServiceRouteProvider());            
        }
    }
}
