using BetHelper.Data;
using BetHelper.Models;
using BetHelper.Web.Models.Players;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Text.RegularExpressions;
namespace DemoRight
{
    public class MainLineUpParser
    {
        private WebClient webClient;
        private string matchId;
        private BetHelperData dat;
        private string currentMatchURI;
        public HtmlNode mainContainer;
        HtmlNode mainHomeNode;
        List<string> homeTeamCurentGamePlayerIds;
        List<string> homeTeamCurentGameReservePlayerIds;
        List<string> allHomePlayersNames;
        List<string> allAwayPlayersNames;
        List<string> allHomePlayersIds;
        List<string> allHomePlayersEmptyActivityIds;
        List<string> newHomePlayersIds;
        string[] currentHomeOldPlayersIds;
        Game currentGame;
        List<string> newHomePlayersReserveIds;
        string homeTeamId;
        HtmlNode mainAwayNode;
        List<string> awayTeamCurentGamePlayerIds;
        List<string> awayTeamCurentGameReservePlayerIds;
        List<string> allAwayPlayersIds;
        List<string> allAwayPlayersEmptyActivityIds;
        List<string> newAwayPlayersIds;
        string[] currentAwayOldPlayersIds;
        List<string> newAwayPlayersReserveIds;
        string awayTeamId;
        PlayersNewEntryPushModel[] awayTeamCurentNewPlayers;
        PlayersNewEntryPushModel[] homeTeamCurentNewPlayers;
        SignaRPushModel[] homeTeamCurentOldPlayers;
        SignaRPushModel[] awayTeamCurentOldPlayers;

