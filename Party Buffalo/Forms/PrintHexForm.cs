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
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CLKsFATXLib;
using System.Threading;

namespace Party_Buffalo.Forms
{
    public partial class PrintHexForm : Form
    {
        PropertyGrid propertyGrid1 = new PropertyGrid();
        byte[] Data;
        Stream strm;
        public PrintHexForm(CLKsFATXLib.File f)
        {
            InitializeComponent();

            strm = f.GetStream();
            if (Data != null)
                Array.Clear(Data, 0, Data.Length);
            Data = new Byte[strm.Length];
            strm.Read(Data, 0, (int)strm.Length);
            //strm.Close();
            this.Text += " " + f.Name.ToString();
            FillTextBox();
        }

        public PrintHexForm(CLKsFATXLib.Folder f)
        {
            InitializeComponent();

            this.Text += " " + f.Name.ToString();
            textBox1.Text = "Unable to Show Hex View of a Folder";
        }

        private void FillTextBox()
        {
            textBox1.Text = "";
            StringBuilder strb = new StringBuilder();
            StringBuilder text = new StringBuilder();
            char[] ch = new char[1];
            for (int x = 0; x < (512 * 4); x += 16)
            {
                text.Length = 0;
                strb.Length = 0;
                for (int y = 0; y < 16; ++y)
                {
                    if ((x + y) > (Data.Length - 1))
                        break;
                    ch[0] = (char)Data[x + y];
                    strb.AppendFormat("{0,0:X2} ", (int)ch[0]);
                    if (((int)ch[0] < 32) || ((int)ch[0] > 127))
                        ch[0] = '.';
                    text.Append(ch);
                }
                text.Append("\r\n");
                while (strb.Length < 52)
                    strb.Append(" ");
                strb.Append(text.ToString());
                textBox1.Text += strb.ToString();
            }
            textBox1.Select(0, 0);
        }
    }
}
