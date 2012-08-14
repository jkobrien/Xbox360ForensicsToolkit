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
    public partial class Clone : Form
    {
        CLKsFATXLib.Drive Original, Destination;
        public Clone(CLKsFATXLib.Drive Drive1, CLKsFATXLib.Drive Drive2)
        {
            InitializeComponent();
            this.Original = Drive1;
            this.Destination = Drive2;
            this.Load += new EventHandler(Clone_Load);
        }

        void Clone_Load(object sender, EventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                System.Threading.ThreadStart ts = delegate
                {
                    CLKsFATXLib.Streams.Reader OR = Original.Reader();
                    CLKsFATXLib.Streams.Writer D = Destination.Writer();
                    OR.BaseStream.Position = 0;
                    D.BaseStream.Position = 0;
                    for (long i = 0; i < Original.Length; i += 0x6000)
                    {
                        D.Write(OR.ReadBytes(0x6000));
                        progressBar1.Invoke((MethodInvoker)delegate
                        {
                            try
                            {
                                progressBar1.Maximum = (int)(Original.Length >> 4);
                                progressBar1.Value = (int)(((i >> 8) < 0) ? 0 : i >> 4);
                            }
                            catch { }
                        });
                    }
                    OR.Close();
                    D.Close();
                };
                System.Threading.Thread t = new System.Threading.Thread(ts);
                t.Start();
            }
            catch (Exception x) { MessageBox.Show(x.Message); }
        }
    }
}
