namespace Party_Buffalo.Forms
{
    partial class InjectDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InjectDialog));
            this.l_Description = new System.Windows.Forms.Label();
            this.b_Option1 = new System.Windows.Forms.Button();
            this.b_Option2 = new System.Windows.Forms.Button();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // l_Description
            // 
            this.l_Description.Location = new System.Drawing.Point(13, 13);
            this.l_Description.Name = "l_Description";
            this.l_Description.Size = new System.Drawing.Size(355, 46);
            this.l_Description.TabIndex = 1;
            this.l_Description.Text = "The file/folder \"file/foldername\" already exists in the folder \" folderpath\"";
            // 
            // b_Option1
            // 
            this.b_Option1.DialogResult = System.Windows.Forms.DialogResult.Yes;
            this.b_Option1.Location = new System.Drawing.Point(211, 62);
            this.b_Option1.Name = "b_Option1";
            this.b_Option1.Size = new System.Drawing.Size(75, 23);
            this.b_Option1.TabIndex = 2;
            this.b_Option1.Text = "Merge";
            this.b_Option1.UseVisualStyleBackColor = true;
            this.b_Option1.Click += new System.EventHandler(this.b_Option1_Click);
            // 
            // b_Option2
            // 
            this.b_Option2.DialogResult = System.Windows.Forms.DialogResult.Ignore;
            this.b_Option2.Location = new System.Drawing.Point(292, 62);
            this.b_Option2.Name = "b_Option2";
            this.b_Option2.Size = new System.Drawing.Size(75, 23);
            this.b_Option2.TabIndex = 2;
            this.b_Option2.Text = "Skip";
            this.b_Option2.UseVisualStyleBackColor = true;
            this.b_Option2.Click += new System.EventHandler(this.b_Option2_Click);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.checkBox1.Location = new System.Drawing.Point(0, 80);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(380, 17);
            this.checkBox1.TabIndex = 0;
            this.checkBox1.Text = "Do this for all current items (x)";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // InjectDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(380, 97);
            this.Controls.Add(this.b_Option2);
            this.Controls.Add(this.b_Option1);
            this.Controls.Add(this.l_Description);
            this.Controls.Add(this.checkBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "InjectDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Entry Already Exists";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label l_Description;
        private System.Windows.Forms.Button b_Option1;
        private System.Windows.Forms.Button b_Option2;
        internal System.Windows.Forms.CheckBox checkBox1;
    }
}