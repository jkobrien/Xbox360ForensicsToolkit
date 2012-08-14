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
    public partial class BookmarkCreate : Form
    {
        public BookmarkCreate(string FolderPath, string Name)
        {
            InitializeComponent();
            t_Name.Text = Name;
            t_Path.Text = FolderPath;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Bookmarks b = new Bookmarks();
            if (b.CheckIfBookmarkExists(t_Name.Text, t_Path.Text))
            {
                MessageBox.Show("Bookmark already exists!");
            }
            else
            {
                b.CreateNewBookmark(t_Name.Text, t_Path.Text);
                this.DialogResult = DialogResult.OK;
                Close();
            }
        }

        public Bookmarks.BookmarkData New
        {
            get
            {
                Bookmarks.BookmarkData bd = new Bookmarks.BookmarkData();
                bd.Name = t_Name.Text;
                bd.Path = t_Path.Text;
                return bd;
            }
        }
    }
}
