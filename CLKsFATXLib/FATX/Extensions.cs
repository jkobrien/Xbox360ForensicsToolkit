using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CLKsFATXLib.Extensions
{
    static class Extensions
    {
        public static CLKsFATXLib.Entry Entry(this TreeNode node)
        {
            return (CLKsFATXLib.Entry)node.Tag;
        }

        public static string RealPath(this TreeNode node)
        {
            string FullPath = node.FullPath;
            string[] Split = FullPath.Split('\\');
            for (int i = 1; i < Split.Length; i++)
            {
                if (Split[i].Contains('|'))
                {
                    Split[i] = Split[i].Remove(Split[i].IndexOf('|') - 1);
                }
            }
            // Rebuild the path
            FullPath = "";
            for (int i = 1; i < Split.Length; i++)
            {
                if (i != Split.Length - 1)
                {
                    FullPath += Split[i] + "\\";
                }
                else
                {
                    FullPath += Split[i];
                }
            }
            return FullPath;
        }

        public static string RealNodeName(this TreeNode n)
        {
            if (n.Text.Contains('|'))
            {
                return n.Text.Remove(n.Text.IndexOf('|') - 1);
            }
            return n.Text;
        }

        public static string FriendlyNodeName(this TreeNode n)
        {
            if (n.Text.Contains('|'))
            {
                return n.Text.Remove(0, n.RealNodeName().Length + 3);
            }
            return n.Text;
        }

        public static string ToHexString(this byte[] ByteArray)
        {
            string r = "";
            for (int i = 0; i < ByteArray.Length; i++)
            {
                r += ByteArray[i].ToString("X2");
            }
            return r;
        }

        public static string ToASCIIString(this byte[] ByteArray)
        {
            string r = "";
            for (int i = 0; i < ByteArray.Length; i++)
            {
                r += ByteArray[i].ToString();
            }
            return r;
        }

        /// <summary>
        /// Rounds a number down to the nearest 0x200 byte boundary
        /// </summary>
        public static long DownToNearest200(this long val)
        {
            return (val -= (val % 0x200));
        }

        /// <summary>
        /// Rounds a number up to the nearest 0x200 byte boundary
        /// </summary>
        public static long UpToNearest200(this long val)
        {
            long valToAdd = 0x200 - (val % 0x200);
            if (valToAdd == 0x200)
            {
                return val;
            }
            return val + valToAdd;
        }

        /// <summary>
        /// Rounds a number up to the nearest cluster boundary
        /// </summary>
        public static long UpToNearestCluster(this long val, long ClusterSize)
        {
            long valToAdd = ClusterSize - (val % ClusterSize);
            if (valToAdd == ClusterSize)
            {
                return val;
            }
            return val + valToAdd;
        }

        /// <summary>
        /// Rounds a number up to the nearest cluster boundary -- doesn't pay
        /// attention to whether or not we're adding the cluster size (so
        /// if it's already on the proper boundary, it's rounded up anyway)
        /// </summary>
        public static long UpToNearestClusterForce(this long val, long ClusterSize)
        {
            long valToAdd = ClusterSize - (val % ClusterSize);
            return val + valToAdd;
        }

        /// <summary>
        /// Rounds a number down to the nearest cluster value
        /// </summary>
        public static long DownToNearestCluster(this long val, long ClusterSize)
        {
            return (val -= (val % ClusterSize));
        }

        // SoTG Caboose.
        public static void AddPath(this TreeView tree_view, String full_path)
        {
            String[] split_path;
            TreeNodeCollection current_nodes;

            if (tree_view == null)
                throw new ArgumentNullException("tree_view");
            if (String.IsNullOrEmpty(full_path))
                throw new ArgumentNullException("full_path");

            split_path = full_path.Split(tree_view.PathSeparator.ToCharArray());
            current_nodes = tree_view.Nodes;

            for (Int32 i = 0; i < split_path.Length; i++)
            {
                TreeNode[] found_nodes = current_nodes.Find(split_path[i], false);

                if (found_nodes.Length > 0)
                {
                    current_nodes = found_nodes.First().Nodes;
                }
                else
                {
                    TreeNode node;

                    node = new TreeNode();
                    node.Name = split_path[i]; // name is the same thing as key
                    node.Text = split_path[i];

                    current_nodes.Add(node);
                    current_nodes = node.Nodes;
                }
            }
        }

        public static TreeNode NodeFromPath(this TreeView tree_view, String full_path)
        {
            String[] split_path;
            TreeNodeCollection current_nodes;

            if (tree_view == null)
                throw new ArgumentNullException("tree_view");
            if (String.IsNullOrEmpty(full_path))
                throw new ArgumentNullException("full_path");

            split_path = full_path.Split(tree_view.PathSeparator.ToCharArray());
            current_nodes = tree_view.Nodes;

            for (int i = 0; i < split_path.Length; i++)
            {
                TreeNode[] found_nodes = current_nodes.Find(split_path[i], false);

                if (found_nodes.Length > 0)
                {
                    current_nodes = found_nodes.First().Nodes;
                    if (i == split_path.Length - 1)
                    {
                        return current_nodes[0];
                    }
                }
            }

            throw new Exception("Node not found");
        }
    }

    public class ListViewItemComparer : IComparer<ListViewItem>
    {

        public ListViewItemComparer()
        {
            
        }

        #region IComparer<ListViewItem> Members

        public int Compare(ListViewItem x, ListViewItem y)
        {
            return String.Compare(x.Text, y.Text);
        }

        #endregion
    }

    public class MenuItemComparer : IComparer<MenuItem>
    {

        public MenuItemComparer()
        {

        }

        #region IComparer<MenuItem> Members

        public int Compare(MenuItem x, MenuItem y)
        {
            return String.Compare(x.Text, y.Text);
        }

        #endregion
    }
}
