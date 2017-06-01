using Steamworks;
using System;
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
            Environment.SetEnvironmentVariable("SteamAppId", "440");
            if (!SteamAPI.Init()) { }
            var games = Program.GetGames();
            SteamAPI.Shutdown();
            string[] gameList = new string[games.Count];
            for (int i = 0; i < games.Count; i++)
            {
                gameList[i] = games[i].ID + "`" + games[i].Name;
            }
            System.IO.File.WriteAllLines("game-list.txt", gameList);
            Application.Exit();
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }
    }
}
