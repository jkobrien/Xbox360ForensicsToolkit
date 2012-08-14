using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CLKsFATXLib;
using Microsoft.WindowsAPICodePack.Taskbar;
using Microsoft.WindowsAPICodePack.Dialogs;
using Extensions;

namespace Party_Buffalo.Forms
{
    public partial class EntryAction : Form
    {
        TaskbarManager tm;
        System.Threading.Thread t;
        Folder Parent;
        Entry[] Entries;
        Method m;
        string OutPath;
        string[] Paths;
        string[] EntriesToSkip = new string[0];
        volatile bool Cancel;
        Drive xDrive;
        bool Windows7 = false;
        bool Aero = false;
        TaskDialog td;
        bool Timer = true;
        CLKsFATXLib.Structs.Queue[] MultiInject;

        public enum Method
        {
            Extract,
            Inject,
            Delete,
            ExtractSS,
            ExtractJ,
            Backup,
            Restore,
            Move,
        }

        public EntryAction(Entry[] Entries, Method method, string Path)
        {
            InitializeComponent();
            if (Environment.OSVersion.Version.Build >= 7600)
            {
                Windows7 = true;
            }
            if (Environment.OSVersion.Version.Build >= 6000)
            {
                Aero = true;
            }
            if (Windows7)
            {
                tm = TaskbarManager.Instance;
            }
            this.HandleCreated += new EventHandler(EntryAction_HandleCreated);
            this.FormClosing += new FormClosingEventHandler(EntryAction_FormClosing);
            m = method;
            OutPath = Path;
            this.Entries = Entries;
        }

        public EntryAction(CLKsFATXLib.Structs.Queue[] mi)
        {
            InitializeComponent();
            if (Environment.OSVersion.Version.Build >= 7600)
            {
                Windows7 = true;
            }
            if (Environment.OSVersion.Version.Build >= 6000)
            {
                Aero = true;
            }
            if (Windows7)
            {
                tm = TaskbarManager.Instance;
            }
            this.HandleCreated += new EventHandler(EntryAction_HandleCreated);
            this.FormClosing += new FormClosingEventHandler(EntryAction_FormClosing);
            MultiInject = mi;
        }

        public EntryAction(Drive xDrive, Method method, string Path)
        {
            InitializeComponent();
            if (Environment.OSVersion.Version.Build >= 7600)
            {
                Windows7 = true;
            }
            if (Environment.OSVersion.Version.Build >= 6000)
            {
                Aero = true;
            }
            if (Windows7)
            {
                tm = TaskbarManager.Instance;
            }
            this.HandleCreated += new EventHandler(EntryAction_HandleCreated);
            this.FormClosing +=new FormClosingEventHandler(EntryAction_FormClosing);
            m = method;
            OutPath = Path;
            this.xDrive = xDrive;
        }

        public EntryAction(Entry[] Entries, Method method, string[] FoldersToSkip, string Path)
        {
            InitializeComponent();
            if (Environment.OSVersion.Version.Build >= 7600)
            {
                Windows7 = true;
            }
            if (Environment.OSVersion.Version.Build >= 6000)
            {
                Aero = true;
            }
            if (Windows7)
            {
                tm = TaskbarManager.Instance;
            }
            this.HandleCreated += new EventHandler(EntryAction_HandleCreated);
            this.FormClosing += new FormClosingEventHandler(EntryAction_FormClosing);
            m = method;
            OutPath = Path;
            this.Entries = Entries;
            this.EntriesToSkip = FoldersToSkip;
        }

        public EntryAction(string[] Paths, Folder Parent, Method method)
        {
            InitializeComponent();
            if (Environment.OSVersion.Version.Build >= 7600)
            {
                Windows7 = true;
            }
            if (Environment.OSVersion.Version.Build >= 6000)
            {
                Aero = true;
            }
            if (Windows7)
            {
                tm = TaskbarManager.Instance;
            }
            this.HandleCreated += new EventHandler(EntryAction_HandleCreated);
            this.FormClosing +=new FormClosingEventHandler(EntryAction_FormClosing);
            m = method;
            this.Paths = Paths;
            this.Parent = Parent;
        }