        IEnumerable<Player> homePlayers;
        IEnumerable<Player> awayPlayers;
        public MainLineUpParser(HtmlNode mainContainer, BetHelperData dat, string matchId)
            : this(null, matchId, dat, null, mainContainer)
        {
            // this.mainContainer = mainContainer;
        }
        public MainLineUpParser(WebClient wc, string matchId, BetHelperData dat, string currentMatchURI, HtmlNode mainContainer)
        {
            this.webClient = wc;
            this.matchId = matchId;
            this.dat = dat;
            this.currentGame = this.dat.Games.Find(this.matchId);
            this.currentMatchURI = currentMatchURI;
            this.mainContainer = mainContainer;
            this.mainHomeNode = mainContainer.SelectSingleNode("//div[@class='home-team']");
            this.mainAwayNode = this.mainContainer.SelectSingleNode("//div[@class='away-team']");
            this.homeTeamId = getHomeTeamId();
            this.awayTeamId = getAwayTeamId();
            this.homeTeamCurentGamePlayerIds = this.mainHomeNode.SelectSingleNode("//ul[@class='player-list']")
                     .Descendants()
                     .Where(x => x.Attributes["id"] != null).Select(y => y.GetAttributeValue("id", "000")).ToList();

            this.homeTeamCurentGameReservePlayerIds = this.mainHomeNode.SelectSingleNode("//ul[@class='subs-list']")
                                .Descendants()
                                .Where(x => x.Attributes["id"] != null).Select(y => y.GetAttributeValue("id", "000")).ToList();
            //change vsi4ki koito imat aktiviti za kluba
            //all players of both teams ever including not appeared at the moment reserves
            var allPlayers = this.dat.Players.All().Where(x => x.TeamFirstAppearances.FirstOrDefault(y => y.TeamName == this.homeTeamId) != null
               || x.TeamName == this.homeTeamId
                || x.TeamFirstAppearances.FirstOrDefault(y => y.TeamName == this.awayTeamId) != null
                || x.TeamName == this.awayTeamId).Include(f => f.TeamFirstAppearances).ToList();


            this.homePlayers = allPlayers.Where(x => x.TeamFirstAppearances.FirstOrDefault(y => y.TeamName == this.homeTeamId) != null || x.TeamName == this.homeTeamId);
            this.awayPlayers = allPlayers.Where(x => x.TeamFirstAppearances.FirstOrDefault(y => y.TeamName == this.awayTeamId) != null || x.TeamName == this.awayTeamId);

            this.allHomePlayersNames = this.homePlayers.Select(x => x.PlayerName).ToList();
            this.allAwayPlayersNames = this.awayPlayers.Select(x => x.PlayerName).ToList();
            this.allHomePlayersIds = this.homePlayers.Select(x => x.PlayerId).ToList();
            //tova mai ne mi trqbva-predi bqha tezi na koito da se vyvegdat prazni aktivitita sega sa vyvedeni!!!!!
            // this.allHomePlayersEmptyActivityIds= this.allHomePlayersIds.Except(this.homeTeamCurentGamePlayerIds).Except(this.homeTeamCurentGameReservePlayerIds).ToList();
            //tezi verno sa novopostypilite
            this.newHomePlayersIds = this.homeTeamCurentGamePlayerIds.Except(this.allHomePlayersIds).ToList();
            this.currentHomeOldPlayersIds = this.homeTeamCurentGamePlayerIds.Intersect(this.allHomePlayersIds).ToArray();
            this.newHomePlayersReserveIds = this.homeTeamCurentGameReservePlayerIds.Except(this.allHomePlayersIds).ToList();

            this.awayTeamCurentGamePlayerIds = this.mainAwayNode.Descendants().First(x => x.Attributes["class"] != null &&
        x.Attributes["class"].Value == "player-list").Descendants().Where(x => x.Attributes["id"] != null)
        .Select(y => y.GetAttributeValue("id", "000")).ToList();
            this.awayTeamCurentGameReservePlayerIds = this.mainAwayNode.Descendants().First(x => x.Attributes["class"] != null &&
                    x.Attributes["class"].Value == "subs-list").Descendants().Where(x => x.Attributes["id"] != null)
                    .Select(y => y.GetAttributeValue("id", "000")).ToList();

            this.allAwayPlayersIds = this.awayPlayers.Select(x => x.PlayerId).ToList();
            // this.allAwayPlayersEmptyActivityIds = this.allAwayPlayersIds.Except(this.awayTeamCurentGamePlayerIds).Except(this.awayTeamCurentGameReservePlayerIds).ToList();
            this.newAwayPlayersIds = this.awayTeamCurentGamePlayerIds.Except(this.allAwayPlayersIds).ToList();
            this.currentAwayOldPlayersIds = this.awayTeamCurentGamePlayerIds.Intersect(this.allAwayPlayersIds).ToArray();
            this.newAwayPlayersReserveIds = this.awayTeamCurentGameReservePlayerIds.Except(this.allAwayPlayersIds).ToList();
            this.homeTeamCurentNewPlayers = getHomeNewPlayers();
            this.awayTeamCurentNewPlayers = getAwayNewPlayers();
            this.homeTeamCurentOldPlayers = getHomeOldPlayers();
            this.awayTeamCurentOldPlayers = getAwayOldPlayers(); ;

        }
        private SignaRPushModel[] getHomeOldPlayers()
        {

            var lastMileStone = this.dat.Teams.Find(this.homeTeamId).TeamGameProfiles.OrderBy(x => x.Game.GameDate).LastOrDefault(x => x.IsMileStone);
            Console.WriteLine("Milestone {0} {1}", this.homeTeamId, lastMileStone);
            var milestoneTitActivities = lastMileStone.Activities.Where(x => Regex.IsMatch(x.MinutesPlayes, "^[t0-9]{1,2}$"));
            List<SignaRPushModel> holder = new List<SignaRPushModel>();
            foreach (var item in milestoneTitActivities)
            {
                var signalProfile = new SignaRPushModel()
                {
                    playerId = item.PlayerId
                };
                if (this.currentHomeOldPlayersIds.Contains(item.PlayerId))
                {
                    signalProfile.minutesPlayes = "t";
                }
                else
                {
                    signalProfile.minutesPlayes = "KM";
                }
                holder.Add(signalProfile);
            }

            return holder.ToArray();

        }
        private SignaRPushModel[] getAwayOldPlayers()
        {

            var lastMileStone = this.dat.Teams.Find(this.awayTeamId).TeamGameProfiles.OrderBy(x => x.Game.GameDate).LastOrDefault(x => x.IsMileStone);
            var milestoneTitActivities = lastMileStone.Activities.Where(x => Regex.IsMatch(x.MinutesPlayes, "^[t0-9]{1,2}$"));
            List<SignaRPushModel> holder = new List<SignaRPushModel>();
            foreach (var item in milestoneTitActivities)
            {
                var signalProfile = new SignaRPushModel()
                {
                    playerId = item.PlayerId
                };
                if (this.currentAwayOldPlayersIds.Contains(item.PlayerId))
                {
                    signalProfile.minutesPlayes = "t";
                }
                else
                {
                    signalProfile.minutesPlayes = "KM";
                }
                holder.Add(signalProfile);
            }

            return holder.ToArray();

        }

