using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace ToDoAPI.API
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            // We add the following line of code to create a global behavior in the app to allow Cross Origin Resource Sharing (CORS) across other applications. This basically authoriuzes which apps can get data from this API
            config.EnableCors();
            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
