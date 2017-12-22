using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace steamGameBooster
{
    static class Program
    {

        public const string CONTROLLER_FILE = "steamGameControl.exe";
        public const string GAME_LIST_FILE = "game-list.txt";
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            if (!System.IO.File.Exists(GAME_LIST_FILE))
            {               
                Process proc = new Process();
                proc.StartInfo.FileName = CONTROLLER_FILE;
                proc.StartInfo.Arguments = "gamelist";
                proc.Start();

                while (!System.IO.File.Exists(GAME_LIST_FILE))
                {
                    System.Threading.Thread.Sleep(1000);
                }
            }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}