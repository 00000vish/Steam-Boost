using Newtonsoft.Json.Linq;
using Steamworks;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace steamGameControl
{
    public partial class Form1 : Form
    {
        public Form1(String id)
        {
            if(Program.asBot)
                Program.checkSteam();
            InitializeComponent();
            pictureBox1.Load("http://cdn.akamai.steamstatic.com/steam/apps/" + id + "/header_292x136.jpg");
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
