namespace Party_Buffalo.Forms
{
    partial class ForensicReport
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.deletedEntries = new System.Windows.Forms.CheckBox();
            this.folders = new System.Windows.Forms.RadioButton();
            this.CRCInfo = new System.Windows.Forms.CheckBox();
            this.files = new System.Windows.Forms.RadioButton();
            this.button1 = new System.Windows.Forms.Button();
            this.both = new System.Windows.Forms.RadioButton();
            this.listView1 = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader8 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader9 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader11 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader10 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.listview_ContextMenu = new System.Windows.Forms.ContextMenu();
            this.c_Extract = new System.Windows.Forms.MenuItem();
            this.menuItem10 = new System.Windows.Forms.MenuItem();
            this.menuItem43 = new System.Windows.Forms.MenuItem();
            this.c_Properties = new System.Windows.Forms.MenuItem();
            this.m_Sort = new System.Windows.Forms.MenuItem();
            this.sort_Name = new System.Windows.Forms.MenuItem();
            this.sort_Type = new System.Windows.Forms.MenuItem();
            this.sort_Size = new System.Windows.Forms.MenuItem();
            this.sort_Date = new System.Windows.Forms.MenuItem();
            this.sort_STFS = new System.Windows.Forms.MenuItem();
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.IsSplitterFixed = true;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.groupBox1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.listView1);
            this.splitContainer1.Size = new System.Drawing.Size(834, 362);
            this.splitContainer1.SplitterDistance = 47;
            this.splitContainer1.TabIndex = 12;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.button2);
            this.groupBox1.Controls.Add(this.deletedEntries);
            this.groupBox1.Controls.Add(this.folders);
            this.groupBox1.Controls.Add(this.CRCInfo);
            this.groupBox1.Controls.Add(this.files);
            this.groupBox1.Controls.Add(this.button1);
            this.groupBox1.Controls.Add(this.both);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(834, 49);
            this.groupBox1.TabIndex = 12;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Forensic Reporting Details";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(675, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(0, 13);
            this.label1.TabIndex = 9;
            // 
            // button2
            // 
            this.button2.Enabled = false;
            this.button2.Location = new System.Drawing.Point(133, 18);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(130, 23);
            this.button2.TabIndex = 2;
            this.button2.Text = "Export Report";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // deletedEntries
            // 
            this.deletedEntries.AutoSize = true;
            this.deletedEntries.Location = new System.Drawing.Point(366, 21);
            this.deletedEntries.Name = "deletedEntries";
            this.deletedEntries.Size = new System.Drawing.Size(136, 17);
            this.deletedEntries.TabIndex = 5;
            this.deletedEntries.Text = "Include Deleted Entries";
            this.deletedEntries.UseVisualStyleBackColor = true;
            // 
            // folders
            // 
            this.folders.AutoSize = true;
            this.folders.Location = new System.Drawing.Point(559, 21);
            this.folders.Name = "folders";
            this.folders.Size = new System.Drawing.Size(59, 17);
            this.folders.TabIndex = 8;
            this.folders.Text = "Folders";
            this.folders.UseVisualStyleBackColor = true;
            // 
            // CRCInfo
            // 
            this.CRCInfo.AutoSize = true;
            this.CRCInfo.Location = new System.Drawing.Point(269, 22);
            this.CRCInfo.Name = "CRCInfo";
            this.CRCInfo.Size = new System.Drawing.Size(86, 17);
            this.CRCInfo.TabIndex = 4;
            this.CRCInfo.Text = "Include CRC";
            this.CRCInfo.UseVisualStyleBackColor = true;
            // 
            // files
            // 
            this.files.AutoSize = true;
            this.files.Location = new System.Drawing.Point(507, 20);
            this.files.Name = "files";
            this.files.Size = new System.Drawing.Size(46, 17);
            this.files.TabIndex = 7;
            this.files.Text = "Files";
            this.files.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 17);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(115, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "Run Report";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // both
            // 
            this.both.AutoSize = true;
            this.both.Checked = true;
            this.both.Location = new System.Drawing.Point(622, 21);
            this.both.Name = "both";
            this.both.Size = new System.Drawing.Size(47, 17);
            this.both.TabIndex = 6;
            this.both.TabStop = true;
            this.both.Text = "Both";
            this.both.UseVisualStyleBackColor = true;
            // 
            // listView1
            // 
            this.listView1.AllowDrop = true;
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader8,
            this.columnHeader9,
            this.columnHeader5,
            this.columnHeader11,
            this.columnHeader6,
            this.columnHeader7,
            this.columnHeader10});
            this.listView1.ContextMenu = this.listview_ContextMenu;
            this.listView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView1.FullRowSelect = true;
            this.listView1.GridLines = true;
            this.listView1.Location = new System.Drawing.Point(0, 0);
            this.listView1.MultiSelect = false;
            this.listView1.Name = "listView1";
            this.listView1.ShowItemToolTips = true;
            this.listView1.Size = new System.Drawing.Size(834, 311);
            this.listView1.TabIndex = 11;
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
            this.columnHeader4.Text = "Created Date";
            this.columnHeader4.Width = 160;
            // 
            // columnHeader8
            // 
            this.columnHeader8.Text = "Accessed Date";
            this.columnHeader8.Width = 160;
            // 
            // columnHeader9
            // 
            this.columnHeader9.Text = "Modified Date";
            this.columnHeader9.Width = 160;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "STFS Name";
            this.columnHeader5.Width = 160;
            // 
            // columnHeader11
            // 
            this.columnHeader11.Text = "STFS Entry Type";
            this.columnHeader11.Width = 100;
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "Path";
            this.columnHeader6.Width = 150;
            // 
            // columnHeader7
            // 
            this.columnHeader7.Text = "Deleted";
            // 
            // columnHeader10
            // 
            this.columnHeader10.Text = "MD5";
            this.columnHeader10.Width = 160;
            // 
            // listview_ContextMenu
            // 
            this.listview_ContextMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.c_Extract,
            this.menuItem10,
            this.menuItem43,
            this.c_Properties,
            this.menuItem1,
            this.m_Sort});
            // 
            // c_Extract
            // 
            this.c_Extract.Index = 0;
            this.c_Extract.Text = "Extract";
            this.c_Extract.Click += new System.EventHandler(this.c_Extract_Click);
            // 
            // menuItem10
            // 
            this.menuItem10.Index = 1;
            this.menuItem10.Text = "File Details";
            this.menuItem10.Click += new System.EventHandler(this.menuItem10_Click);
            // 
            // menuItem43
            // 
            this.menuItem43.Index = 2;
            this.menuItem43.Text = "Hex View";
            this.menuItem43.Click += new System.EventHandler(this.menuItem43_Click);
            // 
            // c_Properties
            // 
            this.c_Properties.Index = 3;
            this.c_Properties.Text = "Properties";
            this.c_Properties.Click += new System.EventHandler(this.c_Properties_Click);
            // 
            // m_Sort
            // 
            this.m_Sort.Index = 5;
            this.m_Sort.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.sort_Name,
            this.sort_Type,
            this.sort_Size,
            this.sort_Date,
            this.sort_STFS});
            this.m_Sort.Text = "Sort";
            // 
            // sort_Name
            // 
            this.sort_Name.Index = 0;
            this.sort_Name.RadioCheck = true;
            this.sort_Name.Text = "Name";
            // 
            // sort_Type
            // 
            this.sort_Type.Index = 1;
            this.sort_Type.RadioCheck = true;
            this.sort_Type.Text = "Type";
            // 
            // sort_Size
            // 
            this.sort_Size.Index = 2;
            this.sort_Size.RadioCheck = true;
            this.sort_Size.Text = "Size";
            // 
            // sort_Date
            // 
            this.sort_Date.Index = 3;
            this.sort_Date.RadioCheck = true;
            this.sort_Date.Text = "Date";
            // 
            // sort_STFS
            // 
            this.sort_STFS.Index = 4;
            this.sort_STFS.RadioCheck = true;
            this.sort_STFS.Text = "STFS Name";
            // 
            // menuItem1
            // 
            this.menuItem1.Index = 4;
            this.menuItem1.Text = "Disassemble";
            this.menuItem1.Click += new System.EventHandler(this.menuItem1_Click);
            // 
            // ForensicReport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(834, 362);
            this.Controls.Add(this.splitContainer1);
            this.MinimumSize = new System.Drawing.Size(850, 150);
            this.Name = "ForensicReport";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Forensic Report";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.CheckBox deletedEntries;
        private System.Windows.Forms.RadioButton folders;
        private System.Windows.Forms.CheckBox CRCInfo;
        private System.Windows.Forms.RadioButton files;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.RadioButton both;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.ColumnHeader columnHeader7;
        private System.Windows.Forms.ColumnHeader columnHeader8;
        private System.Windows.Forms.ColumnHeader columnHeader9;
        private System.Windows.Forms.ColumnHeader columnHeader10;
        private System.Windows.Forms.ColumnHeader columnHeader11;
        private System.Windows.Forms.MenuItem c_Extract;
        private System.Windows.Forms.MenuItem menuItem10;
        private System.Windows.Forms.MenuItem menuItem43;
        private System.Windows.Forms.MenuItem c_Properties;
        private System.Windows.Forms.MenuItem m_Sort;
        private System.Windows.Forms.MenuItem sort_Name;
        private System.Windows.Forms.MenuItem sort_Type;
        private System.Windows.Forms.MenuItem sort_Size;
        private System.Windows.Forms.MenuItem sort_Date;
        private System.Windows.Forms.MenuItem sort_STFS;
        public System.Windows.Forms.ContextMenu listview_ContextMenu;
        private System.Windows.Forms.MenuItem menuItem1;

    }
}