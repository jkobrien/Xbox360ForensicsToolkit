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
using System.Security.Cryptography;

namespace Party_Buffalo.Forms
{
    public partial class ForensicReport : Form
    {
        System.Threading.Thread Worker;
        Folder[] Partitions;
        Drive xDrive;
        public string Path;
        ListViewColumnSorter lvwColumnSorter = new ListViewColumnSorter();
        volatile bool stop = false;
        public ForensicReport(Folder[] Partitions,Drive xDrive)
        {
            InitializeComponent();
            listView1.SetExplorerTheme();
            this.Partitions = Partitions;
            this.xDrive = xDrive;
            listView1.ListViewItemSorter = lvwColumnSorter;
            listView1.DoubleClick += new EventHandler(listView1_DoubleClick);
            this.FormClosing += new FormClosingEventHandler(ForensicReport_FormClosing);
            listView1.ColumnClick += new ColumnClickEventHandler(listView1_ColumnClick);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            listView1.Items.Clear();
            bool Deleted = deletedEntries.Checked;
            bool CRC = CRCInfo.Checked;
            bool Files = false;
            bool Folders = false;
            DateTime myStartTime = DateTime.UtcNow;
            DateTime myRunTime = DateTime.UtcNow;
            TimeSpan span = myRunTime.Subtract(myStartTime);
            this.button2.Enabled = true;
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
                    label1.Invoke((MethodInvoker)delegate { text = label1.Text; });
                    for (int i = 0; i < Partitions.Length; i++)
                    {
                        deletedEntries.Invoke((MethodInvoker)delegate { Partitions[i].ReturnDeletedEntries = deletedEntries.Checked; });
                        Search(Partitions[i], CRC, Deleted, Files, Folders, myStartTime);
                    }
                    myRunTime = DateTime.UtcNow;
                    span = myRunTime.Subtract(myStartTime);
                    this.Invoke((MethodInvoker)delegate { label1.Text = "Running Report - " + span.Minutes.ToString() + " Min " + span.Seconds.ToString() + " sec"; });
                    this.Invoke((MethodInvoker)delegate { label1.Text = "Report Complete" + span.Minutes.ToString() + " Min " + span.Seconds.ToString() + " sec"; ; });
                    this.Invoke((MethodInvoker)delegate { this.Text = "Report Complete. " + listView1.Items.Count.ToString() + " items found."; });

                }
                catch { }
            };
            Worker = new System.Threading.Thread(ts);
            Worker.Start();
        }

        void ForensicReport_FormClosing(object sender, FormClosingEventArgs e)
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

        void Search(Folder f, bool CRC, bool Deleted, bool Files, bool Folders, DateTime myStartTime)
        {
            DateTime myRunTime = DateTime.UtcNow;
            TimeSpan span = myRunTime.Subtract(myStartTime);
            if (stop)
            {
                return;
            }
            myRunTime = DateTime.UtcNow;
            span = myRunTime.Subtract(myStartTime);
            this.Invoke((MethodInvoker)delegate { this.Text = "Report Running in " + f.FullPath; });
            foreach (File file in f.Files())
            {
                myRunTime = DateTime.UtcNow;
                span = myRunTime.Subtract(myStartTime);
                this.Invoke((MethodInvoker)delegate { label1.Text = "Running Report - " + span.Minutes.ToString() + " Min " + span.Seconds.ToString() + " sec"; });
                if (stop)
                {
                    return;
                }
                if (Files)
                {
                  AddFile(file,CRC);
                }
            }
            foreach (Folder ff in f.Folders())
            {
                myRunTime = DateTime.UtcNow;
                span = myRunTime.Subtract(myStartTime);
                this.Invoke((MethodInvoker)delegate { label1.Text = "Running Report - " + span.Minutes.ToString() + " Min " + span.Seconds.ToString() + " sec"; });
                if (stop)
                {
                    return;
                }
                if (Folders)
                {
                    AddFolder(ff);
                }
                ff.ReturnDeletedEntries = Deleted;
                Search(ff, CRC, Deleted, Files, Folders, myStartTime);
            }
        }

        void AddFile(File f,bool FileCRC)
        {
            listView1.Invoke((MethodInvoker)delegate
            {
                ListViewItem li = new ListViewItem(f.Name);
                li.Tag = f;
                ListViewItem.ListViewSubItem l1 = new ListViewItem.ListViewSubItem(li, f.EntryType);
                ListViewItem.ListViewSubItem l2 = new ListViewItem.ListViewSubItem(li, f.SizeFriendly);
                ListViewItem.ListViewSubItem l3 = new ListViewItem.ListViewSubItem(li, f.CreationDate.ToString());
                ListViewItem.ListViewSubItem l6 = new ListViewItem.ListViewSubItem(li, f.ModifiedDate.ToString());
                ListViewItem.ListViewSubItem l7 = new ListViewItem.ListViewSubItem(li, f.AccessedDate.ToString());
                li.SubItems.Add(l1);
                li.SubItems.Add(l2);
                li.SubItems.Add(l3);
                li.SubItems.Add(l6);
                li.SubItems.Add(l7);
                if (f.IsSTFSPackage())
                {
                    ListViewItem.ListViewSubItem lsi = new ListViewItem.ListViewSubItem(li, f.ContentName());
                    li.SubItems.Add(lsi);
                    ListViewItem.ListViewSubItem lsi2 = new ListViewItem.ListViewSubItem(li, f.EntryType);
                    li.SubItems.Add(lsi2);
                }
                else
                {
                    ListViewItem.ListViewSubItem lsi = new ListViewItem.ListViewSubItem(li, "");
                    li.SubItems.Add(lsi);
                    ListViewItem.ListViewSubItem lsi2 = new ListViewItem.ListViewSubItem(li, "");
                    li.SubItems.Add(lsi2);
                }
                ListViewItem.ListViewSubItem l4 = new ListViewItem.ListViewSubItem(li, f.FullPath);
                li.SubItems.Add(l4);
                
                if (f.IsDeleted)
                {
                    li.BackColor = Color.Red;
                    ListViewItem.ListViewSubItem l5 = new ListViewItem.ListViewSubItem(li, "Deleted File");
                    li.SubItems.Add(l5);
                }
                else
                {
                    ListViewItem.ListViewSubItem l5 = new ListViewItem.ListViewSubItem(li, "");
                    li.SubItems.Add(l5);
                }
                if (FileCRC)
                {

                    string myFileCRC = GetMD5(f);
                    ListViewItem.ListViewSubItem l8 = new ListViewItem.ListViewSubItem(li, myFileCRC);
                    li.SubItems.Add(l8);
                }
                else
                {
                    ListViewItem.ListViewSubItem l8 = new ListViewItem.ListViewSubItem(li, "No CRC");
                    li.SubItems.Add(l8);
                }
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
                ListViewItem.ListViewSubItem l3 = new ListViewItem.ListViewSubItem(li, Parent.CreationDate.ToString());
                ListViewItem.ListViewSubItem l6 = new ListViewItem.ListViewSubItem(li, Parent.ModifiedDate.ToString());
                ListViewItem.ListViewSubItem l7 = new ListViewItem.ListViewSubItem(li, Parent.AccessedDate.ToString());
                li.SubItems.Add(l1);
                li.SubItems.Add(l2);
                li.SubItems.Add(l3);
                li.SubItems.Add(l6);
                li.SubItems.Add(l7);
                if (Parent.IsTitleIDFolder)
                {
                    if (Parent.IsKnownFolder)
                    {
                        for (int i = 0; i < CLKsFATXLib.VariousFunctions.Known.Length; i++)
                        {
                            if (Parent.Name.ToLower() == CLKsFATXLib.VariousFunctions.Known[i].ToLower())
                            {
                                ListViewItem.ListViewSubItem lsi = new ListViewItem.ListViewSubItem(li, CLKsFATXLib.VariousFunctions.KnownEquivilent[i]);
                                li.SubItems.Add(lsi);
                                ListViewItem.ListViewSubItem lsi2 = new ListViewItem.ListViewSubItem(li, "");
                                li.SubItems.Add(lsi2);
                                break;
                            }
                        }
                    }
                    else
                    {
                        ListViewItem.ListViewSubItem lsi = new ListViewItem.ListViewSubItem(li, Parent.GameName());
                        li.SubItems.Add(lsi);
                        ListViewItem.ListViewSubItem lsi2 = new ListViewItem.ListViewSubItem(li, "");
                        li.SubItems.Add(lsi2);
                    }
                }
                else
                {
                    ListViewItem.ListViewSubItem lsi = new ListViewItem.ListViewSubItem(li, "");
                    li.SubItems.Add(lsi);
                    ListViewItem.ListViewSubItem lsi2 = new ListViewItem.ListViewSubItem(li, "");
                    li.SubItems.Add(lsi2);
                }
                ListViewItem.ListViewSubItem l4 = new ListViewItem.ListViewSubItem(li, Parent.FullPath);
                li.SubItems.Add(l4);
                if (Parent.IsDeleted)
                {
                    li.BackColor = Color.Red;
                    ListViewItem.ListViewSubItem l5 = new ListViewItem.ListViewSubItem(li, "Deleted Folder");
                    li.SubItems.Add(l5);
                }
                else
                {
                    ListViewItem.ListViewSubItem l5 = new ListViewItem.ListViewSubItem(li, "");
                    li.SubItems.Add(l5);
                }
                listView1.Invoke((MethodInvoker)delegate { listView1.Items.Add(li); });
            });
        }

        /**
        *   Title:          Xbox 360 Forensic Toolkit.
        *   Description:    Xbox 360 Forensic Toolkit is built on top of the Party buffalo application. It allows users to read and 
        *                   write to Xbox 360 devices, with support for reading Xbox 360 STFS package names. A number of Forensic Tools have
        *                   been added to extract/view sectors, extract partitions, view file details, examine files in HEX. It also incorporates the wxPIRS
        *                   Application so that we can sissamble STFS packages and see their contents.
        *
        *   Original Author: Party Buffalo - landergriffith@gmail.com, wxPIRS - gael360
        *   Another Author: obriej62@mail.dcu.ie
        *   License:        http://www.gnu.org/licenses/gpl.html GNU GENERAL PUBLIC LICENSE
        *   Link:           https://code.google.com/p/party-buffalo/
        *                  http://gael360.free.fr/
        *   Note:           All Code that follows to the End point was added by obriej62@mail.dcu.ie
        */
        private void button2_Click(object sender, EventArgs e)
        {
            
            StringBuilder myReportFile = new StringBuilder();
            label1.Text = "Generating Report....."; 

            Forms.EntryAction ea = new Party_Buffalo.Forms.EntryAction(xDrive);
            ea.ViewDriveInfo();
            myReportFile.Append(ea.myDriveInfo.ToString());
            ea.ViewJoshSector();
            myReportFile.Append(ea.JoshSector.ToString());
            ea.ViewSecuritySector();
            myReportFile.Append(ea.SecuritySector.ToString());

            label1.Text = "Report Generated" + listView1.Items.Count.ToString() + " items found.";
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.FileName = "Xbox360 Forensic Report " + DateTime.Now.ToString().Replace('/', '-').Replace(':', '_');
            saveFileDialog.DefaultExt = ".csv";
            saveFileDialog.Filter = "CSV (*.csv)|*.csv";
            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                System.IO.StreamWriter sw = new System.IO.StreamWriter(saveFileDialog.FileName);
                sw.WriteLine(myReportFile.ToString());
                WriteToCSV(saveFileDialog.FileName.ToString(), listView1, sw);
            }
            label1.Text = "Report Exported Successfully.";
        }
        /**
        *   Title:          Xbox 360 Forensic Toolkit.
        *   Description:    Xbox 360 Forensic Toolkit is built on top of the Party buffalo application. It allows users to read and 
        *                   write to Xbox 360 devices, with support for reading Xbox 360 STFS package names. A number of Forensic Tools have
        *                   been added to extract/view sectors, extract partitions, view file details, examine files in HEX. It also incorporates the wxPIRS
        *                   Application so that we can sissamble STFS packages and see their contents.
        *
        *   Original Author: Party Buffalo - landergriffith@gmail.com, wxPIRS - gael360
        *   Another Author: obriej62@mail.dcu.ie
        *   License:        http://www.gnu.org/licenses/gpl.html GNU GENERAL PUBLIC LICENSE
        *   Link:           https://code.google.com/p/party-buffalo/
        *                  http://gael360.free.fr/
        *   Note:           All Code that follows to the End point was added by obriej62@mail.dcu.ie
        */
        string GetMD5(File f)
        {
            StringBuilder MD5Hash = new StringBuilder("");
            CLKsFATXLib.Streams.Reader io2 = new CLKsFATXLib.Streams.Reader(f.GetStream());
            byte[] bytes = io2.ReadBytes((int)f.Size);
            MD5 myMD5 = MD5.Create();
            byte[] hashedBytes = myMD5.ComputeHash(bytes);
            for (int i = 0; i < hashedBytes.Length; i++)
            {
                MD5Hash.Append(hashedBytes[i].ToString("x2"));
            }

            return MD5Hash.ToString();
        }
        /**
        *   Title:          Xbox 360 Forensic Toolkit.
        *   Description:    Xbox 360 Forensic Toolkit is built on top of the Party buffalo application. It allows users to read and 
        *                   write to Xbox 360 devices, with support for reading Xbox 360 STFS package names. A number of Forensic Tools have
        *                   been added to extract/view sectors, extract partitions, view file details, examine files in HEX. It also incorporates the wxPIRS
        *                   Application so that we can sissamble STFS packages and see their contents.
        *
        *   Original Author: Party Buffalo - landergriffith@gmail.com, wxPIRS - gael360
        *   Another Author: obriej62@mail.dcu.ie
        *   License:        http://www.gnu.org/licenses/gpl.html GNU GENERAL PUBLIC LICENSE
        *   Link:           https://code.google.com/p/party-buffalo/
        *                  http://gael360.free.fr/
        *   Note:           All Code that follows to the End point was added by obriej62@mail.dcu.ie
        */
        private void fileDetailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (((Entry)listView1.FocusedItem.Tag).IsFolder)
            {
                Forms.FileDetailsForm myFileDetailsForm = new Forms.FileDetailsForm("This is not a Valid File!");
                myFileDetailsForm.Show();
            }
            else
            {
                Forms.EntryAction ea = new Party_Buffalo.Forms.EntryAction((File)listView1.FocusedItem.Tag);
                ea.ViewSTFSFile();
                Forms.FileDetailsForm myFileDetailsForm = new Forms.FileDetailsForm(ea.mySTFSFile.ToString(), ea.SFTSFileName.ToString());
                myFileDetailsForm.Show();
            }
        }
        /**
        *   Title:          Xbox 360 Forensic Toolkit.
        *   Description:    Xbox 360 Forensic Toolkit is built on top of the Party buffalo application. It allows users to read and 
        *                   write to Xbox 360 devices, with support for reading Xbox 360 STFS package names. A number of Forensic Tools have
        *                   been added to extract/view sectors, extract partitions, view file details, examine files in HEX. It also incorporates the wxPIRS
        *                   Application so that we can sissamble STFS packages and see their contents.
        *
        *   Original Author: Party Buffalo - landergriffith@gmail.com, wxPIRS - gael360
        *   Another Author: obriej62@mail.dcu.ie
        *   License:        http://www.gnu.org/licenses/gpl.html GNU GENERAL PUBLIC LICENSE
        *   Link:           https://code.google.com/p/party-buffalo/
        *                  http://gael360.free.fr/
        *   Note:           All Code that follows to the End point was added by obriej62@mail.dcu.ie
        */
        private void c_Delete_Click(object sender, EventArgs e)
        {
            // String.format is a little redundant here...
            DialogResult dr = DialogResult.No;
            dr = MessageBox.Show(string.Format("Are you sure you want to delete the selected{0}?", (listView1.SelectedItems.Count > 1) ? " " + listView1.SelectedItems.Count.ToString() + " items" : " item"), "Confirm Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (dr == DialogResult.Yes)
            {
                List<Entry> items = new List<Entry>();
                foreach (ListViewItem li in listView1.SelectedItems)
                {
                    items.Add((Entry)li.Tag);
                }
                Forms.EntryAction ea = new Party_Buffalo.Forms.EntryAction(items.ToArray(), Party_Buffalo.Forms.EntryAction.Method.Delete, "");
                ea.ShowDialog();
            }
        }
        /**
        *   Title:          Xbox 360 Forensic Toolkit.
        *   Description:    Xbox 360 Forensic Toolkit is built on top of the Party buffalo application. It allows users to read and 
        *                   write to Xbox 360 devices, with support for reading Xbox 360 STFS package names. A number of Forensic Tools have
        *                   been added to extract/view sectors, extract partitions, view file details, examine files in HEX. It also incorporates the wxPIRS
        *                   Application so that we can sissamble STFS packages and see their contents.
        *
        *   Original Author: Party Buffalo - landergriffith@gmail.com, wxPIRS - gael360
        *   Another Author: obriej62@mail.dcu.ie
        *   License:        http://www.gnu.org/licenses/gpl.html GNU GENERAL PUBLIC LICENSE
        *   Link:           https://code.google.com/p/party-buffalo/
        *                  http://gael360.free.fr/
        *   Note:           All Code that follows to the End point was added by obriej62@mail.dcu.ie
        */
        private void c_Extract_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 1 && !((Entry)listView1.FocusedItem.Tag).IsFolder)
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.FileName = ((Entry)listView1.FocusedItem.Tag).Name;
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    Forms.EntryAction ea = new Party_Buffalo.Forms.EntryAction(new Entry[] { ((Entry)listView1.FocusedItem.Tag) }, Party_Buffalo.Forms.EntryAction.Method.Extract, sfd.FileName);
                    ea.ShowDialog();
                }
            }
            else
            {
                FolderBrowserDialog fbd = new FolderBrowserDialog();
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    List<Entry> entries = new List<Entry>();
                    foreach (ListViewItem li in listView1.SelectedItems)
                    {
                        entries.Add((Entry)li.Tag);
                    }
                    Forms.EntryAction ea = new Party_Buffalo.Forms.EntryAction(entries.ToArray(), Party_Buffalo.Forms.EntryAction.Method.Extract, fbd.SelectedPath);
                    ea.ShowDialog();
                }
             }
        }
        /**
        *   Title:          Xbox 360 Forensic Toolkit.
        *   Description:    Xbox 360 Forensic Toolkit is built on top of the Party buffalo application. It allows users to read and 
        *                   write to Xbox 360 devices, with support for reading Xbox 360 STFS package names. A number of Forensic Tools have
        *                   been added to extract/view sectors, extract partitions, view file details, examine files in HEX. It also incorporates the wxPIRS
        *                   Application so that we can sissamble STFS packages and see their contents.
        *
        *   Original Author: Party Buffalo - landergriffith@gmail.com, wxPIRS - gael360
        *   Another Author: obriej62@mail.dcu.ie
        *   License:        http://www.gnu.org/licenses/gpl.html GNU GENERAL PUBLIC LICENSE
        *   Link:           https://code.google.com/p/party-buffalo/
        *                  http://gael360.free.fr/
        *   Note:           All Code that follows to the End point was added by obriej62@mail.dcu.ie
        */
        private void menuItem10_Click(object sender, EventArgs e)
        {

            if (((Entry)listView1.FocusedItem.Tag).IsFolder)
            {
                Forms.FileDetailsForm myFileDetailsForm = new Forms.FileDetailsForm("This is not a Valid File!");
                myFileDetailsForm.Show();
            }
            else
            {
                Forms.EntryAction ea = new Party_Buffalo.Forms.EntryAction((File)listView1.FocusedItem.Tag);
                ea.ViewSTFSFile();
                Forms.FileDetailsForm myFileDetailsForm = new Forms.FileDetailsForm(ea.mySTFSFile.ToString(), ea.SFTSFileName.ToString());
                myFileDetailsForm.Show();
            }
        }
        /**
        *   Title:          Xbox 360 Forensic Toolkit.
        *   Description:    Xbox 360 Forensic Toolkit is built on top of the Party buffalo application. It allows users to read and 
        *                   write to Xbox 360 devices, with support for reading Xbox 360 STFS package names. A number of Forensic Tools have
        *                   been added to extract/view sectors, extract partitions, view file details, examine files in HEX. It also incorporates the wxPIRS
        *                   Application so that we can sissamble STFS packages and see their contents.
        *
        *   Original Author: Party Buffalo - landergriffith@gmail.com, wxPIRS - gael360
        *   Another Author: obriej62@mail.dcu.ie
        *   License:        http://www.gnu.org/licenses/gpl.html GNU GENERAL PUBLIC LICENSE
        *   Link:           https://code.google.com/p/party-buffalo/
        *                  http://gael360.free.fr/
        *   Note:           All Code that follows to the End point was added by obriej62@mail.dcu.ie
        */
        private void menuItem43_Click(object sender, EventArgs e)
        {
            if (((Entry)listView1.FocusedItem.Tag).IsFolder)
            {
                Forms.PrintHexForm ph = new Party_Buffalo.Forms.PrintHexForm((Folder)listView1.FocusedItem.Tag);
                ph.Show();
            }
            else
            {
                Forms.PrintHexForm ph = new Party_Buffalo.Forms.PrintHexForm((File)listView1.FocusedItem.Tag);
                ph.Show();
            }
        }
        /**
        *   Title:          Xbox 360 Forensic Toolkit.
        *   Description:    Xbox 360 Forensic Toolkit is built on top of the Party buffalo application. It allows users to read and 
        *                   write to Xbox 360 devices, with support for reading Xbox 360 STFS package names. A number of Forensic Tools have
        *                   been added to extract/view sectors, extract partitions, view file details, examine files in HEX. It also incorporates the wxPIRS
        *                   Application so that we can sissamble STFS packages and see their contents.
        *
        *   Original Author: Party Buffalo - landergriffith@gmail.com, wxPIRS - gael360
        *   Another Author: obriej62@mail.dcu.ie
        *   License:        http://www.gnu.org/licenses/gpl.html GNU GENERAL PUBLIC LICENSE
        *   Link:           https://code.google.com/p/party-buffalo/
        *                  http://gael360.free.fr/
        *   Note:           All Code that follows to the End point was added by obriej62@mail.dcu.ie
        */
        private void c_Properties_Click(object sender, EventArgs e)
        {
            if (((Entry)listView1.FocusedItem.Tag).IsFolder)
            {
                Forms.PropertiesForm pf = new Party_Buffalo.Forms.PropertiesForm((Folder)listView1.FocusedItem.Tag);
                pf.Show();
            }
            else
            {
                Forms.PropertiesForm pf = new Party_Buffalo.Forms.PropertiesForm((File)listView1.FocusedItem.Tag);
                pf.Show();
            }
        }
        /**
        *   Title:          WriteToCSV
        *   Description:    is used to export a listView to a CSV File.
        *   Link:           http://cyb3r.net/software-development/export-data-from-listview-to-a-csv-file-c/
        */
        public static void WriteToCSV(string title, ListView listView, System.IO.StreamWriter sw)
        {
            if (listView.Items.Count > 0)
            {

                string separator = ","; // Can be different - UK use ',' but for Germany is ';'
                string valueFormat = "\"{0}\"" + separator; // we will replace {0} with the value using string.Format later
                StringBuilder sb = new StringBuilder(); // important: use string builder to improve performance and efficiency

                // section title
                sw.WriteLine("FileName: "+ title);
                sw.WriteLine("=============================================");
                sw.WriteLine("              Drive Image Details            ");
                sw.WriteLine("=============================================");

                // column names
                foreach (ColumnHeader ch in listView.Columns)
                {
                    sb.Append(string.Format(valueFormat, ch.Text));
                }
                sw.WriteLine(sb.ToString());

                // the actual data
                foreach (ListViewItem lvi in listView.Items)
                {

                    sb = new StringBuilder();
                    foreach (ListViewItem.ListViewSubItem listViewSubItem in lvi.SubItems)
                    {
                        sb.Append(string.Format(valueFormat, listViewSubItem.Text));
                    }

                    sw.WriteLine(sb.ToString());
                }

                // and an empty line for prettinness
                sw.WriteLine();
            }
            sw.Close();
        }
        /**
        *   Title:          Xbox 360 Forensic Toolkit.
        *   Description:    Xbox 360 Forensic Toolkit is built on top of the Party buffalo application. It allows users to read and 
        *                   write to Xbox 360 devices, with support for reading Xbox 360 STFS package names. A number of Forensic Tools have
        *                   been added to extract/view sectors, extract partitions, view file details, examine files in HEX. It also incorporates the wxPIRS
        *                   Application so that we can sissamble STFS packages and see their contents.
        *
        *   Original Author: Party Buffalo - landergriffith@gmail.com, wxPIRS - gael360
        *   Another Author: obriej62@mail.dcu.ie
        *   License:        http://www.gnu.org/licenses/gpl.html GNU GENERAL PUBLIC LICENSE
        *   Link:           https://code.google.com/p/party-buffalo/
        *                  http://gael360.free.fr/
        *   Note:           All Code that follows to the End point was added by obriej62@mail.dcu.ie
        */
        private void menuItem1_Click(object sender, EventArgs e)
        {
            if (((Entry)listView1.FocusedItem.Tag).IsFolder)
            {
                //MainForm wxPirs = new MainForm((Folder)listView1.FocusedItem.Tag);
                //wxPirs.Show();
            }
            else
            {
                MainForm wxPirs = new MainForm((File)listView1.FocusedItem.Tag);
                wxPirs.Show();
            }
        }
    }
}
