using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CLKsFATXLib.Structs;

namespace Party_Buffalo.Forms
{
    public partial class PartitionGeometry : UserControl
    {
        string text = "";
        public PartitionGeometry(CLKsFATXLib.Folder e)
        {
            InitializeComponent();
            CLKsFATXLib.Structs.PartitionInfo PI = e.PartitionInfo;
            propertyGrid1.SelectedObject = PI;
        }
    }
}
