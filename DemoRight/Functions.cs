using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using BetHelper.Data;
using BetHelper.Models;
using System.Net.Http;
using HtmlAgilityPack;
using System.Net;
using BetHelper.Web.Models.Games;


       

namespace DemoRight
{
    public class Functions
    {
        const string currentMatchPoolingLineUpString = "web link to look for the game data";
        // This function will get triggered/executed when a new message is written 
        // on an Azure Queue called queue.
        public static void ProcessQueueMessage([QueueTrigger("thumbnailrequest")] string message, TextWriter log)
        {
            log.WriteLine("Function is invoked with value={0}", message);
            log.WriteLine("Following message will be written on the Queue={0}", message);
             DemoLineupMethod();
        }
        private static void DemoLineupMethod()
        {
            BetHelperData dat = new BetHelperData();
            var startDateValue = new DateTime(2015, 4, 6, 3, 0, 0);
            var endDateValue = new DateTime(DateTime.Now.Year, 4, 7, 23, 0, 0);
            var games = dat.Games.All().Where(x => x.GameDate > startDateValue
                 && x.GameDate < endDateValue).Take(6).ToList();
            ParseLineupDemo(games);
        }
        //for each game check if info is already available, if it is push info to application server,remove the game from games list,start new cycle with those whicn left 
        private static async void ParseLineupDemo(List<Game> games)
        {
            BetHelperData dat = new BetHelperData();
            byte[][] listAll;
            List<Task<byte[]>> tasksList = new List<Task<byte[]>>();
            using (var client = new HttpClient())
            {
                foreach (var item in games)
                {
                    Task<byte[]> task = client.GetByteArrayAsync(currentMatchPoolingLineUpString + item.GameId);
                    tasksList.Add(task);
                }
                listAll = await Task.WhenAll(tasksList);
            }
            for (int i = 0; i < listAll.Length; i++)
            {
                if (listAll[i].Length < 1000)
                {
                    continue;
                }
                else
                {
                    var htmlDociment = LoadCurrentHtmlDocument(listAll[i]);

                    var currentParser = ReturnCurrentPartLineUpParser(htmlDociment, games);
                    PushDataToSignalR(currentParser);
                    games.RemoveAll(x => x.HostName == currentParser.HomeTeamId);
                }
            }
        }
        private static void PushDataToSignalR(MainLineUpParser currentParser)
        {
            using (var client = new HttpClient())
            {
                var jObject = new GamePushingProfile()
                {
                    GameId = currentParser.CurrentGame.GameId,
                    CurrentPassedHomeOldPlayers = currentParser.HomeTeamCurentOldPlayers,
                    CurrentPassedAwayOldPlayers = currentParser.AwayTeamCurentOldPlayers,
                    CurrentPassedAwayNewPlayers = currentParser.AwayTeamCurentNewPlayers,
                    CurrentPassedHomeNewPlayers = currentParser.HomeTeamCurentNewPlayers,
                };
                client.PostAsJsonAsync("http://bethelpertrial.azurewebsites.net/api/Games/PostSignalRData", jObject).Result.Content.ReadAsStringAsync();
            }
        }
        //get game data as byte array
        private static HtmlDocument LoadCurrentHtmlDocument(Byte[] responseData)
        {
            String source = Encoding.GetEncoding("utf-8").GetString(responseData, 0, responseData.Length - 1);
            source = WebUtility.HtmlDecode(source);
            HtmlDocument documentResult = new HtmlDocument();
            documentResult.LoadHtml(source);
            return documentResult;
        }
        private static MainLineUpParser ReturnCurrentPartLineUpParser(HtmlDocument document, List<Game> games)
        {


            BetHelperData dat = new BetHelperData();
            var mainContainer = document.GetElementbyId("oppm-team-list");
            var mainHomeNode = mainContainer.SelectSingleNode("//div[@class='home-team']");
            var nodeTeamName = mainHomeNode.SelectSingleNode("//h3[@class='team-name']").InnerText;

            var teamName = dat.Teams.All().First(x => x.Name.Contains(nodeTeamName)).Name;

            string gameId = games.Find(x => x.HostName == teamName).GameId;
            var currentParser = new MainLineUpParser(mainContainer, dat, gameId);

            return currentParser;
        }       
    }
}
