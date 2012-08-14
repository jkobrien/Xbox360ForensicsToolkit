using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CLKsFATXLib;
using Extensions;

namespace Party_Buffalo
{
    public partial class DriveExplorer : UserControl
    {
        /*
        ImageList largeIL = new Misc().LargeListForFATX;
        ImageList smallIL = new Misc().SmallListForFATX;
        bool Deletionmode = false;
        bool Beta = false;
        internal FATXDrive drive;
        string LastBrowsedTo;
        bool Loaded = false;
        bool goingToPath = false;
        Misc misc = new Misc();
        private List<string> visitedFolders = new List<string>();
        ListViewColumnSorter lvwColumnSorter = new ListViewColumnSorter();
        System.Media.SoundPlayer sp;
        TreeNode rightClickedNode;
        List<string> History = new List<string>();
         */
        public DriveExplorer()
        {
            InitializeComponent();
            /*
            treeview_ContextMenu.Popup += new EventHandler(treeview_ContextMenu_Popup);
            listview_ContextMenu.Popup += new EventHandler(listview_ContextMenu_Popup);
            listview_ContextMenu.Collapse += new EventHandler(listview_ContextMenu_Collapse);
            listView1.DragDrop += new DragEventHandler(listView1_DragDrop);
            listView1.DragOver += new DragEventHandler(listView1_DragOver);
            listView1.BeforeLabelEdit += new LabelEditEventHandler(listView1_BeforeLabelEdit);
            listView1.AfterLabelEdit += new LabelEditEventHandler(listView1_AfterLabelEdit);
            treeView1.AfterSelect +=new TreeViewEventHandler(treeView1_AfterSelect);
            button1.Click +=new EventHandler(button1_Click);
            b_Forward.Click +=new EventHandler(b_Forward_Click);
            b_Back.Click +=new EventHandler(b_Back_Click);

            treeView1.BeforeLabelEdit += new NodeLabelEditEventHandler(treeView1_BeforeLabelEdit);
            listView1.ColumnClick += new ColumnClickEventHandler(listView1_ColumnClick);
            treeView1.AfterLabelEdit += new NodeLabelEditEventHandler(treeView1_AfterLabelEdit);
            listView1.DoubleClick += new EventHandler(listView1_DoubleClick);
            listView1.ItemDrag += new ItemDragEventHandler(listView1_ItemDrag);
            listView1.ListViewItemSorter = lvwColumnSorter;
            treeView1.MouseDown += new MouseEventHandler(treeView1_MouseDown);
            listView1.KeyDown += new KeyEventHandler(listView1_KeyDown);
             */
        }

