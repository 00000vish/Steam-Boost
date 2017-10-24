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
            if (args.Length == 0)
            {
                MessageBox.Show("Run the main program", "Opps...", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (args[0] == "gamelist")
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Form2());              
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

        //public profile
        public static List<Game> GetGames()
        {
            ulong steamId = SteamUser.GetSteamID().m_SteamID;
            try
            {
                var apiJson = new StreamReader(
                    WebRequest.Create(
                    "http://api.steampowered.com/IPlayerService/GetOwnedGames/v0001/?key=006C1D814005AF1CAE4B670EE4B38979&steamid=" + steamId + "&l=english&json")
                    .GetResponse().GetResponseStream()).ReadToEnd();
                var gamesList = JObject.Parse(apiJson)["response"]["games"].Children().Select(current => current.SelectToken("appid").ToString()).ToList();
                var list = (from game in gamesList
                            let json = JObject.Parse(new StreamReader(WebRequest.Create("http://store.steampowered.com/api/appdetails?appids=" + game).GetResponse().GetResponseStream()).ReadToEnd())
                            where json[game]["success"].Value<bool>()
                            select new Game(gamesList.Count > 500)
                            {
                                Name = json[game]["data"]["name"].Value<string>(),
                                ID = Convert.ToUInt64(game)
                            }).ToList();
                return list;
            }
            catch (Exception e) { MessageBox.Show("There was an error getting games from steam, if your profile is private please make it public before using this tool."); Application.Exit(); return null; }            
        }
    }
}

public class Game
{
    public Game(bool large) { if(large) System.Threading.Thread.Sleep(1050); System.Threading.Thread.Sleep(500); }
    public string Name { get; set; }
    public ulong ID { get; set; }
}