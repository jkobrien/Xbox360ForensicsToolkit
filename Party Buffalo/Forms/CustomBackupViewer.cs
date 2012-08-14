using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CLKsFATXLib;
using Extensions;

namespace Party_Buffalo.Forms
{
    public partial class CustomBackupViewer : Form
    {

        string FolderPath;
        TreeNode rightClicked;
        public CustomBackupViewer(string FolderPath)
        {
            InitializeComponent();
            listView1.SetExplorerTheme();
            treeView1.SetExplorerTheme();
            this.FolderPath = FolderPath;
            this.Load += new EventHandler(CustomBackupViewer_Load);
            listView1.DoubleClick += new EventHandler(listView1_DoubleClick);
            listView1.ContextMenu = c_listView;
            treeView1.ContextMenu = c_treeView;
            treeView1.MouseClick += new MouseEventHandler(treeView1_MouseClick);
        }

        void treeView1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                try
                {
                    rightClicked = treeView1.GetNodeAt(new Point(e.X, e.Y));
                }
                catch { rightClicked = null; }
            }
        }

        void listView1_DoubleClick(object sender, EventArgs e)
        {
            if ((bool)((object[])listView1.FocusedItem.Tag)[1])
            {
                foreach (TreeNode n in treeView1.SelectedNode.Nodes)
                {
                    string nTag = (string)n.Tag;
                    string tag = (string)(((object[])listView1.FocusedItem.Tag)[0]);
                    if (nTag == tag)
                    {
                        treeView1.SelectedNode = n;
                        AfterSelect();
                        break;
                    }
                }
            }
        }

        public ImageList LargeListForFATX
        {
            get
            {
                ImageList i = new ImageList();
                Image[] images = { Properties.Resources.fatx_folder, Properties.Resources.fatx_file, Properties.Resources.fatx_database };
                i.ColorDepth = ColorDepth.Depth32Bit;
                i.ImageSize = new System.Drawing.Size(64, 64);
                i.Images.AddRange(images);
                return i;
            }
        }

        public ImageList SmallListForFATX
        {
            get
            {
                ImageList i = new ImageList();
                Image[] images = { Properties.Resources.fatx_folder_small, Properties.Resources.fatx_file_small, Properties.Resources.fatx_database };
                i.ColorDepth = ColorDepth.Depth32Bit;
                i.ImageSize = new System.Drawing.Size(16, 16);
                i.Images.AddRange(images);
                return i;
            }
        }

        void CustomBackupViewer_Load(object sender, EventArgs e)
        {
            LoadDirectories();
            treeView1.ImageList = SmallListForFATX;
            listView1.SmallImageList = treeView1.ImageList;
            this.Text += " " + FolderPath;
        }

        void LoadDirectories()
        {
            System.IO.DirectoryInfo currentDirectory = new System.IO.DirectoryInfo(FolderPath);
            string name = currentDirectory.Name;
            if (VariousFunctions.IsTitleIDFolder(currentDirectory.Name) && Party_Buffalo.Cache.CheckCache(name) != null)
            {
                name += " | " + Party_Buffalo.Cache.CheckCache(name);
            }
            TreeNode root = new TreeNode(name);
            root.Tag = currentDirectory.FullName;

            foreach (System.IO.DirectoryInfo d in currentDirectory.GetDirectories())
            {
                AddNodes(ref root, d);
            }
            root.Expand();
            treeView1.Nodes.Add(root);
        }

        void AddNodes(ref TreeNode parent, System.IO.DirectoryInfo directory)
        {
            string name = directory.Name;
            if (VariousFunctions.IsTitleIDFolder(name) && Party_Buffalo.Cache.CheckCache(name) != null)
            {
                name += " | " + Party_Buffalo.Cache.CheckCache(name);
            }
            TreeNode subNode = new TreeNode(name);
            subNode.Tag = directory.FullName;
            foreach (System.IO.DirectoryInfo di in directory.GetDirectories())
            {
                TreeNode subN2 = new TreeNode(di.Name);
                subN2.Tag = di.FullName;
                AddNodes(ref subNode, di);
            }
            parent.Nodes.Add(subNode);
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            AfterSelect();
        }

        void AfterSelect()
        {
            listView1.Items.Clear();

            System.IO.DirectoryInfo clicked = new System.IO.DirectoryInfo((string)treeView1.SelectedNode.Tag);
            foreach (System.IO.DirectoryInfo di in clicked.GetDirectories())
            {
                ListViewItem li = new ListViewItem(di.Name);
                li.SubItems.Add("File Folder");
                li.SubItems.Add("");
                li.SubItems.Add(di.LastWriteTime.ToString());
                li.SubItems.Add((VariousFunctions.IsTitleIDFolder(di.Name)) ? Party_Buffalo.Cache.CheckCache(di.Name) : "");
                li.Tag = new object[] { di.FullName, true };
                li.ImageIndex = 0;
                listView1.Items.Add(li);
            }

            foreach (System.IO.FileInfo fi in clicked.GetFiles())
            {
                ListViewItem li = new ListViewItem(fi.Name);
                li.SubItems.Add("File");
                li.SubItems.Add(VariousFunctions.ByteConversion(fi.Length));
                li.SubItems.Add(fi.LastWriteTime.ToString());
                // Get the file name
                CLKsFATXLib.Streams.Reader br = new CLKsFATXLib.Streams.Reader(new System.IO.FileStream(fi.FullName, System.IO.FileMode.Open));
                if (br.BaseStream.Length > 4)
                {
                    uint header = br.ReadUInt32(true);
                    if (header == 0x434F4E20 || header == 0x4C495645 || header == 0x50495253)
                    {
                        br.BaseStream.Position = (long)CLKsFATXLib.Geometry.STFSOffsets.DisplayName;
                        li.SubItems.Add(br.ReadUnicodeString(0x80));
                    }
                }
                br.Close();
                li.Tag = new object[] { fi.FullName, true };
                li.ImageIndex = 1;
                listView1.Items.Add(li);
            }
        }

        private void menuItem2_Click(object sender, EventArgs e)
        {
            if (rightClicked != null)
            {
                string system = Environment.GetFolderPath(Environment.SpecialFolder.System);
                string[] paths = system.Split('\\');
                system = "";
                for (int i = 0; i < paths.Length - 1; i++)
                {
                    system += paths[i] + "\\";
                }
                system += "explorer.exe";
                try
                {
                    System.Diagnostics.Process.Start(system, ((string)rightClicked.Parent.Tag) + "\\" + ((rightClicked.Text.Contains(" | ")) ? rightClicked.Name.Remove(rightClicked.Text.IndexOf(" | ")) : rightClicked.Text));
                }
                catch { System.Diagnostics.Process.Start((string)rightClicked.Parent.Tag); }
            }
        }

        private void menuItem3_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count != 0)
            {
                string system = Environment.GetFolderPath(Environment.SpecialFolder.System);
                string[] paths = system.Split('\\');
                system = "";
                for (int i = 0; i < paths.Length - 1; i++)
                {
                    system += paths[i] + "\\";
                }
                system += "explorer.exe";
                System.Diagnostics.Process.Start(system, "/select," + ((string)treeView1.SelectedNode.Tag + "\\" + listView1.FocusedItem.Text));
            }
        }

        private void menuItem1_Click(object sender, EventArgs e)
        {
            
        }
    }
}
