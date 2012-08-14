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
    public partial class InjectQueue : Form
    {
        public InjectQueue()
        {
            InitializeComponent();
            listView1.SetExplorerTheme();
        }

        public void AddToQueue(CLKsFATXLib.Structs.Queue Thingy)
        {
            ListViewItem li = new ListViewItem(Thingy.Folder.FullPath);
            li.SubItems.Add(Thingy.Path);
            if (Thingy.Writing)
            {
                listView1.Groups[0].Items.Add(li);
            }
            else
            {
                listView1.Groups[1].Items.Add(li);
            }
        }
    }
}
