

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaskScheduler;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

using System.Threading.Tasks;
using HtmlAgilityPack;
using BetHelper.Web.Models.Players;
using System.Diagnostics;
using System.Data.Entity;
using BetHelper.Web.Models.Games;
using BetHelper.Data;
using BetHelper.Models;
using BetHelper.Engine;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using BetHelper.Data.Migrations;

namespace TestClient
{
    /*Using Scheduler every day in the morning get all games and sets 40 minuti before game start to begin polling for data. When data is received it is  pushed to 1.DB
     * 2.Server Cache 3.Client thus garantee consistency in case of failure
     */
    public class TestClient
    {
        const string currentMatchPoolingLineUpString = "http string url to seek for game data";
        private static ScheduledTaskManager m = new ScheduledTaskManager();

        static void Main(string[] args)
         {

         
         BetHelperData dat = new BetHelperData();
        WebClient wc= new WebClient();
        DateTime newDateT = DateTime.Now.AddSeconds(10);
        var mainTask = m.RegisterAbsoluteTask(() =>
           setMainTask(newDateT), newDateT.Hour, newDateT.Minute, true, newDateT.Second);
        Console.ReadLine();
         }
        //MainMethod-called in serviceStart in case of Web Worker imeplementation fires every day in 10 am check for today's game and set further  tasks
        public static void mainStartLineupMethod()
        {
            //every day  10 am
            DateTime newDateT = DateTime.Now.AddMinutes(1);
            var restask = m.RegisterAbsoluteTask(() =>
                setMainTask(newDateT), 10, 0, true);
            //in case of restart
            if (DateTime.Now > new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 10, 0, 0))
            {
                TaskScheduler.ScheduledTaskManager.ScheduledTask restaskInn = null;
                DateTime actualStart = DateTime.Now.AddSeconds(15);
                Console.WriteLine("ParseSetted");
                restaskInn = m.RegisterAbsoluteTask(() =>
                       setMainTask(newDateT, restaskInn), actualStart.Hour, actualStart.Minute, true, actualStart.Second);
            };
        
            
        }
        private static void setMainTask(DateTime newDateT, TaskScheduler.ScheduledTaskManager.ScheduledTask taskp = null)
        {
            BetHelperData dat = new BetHelperData();     
            var startDateValue = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 3, 0, 0);
            var endDateValue = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 0, 0);
            var games = dat.Games.All().Where(x => x.GameDate > startDateValue
              && x.GameDate < endDateValue && x.GameStatus == GameStatus.Fixture).ToList();
            TaskScheduler.ScheduledTaskManager.ScheduledTask ftask = null;
            TaskScheduler.ScheduledTaskManager.ScheduledTask ftaskDB = null;
            List<Game> listDB = new List<Game>(games);
            List<Game> list = new List<Game>(games);          
            //for demo purposes
            DateTime cecValue = DateTime.Now.AddSeconds(10);         
            ftask = m.RegisterAbsoluteTask(() => ParseLineup(list, ftask), cecValue.Hour, cecValue.Minute, true, cecValue.Second);
            ftaskDB = m.RegisterAbsoluteTask(() => ParseLineupDb(listDB, ftaskDB), cecValue.Hour, cecValue.Minute, true, cecValue.Second);
            

        }
      
        private static async void ParseLineup(List<Game> games,TaskScheduler.ScheduledTaskManager.ScheduledTask taskp)
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
                    PushDataToServerCache(currentParser);
                    PushDataToSignalR(currentParser);
                    games.RemoveAll(x => x.HostName == currentParser.HomeTeamId);
                }
            }
            m.DeleteTask(taskp);
            TaskScheduler.ScheduledTaskManager.ScheduledTask ftask2 = null;

            if (games.Count > 0 && DateTime.Now < (games[0].GameDate.AddMinutes(15)))
            {
                DateTime cecValue = DateTime.Now.AddSeconds(10);
                ftask2 = m.RegisterAbsoluteTask(() => ParseLineup(games, ftask2), cecValue.Hour, cecValue.Minute, true, cecValue.Second);
            }
            else
            {
                if (games.Count > 0)
                {
                    foreach (var item in games)
                    {
                        var game = dat.Games.Find(item.GameId);
                        if (game.GameStatus != GameStatus.PostPoned)
                        {

                            game.GameStatus = GameStatus.PostPoned;
                        }
                    }
                    dat.SaveChanges();
                }
            }

        }
        private static async void ParseLineupCache(List<Game> games1, TaskScheduler.ScheduledTaskManager.ScheduledTask taskp)
        {
            

            BetHelperData dat = new BetHelperData();
            byte[][] listAll;
            List<Task<byte[]>> tasksList = new List<Task<byte[]>>();
            using (var client = new HttpClient())
            {
                foreach (var item in games1)
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
                    var currentParser = ReturnCurrentPartLineUpParser(htmlDociment, games1);

                    PushDataToServerCache(currentParser);
                    games1.RemoveAll(x => x.HostName == currentParser.HomeTeamId);
                }
            }
            m.DeleteTask(taskp);
            TaskScheduler.ScheduledTaskManager.ScheduledTask ftask2 = null;
            if (games1.Count > 0)
            {
                DateTime cecValue = DateTime.Now.AddSeconds(10);
                ftask2 = m.RegisterAbsoluteTask(() => ParseLineupCache(games1, ftask2), cecValue.Hour, cecValue.Minute, true, cecValue.Second);
            }
        }
        private static async void ParseLineupDb(List<Game> games, TaskScheduler.ScheduledTaskManager.ScheduledTask taskp)
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
                    LineUpParser.ParseLineUp(currentParser);
                    ChangeGameStatus(currentParser.CurrentGame, dat);
                    dat.SaveChanges();
                    Console.WriteLine("SavedtoDB {0}", currentParser.HomeTeamId);
                    games.RemoveAll(x => x.HostName == currentParser.HomeTeamId);
                }

                m.DeleteTask(taskp);
                TaskScheduler.ScheduledTaskManager.ScheduledTask ftask2 = null;
                if (games.Count > 0 && DateTime.Now < (games[0].GameDate.AddMinutes(15)))
                {
                    Console.WriteLine("LineUpDb games>0 time : {0}", DateTime.Now);
                    DateTime cecValue = DateTime.Now.AddSeconds(10);
                    ftask2 = m.RegisterAbsoluteTask(() => ParseLineupDb(games, ftask2), cecValue.Hour, cecValue.Minute, true, cecValue.Second);
                }
                else
                {
                    if (games.Count > 0)
                    {
                        foreach (var item in games)
                        {
                            var game = dat.Games.Find(item.GameId);
                            if (game.GameStatus != GameStatus.PostPoned)
                            {
                                game.GameStatus = GameStatus.PostPoned;
                            }
                        }
                        dat.SaveChanges();
                    }
                }
            }
        }
        private static void PushDataToServerCache(MainLineUpParser currentParser)
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
                //await client.PostAsJsonAsync("http://localhost:1337/api/Games/PushLastGameNewActivities", jObject);
                client.PostAsJsonAsync("http://localhost:1337/api/Games/PushLastGameNewActivities", jObject).Result.Content.ReadAsStringAsync();
                //  
            }
        }
        private static void PushDataToSignalR(MainLineUpParser currentParser)
        {
            BetHelperData dat = new BetHelperData();
            
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
                //await client.PostAsJsonAsync("http://localhost:1337/api/Games/PostSignalRData", jObject);
                client.PostAsJsonAsync("http://localhost:1337/api/Games/PostSignalRData", jObject).Result.Content.ReadAsStringAsync();
            }
        }
        private static void ChangeGameStatus(Game game, BetHelperData dat)
        {
            var gameObj = dat.Games.Find(game.GameId);
            gameObj.GameStatus = GameStatus.NowPlayed;
            dat.SaveChanges();
        }

        private static MainLineUpParser ReturnCurrentPartLineUpParser(HtmlDocument document, string gameId)
        {
            BetHelperData dat = new BetHelperData();
            var mainContainer = document.GetElementbyId("oppm-team-list");
            var mainHomeNode = mainContainer.SelectSingleNode("//div[@class='home-team']");
            var nodeTeamName = mainHomeNode.SelectSingleNode("//h3[@class='team-name']").InnerText;
            var currentParser = new MainLineUpParser(mainContainer, dat, gameId);
            return currentParser;
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
        private static HtmlDocument LoadCurrentHtmlDocument(Byte[] responseData)
        {
            String source = Encoding.GetEncoding("utf-8").GetString(responseData, 0, responseData.Length - 1);
            source = WebUtility.HtmlDecode(source);
            HtmlDocument documentResult = new HtmlDocument();
            documentResult.LoadHtml(source);
            return documentResult;
        }
    }
}

