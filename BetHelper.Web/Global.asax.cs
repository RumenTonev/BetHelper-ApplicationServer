namespace BetHelper.Web
{
    using Microsoft.WindowsAzure;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Blob;
    using Microsoft.WindowsAzure.Storage.Queue;
    using Microsoft.WindowsAzure.Storage.RetryPolicies;
    using Microsoft.AspNet.SignalR;
    using System;
    using System.Data.Entity;
    using System.Web.Http;
    using System.Web.Mvc;
    using System.Web.Optimization;
    using System.Web.Routing;
    using BetHelper.Data;
    using BetHelper.Data.Migrations;
    using System.Collections.Generic;
    using System.Linq;
    using BetHelper.Web.Models.Games;
    using System.Web.Caching;
    using Microsoft.Owin.Cors;
    using BetHelper.Models;
    using System.Diagnostics;
    using System.Configuration;
  

    
    

    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {

            Database.SetInitializer(new MigrateDatabaseToLatestVersion<BetHelperContext, BetHelper.Data.Migrations.Configuration>());
            BetHelperContext context = new BetHelperContext();
            BetHelperData data = new BetHelperData(context);
            var startDateValue = new DateTime(2015, 4, 6, 3, 0, 0);
            var endDateValue = new DateTime(DateTime.Now.Year, 4, 7, 23, 0, 0);
            //var startDateValue = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 3, 0, 0);
            //var endDateValue = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 0, 0);

            var games = data.Games.All().Where(x => x.GameDate > startDateValue
                 && x.GameDate < endDateValue).ToList().Select(x => new SignleGameFullViewModel(x)).Take(6).ToArray();
            if (games != null)
            {


                System.Web.HttpContext.Current.Cache.Add("currentGames", games, null, Cache.NoAbsoluteExpiration,
                    Cache.NoSlidingExpiration, CacheItemPriority.NotRemovable, null);
            }
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            InitializeStorage();

        }
        private void InitializeStorage()
        {
            // Open storage account using credentials from .cscfg file.
            var storageAccount = CloudStorageAccount.Parse
                (ConfigurationManager.ConnectionStrings["AzureWebJobsStorage"].ToString());



            Trace.TraceInformation("Creating queues");
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
            var blobnameQueue = queueClient.GetQueueReference("thumbnailrequest");
            blobnameQueue.CreateIfNotExists();

            Trace.TraceInformation("Storage initialized");
        }
    }
}
