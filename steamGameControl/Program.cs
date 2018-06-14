using Newtonsoft.Json.Linq;
using Steamworks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;
using System.Threading;

namespace steamGameControl
{
    static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            checkSteam();
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

        public static string UIname = "";
        public static string UIappid = "";
        public static string UItotal = "";
        public static int  UIindex = 0;

        //check if steam is running
        public static void checkSteam()
        {
            try
            {
                Environment.SetEnvironmentVariable("SteamAppId", "440");
                if (!SteamAPI.Init()) { }
                ulong steamId = SteamUser.GetSteamID().m_SteamID;
                SteamAPI.Shutdown();
            }
            catch(Exception)
            {
                MessageBox.Show("Steam not running or something went wrong :/");
                Environment.Exit(0);
            }

        }

        //public profile
        public static ArrayList GetGames()
        {
            ulong steamId = SteamUser.GetSteamID().m_SteamID;
            try
            {
                var apiJson = new StreamReader(WebRequest.Create("http://api.steampowered.com/IPlayerService/GetOwnedGames/v0001/?key=006C1D814005AF1CAE4B670EE4B38979&steamid=" + steamId + "&l=english&json").GetResponse().GetResponseStream()).ReadToEnd();
                var jsonList = JsonConvert.DeserializeObject<jsonResponse>(apiJson);

                UItotal = "" + jsonList.response.game_count;

                ArrayList gameList = new ArrayList();
                foreach (var item in jsonList.response.games)
                {
                    UIindex++;
                    var gameApiJson = new StreamReader(WebRequest.Create("http://store.steampowered.com/api/appdetails?appids=" + item.appid).GetResponse().GetResponseStream()).ReadToEnd();
                    var gameJsonList = JsonConvert.DeserializeObject<dynamic>(gameApiJson);

                    string pass = gameJsonList[item.appid].success;

                    if (pass.Equals("True"))
                    {
                        UIname = gameJsonList[item.appid].data.name;
                        UIappid = item.appid;
                        gameList.Add(new Game { Name = gameJsonList[item.appid].data.name, Id = item.appid });                        
                    }

                    Thread.Sleep(1200); 
                }
                return gameList;
            }
            catch (Exception e) { MessageBox.Show(e.ToString() + " There was an error getting games from steam, if your profile is private please make it public before using this tool."); Application.Exit(); return null; }
        }
    }
}

public class jsonResponse
{
    public resp response { get; set; }
    public class resp
    {
        public int game_count { get; set; }
        public game[] games { get; set; }
        public class game
        {
            public String appid { get; set; }
            public String playtime_forever { get; set; }
        }
    }
}

public class Game
{
    public string Name { get; set; }
    public string Id { get; set; }
    public void getShit() { }
}