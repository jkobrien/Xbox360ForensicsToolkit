using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Party_Buffalo.Forms
{
    public partial class AddLabel : Form
    {
        public AddLabel(string Path, string FolderName)
        {
            InitializeComponent();
            textBox1.Text = Path;
            textBox2.Text = FolderName;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (Party_Buffalo.Cache.CheckCache(textBox1.Text) != null)
            {
                for (int i = 0; i < Properties.Settings.Default.label.Count; i++)
                {
                    if (Properties.Settings.Default.labelPath[i].ToLower() == textBox1.Text.ToLower())
                    {
                        Properties.Settings.Default.label[i] = textBox2.Text;
                        Properties.Settings.Default.Save();
                        break;
                    }
                }
            }
            else
            {
                Party_Buffalo.Cache.AddLabel(textBox2.Text, textBox1.Text);
            }
        }

        public string Label
        {
            get
            {
                return textBox2.Text;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text == "" || textBox2.Text == "")
            {
                button1.Enabled = false;
            }
            else
            {
                button1.Enabled = true;
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text == "" || textBox2.Text == "")
            {
                button1.Enabled = false;
            }
            else
            {
                button1.Enabled = true;
            }
        }
    }
}
