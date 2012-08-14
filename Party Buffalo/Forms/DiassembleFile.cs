namespace Party_Buffalo
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.IO;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;
    using Party_Buffalo.Properties;
    using CLKsFATXLib;
    using WxTools;

    public class MainForm : Form
    {
        private string[] args;
        private BinaryReader br;
        private ColumnHeader columnHeaderCluster;
        private ColumnHeader columnHeaderDateModified;
        private ColumnHeader columnHeaderName;
        private ColumnHeader columnHeaderSize;
        private ColumnHeader columnHeaderStatus;
        private IContainer components;
        private ContextMenuStrip contextMenuStrip;
        private ContextMenuStrip contextMenuStripFolder;
        private ContextMenuStrip contextMenuStripMulti;
        private ToolStripMenuItem exitToolStripMenuItem;
        private ToolStripMenuItem extractAllToolStripMenuItem;
        private ToolStripMenuItem extractFilesToolStripMenuItem;
        private ToolStripMenuItem extractFileToolStripMenuItem;
        private ToolStripMenuItem extractFolderToolStripMenuItem;
        private ToolStripMenuItem fileToolStripMenuItem;
        private FolderBrowserDialog folderBrowserDialog;
        private FileStream fs;
        private ImageList imageList;
        private ImageList imageListTreeView;
        private ListView listView;
        public static int MAGIC_CON_ = 0x434f4e20;
        public static int MAGIC_LIVE = 0x4c495645;
        public static int MAGIC_PIRS = 0x50495253;
        private MenuStrip menuStrip;
        private OpenFileDialog openFileDialog;
        private ToolStripMenuItem openFileToolStripMenuItem;
        public static long PIRS_BASE = 0xb000L;
        private long pirs_offset;
        private long pirs_start;
        public static long PIRS_TYPE1 = 0x1000L;
        public static long PIRS_TYPE2 = 0x2000L;
        private SaveFileDialog saveFileDialog;
        private SplitContainer splitContainerH;
        private SplitContainer splitContainerV;
        private TextBox textBoxLog;
        private ToolStripContainer toolStripContainer1;
        private ToolStripSeparator toolStripMenuItem2;
        private TreeView treeView;
        private WxReader wr = new WxReader();
        private ImageList siL;
        private ImageList liL;
        private CLKsFATXLib.File xFile;
        private bool STFSFile = false;



        public MainForm()
        {
            this.InitializeComponent();
        }
        /**
        *   Title:          Xbox 360 Forensic Toolkit.
        *   Description:    Xbox 360 Forensic Toolkit is built on top of the Party buffalo application. It allows users to read and 
        *                   write to Xbox 360 devices, with support for reading Xbox 360 STFS package names. A number of Forensic Tools have
        *                   been added to extract/view sectors, extract partitions, view file details, examine files in HEX. It also incorporates the wxPIRS
        *                   Application so that we can sissamble STFS packages and see their contents.
        *
        *   Original Author: Party Buffalo - landergriffith@gmail.com, wxPIRS - gael360
        *   Another Author: obriej62@mail.dcu.ie
        *   License:        http://www.gnu.org/licenses/gpl.html GNU GENERAL PUBLIC LICENSE
        *   Link:           https://code.google.com/p/party-buffalo/
        *                  http://gael360.free.fr/
        *   Note:           All Code that follows to the End point was added by obriej62@mail.dcu.ie
        */
        public MainForm(CLKsFATXLib.File f)
        {
            xFile = f;
            STFSFile = true;
            InitializeComponent();
       }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
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

        private DateTime dosDateTime(int datetime)
        {
            return this.dosDateTime((short) (datetime >> 0x10), (short) (datetime - ((datetime >> 0x10) << 0x10)));
        }

        private DateTime dosDateTime(short date, short time)
        {
            if ((date == 0) && (time == 0))
            {
                return DateTime.Now;
            }
            int year = ((date & 0xfe00) >> 9) + 0x7bc;
            int month = (date & 480) >> 5;
            int day = date & 0x1f;
            int hour = (time & 0xf800) >> 11;
            int minute = (time & 0x7e0) >> 5;
            return new DateTime(year, month, day, hour, minute, (time & 0x1f) * 2);
        }

        private void Exit()
        {
            base.Dispose();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Exit();
        }

        private void extractAll(object sender, EventArgs e)
        {
            this.treeView.SelectedNode = this.treeView.Nodes[0];
            this.extractFolder(sender, e);
        }

        private void extractFile(ListViewItem listViewItem, string filename)
        {
            FileStream stream;
            BinaryWriter writer;
             try
            {
                if (!Directory.Exists(this.wr.extractFolderName(filename)))
                {
                    Directory.CreateDirectory(this.wr.extractFolderName(filename));
                }
            }
            catch (IOException)
            {
            }
            try
            {
                stream = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None);
                writer = new BinaryWriter(stream);
            }
            catch (IOException exception)
            {
                this.log(string.Format("Error : {0}\r\n", exception));
                return;
            }
            long cluster = Convert.ToInt64(listViewItem.SubItems[this.columnHeaderCluster.Index].Text);
            long offset = this.getOffset(cluster);
            long num3 = Convert.ToInt64(listViewItem.SubItems[this.columnHeaderSize.Index].Text);
            long num4 = num3 >> 12;
            long num5 = num3 - (num4 << 12);
            for (long i = cluster; i < (cluster + num4); i += 1L)
            {
                offset = this.getOffset(i);
                if (STFSFile)
                {
                    CLKsFATXLib.Streams.Reader io2 = new CLKsFATXLib.Streams.Reader(xFile.GetStream());
                    io2.BaseStream.Position = (offset);
                    writer.Write(io2.ReadBytes(0x1000));
                    //io2.Close();
                }
                else
                {
                    this.br.BaseStream.Seek(offset, SeekOrigin.Begin);
                    writer.Write(this.br.ReadBytes(0x1000));
                }
                string str = string.Format("{0}%", (100L * (i - cluster)) / num4);
                if (str != listViewItem.SubItems[this.columnHeaderStatus.Index].Text)
                {
                    listViewItem.SubItems[this.columnHeaderStatus.Index].Text = str;
                    Application.DoEvents();
                }
            }
            offset = this.getOffset(cluster + num4);
            if (STFSFile)
            {
                CLKsFATXLib.Streams.Reader io2 = new CLKsFATXLib.Streams.Reader(xFile.GetStream());
                io2.BaseStream.Position = (offset);
                writer.Write(io2.ReadBytes((int) num5));
                //io2.Close();
            }
            else
            {
                this.br.BaseStream.Seek(offset, SeekOrigin.Begin);
                writer.Write(this.br.ReadBytes((int) num5));
            }
            listViewItem.SubItems[this.columnHeaderStatus.Index].Text = "Done";
            Application.DoEvents();
            writer.Close();
            stream.Dispose();
        }

        private void extractFile(long cluster, long size, string filename)
        {
            long num;
            FileStream stream;
            BinaryWriter writer;
            try
            {
                if (!Directory.Exists(this.wr.extractFolderName(filename)))
                {
                    Directory.CreateDirectory(this.wr.extractFolderName(filename));
                }
            }
            catch (IOException)
            {
            }
            try
            {
                stream = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None);
                writer = new BinaryWriter(stream);
            }
            catch (IOException exception)
            {
                this.log(string.Format("Error : {0}\r\n", exception));
                return;
            }
            long num2 = size >> 12;
            long num3 = size - (num2 << 12);
            for (long i = cluster; i < (cluster + num2); i += 1L)
            {
                num = this.getOffset(i);
                this.br.BaseStream.Seek(num, SeekOrigin.Begin);
                writer.Write(this.br.ReadBytes(0x1000));
                Application.DoEvents();
            }
            num = this.getOffset(cluster + num2);
            this.br.BaseStream.Seek(num, SeekOrigin.Begin);
            writer.Write(this.br.ReadBytes((int) num3));
            Application.DoEvents();
            writer.Close();
            stream.Dispose();
        }

        private void extractFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                for (int i = 0; i < this.listView.SelectedItems.Count; i++)
                {
                    if (this.isFolder(this.listView.SelectedItems[i]))
                    {
                        this.log(string.Format("Extract folder {0}\r\n", this.listView.SelectedItems[i].Text));
                        this.extractFolder(this.listView.SelectedItems[i], this.folderBrowserDialog.SelectedPath);
                    }
                    else
                    {
                        this.extractFile(this.listView.SelectedItems[i], this.folderBrowserDialog.SelectedPath + @"\" + this.listView.SelectedItems[i].Text);
                    }
                }
            }
            MessageBox.Show("Extract Completed.",
                   "Important Note",
                   MessageBoxButtons.OK,
                   MessageBoxIcon.Exclamation,
                   MessageBoxDefaultButton.Button1);
        }

        private void extractFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.saveFileDialog.FileName = this.listView.SelectedItems[0].Text;
            if (this.saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                this.extractFile(this.listView.SelectedItems[0], this.saveFileDialog.FileName);
            }
            MessageBox.Show("Extract Completed.",
               "Important Note",
               MessageBoxButtons.OK,
               MessageBoxIcon.Exclamation,
               MessageBoxDefaultButton.Button1);
        }

        private void extractFolder(object sender, EventArgs e)
        {
            if ((this.treeView.SelectedNode != null) && (this.folderBrowserDialog.ShowDialog() == DialogResult.OK))
            {
                for (int i = 0; i < this.listView.Items.Count; i++)
                {
                    if (this.isFolder(this.listView.Items[i]))
                    {
                        this.extractFolder(this.listView.Items[i], this.folderBrowserDialog.SelectedPath);
                    }
                    else
                    {
                        this.extractFile(this.listView.Items[i], this.folderBrowserDialog.SelectedPath + @"\" + this.listView.Items[i].Text);
                    }
                    Application.DoEvents();
                }
            }
            MessageBox.Show("Extract Completed.",
            "Important Note",
            MessageBoxButtons.OK,
            MessageBoxIcon.Exclamation,
            MessageBoxDefaultButton.Button1);
        }

        private void extractFolder(ListViewItem listViewItem, string pathname)
        {
            listViewItem.SubItems[this.columnHeaderStatus.Index].Text = "Working...";
            Application.DoEvents();
            this.extractFolder(Convert.ToUInt16(listViewItem.Tag), listViewItem.Text, pathname);
            listViewItem.SubItems[this.columnHeaderStatus.Index].Text = "Done";
            Application.DoEvents();
        }

        private void extractFolder(ushort tag, string foldername, string pathname)
        {
            ushort num = 0;
            long offset = 0;
            while (true)
            {
                PirsEntry entry = new PirsEntry();
                if (STFSFile)
                {
                    offset = ((PIRS_BASE + this.pirs_offset) + (num * 0x40));
                    entry = this.getEntrySTFS(offset);
                }
                else
                {
                    this.br.BaseStream.Seek((PIRS_BASE + this.pirs_offset) + (num * 0x40), SeekOrigin.Begin);
                    entry = this.getEntry();
                }

                if (entry.Filename.Trim() == "")
                {
                    return;
                }
                if (((entry.Cluster == 0) && (entry.Size == 0)) && (entry.Parent == tag))
                {
                    this.extractFolder(num, entry.Filename, pathname + @"\" + foldername);
                }
                else if ((entry.Cluster != 0) && (entry.Parent == tag))
                {
                    this.extractFile((long) entry.Cluster, (long) entry.Size, pathname + @"\" + foldername + @"\" + entry.Filename);
                }
                num = (ushort) (num + 1);
            }
        }

        private long getCultureOffset()
        {
            if (Application.CurrentCulture.TwoLetterISOLanguageName.ToLower() == "de")
            {
                return 0x200L;
            }
            if (Application.CurrentCulture.TwoLetterISOLanguageName.ToLower() == "fr")
            {
                return 0x300L;
            }
            if (Application.CurrentCulture.TwoLetterISOLanguageName.ToLower() == "es")
            {
                return 0x400L;
            }
            if (Application.CurrentCulture.TwoLetterISOLanguageName.ToLower() == "it")
            {
                return 0x500L;
            }
            return 0L;
        }

        private void getDescription()
        {
            long num = this.getCultureOffset();
            this.br.BaseStream.Seek(0x410L + num, SeekOrigin.Begin);
            byte[] data = new byte[0x100];
            data = this.br.ReadBytes(0x100);
            this.log("Title : " + this.wr.unicodeToStr(data, 2) + "\r\n");
            this.br.BaseStream.Seek(0xd10L + num, SeekOrigin.Begin);
            byte[] buffer2 = new byte[0x100];
            buffer2 = this.br.ReadBytes(0x100);
            this.log("Description : " + this.wr.unicodeToStr(buffer2, 2) + "\r\n");
            this.br.BaseStream.Seek(0x1610L, SeekOrigin.Begin);
            byte[] buffer3 = new byte[0x100];
            buffer3 = this.br.ReadBytes(0x100);
            this.log("Publisher : " + this.wr.unicodeToStr(buffer3, 2) + "\r\n");
        }

        private void getDirectories(TreeNode tn)
        {
            int num = 0;
            while (true)
            {
                this.br.BaseStream.Seek(this.pirs_start + (num * 0x40), SeekOrigin.Begin);
                PirsEntry entry = new PirsEntry();
                entry = this.getEntry();
                if (entry.Filename.Trim() == "")
                {
                    return;
                }
                if (((entry.Size == 0) && (entry.Cluster == 0)) && (entry.Parent == Convert.ToUInt16(tn.Tag)))
                {
                    TreeNode node = new TreeNode(entry.Filename);
                    node.Tag = num;
                    node.ToolTipText = string.Format("0x{0:X4}", node.Tag);
                    tn.Nodes.Add(node);
                    this.getDirectories(node);
                }
                num++;
            }
        }

        private PirsEntry getEntry()
        {
            PirsEntry entry = new PirsEntry();
            entry.Filename = this.wr.readString(this.br, 0x26);
            if (entry.Filename.Trim() != "")
            {
                entry.Unknow = this.wr.readInt32(this.br);
                entry.BlockLen = this.wr.readInt32(this.br);
                entry.Cluster = this.br.ReadInt32() >> 8;
                entry.Parent = this.wr.readUInt16(this.br);
                entry.Size = this.wr.readInt32(this.br);
                entry.DateTime1 = this.dosDateTime(this.wr.readInt32(this.br));
                entry.DateTime2 = this.dosDateTime(this.wr.readInt32(this.br));
            }
            return entry;
        }

        private void getFiles(TreeNode tn)
        {
            int num = 0;
            while (true)
            {
                this.br.BaseStream.Seek(this.pirs_start + (num * 0x40), SeekOrigin.Begin);
                PirsEntry entry = new PirsEntry();
                entry = this.getEntry();
                if (entry.Filename.Trim() == "")
                {
                    break;
                }
                if (((entry.Cluster == 0) && (entry.Size == 0)) && (entry.Parent == Convert.ToUInt16(tn.Tag)))
                {
                    ListViewItem item = new ListViewItem();
                    item.Text = entry.Filename;
                    item.SubItems.Add(entry.Size.ToString());
                    item.SubItems.Add(entry.Cluster.ToString());
                    item.SubItems.Add(entry.DateTime1.ToString());
                    item.SubItems.Add("");
                    item.ImageIndex = 0;
                    item.Tag = num;
                    this.listView.Items.Add(item);
                }
                num++;
            }
            num = 0;
            while (true)
            {
                this.br.BaseStream.Seek(this.pirs_start + (num * 0x40), SeekOrigin.Begin);
                PirsEntry entry2 = new PirsEntry();
                entry2 = this.getEntry();
                if (entry2.Filename.Trim() == "")
                {
                    return;
                }
                if ((entry2.Cluster != 0) && (entry2.Parent == Convert.ToUInt16(tn.Tag)))
                {
                    ListViewItem item2 = new ListViewItem();
                    item2.Text = entry2.Filename;
                    item2.SubItems.Add(entry2.Size.ToString());
                    item2.SubItems.Add(entry2.Cluster.ToString());
                    item2.SubItems.Add(entry2.DateTime1.ToString());
                    item2.SubItems.Add("");
                    item2.ImageIndex = 1;
                    item2.ToolTipText = string.Format("Offset : 0x{0:X8}", this.getOffset((long) entry2.Cluster));
                    this.listView.Items.Add(item2);
                }
                num++;
            }
        }

        private long getOffset(long cluster)
        {
            long num = this.pirs_start + (cluster * 0x1000L);
            long num2 = cluster / 170L;
            long num3 = num2 / 170L;
            if (num2 > 0L)
            {
                num += (num2 + 1L) * this.pirs_offset;
            }
            if (num3 > 0L)
            {
                num += (num3 + 1L) * this.pirs_offset;
            }
            return num;
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.extractAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.splitContainerH = new System.Windows.Forms.SplitContainer();
            this.splitContainerV = new System.Windows.Forms.SplitContainer();
            this.treeView = new System.Windows.Forms.TreeView();
            this.listView = new System.Windows.Forms.ListView();
            this.columnHeaderName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderSize = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderCluster = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderDateModified = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderStatus = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.textBoxLog = new System.Windows.Forms.TextBox();
            this.imageListTreeView = new System.Windows.Forms.ImageList(this.components);
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.contextMenuStripFolder = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.extractFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.extractFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStripMulti = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.extractFilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.menuStrip.SuspendLayout();
            this.toolStripContainer1.ContentPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            this.splitContainerH.Panel1.SuspendLayout();
            this.splitContainerH.Panel2.SuspendLayout();
            this.splitContainerH.SuspendLayout();
            this.splitContainerV.Panel1.SuspendLayout();
            this.splitContainerV.Panel2.SuspendLayout();
            this.splitContainerV.SuspendLayout();
            this.contextMenuStripFolder.SuspendLayout();
            this.contextMenuStrip.SuspendLayout();
            this.contextMenuStripMulti.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(729, 24);
            this.menuStrip.TabIndex = 0;
            this.menuStrip.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openFileToolStripMenuItem,
            this.extractAllToolStripMenuItem,
            this.toolStripMenuItem2,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openFileToolStripMenuItem
            // 
            this.openFileToolStripMenuItem.Name = "openFileToolStripMenuItem";
            this.openFileToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
            this.openFileToolStripMenuItem.Text = "Open...";
            this.openFileToolStripMenuItem.Click += new System.EventHandler(this.openFile);
            // 
            // extractAllToolStripMenuItem
            // 
            this.extractAllToolStripMenuItem.Name = "extractAllToolStripMenuItem";
            this.extractAllToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
            this.extractAllToolStripMenuItem.Text = "Extract all...";
            this.extractAllToolStripMenuItem.Click += new System.EventHandler(this.extractAll);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(130, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // toolStripContainer1
            // 
            // 
            // toolStripContainer1.ContentPanel
            // 
            this.toolStripContainer1.ContentPanel.Controls.Add(this.splitContainerH);
            this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(729, 346);
            this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripContainer1.Location = new System.Drawing.Point(0, 24);
            this.toolStripContainer1.Name = "toolStripContainer1";
            this.toolStripContainer1.Size = new System.Drawing.Size(729, 371);
            this.toolStripContainer1.TabIndex = 2;
            this.toolStripContainer1.Text = "toolStripContainer1";
            // 
            // splitContainerH
            // 
            this.splitContainerH.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerH.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainerH.Location = new System.Drawing.Point(0, 0);
            this.splitContainerH.Name = "splitContainerH";
            this.splitContainerH.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainerH.Panel1
            // 
            this.splitContainerH.Panel1.Controls.Add(this.splitContainerV);
            // 
            // splitContainerH.Panel2
            // 
            this.splitContainerH.Panel2.Controls.Add(this.textBoxLog);
            this.splitContainerH.Size = new System.Drawing.Size(729, 346);
            this.splitContainerH.SplitterDistance = 265;
            this.splitContainerH.TabIndex = 0;
            // 
            // splitContainerV
            // 
            this.splitContainerV.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerV.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainerV.Location = new System.Drawing.Point(0, 0);
            this.splitContainerV.Name = "splitContainerV";
            // 
            // splitContainerV.Panel1
            // 
            this.splitContainerV.Panel1.Controls.Add(this.treeView);
            // 
            // splitContainerV.Panel2
            // 
            this.splitContainerV.Panel2.Controls.Add(this.listView);
            this.splitContainerV.Size = new System.Drawing.Size(729, 265);
            this.splitContainerV.SplitterDistance = 180;
            this.splitContainerV.TabIndex = 0;
            // 
            // treeView
            // 
            this.treeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView.Location = new System.Drawing.Point(0, 0);
            this.treeView.Name = "treeView";
            this.treeView.ShowRootLines = false;
            this.treeView.Size = new System.Drawing.Size(180, 265);
            this.treeView.TabIndex = 0;
            this.treeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView_AfterSelect);
            this.treeView.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeView_NodeMouseClick);
            // 
            // listView
            // 
            this.listView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderName,
            this.columnHeaderSize,
            this.columnHeaderCluster,
            this.columnHeaderDateModified,
            this.columnHeaderStatus});
            this.listView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView.FullRowSelect = true;
            this.listView.Location = new System.Drawing.Point(0, 0);
            this.listView.Name = "listView";
            this.listView.ShowItemToolTips = true;
            this.listView.Size = new System.Drawing.Size(545, 265);
            this.listView.TabIndex = 0;
            this.listView.UseCompatibleStateImageBehavior = false;
            this.listView.View = System.Windows.Forms.View.Details;
            this.listView.MouseClick += new System.Windows.Forms.MouseEventHandler(this.listView_MouseClick);
            // 
            // columnHeaderName
            // 
            this.columnHeaderName.Text = "Name";
            this.columnHeaderName.Width = 200;
            // 
            // columnHeaderSize
            // 
            this.columnHeaderSize.Text = "Size";
            this.columnHeaderSize.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // columnHeaderCluster
            // 
            this.columnHeaderCluster.Text = "Cluster";
            this.columnHeaderCluster.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // columnHeaderDateModified
            // 
            this.columnHeaderDateModified.Text = "Date modified";
            this.columnHeaderDateModified.Width = 120;
            // 
            // columnHeaderStatus
            // 
            this.columnHeaderStatus.Text = "Status";
            this.columnHeaderStatus.Width = 80;
            // 
            // textBoxLog
            // 
            this.textBoxLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxLog.Location = new System.Drawing.Point(0, 0);
            this.textBoxLog.Multiline = true;
            this.textBoxLog.Name = "textBoxLog";
            this.textBoxLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxLog.Size = new System.Drawing.Size(729, 77);
            this.textBoxLog.TabIndex = 0;
            // 
            // imageListTreeView
            // 
            this.imageListTreeView.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imageListTreeView.ImageSize = new System.Drawing.Size(16, 16);
            this.imageListTreeView.TransparentColor = System.Drawing.Color.Fuchsia;
            // 
            // imageList
            // 
            this.imageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imageList.ImageSize = new System.Drawing.Size(16, 16);
            this.imageList.TransparentColor = System.Drawing.Color.Fuchsia;
            // 
            // contextMenuStripFolder
            // 
            this.contextMenuStripFolder.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.extractFolderToolStripMenuItem});
            this.contextMenuStripFolder.Name = "contextMenuStripFolder";
            this.contextMenuStripFolder.Size = new System.Drawing.Size(144, 26);
            // 
            // extractFolderToolStripMenuItem
            // 
            this.extractFolderToolStripMenuItem.Name = "extractFolderToolStripMenuItem";
            this.extractFolderToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.extractFolderToolStripMenuItem.Text = "Extract folder";
            this.extractFolderToolStripMenuItem.Click += new System.EventHandler(this.extractFolder);
            // 
            // openFileDialog
            // 
            this.openFileDialog.Filter = "All files (*.*)|*.*";
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.extractFileToolStripMenuItem});
            this.contextMenuStrip.Name = "contextMenuStrip";
            this.contextMenuStrip.Size = new System.Drawing.Size(153, 48);
            // 
            // extractFileToolStripMenuItem
            // 
            this.extractFileToolStripMenuItem.Name = "extractFileToolStripMenuItem";
            this.extractFileToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.extractFileToolStripMenuItem.Text = "Extract file";
            this.extractFileToolStripMenuItem.Click += new System.EventHandler(this.extractFileToolStripMenuItem_Click);
            // 
            // contextMenuStripMulti
            // 
            this.contextMenuStripMulti.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.extractFilesToolStripMenuItem});
            this.contextMenuStripMulti.Name = "contextMenuStripMulti";
            this.contextMenuStripMulti.Size = new System.Drawing.Size(134, 26);
            // 
            // extractFilesToolStripMenuItem
            // 
            this.extractFilesToolStripMenuItem.Name = "extractFilesToolStripMenuItem";
            this.extractFilesToolStripMenuItem.Size = new System.Drawing.Size(131, 22);
            this.extractFilesToolStripMenuItem.Text = "Extract files";
            this.extractFilesToolStripMenuItem.Click += new System.EventHandler(this.extractFilesToolStripMenuItem_Click);
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.Filter = "All files (*.*)|*.*";
            // 
            // MainForm
            // 
            this.ClientSize = new System.Drawing.Size(729, 395);
            this.Controls.Add(this.toolStripContainer1);
            this.Controls.Add(this.menuStrip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Disassemble File";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.toolStripContainer1.ContentPanel.ResumeLayout(false);
            this.toolStripContainer1.ResumeLayout(false);
            this.toolStripContainer1.PerformLayout();
            this.splitContainerH.Panel1.ResumeLayout(false);
            this.splitContainerH.Panel2.ResumeLayout(false);
            this.splitContainerH.Panel2.PerformLayout();
            this.splitContainerH.ResumeLayout(false);
            this.splitContainerV.Panel1.ResumeLayout(false);
            this.splitContainerV.Panel2.ResumeLayout(false);
            this.splitContainerV.ResumeLayout(false);
            this.contextMenuStripFolder.ResumeLayout(false);
            this.contextMenuStrip.ResumeLayout(false);
            this.contextMenuStripMulti.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private bool isFolder(ListViewItem listViewItem)
        {
            return (listViewItem.ImageIndex == 0);
        }

        private void listView_MouseClick(object sender, MouseEventArgs e)
        {
            if ((e.Button == MouseButtons.Right) && (this.listView.SelectedItems.Count == 1))
            {
                if (this.isFolder(this.listView.SelectedItems[0]))
                {
                    this.contextMenuStripMulti.Show(this.listView, e.X, e.Y);
                }
                else
                {
                    this.contextMenuStrip.Show(this.listView, e.X, e.Y);
                }
            }
            else if ((e.Button == MouseButtons.Right) && (this.listView.SelectedItems.Count > 1))
            {
                this.contextMenuStripMulti.Show(this.listView, e.X, e.Y);
            }
        }

        public void log(string message)
        {
            this.textBoxLog.AppendText(message);
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Exit();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            siL = SmallListForFATX;
            liL = SmallListForFATX;
            this.listView.SmallImageList = siL;
            this.treeView.ImageList = siL;

            if (STFSFile)
            {
                if (xFile.IsFolder)
                {
                    MessageBox.Show("You can not disassemble a Folder.",
                                       "Important Note",
                                       MessageBoxButtons.OK,
                                       MessageBoxIcon.Exclamation,
                                       MessageBoxDefaultButton.Button1);

                }
                else
                {
                    openFileSTFS();
                }
            }
        }

        private void openFile(string filename)
        {
            this.textBoxLog.Clear();
            this.folderBrowserDialog.SelectedPath = this.wr.extractFolderName(this.openFileDialog.FileName);
            if (this.br != null)
            {
                this.br.Close();
            }
            if (this.fs != null)
            {
                this.fs.Dispose();
            }
            this.treeView.BeginUpdate();
            this.treeView.Nodes.Clear();
            this.treeView.EndUpdate();
            this.listView.BeginUpdate();
            this.listView.Items.Clear();
            this.listView.EndUpdate();
            try
            {
                this.fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
                this.br = new BinaryReader(this.fs);
                this.getDescription();
                this.br.BaseStream.Seek(0L, SeekOrigin.Begin);
                int num = this.wr.readInt32(this.br);
                if (((num != MAGIC_PIRS) && (num != MAGIC_LIVE)) && (num != MAGIC_CON_))
                {
                    this.log("Not a PIRS/LIVE file!\r\n");
                    this.br.Close();
                    this.fs.Close();
                }
                else
                {
                    this.br.BaseStream.Seek(0xc030L, SeekOrigin.Begin);
                    int num2 = this.wr.readInt32(this.br);
                    if (num == MAGIC_CON_)
                    {
                        this.pirs_offset = PIRS_TYPE2;
                        this.pirs_start = 0xc000L;
                    }
                    else if (num2 == 0xffff)
                    {
                        this.pirs_offset = PIRS_TYPE1;
                        this.pirs_start = PIRS_BASE + this.pirs_offset;
                    }
                    else
                    {
                        this.pirs_offset = PIRS_TYPE2;
                        this.pirs_start = PIRS_BASE + this.pirs_offset;
                    }
                    this.parse(filename);
                }
            }
            catch (IOException exception)
            {
                this.log(string.Format("{0}\r\n", exception.Message));
            }
        }

        private void openFile(object sender, EventArgs e)
        {
            if (this.openFileDialog.ShowDialog() == DialogResult.OK)
            {
                this.openFile(this.openFileDialog.FileName);
            }
        }

        private void parse(string filename)
        {
            this.treeView.BeginUpdate();
            this.treeView.Nodes.Clear();
            this.listView.BeginUpdate();
            this.listView.Items.Clear();
            TreeNode node = new TreeNode(this.wr.extractFileName(filename), 0, 0);
            node.Tag = (ushort) 0xffff;
            node.ToolTipText = string.Format("0x{0:X4}", node.Tag);
            this.treeView.Nodes.Add(node);
            this.getDirectories(node);
            this.getFiles(node);
            node.Expand();
            this.listView.EndUpdate();
            this.treeView.EndUpdate();
        }

        private void treeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            this.listView.BeginUpdate();
            this.listView.Items.Clear();
            if (STFSFile)
            {
                this.getFilesSTFS(e.Node);
            }
            else
            {
                this.getFiles(e.Node);
            }
            this.listView.EndUpdate();
        }

        private void treeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                this.treeView.SelectedNode = e.Node;
                this.contextMenuStripFolder.Show(this.treeView, e.X, e.Y);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct PirsEntry
        {
            public string Filename;
            public int Unknow;
            public int BlockLen;
            public int Cluster;
            public ushort Parent;
            public int Size;
            public DateTime DateTime1;
            public DateTime DateTime2;
        }


        /**
        *   Title:          Xbox 360 Forensic Toolkit.
        *   Description:    Xbox 360 Forensic Toolkit is built on top of the Party buffalo application. It allows users to read and 
        *                   write to Xbox 360 devices, with support for reading Xbox 360 STFS package names. A number of Forensic Tools have
        *                   been added to extract/view sectors, extract partitions, view file details, examine files in HEX. It also incorporates the wxPIRS
        *                   Application so that we can sissamble STFS packages and see their contents.
        *
        *   Original Author: Party Buffalo - landergriffith@gmail.com, wxPIRS - gael360
        *   Another Author: obriej62@mail.dcu.ie
        *   License:        http://www.gnu.org/licenses/gpl.html GNU GENERAL PUBLIC LICENSE
        *   Link:           https://code.google.com/p/party-buffalo/
        *                  http://gael360.free.fr/
        *   Note:           All Code that follows to the End point was added by obriej62@mail.dcu.ie
        */

        private void openFileSTFS()
        {
            this.textBoxLog.Clear();
            this.treeView.BeginUpdate();
            this.treeView.Nodes.Clear();
            this.treeView.EndUpdate();
            this.listView.BeginUpdate();
            this.listView.Items.Clear();
            this.listView.EndUpdate();
            try
            {
                this.getSTFSDescription();
                CLKsFATXLib.Streams.Reader io2 = new CLKsFATXLib.Streams.Reader(xFile.GetStream());
                io2.BaseStream.Position = 0L;
                int num = io2.ReadInt32();
                if (((num != MAGIC_PIRS) && (num != MAGIC_LIVE)) && (num != MAGIC_CON_))
                {
                    this.log("Not a PIRS/LIVE file!\r\n");
                }
                else
                {
                    io2.BaseStream.Position = 0xc030L;
                    int num2 = io2.ReadInt32();
                    if (num == MAGIC_CON_)
                    {
                        this.pirs_offset = PIRS_TYPE2;
                        this.pirs_start = 0xc000L;
                    }
                    else if (num2 == 0xffff)
                    {
                        this.pirs_offset = PIRS_TYPE1;
                        this.pirs_start = PIRS_BASE + this.pirs_offset;
                    }
                    else
                    {
                        this.pirs_offset = PIRS_TYPE2;
                        this.pirs_start = PIRS_BASE + this.pirs_offset;
                    }
                    this.parseSFTS();
                }
            }
            catch (IOException exception)
            {
                this.log(string.Format("{0}\r\n", exception.Message));
            }
        }
        /**
        *   Title:          Xbox 360 Forensic Toolkit.
        *   Description:    Xbox 360 Forensic Toolkit is built on top of the Party buffalo application. It allows users to read and 
        *                   write to Xbox 360 devices, with support for reading Xbox 360 STFS package names. A number of Forensic Tools have
        *                   been added to extract/view sectors, extract partitions, view file details, examine files in HEX. It also incorporates the wxPIRS
        *                   Application so that we can sissamble STFS packages and see their contents.
        *
        *   Original Author: Party Buffalo - landergriffith@gmail.com, wxPIRS - gael360
        *   Another Author: obriej62@mail.dcu.ie
        *   License:        http://www.gnu.org/licenses/gpl.html GNU GENERAL PUBLIC LICENSE
        *   Link:           https://code.google.com/p/party-buffalo/
        *                  http://gael360.free.fr/
        *   Note:           All Code that follows to the End point was added by obriej62@mail.dcu.ie
        */
        private void getSTFSDescription()
        {
            CLKsFATXLib.Streams.Reader io2 = new CLKsFATXLib.Streams.Reader(xFile.GetStream());
            long num = this.getCultureOffset();
            io2.BaseStream.Position = (0x410L + num);
            byte[] data = new byte[0x100];
            data = io2.ReadBytes(0x100);
            this.log("Title : " + this.wr.unicodeToStr(data, 2) + "\r\n");
            io2.BaseStream.Position = (0xd10L + num);
            byte[] buffer2 = new byte[0x100];
            buffer2 = io2.ReadBytes(0x100);
            this.log("Description : " + this.wr.unicodeToStr(buffer2, 2) + "\r\n");
            io2.BaseStream.Position = (0x1610L);
            byte[] buffer3 = new byte[0x100];
            buffer3 = io2.ReadBytes(0x100);
            this.log("Publisher : " + this.wr.unicodeToStr(buffer3, 2) + "\r\n");
            //io2.Close();
        }
        /**
        *   Title:          Xbox 360 Forensic Toolkit.
        *   Description:    Xbox 360 Forensic Toolkit is built on top of the Party buffalo application. It allows users to read and 
        *                   write to Xbox 360 devices, with support for reading Xbox 360 STFS package names. A number of Forensic Tools have
        *                   been added to extract/view sectors, extract partitions, view file details, examine files in HEX. It also incorporates the wxPIRS
        *                   Application so that we can sissamble STFS packages and see their contents.
        *
        *   Original Author: Party Buffalo - landergriffith@gmail.com, wxPIRS - gael360
        *   Another Author: obriej62@mail.dcu.ie
        *   License:        http://www.gnu.org/licenses/gpl.html GNU GENERAL PUBLIC LICENSE
        *   Link:           https://code.google.com/p/party-buffalo/
        *                  http://gael360.free.fr/
        *   Note:           All Code that follows to the End point was added by obriej62@mail.dcu.ie
        */
        private void parseSFTS()
        {
            this.treeView.BeginUpdate();
            this.treeView.Nodes.Clear();
            this.listView.BeginUpdate();
            this.listView.Items.Clear();
            TreeNode node = new TreeNode(xFile.Name, 0, 0);
            node.Tag = (ushort)0xffff;
            node.ToolTipText = string.Format("0x{0:X4}", node.Tag);
            this.treeView.Nodes.Add(node);
            this.getDirectoriesSTFS(node);
            this.getFilesSTFS(node);
            node.Expand();
            this.listView.EndUpdate();
            this.treeView.EndUpdate();
        }
        /**
        *   Title:          Xbox 360 Forensic Toolkit.
        *   Description:    Xbox 360 Forensic Toolkit is built on top of the Party buffalo application. It allows users to read and 
        *                   write to Xbox 360 devices, with support for reading Xbox 360 STFS package names. A number of Forensic Tools have
        *                   been added to extract/view sectors, extract partitions, view file details, examine files in HEX. It also incorporates the wxPIRS
        *                   Application so that we can sissamble STFS packages and see their contents.
        *
        *   Original Author: Party Buffalo - landergriffith@gmail.com, wxPIRS - gael360
        *   Another Author: obriej62@mail.dcu.ie
        *   License:        http://www.gnu.org/licenses/gpl.html GNU GENERAL PUBLIC LICENSE
        *   Link:           https://code.google.com/p/party-buffalo/
        *                  http://gael360.free.fr/
        *   Note:           All Code that follows to the End point was added by obriej62@mail.dcu.ie
        */
        private void getDirectoriesSTFS(TreeNode tn)
        {
            int num = 0;
            long offset = 0;
            while (true)
            {
                offset = this.pirs_start + (num * 0x40);
                PirsEntry entry = new PirsEntry();
                entry = this.getEntrySTFS(offset);
                if (entry.Filename.Trim() == "")
                {
                    return;
                }
                if (((entry.Size == 0) && (entry.Cluster == 0)) && (entry.Parent == Convert.ToUInt16(tn.Tag)))
                {
                    TreeNode node = new TreeNode(entry.Filename);
                    node.Tag = num;
                    node.ToolTipText = string.Format("0x{0:X4}", node.Tag);
                    tn.Nodes.Add(node);
                    this.getDirectoriesSTFS(node);
                }
                num++;
            }
        }
        /**
        *   Title:          Xbox 360 Forensic Toolkit.
        *   Description:    Xbox 360 Forensic Toolkit is built on top of the Party buffalo application. It allows users to read and 
        *                   write to Xbox 360 devices, with support for reading Xbox 360 STFS package names. A number of Forensic Tools have
        *                   been added to extract/view sectors, extract partitions, view file details, examine files in HEX. It also incorporates the wxPIRS
        *                   Application so that we can sissamble STFS packages and see their contents.
        *
        *   Original Author: Party Buffalo - landergriffith@gmail.com, wxPIRS - gael360
        *   Another Author: obriej62@mail.dcu.ie
        *   License:        http://www.gnu.org/licenses/gpl.html GNU GENERAL PUBLIC LICENSE
        *   Link:           https://code.google.com/p/party-buffalo/
        *                  http://gael360.free.fr/
        *   Note:           All Code that follows to the End point was added by obriej62@mail.dcu.ie
        */
        private void getFilesSTFS(TreeNode tn)
        {
            int num = 0;
            long offset = 0;
            while (true)
            {
                offset = this.pirs_start + (num * 0x40);
                PirsEntry entry = new PirsEntry();
                entry = this.getEntrySTFS(offset);
                if (entry.Filename.Trim() == "")
                {
                    break;
                }
                if (((entry.Cluster == 0) && (entry.Size == 0)) && (entry.Parent == Convert.ToUInt16(tn.Tag)))
                {
                    ListViewItem item = new ListViewItem();
                    item.Text = entry.Filename;
                    item.SubItems.Add(entry.Size.ToString());
                    item.SubItems.Add(entry.Cluster.ToString());
                    item.SubItems.Add(entry.DateTime1.ToString());
                    item.SubItems.Add("");
                    item.ImageIndex = 0;
                    item.Tag = num;
                    this.listView.Items.Add(item);
                }
                num++;
            }
            num = 0;
            while (true)
            {
                offset = this.pirs_start + (num * 0x40);
                PirsEntry entry2 = new PirsEntry();
                entry2 = this.getEntrySTFS(offset);
                if (entry2.Filename.Trim() == "")
                {
                    return;
                }
                if ((entry2.Cluster != 0) && (entry2.Parent == Convert.ToUInt16(tn.Tag)))
                {
                    ListViewItem item2 = new ListViewItem();
                    item2.Text = entry2.Filename;
                    item2.SubItems.Add(entry2.Size.ToString());
                    item2.SubItems.Add(entry2.Cluster.ToString());
                    item2.SubItems.Add(entry2.DateTime1.ToString());
                    item2.SubItems.Add("");
                    item2.ImageIndex = 1;
                    item2.ToolTipText = string.Format("Offset : 0x{0:X8}", this.getOffset((long)entry2.Cluster));
                    this.listView.Items.Add(item2);
                }
                num++;
            }
        }
        /**
        *   Title:          Xbox 360 Forensic Toolkit.
        *   Description:    Xbox 360 Forensic Toolkit is built on top of the Party buffalo application. It allows users to read and 
        *                   write to Xbox 360 devices, with support for reading Xbox 360 STFS package names. A number of Forensic Tools have
        *                   been added to extract/view sectors, extract partitions, view file details, examine files in HEX. It also incorporates the wxPIRS
        *                   Application so that we can sissamble STFS packages and see their contents.
        *
        *   Original Author: Party Buffalo - landergriffith@gmail.com, wxPIRS - gael360
        *   Another Author: obriej62@mail.dcu.ie
        *   License:        http://www.gnu.org/licenses/gpl.html GNU GENERAL PUBLIC LICENSE
        *   Link:           https://code.google.com/p/party-buffalo/
        *                  http://gael360.free.fr/
        *   Note:           All Code that follows to the End point was added by obriej62@mail.dcu.ie
        */
        private PirsEntry getEntrySTFS(long num)
        {
            CLKsFATXLib.Streams.Reader io2 = new CLKsFATXLib.Streams.Reader(xFile.GetStream());
            io2.BaseStream.Position = (num);
            PirsEntry entry = new PirsEntry();
            char ch;
            string str = "";
            for (uint i = 0; i < 0x26; i++)
            {
                ch = io2.ReadChar();
                if (ch != '\0')
                {
                    str = str + Convert.ToString(ch);
                }
            }

            entry.Filename = str;
            if (entry.Filename.Trim() != "")
            {
                entry.Unknow = io2.ReadInt32();
                entry.BlockLen = io2.ReadInt32();
                int x = io2.ReadInt32();
                entry.Cluster = (x >> 16);
                entry.Parent = io2.ReadUInt16();
                entry.Size = io2.ReadInt32();
                entry.DateTime1 = this.dosDateTime(io2.ReadInt32());
                entry.DateTime2 = this.dosDateTime(io2.ReadInt32());
            }
            //io2.Close();
            return entry;
        }
        /**
        *   Title:          Xbox 360 Forensic Toolkit.
        *   Description:    Xbox 360 Forensic Toolkit is built on top of the Party buffalo application. It allows users to read and 
        *                   write to Xbox 360 devices, with support for reading Xbox 360 STFS package names. A number of Forensic Tools have
        *                   been added to extract/view sectors, extract partitions, view file details, examine files in HEX. It also incorporates the wxPIRS
        *                   Application so that we can sissamble STFS packages and see their contents.
        *
        *   Original Author: Party Buffalo - landergriffith@gmail.com, wxPIRS - gael360
        *   Another Author: obriej62@mail.dcu.ie
        *   License:        http://www.gnu.org/licenses/gpl.html GNU GENERAL PUBLIC LICENSE
        *   Link:           https://code.google.com/p/party-buffalo/
        *                  http://gael360.free.fr/
        *   Note:           All Code that follows to the End point was added by obriej62@mail.dcu.ie
        */
        private void extractFileSFTS(ListViewItem listViewItem, string filename)
        {
            FileStream stream;
            BinaryWriter writer; 
            try
            {
                if (!Directory.Exists(this.wr.extractFolderName(filename)))
                {
                    Directory.CreateDirectory(this.wr.extractFolderName(filename));
                }
            }
            catch (IOException)
            {
            }
            try
            {
                stream = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None);
                writer = new BinaryWriter(stream);
            }
            catch (IOException exception)
            {
                this.log(string.Format("Error : {0}\r\n", exception));
                return;
            }
            long cluster = Convert.ToInt64(listViewItem.SubItems[this.columnHeaderCluster.Index].Text);
            long offset = this.getOffset(cluster);
            long num3 = Convert.ToInt64(listViewItem.SubItems[this.columnHeaderSize.Index].Text);
            long num4 = num3 >> 12;
            long num5 = num3 - (num4 << 12);
            for (long i = cluster; i < (cluster + num4); i += 1L)
            {
                offset = this.getOffset(i);
                this.br.BaseStream.Seek(offset, SeekOrigin.Begin);
                writer.Write(this.br.ReadBytes(0x1000));
                string str = string.Format("{0}%", (100L * (i - cluster)) / num4);
                if (str != listViewItem.SubItems[this.columnHeaderStatus.Index].Text)
                {
                    listViewItem.SubItems[this.columnHeaderStatus.Index].Text = str;
                    Application.DoEvents();
                }
            }
            offset = this.getOffset(cluster + num4);
            this.br.BaseStream.Seek(offset, SeekOrigin.Begin);
            writer.Write(this.br.ReadBytes((int)num5));
            listViewItem.SubItems[this.columnHeaderStatus.Index].Text = "Done";
            Application.DoEvents();
            writer.Close();
            stream.Dispose();
        }

    }
}
