using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Extensions;
using CLKsFATXLib;

namespace Party_Buffalo.Forms
{
    public partial class SearchForm : Form
    {
        System.Threading.Thread Worker;
        Folder[] Partitions;
        public string Path;
        ListViewColumnSorter lvwColumnSorter = new ListViewColumnSorter();
        volatile bool stop = false;
        public SearchForm(Folder[] Partitions)
        {
            InitializeComponent();
            listView1.SetExplorerTheme();
            this.Partitions = Partitions;
            listView1.ListViewItemSorter = lvwColumnSorter;
            listView1.DoubleClick +=new EventHandler(listView1_DoubleClick);
            this.FormClosing += new FormClosingEventHandler(SearchForm_FormClosing);
            listView1.ColumnClick +=new ColumnClickEventHandler(listView1_ColumnClick);
        }

        void SearchForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            stop = true;
            if (Worker != null)
            {
                Worker.Abort();
                while (Worker.IsAlive)
                {
                    System.Threading.Thread.Sleep(200);
                    // do nothing
                }
            }
        }

        void listView1_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            // Determine if clicked column is already the column that is being sorted.
            if (e.Column == lvwColumnSorter.SortColumn)
            {
                // Reverse the current sort direction for this column.
                if (lvwColumnSorter.Order == SortOrder.Ascending)
                {
                    lvwColumnSorter.Order = SortOrder.Descending;
                    ListViewExtensions.SetSortIcon(listView1, e.Column, SortOrder.Descending);
                }
                else
                {
                    lvwColumnSorter.Order = SortOrder.Ascending;
                    ListViewExtensions.SetSortIcon(listView1, e.Column, SortOrder.Ascending);
                }
            }
            else
            {
                // Set the column number that is to be sorted; default to ascending.
                lvwColumnSorter.SortColumn = e.Column;
                lvwColumnSorter.Order = SortOrder.Ascending;
                ListViewExtensions.SetSortIcon(listView1, e.Column, SortOrder.Ascending);
            }

