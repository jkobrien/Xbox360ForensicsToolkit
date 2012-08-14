using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32.SafeHandles;
using CLKsFATXLib.Extensions;

namespace CLKsFATXLib
{
    static public class VariousFunctions
    {
        /// <summary>
        /// Creates and returns a new handle for reading a drive
        /// </summary>
        static public SafeFileHandle CreateHandle(int deviceNumber)
        {
            // Gotta multi-line this shit so I'm not scrolling all the way across my screen
            return API.CreateFile(@"\\.\PhysicalDrive" + deviceNumber.ToString(),
                System.IO.FileAccess.ReadWrite,
                System.IO.FileShare.ReadWrite,
                IntPtr.Zero, System.IO.FileMode.Open,
                API.FlagsAndAttributes.Device | API.FlagsAndAttributes.NoBuffering | API.FlagsAndAttributes.Write_Through,
                IntPtr.Zero);
        }

        static public string[] Known = 
                {
                    "000D0000","00009000","00040000","02000000","00080000",
                    "00020000","000A0000","000C0000","00400000","00004000",
                    "000B0000","00002000","000F0000","00000002","00100000",
                    "00300000","00500000","00010000","00000003","00000001",
                    "00050000","00030000","00200000","00090000","00600000",
                    "00070000","00005000","00060000","00001000","00005000",
                    "000E0000","FFFE07D1", "00007000","00008000"
                };
        static public string[] KnownEquivilent =
                {
                    "Arcade Title","Avatar Item","Cache File","Community Game","Game Demo",
                    "Gamer Picture","Game Title","Game Trailer","Game Video","Installed Game",
                    "Installer","IPTV Pause Buffer","License Store","Marketplace Content","Movie",
                    "Music Video","Podcast Video","Profile","Publisher","Saved Game",
                    "Storage Download","Theme","TV","Video","Viral Video",
                    "Xbox Download","Xbox Original Game","Xbox Saved Game","Installed Xbox 360 Title","Xbox Title",
                    "XNA", "Xbox 360 Dashboard", "Games on Demand","Storage Pack"
                };

        /// <summary>
        /// Returns a user-friendly (KB/MB/GB) number
        /// </summary>
        static public string ByteConversion(long byteSize)
        {
            decimal size = byteSize;
            string returnVal = "";
            //There's 0x400 bytes in a kilobyte, 0x400 KB in a MB, 0x400 MB in a GB
            //if the size is below 1KB
            if ((size / 0x400) < 1)
            {
                if (size != 1)
                {
                    returnVal = size.ToString() + " bytes";
                }
                else
                {
                    returnVal = size.ToString() + " byte";
                }
            }
            //If the size is above 1KB
            if (size / 0x400 > 1)
            {
                size = size / 0x400;
                returnVal = size.ToString("#.00") + " KB";
            }

            //if the size is above 1MB
            if (size / 0x400 > 1)
            {
                size = size / 0x400;
                returnVal = size.ToString("#.00") + " MB";
            }

            //If the size is bigger than 1GB
            if (size / 0x400 > 1)
            {
                size = size / 0x400;
                returnVal = size.ToString("#.00") + " GB";
            }

            return returnVal;
        }

        /// <summary>
        /// Returns the offset within the FAT of the next block
        /// </summary>
        /// <param name="baseBlock">The root block for the entry</param>
        static public long BlockToFATOffset(uint baseBlock, Structs.PartitionInfo Partition)
        {
            if (baseBlock > Partition.Clusters)
            {
                // throw new Exception("Cluster ref of range");
            }
            long rVal = baseBlock * (int)Partition.EntrySize;
            rVal += Partition.FATOffset;
            return rVal;
        }

