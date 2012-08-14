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
    public partial class DeviceSelector : Form
    {

        Main m;
        System.Threading.Thread t;
        List<CLKsFATXLib.Drive> driveList = null;
        public DeviceSelector(Main m)
        {
            InitializeComponent();
            listView1.LargeImageList = LargeList;
            listView1.SmallImageList = LargeList;
            listView1.SetExplorerTheme();
            this.m = m;
            this.Load += new EventHandler(DeviceSelector_Load);
            this.listView1.DoubleClick += new EventHandler(listView1_DoubleClick);
            this.FormClosing += new FormClosingEventHandler(DeviceSelector_FormClosing);
        }

        public DeviceSelector()
        {
            InitializeComponent();
            listView1.LargeImageList = LargeList;
            listView1.SmallImageList = LargeList;
            listView1.SetExplorerTheme();
            this.Load += new EventHandler(DeviceSelector_Load);
            this.listView1.DoubleClick += new EventHandler(listView1_DoubleClick);
            this.FormClosing += new FormClosingEventHandler(DeviceSelector_FormClosing);
        }

        ImageList LargeList
        {
            get
            {
                ImageList li = new ImageList();
                Size s = new Size(64, 64);
                li.ImageSize = s;
                li.ColorDepth = ColorDepth.Depth32Bit;
                li.Images.Add(Properties.Resources.HDD);
                li.Images.Add(Properties.Resources.USB);
                li.Images.Add(Properties.Resources.Backup);
                return li;
            }
        }

        void DeviceSelector_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (t != null)
            {
                while (t.IsAlive)
                {
                    // Do nothing
                }
            }
        }

        void listView1_DoubleClick(object sender, EventArgs e)
        {
            if (listView1.FocusedItem != null)
            {
                b_Ok.PerformClick();
            }
        }

        void DeviceSelector_Load(object sender, EventArgs e)
        {
            Go();
        }

        void Go()
        {
            System.Threading.ThreadStart ts = delegate
            {
                try
                {
                    if (driveList != null)
                    {
                        for (int i = 0; i < driveList.Count; i++)
                        {
                            driveList[i].Close();
                        }
                    }
                    listView1.Invoke((MethodInvoker)delegate { listView1.Items.Clear(); });
                    label1.Invoke((MethodInvoker)delegate { label1.Text = "Getting drives..."; });
                    b_Refresh.Invoke((MethodInvoker)delegate { b_Refresh.Enabled = false; });
                    driveList = CLKsFATXLib.StartHere.GetFATXDrives().ToList();
                    if (Properties.Settings.Default.recentFiles != null)
                    {
                        foreach (string s in Properties.Settings.Default.recentFiles)
                        {
                            if (System.IO.File.Exists(s))
                            {
                                try
                                {
                                    CLKsFATXLib.Drive d = new CLKsFATXLib.Drive(s);
                                    if (d.IsFATXDrive())
                                    {
                                        driveList.Add(d);
                                    }
                                }
                                catch (Exception e)
                                {
                                    if (!e.Message.Contains("being used"))
                                    {
                                        MessageBox.Show("An exception was thrown: " + e.Message + "\r\n\r\nStack Trace:\r\n" + e.StackTrace);
                                    }
                                    else
                                    {
                                        continue;
                                    }
                                }
                            }
                        }
                    }
                    List<ListViewItem> liList = new List<ListViewItem>();
                    for (int i = 0; i < driveList.Count; i++)
                    {
                        try
                        {
                            ListViewItem li = new ListViewItem(driveList[i].Name);
                            if (driveList[i].DriveType == CLKsFATXLib.DriveType.HardDisk)
                            {
                                li.ImageIndex = 0;
                                li.SubItems.Add(driveList[i].DeviceIndex.ToString());
                            }
                            else if (driveList[i].DriveType == CLKsFATXLib.DriveType.USB)
                            {
                                li.ImageIndex = 1;
                                li.SubItems.Add(System.IO.Path.GetPathRoot(driveList[i].USBPaths[0]));
                            }
                            else
                            {
                                li.ImageIndex = 2;
                                li.SubItems.Add(System.IO.Path.GetFileName(driveList[i].FilePath));
                            }
                            li.SubItems.Add(driveList[i].LengthFriendly);
                            li.Tag = driveList[i];
                            liList.Add(li);
                        }
                        catch (Exception e) {
                            if (!e.Message.Contains("being used"))
                            {
                                MessageBox.Show("An exception was thrown: " + e.Message + "\r\n\r\nStack Trace:\r\n" + e.StackTrace);
                            }
                            else
                            {
                                continue;
                            }
                        }
                    }
                    listView1.Invoke((MethodInvoker)delegate
                    {
                        listView1.Items.AddRange(liList.ToArray());
                    });
                    label1.Invoke((MethodInvoker)delegate
                    {
                        if (liList.Count == 0)
                        {
                            label1.Text = "No drives found...";
                        }
                        else
                        {
                            label1.Text = liList.Count.ToString() + ((liList.Count > 1) ? " drives found" : " drive found");
                        }
                    });
                    b_Refresh.Invoke((MethodInvoker)delegate { b_Refresh.Enabled = true; });
                }
                catch (Exception e)
                {
                    if (!e.Message.Contains("being used"))
                    {
                        MessageBox.Show("An exception was thrown: " + e.Message + "\r\n\r\nStack Trace:\r\n" + e.StackTrace);
                    }
                }
            };
            t = new System.Threading.Thread(ts);
            t.Start();
        }

        private void b_Refresh_Click(object sender, EventArgs e)
        {
            Go();
        }

        private void b_Ok_Click(object sender, EventArgs e)
        {
            if (m != null)
            {
                if (m.Drive != null)
                {
                    m.Drive.Close();
                }
                m.Drive = (CLKsFATXLib.Drive)listView1.FocusedItem.Tag;
            }
            SelectedDrive = (CLKsFATXLib.Drive)listView1.FocusedItem.Tag;
            foreach (ListViewItem li in listView1.Items)
            {
                if (li != listView1.FocusedItem)
                {
                    ((CLKsFATXLib.Drive)li.Tag).Close();
                }
            }
        }

        private void b_Cancel_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem li in listView1.Items)
            {
                ((CLKsFATXLib.Drive)li.Tag).Close();
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            b_Ok.Enabled = true;
        }

        public CLKsFATXLib.Drive SelectedDrive
        {
            get;
            private set;
        }
    }
}
