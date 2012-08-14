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
    using WxTools;

    public class MainForm : Form
    {
        private ToolStripMenuItem aboutPirsToolStripMenuItem;
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
        private ToolStripButton extractAllToolStripButton;
        private ToolStripMenuItem extractAllToolStripMenuItem;
        private ToolStripMenuItem extractFilesToolStripMenuItem;
        private ToolStripMenuItem extractFileToolStripMenuItem;
        private ToolStripMenuItem extractFolderToolStripMenuItem;
        private ToolStrip fileToolStrip;
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
        private ToolStripButton openFileToolStripButton;
        private ToolStripMenuItem openFileToolStripMenuItem;
        public static long PIRS_BASE = 0xb000L;
        private long pirs_offset;
        private long pirs_start;
        public static long PIRS_TYPE1 = 0x1000L;
        public static long PIRS_TYPE2 = 0x2000L;
        private SaveFileDialog saveFileDialog;
        private SplitContainer splitContainerH;
        private SplitContainer splitContainerV;
        private StatusStrip statusStrip;
        private TextBox textBoxLog;
        private ToolStripContainer toolStripContainer1;
        private ToolStripMenuItem toolStripMenuItem1;
        private ToolStripSeparator toolStripMenuItem2;
        private ToolStripStatusLabel toolStripStatusLabelGael360;
        private ToolStripStatusLabel toolStripStatusLabelSeparator;
        private ToolStripStatusLabel toolStripStatusLabelVersion;
        private TreeView treeView;
        private WxReader wr = new WxReader();

        public MainForm(string[] args)
        {
            this.args = args;
            this.InitializeComponent();
        }

        private void aboutPirsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox box = new AboutBox();
            box.ShowDialog();
            box.Dispose();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
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
            writer.Write(this.br.ReadBytes((int) num5));
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
        }

        private void extractFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.saveFileDialog.FileName = this.listView.SelectedItems[0].Text;
            if (this.saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                this.extractFile(this.listView.SelectedItems[0], this.saveFileDialog.FileName);
            }
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
            while (true)
            {
                this.br.BaseStream.Seek((PIRS_BASE + this.pirs_offset) + (num * 0x40), SeekOrigin.Begin);
                PirsEntry entry = new PirsEntry();
                entry = this.getEntry();
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
            this.components = new Container();
            ComponentResourceManager manager = new ComponentResourceManager(typeof(MainForm));
            this.menuStrip = new MenuStrip();
            this.fileToolStripMenuItem = new ToolStripMenuItem();
            this.openFileToolStripMenuItem = new ToolStripMenuItem();
            this.extractAllToolStripMenuItem = new ToolStripMenuItem();
            this.toolStripMenuItem2 = new ToolStripSeparator();
            this.exitToolStripMenuItem = new ToolStripMenuItem();
            this.toolStripMenuItem1 = new ToolStripMenuItem();
            this.aboutPirsToolStripMenuItem = new ToolStripMenuItem();
            this.statusStrip = new StatusStrip();
            this.toolStripStatusLabelVersion = new ToolStripStatusLabel();
            this.toolStripStatusLabelSeparator = new ToolStripStatusLabel();
            this.toolStripStatusLabelGael360 = new ToolStripStatusLabel();
            this.toolStripContainer1 = new ToolStripContainer();
            this.splitContainerH = new SplitContainer();
            this.splitContainerV = new SplitContainer();
            this.treeView = new TreeView();
            this.imageListTreeView = new ImageList(this.components);
            this.listView = new ListView();
            this.columnHeaderName = new ColumnHeader();
            this.columnHeaderSize = new ColumnHeader();
            this.columnHeaderCluster = new ColumnHeader();
            this.columnHeaderDateModified = new ColumnHeader();
            this.columnHeaderStatus = new ColumnHeader();
            this.imageList = new ImageList(this.components);
            this.textBoxLog = new TextBox();
            this.fileToolStrip = new ToolStrip();
            this.openFileToolStripButton = new ToolStripButton();
            this.extractAllToolStripButton = new ToolStripButton();
            this.contextMenuStripFolder = new ContextMenuStrip(this.components);
            this.extractFolderToolStripMenuItem = new ToolStripMenuItem();
            this.openFileDialog = new OpenFileDialog();
            this.contextMenuStrip = new ContextMenuStrip(this.components);
            this.extractFileToolStripMenuItem = new ToolStripMenuItem();
            this.contextMenuStripMulti = new ContextMenuStrip(this.components);
            this.extractFilesToolStripMenuItem = new ToolStripMenuItem();
            this.saveFileDialog = new SaveFileDialog();
            this.folderBrowserDialog = new FolderBrowserDialog();
            this.menuStrip.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.toolStripContainer1.ContentPanel.SuspendLayout();
            this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            this.splitContainerH.Panel1.SuspendLayout();
            this.splitContainerH.Panel2.SuspendLayout();
            this.splitContainerH.SuspendLayout();
            this.splitContainerV.Panel1.SuspendLayout();
            this.splitContainerV.Panel2.SuspendLayout();
            this.splitContainerV.SuspendLayout();
            this.fileToolStrip.SuspendLayout();
            this.contextMenuStripFolder.SuspendLayout();
            this.contextMenuStrip.SuspendLayout();
            this.contextMenuStripMulti.SuspendLayout();
            base.SuspendLayout();
            this.menuStrip.Items.AddRange(new ToolStripItem[] { this.fileToolStripMenuItem, this.toolStripMenuItem1 });
            this.menuStrip.Location = new Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new Size(0x2d9, 0x18);
            this.menuStrip.TabIndex = 0;
            this.menuStrip.Text = "menuStrip1";
            this.fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { this.openFileToolStripMenuItem, this.extractAllToolStripMenuItem, this.toolStripMenuItem2, this.exitToolStripMenuItem });
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new Size(0x23, 20);
            this.fileToolStripMenuItem.Text = "File";
            this.openFileToolStripMenuItem.Name = "openFileToolStripMenuItem";
            this.openFileToolStripMenuItem.Size = new Size(0x86, 0x16);
            this.openFileToolStripMenuItem.Text = "Open...";
            this.openFileToolStripMenuItem.Click += new EventHandler(this.openFile);
            this.extractAllToolStripMenuItem.Name = "extractAllToolStripMenuItem";
            this.extractAllToolStripMenuItem.Size = new Size(0x86, 0x16);
            this.extractAllToolStripMenuItem.Text = "Extract all...";
            this.extractAllToolStripMenuItem.Click += new EventHandler(this.extractAll);
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new Size(0x83, 6);
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new Size(0x86, 0x16);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new EventHandler(this.exitToolStripMenuItem_Click);
            this.toolStripMenuItem1.DropDownItems.AddRange(new ToolStripItem[] { this.aboutPirsToolStripMenuItem });
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new Size(0x18, 20);
            this.toolStripMenuItem1.Text = "?";
            this.aboutPirsToolStripMenuItem.Name = "aboutPirsToolStripMenuItem";
            this.aboutPirsToolStripMenuItem.Size = new Size(0x7b, 0x16);
            this.aboutPirsToolStripMenuItem.Text = "About Pirs";
            this.aboutPirsToolStripMenuItem.Click += new EventHandler(this.aboutPirsToolStripMenuItem_Click);
            this.statusStrip.Items.AddRange(new ToolStripItem[] { this.toolStripStatusLabelVersion, this.toolStripStatusLabelSeparator, this.toolStripStatusLabelGael360 });
            this.statusStrip.Location = new Point(0, 0x175);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new Size(0x2d9, 0x16);
            this.statusStrip.TabIndex = 1;
            this.statusStrip.Text = "statusStrip1";
            this.toolStripStatusLabelVersion.Name = "toolStripStatusLabelVersion";
            this.toolStripStatusLabelVersion.Size = new Size(0x298, 0x11);
            this.toolStripStatusLabelVersion.Spring = true;
            this.toolStripStatusLabelVersion.Text = "wxPirs";
            this.toolStripStatusLabelVersion.TextAlign = ContentAlignment.MiddleLeft;
            this.toolStripStatusLabelSeparator.BorderSides = ToolStripStatusLabelBorderSides.All;
            this.toolStripStatusLabelSeparator.BorderStyle = Border3DStyle.Sunken;
            this.toolStripStatusLabelSeparator.Name = "toolStripStatusLabelSeparator";
            this.toolStripStatusLabelSeparator.Size = new Size(4, 0x11);
            this.toolStripStatusLabelGael360.Name = "toolStripStatusLabelGael360";
            this.toolStripStatusLabelGael360.Size = new Size(0x2e, 0x11);
            this.toolStripStatusLabelGael360.Text = "Gael360";
            this.toolStripContainer1.ContentPanel.Controls.Add(this.splitContainerH);
            this.toolStripContainer1.ContentPanel.Size = new Size(0x2d9, 0x144);
            this.toolStripContainer1.Dock = DockStyle.Fill;
            this.toolStripContainer1.Location = new Point(0, 0x18);
            this.toolStripContainer1.Name = "toolStripContainer1";
            this.toolStripContainer1.Size = new Size(0x2d9, 0x15d);
            this.toolStripContainer1.TabIndex = 2;
            this.toolStripContainer1.Text = "toolStripContainer1";
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.fileToolStrip);
            this.splitContainerH.Dock = DockStyle.Fill;
            this.splitContainerH.FixedPanel = FixedPanel.Panel2;
            this.splitContainerH.Location = new Point(0, 0);
            this.splitContainerH.Name = "splitContainerH";
            this.splitContainerH.Orientation = Orientation.Horizontal;
            this.splitContainerH.Panel1.Controls.Add(this.splitContainerV);
            this.splitContainerH.Panel2.Controls.Add(this.textBoxLog);
            this.splitContainerH.Size = new Size(0x2d9, 0x144);
            this.splitContainerH.SplitterDistance = 0xf3;
            this.splitContainerH.TabIndex = 0;
            this.splitContainerV.Dock = DockStyle.Fill;
            this.splitContainerV.FixedPanel = FixedPanel.Panel1;
            this.splitContainerV.Location = new Point(0, 0);
            this.splitContainerV.Name = "splitContainerV";
            this.splitContainerV.Panel1.Controls.Add(this.treeView);
            this.splitContainerV.Panel2.Controls.Add(this.listView);
            this.splitContainerV.Size = new Size(0x2d9, 0xf3);
            this.splitContainerV.SplitterDistance = 180;
            this.splitContainerV.TabIndex = 0;
            this.treeView.Dock = DockStyle.Fill;
            this.treeView.ImageIndex = 1;
            this.treeView.ImageList = this.imageListTreeView;
            this.treeView.Location = new Point(0, 0);
            this.treeView.Name = "treeView";
            this.treeView.SelectedImageIndex = 2;
            this.treeView.ShowRootLines = false;
            this.treeView.Size = new Size(180, 0xf3);
            this.treeView.TabIndex = 0;
            this.treeView.AfterSelect += new TreeViewEventHandler(this.treeView_AfterSelect);
            this.treeView.NodeMouseClick += new TreeNodeMouseClickEventHandler(this.treeView_NodeMouseClick);
            //this.imageListTreeView.ImageStream = (ImageListStreamer) manager.GetObject("imageListTreeView.ImageStream");
            this.imageListTreeView.TransparentColor = Color.Fuchsia;
            //this.imageListTreeView.Images.SetKeyName(0, "Control_FolderBrowserDialog.bmp");
            //this.imageListTreeView.Images.SetKeyName(1, "VSFolder_closed.bmp");
            //this.imageListTreeView.Images.SetKeyName(2, "VSFolder_open.bmp");
            this.listView.Columns.AddRange(new ColumnHeader[] { this.columnHeaderName, this.columnHeaderSize, this.columnHeaderCluster, this.columnHeaderDateModified, this.columnHeaderStatus });
            this.listView.Dock = DockStyle.Fill;
            this.listView.FullRowSelect = true;
            this.listView.Location = new Point(0, 0);
            this.listView.Name = "listView";
            this.listView.ShowItemToolTips = true;
            this.listView.Size = new Size(0x221, 0xf3);
            this.listView.SmallImageList = this.imageList;
            this.listView.TabIndex = 0;
            this.listView.UseCompatibleStateImageBehavior = false;
            this.listView.View = View.Details;
            this.listView.MouseClick += new MouseEventHandler(this.listView_MouseClick);
            this.columnHeaderName.Text = "Name";
            this.columnHeaderName.Width = 200;
            this.columnHeaderSize.Text = "Size";
            this.columnHeaderSize.TextAlign = HorizontalAlignment.Right;
            this.columnHeaderCluster.Text = "Cluster";
            this.columnHeaderCluster.TextAlign = HorizontalAlignment.Right;
            this.columnHeaderDateModified.Text = "Date modified";
            this.columnHeaderDateModified.Width = 120;
            this.columnHeaderStatus.Text = "Status";
            this.columnHeaderStatus.Width = 80;
            //this.imageList.ImageStream = (ImageListStreamer) manager.GetObject("imageList.ImageStream");
            this.imageList.TransparentColor = Color.Fuchsia;
            //this.imageList.Images.SetKeyName(0, "VSFolder_closed.bmp");
            //this.imageList.Images.SetKeyName(1, "DocumentHS.png");
            this.textBoxLog.Dock = DockStyle.Fill;
            this.textBoxLog.Location = new Point(0, 0);
            this.textBoxLog.Multiline = true;
            this.textBoxLog.Name = "textBoxLog";
            this.textBoxLog.ScrollBars = ScrollBars.Both;
            this.textBoxLog.Size = new Size(0x2d9, 0x4d);
            this.textBoxLog.TabIndex = 0;
            this.fileToolStrip.Dock = DockStyle.None;
            this.fileToolStrip.Items.AddRange(new ToolStripItem[] { this.openFileToolStripButton, this.extractAllToolStripButton });
            this.fileToolStrip.Location = new Point(3, 0);
            this.fileToolStrip.Name = "fileToolStrip";
            this.fileToolStrip.Size = new Size(0x3a, 0x19);
            this.fileToolStrip.TabIndex = 0;
            this.fileToolStrip.Text = "File";
            this.openFileToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            //this.openFileToolStripButton.Image = Resources.openHS;
            this.openFileToolStripButton.ImageTransparentColor = Color.Magenta;
            this.openFileToolStripButton.Name = "openFileToolStripButton";
            this.openFileToolStripButton.Size = new Size(0x17, 0x16);
            this.openFileToolStripButton.Text = "Open file";
            this.openFileToolStripButton.Click += new EventHandler(this.openFile);
            this.extractAllToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            //this.extractAllToolStripButton.Image = Resources.SaveAllHS;
            this.extractAllToolStripButton.ImageTransparentColor = Color.Magenta;
            this.extractAllToolStripButton.Name = "extractAllToolStripButton";
            this.extractAllToolStripButton.Size = new Size(0x17, 0x16);
            this.extractAllToolStripButton.Text = "Extract all";
            this.extractAllToolStripButton.Click += new EventHandler(this.extractAll);
            this.contextMenuStripFolder.Items.AddRange(new ToolStripItem[] { this.extractFolderToolStripMenuItem });
            this.contextMenuStripFolder.Name = "contextMenuStripFolder";
            this.contextMenuStripFolder.Size = new Size(0x8d, 0x1a);
            this.extractFolderToolStripMenuItem.Name = "extractFolderToolStripMenuItem";
            this.extractFolderToolStripMenuItem.Size = new Size(140, 0x16);
            this.extractFolderToolStripMenuItem.Text = "Extract folder";
            this.extractFolderToolStripMenuItem.Click += new EventHandler(this.extractFolder);
            this.openFileDialog.Filter = "All files (*.*)|*.*";
            this.contextMenuStrip.Items.AddRange(new ToolStripItem[] { this.extractFileToolStripMenuItem });
            this.contextMenuStrip.Name = "contextMenuStrip";
            this.contextMenuStrip.Size = new Size(0x7f, 0x1a);
            this.extractFileToolStripMenuItem.Name = "extractFileToolStripMenuItem";
            this.extractFileToolStripMenuItem.Size = new Size(0x7e, 0x16);
            this.extractFileToolStripMenuItem.Text = "Extract file";
            this.extractFileToolStripMenuItem.Click += new EventHandler(this.extractFileToolStripMenuItem_Click);
            this.contextMenuStripMulti.Items.AddRange(new ToolStripItem[] { this.extractFilesToolStripMenuItem });
            this.contextMenuStripMulti.Name = "contextMenuStripMulti";
            this.contextMenuStripMulti.Size = new Size(0x84, 0x1a);
            this.extractFilesToolStripMenuItem.Name = "extractFilesToolStripMenuItem";
            this.extractFilesToolStripMenuItem.Size = new Size(0x83, 0x16);
            this.extractFilesToolStripMenuItem.Text = "Extract files";
            this.extractFilesToolStripMenuItem.Click += new EventHandler(this.extractFilesToolStripMenuItem_Click);
            this.saveFileDialog.Filter = "All files (*.*)|*.*";
            base.AutoScaleDimensions = new SizeF(6f, 13f);
            //base.AutoScaleMode = AutoScaleMode.Font;
            base.ClientSize = new Size(0x2d9, 0x18b);
            base.Controls.Add(this.toolStripContainer1);
            base.Controls.Add(this.statusStrip);
            base.Controls.Add(this.menuStrip);
            //base.Icon = (Icon) manager.GetObject("$this.Icon");
            base.MainMenuStrip = this.menuStrip;
            base.Name = "MainForm";
            this.Text = "wxPirs";
            base.FormClosing += new FormClosingEventHandler(this.MainForm_FormClosing);
            base.Load += new EventHandler(this.MainForm_Load);
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.toolStripContainer1.ContentPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.PerformLayout();
            this.toolStripContainer1.ResumeLayout(false);
            this.toolStripContainer1.PerformLayout();
            this.splitContainerH.Panel1.ResumeLayout(false);
            this.splitContainerH.Panel2.ResumeLayout(false);
            this.splitContainerH.Panel2.PerformLayout();
            this.splitContainerH.ResumeLayout(false);
            this.splitContainerV.Panel1.ResumeLayout(false);
            this.splitContainerV.Panel2.ResumeLayout(false);
            this.splitContainerV.ResumeLayout(false);
            this.fileToolStrip.ResumeLayout(false);
            this.fileToolStrip.PerformLayout();
            this.contextMenuStripFolder.ResumeLayout(false);
            this.contextMenuStrip.ResumeLayout(false);
            this.contextMenuStripMulti.ResumeLayout(false);
            base.ResumeLayout(false);
            base.PerformLayout();
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
            this.toolStripStatusLabelVersion.Text = Application.ProductName + " - " + Assembly.GetExecutingAssembly().GetName().Version.ToString();
            if (this.args.Length > 0)
            {
                this.openFileDialog.FileName = this.args[0];
                this.openFile(this.args[0]);
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
            this.getFiles(e.Node);
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
    }
}

