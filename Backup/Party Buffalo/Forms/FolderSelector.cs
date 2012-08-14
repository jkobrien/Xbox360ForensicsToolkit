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
    public partial class FolderSelector : Form
    {
        public FolderSelector(TreeView Source, TreeNode Node)
        {
            InitializeComponent();
            treeView1.SetExplorerTheme();
            treeView1.ImageList = Source.ImageList;
            TreeNode NewNode = (TreeNode)Node.Clone();
            treeView1.Nodes.Add(NewNode);
            NewNode.Expand();
        }

        public string SelectedPath
        {
            get
            {
                return treeView1.Nodes[0].Name + "\\" + treeView1.SelectedNode.RealPath();
            }
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            textBox1.Text = treeView1.SelectedNode.Name;
            button1.Enabled = true;
        }
    }
}
