using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Party_Buffalo
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                if ((System.IO.File.Exists(Application.StartupPath + "\\Party Buffalo Drive Explorer.exe") || System.IO.File.Exists(Application.StartupPath + "\\Party Buffalo.exe")) && System.IO.Path.GetFileName(Application.ExecutablePath).Contains("new"))
                {
                    string Existing = (System.IO.File.Exists(Application.StartupPath + "\\Party Buffalo Drive Explorer.exe")) ? Application.StartupPath + "\\Party Buffalo Drive Explorer.exe" : Application.StartupPath + "\\Party Buffalo.exe";
                    System.IO.File.Delete(Existing);
                    System.IO.File.Move(Application.ExecutablePath, Application.StartupPath + "\\Party Buffalo.exe");
                    System.Diagnostics.Process.Start(Application.StartupPath + "\\Party Buffalo.exe");
                    System.Diagnostics.Process.GetCurrentProcess().Kill();
                }
            }
            catch (Exception e) { }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Main());
        }
    }
}
