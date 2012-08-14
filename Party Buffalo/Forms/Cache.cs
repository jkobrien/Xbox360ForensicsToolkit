using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Extensions;

namespace Party_Buffalo.Forms
{
    public partial class Cache : Form
    {

        public Cache()
        {
            InitializeComponent();
            listView1.SetExplorerTheme();
            listView1.ContextMenu = contextMenu1;
            try
            {
                for (int i = 0; i < Properties.Settings.Default.cachedID.Count; i++)
                {
                    ListViewItem li = new ListViewItem(Properties.Settings.Default.cachedID[i]);
                    ListViewItem.ListViewSubItem lsi = new ListViewItem.ListViewSubItem(li, Properties.Settings.Default.correspondingIDName[i]);
                    li.SubItems.Add(lsi);
                    listView1.Items.Add(li);
                }
            }
            catch
            {
                if (Properties.Settings.Default.cachedID == null)
                {
                    Properties.Settings.Default.cachedID = new System.Collections.Specialized.StringCollection();
                    Properties.Settings.Default.correspondingIDName = new System.Collections.Specialized.StringCollection();
                    Properties.Settings.Default.Save();
                }
            }

            if (Properties.Settings.Default.label == null)
            {
                Properties.Settings.Default.label = new System.Collections.Specialized.StringCollection();
                Properties.Settings.Default.labelPath = new System.Collections.Specialized.StringCollection();
                Properties.Settings.Default.Save();
            }

            for (int i = 0; i < Properties.Settings.Default.label.Count; i++)
            {
                ListViewItem li = new ListViewItem(Properties.Settings.Default.labelPath[i]);
                ListViewItem.ListViewSubItem lsi = new ListViewItem.ListViewSubItem(li, Properties.Settings.Default.label[i]);
                li.SubItems.Add(lsi);
                li.Tag = true;
                listView1.Items.Add(li);
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                foreach (ListViewItem li in listView1.Items)
                {
                    li.Checked = true;
                }
                return;
            }
            foreach (ListViewItem li in listView1.Items)
            {
                li.Checked = false;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.cachedID = null;
            Properties.Settings.Default.correspondingIDName = null;
            Properties.Settings.Default.label = null;
            Properties.Settings.Default.labelPath = null;
            Properties.Settings.Default.Save();
            listView1.Items.Clear();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            for(int i = 0; i < listView1.Items.Count; i++)
            {
                if (listView1.Items[i].Checked)
                {
                    if (listView1.Items[i].Tag == null)
                    {
                        Properties.Settings.Default.cachedID.RemoveAt(i);
                        Properties.Settings.Default.correspondingIDName.RemoveAt(i);
                        Properties.Settings.Default.Save();
                    }
                    else
                    {
                        Properties.Settings.Default.labelPath.Remove(listView1.Items[i].Text);
                        Properties.Settings.Default.label.Remove(listView1.Items[i].SubItems[1].Text);
                        Properties.Settings.Default.Save();
                    }
                    listView1.Items[i].Remove();
                    i--;
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Text Document(*.txt)|*.txt";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                System.IO.BinaryWriter bw = new System.IO.BinaryWriter(new System.IO.FileStream(sfd.FileName, System.IO.FileMode.CreateNew));
                bool Friendly = true;
                if (MessageBox.Show("Would you like to use friendly formatting? (Title ID: xxx Game Name: xxx)", "Use Friendly Formatting?", MessageBoxButtons.YesNo) == DialogResult.No)
                {
                    Friendly = false;
                }
                if (Friendly)
                {
                    for (int i = 0; i < Properties.Settings.Default.cachedID.Count; i++)
                    {
                        byte[] toWrite = Encoding.ASCII.GetBytes("Title ID: " + Properties.Settings.Default.cachedID[i] + " Game Name: " + Properties.Settings.Default.correspondingIDName[i] + "\r\n");
                        bw.Write(toWrite);
                    }
                    bw.Close();
                    return;
                }
                for (int i = 0; i < Properties.Settings.Default.cachedID.Count; i++)
                {
                    byte[] toWrite = Encoding.ASCII.GetBytes(Properties.Settings.Default.cachedID[i] + " " + Properties.Settings.Default.correspondingIDName[i] + "\r\n");
                    bw.Write(toWrite);
                }
                bw.Close();
            }
        }

        private void menuItem1_Click(object sender, EventArgs e)
        {
            try
            {
                if (listView1.SelectedItems.Count == 1)
                {
                    Clipboard.SetDataObject("Title ID: " + listView1.SelectedItems[0].SubItems[1].Text + " Game Name: " + listView1.SelectedItems[0].Text, true, 5, 250);
                }
            }
            catch { }
        }

        private void menuItem2_Click(object sender, EventArgs e)
        {
            // lolcheat
            button2.PerformClick();
        }
    }
}