        public static bool IsTitleIDFolder(string Name)
        {
            // Title ID's are 8 digits long; this shit isn't a Title ID folder if it's
            // longer or shorter
            if (Name.Length != 8)
            {
                return false;
            }
            // It was 8 digits long, let's check if the characters are valid hex digits
            char[] Chars = Name.ToArray<char>();
            char[] AcceptableChars = "0123456789ABCDEFabcdef".ToArray<char>();
            for (int i = 0; i < Chars.Length; i++)
            {
                bool Acceptable = false;
                for (int j = 0; j < AcceptableChars.Length; j++)
                {
                    if (Chars[i] == AcceptableChars[j])
                    {
                        Acceptable = true;
                        break;
                    }
                }
                if (!Acceptable)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Who created the app (CLK, dawg)
        /// </summary>
        static public string About
        {
            get
            {
                return "These Xbox 360 FATX classes were coded by CLK Rebellion";
            }
        }

        static public long BlockToFATOffset(uint BaseBlock, Entry e)
        {
            return BlockToFATOffset(BaseBlock, e.PartitionInfo);
        }

        /// <summary>
        /// Returns the remaining size of a file/folder
        /// </summary>
        static public long RemainingData(File f)
        {
            long length = (long)(f.BlocksOccupied.Length - 1) * f.PartitionInfo.ClusterSize;
            return f.Size - length;
        }

        /// <summary>
        /// Returns the remaining size of a file/folder
        /// </summary>
        static public long RemainingData(int NumberOfBlocks, long FileSize, Entry e)
        {
            long length = (long)(NumberOfBlocks - 1) * e.PartitionInfo.ClusterSize;
            return FileSize - length;
        }

        /// <summary>
        /// Converts cluster (block) number to offset
        /// </summary>
        static public long GetBlockOffset(uint block, Structs.PartitionInfo Partition)
        {
            if (block > Partition.Clusters)
            {
                // throw new Exception("Cluster ref of range");
            }
            //The way that FATX works is that the root block is considered block 0,
            //so let's think about this like an array...  if the block is reading block
            //2, then it's really block 1 in an array
            block--;
            long rVal = (Partition.DataOffset + ((long)block * Partition.ClusterSize));
            return rVal;
        }

        static public uint GetBlockFromOffset(long offset, Structs.PartitionInfo Partition)
        {
            return (uint)(((offset - Partition.DataOffset)) / Partition.ClusterSize);
        }

        /// <summary>
        /// Determines whether or not we've reach the end of a chainmap, or entry listing
        /// </summary>
        static public bool EOF(byte[] buffer, bool Entry)
        {
            int NoOfFF = 0;
            for (int i = 0; i < buffer.Length; i++)
            {
                if (buffer[i] == 0xFF)
                {
                    NoOfFF++;
                }
                else if (i == buffer.Length - 1)
                {
                    if (buffer[i] == 0xF8)
                    {
                        NoOfFF++;
                    }
                }
            }

            if (NoOfFF == buffer.Length)
            {
                return true;
            }

            if (Entry)
            {
                if (buffer[0] > 0x2A && buffer[0] != 0xE5)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Converts cluster (block) number to offset
        /// </summary>
        static public long GetBlockOffset(uint block, Entry e)
        {
            return GetBlockOffset(block, e.PartitionInfo);
        }

        /// <summary>
        /// Used for quickly reading bytes for a length that is not a multiple of 0x200
        /// </summary>
        static public byte[] ReadBytes(ref Streams.Reader br, long length)
        {
            byte[] buffer = br.ReadBytes((int)VariousFunctions.UpToNearest200(length));
            List<byte> b = buffer.ToList<byte>();
            b.RemoveRange((int)length, buffer.Length - (int)length);
            buffer = b.ToArray();
            return buffer;
        }

        /* Code from...
        * http://bytes.com/topic/c-sharp/answers/248663-how-tell-if-path-file-directory
        * Too lazy to figure something ref myself.
        */
        /// <summary>
        /// Determines whether a computer path goes to a folder or file
        /// </summary>
        static public bool IsFolder(string Path)
        {
            // get the file attributes for file or directory
            System.IO.FileAttributes attr = System.IO.File.GetAttributes(Path);

            //detect whether its a directory or file
            if ((attr & System.IO.FileAttributes.Directory) == System.IO.FileAttributes.Directory)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Rounds a number down to the nearest 0x200 byte boundary
        /// </summary>
        static public long DownToNearest200(long val)
        {
            return (val -= (val % 0x200));
        }

        /// <summary>
        /// Rounds a number up to the nearest 0x200 byte boundary
        /// </summary>
        static public long UpToNearest200(long val)
        {
            long valToAdd = 0x200 - (val % 0x200);
            if (valToAdd == 0x200)
            {
                return val;
            }
            return val + valToAdd;
        }

        static public int UpToNearestGigabyte(long val)
        {
            long valToAdd = 1073741824 - (val % 1073741824);
            if (valToAdd == 1073741824)
            {
                return (int)(val / 1073741824);
            }
            return (int)((val + valToAdd) / 1073741824);
        }

        /// <summary>
        /// Rounds a number up to the nearest cluster boundary
        /// </summary>
        static public long UpToNearestCluster(long val, long ClusterSize)
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
        static public long UpToNearestClusterForce(long val, long ClusterSize)
        {
            long valToAdd = ClusterSize - (val % ClusterSize);
            return val + valToAdd;
        }

        /// <summary>
        /// Rounds a number down to the nearest cluster value
        /// </summary>
        static public long DownToNearestCluster(long val, long ClusterSize)
        {
            return (val -= (val % ClusterSize));
        }

        /// <summary>
        /// Converts a standard DateTime to a FAT int32
        /// </summary>
        static public int DateTimeToFATInt(DateTime OurDateTime)
        {
            if (OurDateTime.Year < 1980)
            {
                OurDateTime = new DateTime(1980, OurDateTime.Month, OurDateTime.Day, OurDateTime.Hour, OurDateTime.Minute, OurDateTime.Second);
            }
            int Date = ((((int)OurDateTime.Year - 1980) << 9 | ((int)OurDateTime.Month << 5) | (int)OurDateTime.Day));
            int Time = ((((int)OurDateTime.Hour << 11) | ((int)OurDateTime.Minute << 5) | ((int)OurDateTime.Second)));
            return ((Date << 16) | Time);
        }

        static public ushort DateTimeToFATShort(DateTime OurDateTime, bool Date)
        {
            if (OurDateTime.Year < 1980)
            {
                OurDateTime = new DateTime(1980, OurDateTime.Month, OurDateTime.Day, OurDateTime.Hour, OurDateTime.Minute, OurDateTime.Second);
            }
            if (Date)
            {
                return (ushort)((((ushort)OurDateTime.Year - 1980) << 9 | ((ushort)OurDateTime.Month << 5) | (ushort)OurDateTime.Day));
            }
            return (ushort)((((ushort)OurDateTime.Hour << 11) | ((ushort)OurDateTime.Minute << 5) | ((ushort)OurDateTime.Second)));
        }

        /// <summary>
        /// Converts a FAT int32 to a standard DateTime
        /// </summary>
        static public DateTime DateTimeFromFATInt(ushort date, ushort time)
        {
            try
            {
                //If they're both zero, then we don't have a date.
                if (date == 0 && time == 0)
                {
                    return DateTime.Now;
                }
                //We didn't return the date, so ye
                //We mask the upper 8 bits to get only bits 9-15, then shift them to the right 9 times to get the number
                //of years past 2000
                int year = ((date & 0xFE00) >> 9) + 1980;
                int month = (date & 0x1E0) >> 5;
                int day = (date & 0x1F);
                int hour = (time & 0xF800) >> 11;
                int minute = (time & 0x7E0) >> 5;
                int second = (time & 0x1F) * 2;
                return new DateTime(year, month, day, hour, minute, second);
            }
            catch { return DateTime.Now; }
        }

        /// <summary>
        /// Checks to make sure the file name is valid for FATX
        /// </summary>
        static public bool CheckFileName(string name)
        {
            /* I decided I'd put in a little check to see if someone is using
             * my source...  Why?  It annoys me when people lie :P
             * Not a big deal, I just like it when people tell me if they use
             * my things!*/
            if (name == "throwCLKException")
            {
                throw new Exception("Sup, using CLK's source...\r\nFrom version 2.x.x.x, which included GNU public license");
            }
            //If the string is empty, or over ...
            if (name == null)
            {
                return false;
            }
            if (name == "" || name.Length > 0x2A || name.Length == 0)
            {
                return false;
            }
            try
            {
                byte[] stringBytes = Encoding.ASCII.GetBytes(name);
                byte[] Acceptable = Encoding.ASCII.GetBytes("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!#$%&'()-.@[]^_`{}~ ");
                for (int i = 0; i < stringBytes.Length; i++)
                {
                    bool acceptable = false;
                    for (int j = 0; j < Acceptable.Length; j++)
                    {
                        if (stringBytes[i] == Acceptable[j])
                        {
                            acceptable = true;
                            break;
                        }
                    }
                    if (!acceptable)
                    {
                        return false;
                    }
                }

                return true;
            }
            catch { return false; }
        }

        static public string GetFATXPath(string PackagePath)
        {
            // Check the header...
            Streams.Reader io = new Streams.Reader(new System.IO.FileStream(PackagePath, System.IO.FileMode.Open));
            if (io.BaseStream.Length > 4)
            {
                uint header = io.ReadUInt32();
                if (header == 0x434F4E20 || header == 0x4C495645 || header == 0x50495253)
                {
                    // Get the type
                    io.BaseStream.Position = 0x344;
                    byte[] Type = io.ReadBytes(0x4);

                    // Get the profile ID
                    io.BaseStream.Position = 0x371;
                    byte[] ID = io.ReadBytes(0x8);

                    // Get the title ID
                    io.BaseStream.Position = 0x360;
                    byte[] TitleID = io.ReadBytes(0x4);

                    // NOW LET'S DO THIS SHIT
                    return string.Format("Content\\{0}\\{1}\\{2}", ID.ToHexString(), TitleID.ToHexString(), Type.ToHexString());
                }
            }

            throw new Exception(PackagePath + " is not a valid package!");
        }

        public static Geometry.Flags[] Flags(byte Flags)
        {
            List<Geometry.Flags> FL = new List<Geometry.Flags>();
            // Read bit zero, mask the rest of that shit
            for (short i = 1, j = 0; i <= 0x80; i <<= 1, j++)
            {
                if (((Flags & i) >> j) == 1)
                {
                    FL.Add((Geometry.Flags)Enum.Parse(typeof(Geometry.Flags), Enum.GetName(typeof(Geometry.Flags), j)));
                }
            }
            return FL.ToArray();
        }

        public static byte FlagsToByte(Geometry.Flags[] Flags)
        {
            byte Return = 0;
            for (int i = 0; i < Flags.Length; i++)
            {
                Return |= (byte)(1 << (int)Flags[i]);
            }
            return Return;
        }
    }
}
