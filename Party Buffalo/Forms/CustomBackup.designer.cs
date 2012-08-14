namespace Party_Buffalo.Forms
{
    partial class CustomBackup
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CustomBackup));
            this.label1 = new System.Windows.Forms.Label();
            this.g_LIVE = new System.Windows.Forms.GroupBox();
            this.listView1 = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.b_begin = new System.Windows.Forms.Button();
            this.g_LIVE.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(273, 57);
            this.label1.TabIndex = 0;
            this.label1.Text = "Doing a custom backup allows you to choose the content you want to backup to your" +
                " PC.  Double-click items you wish to remove (double-click again to re-add), then" +
                " press next to begin the backup";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // g_LIVE
            // 
            this.g_LIVE.Controls.Add(this.listView1);
            this.g_LIVE.Dock = System.Windows.Forms.DockStyle.Fill;
            this.g_LIVE.Location = new System.Drawing.Point(0, 57);
            this.g_LIVE.Name = "g_LIVE";
            this.g_LIVE.Size = new System.Drawing.Size(273, 232);
            this.g_LIVE.TabIndex = 1;
            this.g_LIVE.TabStop = false;
            // 
            // listView1
            // 
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.listView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView1.FullRowSelect = true;
            this.listView1.GridLines = true;
            this.listView1.Location = new System.Drawing.Point(3, 16);
            this.listView1.Name = "listView1";
            this.listView1.ShowItemToolTips = true;
            this.listView1.Size = new System.Drawing.Size(267, 213);
            this.listView1.TabIndex = 0;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Name";
            this.columnHeader1.Width = 220;
            // 
            // b_begin
            // 
            this.b_begin.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.b_begin.Location = new System.Drawing.Point(0, 289);
            this.b_begin.Name = "b_begin";
            this.b_begin.Size = new System.Drawing.Size(273, 45);
            this.b_begin.TabIndex = 3;
            this.b_begin.Text = "Next";
            this.b_begin.UseVisualStyleBackColor = true;
            this.b_begin.Click += new System.EventHandler(this.b_begin_Click);
            // 
            // CustomBackup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(273, 334);
            this.Controls.Add(this.g_LIVE);
            this.Controls.Add(this.b_begin);
            this.Controls.Add(this.label1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(289, 372);
            this.Name = "CustomBackup";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Custom Backup";
            this.g_LIVE.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox g_LIVE;
        private System.Windows.Forms.Button b_begin;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader columnHeader1;
    }
}