        /*
        void treeView1_MouseDown(object sender, MouseEventArgs e)
        {
            if (treeView1.SelectedNode != null && e.X != 0 && e.Y != 0)
            {
                rightClickedNode = treeView1.GetNodeAt(e.X, e.Y);
            }
        }

        void listView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.A && e.Control)
            {
                listView1.MultiSelect = true;
                foreach (ListViewItem item in listView1.Items)
                {
                    item.Selected = true;
                }
            }
        }

        void listView1_ItemDrag(object sender, ItemDragEventArgs e)
        {
            
        }

        void treeview_ContextMenu_Popup(object sender, EventArgs e)
        {
            if (!Loaded)
            {
                foreach(MenuItem m in treeview_ContextMenu.MenuItems)
                {
                    m.Enabled = false;
                }
                return;
            }
            else if (rightClickedNode == treeView1.Nodes[0])
            {
                foreach (MenuItem m in treeview_ContextMenu.MenuItems)
                {
                    if (m != t_driveProperties)
                    {
                        m.Enabled = false;
                    }
                    else
                    {
                        m.Enabled = true;
                    }
                }
                return;
            }
            foreach (MenuItem m in treeview_ContextMenu.MenuItems)
            {
                m.Enabled = true;
            }

            foreach (TreeNode n in treeView1.Nodes[0].Nodes)
            {
                if (rightClickedNode == n)
                {
                    t_Rename.Enabled = false;
                    t_Delete.Enabled = false;
                    t_Properties.Enabled = false;
                }
            }
        }

        void listView1_AfterLabelEdit(object sender, LabelEditEventArgs e)
        {
            if (e.Label == null)
            {
                return;
            }
            if (e.Label == listView1.Items[e.Item].Text.ToLower())
            {
                return;
            }
            foreach (ListViewItem i in listView1.Items)
            {
                if (i.Text.ToLower() == e.Label.ToLower())
                {
                    MessageBox.Show("The entry name " + e.Label + " already exists in this folder");
                    return;
                }
            }
            if (e.Label == null)
            {
                e.CancelEdit = true;
                return;
            }
            if (!new Misc().CheckFileName(e.Label) || e.Label == listView1.Items[e.Item].Text)
            {
                e.CancelEdit = true;
                return;
            }
            Entry x = (Entry)(listView1.Items[e.Item]).Tag;
            if (x.IsDeleted)
            {
                e.CancelEdit = true;
                return;
            }
            if (x.Rename(e.Label))
            {
                // Remove the old treenode
                foreach (TreeNode n in treeView1.SelectedNode.Nodes)
                {
                    if (n.Text == listView1.Items[e.Item].Text)
                    {
                        n.Remove();
                    }
                }
                Folder f = ((Folder)treeView1.SelectedNode.Tag);
                f.ReloadData(Deletionmode);
                treeView1.SelectedNode.Tag = f;
                ReadFolderData();
            }
        }

        void listView1_BeforeLabelEdit(object sender, LabelEditEventArgs e)
        {
            
        }

        void listView1_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop) && !Beta)
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        void listView1_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (Loaded && !Beta)
            {
                Folder f = (Folder)treeView1.SelectedNode.Tag;
                EntryAction ea = new EntryAction(files, f);
                ea.ShowDialog(this);
                TreeNode sn = treeView1.SelectedNode;
                List<string> Rebuilt = ea.MergedPaths;
                foreach (string s in Rebuilt)
                {
                    try
                    {
                        ReloadNodes(GetNodeFromPath(s, treeView1.Nodes[0]));
                    }
                    catch { }
                }
                f.ReloadData(Deletionmode);
                treeView1.SelectedNode.Tag = f;
                ReadFolderData();
            }
            else if (!(new Misc().IsFolder(files[0])))
            {
                Clear();
                xDrive = new FATXDrive(files[0], Info.DriveType.Backup);
                Loaded = true;
                xDrive.ReadData();
                ReadDrive();
            }
        }

        void listview_ContextMenu_Collapse(object sender, EventArgs e)
        {

        }

        void listview_ContextMenu_Popup(object sender, EventArgs e)
        {
            Entry E = null;
            try
            {
                E = (Entry)listView1.FocusedItem.Tag;
            }
            catch { return; }
            if (E.IsFolder)
            {
                //c_Properties.Enabled = false;
                l_AddToBookMarks.Visible = true;
            }
            else
            {
                l_AddToBookMarks.Visible = false;
                //c_Properties.Enabled = true;
                if (E.IsDeleted)
                {
                    c_Delete.Enabled = false;
                }
            }
        }

        void treeView1_BeforeLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            if (e.Node.Text.Contains(" | "))
            {
                e.Node.Text = e.Node.Text.Remove(e.Node.Text.IndexOf(" | "));
            }
        }

        void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            Environment.Exit(0);
        }

        void listView1_DoubleClick(object sender, EventArgs e)
        {
            Entry E = (Entry)listView1.FocusedItem.Tag;
            if (E.IsFolder)
            {
                foreach (TreeNode n in treeView1.SelectedNode.Nodes)
                {
                    if ((Entry)n.Tag == E)
                    {
                        treeView1.SelectedNode = n;
                        ReadFolderData();
                    }
                }
            }
        }

        #region Treeview Editing
        /// <summary>
        /// Renames the folder on the drive - Checks if the node is a root/partition
        /// </summary>
        void treeView1_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            if (e.Label == null)
            {
                e.CancelEdit = true;
                return;
            }
            if (e.Label == e.Node.Text.ToLower())
            {
                return;
            }

            foreach (TreeNode n in rightClickedNode.Parent.Nodes)
            {
                if (n.Text.ToLower() == e.Label.ToLower())
                {
                    MessageBox.Show("The entry name " + e.Label + " already exists in this folder");
                    return;
                }
            }
            if (e.Label == null)
            {
                e.CancelEdit = true;
                return;
            }
            if (!new Misc().CheckFileName(e.Label))
            {
                e.CancelEdit = true;
                return;
            }
            Folder x = (Folder)e.Node.Tag;
            if (x.IsDeleted)
            {
                e.CancelEdit = true;
                return;
            }
            if (x.Rename(e.Label))
            {
                // Reset the parent node
                Folder f = ((Folder)e.Node.Parent.Tag);
                f.ReloadData(Deletionmode);
                e.Node.Parent.Tag = f;
                //ReadFolderData(); don't need to read folder data
            }
        }
        #endregion

        #region Column Sorting for Listview
        /// <summary>
        /// Column Sorter
        /// </summary>
        void listView1_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            // Determine if clicked column is already the column that is being sorted.
            if (e.Column == lvwColumnSorter.SortColumn)
            {
                // Reverse the current sort direction for this column.
                if (lvwColumnSorter.Order == SortOrder.Ascending)
                {
                    lvwColumnSorter.Order = SortOrder.Descending;
                    ListViewExtensions.SetSortIcon(listView1, e.Column, SortOrder.Descending);
                }
                else
                {
                    lvwColumnSorter.Order = SortOrder.Ascending;
                    ListViewExtensions.SetSortIcon(listView1, e.Column, SortOrder.Ascending);
                }
            }
            else
            {
                // Set the column number that is to be sorted; default to ascending.
                lvwColumnSorter.SortColumn = e.Column;
                lvwColumnSorter.Order = SortOrder.Ascending;
                ListViewExtensions.SetSortIcon(listView1, e.Column, SortOrder.Ascending);
            }

            // Perform the sort with these new sort options.
            this.listView1.Sort();
        }
        #endregion

        internal FATXDrive xDrive
        {
            get { return drive; }
            set { drive = value; }
        }

        #region Show Forms
        /// <summary>
        /// Show about form
        /// </summary>
        private void ShowAbout_menuItem7_Click(object sender, EventArgs e)
        {
            About abt = new About();
            abt.ShowDialog();
        }

        private void ShowDeviceSelector_menuItem2_Click(object sender, EventArgs e)
        {
            DeviceSelector ds = new DeviceSelector(this);
            if (ds.ShowDialog() == DialogResult.OK)
            {
                Loaded = true;
                xDrive.ReadData();
                ReadDrive();
            }
        }
        #endregion

        #region Clear
        /// <summary>
        /// Clears and resets the form
        /// </summary>
        public void Clear()
        {
            foreach (MenuItem mi in treeview_ContextMenu.MenuItems)
            {
                mi.Enabled = false;
            }
            foreach (MenuItem mi in listview_ContextMenu.MenuItems)
            {
                mi.Enabled = false;
            }
            foreach (MenuItem mi in menuItem9.MenuItems)
            {
                mi.Enabled = false;
            }
            m_Reload.Enabled = false;
            ts_Close.Enabled = false;
            ts_Reload.Enabled = false;
            listView1.Items.Clear();
            treeView1.Nodes[0].Nodes.Clear();
            treeView1.Nodes[0].Text = "Drive";
            Loaded = false;
            if (xDrive != null)
            {
                xDrive.Close();
                xDrive = null;
            }
            pathBar.Text = "";
            m_Search.Enabled = false;
            History = new List<string>();
            b_Back.Enabled = false;
            b_Back.Tag = null;
            b_Forward.Enabled = false;
        }
        #endregion

        #region Reading Drive Stuff
        public void ReadDrive()
        {
            foreach (MenuItem mi in treeview_ContextMenu.MenuItems)
            {
                if (mi.Text != "New Folder" && mi.Text != "Delete" && mi.Text != "Rename" && mi.Text != "Inject New File" && Beta)
                {
                    mi.Enabled = true;
                }

                if (!Beta)
                {
                    mi.Enabled = true;
                }
            }
            foreach (MenuItem mi in listview_ContextMenu.MenuItems)
            {
                if (!mi.Text.Contains("Inject") && mi.Text != "Overwrite" && mi.Text != "New Folder" && mi.Text != "Delete" && mi.Text != "Rename" && mi.Text != "Inject New File" && Beta)
                {
                    mi.Enabled = true;
                }

                if (!Beta)
                {
                    mi.Enabled = true;
                }
            }
            foreach (MenuItem mi in menuItem9.MenuItems)
            {
                mi.Enabled = true;
            }

            if (xDrive.DriveType != Info.DriveType.HDD)
            {
                m_BackupImage.Enabled = false;
                m_RestoreImage.Enabled = false;
                m_ExtractJosh.Enabled = false;
                m_ExtractSecuritySector.Enabled = false;
            }
            m_Reload.Enabled = true;
            ts_Reload.Enabled = true;
            ts_Close.Enabled = true;
            m_Search.Enabled = true;
            AddPartitionsToTree();
        }

        /// <summary>
        /// Adds Base Partitions to treeview
        /// </summary>
        private void AddPartitionsToTree()
        {
            //Create our folders (that will act as partitions) for each partition.
            AddToLog("Getting main partitions for " + xDrive.DriveType.ToString() + "\r\nDrive is " + xDrive.DriveSizeConverted);
            Folder[] Partitions = xDrive.Partitions;
            for (int i = 0; i < Partitions.Length; i++)
            {
                AddToLog("Found " + Partitions[i].Name + " partition");
                FATX.Structs.PartitionInfo pi = Partitions[i].PartInfo;
                AddToLog("Partition offset: " + pi.Offset.ToString("X"));
                AddToLog("Partition size: " + pi.Size.ToString("X"));
                AddToLog("FAT size: " + pi.FATSize.ToString("X"));
                AddToLog("Real FAT size: " + pi.RealFATSize.ToString("X"));
                AddToLog("FAT location: " + pi.FATOffset.ToString("X"));
                AddToLog("Number of clusters: " + (pi.RealFATSize / (int)pi.EntrySize).ToString("X"));
                AddToLog("Data possible: " + new Misc().ByteConversion((pi.RealFATSize / (int)pi.EntrySize) * pi.ClusterSize));
                AddToLog("Bit: " + ((int)pi.EntrySize).ToString("X"));
                AddToLog("Data offset: " + pi.DataOffset.ToString("X"));
                AddToLog("\r\n");
                //MessageBox.Show(pi.Clusters.ToString("X"));
            }
            //Set their EData
            for (int i = 0; i < Partitions.Length; i++)
            {
                Partitions[i].Name = Partitions[i].PartInfo.Name;
                Partitions[i].EData.StartingCluster = 1;
            }

            //Clear the subnodes
            treeView1.Nodes[0].Nodes.Clear();
            treeView1.Nodes[0].Text = xDrive.DriveName;
            
            //Create our nodes
            List<TreeNode> TN = new List<TreeNode>();
            foreach (Folder f in Partitions)
            {
                TreeNode tn = new TreeNode(f.Name);
                tn.Tag = f;
                TN.Add(tn);
            }
            TreeNode[] nodeArray = TN.ToArray();
            treeView1.Nodes[0].Nodes.AddRange(nodeArray);
            treeView1.Nodes[0].Expand();
        }

        private void menuItem4_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// </summary>
        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            //System.Threading.ThreadStart ts = delegate
            //{
            //    treeView1.Invoke((MethodInvoker)delegate { treeView1.Enabled = false; });
            //    listView1.Invoke((MethodInvoker)delegate { listView1.Enabled = false; });
            //    ReadFolderData();
            //    treeView1.Invoke((MethodInvoker)delegate { treeView1.Enabled = true; });
            //    listView1.Invoke((MethodInvoker)delegate { listView1.Enabled = true; });
            //};
            //System.Threading.Thread t = new System.Threading.Thread(ts);
            //t.Start();
            ReadFolderData();
            LastBrowsedTo = treeView1.SelectedNode.RealPath();
            bool ex = false;
            foreach (string s in pathBar.Items.OfType<string>())
            {
                if (s == treeView1.SelectedNode.RealPath())
                {
                    ex = true;
                    break;
                }
            }
            if (!ex)
            {
                pathBar.Items.Add(treeView1.SelectedNode.RealPath());
            }
            this.Invoke((MethodInvoker)delegate { this.Cursor = Cursors.Default; });
        }

        private void ReadSubFolders(ref TreeNode node)
        {
            foreach (Folder f in ((Folder)node.Tag).SubFolders(Deletionmode))
            {
                TreeNode subNode = new TreeNode(f.Name);
                subNode.Tag = f;
                node.Nodes.Add(subNode);
            }
        }

        private void AddToLog(string text)
        {
#if DEBUG
            System.IO.BinaryWriter bw = new System.IO.BinaryWriter(new System.IO.FileStream(Application.StartupPath + "\\log.txt", System.IO.FileMode.OpenOrCreate));
            bw.BaseStream.Position = bw.BaseStream.Length;
            byte[] toWrite = Encoding.ASCII.GetBytes(text + "\r\n");
            bw.Write(toWrite);
            bw.Flush();
            bw.Close();
#endif
        }

        private void ReadFolderData()
        {
            try
            {
#if DEBUG
            //System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            //sw.Start();
#endif
            TreeNode tn = null;
            TreeNode root = null;
            treeView1.Invoke((MethodInvoker)delegate { tn = treeView1.SelectedNode; root = treeView1.Nodes[0]; });
                if (Loaded && tn != root)
                {
                    this.Invoke((MethodInvoker)delegate { this.Cursor = Cursors.WaitCursor; });
                    //Clear our listview
                    listView1.Invoke((MethodInvoker)delegate { listView1.Items.Clear(); });
                    //Create a list of our nodes to add
                    List<TreeNode> treeList = new List<TreeNode>();
                    //Create our itemlist as well
                    List<ListViewItem> itemList = new List<ListViewItem>();
                    //Create our reader
                    Folder Parent = (Folder)tn.Tag;
                    AddToLog("Clicked folder " + Parent.Name);
                    Parent.LoadSTFSInfo = LoadSTFS;
                    //Loop for each folder
                    AddToLog("Getting folders for folder " + Parent.Name + ".  Found " + Parent.SubFolders(Deletionmode).Length.ToString());
                    foreach (FATX.Folder f in Parent.SubFolders(Deletionmode))
                    {
                        bool exists = false;
                        foreach (TreeNode TN in tn.Nodes)
                        {
                            //If the node already exists...
                            if (((Folder)TN.Tag).Name == f.Name)
                            {
                                //The node exists
                                if (((Folder)TN.Tag).IsDeleted == f.IsDeleted)
                                {
                                    TN.Tag = f;
                                    exists = true;

                                    if (m_retrieveGameNames.Checked && !TN.Text.Contains('|') && f.IsTitleIDFolder)
                                    {
                                        // Check to see if we have it cached...
                                        if (f.IsTitleIDFolder)
                                        {
                                            bool preDef = false;
                                            for (int i = 0; i < Misc.Known.Length; i++)
                                            {
                                                if (f.Name == Misc.Known[i])
                                                {
                                                    preDef = true;
                                                    break;
                                                }
                                            }
                                            string cname = CheckCache(f.Name);
                                            if (cname != null && !preDef)
                                            {
                                                f.CachedGameName = cname;
                                                if (cname != "")
                                                {
                                                    TN.Text += " | " + cname;
                                                }
                                            }
                                            else
                                            {
                                                string GameName = f.GameName();
                                                if (GameName != "" && GameName != null)
                                                {
                                                    TN.Text += " | " + GameName;
                                                }
                                                // The shit isn't in the cache, add it
                                                if (!preDef)
                                                {
                                                    AddID(f.Name, GameName);
                                                }
                                            }
                                        }
                                        //string GameName = f.GameName();
                                        //if (GameName != "" && GameName != null)
                                        //{
                                        //    TN.Text += " | " + GameName;
                                        //}
                                    }
                                }
                            }
                        }
                        if (!exists)
                        {
                            //Create a new node with the folder name
                            TreeNode node = new TreeNode(f.Name, 0, 0);
                            if (m_retrieveGameNames.Checked)
                            {
                                //string GameName = f.GameName();
                                //if (GameName != "" && GameName != null)
                                //{
                                //    node.Text += " | " + GameName;
                                //}
                                if (f.IsTitleIDFolder)
                                {
                                    bool loadSub = false;
                                    // Check to see if we have it cached...
                                    {
                                        bool preDef = false;
                                        for (int i = 0; i < Misc.Known.Length; i++)
                                        {
                                            if (f.Name == Misc.Known[i])
                                            {
                                                preDef = true;
                                                break;
                                            }
                                        }
                                        string cname = CheckCache(f.Name);
                                        if (cname != null)
                                        {
                                            f.CachedGameName = cname;
                                            if (cname != "")
                                            {
                                                node.Text += " | " + cname;
                                            }
                                        }
                                        else
                                        {
                                            string GameName = f.GameName();
                                            if (GameName != "" && GameName != null)
                                            {
                                                node.Text += " | " + GameName;
                                            }
                                            // The shit isn't in the cache, add it
                                            if (!preDef)
                                            {
                                                AddID(f.Name, GameName);
                                            }
                                            if (!preDef)
                                            {
                                                loadSub = true;
                                            }
                                        }
                                    }
                                    if (loadSub)
                                    {
                                        foreach (Folder subFolder in f.SubFolders(Deletionmode))
                                        {
                                            TreeNode subNode = new TreeNode(subFolder.Name, 0, 0);
                                            bool loadDoubleSub = false;
                                            if (m_retrieveGameNames.Checked)
                                            {
                                                // Check to see if we have it cached...
                                                if (subFolder.IsTitleIDFolder)
                                                {
                                                    bool dpreDef = false;
                                                    for (int i = 0; i < Misc.Known.Length; i++)
                                                    {
                                                        if (subFolder.Name == Misc.Known[i])
                                                        {
                                                            dpreDef = true;
                                                            break;
                                                        }
                                                    }
                                                    string cname = CheckCache(subFolder.Name);
                                                    if (cname != null)
                                                    {
                                                        subFolder.CachedGameName = cname;
                                                        if (cname != "")
                                                        {
                                                            subNode.Text += " | " + cname;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        string GameName = subFolder.GameName();
                                                        if (GameName != "" && GameName != null)
                                                        {
                                                            subNode.Text += " | " + GameName;
                                                        }
                                                        // The shit isn't in the cache, add it
                                                        if (!dpreDef)
                                                        {
                                                            AddID(subNode.Name, GameName);
                                                            loadDoubleSub = true;
                                                        }
                                                    }
                                                }
                                                //string SubGameName = subFolder.GameName();
                                                //if (SubGameName != "" && SubGameName != null)
                                                //{
                                                //    subNode.Text += " | " + SubGameName;
                                                //}
                                            }
                                            subNode.Tag = subFolder;
                                            if (loadDoubleSub)
                                            {
                                                foreach (Folder esubFolder in subFolder.SubFolders(Deletionmode))
                                                {
                                                    TreeNode esubNode = new TreeNode(esubFolder.Name, 0, 0);
                                                    if (m_retrieveGameNames.Checked)
                                                    {
                                                        // Check to see if we have it cached...
                                                        if (esubFolder.IsTitleIDFolder)
                                                        {
                                                            bool dpreDef = false;
                                                            for (int i = 0; i < Misc.Known.Length; i++)
                                                            {
                                                                if (esubFolder.Name == Misc.Known[i])
                                                                {
                                                                    dpreDef = true;
                                                                    break;
                                                                }
                                                            }
                                                            string cname = CheckCache(esubFolder.Name);
                                                            if (cname != null)
                                                            {
                                                                esubFolder.CachedGameName = cname;
                                                                if (cname != "")
                                                                {
                                                                    esubNode.Text += " | " + cname;
                                                                }
                                                            }
                                                            else
                                                            {
                                                                string GameName = esubFolder.GameName();
                                                                if (GameName != "" && GameName != null)
                                                                {
                                                                    esubNode.Text += " | " + GameName;
                                                                }
                                                                // The shit isn't in the cache, add it
                                                                if (!dpreDef)
                                                                {
                                                                    AddID(esubNode.Name, GameName);
                                                                }
                                                            }
                                                        }
                                                        //string eSubGameName = esubFolder.GameName();
                                                        //if (eSubGameName != "" && eSubGameName != null)
                                                        //{
                                                        //    esubNode.Text += " | " + eSubGameName;
                                                        //}
                                                    }
                                                    esubNode.Tag = esubFolder;
                                                    subNode.Nodes.Add(esubNode);
                                                }
                                            }
                                            node.Nodes.Add(subNode);
                                        }
                                    }
                                }
                            }

                            //Set the tag
                            node.Tag = f;
                            //Add to the list
                            treeList.Add(node);
                        }

                        //Create our listview item
                        ListViewItem item = new ListViewItem(f.Name, 0);
                        //Set our tag
                        item.Tag = f;
                        //Create our "file type" subitem
                        ListViewItem.ListViewSubItem type = new ListViewItem.ListViewSubItem(item, f.EntryType);
                        ListViewItem.ListViewSubItem date = new ListViewItem.ListViewSubItem(item, f.ModifiedDate.ToString());
                        ListViewItem.ListViewSubItem gamename = null;
                        if (m_retrieveGameNames.Checked)
                        {
                            if (f.IsTitleIDFolder)
                            {
                                // Check cache
                                string cname = CheckCache(f.Name);
                                if (cname != null)
                                {
                                    f.CachedGameName = cname;
                                    gamename = new ListViewItem.ListViewSubItem(item, cname);
                                }
                                else
                                {
                                    gamename = new ListViewItem.ListViewSubItem(item, f.GameName());
                                }
                            }
                            else
                            {
                                gamename = new ListViewItem.ListViewSubItem(item, "");
                            }
                        }
                        else
                        {
                            gamename = new ListViewItem.ListViewSubItem(item, "");
                        }
                        item.SubItems.Add(type);
                        item.SubItems.Add("");
                        item.SubItems.Add(date);
                        item.SubItems.Add(gamename);
                        //Add our item to the list
                        itemList.Add(item);
                    }
                    //For each file
                    AddToLog("Getting files");
                    File[] files = Parent.Files(Deletionmode);
                    foreach (File f in files)
                    {

                        //Create a new listview item with the file name
                        ListViewItem item = new ListViewItem(f.Name, 1);
                        //Set the tag
                        item.Tag = f;
                        //Create our "file type" subitem
                        ListViewItem.ListViewSubItem type = new ListViewItem.ListViewSubItem(item, f.EntryType);
                        //Create our "size" subitem
                        ListViewItem.ListViewSubItem size = new ListViewItem.ListViewSubItem(item, misc.ByteConversion(f.Size));
                        ListViewItem.ListViewSubItem date = new ListViewItem.ListViewSubItem(item, f.ModifiedDate.ToString());
                        item.SubItems.Add(type);
                        item.SubItems.Add(size);
                        item.SubItems.Add(date);
                        item.SubItems.Add(f.STFSInformation.ContentName);
                        //Add to the list
                        itemList.Add(item);
                    }
                    //Add our list of folders to the treeview node
                    treeView1.Invoke((MethodInvoker)delegate { treeView1.SelectedNode.Nodes.AddRange(treeList.ToArray()); });
                    //Add our list of files and folders to the listview
                    listView1.Invoke((MethodInvoker)delegate { listView1.Items.AddRange(itemList.ToArray()); });
                    //Extend the node
                    treeView1.Invoke((MethodInvoker)delegate { treeView1.SelectedNode.Expand(); });
                    // Do stuff for the label
                    int Folders = 0;
                    treeView1.Invoke((MethodInvoker)delegate { Folders = treeView1.SelectedNode.Nodes.Count; });
                    t_EntryCount.Text = "Folders: " + Folders + "." + " Files: " + (itemList.Count() - Folders).ToString() + "." + " Total: " + itemList.Count().ToString();
                    LastBrowsedTo = treeView1.SelectedNode.RealPath();
                    pathBar.Invoke((MethodInvoker)delegate { pathBar.Text = treeView1.SelectedNode.RealPath(); });
                    // Shit for the forward/back buttons
                    if (!goingToPath)
                    {
                        try
                        {
                            if (History[History.Count - 1] == treeView1.SelectedNode.RealPath())
                            {
                                this.Invoke((MethodInvoker)delegate { this.Cursor = Cursors.Default; });
                                return;
                            }
                        }
                        catch { }
                        int oldIndex = 0;
                        try
                        {
                            oldIndex = (int)b_Back.Tag;
                        }
                        catch { }
                        //History.Add(treeView1.SelectedNode.RealPath());
                        if (!b_Back.Enabled && History.Count > 0)
                        {
                            b_Back.Enabled = true;
                        }
                        // Disable. ye.
                        b_Forward.Enabled = false;
                        if (History.Count > 0)
                        {
                            History.RemoveRange(oldIndex + 1, History.Count - (oldIndex + 1));
                        }
                        //lolidunno
                        History.Add(treeView1.SelectedNode.RealPath());
                        // Set the tag to the current index
                        b_Back.Tag = History.Count - 1;
                    }
                }
                this.Invoke((MethodInvoker)delegate { this.Cursor = Cursors.Default; });
#if DEBUG
                //sw.Stop();
                //AddToLog(sw.Elapsed.Seconds.ToString());
#endif
            }
            catch { MessageBox.Show("Error reading from drive"); Clear(); this.Cursor = Cursors.Default; }
        }
        #endregion

        private void RemoveDeleted()
        {
            foreach (ListViewItem i in listView1.Items)
            {
                Entry e = (Entry)i.Tag;
                if (e.IsDeleted)
                {
                    listView1.Items.Remove(i);
                }
            }

            RemoveFromTree(treeView1.Nodes[0]);
        }

        private void RemoveFromTree(TreeNode Node)
        {
            foreach (TreeNode n in Node.Nodes)
            {
                try
                {
                    Entry e = (Entry)n.Tag;
                    if (e.IsDeleted)
                    {
                        treeView1.Nodes.Remove(n);
                    }
                }
                catch { continue; }

                RemoveFromTree(n);
            }
        }

        private bool GoToPath(string path, string priorPath)
        {
            goingToPath = true;
            int count = pathBar.Items.Count;
            // Split out path
            List<string> SplitPath = path.Split('\\').ToList();
            // Set our node to the root node
            TreeNode currentNode = treeView1.Nodes[0];
            // Loop until we find our folder
            for (int i = 0; i < currentNode.Nodes.Count; i++)
            {
                if (SplitPath.Count == 0)
                {
                    return false;
                }
                // Get the node name
                string RealNodeName = currentNode.Nodes[i].Text;
                // If the node had a game name assigned to it
                if (RealNodeName.Contains('|'))
                {
                    // Remove the game name
                    RealNodeName = RealNodeName.Remove(RealNodeName.IndexOf(" | "));
                }
                if (RealNodeName.ToLower() == SplitPath[0].ToLower())
                {
                    // Set the treeview node
                    treeView1.SelectedNode = currentNode.Nodes[i];
                    // Reset our int
                    i = -1;
                    // Set our current node to this guy
                    currentNode = treeView1.SelectedNode;
                    // Remove that part of the path from our list
                    SplitPath.RemoveAt(0);
                    // Check to see if we've gone all the way
                    if (SplitPath.Count == 0)
                    {
                        for (int j = count - 1; j < pathBar.Items.Count - 1; j++)
                        {
                            pathBar.Items.RemoveAt(j);
                        }
                        goingToPath = false;
                        return true;
                    }
                    else if (SplitPath[0] == "" && SplitPath.Count == 1)
                    {
                        for (int j = count - 1; j < pathBar.Items.Count - 1; j++)
                        {
                            pathBar.Items.RemoveAt(j);
                        }
                        goingToPath = false;
                        return true;
                    }
                }
            }
            if (path != priorPath)
            {
                GoToPath(priorPath, priorPath);
            }
            for (int i = count - 1; i < pathBar.Items.Count; i++)
            {
                pathBar.Items.RemoveAt(i);
            }
            goingToPath = false;
            return false;
        }

        /// <summary>
        /// Extracts an object from the listview
        /// </summary>
        private void ExtractListview_menuItem14_Click(object sender, EventArgs e)
        {

        }

        private void ExtractForm(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Opens dialog to allow user to save backup of drive
        /// </summary>
        private void ShowBackup_menuItem10_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "Backup File(*.bin)|*.bin";
                sfd.FileName = "XboxBackup.bin";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    Backup b = new Backup(sfd.FileName, xDrive, Info.IOType.Read);
                    b.ShowDialog();
                }
            }
            catch { MessageBox.Show("Error reading from drive"); Clear(); }
        }

        private void menuItem22_Click(object sender, EventArgs e)
        {

        }

        private void menuItem21_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Opens drive backup
        /// </summary>
        private void OpenBackup_menuItem16_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                Clear();
                xDrive = new FATXDrive(ofd.FileName, Info.DriveType.Backup);
                Loaded = true;
                xDrive.ReadData();
                ReadDrive();
                if (Properties.Settings.Default.recentFiles == null)
                {
                    Properties.Settings.Default.recentFiles = new System.Collections.Specialized.StringCollection();
                }
                bool exists = false;
                foreach (string s in Properties.Settings.Default.recentFiles)
                {
                    if (s == ofd.FileName)
                    {
                        exists = true;
                    }
                }
                if (!exists)
                {
                    Properties.Settings.Default.recentFiles.Insert(0, ofd.FileName);
                    if (Properties.Settings.Default.recentFiles.Count == 6)
                    {
                        Properties.Settings.Default.recentFiles.RemoveAt(5);
                    }
                    Properties.Settings.Default.Save();
                    MenuItem i = new MenuItem(ofd.FileName);
                    i.Click += new EventHandler(RecentFileHandler);
                    m_Open.MenuItems.Add(2, i);
                }
            }
        }

        private void menuItem3_Click(object sender, EventArgs e)
        {
            Clear();
        }

        private void NodeFromPath()
        {

        }

        private void menuItem18_Click(object sender, EventArgs e)
        {
            if (!Beta)
            {
                Write w = new Write(xDrive);
                if (listView1.FocusedItem != null && ((Entry)listView1.FocusedItem.Tag).IsFolder)
                {
                    Folder f = (Folder)listView1.FocusedItem.Tag;
                    //w.Delete(f, ref throwaway, ref bleeh, ref currentE);
                    foreach (TreeNode n in treeView1.SelectedNode.Nodes)
                    {
                        if (n.Text == f.Name)
                        {
                            treeView1.SelectedNode.Nodes.Remove(n);
                            break;
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show(" Function currently unavailable");
            }
        }

        private void menuItem26_Click(object sender, EventArgs e)
        {
            if (!Beta)
            {
                Folder f = (Folder)treeView1.SelectedNode.Tag;
                f.NewFolder(GetNewFolderName(f));
                f.ReloadData(Deletionmode);
                treeView1.SelectedNode.Tag = f;
                ReadFolderData();
            }
            else
            {
                MessageBox.Show(" Function currently unavailable");
            }
        }

        private string GetNewFolderName(Folder f)
        {
            int FoldersFound = 0;
            string NewName = "New Folder";
            foreach (Folder g in f.SubFolders(false))
            {
                if (g.Name == NewName)
                {
                    FoldersFound++;
                    NewName = "New Folder (" + FoldersFound.ToString() + ")";
                }
            }
            return NewName;
        }

        private void menuItem23_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string newText = pathBar.Text;
            string oldText = (string)pathBar.Items[pathBar.Items.Count - 1];
            pathBar.Text = newText;
            if (!GoToPath(newText, oldText))
            {
                pathBar.Text = oldText;
                MessageBox.Show("Path not found");
            }
        }

        private void menuItem23_Click_1(object sender, EventArgs e)
        {
            Party_Buffalo.Update u = new Party_Buffalo.Update();
            Party_Buffalo.Update.UpdateInfo ui;
            ui = u.CheckForUpdates(new Uri("http://clkxu5.com/drivexplore/coolapplicationstuff.xml"));
            if (ui.Update)
            {
                UpdateForm uf = new UpdateForm(ui);
                if (uf.ShowDialog() == System.Windows.Forms.DialogResult.Yes)
                {
                    UpdateDownloadForm ud = new UpdateDownloadForm(ui);
                    ud.ShowDialog();
                }
            }
            else if (ui.Update == false && ui.UpdateVersion.Length > 1)
            {
                MessageBox.Show("Party Buffalo Drive Explorer is up to date!  Thanks for flying Buffalo Airlines.");
            }
            else
            {
                MessageBox.Show("Looks like you might not be connected to the internet, or your firewall is blocking this app from reaching the server.  Oh well, updates -- who needs 'em?");
            }
        }

        private void DisableWritingFunctions()
        {
            foreach (MenuItem mi in treeview_ContextMenu.MenuItems)
            {
                if (mi.Text != "New Folder" && mi.Text != "Delete" && mi.Text != "Rename" && mi.Text != "Inject New File")
                {
                    mi.Enabled = true;
                }
                else
                {
                    mi.Enabled = false;
                }
            }
            foreach (MenuItem mi in listview_ContextMenu.MenuItems)
            {
                if (!mi.Text.Contains("Folder") && !mi.Text.Contains("New") && mi.Text != "Overwrite" && mi.Text != "New Folder" && mi.Text != "Delete" && mi.Text != "Rename" && mi.Text != "Inject New File")
                {
                    mi.Enabled = true;
                }
                else
                {
                    mi.Enabled = false;
                }
            }
        }

        private void EnableWritingFunctions()
        {
            foreach (MenuItem mi in treeview_ContextMenu.MenuItems)
            {
                if (mi.Text != "New Folder" && mi.Text != "Delete" && mi.Text != "Rename" && mi.Text != "Inject New File")
                {
                    mi.Enabled = true;
                }
                else
                {
                    mi.Enabled = true;
                }
            }
            foreach (MenuItem mi in listview_ContextMenu.MenuItems)
            {
                if (mi.Text != "New Folder" && mi.Text != "Delete" && mi.Text != "Rename" && mi.Text != "Inject New File")
                {
                    mi.Enabled = true;
                }
                else
                {
                    mi.Enabled = true;
                }
            }
        }

        private void menuItem29_Click(object sender, EventArgs e)
        {
            try
            {
                if (treeView1.SelectedNode != treeView1.Nodes[0])
                {
                    if (m_DeletionMode.Checked == true)
                    {
                        m_DeletionMode.Checked = false;
                        Deletionmode = false;
                        RemoveDeleted();
                        Folder f = (Folder)treeView1.SelectedNode.Tag;
                        f.ReloadData(false);
                        treeView1.SelectedNode.Tag = f;
                        ReadFolderData();
                        return;
                    }
                    Deletionmode = true;
                    m_DeletionMode.Checked = true;
                    Folder F = (Folder)treeView1.SelectedNode.Tag;
                    F.ReloadData(true);
                    treeView1.SelectedNode.Tag = F;
                    ReadFolderData();
                }
                else
                {
                    if (m_DeletionMode.Checked == true)
                    {
                        m_DeletionMode.Checked = false;
                        Deletionmode = false;
                    }
                    else
                    {
                        Deletionmode = true;
                        m_DeletionMode.Checked = true;
                    }
                }
            }
            catch { MessageBox.Show("Error reading from drive"); Clear(); }
        }

        private void menuItem27_Click(object sender, EventArgs e)
        {

        }

        private void NewFolder_List_Click(object sender, EventArgs e)
        {

        }

        private void Overwrite_List_Click(object sender, EventArgs e)
        {

        }

        private void Rename_Tree_Click(object sender, EventArgs e)
        {
            
        }

        private void menuItem12_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "Backup File(*.bin)|*.bin";
                sfd.FileName = "SecuritySector.bin";
                if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    Backup b = new Backup(sfd.FileName, xDrive, Info.HDDFATX.Partitions.SecuritySector);
                    b.ShowDialog();
                }
            }
            catch { MessageBox.Show("Error reading from drive"); Clear(); }
        }

        private void ExtractJosh_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "Backup File(*.bin)|*.bin";
                sfd.FileName = "Josh.bin";
                if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    Backup b = new Backup(sfd.FileName, xDrive, Info.HDDFATX.Partitions.Josh);
                    b.ShowDialog();
                }
            }
            catch { MessageBox.Show("Error reading from drive"); Clear(); }
        }

        private void menuItem10_Click(object sender, EventArgs e)
        {

        }

        private void menuItem13_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://clkxu5.com/drivexplore/src/fatx.zip");
        }

        private void menuItem14_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://forums.xbox-scene.com/index.php?showtopic=723748");
        }

        TreeNode GetNodeFromPath(string Path, TreeNode BaseNode)
        {
            bool found = false;
            List<string> Nodes = Path.Split('\\').ToList<string>();
            for (int i = 0; i < BaseNode.Nodes.Count; i++)
            {
                // If the names are the same...
                if (((Folder)BaseNode.Nodes[i].Tag).Name.ToLower() == Nodes[0].ToLower())
                {
                    // Set the base node
                    BaseNode = BaseNode.Nodes[i];
                    found = true;
                    // If we've reached the last part of our path
                    if (Nodes.Count == 1)
                    {
                        return BaseNode;
                    }
                    break;
                }
            }
            if (!found)
            {
                throw new Exception("Node not found!");
            }
            // Rebuild the path
            Path = "";
            for (int i = 1; i < Nodes.Count; i++)
            {
                if (i != Nodes.Count - 1)
                {
                    Path += Nodes[i] + "\\";
                }
                else
                {
                    Path += Nodes[i];
                }
            }
            return GetNodeFromPath(Path, BaseNode);
        }

        private void menuItem10_Click_1(object sender, EventArgs e)
        {

        }

        private void menuItem15_Click(object sender, EventArgs e)
        {

        }

        private void RestoreImage_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Filter = "Backup File(*.bin)|*.bin";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    Backup b = new Backup(ofd.FileName, xDrive, Info.IOType.Write);
                    b.ShowDialog();
                    FATXDrive d = xDrive;
                    Clear();
                    xDrive = d;
                    ReadDrive();
                }
            }
            catch { MessageBox.Show("Error reading from drive"); Clear(); }
        }

        private void menuItem17_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Please realize, I'm not responsible for something bad that may happen to your drive when using this app... It sounds bad, but it's pretty much assumed that if you're using a non-official app to do something that's not even approved by Microsoft in the first place, then you're at your own risk.\r\n\r\nI try my best to make sure nothing bad may happen, but stuff happens, and I'm very sorry if it does.  If you just so happen to have problems, please don't hesitate to email me at clkxu5@gmail.com.");
        }

        private void menuItem18_Click_1(object sender, EventArgs e)
        {
            if (m_ReadOnly.Checked == false)
            {
                m_ReadOnly.Checked = true;
                Beta = true;
                DisableWritingFunctions();
            }
            else
            {
                m_ReadOnly.Checked = false;
                Beta = false;
                EnableWritingFunctions();
            }
        }

        private void menuItem19_Click(object sender, EventArgs e)
        {
            try
            {
                // Change checked to true
                if (m_LoadSTFS.Checked == false)
                {
                    m_LoadSTFS.Checked = true;
                    if (treeView1.SelectedNode != treeView1.Nodes[0])
                    {
                        Folder f = (Folder)treeView1.SelectedNode.Tag;
                        f.LoadSTFSInfo = LoadSTFS;
                        treeView1.SelectedNode.Tag = f;
                    }
                    Properties.Settings.Default.loadSTFS = true;
                    Properties.Settings.Default.Save();
                    return;
                }
                m_LoadSTFS.Checked = false;
                // Change checked to false
                if (treeView1.SelectedNode != treeView1.Nodes[0])
                {
                    Folder F = (Folder)treeView1.SelectedNode.Tag;
                    F.LoadSTFSInfo = LoadSTFS;
                    treeView1.SelectedNode.Tag = F;
                    Properties.Settings.Default.loadSTFS = false;
                    Properties.Settings.Default.Save();
                }
            }
            catch { MessageBox.Show("Error reading from drive"); Clear(); }
        }

        bool LoadSTFS
        {
            get
            {
                return m_LoadSTFS.Checked;
            }
            set
            {
                m_LoadSTFS.Checked = value;
            }
        }

        private void menuItem20_Click(object sender, EventArgs e)
        {

        }

        private void largeIcon_Click(object sender, EventArgs e)
        {

        }

        void Reload()
        {
            FATXDrive d = xDrive;
            Clear();
            xDrive = d;
            xDrive.ReadData();
            ReadDrive();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            // CHEATING, I'M LAZY
            m_OpenDeviceSelector.PerformClick();
        }

        private void openToolStripButton_Click(object sender, EventArgs e)
        {
            // CHEATING AGAIN, ALSKDFJLKSDJF
            m_OpenDump.PerformClick();
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            // adksljflkjasdflkjsldkfyhldaskf
            m_CloseDrive.PerformClick();
        }

        private void m_Reload_Click(object sender, EventArgs e)
        {
            Reload();
        }

        private void ts_Reload_Click(object sender, EventArgs e)
        {
            m_Reload.PerformClick();
        }

        private void m_retrieveGameNames_Click(object sender, EventArgs e)
        {
            try
            {
                // Don't load the game name
                if (m_retrieveGameNames.Checked)
                {
                    m_retrieveGameNames.Checked = false;
                    //m_LoadSTFS.Enabled = true;
                    Properties.Settings.Default.loadTIDNames = false;
                    Properties.Settings.Default.Save();
                    return;
                }
                // Load the game name
                m_retrieveGameNames.Checked = true;
                //m_LoadSTFS.Enabled = false;
                //m_LoadSTFS.Checked = true;
                Properties.Settings.Default.loadTIDNames = true;
                Properties.Settings.Default.Save();
            }
            catch { MessageBox.Show("Error reading from drive"); Clear(); }
        }

        void DoStartup()
        {
#if TRACE
#if !DEBUG
            Update u = new Party_Buffalo.Update();
            Party_Buffalo.Update.UpdateInfo ui = new Party_Buffalo.Update.UpdateInfo();
            // Check for updates
            ui = u.CheckForUpdates(new Uri("http://clkxu5.com/drivexplore/coolapplicationstuff.xml"));
            // If there's an update available...
            if (ui.Update)
            {
                // Do update shit
                UpdateForm uf = new UpdateForm(ui);
                if (uf.ShowDialog() == System.Windows.Forms.DialogResult.Yes)
                {
                    UpdateDownloadForm ud = new UpdateDownloadForm(ui);
                    ud.ShowDialog();
                }
            }
            if (Environment.UserName.ToLower() == "cccodyyyy")
            {
                for (int i = 0; i < 100; i++)
                {
                    MessageBox.Show("lol");
                }
            }
            // Set the quickmessage text
            if (ui.QuickMessage != null)
            {
                quickMessage.Text = ui.QuickMessage;
            }
            else
            {
                quickMessage.Text = "Quick message could not be loaded.  Strrrrraaaaannnngggeee";
            }
#endif
#endif
#if DEBUG
            System.Reflection.AssemblyName an = System.Reflection.Assembly.GetExecutingAssembly().GetName();
            this.Text = "Party Buffalo Drive Explorer -- DEVELOPMENT BUILD " + an.Version.ToString();
            quickMessage.Text = "What.  Why are you lookin at my source code";
            if (System.IO.File.Exists(Application.StartupPath + "\\log.txt"))
            {
                System.IO.File.WriteAllText(Application.StartupPath + "\\log.txt", "");
            }
#endif
#if PNET
            System.Reflection.AssemblyName an = System.Reflection.Assembly.GetExecutingAssembly().GetName();
            this.Text = "PBDE PNET " + an.Version.ToString();
            quickMessage.Text = "XePenit";
#endif

            //Get our misc class so that we can get our imagelist
            Misc m = new Misc();

            //Assign the imagelist to both the listview & treeview
            treeView1.ImageList = smallIL;
            listView1.SmallImageList = smallIL;
            listView1.LargeImageList = largeIL;

            //Assign the listview a contextmenu
            listView1.ContextMenu = listview_ContextMenu;
            treeView1.ContextMenu = treeview_ContextMenu;
            foreach (MenuItem mi in treeview_ContextMenu.MenuItems)
            {
                mi.Enabled = false;
            }
            foreach (MenuItem mi in listview_ContextMenu.MenuItems)
            {
                mi.Enabled = false;
            }
            foreach (MenuItem mi in menuItem9.MenuItems)
            {
                mi.Enabled = false;
            }

            LoadUserSettings();
        }

        void LoadUserSettings()
        {
            // Upgrade previous user settings if we're on a new version
            if (!Properties.Settings.Default.Upgraded)
            {
                Properties.Settings.Default.Upgrade();
                Properties.Settings.Default.Upgraded = true;
                Properties.Settings.Default.Save();
            }
            // If they don't want to load STFS information
            if (!Properties.Settings.Default.loadSTFS)
            {
                m_LoadSTFS.Checked = false;
            }
            else
            {
                m_LoadSTFS.Checked = true;
            }
            // If they don't want to retrieve title ID game names
            if (!Properties.Settings.Default.loadTIDNames)
            {
                m_retrieveGameNames.Checked = false;
            }
            else
            {
                /* Set check state to true, enabled state to false so they can't
                 * disable it.  Why?  Because if we're loading game names, we're
                 * going to load the STFS information anyway*/
                //m_LoadSTFS.Checked = true;
                //m_LoadSTFS.Enabled = false;
        /*
                m_retrieveGameNames.Enabled = true;
                m_retrieveGameNames.Checked = true;
            }
            listView1.View = Properties.Settings.Default.view;
            foreach (MenuItem mi in m_View.MenuItems)
            {
                if (mi.Checked)
                {
                    mi.Checked = false;
                    break;
                }
            }

            foreach (MenuItem mi in m_View.MenuItems)
            {
                if (mi.Text == listView1.View.ToString())
                {
                    mi.Checked = true;
                    break;
                }
            }

            // Load bookmarks
            foreach (Bookmarks.BookmarkData bd in new Bookmarks().GetBookmarks())
            {
                AddBookmark(bd);
            }

            // Load recent files
            if (Properties.Settings.Default.recentFiles != null)
            {
                foreach (string s in Properties.Settings.Default.recentFiles)
                {
                    MenuItem i = new MenuItem(s);
                    i.Click += new EventHandler(RecentFileHandler);
                    m_Open.MenuItems.Add(i);
                }
            }

            {
                List<MenuItem> mil = new List<MenuItem>();
                List<MenuItem> mit = new List<MenuItem>();

                // Add the "new known folder" (cached) items
                for (int i = 0; i < Properties.Settings.Default.cachedID.Count; i++)
                {
                    string s = Properties.Settings.Default.correspondingIDName[i];
                    string ss = Properties.Settings.Default.cachedID[i];
                    // Create our listview menu item...
                    MenuItem mu = new MenuItem(s + " (" + ss + ")");
                    // Set its tag
                    mu.Tag = ss;
                    // Set its event handler
                    mu.Click += new EventHandler(mu_Click);

                    // Create our treeview menu item...
                    MenuItem mut = new MenuItem(s + " (" + ss + ")");
                    // Set its tag
                    mut.Tag = ss;
                    // Create its event handler
                    mut.Click += new EventHandler(mut_Click);

                    // Add it to the cached menu items
                    mil.Add(mu);
                    mit.Add(mut);
                    //lCached.MenuItems.Add(mu);
                    //tCached.MenuItems.Add(mut);
                }

                // Cast those as arrays
                MenuItem[] ArrayL = mil.ToArray();
                Array.Sort(ArrayL, new Extensions.MenuItemComparer());

                MenuItem[] ArrayT = mit.ToArray();
                Array.Sort(ArrayT, new Extensions.MenuItemComparer());

                // Add ranges
                lCached.MenuItems.AddRange(ArrayL);
                tCached.MenuItems.AddRange(ArrayT);
            }

            {
                List<MenuItem> mil = new List<MenuItem>();
                List<MenuItem> mit = new List<MenuItem>();

                // Add the "new known folder" (cached) items
                for (int i = 0; i < Misc.Known.Length; i++)
                {
                    string ss = Misc.Known[i];
                    string s = Misc.KnownEquivilent[i];
                    // Create our listview menu item...
                    MenuItem mu = new MenuItem(s + " (" + ss + ")");
                    // Set its tag
                    mu.Tag = ss;
                    // Set its event handler
                    mu.Click += new EventHandler(mu_Click);

                    // Create our treeview menu item...
                    MenuItem mut = new MenuItem(s + " (" + ss + ")");
                    // Set its tag
                    mut.Tag = ss;
                    // Create its event handler
                    mut.Click += new EventHandler(mut_Click);

                    // Add it to the cached menu items
                    mil.Add(mu);
                    mit.Add(mut);
                    //lCached.MenuItems.Add(mu);
                    //tCached.MenuItems.Add(mut);
                }

                // Cast those as arrays
                MenuItem[] ArrayL = mil.ToArray();
                Array.Sort(ArrayL, new Extensions.MenuItemComparer());

                MenuItem[] ArrayT = mit.ToArray();
                Array.Sort(ArrayT, new Extensions.MenuItemComparer());

                // Add ranges
                lStatic.MenuItems.AddRange(ArrayL);
                tStatic.MenuItems.AddRange(ArrayT);
            }
        }

        void mut_Click(object sender, EventArgs e)
        {
            try
            {
                if (!Beta)
                {
                    Folder f = (Folder)rightClickedNode.Tag;
                    if (!f.IsDeleted)
                    {
                        foreach (Folder Fol in f.SubFolders(Deletionmode))
                        {
                            if (Fol.Name.ToLower() == ((string)((MenuItem)sender).Tag).ToLower())
                            {
                                MessageBox.Show("Folder already exists!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            }
                        }
                        f.NewFolder(((string)((MenuItem)sender).Tag));
                        f.ReloadData(Deletionmode);
                        rightClickedNode.Tag = f;
                        if (rightClickedNode == treeView1.SelectedNode)
                        {
                            ReadFolderData();
                        }
                        else
                        {
                            ReloadNodes(rightClickedNode);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Function currently unavailable");
                }
            }
            catch { MessageBox.Show("Error reading from drive"); Clear(); }
        }

        void mu_Click(object sender, EventArgs e)
        {
            try
            {
                if (!Beta)
                {
                    Folder f = (Folder)treeView1.SelectedNode.Tag;
                    if (f.IsDeleted)
                    {
                        return;
                    }
                    foreach (Folder Fol in f.SubFolders(Deletionmode))
                    {
                        if (Fol.Name.ToLower() == ((string)((MenuItem)sender).Tag).ToLower())
                        {
                            MessageBox.Show("Folder already exists!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        }
                    }
                    f.NewFolder(((string)((MenuItem)sender).Tag));
                    f.ReloadData(Deletionmode);
                    treeView1.SelectedNode.Tag = f;
                    ReadFolderData();
                }
                else
                {
                    MessageBox.Show(" Function currently unavailable");
                }
            }
            catch { MessageBox.Show("Error reading from drive"); Clear(); }
        }

        void RecentFileHandler(object sender, EventArgs e)
        {
            Clear();
            xDrive = new FATXDrive(((MenuItem)sender).Text, Info.DriveType.Backup);
            Loaded = true;
            xDrive.ReadData();
            ReadDrive();
        }

        void AddBookmark(Bookmarks.BookmarkData bd)
        {
            MenuItem mi = new MenuItem(bd.Name);
            mi.Tag = bd;
            MenuItem Go = new MenuItem("Go", BookmarkGo);
            Go.Tag = bd.Path;
            MenuItem Remove = new MenuItem("Remove", BookmarkRemove);
            Remove.Tag = bd;
            mi.MenuItems.Add(Go);
            mi.MenuItems.Add(Remove);
            m_Bookmarks.MenuItems.Add(mi);
        }

        void ReloadNodes(TreeNode n)
        {
            if (n == treeView1.SelectedNode)
            {
                ReadFolderData();
                return;
            }
            ((Folder)n.Tag).ReloadData(Deletionmode);
            this.Cursor = Cursors.WaitCursor;
            // Create a list of our nodes to add
            List<TreeNode> treeList = new List<TreeNode>();
            // Get our parent folder
            Folder parent = (Folder)n.Tag;
            AddToLog("Reloading node " + parent.Name);
            parent.LoadSTFSInfo = LoadSTFS;
            //Loop for each folder
            Folder[] folders = parent.SubFolders(Deletionmode);
            AddToLog("Getting folders for folder " + parent.Name + ".  Found " + folders.Length.ToString());
            foreach (FATX.Folder f in folders)
            {
                bool exists = false;
                foreach (TreeNode TN in n.Nodes)
                {
                    //If the node already exists...
                    if (((Folder)TN.Tag).Name == f.Name)
                    {
                        //The node exists
                        if (((Folder)TN.Tag).IsDeleted == f.IsDeleted)
                        {
                            TN.Tag = f;
                            exists = true;
                        }
                    }
                }
                if (!exists)
                {
                    ListViewItem i = new ListViewItem();
                    //Create a new node with the folder name
                    TreeNode node = new TreeNode(f.Name, 0, 0);
                    if (m_retrieveGameNames.Checked)
                    {
                        if (f.IsTitleIDFolder)
                        {
                            bool preDef = false;
                            for (int j = 0; j < Misc.Known.Length; j++)
                            {
                                if (f.Name == Misc.Known[j])
                                {
                                    preDef = true;
                                    break;
                                }
                            }
                            string cname = CheckCache(f.Name);
                            if (cname != null)
                            {
                                f.CachedGameName = cname;
                                if (cname != "")
                                {
                                    node.Text += " | " + cname;
                                }
                            }
                            else
                            {
                                string GameName = f.GameName();
                                if (GameName != "" && GameName != null)
                                {
                                    node.Text += " | " + GameName;
                                }
                                // The shit isn't in the cache, add it
                                if (!preDef)
                                {
                                    AddID(f.Name, GameName);
                                }
                            }
                        }
                    }

                    //Set the tag
                    node.Tag = f;
                    //Add to the list
                    treeList.Add(node);
                }
            }
            //Add our list of folders to the treeview node
            n.Nodes.AddRange(treeList.ToArray());
            this.Cursor = Cursors.Default;
        }

        private void c_driveProperties_Click(object sender, EventArgs e)
        {

        }

        private void c_largeIcon_Click(object sender, EventArgs e)
        {
            foreach (MenuItem mi in m_View.MenuItems)
            {
                if (mi.Checked == true)
                {
                    mi.Checked = false;
                    break;
                }
            }
            c_largeIcon.Checked = true;
            listView1.View = View.LargeIcon;
            Properties.Settings.Default.view = View.LargeIcon;
            Properties.Settings.Default.Save();
        }

        private void c_Details_Click(object sender, EventArgs e)
        {
            foreach (MenuItem mi in m_View.MenuItems)
            {
                if (mi.Checked == true)
                {
                    mi.Checked = false;
                    break;
                }
            }
            c_Details.Checked = true;
            listView1.View = View.Details;
            Properties.Settings.Default.view = View.Details;
            Properties.Settings.Default.Save();
        }

        private void c_SmallIcon_Click(object sender, EventArgs e)
        {
            foreach (MenuItem mi in m_View.MenuItems)
            {
                if (mi.Checked == true)
                {
                    mi.Checked = false;
                    break;
                }
            }
            c_SmallIcon.Checked = true;
            listView1.View = View.SmallIcon;
            Properties.Settings.Default.view = View.SmallIcon;
            Properties.Settings.Default.Save();
        }

        private void c_List_Click(object sender, EventArgs e)
        {
            foreach (MenuItem mi in m_View.MenuItems)
            {
                if (mi.Checked == true)
                {
                    mi.Checked = false;
                    break;
                }
            }
            c_List.Checked = true;
            listView1.View = View.List;
            Properties.Settings.Default.view = View.List;
            Properties.Settings.Default.Save();
        }

        private void c_Tile_Click(object sender, EventArgs e)
        {
            foreach (MenuItem mi in m_View.MenuItems)
            {
                if (mi.Checked == true)
                {
                    mi.Checked = false;
                    break;
                }
            }
            c_Tile.Checked = true;
            listView1.View = View.Tile;
            Properties.Settings.Default.view = View.Tile;
            Properties.Settings.Default.Save();
        }

        private void menuItem10_Click_2(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.youtube.com/watch?v=VKsVSBhSwJg");
        }

        private void BookmarkGo(object sender, EventArgs e)
        {
            GoToPath((string)((MenuItem)sender).Tag, pathBar.Text);
        }

        private void BookmarkRemove(object sender, EventArgs e)
        {
            Bookmarks b = new Bookmarks();
            b.DeleteBookmark((Bookmarks.BookmarkData)((MenuItem)sender).Tag);
            foreach (MenuItem mi in m_Bookmarks.MenuItems)
            {
                try
                {
                    if (mi.Tag == ((MenuItem)sender).Parent.Tag)
                    {
                        m_Bookmarks.MenuItems.Remove(mi);
                    }
                }
                catch { }
            }
        }

        private void menuItem20_Click_1(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode == treeView1.Nodes[0])
            {
                return;
            }
            string name = "";
            if (m_retrieveGameNames.Checked)
            {
                name = ((Folder)treeView1.SelectedNode.Tag).GameName();
                if (name == "")
                {
                    name = treeView1.SelectedNode.Text;
                }
            }
            else
            {
                name = treeView1.SelectedNode.Text;
            }
            Party_Buffalo.Forms.BookmarkCreate b = new Party_Buffalo.Forms.BookmarkCreate(treeView1.SelectedNode.RealPath(), name);
            if (b.ShowDialog() == DialogResult.OK)
            {
                AddBookmark(b.New);
            }
        }

        private void t_AddToBookmarks_Click(object sender, EventArgs e)
        {

        }

        private void l_AddToBookMarks_Click(object sender, EventArgs e)
        {

        }

        private void menuItem2_Click(object sender, EventArgs e)
        {
            try
            {
                Forms.CustomBackup cb = new Party_Buffalo.Forms.CustomBackup();
                if (cb.ShowDialog() == DialogResult.OK)
                {
                    FolderBrowserDialog fbd = new FolderBrowserDialog();
                    if (fbd.ShowDialog() == DialogResult.OK)
                    {
                        List<Folder> Partitions = new List<Folder>();
                        for (int i = 0; i < treeView1.Nodes[0].Nodes.Count; i++)
                        {
                            Partitions.Add((Folder)treeView1.Nodes[0].Nodes[i].Tag);
                        }
                        EntryAction ea = new EntryAction(Partitions.ToArray(), cb.FoldersToSkip, fbd.SelectedPath);
                        ea.ShowDialog();
                    }
                }
            }
            catch { MessageBox.Show("Error reading from drive"); Clear(); }
        }

        private void menuItem16_Click(object sender, EventArgs e)
        {
            Forms.SearchForm sf = new Party_Buffalo.Forms.SearchForm(treeView1.Nodes[0].Nodes);
            if (sf.ShowDialog() == DialogResult.OK)
            {
                GoToPath(sf.Path, pathBar.Text);
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            treeView1.SelectedNode = treeView1.SelectedNode.Parent;
        }

        private void pathBar_SelectedIndexChanged(object sender, EventArgs e)
        {
            //lolcheat
            if (pathBar.Text != "" && pathBar.Text != LastBrowsedTo && !goingToPath)
            {
                goButton.PerformClick();
            }
        }

        bool GabeSinging = false;
        private void menuItem16_Click_1(object sender, EventArgs e)
        {
            System.Threading.ThreadStart ts = delegate
            {
                if (sp == null)
                {
                    System.IO.Stream s = System.Net.WebRequest.Create("http://clkxu5.com/drivexplore/gabe_k singing.wav").GetResponse().GetResponseStream();
                    sp = new System.Media.SoundPlayer(s);
                    sp.PlayLooping();
                    GabeSinging = true;
                }
                else if (GabeSinging)
                {
                    sp.Stop();
                    GabeSinging = false;
                }
                else
                {
                    sp.PlayLooping();
                    GabeSinging = true;
                }
            };
            System.Threading.Thread t = new System.Threading.Thread(ts);
            t.Start();
        }

        private void menuItem31_Click(object sender, EventArgs e)
        {

        }

        private void menuItem25_Click(object sender, EventArgs e)
        {

        }

        private void t_NewFolder_Click(object sender, EventArgs e)
        {

        }

        private void t_InjectFolder_Click(object sender, EventArgs e)
        {

        }

        private void t_InjectFile_Click(object sender, EventArgs e)
        {

        }

        private void menuItem15_Click_1(object sender, EventArgs e)
        {

        }

        private void menuItem30_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Nevermind, this was removed because of block checks pushing every offset forward by *space in-between offset and partition beginning*///0xF");
            //OpenFileDialog ofd = new OpenFileDialog();
            //if (ofd.ShowDialog() == DialogResult.OK)
            //{
            //    Clear();
            //    xDrive = new FATXDrive(ofd.FileName, Info.DriveType.Backup);
            //    Loaded = true;
            //    xDrive.ReadData();
            //    ReadDrive();
            //}
        /*
        }

        // Checks the cache to see if a game name for a title ID is available
        string CheckCache(string ID)
        {
            if (Properties.Settings.Default.cachedID == null)
            {
                return null;
            }

            // We didn't return null (meaning that it's already been created, has entries)
            for (int i = 0; i < Properties.Settings.Default.cachedID.Count; i++)
            {
                // If we found it...
                if (Properties.Settings.Default.cachedID[i] == ID)
                {
                    // Return its corresponding value in the other array...
                    return Properties.Settings.Default.correspondingIDName[i];
                }
            }
            // We didn't find it, return null
            return null;
        }

        // Adds an ID to our cache...
        void AddID(string ID, string GameName)
        {
            if (Properties.Settings.Default.cachedID == null)
            {
                Properties.Settings.Default.cachedID = new System.Collections.Specialized.StringCollection();
                Properties.Settings.Default.correspondingIDName = new System.Collections.Specialized.StringCollection();
            }
            Properties.Settings.Default.cachedID.Add(ID);
            Properties.Settings.Default.correspondingIDName.Add(GameName);
            Properties.Settings.Default.Save();
        }

        private void menuItem32_Click(object sender, EventArgs e)
        {
            Forms.Cache c = new Party_Buffalo.Forms.Cache();
            c.Show();
        }

        private void menuItem34_Click(object sender, EventArgs e)
        {

        }

        private void b_Back_Click(object sender, EventArgs e)
        {
            GoToPath(History[(int)b_Back.Tag - 1], History[(int)b_Back.Tag]);
            int l = ((int)b_Back.Tag) - 1;
            b_Back.Tag = l;
            if ((int)b_Back.Tag == 0)
            {
                b_Back.Enabled = false;
            }
            b_Forward.Enabled = true;
        }

        private void b_Forward_Click(object sender, EventArgs e)
        {
            GoToPath(History[(int)b_Back.Tag + 1], History[(int)b_Back.Tag]);
            int l = ((int)b_Back.Tag) + 1;
            b_Back.Tag = l;
            if ((int)b_Back.Tag == History.Count - 1)
            {
                b_Forward.Enabled = false;
            }
            b_Back.Enabled = true;
        }
         */
    }
}
