using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace steamGameBooster
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            if (!System.IO.File.Exists("game-list.txt"))
            {               
                Process proc = new Process();
                proc.StartInfo.FileName = "steamGameControl.exe";
                proc.StartInfo.Arguments = "gamelist";
                proc.Start();

                while (!System.IO.File.Exists("game-list.txt"))
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