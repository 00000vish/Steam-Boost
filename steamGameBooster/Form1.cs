using Steamworks;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace steamGameBooster
{
    public partial class Form1 : Form
    {
        List<string> toIdleList = new List<string>();
        List<Process> idleProcess = new List<Process>();

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

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void listView1_ColumnClick(object sender, ColumnClickEventArgs e)
        {

        }

        //kill all the process of idler
        private void endAllIdleProcess()
        {
            foreach (Process p in idleProcess)
            {
                if (p != null)
                {
                    try { p.Kill(); } catch (Exception e) { }
                }
            }
        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            endAllIdleProcess();
            idleProcess.Add(Process.Start(new ProcessStartInfo("steamGameControl.exe", listView1.SelectedItems[0].SubItems[1].Text) { WindowStyle = ProcessWindowStyle.Hidden }));
        }

        public void addGamesToList(Object games)
        {

        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            endAllIdleProcess();
        }

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

        private void gameIdler()
        {
            do
            {
                foreach (string item in toIdleList)
                {
                    idleProcess.Add(Process.Start(new ProcessStartInfo("steamGameControl.exe", item) { WindowStyle = ProcessWindowStyle.Hidden }));
                    int x = 0;
                    Int32.TryParse(domainUpDown1.Text, out x);
                    if (!checkBox2.Checked) {
                        System.Threading.Thread.Sleep(x);
                        endAllIdleProcess();                       
                    }
                }
            } while (!checkBox2.Checked);
        }

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
                button1.Text = "Stop idleing";               
                threadOne = new ThreadStart(gameIdler);
                childThread = new Thread(threadOne);
                childThread.Start();
            }
        }

        private void listView1_ItemChecked(object sender, ItemCheckedEventArgs e)
        {            
            foreach (ListViewItem item in listView1.Items)
            {
                if (item.Checked)
                {                    
                    toIdleList.Add(item.SubItems[1].Text);
                }
                else
                {
                    toIdleList.Remove(item.SubItems[1].Text);
                }
            }
        }
    }
}
