using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using CLKsFATXLib;

namespace Party_Buffalo.Forms
{
    public partial class InjectDialog : Form
    {
        const int MF_BYPOSITION = 0x400;

        [DllImport("User32")]

        private static extern int RemoveMenu(IntPtr hMenu, int nPosition, int wFlags);

        [DllImport("User32")]

        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("User32")]

        private static extern int GetMenuItemCount(IntPtr hWnd);

        CLKsFATXLib.Structs.ExistingEntry exEntry;
        CLKsFATXLib.Structs.WriteResult WR;
        int ItemCount;
        public InjectDialog(CLKsFATXLib.Structs.ExistingEntry exEntry, int ItemCount)
        {
            InitializeComponent();
            this.ItemCount = ItemCount;
            this.exEntry = exEntry;
            this.Load += new EventHandler(InjectDialog_Load);
        }

        public InjectDialog(CLKsFATXLib.Structs.WriteResult wr, int ItemCount)
        {
            InitializeComponent();
            this.ItemCount = ItemCount;
            WR = wr;
            this.Load += new EventHandler(InjectDialog_Load);
        }

        void InjectDialog_Load(object sender, EventArgs e)
        {
            // Disabling the close button (because I'm a boss)
            IntPtr hMenu = GetSystemMenu(this.Handle, false);
            int menuItemCount = GetMenuItemCount(hMenu);
            RemoveMenu(hMenu, menuItemCount - 1, MF_BYPOSITION);
            //if (exEntry != null)
            //{
                // Changing the UI to fit the entry
                if (exEntry.Existing.IsFolder)
                {
                    this.l_Description.Text = "A folder with the name \"" + exEntry.Existing.Name + "\" already exists in the folder \"" + exEntry.Existing.Parent.Name + "\"";
                    if (VariousFunctions.IsFolder(exEntry.NewPath))
                    {
                        b_Option1.Text = "Merge";
                    }
                    else
                    {
                        b_Option1.Text = "Overwrite";
                    }
                    b_Option2.Text = "Skip";
                }
                else
                {
                    l_Description.Text = "A file with the name \"" + exEntry.Existing.Name + "\" already exists in the folder \"" + exEntry.Existing.Parent.Name + "\"";
                    b_Option1.Text = "Overwrite";
                    b_Option2.Text = "Skip";
                }
                checkBox1.Text = string.Format("Do this for all current items ({0})", ItemCount.ToString());
            //}
            //else
            //{
            //    // Changing the UI to fit the entry
            //    if (WR.Entry.IsFolder)
            //    {
            //        this.l_Description.Text = "A folder with the name \"" + WR.Entry.Name + "\" already exists in the folder \"" + WR.Entry.Parent.Name + "\"";
            //        if (WR.Entry.IsFolder)
            //        {
            //            b_Option1.Text = "Merge";
            //        }
            //        else
            //        {
            //            b_Option1.Text = "Overwrite";
            //        }
            //        b_Option2.Text = "Skip";
            //    }
            //    else
            //    {
            //        l_Description.Text = "A file with the name \"" + WR.Entry.Name + "\" already exists in the folder \"" + WR.Entry.Parent.Name + "\"";
            //        b_Option1.Text = "Overwrite";
            //        b_Option2.Text = "Skip";
            //    }
            //    checkBox1.Text = string.Format("Do this for all current items ({0})", ItemCount.ToString());
            //}
        }

        private void b_Option1_Click(object sender, EventArgs e)
        {

        }

        private void b_Option2_Click(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
