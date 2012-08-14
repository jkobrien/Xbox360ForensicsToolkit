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
using Microsoft.WindowsAPICodePack.Controls;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace Party_Buffalo
{
    public partial class Main : Form
    {

        string CurrentPath;
        ListViewColumnSorter lvwColumnSorter = new ListViewColumnSorter();
        ImageList siL;
        ImageList liL;
        TreeNode rightClickedNode;
        bool Aero;
        List<string> BrowsedPaths = new List<string>();
        int CurrentPathIndex;
        bool DirectionButtonPressed;
        bool UpdateDebug = false;
        bool UpdateDebugForce = true;
        System.IO.FileSystemWatcher[] Watchers;

        public Main()
        {
            InitializeComponent();
            if (Environment.OSVersion.Version.Build >= 6000)
            {
                Aero = true;
            }
            listView1.SetExplorerTheme();
            treeView1.SetExplorerTheme();
            pathBar.SetExplorerTheme();
        }

        private void Main_Load(object sender, EventArgs e)
        {
            siL = SmallListForFATX;
            liL = LargeListForFATX;

            #region TreeView Shit
            treeView1.ImageList = siL;
            treeView1.MouseDown += new MouseEventHandler(treeView1_MouseDown);
            treeView1.ContextMenu = treeview_ContextMenu;
            treeview_ContextMenu.Popup += new EventHandler(treeview_ContextMenu_Popup);
            treeView1.BeforeLabelEdit += new NodeLabelEditEventHandler(treeView1_BeforeLabelEdit);
            treeView1.AfterLabelEdit += new NodeLabelEditEventHandler(treeView1_AfterLabelEdit);
            #endregion

            #region ListView Shit
            listView1.SmallImageList = siL;
            listView1.LargeImageList = liL;
            listView1.ContextMenu = listview_ContextMenu;
            listView1.DoubleClick += new EventHandler(listView1_DoubleClick);
            listView1.ColumnClick += new ColumnClickEventHandler(listView1_ColumnClick);
            listView1.KeyDown += new KeyEventHandler(listView1_KeyDown);
            listView1.ListViewItemSorter = lvwColumnSorter;
            listView1.BeforeLabelEdit += new LabelEditEventHandler(listView1_BeforeLabelEdit);
            listView1.AfterLabelEdit += new LabelEditEventHandler(listView1_AfterLabelEdit);
            listView1.DragDrop += new DragEventHandler(listView1_DragDrop);
            listView1.DragOver += new DragEventHandler(listView1_DragOver);
            listView1.MouseDown += new MouseEventHandler(listView1_MouseDown);
            listView1.MouseUp += new MouseEventHandler(listView1_MouseUp);
            listView1.KeyUp += new KeyEventHandler(listView1_KeyUp);
            listView1.KeyPress += new KeyPressEventHandler(listView1_KeyPress);
            listview_ContextMenu.Popup += new EventHandler(listview_ContextMenu_Popup);
            listView1.ItemDrag += new ItemDragEventHandler(listView1_ItemDrag);
            listView1.DragEnter += new DragEventHandler(listView1_DragEnter);
            #endregion

            this.Resize += new EventHandler(Main_Resize);
            this.Shown += new EventHandler(Main_Shown);
            this.FormClosing += new FormClosingEventHandler(Main_FormClosing);

            // Initialize our drive watchers
            List<System.IO.FileSystemWatcher> fsL = new List<System.IO.FileSystemWatcher>();
            foreach (string s in Environment.GetLogicalDrives())
            {
                try
                {
                    System.IO.FileSystemWatcher Watcher = new System.IO.FileSystemWatcher(s);
                    // Set the file created event
                    Watcher.Created += new System.IO.FileSystemEventHandler(Watcher_Created);
                    fsL.Add(Watcher);
                }
                catch { continue; }
            }
            Watchers = fsL.ToArray();

            DoStartup();
        }

        void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            Environment.Exit(0);
        }

        void Main_Shown(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.loadLatestNews)
            {
                #region Latest News

#if TRACE
                bool Show = false;
                if (Properties.Settings.Default.latestNewsLastShown.Date.Month != DateTime.Now.Date.Month)
                {
                    Show = true;
                }
                else if (Properties.Settings.Default.latestNewsLastShown.Date.Day != DateTime.Now.Date.Day)
                {
                    Show = true;
                }
                if (Show)
                {
                    Forms.LatestNews ln = new Party_Buffalo.Forms.LatestNews();
                    ln.Show();
                }
#endif

                #endregion
            }
            else
            {
                m_DisableNews.Checked = true;
            }
        }

        void listView1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(ListViewItem)))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.Copy;
            }
        }

        void listView1_ItemDrag(object sender, ItemDragEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                /* So, here's the ghetto method I'm doing that I sort of jacked from Le Fluffie's method
                 * 
                 * Extract file to temp directory
                 * Do the "DoDragDrop" thing to make the system move a temp file
                 * Use the FileSystemWatcher to see where the file was moved to
                 * 
                 */
                string Temp = System.IO.Path.GetTempPath() + "\\Party Buffalo";
                if (!System.IO.Directory.Exists(Temp))
                    System.IO.Directory.CreateDirectory(Temp);
                int Random = new Random().Next(0, 1000);
                string TempName = "\\partytempfile.tmp" + Random.ToString();
                if (!System.IO.File.Exists(Temp + TempName))
                {
                    // Create the temp file
                    System.IO.File.Create(Temp + TempName).Dispose();
                    // Set the attributes to hidden and temporary so they can be cleaned up later
                    System.IO.File.SetAttributes(Temp + TempName, System.IO.FileAttributes.Hidden | System.IO.FileAttributes.Temporary);
                }
                // Get the drag/drop data object
                DataObject file = new DataObject(DataFormats.FileDrop, new string[] { Temp + TempName });
                // Set the data
                file.SetData(DataFormats.StringFormat, Temp + TempName);
                
                // Now mess with the FileSystemWatchers so they will only see this file
                foreach (System.IO.FileSystemWatcher Watcher in Watchers)
                {
                    Watcher.Filter = TempName.Replace("\\", "");
                    Watcher.NotifyFilter = System.IO.NotifyFilters.FileName;
                    Watcher.EnableRaisingEvents = true;
                    Watcher.IncludeSubdirectories = true;
                }

                // Do the drag/drop stuff
                DoDragDrop(file, DragDropEffects.Move);
            }
        }

        void Watcher_Created(object sender, System.IO.FileSystemEventArgs e)
        {
            // Get the directory that the temporary file was moved to
            string OutputDirectory = System.IO.Directory.GetParent(e.FullPath).FullName;
            // Delete the temporary file
            try
            {
                System.IO.File.Delete(e.FullPath);
            }
            catch
            {
                System.Threading.Thread.Sleep(500);
                System.IO.File.Delete(e.FullPath);
            }
            // Disable the watchers
            foreach (System.IO.FileSystemWatcher Watcher in Watchers)
            {
                Watcher.EnableRaisingEvents = false;
            }

            this.Invoke((MethodInvoker)delegate
            {
                // Round up the entries!
                List<Entry> entries = new List<Entry>();
                foreach (ListViewItem li in listView1.SelectedItems)
                {
                    entries.Add((Entry)li.Tag);
                }

                // If we're dealing with one entry and it's a file, we'll probably want to 
                if (entries.Count == 1 && !((Entry)listView1.FocusedItem.Tag).IsFolder)
                {
                    OutputDirectory += "\\" + ((Entry)listView1.FocusedItem.Tag).Name;
                }


                // Start extracting
                Forms.EntryAction ea = new Party_Buffalo.Forms.EntryAction(entries.ToArray(), Party_Buffalo.Forms.EntryAction.Method.Extract, OutputDirectory);
                ea.ShowDialog();
            });
        }

        void listview_ContextMenu_Popup(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode == treeView1.Nodes[0])
            {
                foreach (MenuItem mi in listview_ContextMenu.MenuItems)
                {
                    mi.Enabled = false;
                }
                return;
            }
            else
            {
                foreach (MenuItem mi in listview_ContextMenu.MenuItems)
                {
                    mi.Enabled = true;
                }
            }

            if (((Folder)treeView1.SelectedNode.Tag).IsDeleted)
            {
                foreach (MenuItem mi in listview_ContextMenu.MenuItems)
                {
                    mi.Enabled = false;
                }
                c_Extract.Enabled = true;
            }
            else
            {
                foreach (MenuItem mi in listview_ContextMenu.MenuItems)
                {
                    mi.Enabled = true;
                }
            }
        }

        public CLKsFATXLib.Drive Drive
        {
            get;
            set;
        }

        public ImageList LargeListForFATX
        {
            get
            {
                ImageList i = new ImageList();
                Image[] images = { Properties.Resources.fatx_folder, Properties.Resources.fatx_file, Properties.Resources.fatx_database, Properties.Resources.HDD, Properties.Resources.USB, Properties.Resources.Backup };
                i.ColorDepth = ColorDepth.Depth32Bit;
                i.TransparentColor = Color.White;
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
                Image[] images = { Properties.Resources.fatx_folder, Properties.Resources.fatx_file, Properties.Resources.fatx_database, Properties.Resources.HDD, Properties.Resources.USB, Properties.Resources.Backup };
                i.ColorDepth = ColorDepth.Depth32Bit;
                i.TransparentColor = Color.White;
                i.ImageSize = new System.Drawing.Size(16, 16);
                i.Images.AddRange(images);
                return i;
            }
        }

        #region MenuItem Clicks

        void listView1_DoubleClick(object sender, EventArgs e)
        {
            if (listView1.FocusedItem != null && ((Entry)listView1.FocusedItem.Tag).IsFolder)
            {
                treeView1.SelectedNode = treeView1.SelectedNode.Nodes.Find(listView1.FocusedItem.Text, false)[0];
            }
        }

        private void m_OpenDeviceSelector_Click(object sender, EventArgs e)
        {
            Forms.DeviceSelector d = new Party_Buffalo.Forms.DeviceSelector(this);
            if (d.ShowDialog() == DialogResult.OK)
            {
                LoadDrive();
            }
        }

        private void m_OpenDump_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                if (Drive != null)
                {
                    Drive.Close();
                }
                Drive = new Drive(ofd.FileName);
                LoadDrive();
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
                    //string shell32DllPath = Environment.GetFolderPath(Environment.SpecialFolder.System) + "\\" + "shell32.dll";
                    Properties.Settings.Default.recentFiles.Insert(0, ofd.FileName);
                    // Change the jumplist
                    //Windows7.DesktopIntegration.JumpListManager jm = this.CreateJumpListManager();
                    if (Properties.Settings.Default.recentFiles.Count == 6)
                    {
                        Properties.Settings.Default.recentFiles.RemoveAt(5);
                    }
                    Properties.Settings.Default.Save();
                    MenuItem i = new MenuItem(ofd.FileName);
                    i.Click += new EventHandler(RecentFileHandler);
                    m_Open.MenuItems.Add(4, i);

                    //jm.AddCustomDestination(new ShellLink
                    //{
                    //    Path = ofd.FileName,
                    //    Title = Drive.Name,
                    //    Category = "Recent Files",
                    //    IconLocation = shell32DllPath,
                    //    IconIndex = 1
                    //});
                    //jm.AddToRecent(Properties.Settings.Default.recentFiles[0]);
                }
            }
        }

        private void m_OpenCustomBackup_Click(object sender, EventArgs e)
        {
            if (!Aero)
            {
                FolderBrowserDialog fbd = new FolderBrowserDialog();
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    Forms.CustomBackupViewer cb = new Party_Buffalo.Forms.CustomBackupViewer(fbd.SelectedPath);
                    cb.Show();
                }
            }
            else
            {
                CommonOpenFileDialog cfd = new CommonOpenFileDialog();
                cfd.IsFolderPicker = true;
                cfd.Multiselect = false;
                if (cfd.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    Forms.CustomBackupViewer cb = new Party_Buffalo.Forms.CustomBackupViewer(cfd.FileName);
                    cb.Show();
                }
            }
        }

        private void m_CloseDrive_Click(object sender, EventArgs e)
        {
            Clear();
        }

        private void menuItem4_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void menuItem2_Click(object sender, EventArgs e)
        {
            Forms.CustomBackup cb = new Party_Buffalo.Forms.CustomBackup();
            if (cb.ShowDialog() == DialogResult.OK)
            {
                if (!Aero)
                {
                    FolderBrowserDialog fbd = new FolderBrowserDialog();
                    if (fbd.ShowDialog() == DialogResult.OK)
                    {
                        List<Folder> Partitions = new List<Folder>();
                        for (int i = 0; i < treeView1.Nodes[0].Nodes.Count; i++)
                        {
                            Partitions.Add((Folder)treeView1.Nodes[0].Nodes[i].Tag);
                        }
                        Forms.EntryAction ea = new Party_Buffalo.Forms.EntryAction(Partitions.ToArray(), Party_Buffalo.Forms.EntryAction.Method.Extract, cb.FoldersToSkip, fbd.SelectedPath);
                        ea.ShowDialog();
                    }
                }
                else
                {
                    CommonOpenFileDialog cfd = new CommonOpenFileDialog();
                    cfd.Multiselect = false;
                    cfd.IsFolderPicker = true;
                    if (cfd.ShowDialog() == CommonFileDialogResult.Ok)
                    {
                        List<Folder> Partitions = new List<Folder>();
                        for (int i = 0; i < treeView1.Nodes[0].Nodes.Count; i++)
                        {
                            Partitions.Add((Folder)treeView1.Nodes[0].Nodes[i].Tag);
                        }
                        Forms.EntryAction ea = new Party_Buffalo.Forms.EntryAction(Partitions.ToArray(), Party_Buffalo.Forms.EntryAction.Method.Extract, cb.FoldersToSkip, cfd.FileName);
                        ea.ShowDialog();
                    }
                }
            }
        }

        private void m_diskCleanup_Click(object sender, EventArgs e)
        {

        }

        private void m_QuickAdd_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = true;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                Forms.EntryAction ea = new Party_Buffalo.Forms.EntryAction(ofd.FileNames, Drive, Party_Buffalo.Forms.EntryAction.Method.Inject);
                ea.ShowDialog(this);
            }
        }

        private void m_BackupImage_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Backup File(*.bin)|*.bin";
            sfd.FileName = "Xbox Backup " + Drive.LengthFriendly + ".bin";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                Forms.EntryAction ea = new Party_Buffalo.Forms.EntryAction(Drive, Party_Buffalo.Forms.EntryAction.Method.Backup, sfd.FileName);
                ea.ShowDialog();
            }
        }

        private void m_RestoreImage_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Backup File(*.bin)|*.bin";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                Forms.EntryAction ea = new Party_Buffalo.Forms.EntryAction(Drive, Party_Buffalo.Forms.EntryAction.Method.Restore, ofd.FileName);
                ea.ShowDialog();
            }
        }

        private void m_ExtractSecuritySector_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Backup File(*.bin)|*.bin";
            sfd.FileName = "Security Sector " + Drive.LengthFriendly + ".bin";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                Forms.EntryAction ea = new Party_Buffalo.Forms.EntryAction(Drive, Party_Buffalo.Forms.EntryAction.Method.ExtractSS, sfd.FileName);
                ea.ShowDialog();
            }
        }

        private void m_ExtractJosh_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Backup File(*.bin)|*.bin";
            sfd.FileName = "Josh " + Drive.LengthFriendly + ".bin";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                Forms.EntryAction ea = new Party_Buffalo.Forms.EntryAction(Drive, Party_Buffalo.Forms.EntryAction.Method.ExtractJ, sfd.FileName);
                ea.ShowDialog();
            }
        }

        private void m_ReadOnly_Click(object sender, EventArgs e)
        {

        }

        private void m_DeletionMode_Click(object sender, EventArgs e)
        {
            m_DeletionMode.Checked = !m_DeletionMode.Checked;
            Reload();
        }

        private void m_LoadSTFS_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.loadSTFS = !Properties.Settings.Default.loadSTFS;
            Properties.Settings.Default.Save();
            m_LoadSTFS.Checked = !m_LoadSTFS.Checked;
        }

        private void m_retrieveGameNames_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.loadTIDNames = !Properties.Settings.Default.loadTIDNames;
            Properties.Settings.Default.Save();
            m_retrieveGameNames.Checked = !m_retrieveGameNames.Checked;
        }

        private void SaveIcons_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.SaveIcons = !Properties.Settings.Default.SaveIcons;
            Properties.Settings.Default.Save();
            SaveIcons.Checked = !SaveIcons.Checked;
        }

        private void m_loadEntireDrive_Click(object sender, EventArgs e)
        {
            m_loadEntireDrive.Checked = !m_loadEntireDrive.Checked;
            Properties.Settings.Default.loadEntireDrive = m_loadEntireDrive.Checked;
            Properties.Settings.Default.Save();
        }

        private void menuItem32_Click(object sender, EventArgs e)
        {
            Forms.Cache c = new Party_Buffalo.Forms.Cache();
            c.Show();
        }

        private void menuItem30_Click(object sender, EventArgs e)
        {
            string FolderPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Party Buffalo Drive Explorer\\Cached Icons";
            if (System.IO.Directory.Exists(FolderPath))
            {
                System.Diagnostics.Process.Start(FolderPath);
            }
            else
            {
                MessageBox.Show("Cache has not been created yet.");
            }
        }

        private void c_largeIcon_Click(object sender, EventArgs e)
        {
            if (!c_largeIcon.Checked)
            {
                c_largeIcon.Checked = true;
                c_Details.Checked = false;
                c_SmallIcon.Checked = false;
                c_Tile.Checked = false;
                c_List.Checked = false;
            }
            Properties.Settings.Default.view = View.LargeIcon;
            listView1.View = Properties.Settings.Default.view;
            Properties.Settings.Default.Save();
        }

        private void c_Details_Click(object sender, EventArgs e)
        {
            if (!c_Details.Checked)
            {
                c_largeIcon.Checked = false;
                c_Details.Checked = true;
                c_SmallIcon.Checked = false;
                c_Tile.Checked = false;
                c_List.Checked = false;
            }
            Properties.Settings.Default.view = View.Details;
            listView1.View = Properties.Settings.Default.view;
            Properties.Settings.Default.Save();
        }

        private void c_SmallIcon_Click(object sender, EventArgs e)
        {
            if (!c_SmallIcon.Checked)
            {
                c_largeIcon.Checked = false;
                c_Details.Checked = false;
                c_SmallIcon.Checked = true;
                c_Tile.Checked = false;
                c_List.Checked = false;
            }
            Properties.Settings.Default.view = View.SmallIcon;
            listView1.View = Properties.Settings.Default.view;
            Properties.Settings.Default.Save();
        }

        private void c_List_Click(object sender, EventArgs e)
        {
            if (!c_List.Checked)
            {
                c_largeIcon.Checked = false;
                c_Details.Checked = false;
                c_SmallIcon.Checked = false;
                c_Tile.Checked = false;
                c_List.Checked = true;
            }
            Properties.Settings.Default.view = View.List;
            listView1.View = Properties.Settings.Default.view;
            Properties.Settings.Default.Save();
        }

        private void c_Tile_Click(object sender, EventArgs e)
        {
            if (!c_Tile.Checked)
            {
                c_largeIcon.Checked = false;
                c_Details.Checked = false;
                c_SmallIcon.Checked = false;
                c_Tile.Checked = true;
                c_List.Checked = false;
            }
            Properties.Settings.Default.view = View.Tile;
            listView1.View = Properties.Settings.Default.view;
            Properties.Settings.Default.Save();
        }

        private void menuItem20_Click(object sender, EventArgs e)
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
            Forms.BookmarkCreate b = new Forms.BookmarkCreate(treeView1.SelectedNode.RealPath(), name);
            if (b.ShowDialog() == DialogResult.OK)
            {
                AddBookmark(b.New);
            }
        }

        private void m_Search_Click(object sender, EventArgs e)
        {
            Forms.SearchForm sf = new Party_Buffalo.Forms.SearchForm(Drive.Partitions);
            if (sf.ShowDialog() == DialogResult.OK)
            {
                pathBar.Text = sf.Path;
                goButton.PerformClick();
            }
        }

        private void menuItem37_Click(object sender, EventArgs e)
        {

        }

        private void menuItem14_Click(object sender, EventArgs e)
        {

        }

        private void menuItem10_Click(object sender, EventArgs e)
        {

        }

        private void menuItem16_Click(object sender, EventArgs e)
        {

        }

        private void menuItem23_Click(object sender, EventArgs e)
        {

        }

        private void menuItem17_Click(object sender, EventArgs e)
        {
            if (!Aero)
            {
                MessageBox.Show("I'm not trying to break your shit, but it could happen.  I'm not responsible for your stuff.");
            }
            else
            {
                TaskDialog td = new TaskDialog();
                td.Caption = "Disclaimer";
                td.InstructionText = "Assumed Disclaimer";
                td.Text = "I'm not trying to break your shit, but it could happen.  I'm not responsible for your stuff.";
                td.StandardButtons = TaskDialogStandardButtons.Ok;
                td.Show();
            }
        }

        private void menuItem42_Click(object sender, EventArgs e)
        {

        }

        private void menuItem43_Click(object sender, EventArgs e)
        {

        }

        private void menuItem7_Click(object sender, EventArgs e)
        {
            string Text = "Party Buffalo was created by CLK Rebellion (Lander Griffith) with help from gabe_k.  You may contact me at clkxu5@gmail.com\r\n\r\nThis application is not affiliated with Microsoft Corp. \"Microsoft\", \"Xbox\", \"Xbox 360\" and \"Xbox LIVE\" are registered trademarks of Microsoft Corp.";
            string Thanks = "skitzo, gabe_k, Cody, hippie, Rickshaw, Cheater912, unknown_v2, sonic-iso, XeNoN.7\r\n\r\nCaboose (Nyan Cat progress bar), Mathieulh (stealing credit cards), idc \"Looks like a list of attendees for a furry convention to me\", roofus & angerwound for the first Xbox 360 FATX explorer which still keeps people satisfied...";
            string Version = "Version: " + Application.ProductVersion.ToString();

            if (Aero)
            {
                Microsoft.WindowsAPICodePack.Dialogs.TaskDialog td = new Microsoft.WindowsAPICodePack.Dialogs.TaskDialog();
                td.Caption = "About Party Buffalo";
                td.Text = Text;
                td.InstructionText = "About Party Buffalo";
                td.DetailsCollapsedLabel = "Special Thanks To...";
                td.DetailsExpandedText = Thanks;
                Microsoft.WindowsAPICodePack.Dialogs.TaskDialogButton Donate = new Microsoft.WindowsAPICodePack.Dialogs.TaskDialogButton("Donate", "Donate");
                Donate.Click += (o, f) => { MessageBox.Show("Thank you for your contribution!"); System.Diagnostics.Process.Start("https://www.paypal.com/cgi-bin/webscr?cmd=_donations&business=JWATGN6RETA5Y&lc=US&item_name=Party%20Buffalo%20Drive%20Explorer&currency_code=USD&bn=PP%2dDonationsBF%3abtn_donateCC_LG%2egif%3aNonHosted"); };

                //Microsoft.WindowsAPICodePack.Dialogs.TaskDialogButton Visit = new Microsoft.WindowsAPICodePack.Dialogs.TaskDialogButton("Visit", "Visit CLKXU5.com");
                //Visit.Click += (o, f) => { System.Diagnostics.Process.Start("http://clkxu5.com"); };

                Microsoft.WindowsAPICodePack.Dialogs.TaskDialogButton Close = new Microsoft.WindowsAPICodePack.Dialogs.TaskDialogButton("Close", "Close");
                Close.Click += (o, f) => { td.Close(); };

                td.HyperlinksEnabled = true;
                td.HyperlinkClick += (o, f) => { switch (f.LinkText)
                {
                    case "Visit the Development Blog":
                        System.Diagnostics.Process.Start("http://clkxu5.com");
                        break;
                    default:
                        System.Diagnostics.Process.Start("http://free60.org/FATX");
                        break;
                }};
                td.FooterText = "Thank you for using Party Buffalo Drive Explorer\r\n" + Version + "\r\n<a href=\"http://clkxu5.com\">Visit the Development Blog</a> - <a href=\"http://free60.org/FATX\">FATX Research</a>";

                td.Controls.Add(Donate);
                //td.Controls.Add(Visit);
                td.Controls.Add(Close);
                td.Show();
            }
            else
            {
                if (MessageBox.Show(Text + "\r\n" + Thanks + "\r\n" + Version + "\r\nWould you like to donate?", "About Party Buffalo", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    MessageBox.Show("Thank you for your contribution!"); System.Diagnostics.Process.Start("https://www.paypal.com/cgi-bin/webscr?cmd=_donations&business=JWATGN6RETA5Y&lc=US&item_name=Party%20Buffalo%20Drive%20Explorer&currency_code=USD&bn=PP%2dDonationsBF%3abtn_donateCC_LG%2egif%3aNonHosted");
                }
            }
        }

        private void menuItem13_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://clkxu5.com/drivexplore/src");
        }

        #endregion

        #region Listview Stuff

        private void c_Extract_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 1 && !((Entry)listView1.FocusedItem.Tag).IsFolder)
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.FileName = ((Entry)listView1.FocusedItem.Tag).Name;
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    Forms.EntryAction ea = new Party_Buffalo.Forms.EntryAction(new Entry[] { ((Entry)listView1.FocusedItem.Tag) }, Party_Buffalo.Forms.EntryAction.Method.Extract, sfd.FileName);
                    ea.ShowDialog();
                }
            }
            else
            {
                if (!Aero)
                {
                    FolderBrowserDialog fbd = new FolderBrowserDialog();
                    if (fbd.ShowDialog() == DialogResult.OK)
                    {
                        List<Entry> entries = new List<Entry>();
                        foreach (ListViewItem li in listView1.SelectedItems)
                        {
                            entries.Add((Entry)li.Tag);
                        }
                        Forms.EntryAction ea = new Party_Buffalo.Forms.EntryAction(entries.ToArray(), Party_Buffalo.Forms.EntryAction.Method.Extract, fbd.SelectedPath);
                        ea.ShowDialog();
                    }
                }
                else
                {
                    CommonOpenFileDialog cfd = new CommonOpenFileDialog();
                    cfd.IsFolderPicker = true;
                    cfd.Multiselect = false;
                    if (cfd.ShowDialog() == CommonFileDialogResult.Ok)
                    {
                        List<Entry> entries = new List<Entry>();
                        foreach (ListViewItem li in listView1.SelectedItems)
                        {
                            entries.Add((Entry)li.Tag);
                        }
                        Forms.EntryAction ea = new Party_Buffalo.Forms.EntryAction(entries.ToArray(), Party_Buffalo.Forms.EntryAction.Method.Extract, cfd.FileName);
                        ea.ShowDialog();
                    }
                }
            }
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            m_OpenDeviceSelector.PerformClick();
        }

        private void openToolStripButton_Click(object sender, EventArgs e)
        {
            m_OpenDump.PerformClick();
        }

        private void ts_Close_Click(object sender, EventArgs e)
        {
            Clear();
        }

        private void goButton_Click(object sender, EventArgs e)
        {
            if (Drive != null)
            {
                if (pathBar.Text.ToLower() != "brick squad")
                {
                    treeView1.SelectedNode = treeView1.NodeFromPath(pathBar.Text);
                }
                else
                {
                    System.Diagnostics.Process.Start("http://www.youtube.com/watch?v=CjhU6mx6tNY");
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode.Parent != treeView1.Nodes[0] && Drive != null)
            {
                treeView1.SelectedNode = treeView1.SelectedNode.Parent;
            }
        }

        private void l_AddToBookMarks_Click(object sender, EventArgs e)
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
            Forms.BookmarkCreate b = new Forms.BookmarkCreate(treeView1.SelectedNode.RealPath(), name);
            if (b.ShowDialog() == DialogResult.OK)
            {
                AddBookmark(b.New);
            }
        }

        private void c_NewFolder_Click(object sender, EventArgs e)
        {
            ((Folder)treeView1.SelectedNode.Tag).CreateNewFolder(GetNewFolderName((Folder)treeView1.SelectedNode.Tag));
            listView1.Items[listView1.Items.Count - 1].BeginEdit();
        }

        private void c_Delete_Click(object sender, EventArgs e)
        {
            // String.format is a little redundant here...
            DialogResult dr = DialogResult.No;
            if (!Aero)
            {
                dr = MessageBox.Show(string.Format("Are you sure you want to delete the selected{0}?", (listView1.SelectedItems.Count > 1) ? " " + listView1.SelectedItems.Count.ToString() + " items" : " item"), "Confirm Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            }
            else
            {
                TaskDialog td = new TaskDialog();
                td.Caption = "Confirm Deletion";
                td.InstructionText = "Confirm Deletion";
                td.Text = string.Format("Are you sure you want to delete the selected{0}?", (listView1.SelectedItems.Count > 1) ? " " + listView1.SelectedItems.Count.ToString() + " items" : " item");
                td.StandardButtons = TaskDialogStandardButtons.Yes | TaskDialogStandardButtons.No | TaskDialogStandardButtons.Cancel;
                if (td.ShowDialog(this.Handle) == TaskDialogResult.Yes)
                {
                    dr = DialogResult.Yes;
                }
            }
            if (dr == DialogResult.Yes)
            {
                List<Entry> items = new List<Entry>();
                foreach (ListViewItem li in listView1.SelectedItems)
                {
                    items.Add((Entry)li.Tag);
                }
                Forms.EntryAction ea = new Party_Buffalo.Forms.EntryAction(items.ToArray(), Party_Buffalo.Forms.EntryAction.Method.Delete, "");
                ea.ShowDialog();
            }
        }

        private void c_Rename_Click(object sender, EventArgs e)
        {
            listView1.FocusedItem.BeginEdit();
        }

        private void c_InjectFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = true;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                Forms.EntryAction ea = new Party_Buffalo.Forms.EntryAction(ofd.FileNames, ((Folder)treeView1.SelectedNode.Tag), Party_Buffalo.Forms.EntryAction.Method.Inject);
                ea.ShowDialog(this);
            }
        }

        private void c_InjectFolder_Click(object sender, EventArgs e)
        {
            if (!Aero)
            {
                FolderBrowserDialog fbd = new FolderBrowserDialog();
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    Forms.EntryAction ea = new Party_Buffalo.Forms.EntryAction(new string[] { fbd.SelectedPath }, (Folder)treeView1.SelectedNode.Tag, Party_Buffalo.Forms.EntryAction.Method.Inject);
                    ea.ShowDialog(this);
                }
            }
            else
            {
                CommonOpenFileDialog cfd = new CommonOpenFileDialog();
                cfd.IsFolderPicker = true;
                cfd.Multiselect = true;
                if (cfd.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    Forms.EntryAction ea = new Party_Buffalo.Forms.EntryAction(cfd.FileNames.ToArray(), (Folder)treeView1.SelectedNode.Tag, Party_Buffalo.Forms.EntryAction.Method.Inject);
                    ea.ShowDialog(this);
                }
            }
        }

        private void c_Properties_Click(object sender, EventArgs e)
        {
            if (((Entry)listView1.FocusedItem.Tag).IsFolder)
            {
                Forms.PropertiesForm pf = new Party_Buffalo.Forms.PropertiesForm((Folder)listView1.FocusedItem.Tag);
                pf.Show();
            }
            else
            {
                Forms.PropertiesForm pf = new Party_Buffalo.Forms.PropertiesForm((File)listView1.FocusedItem.Tag);
                pf.Show();
            }
        }

        private void menuItem36_Click(object sender, EventArgs e)
        {

        }


        void listView1_KeyPress(object sender, KeyPressEventArgs e)
        {
            ListviewLabelUpdate();
        }

        void listView1_KeyUp(object sender, KeyEventArgs e)
        {
            ListviewLabelUpdate();
            if (e.KeyCode == Keys.Delete)
            {
                c_Delete.PerformClick();
            }
        }

        void listView1_MouseUp(object sender, MouseEventArgs e)
        {
            ListviewLabelUpdate();
        }

        void listView1_MouseDown(object sender, MouseEventArgs e)
        {
            ListviewLabelUpdate();
        }

        void listView1_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else if (e.Data.GetDataPresent(typeof(ListViewItem)))
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
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (Drive != null)
                {
                    if (!quickAddMode.Checked)
                    {
                        Folder f = (Folder)treeView1.SelectedNode.Tag;
                        Forms.EntryAction ea = new Forms.EntryAction(files, f, Party_Buffalo.Forms.EntryAction.Method.Inject);
                        ea.ShowDialog(this);
                    }
                    else
                    {
                        Forms.EntryAction ea = new Forms.EntryAction(files, Drive, Party_Buffalo.Forms.EntryAction.Method.Inject);
                        ea.ShowDialog(this);
                    }
                }
                else if (!(VariousFunctions.IsFolder(files[0])) && files.Length == 1)
                {
                    Drive d = new Drive(files[0]);
                    if (d.IsFATXDrive())
                    {
                        if (Drive != null)
                        {
                            Drive.Close();
                        }
                        Drive = d;
                        LoadDrive();
                    }
                }
            }
            else if (e.Data.GetDataPresent(typeof(ListViewItem)))
            {
                return;
            }
        }

        void listView1_AfterLabelEdit(object sender, LabelEditEventArgs e)
        {
            if (e.Label == null)
            {
                e.CancelEdit = true;
                return;
            }
            Entry ye = ((Entry)listView1.Items[e.Item].Tag);
            if (e.Label.Length > 0x2A)
            {
                MessageBox.Show("Desired name is too long!", "Name Too Long", MessageBoxButtons.OK, MessageBoxIcon.Information);
                e.CancelEdit = true;
            }
            if (!ye.Rename(e.Label))
            {
                MessageBox.Show("Desired name invalid!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                e.CancelEdit = true;
                return;
            }
            if (ye.IsFolder)
            {
                Folder f = (Folder)ye;
                TreeNode tn = treeView1.SelectedNode.Nodes.Find(listView1.Items[e.Item].Name, false)[0];
                listView1.Items[e.Item].Name = e.Label;
                tn.Name = e.Label;
                tn.Text = tn.Name;

                string Label = Cache.CheckCache(f.FullPath);
                if (Label != null)
                {
                    tn.Text += " | " + Label;
                    if (listView1.Items[e.Item].SubItems.Count == 0x4)
                    {
                        listView1.Items[e.Item].SubItems.Add(Label);
                    }
                    else
                    {
                        listView1.Items[e.Item].SubItems[4].Text = Label;
                    }
                }
                if (f.IsTitleIDFolder)
                {
                    if (Properties.Settings.Default.loadTIDNames)
                    {
                        if (Cache.CheckCache(f.Name) != null)
                        {
                            tn.Text += " | " + Cache.CheckCache(f.Name);
                            if (listView1.Items[e.Item].SubItems.Count == 0x4)
                            {
                                listView1.Items[e.Item].SubItems.Add(tn.GameName());
                            }
                            else
                            {
                                listView1.Items[e.Item].SubItems[4].Text = tn.GameName();
                            }
                        }
                        // Didn't find it, need to grab that shit
                        else if (!f.IsKnownFolder)
                        {
                            if (f.GameName() != null && f.GameName() != "")
                            {
                                if (!f.IsKnownFolder)
                                {
                                    Cache.AddID(f.Name, f.GameName());
                                }
                                tn.Text += " | " + f.GameName();
                                listView1.Items[e.Item].SubItems[4].Text = f.GameName();
                            }
                        }
                    }

                    if (Properties.Settings.Default.SaveIcons)
                    {
                        // Getting the icon
                        if (!f.IsKnownFolder || f.Name.ToLower() == "FFFE07D1".ToLower())
                        {
                            if (!siL.Images.Keys.Contains(f.Name.ToUpper()))
                            {
                                // Check the cache for the icon
                                if (Cache.GetIcon(f.Name) != null)
                                {
                                    treeView1.Invoke((MethodInvoker)delegate
                                    {
                                        siL.Images.Add(f.Name.ToUpper(), Cache.GetIcon(f.Name));
                                        liL.Images.Add(f.Name.ToUpper(), Cache.GetIcon(f.Name));
                                        tn.ImageKey = f.Name.ToUpper();
                                        tn.SelectedImageKey = tn.ImageKey;
                                        listView1.Items[e.Item].ImageKey = tn.ImageKey;
                                    });
                                }
                                else
                                {
                                    if (f.GameIcon() != null)
                                    {
                                        Cache.AddIcon(f.GameIcon(), f.Name);
                                        treeView1.Invoke((MethodInvoker)delegate
                                        {
                                            siL.Images.Add(f.Name.ToUpper(), f.GameIcon());
                                            liL.Images.Add(f.Name.ToUpper(), f.GameIcon());
                                            //treeView1.ImageList = SmallListForFATX;
                                            //listView1.SmallImageList = SmallListForFATX;
                                            //listView1.LargeImageList = LargeListForFATX;
                                            tn.ImageKey = f.Name.ToUpper();
                                            tn.SelectedImageKey = tn.ImageKey;
                                            listView1.Items[e.Item].ImageKey = tn.ImageKey;
                                        }
                                        );
                                    }
                                }
                            }
                            else
                            {
                                tn.ImageKey = f.Name.ToUpper();
                                tn.SelectedImageKey = tn.ImageKey;
                                listView1.Items[e.Item].ImageKey = tn.ImageKey;
                            }
                        }
                    }
                }
                else
                {
                    if (listView1.Items[e.Item].SubItems.Count == 5)
                    {
                        listView1.Items[e.Item].SubItems[4].Text = "";
                    }
                    tn.ImageKey = "";
                    tn.SelectedImageKey = "";
                    if (ye.IsFolder)
                    {
                        listView1.Items[e.Item].ImageIndex = 0;
                    }
                    else
                    {
                        listView1.Items[e.Item].ImageIndex = 1;
                    }
                }
                tn.Tag = f;
            }
            else
            {
                listView1.Items[e.Item].Name = e.Label;
            }
        }

        void listView1_BeforeLabelEdit(object sender, LabelEditEventArgs e)
        {
            Entry ye = ((Entry)listView1.Items[e.Item].Tag);
            if (ye.IsDeleted)
            {
                e.CancelEdit = true;
            }
        }

        void listView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.A && e.Control)
            {
                foreach (ListViewItem item in listView1.Items)
                {
                    item.Selected = true;
                }
            }
        }

        void listView1_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            ColumnClickHandler(e.Column);
        }