        public EntryAction(string[] FilePaths,Drive Drive, Method method)
        {
            InitializeComponent();
            if (Environment.OSVersion.Version.Build >= 7600)
            {
                Windows7 = true;
            }
            if (Environment.OSVersion.Version.Build >= 6000)
            {
                Aero = true;
            }
            if (Windows7)
            {
                tm = TaskbarManager.Instance;
            }
            this.HandleCreated += new EventHandler(EntryAction_HandleCreated);
            this.FormClosing += new FormClosingEventHandler(EntryAction_FormClosing);
            m = method;
            this.Paths = FilePaths;
            xDrive = Drive;
        }

        void EntryAction_FormClosing(object sender, FormClosingEventArgs e)
        {
            System.Media.SystemSounds.Asterisk.Play();
            if (Windows7)
            {
                tm.SetProgressState(TaskbarProgressBarState.NoProgress);
            }
            if (Entries != null)
            {
                for (int i = 0; i < Entries.Length; i++)
                {
                    if (Entries[i].IsFolder)
                    {
                        ResetFolderActions((Folder)Entries[i]);
                    }
                }
            }
        }

        void ResetFolderActions(Folder folder)
        {
            folder.ResetFolderAction();
            foreach (Folder f in folder.Folders())
            {
                ResetFolderActions(f);
            }
        }

