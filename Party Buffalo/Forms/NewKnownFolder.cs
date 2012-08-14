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
    public partial class NewKnownFolder : Form
    {
        ImageList bigtits = new ImageList();
        string selected = null;
        public NewKnownFolder()
        {
            InitializeComponent();
            bigtits.ImageSize = new Size(64, 64);
            bigtits.ColorDepth = ColorDepth.Depth32Bit;
            listView1.LargeImageList = bigtits;

            listView1.SetExplorerTheme();
            listView1.DoubleClick += new EventHandler(listView1_DoubleClick);
            HelpButtonClicked += new CancelEventHandler(NewKnownFolder_HelpButtonClicked);
            bigtits.Images.Add(Properties.Resources.blankBox);
            Populate();
        }

        void NewKnownFolder_HelpButtonClicked(object sender, CancelEventArgs e)
        {
            MessageBox.Show("The reason why there may be whitespace here is because the icons either haven't been cached, or there is no available icon for the game based off of previous loadings.\r\n\r\nTo make sure that you are caching the icons, click the \"Drive\" menu, go down to loading options, and select to automatically cache game icons.  This will make loading a little slower at first, but will speed up over time.");
        }

        void listView1_DoubleClick(object sender, EventArgs e)
        {
            if (listView1.FocusedItem != null)
            {
                Selected = (string)listView1.FocusedItem.Tag;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        public string Selected
        {
            get
            {
                return selected;
            }

            private set
            {
                selected = value;
            }
        }

        void Populate()
        {
            List<ListViewItem> mil = new List<ListViewItem>();
                // Add the "new known folder" (cached) items
                for (int i = 0; i < Properties.Settings.Default.cachedID.Count; i++)
                {
                    string s = Properties.Settings.Default.correspondingIDName[i];
                    string ss = Properties.Settings.Default.cachedID[i];
                    // Create our listview menu item...
                    ListViewItem li = null;
                    if (s == "" || s == null)
                    {
                        li = new ListViewItem(ss);
                    }
                    else
                    {
                        li = new ListViewItem(s);
                        ListViewItem.ListViewSubItem lis = new ListViewItem.ListViewSubItem(li, ss);
                        li.SubItems.Add(lis);
                    }
                    try
                    {
                        bigtits.Images.Add(GetIcon(ss));
                        li.ImageIndex = bigtits.Images.Count - 1;
                    }
                    catch { li.ImageIndex = 0; }
                    // Set its tag
                    li.Tag = ss;

                    // Add it to the cached menu items
                    mil.Add(li);
                }

                // Cast those as arrays
                ListViewItem[] ArrayL = mil.ToArray();
                Array.Sort(ArrayL, new Extensions.ListViewItemComparer());

                // Add ranges
                listView1.Items.AddRange(ArrayL);
        }

        System.Drawing.Image GetIcon(string Name)
        {
            string FolderPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Party Buffalo Drive Explorer\\Cached Icons";
            if (!System.IO.Directory.Exists(FolderPath))
            {
                return null;
            }

            if (System.IO.File.Exists(FolderPath + "\\" + Name + ".png"))
            {
                return Image.FromFile(FolderPath + "\\" + Name + ".png");
            }
            return null;
        }
    }
}
