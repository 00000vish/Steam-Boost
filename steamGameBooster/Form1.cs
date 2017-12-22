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
            string[] gameList = System.IO.File.ReadAllLines(Program.GAME_LIST_FILE);
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
            foreach (var process in Process.GetProcessesByName(Program.CONTROLLER_FILE.Split('.')[0]))
            {
                process.Kill();
            }
            try
            {
                pictureBox1.Invoke(new Action(() =>
                {
                    groupBox2.Visible = false;
                    pictureBox1.Refresh();
                    linkLabel1.Text = "";
                }));
            }
            catch (Exception e) { };
        }

        //kill all the idle process when form program is closing
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (childThread != null) childThread.Abort();
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
            int v = 0; Int32.TryParse(domainUpDown1.Text, out v);
            v = ((v * 60000));
            pictureBox1.Invoke(new Action(() =>
            {
                timer1.Enabled = true;
                timer1.Interval = v;
                timer1.Start();
            })); //setup the timer to close

            do
            {
                foreach (string item in toIdleList)
                {
                    try
                    {
                        pictureBox1.Invoke(new Action(() =>
                        {
                            button2.Tag = item;
                            groupBox2.Visible = true;
                            pictureBox1.BackColor = System.Drawing.Color.Black;
                            pictureBox1.Load("http://cdn.akamai.steamstatic.com/steam/apps/" + item + "/header.jpg");
                            linkLabel1.Links.Clear();
                            string gameName = listView1.FindItemWithText(item).SubItems[2].Text;
                            linkLabel1.Text = "Visit Store Page for " + gameName;
                            if (gameName.Length > 11) linkLabel1.Text = "Visit Store Page for " + gameName.Substring(0, 12) + Environment.NewLine + gameName.Substring(11, gameName.Length - 11);
                            linkLabel1.Links.Add(0, linkLabel1.Text.Length, "http://store.steampowered.com/app/" + item);
                        }));
                    }
                    catch (Exception e) { };

                    Process.Start(new ProcessStartInfo(Program.CONTROLLER_FILE, item) { WindowStyle = ProcessWindowStyle.Hidden });

                    if (!checkBox2.Checked) //simultaneously unlchecked
                    {
                        int x = 0;
                        Int32.TryParse(domainUpDown1.Text, out x);
                        //System.Threading.Thread.Sleep((x * 60000) / toIdleList.Count);
                        System.Threading.Thread.Sleep(6000);
                        endAllIdleProcess();
                        System.Threading.Thread.Sleep(6000);
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
                    if (item.Checked) toIdleList.Add(item.SubItems[1].Text);
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
            if (!checkBox2.Checked)
            {
                int temp = 0;
                foreach (ListViewItem item in listView1.Items)
                {
                    if (item.Checked) temp++;
                }
                if (temp > 4)
                {
                    MessageBox.Show(this, "Make sure the game has been idle simultaneously for atleast 2 hours before idling unsimultaneously.", "Steam Boost", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show(this, "Make sure you have atleast 5 games selected to quickly switch between and make sure the game has been idle simultaneously for atleast 2 hours.", "Steam Boost", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    checkBox2.Checked = true;
                }                
            }
        }

        //link label clicked
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(e.Link.LinkData.ToString());
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //nothing
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //<TODO>: STEAM UNLOCK ACHIEMENTS!
            Form f2 = new Form2(button2.Tag.ToString());
            f2.Show();
        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            //<TODO>: STEAM UNLOCK ACHIEMENTS!
            Form f2 = new Form2(listView1.SelectedItems[0].SubItems[1].Text);
            f2.Show();
        }

        //stops idling after time is up
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (button1.Text == "Stop idleing") button1.PerformClick();
            timer1.Stop();
        }

        private void richTextBox1_Click(object sender, EventArgs e)
        {
            if(richTextBox1.Text == "Search")
                richTextBox1.Clear();
        }

        private void richTextBox1_Leave(object sender, EventArgs e)
        {
            richTextBox1.Text = "Search";
        }

        private void richTextBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            foreach (ListViewItem item in listView1.Items)
            {
                item.Selected = false;
            }
            if(e.KeyChar == 13)
            {
                if (richTextBox1.Text != "Search" && richTextBox1.Text.Length > 2)
                {
                    try
                    {
                        listView1.FindItemWithText(richTextBox1.Text).Selected = true;
                        listView1.Select();
                        listView1.EnsureVisible(listView1.Items.IndexOf( listView1.SelectedItems[0]));
                    }
                    catch (Exception z) { }
                }
            }
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.IO.File.Delete(Program.GAME_LIST_FILE);
            Application.Restart();
        }
    }
}
