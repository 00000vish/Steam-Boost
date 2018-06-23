using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace steamGameControl
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            if (Program.asBot)
                Program.checkSteam();
            InitializeComponent();
        }

        private void Form2_Shown(object sender, EventArgs e)
        {
            ThreadStart threadOne = new ThreadStart(getGame);
            Thread childThread = new Thread(threadOne);
            childThread.Start();
        }

        public void getGame()
        {
            if (!Program.asBot)
            {
                Environment.SetEnvironmentVariable("SteamAppId", "440");
                if (!SteamAPI.Init()) { }
            }
        
            ArrayList games = Program.GetGames();
            SteamAPI.Shutdown();
            string[] gameList = new string[games.Count];
            for (int i = 0; i < games.Count; i++)
            {
                Game item = (Game)games[i];
                gameList[i] = item.Id + "`" + item.Name;
            }
            if (Program.asBot)
            {
                System.IO.File.WriteAllLines("bot-game-list.txt", gameList);
            }
            else
            {
                System.IO.File.WriteAllLines("game-list.txt", gameList);
            }
            Application.Exit();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Size = new Size(516, 338);
            label1.Text = "Getting games from steam... [" + Program.UIname + "]";
            Text = "This might take awhile... [" + Program.UIindex + "/" + Program.UItotal + "]";
            try
            {
                pictureBox1.Load("http://cdn.akamai.steamstatic.com/steam/apps/" + Program.UIappid + "/header.jpg");
            }
            catch (Exception){}
        }
    }
}