        void EntryAction_HandleCreated(object sender, EventArgs e)
        {
            if (Aero)
            {
                td = new TaskDialog();
            }
            System.Threading.ThreadStart ts = delegate
            {
#if TRACE
                try
                {
#endif
                //while (true)//(!this.IsHandleCreated || !progressBar1.IsHandleCreated || !label1.IsHandleCreated || !lPercent.IsHandleCreated || !button1.IsHandleCreated)
                //{
                //    try
                //    {
                //        this.Invoke((MethodInvoker)delegate { });
                //        progressBar1.Invoke((MethodInvoker)delegate { });
                //        label1.Invoke((MethodInvoker)delegate { });
                //        lPercent.Invoke((MethodInvoker)delegate { });
                //        button1.Invoke((MethodInvoker)delegate { });
                //        break;
                //    }
                //    catch(Exception E) { Application.DoEvents(); }
                //}
                if (xDrive != null && m == Method.Backup || m == Method.ExtractJ || m == Method.ExtractSS || m == Method.Restore)
                {
                    switch (m)
                    {
                        case Method.Backup:
                            this.Invoke((MethodInvoker)delegate
                            {
                                this.Icon = Properties.Resources.Add;
                            });
                            CLKsFATXLib.Streams.Reader r = xDrive.Reader();
                            CLKsFATXLib.Streams.Writer w = new CLKsFATXLib.Streams.Writer(new System.IO.FileStream(OutPath, System.IO.FileMode.Create));
                            int ReadLength = 0x200;
                            if (xDrive.Length % 0x100000 == 0)
                            {
                                ReadLength = 0x100000;
                            }
                            else if (xDrive.Length % 0x40000 == 0)
                            {
                                ReadLength = 0x40000;
                            }
                            else if (xDrive.Length % 0x10000 == 0)
                            {
                                ReadLength = 0x10000;
                            }
                            else if (xDrive.Length % 0x5000 == 0)
                            {
                                ReadLength = 0x5000;
                            }
                            for (int i = 0; i < xDrive.Length / ReadLength; i++)
                            {
                                if (Cancel)
                                {
                                    break;
                                }
                                w.Write(r.ReadBytes(ReadLength));
                                progressBar1.Invoke((MethodInvoker)delegate
                                {
                                    try
                                    {
                                        progressBar1.Maximum = (int)(xDrive.Length / ReadLength);
                                        progressBar1.Value = (i + 1);
                                        if (Windows7)
                                        {
                                            tm.SetProgressValue(progressBar1.Value, progressBar1.Maximum);
                                        }
                                    }
                                    catch { }
                                });
                                this.Invoke((MethodInvoker)delegate
                                {
                                    this.Text = "Backing Up Drive";
                                });
                                label1.Invoke((MethodInvoker)delegate
                                {
                                    label1.Text = (((decimal)(i + 1) / (decimal)(xDrive.Length / ReadLength)) * 100).ToString("#") + "%";
                                });
                            }
                            w.Close();
                            break;
                        case Method.ExtractSS:
                            this.Invoke((MethodInvoker)delegate
                            {
                                this.Icon = Properties.Resources.Add;
                            });
                            //Create our io for the drive
                            CLKsFATXLib.Streams.Reader io = xDrive.Reader();
                            //Go to the location of the security sector
                            io.BaseStream.Position = 0x2000;
                            //Create our ref io for the file
                            CLKsFATXLib.Streams.Writer bw = new CLKsFATXLib.Streams.Writer(new System.IO.FileStream(OutPath, System.IO.FileMode.Create));
                            //Read the sector.  The size is an estimation, since I have no idea how big it really is
                            bw.Write(io.ReadBytes(0xE00));
                            //Close our io
                            bw.Close();
                            break;
                        case Method.ExtractJ:
                            this.Invoke((MethodInvoker)delegate
                            {
                                this.Icon = Properties.Resources.Add;
                            });
                            //Create our io for the drive
                            CLKsFATXLib.Streams.Reader io2 = xDrive.Reader();
                            //Go to the location of the security sector
                            io2.BaseStream.Position = 0x800;
                            //Create our ref io for the file
                            CLKsFATXLib.Streams.Writer bw2 = new CLKsFATXLib.Streams.Writer(new System.IO.FileStream(OutPath, System.IO.FileMode.Create));
                            //Read the sector.  The size is an estimation, since I have no idea how big it really is
                            bw2.Write(io2.ReadBytes(0x400));
                            //Close our io
                            bw2.Close();
                            break;
                        case Method.Restore:
                            this.Invoke((MethodInvoker)delegate
                            {
                                this.Icon = Properties.Resources.Remove;
                            });
                            if (MessageBox.Show("WARNING: Restoring a drive that does not match your current one can cause for data to not be read correctly by the Xbox 360, or for other unforseen problems!  Please make sure you know what you're doing before continuing.  Are you sure you want to continue?", "WARNING AND STUFF", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                            {
                                if (MessageBox.Show("This is your last chance to stop!  Are you POSITIVE you want to continue?", "Last Chance!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                                {
                                    CLKsFATXLib.Streams.Reader r2 = new CLKsFATXLib.Streams.Reader(new System.IO.FileStream(OutPath, System.IO.FileMode.Open));
                                    CLKsFATXLib.Streams.Writer w2 = xDrive.Writer();
                                    int ReadLength2 = 0x200;
                                    if (xDrive.Length % 0x4000 != 0)
                                    {
                                        ReadLength2 = 0x4000;
                                    }
                                    else
                                    {
                                        for (int i = 0x300000; i > 0x200; i -= 0x1000)
                                        {
                                            if (xDrive.Length % i == 0)
                                            {
                                                ReadLength2 = i;
                                                break;
                                            }
                                        }
                                    }
                                    for (int i = 0; i < xDrive.Length / ReadLength2; i++)
                                    {
                                        if (Cancel)
                                        {
                                            break;
                                        }
                                        w2.Write(r2.ReadBytes(ReadLength2));
                                        progressBar1.Invoke((MethodInvoker)delegate
                                        {
                                            try
                                            {
                                                progressBar1.Maximum = (int)(xDrive.Length / ReadLength2);
                                                progressBar1.Value = (i + 1);
                                                if (Windows7)
                                                {
                                                    tm.SetProgressValue(progressBar1.Value, progressBar1.Maximum);
                                                }
                                            }
                                            catch { }
                                        });
                                        this.Invoke((MethodInvoker)delegate
                                        {
                                            this.Text = "Restoring Drive";
                                        });
                                        label1.Invoke((MethodInvoker)delegate
                                        {
                                            label1.Text = (((decimal)(i + 1) / (decimal)(xDrive.Length / ReadLength2)) * 100).ToString("#") + "%";
                                        });
                                    }
                                    r2.Close();
                                }
                            }
                            break;
                    }
                }
                else
                {
                    Folder ParentFolder = null;
                    this.Invoke((MethodInvoker)delegate
                    {
                        ParentFolder = Parent;
                    });
                    switch (m)
                    {
                        case Method.Extract:
#if DEBUG
                            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                            if (Timer)
                            {
                                sw.Start();
                            }
#endif
                            this.Invoke((MethodInvoker)delegate
                            {
                                this.Icon = Properties.Resources.Add;
                            });
                            foreach (Entry entry in Entries)
                            {
                                if (!entry.IsFolder)
                                {
                                    ((File)entry).FileAction += new CLKsFATXLib.Structs.FileActionChanged(EntryAction_FileAction);
                                    this.Invoke((MethodInvoker)delegate { this.Text = entry.FullPath; });
                                    label1.Invoke((MethodInvoker)delegate { label1.Text = entry.Name; });
                                    // Check to see if we're batch-extracting...
                                    if (Entries.Length == 1)
                                    {
                                        ((File)entry).Extract(OutPath);
                                    }
                                    else
                                    {
                                        ((File)entry).Extract(OutPath + "\\" + entry.Name);
                                    }
                                }
                                else
                                {
                                    ((Folder)entry).FolderAction += new CLKsFATXLib.Structs.FolderActionChanged(EntryAction_FolderAction);
                                    ((Folder)entry).Extract(OutPath, EntriesToSkip);
                                }
                                if (Cancel)
                                {
                                    break;
                                }
                            }
#if DEBUG
                            if (Timer)
                            {
                                sw.Stop();
                                MessageBox.Show(string.Format("{0}:{1}:{2}", sw.Elapsed.Minutes, sw.Elapsed.Seconds, sw.Elapsed.Milliseconds));
                            }
#endif
                            break;
                        case Method.Delete:
                            this.Invoke((MethodInvoker)delegate
                            {
                                this.Icon = Properties.Resources.Remove;
                            });
                            foreach (Entry entry in Entries)
                            {
                                if (Cancel)
                                {
                                    break;
                                }
                                if (entry.IsFolder)
                                {
                                    Folder current = ((Folder)entry);
                                    current.ResetFolderAction();
                                    current.FolderAction += new CLKsFATXLib.Structs.FolderActionChanged(EntryAction_FolderAction);
                                    current.Delete();
                                }
                                else
                                {
                                    this.Invoke((MethodInvoker)delegate { this.Text = entry.FullPath; });
                                    label1.Invoke((MethodInvoker)delegate { label1.Text = entry.Name; });
                                    File current = ((File)entry);
                                    current.FileAction += new CLKsFATXLib.Structs.FileActionChanged(EntryAction_FileAction);
                                    current.Delete();
                                }
                            }
                            break;
                        case Method.Inject:
#if DEBUG
                            System.Diagnostics.Stopwatch sw2 = new System.Diagnostics.Stopwatch();
                            if (Timer)
                            {
                                sw2.Start();
                            }
#endif
                            this.Invoke((MethodInvoker)delegate
                            {
                                this.Icon = Properties.Resources.Add;
                            });
                            if (ParentFolder != null)
                            {
                                ParentFolder.ResetFolderAction();
                                ParentFolder.FolderAction += new CLKsFATXLib.Structs.FolderActionChanged(EntryAction_FolderAction);
                                List<CLKsFATXLib.Structs.ExistingEntry> Existing = new List<CLKsFATXLib.Structs.ExistingEntry>();
                                foreach (string s in Paths)
                                {
                                    if (Cancel)
                                    {
                                        break;
                                    }
                                    if (VariousFunctions.IsFolder(s))
                                    {
                                        Existing.AddRange(ParentFolder.InjectFolder(s, false, false));
                                    }
                                    else
                                    {
                                        ParentFolder.FolderAction += new CLKsFATXLib.Structs.FolderActionChanged(EntryAction_FolderAction);
                                        CLKsFATXLib.Structs.WriteResult wr = ParentFolder.CreateNewFile(s);
                                        if (wr.CouldNotWrite)
                                        {
                                            CLKsFATXLib.Structs.ExistingEntry ex = new CLKsFATXLib.Structs.ExistingEntry();
                                            ex.Existing = wr.Entry;
                                            ex.NewPath = s;
                                            Existing.Add(ex);
                                        }
                                    }
                                }

                                DoExisting(Existing);
                            }
                            else
                            {
                                List<CLKsFATXLib.Structs.ExistingEntry> Existing = new List<CLKsFATXLib.Structs.ExistingEntry>();
                                foreach (string s in Paths)
                                {
                                    string Path = "";
                                    try
                                    {
                                        Path = VariousFunctions.GetFATXPath(s);
                                    }
                                    catch (Exception x)
                                    {
                                        ExceptionHandler(x);
                                        continue;
                                    }
                                    Folder thisFolder = xDrive.CreateDirectory("Data\\" + Path);
                                    thisFolder.ResetFolderAction();
                                    thisFolder.FolderAction += new CLKsFATXLib.Structs.FolderActionChanged(EntryAction_FolderAction);
                                    if (Cancel)
                                    {
                                        break;
                                    }
                                    if (VariousFunctions.IsFolder(s))
                                    {
                                        ExceptionHandler(new Exception("Can not write folder as STFS package (silly error wording)"));
                                        continue;
                                    }
                                    else
                                    {
                                        thisFolder.FolderAction += new CLKsFATXLib.Structs.FolderActionChanged(EntryAction_FolderAction);
                                        CLKsFATXLib.Structs.WriteResult wr = thisFolder.CreateNewFile(s);
                                        if (wr.CouldNotWrite)
                                        {
                                            CLKsFATXLib.Structs.ExistingEntry ex = new CLKsFATXLib.Structs.ExistingEntry();
                                            ex.Existing = wr.Entry;
                                            ex.NewPath = s;
                                            Existing.Add(ex);
                                        }
                                    }
                                }

                                DoExisting(Existing);
                            }
#if DEBUG
                            if (Timer)
                            {
                                sw2.Stop();
                                MessageBox.Show(string.Format("{0}:{1}:{2}", sw2.Elapsed.Minutes, sw2.Elapsed.Seconds, sw2.Elapsed.Milliseconds));
                            }
#endif
                            break;
                        case Method.Move:
                            List<CLKsFATXLib.Structs.WriteResult> Results = new List<CLKsFATXLib.Structs.WriteResult>();
                            foreach (Entry Entry in Entries)
                            {
                                CLKsFATXLib.Structs.WriteResult wr = Entry.Move(OutPath);
                                if (wr.CouldNotWrite)
                                {
                                    Results.Add(wr);
                                }
                            }
                            break;
                    }
                }
                this.Invoke((MethodInvoker)delegate { this.Close(); });
#if TRACE
                }
                catch (Exception x)
                {
                    ExceptionHandler(x);
                    this.Invoke((MethodInvoker)delegate { this.Close(); });
                }
#endif
            };
            t = new System.Threading.Thread(ts);
            t.Start();
        }

        void ExceptionHandler(Exception x)
        {
            if (!Aero)
            {
                MessageBox.Show("An exception was thrown: " + x.Message + "\r\n\r\nPress CTRL + C to copy the stack trace:\r\n" + x.StackTrace);
            }
            else
            {
                tm.SetProgressState(TaskbarProgressBarState.Error);
                this.Invoke((MethodInvoker)delegate
                {
                    TaskDialog td = new TaskDialog();
                    td.Caption = "Unhandled Exception";
                    td.InstructionText = "An Unhandled Exception was Thrown";
                    td.Text = string.Format("An exception was thrown: {0}\r\n\r\nIf this appears to be a bug, please email me at clkxu5@gmail.com with the details below", x.Message);
                    td.DetailsCollapsedLabel = "Details";
                    td.DetailsExpandedLabel = "Details";
                    td.DetailsExpandedText = x.StackTrace;

                    TaskDialogButton Copy = new TaskDialogButton("Copy", "Copy Details to Clipboard");
                    Copy.Click += (o, f) => { this.Invoke((MethodInvoker)delegate { Clipboard.SetDataObject(x.Message + "\r\n\r\n" + x.StackTrace, true, 10, 200); }); };

                    TaskDialogButton Close = new TaskDialogButton("Close", "Close");
                    Close.Click += (o, f) => { td.Close(); };

                    td.Controls.Add(Copy);
                    td.Controls.Add(Close);
                    td.ShowDialog(this.Handle);
                });
            }
        }

        void DoExisting(List<CLKsFATXLib.Structs.ExistingEntry> Existing)
        {
            if (Existing.Count == 0)
            {
                return;
            }
            List<CLKsFATXLib.Structs.ExistingEntry> FilesWithFolders = new List<CLKsFATXLib.Structs.ExistingEntry>();
            List<CLKsFATXLib.Structs.ExistingEntry> FoldersWithFiles = new List<CLKsFATXLib.Structs.ExistingEntry>();
            List<CLKsFATXLib.Structs.ExistingEntry> Folders = new List<CLKsFATXLib.Structs.ExistingEntry>();
            List<CLKsFATXLib.Structs.ExistingEntry> Files = new List<CLKsFATXLib.Structs.ExistingEntry>();
            MergedPaths = new List<CLKsFATXLib.Structs.ExistingEntry>();
            goto _Sort;
        _Sort:
            {
                foreach (CLKsFATXLib.Structs.ExistingEntry ex in Existing)
                {
                    if (ex.Existing.IsFolder && VariousFunctions.IsFolder(ex.NewPath))
                    {
                        Folders.Add(ex);
                    }
                    else if (ex.Existing.IsFolder && !VariousFunctions.IsFolder(ex.NewPath))
                    {
                        FilesWithFolders.Add(ex);
                    }
                    else if (!ex.Existing.IsFolder && VariousFunctions.IsFolder(ex.NewPath))
                    {
                        FoldersWithFiles.Add(ex);
                    }
                    else
                    {
                        Files.Add(ex);
                    }
                }
                Existing = new List<CLKsFATXLib.Structs.ExistingEntry>();
            }

            bool Delete = false;

            for (int i = 0; i < Folders.Count; i++)
            {
                if (Cancel)
                {
                    return;
                }
                if (Windows7)
                {
                    tm.SetProgressValue(1, 1);
                    tm.SetProgressState(TaskbarProgressBarState.Error);
                }
                DialogResult dr = DialogResult.Ignore;
                bool Checked = false;
                if (!Aero)
                {
                    Forms.InjectDialog id = new InjectDialog(Folders[i], Folders.Count - 1);
                    CheckForIllegalCrossThreadCalls = false;
                    id.ShowDialog(this.Owner);
                    Checked = id.checkBox1.Checked;
                    CheckForIllegalCrossThreadCalls = true;
                }
                else
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        td.Caption = "Entry Already Exists";
                        td.InstructionText = "Cannot Write Folder \"" + Folders[i].NewPath + "\"";
                        td.Text = "A " + ((Folders[i].Existing.IsFolder) ? "folder " : "file ") + "named \"" + Folders[i].Existing.Name + "\" already exists in the directory \"" + Folders[i].Existing.Parent.FullPath + "\".  Would you like to " + ((Folders[i].Existing.IsFolder) ? "merge the already existing folder with the new one?" : "overwrite the currently existing file to write the new folder?");
                        td.FooterCheckBoxChecked = false;
                        td.FooterCheckBoxText = "Do this for all current items? (" + Folders.Count + ")";
                        td.Icon = TaskDialogStandardIcon.Error;
                        td.StandardButtons = TaskDialogStandardButtons.Yes | TaskDialogStandardButtons.No;
                        if (td.ShowDialog(this.Handle) == TaskDialogResult.Yes)
                        {
                            dr = DialogResult.Yes;
                        }
                        Checked = (bool)td.FooterCheckBoxChecked;
                    });
                }
                // If they want to merge the folders
                if (dr == DialogResult.Yes)
                {
                    if (Windows7)
                    {
                        tm.SetProgressState(TaskbarProgressBarState.Normal);
                    }
                    // If they want to do all of them...
                    if (Checked)
                    {
                        foreach (var existing in Folders)
                        {
                            Existing.AddRange(((Folder)existing.Existing).Parent.InjectFolder(existing.NewPath, true, Delete));
                            MergedPaths.Add(existing);
                        }
                        Folders = new List<CLKsFATXLib.Structs.ExistingEntry>();
                        break;
                    }
                    else
                    {
                        foreach (System.IO.DirectoryInfo di in new System.IO.DirectoryInfo(Folders[i].NewPath).GetDirectories())
                        {
                            Existing.AddRange(((Folder)Folders[i].Existing).InjectFolder(di.FullName, false, Delete));
                        }
                        foreach (System.IO.FileInfo fi in new System.IO.DirectoryInfo(Folders[i].NewPath).GetFiles())
                        {
                            CLKsFATXLib.Structs.WriteResult wr = ((Folder)Folders[i].Existing).CreateNewFile(fi.FullName);
                            CLKsFATXLib.Structs.ExistingEntry exe = new CLKsFATXLib.Structs.ExistingEntry();
                            exe.Existing = wr.Entry;
                            exe.NewPath = fi.FullName;
                            Existing.Add(exe);
                        }
                        //Existing.AddRange(((Folder)Folders[i].Existing).Parent.InjectFolder(Folders[i].NewPath, false, Delete));
                        MergedPaths.Add(Folders[i]);
                        Folders.RemoveAt(i);
                        i--;
                    }
                }
                else if (Checked)
                {
                    Folders = new List<CLKsFATXLib.Structs.ExistingEntry>();
                }
            }

            if (Existing.Count != 0)
            {
                goto _Sort;
            }

            for (int i = 0; i < Files.Count; i++)
            {
                if (Cancel)
                {
                    return;
                }
                // If we don't even have to show a dialog...
                if (Delete)
                {
                    Files[i].Existing.Parent.FolderAction += new CLKsFATXLib.Structs.FolderActionChanged(EntryAction_FolderAction);
                    ((File)Files[i].Existing).Delete();
                    Files[i].Existing.Parent.CreateNewFile(Files[i].NewPath);
                }
                else
                {
                    if (Windows7)
                    {
                        tm.SetProgressValue(1, 1);
                        tm.SetProgressState(TaskbarProgressBarState.Error);
                    }
                    DialogResult dr = DialogResult.Ignore;
                    bool Checked = false;
                    if (!Aero)
                    {
                        Forms.InjectDialog id = new InjectDialog(Files[i], Files.Count - 1);
                        CheckForIllegalCrossThreadCalls = false;
                        id.ShowDialog(this.Owner);
                        Checked = id.checkBox1.Checked;
                        CheckForIllegalCrossThreadCalls = true;
                    }
                    else
                    {
                        this.Invoke((MethodInvoker)delegate
                        {
                            td.Caption = "File Already Exists";
                            td.InstructionText = "Cannot Write File \"" + Files[i].NewPath + "\"";
                            td.Text = "A file named \"" + Files[i].Existing.Name + "\" already exists in the directory \"" + Files[i].Existing.Parent.FullPath + "\".  Would you like to overwrite the currently existing file to write the new file?";
                            td.FooterCheckBoxChecked = false;
                            td.FooterCheckBoxText = "Do this for all current items (" + Files.Count + ")";
                            td.Icon = TaskDialogStandardIcon.Error;
                            td.StandardButtons = TaskDialogStandardButtons.Yes | TaskDialogStandardButtons.No;
                            if (td.ShowDialog(this.Handle) == TaskDialogResult.Yes)
                            {
                                dr = DialogResult.Yes;
                            }
                            Checked = (bool)td.FooterCheckBoxChecked;
                        });
                    }
                    if (dr == DialogResult.Yes)
                    {
                        if (Windows7)
                        {
                            tm.SetProgressState(TaskbarProgressBarState.Normal);
                        }
                        if (Checked)
                        {
                            Delete = true;
                            Files[i].Existing.Parent.FolderAction += new CLKsFATXLib.Structs.FolderActionChanged(EntryAction_FolderAction);
                            ((File)Files[i].Existing).Delete();
                            Files[i].Existing.Parent.CreateNewFile(Files[i].NewPath);
                            Files.RemoveAt(i);
                            i--;
                        }
                        else
                        {
                            ((File)Files[i].Existing).Delete();
                            Files[i].Existing.Parent.CreateNewFile(Files[i].NewPath);
                            Files.RemoveAt(i);
                            i--;
                        }
                    }
                    else if (Checked)
                    {
                        Files = new List<CLKsFATXLib.Structs.ExistingEntry>();
                    }
                }
            }

            if (Existing.Count != 0)
            {
                goto _Sort;
            }

            if (FilesWithFolders.Count > 0 || FoldersWithFiles.Count > 0)
            {
                Forms.Existing exForm = new Existing(FilesWithFolders, FoldersWithFiles);
                CheckForIllegalCrossThreadCalls = false;
                exForm.Show(this.Owner);
                CheckForIllegalCrossThreadCalls = true;
            }
        }

