using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;

namespace steamGameBooster
{
    public partial class Form1 : Form
    {
        List<string> toIdleList = new List<string>();

        ThreadStart threadOne;
        Thread childThread;

        public Form1()
        {
            InitializeComponent();
            readAllGames();
        }

        //reads the text file with all the games in it
        private void readAllGames()
        {
            string[] gameList = System.IO.File.ReadAllLines("game-list.txt");
            foreach (string game in gameList)
            {
                ListViewItem l1 = listView1.Items.Add("");
                l1.SubItems.Add(game.Split('`')[0]);
                l1.SubItems.Add(game.Split('`')[1]);
            }
        }

        //kill all the process of idler
        private void endAllIdleProcess()
        {
            foreach (var process in Process.GetProcessesByName("steamGameControl"))
            {
                process.Kill();
            }
            pictureBox1.Invoke(new Action(() => {
                pictureBox1.Visible = false;
                pictureBox1.Refresh();
            }));
            linkLabel1.Invoke(new Action(() => {
                linkLabel1.Text = "";
            }));
        }      

        //kill all the idle process when form program is closing
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            endAllIdleProcess();
        }

        //add game id to array when listview item is checked
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                foreach (ListViewItem item in listView1.Items)
                {
                    item.Checked = true;
                }
            }
            else
            {
                foreach (ListViewItem item in listView1.Items)
                {
                    item.Checked = false;
                }
            }
        }

        //controls the game idleing process
        private void gameIdler()
        {
            do
            {
                foreach (string item in toIdleList)
                {
                    pictureBox1.Invoke(new Action(() => {
                        pictureBox1.Visible = true;
                        pictureBox1.BackColor = System.Drawing.Color.Black;
                        pictureBox1.Load("http://cdn.akamai.steamstatic.com/steam/apps/" + item + "/header.jpg");
                    }));
                    linkLabel1.Invoke(new Action(() => {
                        linkLabel1.Links.Clear();
                        linkLabel1.Text = "Visit " + listView1.FindItemWithText(item).SubItems[2].Text + " Store Page";
                        linkLabel1.Links.Add(0, linkLabel1.Text.Length, "http://store.steampowered.com/app/" + item);
                    }));
                    Process.Start(new ProcessStartInfo("steamGameControl.exe", item) { WindowStyle = ProcessWindowStyle.Hidden });
                    int x = 0;
                    Int32.TryParse(domainUpDown1.Text, out x);
                   
                    if (!checkBox2.Checked)
                    {
                        System.Threading.Thread.Sleep(x);
                        endAllIdleProcess();                        
                    }
                }
            } while (!checkBox2.Checked);
        }

        //start idleing button
        private void button1_Click(object sender, EventArgs e)
        {
            if (button1.Text == "Stop idleing")
            {
                button1.Text = "Start idleing";
                endAllIdleProcess();
                childThread.Abort();
            }
            else
            {
                toIdleList.Clear();
                foreach (ListViewItem item in listView1.Items)
                {
                    if(item.Checked)toIdleList.Add(item.SubItems[1].Text);
                }

                button1.Text = "Stop idleing";
                threadOne = new ThreadStart(gameIdler);
                childThread = new Thread(threadOne);
                childThread.Start();
            }
        }

        //when simultaneously is checked
        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked)
            {
                domainUpDown1.Enabled = false;
            }
            else
            {
                domainUpDown1.Enabled = true;

            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
