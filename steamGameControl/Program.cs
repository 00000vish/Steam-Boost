using Newtonsoft.Json.Linq;
using Steamworks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace steamGameControl
{
    static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            if (args[0] == "gamelist")
            {
                Environment.SetEnvironmentVariable("SteamAppId", "440");
                if (!SteamAPI.Init()) { }
                var games = GetGames();
                SteamAPI.Shutdown();
                string[] gameList = new string[games.Count];
                for (int i = 0; i < games.Count; i++)
                {
                    gameList[i] = games[i].ID + "`" + games[i].Name;
                }
                System.IO.File.WriteAllLines("game-list.txt", gameList);
            }
            else
            {
                Environment.SetEnvironmentVariable("SteamAppId", args[0]);
                if (!SteamAPI.Init())
                {
                    return;
                }
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Form1(args[0]));
            }

        }

        public static List<Game> GetGames()
        {
            ulong steamId = SteamUser.GetSteamID().m_SteamID;
            var apiJson = new StreamReader(
                WebRequest.Create(
                "http://api.steampowered.com/IPlayerService/GetOwnedGames/v0001/?key=006C1D814005AF1CAE4B670EE4B38979&steamid=" + steamId + "&l=english&json")
                .GetResponse().GetResponseStream()).ReadToEnd();
            var gamesList = JObject.Parse(apiJson)["response"]["games"].Children().Select(current => current.SelectToken("appid").ToString()).ToList();
            var list = (from game in gamesList
                        let json = JObject.Parse(new StreamReader(WebRequest.Create("http://store.steampowered.com/api/appdetails?appids=" + game).GetResponse().GetResponseStream()).ReadToEnd())
                        where json[game]["success"].Value<bool>()
                        select new Game()
                        {
                            Name = json[game]["data"]["name"].Value<string>(),
                            ID = Convert.ToUInt64(game)
                        }).ToList();
            return list;
        }
    }
}

public class Game
{
    public string Name { get; set; }
    public ulong ID { get; set; }
}