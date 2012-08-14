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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CLKsFATXLib;
using Extensions;

namespace Party_Buffalo.Forms
{
    public partial class FileDetailsForm : Form
    {
        PropertyGrid propertyGrid1 = new PropertyGrid();
        public string stringToPrint = "";
        public string FileNameToPrint = "";
        public FileDetailsForm()
        {
            InitializeComponent();
        }
        public FileDetailsForm(string myString, string myFileName)
        {
            InitializeComponent();
            stringToPrint = myString;
            FileNameToPrint = myFileName;   
        }
        public FileDetailsForm(string myString)
        {
            InitializeComponent();
            stringToPrint = myString;
        }

        //public FileDetailsForm(File f)
        //{
        //    InitializeComponent();
        //    xFile = f;
        //    this.Text = "Properties -- " + f.Name;
        //    if (f.IsSTFSPackage())
        //    {
        //        Forms.EntryAction ea = new Party_Buffalo.Forms.EntryAction(File);
        //        ea.STFSFile();
        //        Forms.FileDetailsForm myFileDetailsForm = new Forms.FileDetailsForm(ea.JoshSector.ToString());
        //        myFileDetailsForm.Show();
                
        //        stringToPrint =  xFile.STFSFile(xFile);
        //        textBox1.Text = stringToPrint.ToString();
        //    }

        //}

        //public FileDetailsForm(Folder f)
        //{
        //    InitializeComponent();
        //    xFolder = f;
        //    stringToPrint = f.Name + " is a Folder.";
        //    textBox1.Text = stringToPrint.ToString();

        //}

        private void FileDetailsForm_Load(object sender, EventArgs e)
        {
            int i = stringToPrint.Length;
            this.Text = this.Text + ": FileName = " + FileNameToPrint;
            textBox1.Text = stringToPrint.ToString();
        }
    }
}