            // Perform the sort with these new sort options.
            this.listView1.Sort();
        }

        void listView1_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                if (listView1.SelectedItems.Count == 1)
                {
                    Path = ((Entry)listView1.FocusedItem.Tag).FullPath;
                    if (!((Entry)listView1.FocusedItem.Tag).IsFolder)
                    {
                        Path = ((Entry)listView1.FocusedItem.Tag).Parent.FullPath;
                    }
                    stop = true;
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
            catch { }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            listView1.Items.Clear();
            button2.Enabled = true;
            bool Deleted = deletedEntries.Checked;
            bool STFS = stfsInfo.Checked;
            bool Files = false;
            bool Folders = false;
            if (both.Checked || folders.Checked)
            {
                Folders = true;
            }
            if (both.Checked || files.Checked)
            {
                Files = true;
            }
            System.Threading.ThreadStart ts = delegate
            {
                try
                {
                    string text = "";
                    textField.Invoke((MethodInvoker)delegate { text = textField.Text; });
                    this.Invoke((MethodInvoker)delegate { this.Text = "Searching for \"" + text + "\""; });
                    for (int i = 0; i < Partitions.Length; i++)
                    {
                        deletedEntries.Invoke((MethodInvoker)delegate { Partitions[i].ReturnDeletedEntries = deletedEntries.Checked; });
                        Search(Partitions[i], text, STFS, Deleted, Files, Folders);
                    }
                    this.Invoke((MethodInvoker)delegate { this.Text = "Search for \"" + text + "\" complete. " + listView1.Items.Count.ToString() + " items found."; });
                    System.Media.SystemSounds.Asterisk.Play();
                }
                catch { }
            };
            Worker = new System.Threading.Thread(ts);
            Worker.Start();
        }

        void Search(Folder f, string SearchTerm, bool STFS, bool Deleted, bool Files, bool Folders)
        {
            if (stop)
            {
                return;
            }
            this.Invoke((MethodInvoker)delegate { this.Text = "Searching for \"" + SearchTerm + "\"" + " in " + f.FullPath; });
            foreach (File file in f.Files())
            {
                if (stop)
                {
                    return;
                }
                if (Files)
                {
                    if (file.Name.ToLower().Contains(SearchTerm.ToLower()) || file.SizeFriendly.ToLower().Contains(SearchTerm.ToLower()))
                    {
                        AddFile(file);
                    }
                    else if (STFS && file.IsSTFSPackage())
                    {
                        if (file.TitleName().ToLower().Contains(SearchTerm.ToLower()) || file.TitleID().ToString("X").ToLower().Contains(SearchTerm.ToLower()) || file.ContentName().ToLower().Contains(SearchTerm.ToLower()) || file.DeviceID().ToHexString().ToLower().Contains(SearchTerm.ToLower()) || file.ProfileID().ToHexString().ToLower().Contains(SearchTerm.ToLower()) || file.ConsoleID().ToHexString().ToLower().Contains(SearchTerm.ToLower()))
                        {
                            AddFile(file);
                        }
                    }
                }
            }
            foreach (Folder ff in f.Folders())
            {
                if (stop)
                {
                    return;
                }
                if (Folders)
                {
                    if (ff.Name.ToLower().Contains(SearchTerm.ToLower()))
                    {
                        AddFolder(ff);
                    }
                    else if(STFS)
                    {
                        if (ff.GameName() != null)
                        {
                            if (ff.GameName().ToLower().Contains(SearchTerm.ToLower()))
                            {
                                AddFolder(ff);
                            }
                        }
                    }
                }
                ff.ReturnDeletedEntries = Deleted;
                Search(ff, SearchTerm, STFS, Deleted, Files, Folders);
            }
        }

        void AddFile(File f)
        {
            listView1.Invoke((MethodInvoker)delegate
            {
                ListViewItem li = new ListViewItem(f.Name);
                li.Tag = f;
                ListViewItem.ListViewSubItem l1 = new ListViewItem.ListViewSubItem(li, f.EntryType);
                ListViewItem.ListViewSubItem l2 = new ListViewItem.ListViewSubItem(li, f.SizeFriendly);
                ListViewItem.ListViewSubItem l3 = new ListViewItem.ListViewSubItem(li, f.ModifiedDate.ToString());
                li.SubItems.Add(l1);
                li.SubItems.Add(l2);
                li.SubItems.Add(l3);
                if (stfsInfo.Checked && f.IsSTFSPackage())
                {
                    ListViewItem.ListViewSubItem lsi = new ListViewItem.ListViewSubItem(li, f.ContentName());
                    li.SubItems.Add(lsi);
                }
                else
                {
                    ListViewItem.ListViewSubItem lsi = new ListViewItem.ListViewSubItem(li, "");
                    li.SubItems.Add(lsi);
                }
                ListViewItem.ListViewSubItem l4 = new ListViewItem.ListViewSubItem(li, f.FullPath);
                li.SubItems.Add(l4);
                listView1.Invoke((MethodInvoker)delegate { listView1.Items.Add(li); });
            });
        }

        void AddFolder(Folder Parent)
        {
            listView1.Invoke((MethodInvoker)delegate
            {
                ListViewItem li = new ListViewItem(Parent.Name);
                li.Tag = Parent;
                ListViewItem.ListViewSubItem l1 = new ListViewItem.ListViewSubItem(li, Parent.EntryType);
                ListViewItem.ListViewSubItem l2 = new ListViewItem.ListViewSubItem(li, "");
                ListViewItem.ListViewSubItem l3 = new ListViewItem.ListViewSubItem(li, Parent.ModifiedDate.ToString());
                li.SubItems.Add(l1);
                li.SubItems.Add(l2);
                li.SubItems.Add(l3);
                if (stfsInfo.Checked && Parent.IsTitleIDFolder)
                {
                    if (Parent.IsKnownFolder)
                    {
                        for (int i = 0; i < CLKsFATXLib.VariousFunctions.Known.Length; i++)
                        {
                            if (Parent.Name.ToLower() == CLKsFATXLib.VariousFunctions.Known[i].ToLower())
                            {
                                ListViewItem.ListViewSubItem lsi = new ListViewItem.ListViewSubItem(li, CLKsFATXLib.VariousFunctions.KnownEquivilent[i]);
                                li.SubItems.Add(lsi);
                                break;
                            }
                        }
                    }
                    else
                    {
                        ListViewItem.ListViewSubItem lsi = new ListViewItem.ListViewSubItem(li, Parent.GameName());
                        li.SubItems.Add(lsi);
                    }
                }
                else
                {
                    ListViewItem.ListViewSubItem lsi = new ListViewItem.ListViewSubItem(li, "");
                    li.SubItems.Add(lsi);
                }
                ListViewItem.ListViewSubItem l4 = new ListViewItem.ListViewSubItem(li, Parent.FullPath);
                li.SubItems.Add(l4);
                listView1.Invoke((MethodInvoker)delegate { listView1.Items.Add(li); });
            });
        }

        private void button2_Click(object sender, EventArgs e)
        {
            stop = true;
            button2.Enabled = false;
        }
    }
}