#endregion

        #region TreeView Stuff

        private void t_AddToBookmarks_Click(object sender, EventArgs e)
        {
            if (rightClickedNode == null || rightClickedNode == treeView1.Nodes[0])
            {
                return;
            }
            string name = "";
            if (m_retrieveGameNames.Checked)
            {
                name = ((Folder)rightClickedNode.Tag).GameName();
                if (name == "")
                {
                    name = rightClickedNode.Text;
                }
            }
            else
            {
                name = rightClickedNode.Text;
            }
            Forms.BookmarkCreate b = new Forms.BookmarkCreate(rightClickedNode.RealPath(), name);
            if (b.ShowDialog() == DialogResult.OK)
            {
                AddBookmark(b.New);
            }
        }

        private void t_driveProperties_Click(object sender, EventArgs e)
        {
            Forms.Drive_Properties dp = new Party_Buffalo.Forms.Drive_Properties(Drive);
            dp.ShowDialog();
        }

        private void t_Rename_Click(object sender, EventArgs e)
        {
            rightClickedNode.BeginEdit();
        }

        private void t_InjectFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = true;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                Forms.EntryAction ea = new Party_Buffalo.Forms.EntryAction(ofd.FileNames, ((Folder)rightClickedNode.Tag), Party_Buffalo.Forms.EntryAction.Method.Inject);
                ea.ShowDialog(this);
            }
        }

        private void t_InjectFolder_Click(object sender, EventArgs e)
        {
            if (!Aero)
            {
                FolderBrowserDialog fbd = new FolderBrowserDialog();
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    Forms.EntryAction ea = new Party_Buffalo.Forms.EntryAction(new string[] { fbd.SelectedPath }, (Folder)rightClickedNode.Tag, Party_Buffalo.Forms.EntryAction.Method.Inject);
                    ea.ShowDialog(this);
                }
            }
            else
            {
                CommonOpenFileDialog cfd = new CommonOpenFileDialog();
                cfd.Multiselect = true;
                cfd.IsFolderPicker = true;
                if (cfd.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    Forms.EntryAction ea = new Party_Buffalo.Forms.EntryAction(cfd.FileNames.ToArray(), (Folder)rightClickedNode.Tag, Party_Buffalo.Forms.EntryAction.Method.Inject);
                    ea.ShowDialog(this);
                }
            }
        }

        private void t_NewFolder_Click(object sender, EventArgs e)
        {
            try
            {
                ((Folder)rightClickedNode.Tag).CreateNewFolder(GetNewFolderName((Folder)rightClickedNode.Tag));
            }
            catch (Exception x)
            {
                if (!Aero)
                {
                    MessageBox.Show("An exception was thrown: " + x.Message + "\r\n\r\nIf this appears to be a bug, press CTRL + C to copy the stack trace, then please email it to me at clkxu5@gmail.com:\r\n" + x.StackTrace);
                }
                else
                {
                    TaskDialog td = new TaskDialog();
                    td.Caption = "Unhandled Exception";
                    td.InstructionText = "An Unhandled Exception was Thrown";
                    td.Text = string.Format("An exception was thrown: {0}\r\n\r\nIf this appears to be a bug, please email me at clkxu5@gmail.com with the details below", x.Message);
                    td.DetailsCollapsedLabel = "Details";
                    td.DetailsExpandedLabel = "Details";
                    td.DetailsExpandedText = x.StackTrace;

                    TaskDialogButton Copy = new TaskDialogButton("Copy", "Copy Details to Clipboard");
                    Copy.Click += (o, f) => { Clipboard.SetDataObject(x.Message + "\r\n\r\n" + x.StackTrace, true, 10, 200); };

                    TaskDialogButton Close = new TaskDialogButton("Close", "Close");
                    Close.Click += (o, f) => { td.Close(); };

                    td.Controls.Add(Copy);
                    td.Controls.Add(Close);
                    td.ShowDialog(this.Handle);
                }
            }
        }

        private void t_Delete_Click(object sender, EventArgs e)
        {
            DialogResult dr = DialogResult.No;
            if (!Aero)
            {
                dr = MessageBox.Show("Are you sure you want to delete the selected folder?", "Confirm Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            }
            else
            {
                TaskDialog td = new TaskDialog();
                td.Caption = "Confirm Deletion";
                td.InstructionText = "Confirm Deletion";
                td.Text = string.Format("Are you sure you want to delete folder \"{0}\"?", rightClickedNode.Name);
                td.StandardButtons = TaskDialogStandardButtons.Yes | TaskDialogStandardButtons.No | TaskDialogStandardButtons.Cancel;
                if (td.ShowDialog(this.Handle) == TaskDialogResult.Yes)
                {
                    dr = DialogResult.Yes;
                }
            }

            if (dr == DialogResult.Yes)
            {
                List<Entry> items = new List<Entry>();
                items.Add((Entry)rightClickedNode.Tag);
                if (rightClickedNode == treeView1.SelectedNode)
                {
                    foreach (ListViewItem li in listView1.SelectedItems)
                    {
                        items.Add((Entry)li.Tag);
                    }
                }
                Forms.EntryAction ea = new Party_Buffalo.Forms.EntryAction(items.ToArray(), Party_Buffalo.Forms.EntryAction.Method.Delete, "");
                ea.ShowDialog();
                // Remove those treeview items
                foreach (Entry en in items)
                {
                    if (en.IsFolder)
                    {
                        try
                        {
                            treeView1.SelectedNode.Nodes.Find(en.Name, false)[0].Remove();
                        }
                        catch { }
                    }
                    if (rightClickedNode == treeView1.SelectedNode)
                    {
                        listView1.Items.Find(en.Name, false)[0].Remove();
                    }
                }
                ((Folder)rightClickedNode.Tag).Reload();
            }
        }

        private void t_CopyPath_Click(object sender, EventArgs e)
        {
            Clipboard.SetDataObject(rightClickedNode.RealPath(), true, 10, 200);
        }

        private void menuItem38_Click(object sender, EventArgs e)
        {
            rightClickedNode.ExpandAll();
        }

        private void menuItem39_Click(object sender, EventArgs e)
        {
            rightClickedNode.Collapse(false);
        }

        private void t_Properties_Click(object sender, EventArgs e)
        {
            Forms.PropertiesForm pf = new Party_Buffalo.Forms.PropertiesForm((Folder)rightClickedNode.Tag);
            pf.Show();
        }


        private void t_Extract_Click(object sender, EventArgs e)
        {
            if (!Aero)
            {
                FolderBrowserDialog fbd = new FolderBrowserDialog();
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    Forms.EntryAction ea = new Party_Buffalo.Forms.EntryAction(new Entry[] { (Folder)rightClickedNode.Tag }, Party_Buffalo.Forms.EntryAction.Method.Extract, fbd.SelectedPath);
                    ea.ShowDialog();
                }
            }
            else
            {
                CommonOpenFileDialog cfd = new CommonOpenFileDialog();
                cfd.IsFolderPicker = true;
                cfd.Multiselect = false;
                if (cfd.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    Forms.EntryAction ea = new Party_Buffalo.Forms.EntryAction(new Entry[] { (Folder)rightClickedNode.Tag }, Party_Buffalo.Forms.EntryAction.Method.Extract, cfd.FileName);
                    ea.ShowDialog();
                }
            }
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (treeView1.SelectedNode != null && treeView1.SelectedNode != treeView1.Nodes[0])
            {
                foreach (MenuItem i in listview_ContextMenu.MenuItems)
                {
                    i.Enabled = true;
                }
                listView1.Items.Clear();

                /* --Folders-- */
                AddNodes(treeView1.SelectedNode, (Folder)treeView1.SelectedNode.Tag, false, true);

                treeView1.SelectedNode.Expand();

                List<ListViewItem> lvi = new List<ListViewItem>();
                /* --Files-- */
                File[] Files = ((Folder)treeView1.SelectedNode.Tag).Files();
                for (int i = 0; i < Files.Length; i++)
                {
                    ListViewItem li = new ListViewItem(Files[i].Name);
                    li.SubItems.Add(Files[i].EntryType);
                    li.SubItems.Add(VariousFunctions.ByteConversion(Files[i].Size));
                    li.SubItems.Add(Files[i].ModifiedDate.ToString());
                    if (Properties.Settings.Default.loadSTFS)
                    {
                        if (Files[i].IsSTFSPackage())
                        {
                            li.SubItems.Add(Files[i].ContentName());
                        }
                        else if (Files[i].Parent.FullPath == Files[i].Drive.CacheFolderPath && CLKsFATXLib.Geometry.CacheFilePrefixes.CachePrefixes.Contains(Files[i].Name.Substring(0, 2)))
                        {
                            for (int j = 0; j < CLKsFATXLib.Geometry.CacheFilePrefixes.CachePrefixes.Length; j++)
                            {
                                if (CLKsFATXLib.Geometry.CacheFilePrefixes.CachePrefixes[j] == Files[i].Name.Substring(0, 2))
                                {
                                    li.SubItems.Add(CLKsFATXLib.Geometry.CacheFilePrefixes.PrefixNames[j]);
                                    break;
                                }
                            }
                        }
                    }
                    li.Tag = Files[i];
                    li.Name = Files[i].Name;

                    if (Properties.Settings.Default.cacheContentIcons)
                    {
                        if (listView1.LargeImageList.Images.ContainsKey(Files[i].Name))
                        {
                            li.ImageKey = Files[i].Name;
                        }
                        else
                        {
                            // Check if there's an icon for this guy cached
                            if (Cache.GetIcon(Files[i].Name) != null)
                            {
                                Image Icon = Cache.GetIcon(Files[i].Name);
                                treeView1.ImageList.Images.Add(Files[i].Name, Icon);
                                listView1.LargeImageList.Images.Add(Files[i].Name, Icon);
                                listView1.SmallImageList.Images.Add(Files[i].Name, Icon);
                                li.ImageKey = Files[i].Name;
                            }
                            // Check if the file is an STFS package, and if it is blahblahblah
                            else if (Files[i].IsSTFSPackage())
                            {
                                Image Icon = Files[i].ContentIcon();
                                if (Icon != null)
                                {
                                    Cache.AddIcon(Icon, Files[i].Name);
                                    treeView1.ImageList.Images.Add(Files[i].Name, Icon);
                                    listView1.LargeImageList.Images.Add(Files[i].Name, Icon);
                                    listView1.SmallImageList.Images.Add(Files[i].Name, Icon);
                                    li.ImageKey = Files[i].Name;
                                }
                            }
                        }
                    }
                    if (li.ImageKey == "")
                    {
                        li.ImageIndex = 1;
                    }

                    lvi.Add(li);
                }
                listView1.Items.AddRange(lvi.ToArray());
                //MessageBox.Show(sw.Elapsed.Seconds + "\r\n" + sw.Elapsed.Milliseconds.ToString());
                //sw.Stop();
                CurrentPath = treeView1.SelectedNode.FullPath.Remove(0, treeView1.Nodes[0].Text.Length + 1);
                string[] split = CurrentPath.Split('\\');
                CurrentPath = "";
                for (int i = 0; i < split.Length; i++)
                {
                    if (split[i].Contains(" | "))
                    {
                        split[i] = split[i].Remove(split[i].IndexOf(" | "));
                    }
                    CurrentPath += (i != split.Length - 1) ? split[i] + "\\" : split[i];
                }
                if (!DirectionButtonPressed)
                {
                    try
                    {
                        if (CurrentPathIndex != BrowsedPaths.Count - 1)
                        {
                            BrowsedPaths.RemoveRange(CurrentPathIndex + 1, BrowsedPaths.Count - (CurrentPathIndex + 1));
                        }
                    }
                    catch { }
                }
                pathBar.Text = CurrentPath;
                if (!DirectionButtonPressed)
                {
                    BrowsedPaths.Add(CurrentPath);
                    CurrentPathIndex = BrowsedPaths.Count - 1;
                    b_Back.Enabled = true;
                    b_Forward.Enabled = false;
                }
                DirectionButtonPressed = false;
            }
            else
            {
                foreach (MenuItem i in listview_ContextMenu.MenuItems)
                {
                    i.Enabled = false;
                }
            }
        }

        void treeView1_BeforeLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            Entry ye = ((Entry)e.Node.Tag);
            if (ye.IsDeleted)
            {
                e.CancelEdit = true;
                return;
            }
            e.Node.Text = e.Node.Name;
        }

        void treeView1_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            if (e.Label == null || e.Label == e.Node.Name)
            {
                //string Label = Cache.CheckCache(f.FullPath);
                //if (Label != null)
                //{
                    //tn.Text += " | " + Label;
                    //li.SubItems.Add(Label);
                //}
                //else
                //{
                    if (Cache.CheckCache(e.Node.Name) != null)
                    {
                        e.Node.Text += " | " + Cache.CheckCache(e.Node.Name);
                    }
                //}
                e.CancelEdit = true;
                return;
            }
            Entry ye = ((Entry)e.Node.Tag);
            if (e.Label.Length > 0x2A)
            {
                MessageBox.Show("Desired name is too long!", "Name Too Long", MessageBoxButtons.OK, MessageBoxIcon.Information);
                e.CancelEdit = true;
            }
            if (!ye.Rename(e.Label))
            {
                MessageBox.Show("Desired name invalid!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                e.CancelEdit = true;
                return;
            }
            TreeNode tn = e.Node;
            Folder f = (Folder)tn.Tag;
            //string Label = Cache.CheckCache(f.FullPath);
            //if (Label != null)
            //{
                //tn.Text += " | " + Label;
                //li.SubItems.Add(Label);
            //}

            if (f.IsTitleIDFolder)
            {
                if (Properties.Settings.Default.loadTIDNames)
                {
                    if (Cache.CheckCache(f.Name) != null)
                    {
                        tn.Text += " | " + Cache.CheckCache(f.Name);
                    }
                    // Didn't find it, need to grab that shit
                    else if (!f.IsKnownFolder)
                    {
                        if (f.GameName() != null && f.GameName() != "")
                        {
                            if (!f.IsKnownFolder)
                            {
                                Cache.AddID(f.Name, f.GameName());
                            }
                            tn.Text += " | " + f.GameName();
                        }
                    }
                }

                if (Properties.Settings.Default.SaveIcons)
                {
                    // Getting the icon
                    if (!f.IsKnownFolder || f.Name.ToLower() == "FFFE07D1".ToLower())
                    {
                        if (!siL.Images.Keys.Contains(f.Name.ToUpper()))
                        {
                            // Check the cache for the icon
                            if (Cache.GetIcon(f.Name) != null)
                            {
                                treeView1.Invoke((MethodInvoker)delegate
                                {
                                    siL.Images.Add(f.Name.ToUpper(), Cache.GetIcon(f.Name));
                                    liL.Images.Add(f.Name.ToUpper(), Cache.GetIcon(f.Name));
                                    tn.ImageKey = f.Name.ToUpper();
                                    tn.SelectedImageKey = tn.ImageKey;
                                });
                            }
                            else
                            {
                                if (f.GameIcon() != null)
                                {
                                    Cache.AddIcon(f.GameIcon(), f.Name);
                                    treeView1.Invoke((MethodInvoker)delegate
                                    {
                                        siL.Images.Add(f.Name.ToUpper(), f.GameIcon());
                                        liL.Images.Add(f.Name.ToUpper(), f.GameIcon());
                                        //treeView1.ImageList = SmallListForFATX;
                                        //listView1.SmallImageList = SmallListForFATX;
                                        //listView1.LargeImageList = LargeListForFATX;
                                        tn.ImageKey = f.Name.ToUpper();
                                        tn.SelectedImageKey = tn.ImageKey;
                                    }
                                    );
                                }
                            }
                        }
                        else
                        {
                            tn.ImageKey = f.Name.ToUpper();
                            tn.SelectedImageKey = tn.ImageKey;
                        }
                    }
                }
            }
            else
            {
                tn.ImageKey = "";
                tn.SelectedImageKey = tn.ImageKey;
            }
            tn.Tag = (Folder)ye;
            if (e.Node.RealPath() == treeView1.SelectedNode.Parent.RealPath())
            {
                ListViewItem li = listView1.Items.Find(tn.Name, false)[0];
                li.Name = e.Label;
                li.Text = li.Name;
                li.ImageKey = tn.ImageKey;
                tn.SelectedImageKey = tn.ImageKey;
            }
            tn.Name = e.Label;
        }

        void treeview_ContextMenu_Popup(object sender, EventArgs e)
        {
            if (rightClickedNode == null)
            {
                foreach (MenuItem mi in treeview_ContextMenu.MenuItems)
                {
                    mi.Enabled = false;
                }
                return;
            }
            if (rightClickedNode.Text.Contains('|'))
            {
                t_AddLabel.Text = "Add Label";
            }
            if (rightClickedNode == null)
            {
                foreach (MenuItem i in treeview_ContextMenu.MenuItems)
                {
                    i.Enabled = false;
                }
                return;
            }
            else
            {
                foreach (MenuItem i in treeview_ContextMenu.MenuItems)
                {
                    i.Enabled = true;
                }
            }
            if (Drive == null)
            {
                foreach (MenuItem i in treeview_ContextMenu.MenuItems)
                {
                    i.Enabled = false;
                }
                return;
            }
            else
            {
                foreach (MenuItem i in treeview_ContextMenu.MenuItems)
                {
                    i.Enabled = true;
                }
            }
            if (rightClickedNode == treeView1.Nodes[0])
            {
                foreach (MenuItem i in treeView1.ContextMenu.MenuItems)
                {
                    i.Enabled = false;
                }
                menuItem38.Enabled = t_driveProperties.Enabled =  menuItem39.Enabled = true;
                return;
            }
            if (((Folder)rightClickedNode.Tag).IsDeleted)
            {
                foreach (MenuItem i in treeview_ContextMenu.MenuItems)
                {
                    i.Enabled = false;
                }
                menuItem38.Enabled = true;
                menuItem39.Enabled = true;
                t_driveProperties.Enabled = true;
                t_Extract.Enabled = true;
                t_CopyPath.Enabled = true;
                return;
            }

            if (((Folder)rightClickedNode.Tag).IsTitleIDFolder || ((Folder)rightClickedNode.Tag).IsKnownFolder)
            {
                t_AddLabel.Enabled = false;
            }
            else
            {
                if (rightClickedNode.Text.Contains('|'))
                {
                    t_AddLabel.Text = "Edit Label";
                }
                t_AddLabel.Enabled = true;
            }

            if (rightClickedNode.Level == 1)
            {
                t_Delete.Enabled = false;
                t_AddLabel.Enabled = false;
                t_Move.Enabled = false;
            }
            else
            {
                t_Delete.Enabled = true;
                t_AddLabel.Enabled = true;
            }
        }

        void treeView1_MouseDown(object sender, MouseEventArgs e)
        {
            if (treeView1.SelectedNode != null && e.X != 0 && e.Y != 0)
            {
                rightClickedNode = treeView1.GetNodeAt(e.X, e.Y);
            }
        }

        #endregion

        #region Other Functions

        void Clear()
        {
            ts_Close.Enabled = false;
            m_CloseDrive.Enabled = false;
            BrowsedPaths = new List<string>();
            if (Drive != null)
            {
                Drive.Close();
            }
            Drive = null;
            treeView1.Nodes[0].ImageIndex = 2;
            treeView1.Nodes[0].Text = "Drive";
            treeView1.Nodes[0].Nodes.Clear();
            listView1.Items.Clear();
            m_drive.Enabled = false;
            m_Search.Enabled = false;
            m_Bookmarks.Enabled = false;
            treeView1.Nodes[0].Text = "Drive";
            treeView1.Nodes[0].ImageIndex = 2;
            treeView1.Nodes[0].SelectedImageIndex = 2;
        }

        private void LoadDrive()
        {
            if (Drive != null)
            {
                foreach (MenuItem i in listview_ContextMenu.MenuItems)
                {
                    i.Enabled = true;
                }
                Drive d = Drive;
                Clear();
                System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                sw.Start();
                d.Open();
                ts_Close.Enabled = true;
                m_CloseDrive.Enabled = true;
                switch (d.DriveType)
                {
                    case DriveType.HardDisk:
                        treeView1.Nodes[0].ImageIndex = 3;
                        treeView1.Nodes[0].SelectedImageIndex = treeView1.Nodes[0].ImageIndex;
                        break;
                    case DriveType.USB:
                        treeView1.Nodes[0].ImageIndex = 4;
                        treeView1.Nodes[0].SelectedImageIndex = treeView1.Nodes[0].ImageIndex;
                        break;
                    case DriveType.Backup:
                        treeView1.Nodes[0].ImageIndex = 5;
                        treeView1.Nodes[0].SelectedImageIndex = treeView1.Nodes[0].ImageIndex;
                        break;
                }
                treeView1.Nodes[0].Text = d.Name;
                m_drive.Enabled = true;
                m_Search.Enabled = true;
                m_Bookmarks.Enabled = true;
                Drive = d;
                Drive.EntryWatcher += new CLKsFATXLib.Structs.OnEntryEvent(Main_OnEntryEvent);
                // Grab the cached title names from the drive
                foreach (CLKsFATXLib.Structs.CachedTitleName ctn in Drive.CachedTitleNames())
                {
                    string Result = Cache.CheckCache(ctn.ID.ToString("X"));
                    if (Result == null)
                    {
                        Cache.AddID(ctn.ID.ToString("X"), ctn.Name);
                    }
                        // If we have this one cached but it doesn't equal
                        // what it should be
                    else if (Result != ctn.Name)
                    {
                        for (int i = 0; i < Properties.Settings.Default.cachedID.Count; i++)
                        {
                            if (Properties.Settings.Default.cachedID[i].ToLower() == ctn.ID.ToString("X").ToLower())
                            {
                                Properties.Settings.Default.cachedID.RemoveAt(i);
                                Properties.Settings.Default.correspondingIDName.RemoveAt(i);
                                Properties.Settings.Default.Save();
                                Cache.AddID(ctn.ID.ToString("X"), ctn.Name);
                            }
                        }
                    }
                }
                for (int i = 0; i < Drive.Partitions.Length; i++)
                {
                    TreeNode tn = new TreeNode(Drive.Partitions[i].Name);
                    tn.Tag = Drive.Partitions[i];
                    tn.Name = tn.Text;
                    //((Folder)tn.Tag).EntryEvent += new CLKsFATXLib.Structs.OnEntryEvent(Main_OnEntryEvent);
                    ((Folder)tn.Tag).ReturnDeletedEntries = m_DeletionMode.Checked;
                    if (Properties.Settings.Default.loadEntireDrive)
                    {
                        AddNodes(tn, Drive.Partitions[i], true, false);
                    }
                    treeView1.Nodes[0].Nodes.Add(tn);
                    Console.WriteLine(Drive.Partitions[i].Name + ": " + Drive.PartitionTimeStamp(Drive.Partitions[i].PartitionInfo));
                }
                treeView1.Nodes[0].Expand();
                sw.Stop();
                l_selectedItems.Text = d.Name + " loaded in " + sw.Elapsed.ToString();
            }
        }

        void ReloadNode(TreeNode current)
        {
            ((Folder)current.Tag).Reload();
        }

        void AddNodes(TreeNode current, Folder parent, bool AllTheWay, bool AddListviewItem)
        {
            List<ListViewItem> lvi = new List<ListViewItem>();
            Folder[] Folders = parent.Folders();
            for (int i = 0; i < Folders.Length; i++)
            {
                ListViewItem li = new ListViewItem(Folders[i].Name);
                li.Tag = Folders[i];
                li.SubItems.Add(Folders[i].EntryType);
                li.SubItems.Add("");
                li.SubItems.Add(Folders[i].ModifiedDate.ToString());
                li.ImageIndex = 0;
                li.Name = Folders[i].Name;
                TreeNode tn = new TreeNode(Folders[i].Name);
                tn.Name = Folders[i].Name;
                string Label = Cache.CheckCache(Folders[i].FullPath);
                if (Label != null)
                {
                    tn.Text += " | " + Label;
                    li.SubItems.Add(Label);
                }

                if (Folders[i].IsTitleIDFolder)
                {
                    if (Properties.Settings.Default.loadTIDNames)
                    {
                        if (Cache.CheckCache(Folders[i].Name) != null)
                        {
                            tn.Text += " | " + Cache.CheckCache(Folders[i].Name);
                            li.SubItems.Add(Cache.CheckCache(Folders[i].Name));
                        }
                        // Didn't find it, need to grab that shit
                        else if (!Folders[i].IsKnownFolder)
                        {
                            if (Folders[i].GameName() != null && Folders[i].GameName() != "")
                            {
                                if (!Folders[i].IsKnownFolder)
                                {
                                    Cache.AddID(Folders[i].Name, Folders[i].GameName());
                                }
                                tn.Text += " | " + Folders[i].GameName();
                                li.SubItems.Add(Folders[i].GameName());
                            }
                        }
                    }

                    if (Properties.Settings.Default.SaveIcons && (!Folders[i].IsKnownFolder || Folders[i].Name.ToLower() == "fffe07d1"))
                    {
                        // Getting the icon
                        if (!Folders[i].IsKnownFolder || Folders[i].Name.ToLower() == "FFFE07D1".ToLower())
                        {
                            if (!siL.Images.Keys.Contains(Folders[i].Name.ToUpper()))
                            {
                                // Check the cache for the icon
                                if (Cache.GetIcon(Folders[i].Name) != null)
                                {
                                    siL.Images.Add(Folders[i].Name.ToUpper(), Cache.GetIcon(Folders[i].Name));
                                    liL.Images.Add(Folders[i].Name.ToUpper(), Cache.GetIcon(Folders[i].Name));
                                    tn.ImageKey = Folders[i].Name.ToUpper();
                                    tn.SelectedImageKey = tn.ImageKey;
                                    li.ImageKey = Folders[i].Name.ToUpper();
                                }
                                else
                                {
                                    if ( Folders[i].GameName() != null && Folders[i].GameIcon() != null)
                                    {
                                        Cache.AddIcon(Folders[i].GameIcon(), Folders[i].Name);
                                        siL.Images.Add(Folders[i].Name.ToUpper(), Folders[i].GameIcon());
                                        liL.Images.Add(Folders[i].Name.ToUpper(), Folders[i].GameIcon());
                                        //treeView1.ImageList = SmallListForFATX;
                                        //listView1.SmallImageList = SmallListForFATX;
                                        //listView1.LargeImageList = LargeListForFATX;
                                        tn.ImageKey = Folders[i].Name.ToUpper();
                                        tn.SelectedImageKey = tn.ImageKey;
                                        li.ImageKey = Folders[i].Name.ToUpper();
                                    }
                                }
                            }
                            else
                            {
                                tn.ImageKey = Folders[i].Name.ToUpper();
                                tn.SelectedImageKey = tn.ImageKey;
                                li.ImageKey = Folders[i].Name.ToUpper();
                            }
                        }
                    }
                }
                // For profile folders
                else if (Properties.Settings.Default.SaveIcons)
                {
                    Image I = Cache.GetIcon(Folders[i].Name);
                    if (I != null)
                    {
                        treeView1.Invoke((MethodInvoker)delegate
                        {
                            if (!siL.Images.ContainsKey(Folders[i].Name.ToUpper()))
                            {
                                siL.Images.Add(Folders[i].Name.ToUpper(), I);
                                liL.Images.Add(Folders[i].Name.ToUpper(), I);
                            }
                            tn.ImageKey = Folders[i].Name.ToUpper();
                            tn.SelectedImageKey = tn.ImageKey;
                            li.ImageKey = Folders[i].Name.ToUpper();
                        });
                    }
                    else
                    {
                        File FILE = Folders[i].IsProfileFolder();
                        if (FILE != null)
                        {
                            I = FILE.ContentIcon();
                            if (I != null)
                            {
                                Cache.AddIcon(I, Folders[i].Name);
                                if (!siL.Images.ContainsKey(Folders[i].Name.ToUpper()))
                                {
                                    siL.Images.Add(Folders[i].Name.ToUpper(), I);
                                    liL.Images.Add(Folders[i].Name.ToUpper(), I);
                                }
                                tn.ImageKey = Folders[i].Name.ToUpper();
                                tn.SelectedImageKey = tn.ImageKey;
                                li.ImageKey = Folders[i].Name.ToUpper();
                            }
                        }
                    }
                }
                tn.ToolTipText = tn.Text;
                tn.Tag = Folders[i];

                // If we already have a treenode under the current node with
                // the same name...
                if (current.Nodes.Find(tn.Name, false).Length != 0)
                {

                }
                else
                {
                    AddNodes(tn, Folders[i], AllTheWay, false);
                    current.Nodes.Add(tn);
                }

                if (!AllTheWay && AddListviewItem)
                {
                    if (listView1.Items.Find(li.Name, false).Length == 0)
                    {
                        lvi.Add(li);
                    }
                }
            }
            listView1.Items.AddRange(lvi.ToArray());
        }

        void AddNode(Folder f, TreeNode n, ListViewCreateItem CreateListViewItem)
        {
            ListViewItem li = new ListViewItem(f.Name);
            li.Tag = f;
            li.SubItems.Add(f.EntryType);
            li.SubItems.Add("");
            li.SubItems.Add(f.ModifiedDate.ToString());
            li.ImageIndex = 0;
            li.Name = f.Name;
            TreeNode tn = new TreeNode(f.Name);
            tn.Name = f.Name;
            // Check our cache to see if we have cached this title ID

            string Label = Cache.CheckCache(f.FullPath);
            if (Label != null)
            {
                tn.Text += " | " + Label;
                li.SubItems.Add(Label);
            }

            if (f.IsTitleIDFolder)
            {
                if (Properties.Settings.Default.loadTIDNames)
                {
                    if (Cache.CheckCache(f.Name) != null)
                    {
                        tn.Text += " | " + Cache.CheckCache(f.Name);
                        li.SubItems.Add(Cache.CheckCache(f.Name));
                    }
                    // Didn't find it, need to grab that shit
                    else if (!f.IsKnownFolder )
                    {
                        if (f.GameName() != null && f.GameName() != "")
                        {
                            if (!f.IsKnownFolder)
                            {
                                Cache.AddID(f.Name, f.GameName());
                            }
                            tn.Text += " | " + f.GameName();
                            li.SubItems.Add(f.GameName());
                        }
                    }
                }

                if (Properties.Settings.Default.SaveIcons)
                {
                    // Getting the icon
                    if (!f.IsKnownFolder || f.Name.ToLower() == "FFFE07D1".ToLower())
                    {
                        if (!siL.Images.Keys.Contains(f.Name.ToUpper()))
                        {
                            // Check the cache for the icon
                            if (Cache.GetIcon(f.Name) != null)
                            {
                                treeView1.Invoke((MethodInvoker)delegate
                                {
                                    if (!siL.Images.ContainsKey(f.Name.ToUpper()))
                                    {
                                        siL.Images.Add(f.Name.ToUpper(), Cache.GetIcon(f.Name));
                                        liL.Images.Add(f.Name.ToUpper(), Cache.GetIcon(f.Name));
                                    }
                                    tn.ImageKey = f.Name.ToUpper();
                                    tn.SelectedImageKey = tn.ImageKey;
                                    li.ImageKey = f.Name.ToUpper();
                                });
                            }
                            else
                            {
                                if (f.GameIcon() != null)
                                {
                                    Cache.AddIcon(f.GameIcon(), f.Name);
                                    treeView1.Invoke((MethodInvoker)delegate
                                    {
                                        if (!siL.Images.ContainsKey(f.Name.ToUpper()))
                                        {
                                            siL.Images.Add(f.Name.ToUpper(), f.GameIcon());
                                            liL.Images.Add(f.Name.ToUpper(), f.GameIcon());
                                        }
                                        tn.ImageKey = f.Name.ToUpper();
                                        tn.SelectedImageKey = tn.ImageKey;
                                        li.ImageKey = f.Name.ToUpper();
                                    }
                                    );
                                }
                            }
                        }
                        else
                        {
                            tn.ImageKey = f.Name.ToUpper();
                            tn.SelectedImageKey = tn.ImageKey;
                            li.ImageKey = f.Name.ToUpper();
                        }
                    }
                        // For profile folders
                    else
                    {
                        Image I = Cache.GetIcon(f.Name);
                        if (I != null)
                        {
                            treeView1.Invoke((MethodInvoker)delegate
                            {
                                if (!siL.Images.ContainsKey(f.Name.ToUpper()))
                                {
                                    siL.Images.Add(f.Name.ToUpper(), I);
                                    liL.Images.Add(f.Name.ToUpper(), I);
                                }
                                tn.ImageKey = f.Name.ToUpper();
                                tn.SelectedImageKey = tn.ImageKey;
                                li.ImageKey = f.Name.ToUpper();
                            });
                        }
                        else
                        {
                            File FILE = f.IsProfileFolder();
                            if (FILE != null)
                            {
                                I = FILE.ContentIcon();
                                if (I != null)
                                {
                                    Cache.AddIcon(I, f.Name);
                                    if (!siL.Images.ContainsKey(f.Name.ToUpper()))
                                    {
                                        siL.Images.Add(f.Name.ToUpper(), I);
                                        liL.Images.Add(f.Name.ToUpper(), I);
                                    }
                                    tn.ImageKey = f.Name.ToUpper();
                                    tn.SelectedImageKey = tn.ImageKey;
                                    li.ImageKey = f.Name.ToUpper();
                                }
                            }
                        }
                    }
                }
            }
            tn.ToolTipText = tn.Text;
            tn.Tag = (Folder)f;
            treeView1.Invoke((MethodInvoker)delegate
            {
                n.Nodes.Add(tn);
                if (CreateListViewItem == ListViewCreateItem.DecideForMeNigga)
                {
                    if (((Folder)n.Tag).FullPath == treeView1.SelectedNode.RealPath())
                    {
                        listView1.Invoke((MethodInvoker)delegate { listView1.Items.Add(li); });
                    }
                }
                else if (CreateListViewItem == ListViewCreateItem.FuckYes)
                {
                    listView1.Invoke((MethodInvoker)delegate { listView1.Items.Add(li); });
                }
            });
        }

        enum ListViewCreateItem
        {
            DecideForMeNigga,
            FuckYes,
            FuckNo,
        }

        void Main_OnEntryEvent(ref CLKsFATXLib.Structs.EntryEventArgs e)
        {
            CLKsFATXLib.Structs.EntryEventArgs eg = e;
            if (e.Deleting)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    if (!eg.ModifiedEntry.IsFolder)
                    {
                        if (eg.ParentFolder.FullPath == treeView1.SelectedNode.RealPath())
                        {
                            for (int i = 0; i < listView1.Items.Count; i++)
                            {
                                if (listView1.Items[i].Name.ToLower() == eg.ModifiedEntry.Name.ToLower())
                                {
                                    listView1.Items[i].Remove();
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        try
                        {
                            TreeNode deletedNode = treeView1.NodeFromPath(eg.ModifiedEntry.FullPath);
                            if (deletedNode.Parent == treeView1.SelectedNode)
                            {
                                listView1.Invoke((MethodInvoker)delegate 
                                {
                                    ListViewItem li = listView1.Items.Find(deletedNode.Name, false)[0];
                                    li.Remove();
                                });
                            }
                            else if (deletedNode == treeView1.SelectedNode)
                            {
                                treeView1.SelectedNode = deletedNode.Parent;
                            }
                            deletedNode.Remove();
                        }
                        catch { }
                    }
                });
                return;
            }
            TreeNode n = null;
            try
            {
                n = treeView1.NodeFromPath(eg.FullParentPath);
                TreeNode[] nodes = n.Nodes.Find(e.ModifiedEntry.Name, false);
                foreach (TreeNode nodenigga in nodes)
                {
                    if (((Folder)nodenigga.Tag).IsDeleted == eg.ModifiedEntry.IsDeleted)
                    {
                        return;
                    }
                }
            }
            catch (Exception f)
            {
                AddNode(eg.ParentFolder, treeView1.NodeFromPath(((eg.FullParentPath.Contains('\\')) ? eg.FullParentPath.Remove(eg.FullParentPath.LastIndexOf('\\')) : eg.FullParentPath)), ListViewCreateItem.DecideForMeNigga);
                n = treeView1.NodeFromPath(eg.FullParentPath);
            }
            n.Tag = eg.ParentFolder;

            // If we're dealing with a folder...
            if (eg.ModifiedEntry.IsFolder)
            {
                AddNode((Folder)eg.ModifiedEntry, n, ListViewCreateItem.DecideForMeNigga);
            }
                // If we're dealing with a file
            else
            {
                this.Invoke((MethodInvoker)delegate
                {
                    // If the parent folder to this file is the selected path
                    if (((Folder)n.Tag).FullPath == treeView1.SelectedNode.RealPath())
                    {
                        // Add all the listview shit
                        ListViewItem li2 = null;
                        bool ExistsAlready = false;
                        if (listView1.Items.Find(((File)eg.ModifiedEntry).Name, false).Length != 0)
                        {
                            li2 = listView1.Items.Find(((File)eg.ModifiedEntry).Name, false)[0];
                            foreach (ListViewItem.ListViewSubItem subitem in li2.SubItems)
                            {
                                li2.SubItems.Remove(subitem);
                            }
                            ExistsAlready = true;
                        }
                        else
                        {
                            li2 = new ListViewItem(((File)eg.ModifiedEntry).Name);
                        }
                        li2.SubItems.Add(((File)eg.ModifiedEntry).EntryType);
                        li2.SubItems.Add(VariousFunctions.ByteConversion(((File)eg.ModifiedEntry).Size));
                        li2.SubItems.Add(((File)eg.ModifiedEntry).ModifiedDate.ToString());
                        if (Properties.Settings.Default.loadSTFS)
                        {
                            if (((File)eg.ModifiedEntry).IsSTFSPackage())
                            {
                                li2.SubItems.Add(((File)eg.ModifiedEntry).ContentName());
                            }
                            else if (((File)eg.ModifiedEntry).Parent.FullPath == ((File)eg.ModifiedEntry).Drive.CacheFolderPath && CLKsFATXLib.Geometry.CacheFilePrefixes.CachePrefixes.Contains(((File)eg.ModifiedEntry).Name.Substring(0, 2)))
                            {
                                for (int i = 0; i < CLKsFATXLib.Geometry.CacheFilePrefixes.CachePrefixes.Length; i++)
                                {
                                    if (CLKsFATXLib.Geometry.CacheFilePrefixes.CachePrefixes[i] == ((File)eg.ModifiedEntry).Name.Substring(0, 2))
                                    {
                                        li2.SubItems.Add(CLKsFATXLib.Geometry.CacheFilePrefixes.PrefixNames[i]);
                                        break;
                                    }
                                }
                            }
                        }
                        li2.Tag = ((File)eg.ModifiedEntry);
                        li2.Name = ((File)eg.ModifiedEntry).Name;
                        if (Properties.Settings.Default.cacheContentIcons)
                        {
                            if (listView1.LargeImageList.Images.ContainsKey(((File)eg.ModifiedEntry).Name))
                            {
                                li2.ImageKey = ((File)eg.ModifiedEntry).Name;
                            }
                            else
                            {
                                // Check if there's an icon for this guy cached
                                if (Cache.GetIcon(((File)eg.ModifiedEntry).Name) != null)
                                {
                                    Image Icon = Cache.GetIcon(((File)eg.ModifiedEntry).Name);
                                    treeView1.ImageList.Images.Add(((File)eg.ModifiedEntry).Name, Icon);
                                    listView1.Invoke((MethodInvoker)delegate
                                    {
                                        listView1.LargeImageList.Images.Add(((File)eg.ModifiedEntry).Name, Icon);
                                        listView1.SmallImageList.Images.Add(((File)eg.ModifiedEntry).Name, Icon);
                                    });
                                    li2.ImageKey = ((File)eg.ModifiedEntry).Name;
                                }
                                // Check if the file is an STFS package, and if it is blahblahblah
                                else if (((File)eg.ModifiedEntry).IsSTFSPackage())
                                {
                                    Image Icon = ((File)eg.ModifiedEntry).ContentIcon();
                                    if (Icon != null)
                                    {
                                        Cache.AddIcon(Icon, ((File)eg.ModifiedEntry).Name);
                                        treeView1.ImageList.Images.Add(((File)eg.ModifiedEntry).Name, Icon);
                                        listView1.Invoke((MethodInvoker)delegate
                                        {
                                            listView1.LargeImageList.Images.Add(((File)eg.ModifiedEntry).Name, Icon);
                                            listView1.SmallImageList.Images.Add(((File)eg.ModifiedEntry).Name, Icon);
                                        });
                                        li2.ImageKey = ((File)eg.ModifiedEntry).Name;
                                    }
                                }
                            }
                        }
                        if (li2.ImageKey == "")
                        {
                            li2.ImageIndex = 1;
                        }
                        if (!ExistsAlready)
                        {
                            listView1.Invoke((MethodInvoker)delegate { listView1.Items.Add(li2); });
                        }
                    }
                    /* Here we'll check to see if this is the first file added
                     * to the folder.  If it is, grab the icon/name */
                    if (((File)eg.ModifiedEntry).Parent.Files().Length == 1)
                    {
                        if (Cache.IsTitleIDFolder(n.Name) && VariousFunctions.Known.Contains(n.Name))
                        {
                            n = n.Parent;
                        }
                        File f = ((File)eg.ModifiedEntry).Parent.Files()[0];
                        // It was the first file, do the name and icon stuff
                        if (!n.Text.Contains(" | ") && ((File)eg.ModifiedEntry).Parent.Parent.IsTitleIDFolder && !((File)eg.ModifiedEntry).Parent.Parent.IsKnownFolder)
                        {
                            ((File)eg.ModifiedEntry).Parent.Parent.ForceGameName = true;
                            string Name = ((File)eg.ModifiedEntry).Parent.Parent.GameName();
                            ((File)eg.ModifiedEntry).Parent.Parent.ForceGameName = false;
                            if (Name != null && Name != "")
                            {
                                n.Text += " | " + ((File)eg.ModifiedEntry).Parent.Parent.GameName();
                            }
                            // If there is no image assigned to the node
                            if (n.ImageKey == "")
                            {
                                if (Properties.Settings.Default.SaveIcons)
                                {
                                    Image Icon = ((File)eg.ModifiedEntry).Parent.Parent.GameIcon();
                                    if (Icon != null)
                                    {
                                        treeView1.Invoke((MethodInvoker)delegate
                                        {
                                            siL.Images.Add(((File)eg.ModifiedEntry).Name.ToUpper(), Icon);
                                            liL.Images.Add(((File)eg.ModifiedEntry).Name.ToUpper(), Icon);
                                            n.ImageKey = ((File)eg.ModifiedEntry).Name.ToUpper();
                                            n.SelectedImageKey = n.ImageKey;
                                        });
                                    }
                                }
                            }
                            // Now check to see if the parent folder is being displayed in the listview...
                            if (n.Parent.RealPath() == ((File)eg.ModifiedEntry).Parent.Parent.Parent.Parent.FullPath)
                            {
                                // OH SHIT IT IS
                                // Change the STFS name and imagekey
                                ListViewItem li = listView1.Items.Find(n.Name, false)[0];
                                if (Name != null)
                                {
                                    li.SubItems.Add(Name);
                                }
                                if (n.ImageKey != "" && li.ImageKey == "")
                                {
                                    li.ImageKey = n.ImageKey;
                                }
                            }
                        }
                            // For profile folders...
                        if (n.Name.ToLower() == "fffe07d1" && n.Parent.ImageKey == "" && Properties.Settings.Default.SaveIcons)
                        {
                            n = n.Parent;
                            Folder ProfileFolder = ((File)eg.ModifiedEntry).Parent.Parent.Parent;
                            // Treeview Handling
                            Image I = Cache.GetIcon(ProfileFolder.Name);
                            {
                                if (I != null)
                                {
                                    treeView1.Invoke((MethodInvoker)delegate
                                    {
                                        if (!siL.Images.ContainsKey(ProfileFolder.Name.ToUpper()))
                                        {
                                            siL.Images.Add(ProfileFolder.Name.ToUpper(), I);
                                            liL.Images.Add(ProfileFolder.Name.ToUpper(), I);
                                        }
                                        n.ImageKey = ProfileFolder.Name.ToUpper();
                                        n.SelectedImageKey = n.ImageKey;
                                    });
                                }
                                else
                                {
                                    File FILE = ProfileFolder.IsProfileFolder();
                                    if (FILE != null)
                                    {
                                        I = FILE.ContentIcon();
                                        if (I != null)
                                        {
                                            Cache.AddIcon(I, ProfileFolder.Name);
                                            if (!siL.Images.ContainsKey(ProfileFolder.Name.ToUpper()))
                                            {
                                                siL.Images.Add(ProfileFolder.Name.ToUpper(), I);
                                                liL.Images.Add(ProfileFolder.Name.ToUpper(), I);
                                            }
                                            n.ImageKey = (ProfileFolder.Name.ToUpper());
                                            n.SelectedImageKey = n.ImageKey;
                                        }
                                    }
                                }
                            }
                            // Listview handling
                            if (I != null && n.Parent.RealPath() == ProfileFolder.Parent.FullPath)
                            {
                                ListViewItem li = listView1.Items.Find(n.Name, false)[0];
                                if (n.ImageKey != "" && li.ImageKey == "")
                                {
                                    li.ImageKey = n.ImageKey;
                                }
                            }
                        }
                    }
                });
            }
        }

        private string GetNewFolderName(Folder f)
        {
            int FoldersFound = 0;
            string NewName = "New Folder";
            foreach (Folder g in f.Folders())
            {
                if (g.Name == NewName)
                {
                    FoldersFound++;
                    NewName = "New Folder (" + FoldersFound.ToString() + ")";
                }
            }
            return NewName;
        }

        void Main_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal)
            {
                Properties.Settings.Default.width = this.Width;
                Properties.Settings.Default.height = this.Height;
                Properties.Settings.Default.Save();
            }
        }

        void DoStartup()
        {
            if (Properties.Settings.Default.checkForUpdates)
            {
                #region Update Shit

                Party_Buffalo.Update u = new Update();
#if TRACE
                Party_Buffalo.Update.UpdateInfo ud = u.CheckForUpdates(new Uri("http://clkxu5.com/drivexplore/coolapplicationstuff.xml"));
#endif
#if DEBUG
                Party_Buffalo.Update.UpdateInfo ud = new Update.UpdateInfo();
                if (UpdateDebug)
                {
                    if (!UpdateDebugForce)
                    {
                        ud = u.CheckForUpdates(new Uri("http://clkxu5.com/drivexplore/dev/coolapplicationstuff.xml"));
                    }
                    else
                    {
                        ud.Update = true;
                        ud.UpdateText = "This`Is`A`Test";
                        ud.UpdateVersion = "9000";
                    }
                }
#endif
                if (ud.Update)
                {
                    if (!Aero)
                    {
                        Forms.UpdateForm uf = new Party_Buffalo.Forms.UpdateForm(ud);
                        uf.ShowDialog();
                    }
                    else
                    {
                        TaskDialog td = new TaskDialog();
                        td.Caption = "Update Available";
                        td.InstructionText = "Version " + ud.UpdateVersion + " is Available";
                        td.Text = "An update is available for Party Buffalo";
                        td.DetailsCollapsedLabel = "Change Log";
                        td.DetailsExpandedLabel = "Change Log";
                        td.ExpansionMode = TaskDialogExpandedDetailsLocation.ExpandFooter;
                        string[] Split = ud.UpdateText.Split('`');
                        string UpdateText = "";
                        for (int i = 0; i < Split.Length; i++)
                        {
                            if (i != 0)
                            {
                                UpdateText += "\r\n";
                            }
                            if (Split[i] == "")
                            {
                                continue;
                            }
                            UpdateText += "-" + Split[i];
                        }
                        td.DetailsExpandedText = UpdateText;
                        TaskDialogCommandLink Download = new TaskDialogCommandLink("Download", "Download Party Buffalo version " + ud.UpdateVersion, "");
                        Download.Click += (o, f) =>
                        {
                            Forms.UpdateDownloadForm udf = new Party_Buffalo.Forms.UpdateDownloadForm(ud);
                            udf.ShowDialog();
                        };

                        TaskDialogCommandLink DontDownload = new TaskDialogCommandLink("DontDownload", "Let me go about my business and I'll download this stuff later...", "");
                        DontDownload.Click += (o, f) =>
                        {
                            td.Close();
                        };
                        td.Controls.Add(Download);
                        td.Controls.Add(DontDownload);
                        td.Show();
                    }
                }
                if (ud.QuickMessage != null && ud.QuickMessage != "")
                {
                    quickMessage.Text = ud.QuickMessage;
                }
                else
                {
                    quickMessage.Text = "Could not load quick message";
                }

                #endregion
            }
            else
            {
                m_DisableUpdates.Checked = true;
            }

            if (!Properties.Settings.Default.Upgraded)
            {
                Properties.Settings.Default.Upgrade();
                Properties.Settings.Default.Upgraded = true;
                Properties.Settings.Default.Save();
            }
            if (Properties.Settings.Default.width > 50 && Properties.Settings.Default.height > 50)
            {
                this.Height = Properties.Settings.Default.height;
                this.Width = Properties.Settings.Default.width;
            }
            quickAddMode.Checked = Properties.Settings.Default.quickAddMode;
            SaveIcons.Checked = Properties.Settings.Default.SaveIcons;
            m_loadEntireDrive.Checked = Properties.Settings.Default.loadEntireDrive;
            m_LoadSTFS.Checked = Properties.Settings.Default.loadSTFS;
            m_retrieveGameNames.Checked = Properties.Settings.Default.loadTIDNames;
            cacheContentIcons.Checked = Properties.Settings.Default.cacheContentIcons;
            if (Properties.Settings.Default.hideToolBar)
            {
                m_hideToolBar.PerformClick();
            }
            if (Properties.Settings.Default.hideAddressBar)
            {
                m_hideAddressBar.PerformClick();
            }
            if (Properties.Settings.Default.hideStatusBar)
            {
                m_hideStatusBar.PerformClick();
            }
            switch (Properties.Settings.Default.view)
            {
                case View.Details:
                    c_Details.Checked = true;
                    listView1.View = View.Details;
                    break;
                case View.LargeIcon:
                    c_largeIcon.Checked = true;
                    listView1.View = View.LargeIcon;
                    break;
                case View.List:
                    c_List.Checked = true;
                    listView1.View = View.List;
                    break;
                case View.SmallIcon:
                    c_SmallIcon.Checked = true;
                    listView1.View = View.SmallIcon;
                    break;
                case View.Tile:
                    c_Tile.Checked = true;
                    listView1.View = View.Tile;
                    break;

            }

            if (Properties.Settings.Default.recentFiles != null)
            {
                List<string> Remove = new List<string>();
                foreach (string s in Properties.Settings.Default.recentFiles)
                {
                    if (System.IO.File.Exists(s))
                    {
                        MenuItem i = new MenuItem(s);
                        i.Click += new EventHandler(RecentFileHandler);
                        m_Open.MenuItems.Add(i);
                    }
                    else
                    {
                        Remove.Add(s);
                    }
                }

                for (int i = 0; i < Remove.Count; )
                {
                    Properties.Settings.Default.recentFiles.Remove(Remove[i]);
                    Remove.RemoveAt(i);
                }
            }

            List<MenuItem> mil = new List<MenuItem>();
            List<MenuItem> mit = new List<MenuItem>();

            // Add the "new known folder" (cached) items
            for (int i = 0; i < CLKsFATXLib.VariousFunctions.Known.Length; i++)
            {
                string ss = CLKsFATXLib.VariousFunctions.Known[i];
                string s = CLKsFATXLib.VariousFunctions.KnownEquivilent[i];
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

            // For treeview icon size
            switch (Properties.Settings.Default.treeViewIconWidthHeight)
            {
                case 16:
                    size16.Checked = true;
                    break;
                case 24:
                    size24.Checked = true;
                    break;
                case 32:
                    size32.Checked = true;
                    break;
                case 64:
                    size64.Checked = true;
                    break;
            }
            treeView1.ImageList.ImageSize = new Size(Properties.Settings.Default.treeViewIconWidthHeight, Properties.Settings.Default.treeViewIconWidthHeight);
        }

        void mut_Click(object sender, EventArgs e)
        {
            Folder f = (Folder)rightClickedNode.Tag;
            if (!f.IsDeleted)
            {
                foreach (Folder Fol in f.Folders())
                {
                    if (Fol.Name.ToLower() == ((string)((MenuItem)sender).Tag).ToLower())
                    {
                        MessageBox.Show("Folder already exists!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                }
                f.CreateNewFolder(((string)((MenuItem)sender).Tag));
                rightClickedNode.Tag = f;
            }
        }

        void mu_Click(object sender, EventArgs e)
        {
            Folder f = (Folder)treeView1.SelectedNode.Tag;
            if (f.IsDeleted)
            {
                return;
            }
            foreach (Folder Fol in f.Folders())
            {
                if (Fol.Name.ToLower() == ((string)((MenuItem)sender).Tag).ToLower())
                {
                    MessageBox.Show("Folder already exists!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            f.CreateNewFolder(((string)((MenuItem)sender).Tag));
            treeView1.SelectedNode.Tag = f;
        }

        void RecentFileHandler(object sender, EventArgs e)
        {
            Drive = new Drive(((MenuItem)sender).Text);
            LoadDrive();
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

        private void BookmarkGo(object sender, EventArgs e)
        {
            treeView1.SelectedNode = treeView1.NodeFromPath((string)((MenuItem)sender).Tag);
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

        void ListviewLabelUpdate()
        {
            int Files = 0;
            int Folders = 0;
            long size = 0;
            foreach (ListViewItem i in listView1.SelectedItems)
            {
                if (((Entry)i.Tag).IsFolder)
                {
                    Folders++;
                }
                else
                {
                    size += ((File)i.Tag).Size;
                    Files++;
                }
            }

            if (Folders == 0 && Files > 0)
            {
                string f = "files";
                if (Files == 1)
                {
                    f = "file";
                }
                l_selectedItems.Text = Files.ToString() + " " + f + " selected for a total of " + VariousFunctions.ByteConversion(size);
            }
            else if (Folders > 0 && Files > 0)
            {
                string file = "files";
                if (Files == 1)
                {
                    file = "file";
                }
                string folder = "folders";
                if (Folders == 1)
                {
                    folder = "folder";
                }
                l_selectedItems.Text = Folders.ToString() + " " + folder + " and " + Files.ToString() + " " + file + " selected for a total of " + VariousFunctions.ByteConversion(size);
            }
            else if (Folders > 0 && Files == 0)
            {
                string f = "folders";
                if (Folders == 1)
                {
                    f = "folder";
                }
                l_selectedItems.Text = Folders.ToString() + " " + f + " selected";
            }
            else
            {
                l_selectedItems.Text = "";
            }
        }

        void Reload()
        {
            if (Drive != null)
            {
                CLKsFATXLib.Drive d = Drive;
                Clear();
                switch (d.DriveType)
                {
                    case DriveType.USB:
                        Drive = new Drive(d.USBPaths);
                        break;
                    case DriveType.HardDisk:
                        Drive = new Drive(d.DeviceIndex);
                        break;
                    case DriveType.Backup:
                        Drive = new Drive(d.FilePath);
                        break;
                }
                LoadDrive();
            }
        }

        #endregion

        private void lCached_Click(object sender, EventArgs e)
        {
            Forms.NewKnownFolder n = new Forms.NewKnownFolder();
            if (n.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    Folder f = (Folder)treeView1.SelectedNode.Tag;
                    if (f.IsDeleted)
                    {
                        return;
                    }
                    foreach (Folder Fol in f.Folders())
                    {
                        if (Fol.Name.ToLower() == (n.Selected.ToLower()))
                        {
                            MessageBox.Show("Folder already exists!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        }
                    }
                    f.CreateNewFolder(n.Selected);
                    treeView1.SelectedNode.Tag = f;
                }
                catch(Exception x)
                {
                    if (!Aero)
                    {
                        MessageBox.Show("An exception was thrown: " + x.Message + "\r\n\r\nIf this appears to be a bug, press CTRL + C to copy the stack trace, then please email it to me at clkxu5@gmail.com:\r\n" + x.StackTrace);
                    }
                    else
                    {
                        TaskDialog td = new TaskDialog();
                        td.Caption = "Unhandled Exception";
                        td.InstructionText = "An Unhandled Exception was Thrown";
                        td.Text = string.Format("An exception was thrown: {0}\r\n\r\nIf this appears to be a bug, please email me at clkxu5@gmail.com with the details below", x.Message);
                        td.DetailsCollapsedLabel = "Details";
                        td.DetailsExpandedLabel = "Details";
                        td.DetailsExpandedText = x.StackTrace;

                        TaskDialogButton Copy = new TaskDialogButton("Copy", "Copy Details to Clipboard");
                        Copy.Click += (o, f) => { Clipboard.SetDataObject(x.StackTrace, true, 10, 200); };

                        TaskDialogButton Close = new TaskDialogButton("Close", "Close");
                        Close.Click += (o, f) => { td.Close(); };

                        td.Controls.Add(Copy);
                        td.Controls.Add(Close);
                    }
                    Clear();
                }
            }
        }

        private void tCached_Click(object sender, EventArgs e)
        {
            Forms.NewKnownFolder n = new Forms.NewKnownFolder();
            if (n.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    Folder f = (Folder)rightClickedNode.Tag;
                    if (!f.IsDeleted)
                    {
                        foreach (Folder Fol in f.Folders())
                        {
                            if (Fol.Name.ToLower() == (n.Selected.ToLower()))
                            {
                                MessageBox.Show("Folder already exists!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            }
                        }
                        f.CreateNewFolder(n.Selected);
                        rightClickedNode.Tag = f;
                    }
                }
                catch(Exception x)
                {
                    if (!Aero)
                    {
                        MessageBox.Show("An exception was thrown: " + x.Message + "\r\n\r\nIf this appears to be a bug, press CTRL + C to copy the stack trace, then please email it to me at clkxu5@gmail.com:\r\n" + x.StackTrace);
                    }
                    else
                    {
                        TaskDialog td = new TaskDialog();
                        td.Caption = "Unhandled Exception";
                        td.InstructionText = "An Unhandled Exception was Thrown";
                        td.Text = string.Format("An exception was thrown: {0}\r\n\r\nIf this appears to be a bug, please email me at clkxu5@gmail.com with the details below", x.Message);
                        td.DetailsCollapsedLabel = "Details";
                        td.DetailsExpandedLabel = "Details";
                        td.DetailsExpandedText = x.StackTrace;

                        TaskDialogButton Copy = new TaskDialogButton("Copy", "Copy Details to Clipboard");
                        Copy.Click += (o, f) => { Clipboard.SetDataObject(x.StackTrace, true, 10, 200); };

                        TaskDialogButton Close = new TaskDialogButton("Close", "Close");
                        Close.Click += (o, f) => { td.Close(); };

                        td.Controls.Add(Copy);
                        td.Controls.Add(Close);
                    }
                    Clear();
                }
            }
        }

        private void t_AddLabel_Click(object sender, EventArgs e)
        {
            Forms.AddLabel al = new Party_Buffalo.Forms.AddLabel(((Folder)rightClickedNode.Tag).FullPath, ((rightClickedNode.Text.Contains('|')) ? rightClickedNode.GameName() : rightClickedNode.Name));
            if (rightClickedNode.Text.Contains('|'))
            {
                al.Text = "Edit Label";
                al.button1.Text = "Edit Label";
            }
            else
            {
                al.Text = "Add Label";
            }
            if (al.ShowDialog() == DialogResult.OK)
            {
                rightClickedNode.Text = rightClickedNode.Name + " | " + al.Label;
                if (rightClickedNode.Parent == treeView1.SelectedNode)
                {
                    ListViewItem li = listView1.Items.Find(rightClickedNode.Name, false)[0];
                    if (li.SubItems.Count == 5)
                    {
                        li.SubItems[4].Text = al.Label;
                    }
                    else
                    {
                        li.SubItems.Add(al.Label);
                    }
                }
            }
        }

        private void b_Back_Click(object sender, EventArgs e)
        {
            if (CurrentPathIndex == 0)
            {
                return;
            }
            DirectionButtonPressed = true;
            CurrentPathIndex--;
            b_Forward.Enabled = true;
            if (CurrentPathIndex == 0)
            {
                b_Back.Enabled = false;
            }
            pathBar.Text = BrowsedPaths[CurrentPathIndex];
            goButton_Click(null, null);
        }

        private void b_Forward_Click(object sender, EventArgs e)
        {
            DirectionButtonPressed = true;
            CurrentPathIndex++;
            b_Back.Enabled = true;
            if (CurrentPathIndex == BrowsedPaths.Count - 1)
            {
                b_Forward.Enabled = false;
            }
            pathBar.Text = BrowsedPaths[CurrentPathIndex];
            goButton_Click(null, null);
        }

        private void menuItem8_Click(object sender, EventArgs e)
        {
            Forms.DeviceSelector ds1 = new Party_Buffalo.Forms.DeviceSelector();
            if (ds1.ShowDialog() == DialogResult.OK)
            {
                CLKsFATXLib.Drive Original = ds1.SelectedDrive;
                Forms.DeviceSelector ds2 = new Party_Buffalo.Forms.DeviceSelector();
                if (ds2.ShowDialog() == DialogResult.OK)
                {
                    CLKsFATXLib.Drive Destination = ds2.SelectedDrive;
                    Forms.Clone c = new Party_Buffalo.Forms.Clone(Original, Destination);
                    c.ShowDialog();
                }
            }
        }

        private void menuItem9_Click(object sender, EventArgs e)
        {
            DialogResult dr = DialogResult.No;
            string Path = "";
            if (Aero)
            {
                CommonOpenFileDialog ofd = new CommonOpenFileDialog();
                ofd.IsFolderPicker = true;
                ofd.Multiselect = false;
                if (ofd.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    dr = DialogResult.OK;
                    Path = ofd.FileName;
                }
            }
            else
            {
                FolderBrowserDialog fbd = new FolderBrowserDialog();
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    Path = fbd.SelectedPath;
                }
            }
            if (dr == DialogResult.OK)
            {
                List<string> filePaths = new List<string>();
                for (int i = 0; i < 10000; i++)
                {
                    string extra = "";
                    if (i < 10)
                    {
                        extra = "000";
                    }
                    else if (i < 100)
                    {
                        extra = "00";
                    }
                    else if (i < 1000)
                    {
                        extra = "0";
                    }
                    if (System.IO.File.Exists(Path + "\\Data" + extra + i.ToString()))
                    {
                        filePaths.Add(Path + "\\Data" + extra + i.ToString());
                    }
                    else { break; }
                }
                Drive d = new Drive(filePaths.ToArray());
                if (filePaths.Count < 3 || !d.IsFATXDrive())
                {
                    if (Aero)
                    {
                        Microsoft.WindowsAPICodePack.Dialogs.TaskDialog td = new Microsoft.WindowsAPICodePack.Dialogs.TaskDialog();
                        td.Caption = "Files not valid";
                        td.Text = "The selected path doesn't contain a valid USB backup/dump.";
                        td.InstructionText = "Files Not Valid";
                        td.Icon = TaskDialogStandardIcon.Error;
                        td.ShowDialog(this.Handle);
                    }
                    else
                    {
                        MessageBox.Show("The selected path doesn't contain a valid USB backup/dump.", "Files Not Valid", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    if (Drive != null)
                    {
                        Drive.Close();
                    }
                    Drive = d;
                    LoadDrive();
                }
            }
        }

        private void size16_Click(object sender, EventArgs e)
        {
            foreach (MenuItem mi in m_treeviewIconSize.MenuItems)
            {
                mi.Checked = false;
            }
            size16.Checked = true;
            Properties.Settings.Default.treeViewIconWidthHeight = 16;
            Properties.Settings.Default.Save();
            treeView1.ImageList.ImageSize = new Size(Properties.Settings.Default.treeViewIconWidthHeight, Properties.Settings.Default.treeViewIconWidthHeight);
        }

        private void size24_Click(object sender, EventArgs e)
        {
            foreach (MenuItem mi in m_treeviewIconSize.MenuItems)
            {
                mi.Checked = false;
            }
            size24.Checked = true;
            Properties.Settings.Default.treeViewIconWidthHeight = 24;
            Properties.Settings.Default.Save();
            treeView1.ImageList.ImageSize = new Size(Properties.Settings.Default.treeViewIconWidthHeight, Properties.Settings.Default.treeViewIconWidthHeight);
        }

        private void size32_Click(object sender, EventArgs e)
        {
            foreach (MenuItem mi in m_treeviewIconSize.MenuItems)
            {
                mi.Checked = false;
            }
            size32.Checked = true;
            Properties.Settings.Default.treeViewIconWidthHeight = 64;
            Properties.Settings.Default.Save();
            treeView1.ImageList.ImageSize = new Size(Properties.Settings.Default.treeViewIconWidthHeight, Properties.Settings.Default.treeViewIconWidthHeight);
        }

        private void size64_Click(object sender, EventArgs e)
        {
            foreach (MenuItem mi in m_treeviewIconSize.MenuItems)
            {
                mi.Checked = false;
            }
            size64.Checked = true;
            Properties.Settings.Default.treeViewIconWidthHeight = 64;
            Properties.Settings.Default.Save();
            treeView1.ImageList.ImageSize = new Size(Properties.Settings.Default.treeViewIconWidthHeight, Properties.Settings.Default.treeViewIconWidthHeight);
        }

        private void c_Move_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                string s = "Are you sure you want to move the selected " + ((listView1.SelectedItems.Count > 1) ? "items?" : "item?");
                DialogResult dr = DialogResult.No;
                if (Aero)
                {
                    TaskDialog td = new TaskDialog();
                    td.Caption = "Confirm Move";
                    td.InstructionText = s;
                    td.StandardButtons = TaskDialogStandardButtons.Yes | TaskDialogStandardButtons.No | TaskDialogStandardButtons.Cancel;
                    if (td.ShowDialog(this.Handle) == TaskDialogResult.Yes)
                    {
                        dr = DialogResult.Yes;
                    }
                }
                else
                {
                    dr = MessageBox.Show("Confirm Move", s, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                }
                if (dr == DialogResult.Yes)
                {
                    Forms.FolderSelector fs = new Party_Buffalo.Forms.FolderSelector(treeView1, treeView1.Nodes[0].Nodes.Find(treeView1.SelectedNode.RealPath().Split('\\')[0], false)[0]);
                    if (fs.ShowDialog() == DialogResult.OK)
                    {
                        string SelectedPath = fs.SelectedPath;
                        List<CLKsFATXLib.Structs.WriteResult> List = new List<CLKsFATXLib.Structs.WriteResult>();
                        foreach (ListViewItem li in listView1.SelectedItems)
                        {
                            Entry Entry = (Entry)li.Tag;
                            CLKsFATXLib.Structs.WriteResult wr = Entry.Move(SelectedPath);
                            if (wr.CouldNotWrite)
                            {
                                List.Add(wr);
                            }
                        }
                        if (List.Count != 0)
                        {
                            string UnableToWrite = "Some of the entries could not be written";
                            string BadEntries = "";
                            for (int i = 0; i < List.Count; i++)
                            {
                                BadEntries += List[i].AttemptedEntryToMove.FullPath;
                                if (i != List.Count - 1)
                                {
                                    BadEntries += "\r\n";
                                }
                            }
                            if (Aero)
                            {
                                TaskDialog td = new TaskDialog();
                                td.Caption = "Moving Error";
                                td.InstructionText = UnableToWrite;
                                td.Text = "Press the details button for a list of entries that could not be moved";
                                td.DetailsExpandedText = BadEntries;
                                td.StandardButtons = TaskDialogStandardButtons.Ok;
                                td.ShowDialog(this.Handle);
                            }
                            else
                            {
                                MessageBox.Show("Moving Error", UnableToWrite + "\r\n\r\n" + BadEntries, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            }
                        }
                    }
                }
            }
        }

        private void menuItem14_Click_1(object sender, EventArgs e)
        {
            string s = "Are you sure you want to move the selected " + ((listView1.SelectedItems.Count > 1) ? "items?" : "item?");
            DialogResult dr = DialogResult.No;
            if (Aero)
            {
                TaskDialog td = new TaskDialog();
                td.Caption = "Confirm Move";
                td.InstructionText = s;
                td.StandardButtons = TaskDialogStandardButtons.Yes | TaskDialogStandardButtons.No | TaskDialogStandardButtons.Cancel;
                if (td.ShowDialog(this.Handle) == TaskDialogResult.Yes)
                {
                    dr = DialogResult.Yes;
                }
            }
            else
            {
                dr = MessageBox.Show("Confirm Move", s, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
            }
            if (dr == DialogResult.Yes)
            {
                Forms.FolderSelector fs = new Party_Buffalo.Forms.FolderSelector(treeView1, treeView1.Nodes[0].Nodes.Find(treeView1.SelectedNode.RealPath().Split('\\')[0], false)[0]);
                List<CLKsFATXLib.Structs.WriteResult> List = new List<CLKsFATXLib.Structs.WriteResult>();
                if (fs.ShowDialog() == DialogResult.OK)
                {
                    string SelectedPath = fs.SelectedPath;
                    Entry Entry = (Entry)rightClickedNode.Tag;
                    CLKsFATXLib.Structs.WriteResult wr = Entry.Move(SelectedPath);
                    if (wr.CouldNotWrite)
                    {
                        List.Add(wr);
                    }
                    if (List.Count != 0)
                    {
                        string UnableToWrite = "Some of the entries could not be written";
                        string BadEntries = "";
                        for (int i = 0; i < List.Count; i++)
                        {
                            BadEntries += List[i].AttemptedEntryToMove.FullPath;
                            if (i != List.Count - 1)
                            {
                                BadEntries += "\r\n";
                            }
                        }
                        if (Aero)
                        {
                            TaskDialog td = new TaskDialog();
                            td.Caption = "Moving Error";
                            td.InstructionText = UnableToWrite;
                            td.Text = "Press the details button for a list of entries that could not be moved";
                            td.DetailsExpandedText = BadEntries;
                            td.StandardButtons = TaskDialogStandardButtons.Ok;
                            td.ShowDialog(this.Handle);
                        }
                        else
                        {
                            MessageBox.Show("Moving Error", UnableToWrite + "\r\n\r\n" + BadEntries, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        }
                    }
                }
            }
        }

        private void quickAddMode_Click(object sender, EventArgs e)
        {
            quickAddMode.Checked = !quickAddMode.Checked;
            Properties.Settings.Default.quickAddMode = quickAddMode.Checked;
            Properties.Settings.Default.Save();
        }

        private void m_Reload_Click(object sender, EventArgs e)
        {

        }

        private void cacheContentIcons_Click(object sender, EventArgs e)
        {
            cacheContentIcons.Checked = !cacheContentIcons.Checked;
            Properties.Settings.Default.cacheContentIcons = cacheContentIcons.Checked;
            Properties.Settings.Default.Save();
        }

        enum Columns
        {
            Name,
            Type,
            Size,
            Date,
            STFSName,
            Default,
        }

        void ColumnClickHandler(int Column)
        {
            // Determine if clicked column is already the column that is being sorted.
            if (Column == lvwColumnSorter.SortColumn)
            {
                // Reverse the current sort direction for this column.
                if (lvwColumnSorter.Order == SortOrder.Ascending)
                {
                    lvwColumnSorter.Order = SortOrder.Descending;
                    ListViewExtensions.SetSortIcon(listView1, Column, SortOrder.Descending);
                }
                else
                {
                    lvwColumnSorter.Order = SortOrder.Ascending;
                    ListViewExtensions.SetSortIcon(listView1, Column, SortOrder.Ascending);
                }
            }
            else
            {
                // Set the column number that is to be sorted; default to ascending.
                lvwColumnSorter.SortColumn = Column;
                lvwColumnSorter.Order = SortOrder.Ascending;
                ListViewExtensions.SetSortIcon(listView1, Column, SortOrder.Ascending);
            }

            // Perform the sort with these new sort options.
            this.listView1.Sort();
        }

        private void sort_Name_Click(object sender, EventArgs e)
        {
            ColumnClickHandler((int)Columns.Name);
            foreach (MenuItem mi in m_Sort.MenuItems)
            {
                mi.Checked = false;
            }
            sort_Name.Checked = true;
        }

        private void sort_Type_Click(object sender, EventArgs e)
        {
            ColumnClickHandler((int)Columns.Type);
            foreach (MenuItem mi in m_Sort.MenuItems)
            {
                mi.Checked = false;
            }
            sort_Type.Checked = true;
        }

        private void sort_Size_Click(object sender, EventArgs e)
        {
            ColumnClickHandler((int)Columns.Size);
            foreach (MenuItem mi in m_Sort.MenuItems)
            {
                mi.Checked = false;
            }
            sort_Size.Checked = true;
        }

        private void sort_Date_Click(object sender, EventArgs e)
        {
            ColumnClickHandler((int)Columns.Date);
            foreach (MenuItem mi in m_Sort.MenuItems)
            {
                mi.Checked = false;
            }
            sort_Date.Checked = true;
        }

        private void sort_STFS_Click(object sender, EventArgs e)
        {
            ColumnClickHandler((int)Columns.STFSName);
            foreach (MenuItem mi in m_Sort.MenuItems)
            {
                mi.Checked = false;
            }
            sort_STFS.Checked = true;
        }

        private void reCache_Click(object sender, EventArgs e)
        {
            // Check if the directory contains files
            if (new System.IO.DirectoryInfo(Cache.ImageCachePath).GetFiles().Length > 0)
            {
                System.Threading.Thread t = new System.Threading.Thread((System.Threading.ThreadStart)delegate
                    {
                        Image rootImage = treeView1.ImageList.Images[0];
                        string rootKey = treeView1.ImageList.Images.Keys[0];
                        List<string> UsedImages = new List<string>();
                        this.Invoke((MethodInvoker)delegate
                        {
                            for (int i = 6; i < treeView1.ImageList.Images.Count; i++)
                            {
                                UsedImages.Add(treeView1.ImageList.Images.Keys[i]);
                            }
                            // Clear the set imagelists so we don't get an error about how the file is being used...
                            treeView1.ImageList = SmallListForFATX;
                            listView1.LargeImageList = LargeListForFATX;
                            listView1.SmallImageList = SmallListForFATX;
                        });
                        foreach (System.IO.FileInfo fi in new System.IO.DirectoryInfo(Cache.ImageCachePath).GetFiles().Where( name => name.Name.Length == 12 ))
                        {
                            // Get the title ID
                            string TitleID = System.IO.Path.GetFileNameWithoutExtension(fi.FullName);
                            if (!Cache.IsTitleIDFolder(TitleID))
                            {
                                continue;
                            }
                            // Create a webrequest for the image
                            try
                            {
                                this.Invoke((MethodInvoker)delegate
                                {
                                    l_selectedItems.Text = "Downloading " + System.IO.Path.GetFileNameWithoutExtension(fi.Name) + "...";
                                });
                                System.Net.HttpWebRequest wr = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(string.Format("http://avatar.xboxlive.com/global/t.{0}/icon/0/8000", TitleID));
                                // Get the image
                                Image i = Image.FromStream(wr.GetResponse().GetResponseStream());
                                // Delete the file
                                fi.Delete();
                                // Save the image
                                Cache.AddIcon(i, TitleID);
                            }
                            catch(Exception E) { continue; }
                        }

                        // Cache ones that have no icon as well
                        foreach (string s in Properties.Settings.Default.cachedID)
                        {
                            if (!new System.IO.DirectoryInfo(Cache.ImageCachePath).GetFiles().Contains(new System.IO.FileInfo(Cache.ImageCachePath + "\\" + s + ".png")))
                            {
                                // Get the title ID
                                string TitleID = s;
                                // Create a webrequest for the image
                                try
                                {
                                    this.Invoke((MethodInvoker)delegate
                                    {
                                        l_selectedItems.Text = "Downloading " + System.IO.Path.GetFileNameWithoutExtension(s + ".png") + "...";
                                    });
                                    System.Net.HttpWebRequest wr = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(string.Format("http://avatar.xboxlive.com/global/t.{0}/icon/0/8000", TitleID));
                                    // Get the image
                                    Image i = Image.FromStream(wr.GetResponse().GetResponseStream());
                                    // Save the image
                                    Cache.AddIcon(i, s.ToUpper());
                                    UsedImages.Add(s);
                                }
                                catch (Exception E) { continue; }
                            }
                        }

                        // Reset the imagelists
                        foreach (string s in UsedImages)
                        {
                            Image i = Cache.GetIcon(s);
                            this.Invoke((MethodInvoker)delegate
                            {
                                treeView1.ImageList.Images.Add(s, i);
                                listView1.SmallImageList.Images.Add(s, i);
                                listView1.LargeImageList.Images.Add(s, i);
                            });
                        }
                        treeView1.ImageList.Images[0] = rootImage;
                        treeView1.ImageList.Images.Keys[0] = rootKey;
                        this.Invoke((MethodInvoker)delegate
                        {
                            l_selectedItems.Text = "Image re-caching complete!";
                        });
                    });
                t.Start();
            }
        }

        private void m_CheckForUpdates_Click(object sender, EventArgs e)
        {
            #region Update Shit

            Party_Buffalo.Update u = new Update();
#if TRACE
            Party_Buffalo.Update.UpdateInfo ud = u.CheckForUpdates(new Uri("http://clkxu5.com/drivexplore/coolapplicationstuff.xml"));
#endif
#if DEBUG
            Party_Buffalo.Update.UpdateInfo ud = new Update.UpdateInfo();
            if (UpdateDebug)
            {
                if (!UpdateDebugForce)
                {
                    ud = u.CheckForUpdates(new Uri("http://clkxu5.com/drivexplore/dev/coolapplicationstuff.xml"));
                }
                else
                {
                    ud.Update = true;
                    ud.UpdateText = "This`Is`A`Test";
                    ud.UpdateVersion = "9000";
                }
            }
#endif
            if (ud.Update)
            {
                if (!Aero)
                {
                    Forms.UpdateForm uf = new Party_Buffalo.Forms.UpdateForm(ud);
                    uf.ShowDialog();
                }
                else
                {
                    TaskDialog td = new TaskDialog();
                    td.Caption = "Update Available";
                    td.InstructionText = "Version " + ud.UpdateVersion + " is Available";
                    td.Text = "An update is available for Party Buffalo";
                    td.DetailsCollapsedLabel = "Change Log";
                    td.DetailsExpandedLabel = "Change Log";
                    td.ExpansionMode = TaskDialogExpandedDetailsLocation.ExpandFooter;
                    string[] Split = ud.UpdateText.Split('`');
                    string UpdateText = "";
                    for (int i = 0; i < Split.Length; i++)
                    {
                        if (i != 0)
                        {
                            UpdateText += "\r\n";
                        }
                        if (Split[i] == "")
                        {
                            continue;
                        }
                        UpdateText += "-" + Split[i];
                    }
                    td.DetailsExpandedText = UpdateText;
                    TaskDialogCommandLink Download = new TaskDialogCommandLink("Download", "Download Party Buffalo version " + ud.UpdateVersion, "");
                    Download.Click += (o, f) =>
                    {
                        Forms.UpdateDownloadForm udf = new Party_Buffalo.Forms.UpdateDownloadForm(ud);
                        udf.ShowDialog();
                    };

                    TaskDialogCommandLink DontDownload = new TaskDialogCommandLink("DontDownload", "Let me go about my business and I'll download this stuff later...", "");
                    DontDownload.Click += (o, f) =>
                    {
                        td.Close();
                    };
                    td.Controls.Add(Download);
                    td.Controls.Add(DontDownload);
                    td.Show();
                }
            }
            else
            {
                MessageBox.Show("Party Buffalo is up to date!");
            }
            if (ud.QuickMessage != null && ud.QuickMessage != "")
            {
                quickMessage.Text = ud.QuickMessage;
            }
            else
            {
                quickMessage.Text = "Could not load quick message";
            }

            #endregion
        }

        string AddSlash(int AddAfter, string Input)
        {
            string Output = "";
            for (int i = 0; i < Input.Length; i+= AddAfter)
            {
                Output += Input.Substring(i, AddAfter) + "/";
            }
            return Output;
        }

        private void m_DisableNews_Click(object sender, EventArgs e)
        {
            m_DisableNews.Checked = !m_DisableNews.Checked;
            Properties.Settings.Default.loadLatestNews = !m_DisableNews.Checked;
            Properties.Settings.Default.Save();
        }

        private void m_DisableUpdates_Click(object sender, EventArgs e)
        {
            m_CheckForUpdates.Checked = !m_CheckForUpdates.Checked;
            Properties.Settings.Default.checkForUpdates = !m_DisableUpdates.Checked;
            Properties.Settings.Default.Save();
        }

        private void m_hideToolBar_Click(object sender, EventArgs e)
        {
            m_hideToolBar.Checked = !m_hideToolBar.Checked;
            Properties.Settings.Default.hideToolBar = m_hideToolBar.Checked;
            Properties.Settings.Default.Save();
            panel2.Visible = !m_hideToolBar.Checked;
        }

        private void m_hideAddressBar_Click(object sender, EventArgs e)
        {
            m_hideAddressBar.Checked = !m_hideAddressBar.Checked;
            Properties.Settings.Default.hideAddressBar = m_hideAddressBar.Checked;
            Properties.Settings.Default.Save();
            panel1.Visible = !m_hideAddressBar.Checked;
        }

        private void m_hideStatusBar_Click(object sender, EventArgs e)
        {
            m_hideStatusBar.Checked = !m_hideStatusBar.Checked;
            Properties.Settings.Default.hideStatusBar = m_hideStatusBar.Checked;
            Properties.Settings.Default.Save();
            panel3.Visible = !m_hideStatusBar.Checked;
        }
    }
}
