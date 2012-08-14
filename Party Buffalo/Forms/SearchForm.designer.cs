namespace Party_Buffalo.Forms
{
    partial class SearchForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SearchForm));
            this.listView1 = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.matchCase = new System.Windows.Forms.CheckBox();
            this.stfsInfo = new System.Windows.Forms.CheckBox();
            this.textField = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.both = new System.Windows.Forms.RadioButton();
            this.files = new System.Windows.Forms.RadioButton();
            this.folders = new System.Windows.Forms.RadioButton();
            this.deletedEntries = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.button2 = new System.Windows.Forms.Button();
            this.columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.groupBox1.SuspendLayout();
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
            this.columnHeader5,
            this.columnHeader6,
            this.columnHeader7});
            this.listView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView1.FullRowSelect = true;
            this.listView1.GridLines = true;
            this.listView1.Location = new System.Drawing.Point(0, 0);
            this.listView1.MultiSelect = false;
            this.listView1.Name = "listView1";
            this.listView1.ShowItemToolTips = true;
            this.listView1.Size = new System.Drawing.Size(827, 317);
            this.listView1.TabIndex = 9;
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
            // columnHeader6
            // 
            this.columnHeader6.Text = "Path";
            this.columnHeader6.Width = 150;
            // 
            // matchCase
            // 
            this.matchCase.AutoSize = true;
            this.matchCase.Location = new System.Drawing.Point(312, 21);
            this.matchCase.Name = "matchCase";
            this.matchCase.Size = new System.Drawing.Size(83, 17);
            this.matchCase.TabIndex = 3;
            this.matchCase.Text = "Match Case";
            this.matchCase.UseVisualStyleBackColor = true;
            // 
            // stfsInfo
            // 
            this.stfsInfo.AutoSize = true;
            this.stfsInfo.Checked = true;
            this.stfsInfo.CheckState = System.Windows.Forms.CheckState.Checked;
            this.stfsInfo.Location = new System.Drawing.Point(401, 22);
            this.stfsInfo.Name = "stfsInfo";
            this.stfsInfo.Size = new System.Drawing.Size(145, 17);
            this.stfsInfo.TabIndex = 4;
            this.stfsInfo.Text = "Search STFS Information";
            this.stfsInfo.UseVisualStyleBackColor = true;
            // 
            // textField
            // 
            this.textField.Location = new System.Drawing.Point(6, 19);
            this.textField.Name = "textField";
            this.textField.Size = new System.Drawing.Size(195, 20);
            this.textField.TabIndex = 0;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(207, 17);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(45, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "Go";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // both
            // 
            this.both.AutoSize = true;
            this.both.Checked = true;
            this.both.Location = new System.Drawing.Point(657, 21);
            this.both.Name = "both";
            this.both.Size = new System.Drawing.Size(47, 17);
            this.both.TabIndex = 6;
            this.both.TabStop = true;
            this.both.Text = "Both";
            this.both.UseVisualStyleBackColor = true;
            // 
            // files
            // 
            this.files.AutoSize = true;
            this.files.Location = new System.Drawing.Point(710, 20);
            this.files.Name = "files";
            this.files.Size = new System.Drawing.Size(46, 17);
            this.files.TabIndex = 7;
            this.files.Text = "Files";
            this.files.UseVisualStyleBackColor = true;
            // 
            // folders
            // 
            this.folders.AutoSize = true;
            this.folders.Location = new System.Drawing.Point(762, 21);
            this.folders.Name = "folders";
            this.folders.Size = new System.Drawing.Size(59, 17);
            this.folders.TabIndex = 8;
            this.folders.Text = "Folders";
            this.folders.UseVisualStyleBackColor = true;
            // 
            // deletedEntries
            // 
            this.deletedEntries.AutoSize = true;
            this.deletedEntries.Location = new System.Drawing.Point(553, 21);
            this.deletedEntries.Name = "deletedEntries";
            this.deletedEntries.Size = new System.Drawing.Size(98, 17);
            this.deletedEntries.TabIndex = 5;
            this.deletedEntries.Text = "Deleted Entries";
            this.deletedEntries.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.button2);
            this.groupBox1.Controls.Add(this.textField);
            this.groupBox1.Controls.Add(this.deletedEntries);
            this.groupBox1.Controls.Add(this.matchCase);
            this.groupBox1.Controls.Add(this.folders);
            this.groupBox1.Controls.Add(this.stfsInfo);
            this.groupBox1.Controls.Add(this.files);
            this.groupBox1.Controls.Add(this.button1);
            this.groupBox1.Controls.Add(this.both);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.groupBox1.Location = new System.Drawing.Point(0, 317);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(827, 46);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Search Terms";
            // 
            // button2
            // 
            this.button2.Enabled = false;
            this.button2.Location = new System.Drawing.Point(258, 17);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(48, 23);
            this.button2.TabIndex = 2;
            this.button2.Text = "Stop";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // columnHeader7
            // 
            this.columnHeader7.Text = "CRC";
            // 
            // SearchForm
            // 
            this.AcceptButton = this.button1;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(827, 363);
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.groupBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(843, 125);
            this.Name = "SearchForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Search";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.CheckBox matchCase;
        private System.Windows.Forms.CheckBox stfsInfo;
        private System.Windows.Forms.TextBox textField;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.RadioButton both;
        private System.Windows.Forms.RadioButton files;
        private System.Windows.Forms.RadioButton folders;
        private System.Windows.Forms.CheckBox deletedEntries;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.ColumnHeader columnHeader7;
    }
}