using BetHelper.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.AspNet.SignalR;
using BetHelper.Web.Hubs;
using BetHelper.Web.Models.Games;
using System.Web.Caching;
using BetHelper.Web.Models.Players;
using BetHelper.Models;
using System.Web.Http.Cors;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage;
using System.Configuration;
using System.Diagnostics;
namespace BetHelper.Web.Controllers
{
    public class GamesController : BaseApiController
    {
        private CloudQueue thumbnailRequestQueue;
       
    public GamesController()
            : this(new BetHelperData())
        {
            InitializeStorage();
        }

     public GamesController(IBetHelperData data)
            : base(data)
        {
        }

     private void InitializeStorage()
     {
         // Open storage account using credentials from .cscfg file.
         var storageAccount = CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings["AzureWebJobsStorage"].ToString());

         // Get context object for working with blobs, and 
         // set a default retry policy appropriate for a web user interface.
         //var blobClient = storageAccount.CreateCloudBlobClient();
         //blobClient.DefaultRequestOptions.RetryPolicy = new LinearRetry(TimeSpan.FromSeconds(3), 3);

         // Get a reference to the blob container.
         //imagesBlobContainer = blobClient.GetContainerReference("images");

         // Get context object for working with queues, and 
         // set a default retry policy appropriate for a web user interface.
         CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
         //queueClient.DefaultRequestOptions.RetryPolicy = new LinearRetry(TimeSpan.FromSeconds(3), 3);

         // Get a reference to the queue.
         thumbnailRequestQueue = queueClient.GetQueueReference("thumbnailrequest");
     }
        [HttpPost]
     public IHttpActionResult Demo()
     {
         var queueMessage = new CloudQueueMessage("demo rumen");
         thumbnailRequestQueue.AddMessageAsync(queueMessage);
         Trace.TraceInformation("Created queue message for AdId {0}", "demo rumen");
         return this.Ok();
     }
       //Main Model-all games for today
        [HttpGet]
        //[CacheOutput(ClientTimeSpan = 0, ServerTimeSpan = 1000)]
        public IHttpActionResult GetAllForTodayFullModel()
        {
            if (System.Web.HttpContext.Current.Cache["currentGames"] == null)
            { 
                var startDateValue = new DateTime(2015, 4, 6, 3, 0, 0);
                var endDateValue = new DateTime(DateTime.Now.Year, 4, 7, 23, 0, 0);

                var games = this.Data.Games.All().Where(x => x.GameDate > startDateValue
                     && x.GameDate < endDateValue).ToList().Select(x => new SignleGameFullViewModel(x)).Take(6).ToArray();
                if (games == null)
                {
                    return this.BadRequest("The requested driver was not found in users or is not marked as a driver.");
                }
                System.Web.HttpContext.Current.Cache.Add("currentGames", games, null, Cache.NoAbsoluteExpiration,
                    Cache.NoSlidingExpiration, CacheItemPriority.NotRemovable, null);
                return this.Ok(games);
            }
            else
            {
                return this.Ok(System.Web.HttpContext.Current.Cache.Get("currentGames"));
            } 
        }
        //used from SignalR to update server cache values real-time- for consistency
        [HttpPost]       
        public IHttpActionResult PushLastGameNewActivities(GamePushingProfile gamePushingProfile)
        {
            //GamePushingProfile
            if (System.Web.HttpContext.Current.Cache["currentGames"] != null)
            {
             
                SignleGameFullViewModel[] currentGames = System.Web.HttpContext.Current.Cache["currentGames"] as SignleGameFullViewModel[];
                var currentGame = currentGames.FirstOrDefault(x => x.GameId == gamePushingProfile.GameId);
        
                currentGame.Status = GameStatus.NowPlayed.ToString();

                //NEW Model FILL IN Empty Activities commented out-since we create it in the line up parser property
                currentGame.HostModel.NewPlayers = gamePushingProfile.CurrentPassedHomeNewPlayers;
            
                currentGame.GuestModel.NewPlayers = gamePushingProfile.CurrentPassedAwayNewPlayers;
                foreach (var item in currentGame.HostModel.Players)
                {
                    var currentProfile = gamePushingProfile.CurrentPassedHomeOldPlayers.FirstOrDefault(x => x.playerId == item.PlayerId);
                    if(currentProfile!=null)
                   {
                       item.CurrentActivity.MinutesPlayes = currentProfile.minutesPlayes;
                       
                    }
                }
                foreach (var item in currentGame.GuestModel.Players)
                {
                    var currentProfile = gamePushingProfile.CurrentPassedAwayOldPlayers.FirstOrDefault(x => x.playerId == item.PlayerId);
                    if (currentProfile != null)
                    {
                        item.CurrentActivity.MinutesPlayes = currentProfile.minutesPlayes;

                    }
                }
             
              
                System.Web.HttpContext.Current.Cache.Insert("currentGames", currentGames);
              

                return this.Ok(currentGames);
            }
            else
            {
               return this.BadRequest("Invalid profile value.");
            }      
        }

        [HttpGet]
        
        public IHttpActionResult GetSingle(string id)
        {
            var game = this.Data.Games.Find(id);
            


            if (game == null)
            {
                return this.BadRequest("The requested driver was not found in users or is not marked as a driver.");
            }
            var resultModel = new GameSingleViewModel(game);

            return this.Ok(resultModel);
        }
        public HttpResponseMessage PostSignalRData(GamePushingProfile gameObject)
        {
            
            CurrentGameInfoHub.PushCurrentGameProfile(gameObject);
            var response = Request.CreateResponse<GamePushingProfile>(HttpStatusCode.OK, gameObject);
            string uri = Url.Link("DefaultApi", new { id = gameObject.GameId });
            response.Headers.Location = new Uri(uri);
            return response;
        }
    }
}
