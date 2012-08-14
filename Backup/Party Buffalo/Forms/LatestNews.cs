using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using Extensions;
using System.Web;

namespace Party_Buffalo.Forms
{
    public partial class LatestNews : Form
    {
        public LatestNews()
        {
            InitializeComponent();
            this.Load += new EventHandler(LatestNews_Load);
            listView1.SetExplorerTheme();
            listView1.DoubleClick += new EventHandler(listView1_DoubleClick);
            webBrowser1.Navigating += new WebBrowserNavigatingEventHandler(webBrowser1_Navigating);
        }

        void webBrowser1_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            if (!e.Url.ToString().Contains("http://clkxu5.com"))
            {
                e.Cancel = true;
                System.Diagnostics.Process.Start(e.Url.ToString());
            }
        }

        void listView1_DoubleClick(object sender, EventArgs e)
        {
            if (listView1.FocusedItem != null)
            {
                System.Diagnostics.Process.Start((string)listView1.FocusedItem.Tag);
            }
        }

        void LatestNews_Load(object sender, EventArgs e)
        {
            // Determine the last loaded ad
            switch (Properties.Settings.Default.loadedAd)
            {
                case 1:
                    Properties.Settings.Default.loadedAd = 2;
                    break;
                case 2:
                    Properties.Settings.Default.loadedAd = 1;
                    break;
            }

            // Set the timestamp on the last time we showed this window
            Properties.Settings.Default.latestNewsLastShown = DateTime.Now;
            Properties.Settings.Default.Save();

            System.Threading.ThreadStart ts = delegate
            {
                // Set the web browser page
                webBrowser1.Invoke((MethodInvoker)delegate
                {
                    webBrowser1.Url = new Uri("http://clkxu5.com/drivexplore/ad/ad" + Properties.Settings.Default.loadedAd.ToString() + ".html");
                });

                #region XML handling
                XmlTextReader reader = new XmlTextReader("http://clkxu5.com/feed/rss/");
                List<ListViewItem> List = new List<ListViewItem>();
                ListViewItem li = null;
                while (reader.Read())
                {
                    if (reader.Depth == 2 && reader.Name == "item")
                    {
                        li = new ListViewItem();
                    }
                    else if (reader.Depth == 3 && reader.Name == "title")
                    {
                        li.Text = reader.ReadInnerXml();
                    }
                    else if (reader.Depth == 3 && reader.Name == "description")
                    {
                        string s = reader.ReadInnerXml();
                        if (s == "<![CDATA[]]>")
                        {
                            s = "";
                        }
                        else
                        {
                            s = s.Remove(0, "<![CDATA[".Length);
                            s = s.Remove(s.Length - 2);
                            s = HttpUtility.HtmlDecode(s);
                        }
                        li.SubItems.Add(s);
                    }
                    else if (reader.Depth == 3 && reader.Name == "link")
                    {
                        li.Tag = reader.ReadInnerXml();
                        List.Add(li);
                    }
                }
                listView1.Invoke((MethodInvoker)delegate
                {
                    listView1.Items.AddRange(List.ToArray());
                });
            };
            System.Threading.Thread t = new System.Threading.Thread(ts);
            t.Start();
            #endregion
        }
    }
}
