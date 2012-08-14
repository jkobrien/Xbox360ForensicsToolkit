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
    using WxTools;

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
        File xFile;
        System.IO.BinaryReader myBinaryReader;
        bool Windows7 = false;
        bool Aero = false;
        TaskDialog td;
        bool Timer = true;
        CLKsFATXLib.Structs.Queue[] MultiInject;
        public StringBuilder JoshSector = new StringBuilder();
        public StringBuilder SecuritySector = new StringBuilder();
        public StringBuilder mySTFSFile = new StringBuilder();
        public StringBuilder myDriveInfo = new StringBuilder();
        public string SFTSFileName = "";
        public string myFileName = "";
        public bool isSTFSFile = true;

        public enum Method
        {
            Extract,
            Inject,
            Delete,
            ExtractSS,
            ExtractJ,
            ExtractJoshSectors,
            ExtractSecuritySectors,
            ExtractSystemCache,
            ExtractGameCache,
            ExtractSysExt,
            ExtractSysExt2,
            ExtractXbox,
            ExtractData,
            Backup,
            Restore,
            Move,
        }

        public EntryAction(Drive xDrive)
        {
            InitializeComponent();
            this.xDrive = xDrive;
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
        public EntryAction(string myFileName)
        {
            InitializeComponent();
            this.myFileName = myFileName;
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
        public EntryAction(File xFile)
        {
            InitializeComponent();
            this.xFile = xFile;
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
                int ReadLength;
                long BytesToBeRead;
                if (xDrive != null && m == Method.Backup || m == Method.ExtractJ || m == Method.ExtractSS || m == Method.Restore || m == Method.ExtractJoshSectors || m == Method.ExtractSecuritySectors
                    || m == Method.ExtractSystemCache || m == Method.ExtractGameCache || m == Method.ExtractSysExt || m == Method.ExtractSysExt2 || m == Method.ExtractXbox || m == Method.ExtractData)
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
                            ReadLength = 0x200;
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
                        case Method.ExtractJoshSectors:
                            this.Invoke((MethodInvoker)delegate
                            {
                                this.Icon = Properties.Resources.Add;
                            });
                            //Create our io for the drive
                            CLKsFATXLib.Streams.Reader io3 = xDrive.Reader();
                            //Go to the location of the security sector
                            io3.BaseStream.Position = 0x000;
                            //Create our ref io for the file
                            CLKsFATXLib.Streams.Writer bw3 = new CLKsFATXLib.Streams.Writer(new System.IO.FileStream(OutPath, System.IO.FileMode.Create));
                            //Read the sector.  The size is an estimation, since I have no idea how big it really is
                            bw3.Write(io3.ReadBytes(0x2000));
                            //Close our io
                            bw3.Close();
                            break;
                        case Method.ExtractSecuritySectors:
                            this.Invoke((MethodInvoker)delegate
                            {
                                this.Icon = Properties.Resources.Add;
                            });
                            //Create our io for the drive
                            CLKsFATXLib.Streams.Reader io4 = xDrive.Reader();
                            //Go to the location of the security sector
                            io4.BaseStream.Position = 0x2000;
                            //Create our ref io for the file
                            CLKsFATXLib.Streams.Writer bw4 = new CLKsFATXLib.Streams.Writer(new System.IO.FileStream(OutPath, System.IO.FileMode.Create));
                            //Read the sector.  The size is an estimation, since I have no idea how big it really is
                            bw4.Write(io4.ReadBytes(0x80000));
                            //Close our io
                            bw4.Close();
                            break;
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
                        case Method.ExtractSystemCache:
                            this.Invoke((MethodInvoker)delegate
                            {
                                this.Icon = Properties.Resources.Add;
                            });
                            //Create our io for the drive
                            CLKsFATXLib.Streams.Reader io5 = xDrive.Reader();
                            //Go to the location of the security sector
                            io5.BaseStream.Position = 0x80000;
                            //Create our ref io for the file
                            CLKsFATXLib.Streams.Writer bw5 = new CLKsFATXLib.Streams.Writer(new System.IO.FileStream(OutPath, System.IO.FileMode.Create));
                            //Read the sector.  The size is an estimation, since I have no idea how big it really is
                            BytesToBeRead = 0x80000000;
                            ReadLength = 0x200;
                            if (BytesToBeRead % 0x100000 == 0)
                            {
                                ReadLength = 0x100000;
                            }
                            else if (BytesToBeRead % 0x40000 == 0)
                            {
                                ReadLength = 0x40000;
                            }
                            else if (BytesToBeRead % 0x10000 == 0)
                            {
                                ReadLength = 0x10000;
                            }
                            else if (BytesToBeRead % 0x5000 == 0)
                            {
                                ReadLength = 0x5000;
                            }
                            //long myReadLength = 0x80000000;
                            for (int i = 0; i < BytesToBeRead / ReadLength; i++)
                            {
                                bw5.Write(io5.ReadBytes(ReadLength));
                                progressBar1.Invoke((MethodInvoker)delegate
                                {
                                    try
                                    {
                                        progressBar1.Maximum = (int)(BytesToBeRead / ReadLength);
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
                                    this.Text = "Extracting System Cache";
                                });
                                label1.Invoke((MethodInvoker)delegate
                                {
                                    label1.Text = (((decimal)(i + 1) / (decimal)(BytesToBeRead / ReadLength)) * 100).ToString("#") + "%";
                                });
                            }
                            //Close our io
                            bw5.Close();
                            break;
                        case Method.ExtractGameCache:
                            this.Invoke((MethodInvoker)delegate
                            {
                                this.Icon = Properties.Resources.Add;
                            });
                            //Create our io for the drive
                            CLKsFATXLib.Streams.Reader io6 = xDrive.Reader();
                            //Go to the location of the security sector
                            io6.BaseStream.Position = 0x80080000;
                            //Create our ref io for the file
                            CLKsFATXLib.Streams.Writer bw6 = new CLKsFATXLib.Streams.Writer(new System.IO.FileStream(OutPath, System.IO.FileMode.Create));
                            //Read the sector.  The size is an estimation, since I have no idea how big it really is
                            BytesToBeRead = 0xA0E30000;
                            ReadLength = 0x200;
                            if (BytesToBeRead % 0x100000 == 0)
                            {
                                ReadLength = 0x100000;
                            }
                            else if (BytesToBeRead % 0x40000 == 0)
                            {
                                ReadLength = 0x40000;
                            }
                            else if (BytesToBeRead % 0x10000 == 0)
                            {
                                ReadLength = 0x10000;
                            }
                            else if (BytesToBeRead % 0x5000 == 0)
                            {
                                ReadLength = 0x5000;
                            }
                            //long myReadLength2 = 0xA0E30000;
                            for (int i = 0; i < BytesToBeRead / ReadLength; i++)
                            {
                                bw6.Write(io6.ReadBytes(ReadLength));
                                progressBar1.Invoke((MethodInvoker)delegate
                                {
                                    try
                                    {
                                        progressBar1.Maximum = (int)(BytesToBeRead / ReadLength);
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
                                    this.Text = "Extracting Game Cache";
                                });
                                label1.Invoke((MethodInvoker)delegate
                                {
                                    label1.Text = (((decimal)(i + 1) / (decimal)(BytesToBeRead / ReadLength)) * 100).ToString("#") + "%";
                                });
                            }
                            //bw6.Write(io6.ReadBytes(0xA0E30000));
                            //Close our io
                            bw6.Close();
                            break;
                        case Method.ExtractSysExt:
                            this.Invoke((MethodInvoker)delegate
                            {
                                this.Icon = Properties.Resources.Add;
                            });
                            //Create our io for the drive
                            CLKsFATXLib.Streams.Reader io7 = xDrive.Reader();
                            //Go to the location of the security sector
                            io7.BaseStream.Position = 0x10C080000;
                            //Create our ref io for the file
                            CLKsFATXLib.Streams.Writer bw7 = new CLKsFATXLib.Streams.Writer(new System.IO.FileStream(OutPath, System.IO.FileMode.Create));
                            //Read the sector.  The size is an estimation, since I have no idea how big it really is
                            bw7.Write(io7.ReadBytes(0xCE30000));
                            //Close our io
                            bw7.Close();
                            break;
                        case Method.ExtractSysExt2:
                            this.Invoke((MethodInvoker)delegate
                            {
                                this.Icon = Properties.Resources.Add;
                            });
                            //Create our io for the drive
                            CLKsFATXLib.Streams.Reader io8 = xDrive.Reader();
                            //Go to the location of the security sector
                            io8.BaseStream.Position = 0x118EB0000;
                            //Create our ref io for the file
                            CLKsFATXLib.Streams.Writer bw8 = new CLKsFATXLib.Streams.Writer(new System.IO.FileStream(OutPath, System.IO.FileMode.Create));
                            //Read the sector.  The size is an estimation, since I have no idea how big it really is
                            bw8.Write(io8.ReadBytes(0x8000000));
                            //Close our io
                            bw8.Close();
                            break;
                        case Method.ExtractXbox:
                            this.Invoke((MethodInvoker)delegate
                            {
                                this.Icon = Properties.Resources.Add;
                            });
                            //Create our io for the drive
                            CLKsFATXLib.Streams.Reader io9 = xDrive.Reader();
                            //Go to the location of the security sector
                            io9.BaseStream.Position = 0x120eb0000;
                            //Create our ref io for the file
                            CLKsFATXLib.Streams.Writer bw9 = new CLKsFATXLib.Streams.Writer(new System.IO.FileStream(OutPath, System.IO.FileMode.Create));
                            //Read the sector.  The size is an estimation, since I have no idea how big it really is
                            bw9.Write(io9.ReadBytes(0x10000000));
                            //Close our io
                            bw9.Close();
                            break;
                        case Method.ExtractData:
                            this.Invoke((MethodInvoker)delegate
                            {
                                this.Icon = Properties.Resources.Add;
                            });
                            //Create our io for the drive
                            CLKsFATXLib.Streams.Reader io10 = xDrive.Reader();
                            //Go to the location of the security sector
                            io10.BaseStream.Position = 0x130eb0000;
                            //Create our ref io for the file
                            CLKsFATXLib.Streams.Writer bw10 = new CLKsFATXLib.Streams.Writer(new System.IO.FileStream(OutPath, System.IO.FileMode.Create));
                            //Read the sector.  The size is an estimation, since I have no idea how big it really is
                            BytesToBeRead = xDrive.Length - 0x130eb0000;
                            ReadLength = 0x200;
                            if (BytesToBeRead % 0x100000 == 0)
                            {
                                ReadLength = 0x100000;
                            }
                            else if (BytesToBeRead % 0x40000 == 0)
                            {
                                ReadLength = 0x40000;
                            }
                            else if (BytesToBeRead % 0x10000 == 0)
                            {
                                ReadLength = 0x10000;
                            }
                            else if (BytesToBeRead % 0x5000 == 0)
                            {
                                ReadLength = 0x5000;
                            }
                            //long myReadLength3 = xDrive.Length - 0x130eb0000;
                            for (int i = 0; i < BytesToBeRead / ReadLength; i++)
                            {
                                bw10.Write(io10.ReadBytes(ReadLength));
                                progressBar1.Invoke((MethodInvoker)delegate
                                {
                                    try
                                    {
                                        progressBar1.Maximum = (int)(BytesToBeRead / ReadLength);
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
                                    this.Text = "Extracting Data";
                                });
                                label1.Invoke((MethodInvoker)delegate
                                {
                                    label1.Text = (((decimal)(i + 1) / (decimal)(BytesToBeRead / ReadLength)) * 100).ToString("#") + "%";
                                });
                            }
                            //bw10.Write(io10.ReadBytes(xDrive.Length - 0x130eb0000));
                            //Close our io
                            bw10.Close();
                            break;
/**
 * Note:        End of code developed by obriej62@mail.dcu.ie
*/
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
        public void ViewJoshSector()
        {
            JoshSector.Remove(0, JoshSector.Length);
            //Create our io for the drive
            CLKsFATXLib.Streams.Reader io2 = xDrive.Reader();
            //Go to the location of the security sector
            io2.BaseStream.Position = 0x800;
            string TitleID = "";
            string TitleName = "";
            //Create our ref io for the file
            JoshSector.Append("=============================================\r\n");
            JoshSector.Append("                  Josh Sector                \r\n");
            JoshSector.Append("=============================================\r\n");
            string myValue = "";
            myValue = io2.ReadASCIIString(4);
            JoshSector.Append("Josh Sector Magic Number = " + myValue.ToString() + "\r\n");
            myValue = io2.ReadHexString(2,true);
            JoshSector.Append("Josh Sector Public Key Cert Size = " + myValue.ToString() + "\r\n");
            myValue = io2.ReadHexString(5, true);
            JoshSector.Append("Console ID Number = " + myValue.ToString() + "\r\n");
            myValue = io2.ReadASCIIString(11);
            JoshSector.Append("Console Part Number = " + myValue.ToString() + "\r\n");
            myValue = io2.ReadInt32().ToString();
            myValue = io2.ReadInt16().ToString();
            myValue = io2.ReadInt32().ToString();
            JoshSector.Append("Console Type (2 = Standard, all others = Dev kit) = " + myValue.ToString() + "\r\n");
            myValue = io2.ReadASCIIString(8);
            JoshSector.Append("Console Certificate Generation Date = " + myValue.ToString() + "\r\n");
            myValue = io2.ReadHexString(4, true);
            JoshSector.Append("PublicExponent = " + myValue.ToString() + "\r\n");
            myValue = io2.ReadHexString(128, true);
            JoshSector.Append("PublicModulus = " + myValue.ToString() + "\r\n");
            myValue = io2.ReadHexString(256, true);
            JoshSector.Append("CertificateSignature = " + myValue.ToString() + "\r\n");
            myValue = io2.ReadHexString(128, true);
            JoshSector.Append("Signature = " + myValue.ToString() + "\r\n");
            myValue = io2.ReadHexString(8, true);
            JoshSector.Append("ID Number = " + myValue.ToString() + "\r\n");
            myValue = io2.ReadHexString(20, true);
            JoshSector.Append("SHA1 Hash = " + myValue.ToString() + "\r\n");
            myValue = io2.ReadHexString(4, true);
            JoshSector.Append("Unknown Number 1 = " + myValue.ToString() + "\r\n");
            myValue = io2.ReadHexString(4, true);
            JoshSector.Append("Unknown Number 2 = " + myValue.ToString() + "\r\n");
            myValue = io2.ReadHexString(8, true);
            JoshSector.Append("Unknown Number 3 = " + myValue.ToString() + "\r\n");
            myValue = io2.ReadHexString(20, true);
            JoshSector.Append("Unknown Number 4 = " + myValue.ToString() + "\r\n");
            myValue = io2.ReadHexString(4, true);
            JoshSector.Append("Unknown Number 5 = " + myValue.ToString() + "\r\n");
            myValue = io2.ReadHexString(4, true);
            JoshSector.Append("Unknown Number 6 = " + myValue.ToString() + "\r\n");
            myValue = io2.ReadHexString(4, true);
            JoshSector.Append("Unknown Number 7 = " + myValue.ToString() + "\r\n");
            myValue = io2.ReadHexString(4, true);
            JoshSector.Append("Unknown Number 8 = " + myValue.ToString() + "\r\n");
            TitleID = io2.ReadHexString(4, true);

            for (int i = 0; i < Properties.Settings.Default.cachedID.Count; i++)
            {
                ListViewItem li = new ListViewItem(Properties.Settings.Default.cachedID[i]);
                if (li.Text == TitleID)
                {
                    ListViewItem.ListViewSubItem lsi = new ListViewItem.ListViewSubItem(li, Properties.Settings.Default.correspondingIDName[i]);
                    TitleName = lsi.Text.ToString();
                    break;
                }
                else
                {
                    TitleName = "UnKnown";
                }
                
            }

            JoshSector.Append("2nd to Last Game Played\r\n");
            JoshSector.Append("Game TitleID - " + TitleID.ToString());
            JoshSector.Append("\r\nGame TitleName - " + TitleName.ToString());
            TitleID = io2.ReadHexString(4, true);
                        for (int i = 0; i < Properties.Settings.Default.cachedID.Count; i++)
            {
                ListViewItem li = new ListViewItem(Properties.Settings.Default.cachedID[i]);
                if (li.Text == TitleID)
                {
                    ListViewItem.ListViewSubItem lsi = new ListViewItem.ListViewSubItem(li, Properties.Settings.Default.correspondingIDName[i]);
                    TitleName = lsi.Text.ToString();
                    break;
                }
                else
                {
                    TitleName = "UnKnown";
                }
                
            }
            JoshSector.Append("\r\nLast Game Played\r\n");
            JoshSector.Append("Game TitleID - " + TitleID.ToString());
            JoshSector.Append("\r\nGame TitleName - " + TitleName.ToString() + "\r\n");
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
        public void ViewSecuritySector()
        {
            SecuritySector.Remove(0, SecuritySector.Length);
            //Create our io for the drive
            CLKsFATXLib.Streams.Reader io2 = xDrive.Reader();
            //Go to the location of the security sector
            io2.BaseStream.Position = 0x2000;
            //Create our ref io for the file
            SecuritySector.Append("=============================================\r\n");
            SecuritySector.Append("              Security Sector                \r\n");
            SecuritySector.Append("=============================================\r\n");
            string myValue = "";
            myValue = io2.ReadASCIIString(20);
            SecuritySector.Append("Serial Number = " + myValue.ToString() + "\r\n");
            myValue = io2.ReadASCIIString(8);
            SecuritySector.Append("Firmware Version = " + myValue.ToString() + "\r\n");
            myValue = io2.ReadASCIIString(40);
            SecuritySector.Append("Model Number = " + myValue.ToString() + "\r\n");
            myValue = io2.ReadHexString(20, true);
            SecuritySector.Append("MS Logo Hash = " + myValue.ToString() + "\r\n");
            myValue = io2.ReadHexString(4, false);
            int SectorNumber = int.Parse(myValue, System.Globalization.NumberStyles.HexNumber);
            SecuritySector.Append("Sector Number = " + SectorNumber.ToString() + "\r\n");
            myValue = io2.ReadHexString(256, true);
            SecuritySector.Append("RSA Signature = " + myValue.ToString() + "\r\n");
            myValue = io2.ReadHexString(164, true);
            //SecuritySector.Append("Reserved = " + myValue.ToString() + "\r\n");
            int logosize = io2.ReadInt32();
            myValue = logosize.ToString();
            SecuritySector.Append("LOGO Size = " + myValue.ToString() + "Bytes\r\n");
            myValue = io2.ReadHexString(logosize, true);
            SecuritySector.Append("MS LOGO in HEX = " + myValue.ToString() + "\r\n");
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
        public void ViewDriveInfo()
        {
            //Create our io for the drive
            //Go to the location of the security sector
            //Create our ref io for the file
            myDriveInfo.Append("=============================================\r\n");
            myDriveInfo.Append("              Drive Image Details            \r\n");
            myDriveInfo.Append("=============================================\r\n");
            string myValue = "";
            myValue = xDrive.DeviceIndex.ToString();
            myDriveInfo.Append("Device Index = " + myValue.ToString() + "\r\n");
            if (xDrive.DriveHasJoshSector())
                myValue = "TRUE";
            else
                myValue = "FALSE";
            myDriveInfo.Append("Josh Sector Present = " + myValue.ToString() + "\r\n");
            if (xDrive.DriveHasSecuritySector())
                myValue = "TRUE";
            else
                myValue = "FALSE";
            myDriveInfo.Append("Security Sector Present = " + myValue.ToString() + "\r\n");
            myValue = xDrive.DriveType.ToString();
            myDriveInfo.Append("Drive Type = " + myValue.ToString() + "\r\n");
            myValue = xDrive.GetHashCode().ToString();
            myDriveInfo.Append("Drive Hash Value = " + myValue.ToString() + "\r\n");
            myValue = xDrive.LengthFriendly.ToString();
            myDriveInfo.Append("Drive Size = " + myValue.ToString() + "\r\n");
            myValue = xDrive.RemainingSpace().ToString();
            myDriveInfo.Append("Free Space = " + myValue.ToString() + " Bytes\r\n");
            myValue = xDrive.FilePath.ToString();
            myDriveInfo.Append("Image Path = " + myValue.ToString() + "\r\n");
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
        public void ViewSTFSFile()
        {
            //byte[] myByteArray;
            string myValue = "";
            int result = 0;

                SFTSFileName = xFile.Name.ToString();
                mySTFSFile.Remove(0, mySTFSFile.Length);
                CLKsFATXLib.Streams.Reader io2 = new CLKsFATXLib.Streams.Reader(xFile.GetStream());
                if (xFile.IsSTFSPackage())
                {
                    io2.BaseStream.Position = (long)CLKsFATXLib.Geometry.STFSOffsets.Magic;
                    myValue = io2.ReadASCIIString(4);
                    result = myValue.CompareTo("CON ");
                    if (result == 0)
                    {
                        mySTFSFile.Append("=============================================\r\n");
                        mySTFSFile.Append("           CON FILE HEADER DETAILS           \r\n");
                        mySTFSFile.Append("=============================================\r\n");


                        mySTFSFile.Append("CON FILE Magic Number = " + myValue.ToString() + "\r\n");
                        //char    PublicKeyCertSize[2]<format=hex>;
                        myValue = io2.ReadHexString(2, true);
                        mySTFSFile.Append("CON FILE Public Key Cert Size = " + myValue.ToString() + "\r\n");
                        //char    CertificateOwnerConsoleID[5]<format=hex>;
                        myValue = io2.ReadHexString(5, true);
                        mySTFSFile.Append("Certificate Owner Console ID = " + myValue.ToString() + "\r\n");
                        //char    CertificateOwnerConsolePartNumber[20];
                        myValue = io2.ReadASCIIString(20);
                        myValue = myValue.Replace("\0", string.Empty);
                        mySTFSFile.Append("Certificate Owner Console PartNumber = " + myValue.ToString() + "\r\n");
                        //char    CertificateOwnerConsoleType[1]<format=hex>;
                        myValue = io2.ReadHexString(1, true);
                        mySTFSFile.Append("Certificate Owner Console Type (2 = Standard, all others = Dev kit) = " + myValue.ToString() + "\r\n");
                        //char    CertificateDateofGeneration[8];
                        myValue = io2.ReadASCIIString(8);
                        mySTFSFile.Append("Certificate Date of Generation = " + myValue.ToString() + "\r\n");
                        //char    PublicExponent[4]<format=hex>;
                        myValue = io2.ReadHexString(4, true);
                        mySTFSFile.Append("PublicExponent = " + myValue.ToString() + "\r\n");
                        //char    PublicModulus[128]<format=hex>;
                        myValue = io2.ReadHexString(128, true);
                        mySTFSFile.Append("PublicModulus = " + myValue.ToString() + "\r\n");
                        //char    CertificateSignature[256]<format=hex>;
                        myValue = io2.ReadHexString(256, true);
                        mySTFSFile.Append("CertificateSignature = " + myValue.ToString() + "\r\n");
                        //char    Signature[128]<format=hex>;
                        myValue = io2.ReadHexString(128, true);
                        mySTFSFile.Append("Signature = " + myValue.ToString() + "\r\n");
                    }
                    else
                    {
                        mySTFSFile.Append("=============================================\r\n");
                        mySTFSFile.Append("           " + myValue + " FILE HEADER DETAILS           \r\n");
                        mySTFSFile.Append("=============================================\r\n");
                        mySTFSFile.Append(myValue + " FILE Magic Number = " + myValue.ToString() + "\r\n");
                        myValue = io2.ReadHexString(256, true);
                        mySTFSFile.Append("Package Signature = " + myValue.ToString() + "\r\n");

                    }

                    io2.BaseStream.Position = (long)CLKsFATXLib.Geometry.STFSOffsets.LicenceEntries;
                    mySTFSFile.Append("=============================================\r\n");
                    mySTFSFile.Append("               METADATA DETAILS              \r\n");
                    mySTFSFile.Append("=============================================\r\n");
                    //    char    LicenceEntries[256]<format=hex>;
                    myValue = io2.ReadHexString(256, true);
                    myValue = myValue.Replace("00", string.Empty);
                    mySTFSFile.Append("Licence Entries = " + myValue.ToString() + "\r\n");
                    //char    HeaderSHA1Hash[20];
                    myValue = io2.ReadHexString(20, true);
                    mySTFSFile.Append("SHA1 Hash = " + myValue.ToString() + "\r\n");
                    //uint    HeaderSize;
                    mySTFSFile.Append("Header Size = " + io2.ReadUInt32().ToString() + "\r\n");
                    //int     ContentType;
                    int ContentType = io2.ReadInt32();
                    myValue = ContentTypeID2Name(ContentType);
                    mySTFSFile.Append("Content Type = " + myValue.ToString() + "\r\n");
                    //int     MetadataVersion; 
                    mySTFSFile.Append("Metadata Version = " + io2.ReadInt32().ToString() + "\r\n");
                    //quad    ContentSize;
                    mySTFSFile.Append("Content Size = " + io2.ReadInt64().ToString() + "\r\n");
                    //uint    MediaID;
                    mySTFSFile.Append("Media ID = " + io2.ReadUInt32().ToString() + "\r\n");
                    //int     Version;
                    mySTFSFile.Append("Version = " + io2.ReadInt32().ToString() + "\r\n");
                    //int     BaseVersion;
                    mySTFSFile.Append("Base Version = " + io2.ReadInt32().ToString() + "\r\n");
                    //uint    TitleID;
                    mySTFSFile.Append("Title ID = " + io2.ReadUInt32().ToString() + "\r\n");
                    //char    Platform[1]<format=hex>;
                    myValue = io2.ReadHexString(1, true);
                    mySTFSFile.Append("Platform = " + myValue.ToString() + "\r\n");
                    //char    ExeType[1];
                    myValue = io2.ReadHexString(1, true);
                    mySTFSFile.Append("Exe Type = " + myValue.ToString() + "\r\n");
                    //char    DiskNumber[1];
                    myValue = io2.ReadHexString(1, true);
                    mySTFSFile.Append("Disk Number = " + myValue.ToString() + "\r\n");
                    //char    DiscInSet[1];
                    myValue = io2.ReadHexString(1, true);
                    mySTFSFile.Append("Disc In Set = " + myValue.ToString() + "\r\n");
                    //UINT    SaveGameID;
                    mySTFSFile.Append("SaveGameID = " + io2.ReadUInt32().ToString() + "\r\n");
                    //char    ConsoleID[5]<format=hex>;
                    myValue = io2.ReadHexString(5, true);
                    mySTFSFile.Append("Console ID = " + myValue.ToString() + "\r\n");
                    //char    ProfileID[8]<format=hex>;
                    myValue = io2.ReadHexString(8, true);
                    mySTFSFile.Append("Profile ID = " + myValue.ToString() + "\r\n");
                    //char    FSVolumeDescriptor[36];
                    myValue = io2.ReadHexString(36, true);
                    mySTFSFile.Append("FS Volume Descriptor = " + myValue.ToString() + "\r\n");
                    //int     DataFileCount;
                    mySTFSFile.Append("Data File Count = " + io2.ReadInt32().ToString() + "\r\n");
                    //quad    DataFileCombinesSize;
                    mySTFSFile.Append("Data File Combines Size = " + io2.ReadInt64().ToString() + "\r\n");


                    //char    DeviceID[20];
                    if (result == 0)
                    {
                        io2.BaseStream.Position = (long)CLKsFATXLib.Geometry.STFSOffsets.DeviceID;
                        myValue = io2.ReadASCIIString(20);
                        mySTFSFile.Append("Device ID = " + myValue.ToString() + "\r\n");
                    }
                    //char    DisplayName[2304];   
                    io2.BaseStream.Position = (long)CLKsFATXLib.Geometry.STFSOffsets.DisplayName;
                    myValue = io2.ReadASCIIString(2304);
                    myValue = myValue.Replace("\0", string.Empty);
                    mySTFSFile.Append("Display Name = " + myValue.ToString() + "\r\n");
                    //char    DisplayDescription[2304];
                    myValue = io2.ReadASCIIString(2304);
                    myValue = myValue.Replace("\0", string.Empty);
                    mySTFSFile.Append("Display Description = " + myValue.ToString() + "\r\n");
                    //char    PublisherName[128];
                    myValue = io2.ReadASCIIString(128);
                    myValue = myValue.Replace("\0", string.Empty);
                    mySTFSFile.Append("Publisher Name = " + myValue.ToString() + "\r\n");
                    //char    TitleName[128];
                    myValue = io2.ReadASCIIString(128);
                    myValue = myValue.Replace("\0", string.Empty);
                    mySTFSFile.Append("Title Name = " + myValue.ToString() + "\r\n");
                    //char    TransferFlags[1];
                    myValue = io2.ReadHexString(1, true);
                    myValue = TransferFlag2Name(int.Parse(myValue, System.Globalization.NumberStyles.HexNumber));
                    mySTFSFile.Append("Transfer Flags = " + myValue.ToString() + "\r\n");
                    //int     ThumbnailImageSize;
                    mySTFSFile.Append("ThumbnailImageSize = " + io2.ReadInt32().ToString() + "\r\n");
                    //int     TitleThumbnailImageSize;
                    mySTFSFile.Append("Title Thumbnail Image Size = " + io2.ReadInt32().ToString() + "\r\n");
                }
                else
                {
                    io2.BaseStream.Position = (long)CLKsFATXLib.Geometry.STFSOffsets.Magic;
                    myValue = io2.ReadASCIIString(4);
                    result = myValue.CompareTo("XEX2");
                    uint OptionalHeaderCount;
                    if (result == 0)
                    {
                        mySTFSFile.Append("=============================================\r\n");
                        mySTFSFile.Append("           XEX2 FILE HEADER DETAILS           \r\n");
                        mySTFSFile.Append("=============================================\r\n");
                        mySTFSFile.Append("XEX2 FILE Magic Number = " + myValue.ToString() + "\r\n");
                        myValue = io2.ReadHexString(4, true);
                        myValue = ModuleFlags(int.Parse(myValue, System.Globalization.NumberStyles.HexNumber));
                        mySTFSFile.Append("XEX2 Moduleflags = " + myValue.ToString() + "\r\n");
                        myValue = io2.ReadUInt32().ToString();
                        mySTFSFile.Append("XEX2 PE Data Offset = " + myValue.ToString() + "\r\n");
                        myValue = io2.ReadUInt32().ToString();
                        myValue = io2.ReadUInt32().ToString();
                        mySTFSFile.Append("XEX2 Security Information Offset = " + myValue.ToString() + "\r\n");
                        OptionalHeaderCount = io2.ReadUInt32();
                        myValue = OptionalHeaderCount.ToString();
                        mySTFSFile.Append("XEX2 Optional Header Count = " + myValue.ToString() + "\r\n");
                        mySTFSFile.Append("=============================================\r\n");
                        mySTFSFile.Append("     XEX2 FILE OPTIONAL HEADER DETAILS       \r\n");
                        mySTFSFile.Append("=============================================\r\n");
                        uint myHeaderID;
                        uint DataSize;
                        uint OffsetNumber;
                        int DataRead;
                        long CurrentPosition;
                        for (uint i = 0; i < OptionalHeaderCount; i++)
                        {
                            mySTFSFile.Append("Optional Header Number " + i.ToString() + "\r\n");
                            myHeaderID = io2.ReadUInt32();
                            DataSize = myHeaderID & 0xFF;
                            myValue = myHeaderID.ToString();
                            //myValue = HeaderID(myHeaderID);
                            mySTFSFile.Append("XEX2 Header ID = " + myValue.ToString() + "\r\nXEX2 Header Name =  " + HeaderID(myHeaderID).ToString() + "\r\n");
                            OffsetNumber = io2.ReadUInt32();
                            myValue = OffsetNumber.ToString();
                            if (DataSize == 1)
                            {
                                mySTFSFile.Append("XEX2 Data = " + myValue.ToString() + "\r\n");
                            }
                            else if (DataSize == 255)
                            {
                                mySTFSFile.Append("XEX2 Data Offset = " + OffsetNumber.ToString("X") + "\r\n");
                                CurrentPosition = io2.BaseStream.Position;
                                io2.BaseStream.Position = (long)OffsetNumber;
                                DataRead = io2.ReadInt32();
                                mySTFSFile.Append("XEX2 Data Size = " + DataRead.ToString("X") + "\r\n");
                                myValue = io2.ReadASCIIString(DataRead);
                                myValue = myValue.Replace("\0", string.Empty);
                                mySTFSFile.Append("XEX2 Data = " + myValue.ToString() + "\r\n");
                                io2.BaseStream.Position = CurrentPosition;
                            }
                            else
                            {
                                mySTFSFile.Append("XEX2 Data Offset = " + OffsetNumber.ToString("X") + "\r\n");
                                CurrentPosition = io2.BaseStream.Position;
                                DataSize = DataSize * 4;
                                mySTFSFile.Append("XEX2 Data Size = " + DataSize.ToString("X") + "\r\n");
                                io2.BaseStream.Position = (long)OffsetNumber;
                                myValue = io2.ReadASCIIString((int)DataSize);
                                myValue = myValue.Replace("\0", string.Empty);
                                mySTFSFile.Append("XEX2 Data = " + myValue.ToString() + "\r\n");
                                io2.BaseStream.Position = CurrentPosition;

                            }

                            mySTFSFile.Append("\r\n");

                        }
                    }
                    else
                    {
                        mySTFSFile.Append("File is not a valid STFS package!");
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
        public void ViewSTFSFileStream()
        {
            string myValue = "";
            int result = 0;

            WxReader io2 = new WxReader();
            System.IO.Stream mySTFTStream = System.IO.File.Open(myFileName, System.IO.FileMode.Open);
            myBinaryReader = new System.IO.BinaryReader(mySTFTStream);

            if (!isSTFSFile)
            {
                //myBinaryReader..Position = (long)CLKsFATXLib.Geometry.STFSOffsets.Magic;
                myBinaryReader.BaseStream.Seek((long)CLKsFATXLib.Geometry.STFSOffsets.Magic, System.IO.SeekOrigin.Begin);
                myValue = io2.readString(myBinaryReader, 4);
                result = myValue.CompareTo("CON ");
                if (result == 0)
                {
                    mySTFSFile.Append("=============================================\r\n");
                    mySTFSFile.Append("           CON FILE HEADER DETAILS           \r\n");
                    mySTFSFile.Append("=============================================\r\n");


                    mySTFSFile.Append("CON FILE Magic Number = " + myValue.ToString() + "\r\n");
                    //char    PublicKeyCertSize[2]<format=hex>;
                    myValue = io2.readString(myBinaryReader, 2);
                    mySTFSFile.Append("CON FILE Public Key Cert Size = " + myValue.ToString() + "\r\n");
                    //char    CertificateOwnerConsoleID[5]<format=hex>;
                    myValue = io2.readString(myBinaryReader, 5);
                    mySTFSFile.Append("Certificate Owner Console ID = " + myValue.ToString() + "\r\n");
                    //char    CertificateOwnerConsolePartNumber[20];
                    myValue = io2.readString(myBinaryReader, 20);
                    myValue = myValue.Replace("\0", string.Empty);
                    mySTFSFile.Append("Certificate Owner Console PartNumber = " + myValue.ToString() + "\r\n");
                    //char    CertificateOwnerConsoleType[1]<format=hex>;
                    myValue = io2.readString(myBinaryReader, 1);
                    mySTFSFile.Append("Certificate Owner Console Type (2 = Standard, all others = Dev kit) = " + myValue.ToString() + "\r\n");
                    //char    CertificateDateofGeneration[8];
                    myValue = io2.readString(myBinaryReader, 8);
                    mySTFSFile.Append("Certificate Date of Generation = " + myValue.ToString() + "\r\n");
                    //char    PublicExponent[4]<format=hex>;
                    myValue = io2.readString(myBinaryReader, 4);
                    mySTFSFile.Append("PublicExponent = " + myValue.ToString() + "\r\n");
                    //char    PublicModulus[128]<format=hex>;
                    myValue = io2.readString(myBinaryReader, 128);
                    mySTFSFile.Append("PublicModulus = " + myValue.ToString() + "\r\n");
                    //char    CertificateSignature[256]<format=hex>;
                    myValue = io2.readString(myBinaryReader, 256);
                    mySTFSFile.Append("CertificateSignature = " + myValue.ToString() + "\r\n");
                    //char    Signature[128]<format=hex>;
                    myValue = io2.readString(myBinaryReader, 128);
                    mySTFSFile.Append("Signature = " + myValue.ToString() + "\r\n");
                }
                else
                {
                    mySTFSFile.Append("=============================================\r\n");
                    mySTFSFile.Append("           " + myValue + " FILE HEADER DETAILS           \r\n");
                    mySTFSFile.Append("=============================================\r\n");
                    mySTFSFile.Append(myValue + " FILE Magic Number = " + myValue.ToString() + "\r\n");
                    myValue = io2.readString(myBinaryReader, 256);
                    mySTFSFile.Append("Package Signature = " + myValue.ToString() + "\r\n");

                }

                myBinaryReader.BaseStream.Seek((long)CLKsFATXLib.Geometry.STFSOffsets.LicenceEntries, System.IO.SeekOrigin.Begin);
                mySTFSFile.Append("=============================================\r\n");
                mySTFSFile.Append("               METADATA DETAILS              \r\n");
                mySTFSFile.Append("=============================================\r\n");
                //    char    LicenceEntries[256]<format=hex>;
                myValue = io2.readString(myBinaryReader, 256);
                myValue = myValue.Replace("00", string.Empty);
                mySTFSFile.Append("Licence Entries = " + myValue.ToString() + "\r\n");
                //char    HeaderSHA1Hash[20];
                myValue = io2.readString(myBinaryReader, 20);
                mySTFSFile.Append("SHA1 Hash = " + myValue.ToString() + "\r\n");
                //uint    HeaderSize;
                mySTFSFile.Append("Header Size = " + io2.readUInt32(myBinaryReader).ToString() + "\r\n");
                //int     ContentType;
                int ContentType = io2.readInt32(myBinaryReader);
                myValue = ContentTypeID2Name(ContentType);
                mySTFSFile.Append("Content Type = " + myValue.ToString() + "\r\n");
                //int     MetadataVersion; 
                mySTFSFile.Append("Metadata Version = " + io2.readInt32(myBinaryReader).ToString() + "\r\n");
                //quad    ContentSize;
                mySTFSFile.Append("Content Size = " + io2.readString(myBinaryReader, 16) + "\r\n");
                //uint    MediaID;
                mySTFSFile.Append("Media ID = " + io2.readInt32(myBinaryReader).ToString() + "\r\n");
                //int     Version;
                mySTFSFile.Append("Version = " + io2.readInt32(myBinaryReader).ToString() + "\r\n");
                //int     BaseVersion;
                mySTFSFile.Append("Base Version = " + io2.readInt32(myBinaryReader).ToString() + "\r\n");
                //uint    TitleID;
                mySTFSFile.Append("Title ID = " + io2.readUInt32(myBinaryReader).ToString() + "\r\n");
                //char    Platform[1]<format=hex>;
                myValue = io2.readString(myBinaryReader, 1);
                mySTFSFile.Append("Platform = " + myValue.ToString() + "\r\n");
                //char    ExeType[1];
                myValue = io2.readString(myBinaryReader, 1);
                mySTFSFile.Append("Exe Type = " + myValue.ToString() + "\r\n");
                //char    DiskNumber[1];
                myValue = io2.readString(myBinaryReader, 1);
                mySTFSFile.Append("Disk Number = " + myValue.ToString() + "\r\n");
                //char    DiscInSet[1];
                myValue = io2.readString(myBinaryReader, 1);
                mySTFSFile.Append("Disc In Set = " + myValue.ToString() + "\r\n");
                //UINT    SaveGameID;
                mySTFSFile.Append("SaveGameID = " + io2.readUInt32(myBinaryReader).ToString() + "\r\n");
                //char    ConsoleID[5]<format=hex>;
                myValue = io2.readString(myBinaryReader, 5);
                mySTFSFile.Append("Console ID = " + myValue.ToString() + "\r\n");
                //char    ProfileID[8]<format=hex>;
                myValue = io2.readString(myBinaryReader, 8);
                mySTFSFile.Append("Profile ID = " + myValue.ToString() + "\r\n");
                //char    FSVolumeDescriptor[36];
                myValue = io2.readString(myBinaryReader, 36);
                mySTFSFile.Append("FS Volume Descriptor = " + myValue.ToString() + "\r\n");
                //int     DataFileCount;
                mySTFSFile.Append("Data File Count = " + io2.readInt32(myBinaryReader).ToString() + "\r\n");
                //quad    DataFileCombinesSize;
                mySTFSFile.Append("Data File Combines Size = " + io2.readString(myBinaryReader, 16) + "\r\n");


                //char    DeviceID[20];
                if (result == 0)
                {
                    myBinaryReader.BaseStream.Seek((long)CLKsFATXLib.Geometry.STFSOffsets.DeviceID, System.IO.SeekOrigin.Begin);
                    io2.readString(myBinaryReader, 20);
                    mySTFSFile.Append("Device ID = " + myValue.ToString() + "\r\n");
                }
                //char    DisplayName[2304];   
                myBinaryReader.BaseStream.Seek((long)CLKsFATXLib.Geometry.STFSOffsets.DisplayName, System.IO.SeekOrigin.Begin);
                myValue = io2.readString(myBinaryReader, 2304);
                myValue = myValue.Replace("\0", string.Empty);
                mySTFSFile.Append("Display Name = " + myValue.ToString() + "\r\n");
                //char    DisplayDescription[2304];
                myValue = io2.readString(myBinaryReader, 2304);
                myValue = myValue.Replace("\0", string.Empty);
                mySTFSFile.Append("Display Description = " + myValue.ToString() + "\r\n");
                //char    PublisherName[128];
                myValue = io2.readString(myBinaryReader, 128);
                myValue = myValue.Replace("\0", string.Empty);
                mySTFSFile.Append("Publisher Name = " + myValue.ToString() + "\r\n");
                //char    TitleName[128];
                myValue = io2.readString(myBinaryReader, 128);
                myValue = myValue.Replace("\0", string.Empty);
                mySTFSFile.Append("Title Name = " + myValue.ToString() + "\r\n");
                //char    TransferFlags[1];
                myValue = io2.readString(myBinaryReader, 1);
                myValue = TransferFlag2Name(int.Parse(myValue, System.Globalization.NumberStyles.HexNumber));
                mySTFSFile.Append("Transfer Flags = " + myValue.ToString() + "\r\n");
                //int     ThumbnailImageSize;
                mySTFSFile.Append("ThumbnailImageSize = " + io2.readInt32(myBinaryReader).ToString() + "\r\n");
                //int     TitleThumbnailImageSize;
                mySTFSFile.Append("Title Thumbnail Image Size = " + io2.readInt32(myBinaryReader).ToString() + "\r\n");
            }
            else
            {
                myBinaryReader.BaseStream.Seek((long)CLKsFATXLib.Geometry.STFSOffsets.Magic, System.IO.SeekOrigin.Begin);
                myValue = io2.readString(myBinaryReader, 4);
                result = myValue.CompareTo("XEX2");
                uint OptionalHeaderCount;
                if (result == 0)
                {
                    mySTFSFile.Append("=============================================\r\n");
                    mySTFSFile.Append("           XEX2 FILE HEADER DETAILS           \r\n");
                    mySTFSFile.Append("=============================================\r\n");
                    mySTFSFile.Append("XEX2 FILE Magic Number = " + myValue.ToString() + "\r\n");
                    myValue = io2.readString(myBinaryReader, 4);
                    //myValue = ModuleFlags(int.Parse(myValue, System.Globalization.NumberStyles.HexNumber));
                    mySTFSFile.Append("XEX2 Moduleflags = " + myValue.ToString() + "\r\n");
                    myValue = io2.readInt32(myBinaryReader).ToString();
                    mySTFSFile.Append("XEX2 PE Data Offset = " + myValue.ToString() + "\r\n");
                    myValue = io2.readInt32(myBinaryReader).ToString();
                    myValue = io2.readInt32(myBinaryReader).ToString();
                    mySTFSFile.Append("XEX2 Security Information Offset = " + myValue.ToString() + "\r\n");
                    OptionalHeaderCount = io2.readUInt32(myBinaryReader);
                    myValue = OptionalHeaderCount.ToString();
                    mySTFSFile.Append("XEX2 Optional Header Count = " + myValue.ToString() + "\r\n");
                    mySTFSFile.Append("=============================================\r\n");
                    mySTFSFile.Append("     XEX2 FILE OPTIONAL HEADER DETAILS       \r\n");
                    mySTFSFile.Append("=============================================\r\n");
                    uint myHeaderID;
                    uint DataSize;
                    uint OffsetNumber;
                    int DataRead;
                    uint uDataRead;
                    long CurrentPosition;
                    for (uint i = 0; i < OptionalHeaderCount; i++)
                    {
                        mySTFSFile.Append("Optional Header Number " + i.ToString() + "\r\n");
                        myHeaderID = io2.readUInt32(myBinaryReader);
                        DataSize = myHeaderID & 0xFF;
                        myValue = myHeaderID.ToString();
                        //myValue = HeaderID(myHeaderID);
                        mySTFSFile.Append("XEX2 Header ID = " + myValue.ToString() + "\r\nXEX2 Header Name =  " + HeaderID(myHeaderID).ToString() + "\r\n");
                        OffsetNumber = io2.readUInt32(myBinaryReader);
                        myValue = OffsetNumber.ToString();
                        if (DataSize == 1)
                        {
                            mySTFSFile.Append("XEX2 Data = " + myValue.ToString() + "\r\n");
                        }
                        else if (DataSize == 255)
                        {
                            mySTFSFile.Append("XEX2 Data Offset = " + OffsetNumber.ToString("X") + "\r\n");
                            CurrentPosition = myBinaryReader.BaseStream.Position;

                            myBinaryReader.BaseStream.Seek((long)OffsetNumber, System.IO.SeekOrigin.Begin);
                            uDataRead = io2.readUInt32(myBinaryReader);
                            mySTFSFile.Append("XEX2 Data Size = " + uDataRead.ToString("X") + "\r\n");
                            //myValue = io2.readString(myBinaryReader, uDataRead);
                            //myValue = myValue.Replace("\0", string.Empty);
                            //mySTFSFile.Append("XEX2 Data = " + myValue.ToString() + "\r\n");
                            myBinaryReader.BaseStream.Seek(CurrentPosition, System.IO.SeekOrigin.Begin);
                        }
                        else
                        {
                            mySTFSFile.Append("XEX2 Data Offset = " + OffsetNumber.ToString("X") + "\r\n");
                            CurrentPosition = myBinaryReader.BaseStream.Position;
                            DataSize = DataSize * 4;
                            mySTFSFile.Append("XEX2 Data Size = " + DataSize.ToString("X") + "\r\n");
                            myBinaryReader.BaseStream.Seek((long)OffsetNumber, System.IO.SeekOrigin.Begin);
                            //myValue = io2.readString(myBinaryReader, DataSize);
                            //myValue = myValue.Replace("\0", string.Empty);
                            //mySTFSFile.Append("XEX2 Data = " + myValue.ToString() + "\r\n");
                            myBinaryReader.BaseStream.Seek(CurrentPosition, System.IO.SeekOrigin.Begin);

                        }

                        mySTFSFile.Append("\r\n");

                    }
                }
                else
                {
                    mySTFSFile.Append("File is not a valid STFS package!");
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
        private string HeaderID(uint myHeaderID)
        {
            string HeaderIDName = "";

            switch (myHeaderID)
            {
                case 0x2FF:
                    HeaderIDName = "Resource Info";
                    break;
                case 0x3FF:
                    HeaderIDName = "Base File Format";
                    break;
                case 0x405:
                    HeaderIDName = "Base Reference";
                    break;
                case 0x5FF:
                    HeaderIDName = "Delta Patch Descriptor";
                    break;
                case 0x80FF:
                    HeaderIDName = "Bounding Path";
                    break;
                case 0x8105:
                    HeaderIDName = "Device ID";
                    break;
                case 0x10100:
                    HeaderIDName = "Entry Point";
                    break;
                case 0x10201:
                    HeaderIDName = "Image Base Address";
                    break;
                case 0x103FF:
                    HeaderIDName = "Import Libraries";
                    break;
                case 0x18002:
                    HeaderIDName = "Checksum Timestamp";
                    break;
                case 0x18102:
                    HeaderIDName = "Enabled For Callcap";
                    break;
                case 0x18200:
                    HeaderIDName = "Enabled For Fastcap";
                    break;
                case 0x183FF:
                    HeaderIDName = "Original PE Name";
                    break;
                case 0x200FF:
                    HeaderIDName = "Static Libraries";
                    break;
                case 0x20104:
                    HeaderIDName = "TLS Info";
                    break;
                case 0x20200:
                    HeaderIDName = "Default Stack Size";
                    break;
                case 0x20301:
                    HeaderIDName = "Default Filesystem Cache Size";
                    break;
                case 0x20401:
                    HeaderIDName = "Default Heap Size";
                    break;
                case 0x28002:
                    HeaderIDName = "Page Heap Size and Flags";
                    break;
                case 0x30000:
                    HeaderIDName = "System Flags";
                    break;
                case 0x40006:
                    HeaderIDName = "Execution ID";
                    break;
                case 0x401FF:
                    HeaderIDName = "Service ID List";
                    break;
                case 0x40201:
                    HeaderIDName = "Title Workspace Size";
                    break;
                case 0x40310:
                    HeaderIDName = "Game Ratings";
                    break;
                case 0x40404:
                    HeaderIDName = "LAN Key";
                    break;
                case 0x405FF:
                    HeaderIDName = "Xbox 360 Logo";
                    break;
                case 0x406FF:
                    HeaderIDName = "Multidisc Media IDs";
                    break;
                case 0x407FF:
                    HeaderIDName = "Alternate Title IDs";
                    break;
                case 0x40801:
                    HeaderIDName = "Additional Title Memory";
                    break;
                case 0xE10402:
                    HeaderIDName = "Exports by Name";
                    break;
                default:
                    HeaderIDName = "Unknown";
                    break;
            }
            return HeaderIDName;
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
           private string ModuleFlags(int myModuleFlag)
        {
            string ModuleFlagName = "";

            switch (myModuleFlag)
            {
                case 0x00000001:
                    ModuleFlagName = "Title Module";
                    break;
                case 0x00000010:
                    ModuleFlagName = "Exports To Title";
                    break;
                case 0x00000100:
                    ModuleFlagName = "System Debugger";
                    break;
                case 0x00001000:	 
                    ModuleFlagName = "Module Patch";
                    break;
                case 0x00010000:	 
                    ModuleFlagName = "Patch Full";
                    break;
                case 0x00100000:	 
                    ModuleFlagName = "Patch Delta";
                    break;
                case 0x01000000:	 
                    ModuleFlagName = "User Mode";
                    break;
                default: 
                    ModuleFlagName = "Unknown";
                    break;
            }
            return ModuleFlagName;
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
        private string TransferFlag2Name(int TransferFlag)
        {
            string TransferFlagName = "";

            switch (TransferFlag)
            {
                case 0x00:
                    TransferFlagName = "DeviceID and ProfileID Transfer";
                    break;
                case 0x20:
                    TransferFlagName = "Move Only Transfer";
                    break;
                case 0x40:
                    TransferFlagName = "DeviceID Transfer";
                    break;
                case 0x80:	 
                    TransferFlagName = "ProfileID Transfer";
                    break;
                default: 
                    TransferFlagName = "None";
                    break;
            }
            return TransferFlagName;
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
        private string ContentTypeID2Name(int ContentTypeID)
        {
            string ContentTypeName = "";

            switch (ContentTypeID)
            {
                case 0xD0000:
                    ContentTypeName = "Arcade Title";
                    break;
                case 0x9000:
                    ContentTypeName = "Avatar Item";
                    break;
                case 0x40000:
                    ContentTypeName = "Cache File";
                    break;
                case 0x2000000:
                    ContentTypeName = "Community Game";
                    break;
                case 0x80000:
                    ContentTypeName = "Game Demo";
                    break;
                case 0x20000:
                    ContentTypeName = "Gamer Picture";
                    break;
                case 0xA0000:
                    ContentTypeName = "Game Title";
                    break;
                case 0xC0000:
                    ContentTypeName = "Game Trailer";
                    break;
                case 0x400000:
                    ContentTypeName = "Game Video";
                    break;
                case 0x4000:
                    ContentTypeName = "Installed Game";
                    break;
                case 0xB0000:
                    ContentTypeName = "Installer";
                    break;
                case 0x2000:
                    ContentTypeName = "IPTV Pause Buffer";
                    break;
                case 0xF0000:
                    ContentTypeName = "License Store";
                    break;
                case 0x2:
                    ContentTypeName = "Marketplace Content";
                    break;
                case 0x100000:
                    ContentTypeName = "Movie";
                    break;
                case 0x300000:
                    ContentTypeName = "Music Video";
                    break;
                case 0x500000:
                    ContentTypeName = "Podcast Video";
                    break;
                case 0x10000:
                    ContentTypeName = "Profile";
                    break;
                case 0x3:
                    ContentTypeName = "Publisher";
                    break;
                case 0x1:
                    ContentTypeName = "Saved Game";
                    break;
                case 0x50000:
                    ContentTypeName = "Storage Download";
                    break;
                case 0x30000:
                    ContentTypeName = "Theme";
                    break;
                case 0x200000:
                    ContentTypeName = "TV";
                    break;
                case 0x90000:
                    ContentTypeName = "Video";
                    break;
                case 0x600000:
                    ContentTypeName = "Viral Video";
                    break;
                case 0x70000:
                    ContentTypeName = "Xbox Download";
                    break;
                case 0x5000:
                    ContentTypeName = "Xbox Original Game";
                    break;
                case 0x60000:
                    ContentTypeName = "Xbox Saved Game";
                    break;
                case 0x1000:
                    ContentTypeName = "Xbox 360 Title";
                    break;
                case 0xE0000:
                    ContentTypeName = "XNA";
                    break;
                default:
                    ContentTypeName = "UnKnown";
                    break;  
            }
            return ContentTypeName;
        }
    }
}
	