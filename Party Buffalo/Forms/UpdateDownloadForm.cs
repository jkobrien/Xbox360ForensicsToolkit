using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace Party_Buffalo.Forms
{
    public partial class UpdateDownloadForm : Form
    {
        const int MF_BYPOSITION = 0x400;

        [DllImport("User32")]

        private static extern int RemoveMenu(IntPtr hMenu, int nPosition, int wFlags);

        [DllImport("User32")]

        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("User32")]

        private static extern int GetMenuItemCount(IntPtr hWnd);
        public UpdateDownloadForm(Update.UpdateInfo ui)
        {
            InitializeComponent();
            //Removes the exit button
            IntPtr hMenu = GetSystemMenu(this.Handle, false);
            int menuItemCount = GetMenuItemCount(hMenu);
            RemoveMenu(hMenu, menuItemCount - 1, MF_BYPOSITION);
            Update u = new Update();
            u.DownloadUpdate(ui, this, Application.StartupPath + "\\Party Buffalo_new.exe");
        }

        public int ProgressBarValue
        {
            get
            {
                return progressBar1.Value;
            }
            set
            {
                progressBar1.Value = value;
                if (progressBar1.Value == ProgressBarMax)
                {
                    while (true)
                    {
                        try
                        {
                            System.Diagnostics.Process.Start(System.Windows.Forms.Application.StartupPath + "\\Party Buffalo Drive Explorer_new.exe");
                            break;
                        }
                        catch { }
                    }
                    // Close the app
                    System.Diagnostics.Process.GetCurrentProcess().Kill();
                }
            }
        }

        public int ProgressBarMax
        {
            get
            {
                return progressBar1.Maximum;
            }
            set { progressBar1.Maximum = value; }
        }
    }
}
