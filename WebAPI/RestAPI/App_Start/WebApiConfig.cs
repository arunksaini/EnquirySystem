using System.Web.Http;

namespace RestAPI
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            config.EnableCors();
            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                "DefaultApi",
                "api/v1/{controller}/{action}/{id}",
                new {id = RouteParameter.Optional}
            );
        }
    }
}