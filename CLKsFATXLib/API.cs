using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using System.IO;

namespace CLKsFATXLib
{
    internal class API
    {

        #region CreateFile Function

        /// <summary>
        /// Handle for reading/writing a drive
        /// </summary>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern SafeFileHandle CreateFile(
        string lpFileName,
        FileAccess dwDesiredAccess,
        FileShare dwShareMode,
        IntPtr lpSecurityAttributes,
        FileMode dwCreationDisposition,
        FlagsAndAttributes dwFlagsAndAttributes,
        IntPtr hTemplateFile);

        //We are using win32 api in order to read the drive
        [DllImport("kernel32", CharSet = CharSet.Ansi, SetLastError = true)]
        //This is the function used to open the drive
        public static extern IntPtr CreateFile(
            String lpFileName, uint dwDesiredAccess, uint dwShareMode,
            IntPtr lpSecurityAttributes, uint dwCreationDisposition,
            uint dwFlagsAndAttributes, IntPtr hTemplateFile);

        #endregion

        #region DeviceIOControl FUnction

        //Safehandle
        /// <summary>
        /// Used for establishing a Device IO
        /// </summary>
        [DllImport("kernel32.dll")]
        private static extern bool DeviceIoControl(SafeHandle hDevice, uint dwIoControlCode,
        IntPtr lpInBuffer, uint nInBufferSize, ref DISK_GEOMETRY lpOutBuffer,
        uint nOutBufferSize, out uint lpBytesReturned, IntPtr lpOverlapped);

        #endregion

        #region CloseHandle

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int CloseHandle(IntPtr hObject);

        /// <summary>
        /// Closes the handle
        /// </summary>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int CloseHandle(SafeFileHandle hObject);

        #endregion

        #region DISK_GEOMETRY

        /// <summary>
        /// Gets disk geometry
        /// </summary>
        internal bool GetDriveGeometry(ref DISK_GEOMETRY diskGeo, int driveID, SafeFileHandle handle)
        {
            //This will serve as our output buffer
            var outBuff = IntPtr.Zero;
            //Our result
            bool bResult;
            //Dummy
            uint dummy;

            //Do our DeviceIoControl stuff
            bResult = DeviceIoControl(handle, (uint)EIOControlCode.IOCTL_DISK_GET_DRIVE_GEOMETRY, IntPtr.Zero, 0, ref diskGeo, (uint)Marshal.SizeOf(typeof(DISK_GEOMETRY)), out dummy, IntPtr.Zero);

            handle.Close();

            return (bResult);
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct DISK_GEOMETRY
        {
            long Cylinders;
            MEDIA_TYPE type;
            uint TracksPerCylinder;
            uint SectorsPerTrack;
            uint BytesPerSector;

            public long DiskSize
            {
                get
                {
                    return Cylinders * (long)TracksPerCylinder * (long)SectorsPerTrack * (long)BytesPerSector;
                }
            }
        }

        #endregion

        #region Enums

        public enum EIOControlCode : int
        {
            IOCTL_DISK_GET_DRIVE_GEOMETRY = 0x70000,
        }

        [Flags]

        public enum MEDIA_TYPE : uint
        {
            Unknown,
            F5_1Pt2_512,
            F3_1Pt44_512,
            F3_2Pt88_512,
            F3_20Pt8_512,
            F3_720_512,
            F5_360_512,
            F5_320_512,
            F5_320_1024,
            F5_180_512,
            F5_160_512,
            RemovableMedia,
            FixedMedia,
            F3_120M_512,
            F3_640_512,
            F5_640_512,
            F5_720_512,
            F3_1Pt2_512,
            F3_1Pt23_1024,
            F5_1Pt23_1024,
            F3_128Mb_512,
            F3_230Mb_512,
            F8_256_128,
            F3_200Mb_512,
            F3_240M_512,
            F3_32M_512
        }

        [Flags]
        public enum EFileAccess : uint
        {
            GenericRead = 0x80000000,
            GenericWrite = 0x40000000,
            GenericExecute = 0x20000000,
            GenericAll = 0x10000000,
        }

        [Flags]
        public enum EFileShare : uint
        {
            None = 0x00000000,
            Read = 0x00000001,
            Write = 0x00000002,
            Delete = 0x00000004,
        }

        public enum ECreationDisposition : uint
        {
            New = 1,
            CreateAlways = 2,
            OpenExisting = 3,
            OpenAlways = 4,
            TruncateExisting = 5,
        }

        [Flags]
        public enum FlagsAndAttributes : uint
        {
            Readonly = 0x00000001,
            Hidden = 0x00000002,
            System = 0x00000004,
            Directory = 0x00000010,
            Archive = 0x00000020,
            Device = 0x00000040,
            Normal = 0x00000080,
            Temporary = 0x00000100,
            SparseFile = 0x00000200,
            ReparsePoint = 0x00000400,
            Compressed = 0x00000800,
            Offline = 0x00001000,
            NotContentIndexed = 0x00002000,
            Encrypted = 0x00004000,
            Write_Through = 0x80000000,
            Overlapped = 0x40000000,
            NoBuffering = 0x20000000,
            MiscAccess = 0x10000000,
            SequentialScan = 0x08000000,
            DeleteOnClose = 0x04000000,
            BackupSemantics = 0x02000000,
            PosixSemantics = 0x01000000,
            OpenReparsePoint = 0x00200000,
            OpenNoRecall = 0x00100000,
            FirstPipeInstance = 0x00080000
        }
        #endregion
    }
}
