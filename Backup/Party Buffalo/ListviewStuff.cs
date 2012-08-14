using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Windows.Forms;

namespace Party_Buffalo
{
    /// <summary>
    /// This class is an implementation of the 'IComparer' interface.
    /// </summary>
    public class ListViewColumnSorter : IComparer
    {
        /// <summary>
        /// Specifies the column to be sorted
        /// </summary>
        private int ColumnToSort;
        /// <summary>
        /// Specifies the order in which to sort (i.e. 'Ascending').
        /// </summary>
        private SortOrder OrderOfSort;
        /// <summary>
        /// Case insensitive comparer object
        /// </summary>
        private CaseInsensitiveComparer ObjectCompare;

        /// <summary>
        /// Class constructor.  Initializes various elements
        /// </summary>
        public ListViewColumnSorter()
        {
            // Initialize the column to '0'
            ColumnToSort = 0;

            // Initialize the sort order to 'none'
            OrderOfSort = SortOrder.None;

            // Initialize the CaseInsensitiveComparer object
            ObjectCompare = new CaseInsensitiveComparer();
        }

        /// <summary>
        /// This method is inherited from the IComparer interface.  It compares the two objects passed using a case insensitive comparison.
        /// </summary>
        /// <param name="x">First object to be compared</param>
        /// <param name="y">Second object to be compared</param>
        /// <returns>The result of the comparison. "0" if equal, negative if 'x' is less than 'y' and positive if 'x' is greater than 'y'</returns>
        public int Compare(object x, object y)
        {
            int compareResult;
            ListViewItem listviewX, listviewY;

            // Cast the objects to be compared to ListViewItem objects
            listviewX = (ListViewItem)x;
            listviewY = (ListViewItem)y;

            try
            {
                compareResult = DateTime.Compare(DateTime.Parse(listviewX.SubItems[ColumnToSort].Text), DateTime.Parse(listviewY.SubItems[ColumnToSort].Text));
            }
            catch
            {
                // Compare the two items
                compareResult = ObjectCompare.Compare(listviewX.SubItems[ColumnToSort].Text, listviewY.SubItems[ColumnToSort].Text);
            }

            // Calculate correct return value based on object comparison
            if (OrderOfSort == SortOrder.Ascending)
            {
                // Ascending sort is selected, return normal result of compare operation
                return compareResult;
            }
            else if (OrderOfSort == SortOrder.Descending)
            {
                // Descending sort is selected, return negative result of compare operation
                return (-compareResult);
            }
            else
            {
                // Return '0' to indicate they are equal
                return 0;
            }
        }

        /// <summary>
        /// Gets or sets the number of the column to which to apply the sorting operation (Defaults to '0').
        /// </summary>
        public int SortColumn
        {
            set
            {
                ColumnToSort = value;
            }
            get
            {
                return ColumnToSort;
            }
        }

        /// <summary>
        /// Gets or sets the order of sorting to apply (for example, 'Ascending' or 'Descending').
        /// </summary>
        public SortOrder Order
        {
            set
            {
                OrderOfSort = value;
            }
            get
            {
                return OrderOfSort;
            }
        }

    }

    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public static class ListViewExtensions
    {
        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
        private struct LVCOLUMN
        {
            public Int32 mask;
            public Int32 cx;
            [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPTStr)]
            public string pszText;
            public IntPtr hbm;
            public Int32 cchTextMax;
            public Int32 fmt;
            public Int32 iSubItem;
            public Int32 iImage;
            public Int32 iOrder;
        }

        private const Int32 HDI_FORMAT = 0x4;
        private const Int32 HDF_SORTUP = 0x400;
        private const Int32 HDF_SORTDOWN = 0x200;
        private const Int32 LVM_GETHEADER = 0x101f;
        private const Int32 HDM_GETITEM = 0x120b;
        private const Int32 HDM_SETITEM = 0x120c;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [System.Runtime.InteropServices.DllImport("user32.dll", EntryPoint = "SendMessage")]
        private static extern IntPtr SendMessageLVCOLUMN(IntPtr hWnd, Int32 Msg, IntPtr wParam, ref LVCOLUMN lPLVCOLUMN);

        public static void SetSortIcon(this System.Windows.Forms.ListView ListViewControl, int ColumnIndex, System.Windows.Forms.SortOrder Order)
        {
            IntPtr ColumnHeader = SendMessage(ListViewControl.Handle, LVM_GETHEADER, IntPtr.Zero, IntPtr.Zero);

            for (int ColumnNumber = 0; ColumnNumber <= ListViewControl.Columns.Count - 1; ColumnNumber++)
            {
                IntPtr ColumnPtr = new IntPtr(ColumnNumber);
                LVCOLUMN lvColumn = new LVCOLUMN();
                lvColumn.mask = HDI_FORMAT;
                SendMessageLVCOLUMN(ColumnHeader, HDM_GETITEM, ColumnPtr, ref lvColumn);

                if (!(Order == System.Windows.Forms.SortOrder.None) && ColumnNumber == ColumnIndex)
                {
                    switch (Order)
                    {
                        case System.Windows.Forms.SortOrder.Ascending:
                            lvColumn.fmt &= ~HDF_SORTDOWN;
                            lvColumn.fmt |= HDF_SORTUP;
                            break;
                        case System.Windows.Forms.SortOrder.Descending:
                            lvColumn.fmt &= ~HDF_SORTUP;
                            lvColumn.fmt |= HDF_SORTDOWN;
                            break;
                    }
                }
                else
                {
                    lvColumn.fmt &= ~HDF_SORTDOWN & ~HDF_SORTUP;
                }

                SendMessageLVCOLUMN(ColumnHeader, HDM_SETITEM, ColumnPtr, ref lvColumn);
            }
        }
    }
}