        public List<CLKsFATXLib.Structs.ExistingEntry> MergedPaths
        {
            get;
            private set;
        }

        void EntryAction_FolderAction(ref CLKsFATXLib.Structs.FolderAction Progress)
        {
            try
            {
                int p = Progress.Progress;
                int m = Progress.MaxValue;
                string file = Progress.CurrentFile;
                string filepath = Progress.CurrentFilePath;
                if (p == 0 && m == 0 && file == null && filepath == null && Progress.Cancel != false)
                {
                    Progress.Cancel = Cancel;
                    return;
                }
                Progress.Cancel = Cancel;
                try
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        this.Text = filepath;
                        if (file != null)
                        {
                            label1.Text = file;
                        }
                        else if (filepath != null)
                        {
                            label1.Text = filepath.Remove(0, filepath.LastIndexOf('\\') + 1);
                        }
                        lPercent.Text = (p == 0 || (((decimal)p / (decimal)m) * 100).ToString("#") == "0") ? "" : (((decimal)p / (decimal)m) * 100).ToString("#") + "%";
                        progressBar1.Maximum = m;
                        progressBar1.Value = p;
                        if (Windows7)
                        {
                            tm.SetProgressValue(p, m);
                        }
                    });
                }
                catch(Exception e) { }
            }
            catch(Exception e) { }
        }

        void EntryAction_FileAction(ref CLKsFATXLib.Structs.FileAction Progress)
        {
            try
            {
                int p = Progress.Progress;
                int m = Progress.MaxValue;
                Progress.Cancel = Cancel;
                lPercent.Invoke((MethodInvoker)delegate { lPercent.Text = (p == 0 || (((decimal)p / (decimal)m) * 100).ToString("#") == "0") ? "" : (((decimal)p / (decimal)m) * 100).ToString("#") + "%"; });
                progressBar1.Invoke((MethodInvoker)delegate { 
                    progressBar1.Maximum = m;
                    progressBar1.Value = p;
                    if (Windows7)
                    {
                        tm.SetProgressValue(p, m);
                    }
                });
            }
            catch { }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Cancel = true;
        }
    }
}
