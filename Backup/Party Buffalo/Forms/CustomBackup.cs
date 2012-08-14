using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Extensions;

namespace Party_Buffalo.Forms
{
    public partial class CustomBackup : Form
    {
        public CustomBackup()
        {
            InitializeComponent();
            listView1.SetExplorerTheme();
            listView1.DoubleClick += new EventHandler(listView1_DoubleClick);
            for (int i = 0; i < CLKsFATXLib.VariousFunctions.Known.Length; i++)
            {
                ListViewItem item = new ListViewItem(CLKsFATXLib.VariousFunctions.KnownEquivilent[i] + " (" + CLKsFATXLib.VariousFunctions.Known[i] + ")");
                item.UseItemStyleForSubItems = true;
                item.BackColor = Color.White;
                item.Tag = CLKsFATXLib.VariousFunctions.Known[i];
                listView1.Items.Add(item);
            }
        }

        void listView1_DoubleClick(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 1)
            {
                if (listView1.FocusedItem.BackColor == Color.White)
                {
                    listView1.FocusedItem.BackColor = Color.Red;
                }
                else
                {
                    listView1.FocusedItem.BackColor = Color.White;
                }
            }
        }

        private void b_begin_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            List<string> ToSkip = new List<string>();
            foreach (ListViewItem i in listView1.Items)
            {
                if (i.BackColor == Color.Red)
                {
                    ToSkip.Add((string)i.Tag);
                }
            }
            FoldersToSkip = ToSkip.ToArray();
            this.Close();
        }

        public string[] FoldersToSkip
        {
            get;
            set;
        }
    }
}