        private PlayersNewEntryPushModel[] getHomeNewPlayers()
        {

            var result = this.mainHomeNode.Descendants().First(x => x.Attributes["class"] != null &&
    x.Attributes["class"].Value == "player-list").Descendants().Where(x => x.Attributes["id"] != null && this.newHomePlayersIds.Contains(x.Attributes["id"].Value))
    .Select(y => new PlayersNewEntryPushModel(this.ReturnProfilesCount(this.homeTeamId))
    {
        currentMinPlayes = "t",
        playerName = ParsedNameArray(y.InnerText)[1],
        playerNumber = ParsedNameArray(y.InnerText)[0],


    }).ToArray();
            foreach (var item in result)
            {
                FillInEmptyActivities(item.activities);
            }
            return result;
        }
        private PlayersNewEntryPushModel[] getAwayNewPlayers()
        {
            var result = this.mainAwayNode.Descendants().First(x => x.Attributes["class"] != null &&
        x.Attributes["class"].Value == "player-list").Descendants().Where(x => x.Attributes["id"] != null && this.newAwayPlayersIds.Contains(x.Attributes["id"].Value))
        .Select(y => new PlayersNewEntryPushModel(this.ReturnProfilesCount(this.awayTeamId))
        {
            currentMinPlayes = "t",
            playerName = ParsedNameArray(y.InnerText)[1],
            playerNumber = ParsedNameArray(y.InnerText)[0],


        }).ToArray();
            foreach (var item in result)
            {
                FillInEmptyActivities(item.activities);
            }
            return result;
        }
        private string getAwayTeamId()
        {
            var nodeTeamName = this.mainAwayNode.Descendants().Where(x => x.Attributes["class"] != null &&
                      x.Attributes["class"].Value == "team-name").ToList()[0].InnerText;

            string teamName = null;

            if (nodeTeamName != "Bury")
            {
                teamName = this.dat.Teams.All().First(x => x.Name.Contains(nodeTeamName)).Name;
            }
            else
            {
                teamName = "Bury";
            }
            return teamName;
        }
        private string getHomeTeamId()
        {

            var nodeTeamName = this.mainHomeNode.SelectSingleNode("//h3[@class='team-name']").InnerText;
            string teamName = null;

            if (nodeTeamName != "Bury")
            {
                var ggg = this.dat.Teams.All().First(x => x.Name.Contains(nodeTeamName));
                teamName = ggg.Name;
            }
            else
            {
                teamName = "Bury";
            }
            return teamName;
        }
        public Game CurrentGame
        {
            get
            {
                return this.currentGame;
            }
        }
        public HtmlNode MainHomeNode
        {
            get
            {


                return this.mainHomeNode;
            }
        }
        public HtmlNode MainAwayNode
        {
            get
            {
                return this.mainAwayNode;
            }
        }
        public string HomeTeamId
        {
            get
            {

                return this.homeTeamId;
            }
        }
        public string AwayTeamId
        {
            get
            {

                return this.awayTeamId;
            }
        }
        public string HomeTeamNickName
        {
            get
            {

                var nickName = this.dat.Teams.Find(this.HomeTeamId).NickName;


                return nickName;
            }
        }
        public string AwayTeamNickName
        {

            get
            {

                var nickName = this.dat.Teams.Find(this.AwayTeamId).NickName;


                return nickName;
            }

        }

        //ALL Current GAme TITULAR PLAYERS IDS
        public List<string> HomeTeamCurentGamePlayerIds
        {
            get
            {
                return this.homeTeamCurentGamePlayerIds;
            }
        }
        //Reserve Players Current Game Ids
        public List<string> HomeTeamCurentGameReservePlayerIds
        {
            get
            {
                return this.homeTeamCurentGameReservePlayerIds;
            }
        }
        //ALL HOME PLAYER IDS ever played for the team
        public IList<string> AllHomePlayersIds
        {
            get
            {

                return this.allHomePlayersIds;
            }
        }

        public IList<string> NewHomePlayersIds
        {
            get
            {
                return this.newHomePlayersIds;
            }
        }
        public SignaRPushModel[] HomeTeamCurentOldPlayers
        {
            get
            {
                return this.homeTeamCurentOldPlayers;
            }
        }
        public IList<string> NewHomePlayersReserveIds
        {
            get
            {
                return this.newHomePlayersReserveIds;
            }
        }

        //tezi dvete gi izpolzvam v ParseLineup za Possible duplictae
        public IList<string> AwayTeamPlayersNames
        {
            get
            {
                return this.allAwayPlayersNames;
            }
        }
        public IList<string> HomeTeamPlayersNames
        {
            get
            {
                return this.allHomePlayersNames;
            }
        }

