using Microsoft.Owin;

[assembly: OwinStartup(typeof(BetHelper.Web.Startup))]

namespace BetHelper.Web
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Cors;
    using System.Web.Http;

    using Microsoft.Owin;
    using Microsoft.Owin.Cors;

    using Owin;
    using Microsoft.AspNet.SignalR;

    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
           
            app.UseCors(new CorsOptions()
            {
                PolicyProvider = new CorsPolicyProvider()
                {
                    PolicyResolver = request =>
                    {
                        if (request.Path.StartsWithSegments(new PathString(TokenEndpointPath)))
                        {
                            return Task.FromResult(new CorsPolicy { AllowAnyOrigin = true });
                        }

                        return Task.FromResult<CorsPolicy>(null);
                    }
                }
            });
            
            ConfigureAuth(app);

            GlobalConfiguration.Configuration.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;
            app.Map("/signalr", map =>
            {
                // Setup the cors middleware to run before SignalR.
                // By default this will allow all origins. You can 
                // configure the set of origins and/or http verbs by
                // providing a cors options with a different policy.
                map.UseCors(CorsOptions.AllowAll);

                var hubConfiguration = new HubConfiguration
                {
                    EnableJavaScriptProxies = true
                    // You can enable JSONP by uncommenting line below.
                    // JSONP requests are insecure but some older browsers (and some
                    // versions of IE) require JSONP to work cross domain
                    // EnableJSONP = true
                };

                // Run the SignalR pipeline. We're not using MapSignalR
                // since this branch is already runs under the "/signalr"
                // path.
                map.RunSignalR(hubConfiguration);
            });
           
            //this.ConfigureAuth(app);
           // app.MapSignalR();
        }
    }
}
