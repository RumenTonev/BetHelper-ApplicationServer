namespace BetHelper.Web
{
    using System.Web.Http;

    using Microsoft.Owin.Security.OAuth;

    using Newtonsoft.Json.Serialization;
    using System.Web.Http.Cors;

    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            // Configure Web API to use only bearer token authentication.
            config.SuppressDefaultHostAuthentication();
            config.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));
            config.EnableCors();
            // Web API routes
            config.MapHttpAttributeRoutes();
          
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional });
            //config.Routes.MapHttpRoute(
             //name: "MyActionApi",
             //routeTemplate: "api/{controller}/{action}/{id}"
             //);

            var jsonFormatter = GlobalConfiguration.Configuration.Formatters.JsonFormatter;
            jsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
        }
    }
}
