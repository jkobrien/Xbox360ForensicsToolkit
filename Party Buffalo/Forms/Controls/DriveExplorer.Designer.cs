namespace Party_Buffalo
{
    partial class DriveExplorer
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.TreeNode treeNode21 = new System.Windows.Forms.TreeNode("Drive", 2, 2);
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DriveExplorer));
            this.listView1 = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader4 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader5 = new System.Windows.Forms.ColumnHeader();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.b_Back = new System.Windows.Forms.ToolStripButton();
            this.b_Forward = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButton2 = new System.Windows.Forms.ToolStripButton();
            this.openToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.ts_Reload = new System.Windows.Forms.ToolStripButton();
            this.ts_Close = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.statusLabel = new System.Windows.Forms.ToolStripLabel();
            this.quickMessage = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.t_EntryCount = new System.Windows.Forms.ToolStripLabel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.pathBar = new System.Windows.Forms.ComboBox();
            this.goButton = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.treeview_ContextMenu = new System.Windows.Forms.ContextMenu();
            this.t_Refresh = new System.Windows.Forms.MenuItem();
            this.t_Extract = new System.Windows.Forms.MenuItem();
            this.menuItem21 = new System.Windows.Forms.MenuItem();
            this.t_Rename = new System.Windows.Forms.MenuItem();
            this.t_InjectFile = new System.Windows.Forms.MenuItem();
            this.t_InjectFolder = new System.Windows.Forms.MenuItem();
            this.t_NewFolder = new System.Windows.Forms.MenuItem();
            this.menuItem34 = new System.Windows.Forms.MenuItem();
            this.tCached = new System.Windows.Forms.MenuItem();
            this.tStatic = new System.Windows.Forms.MenuItem();
            this.menuItem25 = new System.Windows.Forms.MenuItem();
            this.t_Delete = new System.Windows.Forms.MenuItem();
            this.menuItem19 = new System.Windows.Forms.MenuItem();
            this.t_AddToBookmarks = new System.Windows.Forms.MenuItem();
            this.t_CopyPath = new System.Windows.Forms.MenuItem();
            this.t_Properties = new System.Windows.Forms.MenuItem();
            this.menuItem18 = new System.Windows.Forms.MenuItem();
            this.t_driveProperties = new System.Windows.Forms.MenuItem();
            this.listview_ContextMenu = new System.Windows.Forms.ContextMenu();
            this.c_Refresh = new System.Windows.Forms.MenuItem();
            this.c_Extract = new System.Windows.Forms.MenuItem();
            this.menuItem31 = new System.Windows.Forms.MenuItem();
            this.c_Rename = new System.Windows.Forms.MenuItem();
            this.c_InjectFile = new System.Windows.Forms.MenuItem();
            this.c_InjectFolder = new System.Windows.Forms.MenuItem();
            this.c_NewFolder = new System.Windows.Forms.MenuItem();
            this.menuItem15 = new System.Windows.Forms.MenuItem();
            this.lCached = new System.Windows.Forms.MenuItem();
            this.lStatic = new System.Windows.Forms.MenuItem();
            this.menuItem24 = new System.Windows.Forms.MenuItem();
            this.c_Delete = new System.Windows.Forms.MenuItem();
            this.menuItem11 = new System.Windows.Forms.MenuItem();
            this.l_AddToBookMarks = new System.Windows.Forms.MenuItem();
            this.c_Properties = new System.Windows.Forms.MenuItem();
            this.toolStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // listView1
            // 
            this.listView1.AllowDrop = true;
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader5});
            this.listView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView1.FullRowSelect = true;
            this.listView1.GridLines = true;
            this.listView1.LabelEdit = true;
            this.listView1.Location = new System.Drawing.Point(278, 25);
            this.listView1.Name = "listView1";
            this.listView1.ShowItemToolTips = true;
            this.listView1.Size = new System.Drawing.Size(721, 353);
            this.listView1.TabIndex = 4;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Name";
            this.columnHeader1.Width = 150;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Type";
            this.columnHeader2.Width = 100;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Size";
            this.columnHeader3.Width = 100;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Date";
            this.columnHeader4.Width = 160;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "STFS Name";
            this.columnHeader5.Width = 160;
            // 
            // treeView1
            // 
            this.treeView1.Dock = System.Windows.Forms.DockStyle.Left;
            this.treeView1.Location = new System.Drawing.Point(0, 25);
            this.treeView1.Name = "treeView1";
            treeNode21.ImageIndex = 2;
            treeNode21.Name = "Node0";
            treeNode21.SelectedImageIndex = 2;
            treeNode21.Tag = "";
            treeNode21.Text = "Drive";
            this.treeView1.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode21});
            this.treeView1.ShowNodeToolTips = true;
            this.treeView1.Size = new System.Drawing.Size(278, 353);
            this.treeView1.TabIndex = 3;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.b_Back,
            this.b_Forward,
            this.toolStripSeparator3,
            this.toolStripButton2,
            this.openToolStripButton,
            this.ts_Reload,
            this.ts_Close,
            this.toolStripSeparator1,
            this.statusLabel,
            this.quickMessage,
            this.toolStripSeparator2,
            this.t_EntryCount});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(999, 25);
            this.toolStrip1.TabIndex = 2;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // b_Back
            // 
            this.b_Back.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.b_Back.Enabled = false;
            this.b_Back.Image = ((System.Drawing.Image)(resources.GetObject("b_Back.Image")));
            this.b_Back.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.b_Back.Name = "b_Back";
            this.b_Back.Size = new System.Drawing.Size(23, 22);
            this.b_Back.Text = "Back";
            this.b_Back.ToolTipText = "Browse Back";
            // 
            // b_Forward
            // 
            this.b_Forward.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.b_Forward.Enabled = false;
            this.b_Forward.Image = ((System.Drawing.Image)(resources.GetObject("b_Forward.Image")));
            this.b_Forward.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.b_Forward.Name = "b_Forward";
            this.b_Forward.Size = new System.Drawing.Size(23, 22);
            this.b_Forward.Text = "Forward";
            this.b_Forward.ToolTipText = "Browse Forward";
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButton2
            // 
            this.toolStripButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton2.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton2.Image")));
            this.toolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton2.Name = "toolStripButton2";
            this.toolStripButton2.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton2.Text = "Open &Drive";
            // 
            // openToolStripButton
            // 
            this.openToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.openToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("openToolStripButton.Image")));
            this.openToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.openToolStripButton.Name = "openToolStripButton";
            this.openToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.openToolStripButton.Text = "&Open";
            this.openToolStripButton.ToolTipText = "Open File";
            // 
            // ts_Reload
            // 
            this.ts_Reload.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.ts_Reload.Enabled = false;
            this.ts_Reload.Image = ((System.Drawing.Image)(resources.GetObject("ts_Reload.Image")));
            this.ts_Reload.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ts_Reload.Name = "ts_Reload";
            this.ts_Reload.Size = new System.Drawing.Size(23, 22);
            this.ts_Reload.Text = "&Refresh";
            this.ts_Reload.Visible = false;
            // 
            // ts_Close
            // 
            this.ts_Close.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.ts_Close.Enabled = false;
            this.ts_Close.Image = ((System.Drawing.Image)(resources.GetObject("ts_Close.Image")));
            this.ts_Close.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ts_Close.Name = "ts_Close";
            this.ts_Close.Size = new System.Drawing.Size(23, 22);
            this.ts_Close.Text = "&Close Drive";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // statusLabel
            // 
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(0, 22);
            // 
            // quickMessage
            // 
            this.quickMessage.Name = "quickMessage";
            this.quickMessage.Size = new System.Drawing.Size(90, 22);
            this.quickMessage.Text = "Quick Message:";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // t_EntryCount
            // 
            this.t_EntryCount.Name = "t_EntryCount";
            this.t_EntryCount.Size = new System.Drawing.Size(115, 22);
            this.t_EntryCount.Text = "No Folder Selected...";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.pathBar);
            this.panel1.Controls.Add(this.goButton);
            this.panel1.Controls.Add(this.button1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 378);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(999, 22);
            this.panel1.TabIndex = 5;
            // 
            // pathBar
            // 
            this.pathBar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pathBar.FormattingEnabled = true;
            this.pathBar.Location = new System.Drawing.Point(0, 0);
            this.pathBar.Name = "pathBar";
            this.pathBar.Size = new System.Drawing.Size(897, 21);
            this.pathBar.TabIndex = 8;
            // 
            // goButton
            // 
            this.goButton.Dock = System.Windows.Forms.DockStyle.Right;
            this.goButton.Location = new System.Drawing.Point(897, 0);
            this.goButton.Name = "goButton";
            this.goButton.Size = new System.Drawing.Size(72, 22);
            this.goButton.TabIndex = 6;
            this.goButton.Text = "Go";
            this.goButton.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Dock = System.Windows.Forms.DockStyle.Right;
            this.button1.Location = new System.Drawing.Point(969, 0);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(30, 22);
            this.button1.TabIndex = 7;
            this.button1.Text = "...";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // splitter1
            // 
            this.splitter1.Location = new System.Drawing.Point(278, 25);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(3, 353);
            this.splitter1.TabIndex = 6;
            this.splitter1.TabStop = false;
            // 
            // treeview_ContextMenu
            // 
            this.treeview_ContextMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.t_Refresh,
            this.t_Extract,
            this.menuItem21,
            this.t_Rename,
            this.t_InjectFile,
            this.t_InjectFolder,
            this.t_NewFolder,
            this.menuItem34,
            this.menuItem25,
            this.t_Delete,
            this.menuItem19,
            this.t_AddToBookmarks,
            this.t_CopyPath,
            this.t_Properties,
            this.menuItem18,
            this.t_driveProperties});
            // 
            // t_Refresh
            // 
            this.t_Refresh.Index = 0;
            this.t_Refresh.Text = "Refresh";
            // 
            // t_Extract
            // 
            this.t_Extract.Index = 1;
            this.t_Extract.Text = "Extract";
            // 
            // menuItem21
            // 
            this.menuItem21.Index = 2;
            this.menuItem21.Text = "-";
            // 
            // t_Rename
            // 
            this.t_Rename.Index = 3;
            this.t_Rename.Text = "Rename";
            this.t_Rename.Visible = false;
            // 
            // t_InjectFile
            // 
            this.t_InjectFile.Index = 4;
            this.t_InjectFile.Text = "Inject New File";
            // 
            // t_InjectFolder
            // 
            this.t_InjectFolder.Index = 5;
            this.t_InjectFolder.Text = "Inject New Folder";
            // 
            // t_NewFolder
            // 
            this.t_NewFolder.Index = 6;
            this.t_NewFolder.Text = "New Folder";
            // 
            // menuItem34
            // 
            this.menuItem34.Index = 7;
            this.menuItem34.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.tCached,
            this.tStatic});
            this.menuItem34.Text = "New Known Folder";
            // 
            // tCached
            // 
            this.tCached.Index = 0;
            this.tCached.Text = "Cached";
            // 
            // tStatic
            // 
            this.tStatic.Index = 1;
            this.tStatic.Text = "Static Subfolder";
            // 
            // menuItem25
            // 
            this.menuItem25.Index = 8;
            this.menuItem25.Text = "-";
            // 
            // t_Delete
            // 
            this.t_Delete.Index = 9;
            this.t_Delete.Text = "Delete";
            // 
            // menuItem19
            // 
            this.menuItem19.Index = 10;
            this.menuItem19.Text = "-";
            // 
            // t_AddToBookmarks
            // 
            this.t_AddToBookmarks.Index = 11;
            this.t_AddToBookmarks.Text = "Add To Bookmarks";
            // 
            // t_CopyPath
            // 
            this.t_CopyPath.Index = 12;
            this.t_CopyPath.Text = "Copy Path";
            // 
            // t_Properties
            // 
            this.t_Properties.Index = 13;
            this.t_Properties.Text = "Properties";
            // 
            // menuItem18
            // 
            this.menuItem18.Index = 14;
            this.menuItem18.Text = "-";
            // 
            // t_driveProperties
            // 
            this.t_driveProperties.Index = 15;
            this.t_driveProperties.Text = "Hard Drive Partition Geometry";
            // 
            // listview_ContextMenu
            // 
            this.listview_ContextMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.c_Refresh,
            this.c_Extract,
            this.menuItem31,
            this.c_Rename,
            this.c_InjectFile,
            this.c_InjectFolder,
            this.c_NewFolder,
            this.menuItem15,
            this.menuItem24,
            this.c_Delete,
            this.menuItem11,
            this.l_AddToBookMarks,
            this.c_Properties});
            // 
            // c_Refresh
            // 
            this.c_Refresh.Index = 0;
            this.c_Refresh.Shortcut = System.Windows.Forms.Shortcut.CtrlR;
            this.c_Refresh.Text = "Refresh";
            // 
            // c_Extract
            // 
            this.c_Extract.Index = 1;
            this.c_Extract.Text = "Extract";
            // 
            // menuItem31
            // 
            this.menuItem31.Index = 2;
            this.menuItem31.Text = "-";
            // 
            // c_Rename
            // 
            this.c_Rename.Index = 3;
            this.c_Rename.Text = "Rename";
            // 
            // c_InjectFile
            // 
            this.c_InjectFile.Index = 4;
            this.c_InjectFile.Shortcut = System.Windows.Forms.Shortcut.CtrlN;
            this.c_InjectFile.Text = "Inject New File";
            // 
            // c_InjectFolder
            // 
            this.c_InjectFolder.Index = 5;
            this.c_InjectFolder.Shortcut = System.Windows.Forms.Shortcut.CtrlShiftF;
            this.c_InjectFolder.Text = "Inject Folder";
            // 
            // c_NewFolder
            // 
            this.c_NewFolder.Index = 6;
            this.c_NewFolder.Shortcut = System.Windows.Forms.Shortcut.CtrlN;
            this.c_NewFolder.Text = "New Folder";
            // 
            // menuItem15
            // 
            this.menuItem15.Index = 7;
            this.menuItem15.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.lCached,
            this.lStatic});
            this.menuItem15.Text = "New Known Folder";
            // 
            // lCached
            // 
            this.lCached.Index = 0;
            this.lCached.Text = "Cached";
            // 
            // lStatic
            // 
            this.lStatic.Index = 1;
            this.lStatic.Text = "Static Subfolders";
            // 
            // menuItem24
            // 
            this.menuItem24.Index = 8;
            this.menuItem24.Text = "-";
            // 
            // c_Delete
            // 
            this.c_Delete.Index = 9;
            this.c_Delete.Text = "Delete";
            // 
            // menuItem11
            // 
            this.menuItem11.Index = 10;
            this.menuItem11.Text = "-";
            // 
            // l_AddToBookMarks
            // 
            this.l_AddToBookMarks.Index = 11;
            this.l_AddToBookMarks.Text = "Add to Bookmarks";
            this.l_AddToBookMarks.Visible = false;
            // 
            // c_Properties
            // 
            this.c_Properties.Index = 12;
            this.c_Properties.Text = "Properties";
            // 
            // DriveExplorer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.treeView1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.toolStrip1);
            this.Name = "DriveExplorer";
            this.Size = new System.Drawing.Size(999, 400);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        internal System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton b_Back;
        private System.Windows.Forms.ToolStripButton b_Forward;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripButton toolStripButton2;
        private System.Windows.Forms.ToolStripButton openToolStripButton;
        private System.Windows.Forms.ToolStripButton ts_Reload;
        private System.Windows.Forms.ToolStripButton ts_Close;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripLabel statusLabel;
        private System.Windows.Forms.ToolStripLabel quickMessage;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripLabel t_EntryCount;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ComboBox pathBar;
        private System.Windows.Forms.Button goButton;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.ContextMenu treeview_ContextMenu;
        private System.Windows.Forms.MenuItem t_Refresh;
        private System.Windows.Forms.MenuItem t_Extract;
        private System.Windows.Forms.MenuItem menuItem21;
        private System.Windows.Forms.MenuItem t_Rename;
        private System.Windows.Forms.MenuItem t_InjectFile;
        private System.Windows.Forms.MenuItem t_InjectFolder;
        private System.Windows.Forms.MenuItem t_NewFolder;
        private System.Windows.Forms.MenuItem menuItem34;
        private System.Windows.Forms.MenuItem tCached;
        private System.Windows.Forms.MenuItem tStatic;
        private System.Windows.Forms.MenuItem menuItem25;
        private System.Windows.Forms.MenuItem t_Delete;
        private System.Windows.Forms.MenuItem menuItem19;
        private System.Windows.Forms.MenuItem t_AddToBookmarks;
        private System.Windows.Forms.MenuItem t_CopyPath;
        private System.Windows.Forms.MenuItem t_Properties;
        private System.Windows.Forms.MenuItem menuItem18;
        private System.Windows.Forms.MenuItem t_driveProperties;
        private System.Windows.Forms.ContextMenu listview_ContextMenu;
        private System.Windows.Forms.MenuItem c_Refresh;
        private System.Windows.Forms.MenuItem c_Extract;
        private System.Windows.Forms.MenuItem menuItem31;
        private System.Windows.Forms.MenuItem c_Rename;
        private System.Windows.Forms.MenuItem c_InjectFile;
        private System.Windows.Forms.MenuItem c_InjectFolder;
        private System.Windows.Forms.MenuItem c_NewFolder;
        private System.Windows.Forms.MenuItem menuItem15;
        private System.Windows.Forms.MenuItem lCached;
        private System.Windows.Forms.MenuItem lStatic;
        private System.Windows.Forms.MenuItem menuItem24;
        private System.Windows.Forms.MenuItem c_Delete;
        private System.Windows.Forms.MenuItem menuItem11;
        private System.Windows.Forms.MenuItem l_AddToBookMarks;
        private System.Windows.Forms.MenuItem c_Properties;

    }
}
