# SharedMVCContract
Similar to WCF channel factory but for MVC services/clients.   

This shows how to generate a dynamic client proxy based on a shared service contract implemented by a .net MVC service.  The proxy is genereated using Castle dynamic proxy.   It reads route values from an interface implemented by an MVC service and generates HTTP requests to talk to the service using a strongly typed model

Dependencies:

ASP.NET MVC

Castle DynamicProxy

Newtonsoft Json.Net
