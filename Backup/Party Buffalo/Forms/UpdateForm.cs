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
    public partial class UpdateForm : Form
    {

        public UpdateForm(Update.UpdateInfo updateinfo)
        {
            InitializeComponent();
            AddToUpdate("Update Version: " + updateinfo.UpdateVersion);
            string[] changeLog = updateinfo.UpdateText.Split('`');
            AddToUpdate("Changelog:");
            for (int i = 0; i < changeLog.Length; i++)
            {
                if (changeLog[i] == "")
                {
                    AddToUpdate("");
                    continue;
                }
                AddToUpdate("-" + changeLog[i]);
            }
        }

        void AddToUpdate(string text)
        {
            if (updateBox.Text.Length != 0x0)
            {
                updateBox.Text += "\r\n";
            }
            updateBox.Text += text;
        }
    }
}
