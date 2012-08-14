namespace Party_Buffalo.Forms
{
    partial class Drive_Properties
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Drive_Properties));
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.l_Remaining = new System.Windows.Forms.Label();
            this.l_totalUsable = new System.Windows.Forms.Label();
            this.l_used = new System.Windows.Forms.Label();
            this.tits = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.dicks = new System.Windows.Forms.Label();
            this.l_drivePath = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.l_DiskName = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.progressBar1 = new Extensions.NyanCatBar();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(420, 310);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.progressBar1);
            this.tabPage1.Controls.Add(this.l_Remaining);
            this.tabPage1.Controls.Add(this.l_totalUsable);
            this.tabPage1.Controls.Add(this.l_used);
            this.tabPage1.Controls.Add(this.tits);
            this.tabPage1.Controls.Add(this.label4);
            this.tabPage1.Controls.Add(this.dicks);
            this.tabPage1.Controls.Add(this.l_drivePath);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Controls.Add(this.l_DiskName);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(412, 284);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Drive Information";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // l_Remaining
            // 
            this.l_Remaining.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.l_Remaining.AutoSize = true;
            this.l_Remaining.Location = new System.Drawing.Point(124, 102);
            this.l_Remaining.Name = "l_Remaining";
            this.l_Remaining.Size = new System.Drawing.Size(0, 13);
            this.l_Remaining.TabIndex = 6;
            // 
            // l_totalUsable
            // 
            this.l_totalUsable.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.l_totalUsable.AutoSize = true;
            this.l_totalUsable.Location = new System.Drawing.Point(124, 54);
            this.l_totalUsable.Name = "l_totalUsable";
            this.l_totalUsable.Size = new System.Drawing.Size(0, 13);
            this.l_totalUsable.TabIndex = 6;
            // 
            // l_used
            // 
            this.l_used.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.l_used.AutoSize = true;
            this.l_used.Location = new System.Drawing.Point(124, 78);
            this.l_used.Name = "l_used";
            this.l_used.Size = new System.Drawing.Size(0, 13);
            this.l_used.TabIndex = 6;
            // 
            // tits
            // 
            this.tits.AutoSize = true;
            this.tits.Location = new System.Drawing.Point(8, 102);
            this.tits.Name = "tits";
            this.tits.Size = new System.Drawing.Size(60, 13);
            this.tits.TabIndex = 4;
            this.tits.Text = "Remaining:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(8, 54);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(110, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "Total Usable Storage:";
            // 
            // dicks
            // 
            this.dicks.AutoSize = true;
            this.dicks.Location = new System.Drawing.Point(8, 78);
            this.dicks.Name = "dicks";
            this.dicks.Size = new System.Drawing.Size(35, 13);
            this.dicks.TabIndex = 4;
            this.dicks.Text = "Used:";
            // 
            // l_drivePath
            // 
            this.l_drivePath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.l_drivePath.AutoSize = true;
            this.l_drivePath.Location = new System.Drawing.Point(126, 25);
            this.l_drivePath.Name = "l_drivePath";
            this.l_drivePath.Size = new System.Drawing.Size(89, 13);
            this.l_drivePath.TabIndex = 3;
            this.l_drivePath.Text = "\\\\.\\PhysicalDrive";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 25);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(60, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Drive Path:";
            // 
            // l_DiskName
            // 
            this.l_DiskName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.l_DiskName.AutoSize = true;
            this.l_DiskName.Location = new System.Drawing.Point(126, 3);
            this.l_DiskName.Name = "l_DiskName";
            this.l_DiskName.Size = new System.Drawing.Size(35, 13);
            this.l_DiskName.TabIndex = 1;
            this.l_DiskName.Text = "label2";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(62, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Disk Name:";
            // 
            // progressBar1
            // 
            this.progressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar1.Location = new System.Drawing.Point(10, 118);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(396, 23);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressBar1.TabIndex = 8;
            // 
            // Drive_Properties
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(420, 310);
            this.Controls.Add(this.tabControl1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Drive_Properties";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Drive Information";
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Label l_DiskName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label tits;
        private System.Windows.Forms.Label dicks;
        private System.Windows.Forms.Label l_drivePath;
        private System.Windows.Forms.Label l_Remaining;
        private System.Windows.Forms.Label l_used;
        private System.Windows.Forms.Label l_totalUsable;
        private System.Windows.Forms.Label label4;
        private Extensions.NyanCatBar progressBar1;
    }
}