        //ALL Current GAme AwayE TITULAR PLAYERS IDS
        public IList<string> AwayTeamCurentGamePlayerIds
        {
            get
            {
                return this.awayTeamCurentGamePlayerIds;
            }
        }

        public IList<string> AwayTeamCurentGameReservePlayerIds
        {
            get
            {
                return this.awayTeamCurentGameReservePlayerIds;
            }
        }


        public IList<string> AllAwayPlayersIds
        {
            get
            {
                return this.allAwayPlayersIds;
            }
        }



        public IList<string> NewAwayPlayersIds
        {
            get
            {
                return this.newAwayPlayersIds;
            }
        }


        public SignaRPushModel[] AwayTeamCurentOldPlayers
        {
            get
            {
                return this.awayTeamCurentOldPlayers;
            }
        }

        public IList<string> NewAwayPlayersReserveIds
        {
            get
            {
                return this.newAwayPlayersReserveIds;
            }
        }



        //TODO:da go izmislq kato pole v bazata!!!!!!
        //FOR empty activities of the new players
        public int ReturnProfilesCount(string teamId)
        {
            var todayDateOnly = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 1, 0, 0);
            var result = this.dat.TeamGameProfiles.All().Where(x => x.TeamName == teamId && x.Game.GameDate < todayDateOnly).Count();
            return result;
        }

        private int GetNumber(string innerText)
        {
            string[] splitters = { " ", "  ", "." };
            string[] splitedValues = innerText.Trim().Split(splitters, StringSplitOptions.RemoveEmptyEntries);
            return Convert.ToInt32(splitedValues[0]);
        }
        private List<IdNumber> SetIdNumbers()
        {
            List<IdNumber> list = new List<IdNumber>();
            var homeNodes = this.MainHomeNode.SelectSingleNode("//ul[@class='player-list']")
                    .Descendants()
                    .Where(x => x.Attributes["id"] != null);
            var awayNodes = this.MainAwayNode.Descendants().First(x => x.Attributes["class"] != null &&
        x.Attributes["class"].Value == "player-list").Descendants().Where(x => x.Attributes["id"] != null);
            foreach (var item in homeNodes)
            {
                IdNumber curNum = new IdNumber();
                curNum.PlayerId = item.GetAttributeValue("id", "000");
                curNum.Number = GetNumber(item.InnerText);
                list.Add(curNum);
            }
            foreach (var item in awayNodes)
            {
                IdNumber curNum = new IdNumber();
                curNum.PlayerId = item.GetAttributeValue("id", "000");
                curNum.Number = GetNumber(item.InnerText);
                list.Add(curNum);
            }
            return list;
        }
        public List<IdNumber> AllTeamCurentGameIdNumbersObjects
        {
            get
            {
                return SetIdNumbers();
            }
        }
        //TODO: Tuka i na Away titulqrite da oproverqvame za Datefirstappearance s tozi tim i ako enull
        //,setvame teku6tia ma4-tova zna4i 4e do momenta e bil rezerva i da dobavqme i v DateFirstApperance zapis!!!!



        public string[] ParsedNameArray(string nameAndNumber)
        {
            string[] splitters = { " ", "  " };
            string[] splitedValues = nameAndNumber.Trim().Split(splitters, StringSplitOptions.RemoveEmptyEntries);
            return splitedValues;
        }

        //With this we fullfill empty array for empty acticitie on SugnalR pushmodel-FOR NEW PLAYERS ONLY-not for DB use
        public void FillInEmptyActivities(EmptyActivity[] activitiesArray)
        {

            for (int i = 0; i < activitiesArray.Length; i++)
            {
                activitiesArray[i] = new EmptyActivity();

            }


        }
        //Return push model to SignalR i ServerCache 
        public PlayersNewEntryPushModel[] HomeTeamCurentNewPlayers
        {
            get
            {

                return this.homeTeamCurentNewPlayers;
            }
        }
        //TUKA MOGE i BEZ new empty Activity
        public PlayersNewEntryPushModel[] AwayTeamCurentNewPlayers
        {
            get
            {
                return this.awayTeamCurentNewPlayers;
            }
        }

        //Select Players by DateFirstAppearance(player on loan returning and player has only bench appearance at the moment




        public string[] NameAndNumbers
        {
            get
            {
                return new string[] { };
            }
        }






        string[] nameAndNumbers = new string[] { };
    }
}
