using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using CLKsFATXLib.Extensions;
using System.Management;

namespace CLKsFATXLib
{
    /// <summary>
    /// Provides the GetFATXDrives function for getting a list of all FATX-formatted drives connected to the computer.
    /// </summary>
    static public class StartHere
    {
        /// <summary>
        /// Gets the FATX-formatted drives connected to the computer.
        /// </summary>
        /// <returns>List of all FATX drives connected to the computer</returns>
        static public Drive[] GetFATXDrives()
        {
            // Our drive list
            List<Drive> dL = new List<Drive>();
            int Count = 10;
            try
            {
                ManagementObjectCollection drives = new ManagementObjectSearcher(
         "SELECT Caption, DeviceID FROM Win32_DiskDrive").Get();
                Count = drives.Count;
            }
            catch { Console.WriteLine("Encountered error when trying to open managed object collection: GetFATXDrives();"); }
            for (int i = 0; i < Count; i++)
            {
                Drive d = new Drive(i);
                if (d.IsFATXDrive())
                {
                    dL.Add(d);
                }
            }
            // Sort out our USB/logical drives...
            foreach (string s in Environment.GetLogicalDrives().Where(drive => System.IO.Directory.Exists(drive + "\\Xbox360")))
            {
                List<string> filePaths = new List<string>();
                for (int i = 0; i < 10000; i++)
                {
                    string extra = "";
                    if (i < 10)
                    {
                        extra = "000";
                    }
                    else if (i < 100)
                    {
                        extra = "00";
                    }
                    else if (i < 1000)
                    {
                        extra = "0";
                    }
                    if (System.IO.File.Exists(s + "\\Xbox360\\Data" + extra + i.ToString()))
                    {
                        filePaths.Add(s + "\\Xbox360\\Data" + extra + i.ToString());
                    }
                    else { break; }
                }
                if (filePaths.Count >= 3 && !IsLocked(filePaths[0]))
                {
                    Drive d = new Drive(filePaths.ToArray());
                    dL.Add(d);
                }
            }

            return dL.ToArray();
        }

        private static bool IsLocked(string FilePath)
        {
            try
            {
                using (FileStream fs = new System.IO.FileStream(FilePath, FileMode.Open))
                {
                    fs.Close();
                }
                // The file is not locked
                return false;
            }
            catch (Exception)
            {
                return true;
                // The file is locked
            }
        }
    }
}

namespace CLKsFATXLib.Streams
{

    /* I actually didn't recode these because I figured they work fine... */

    public class Writer : System.IO.BinaryWriter
    {
        public Writer(string[] ye)
            : base(new USBStream(ye, System.IO.FileMode.Open))
        {

        }

        public Writer(Stream stream)
            : base(stream)
        {

        }
    }

    public class Reader : System.IO.BinaryReader
    {
        public Reader(string[] ye)
            : base(new USBStream(ye, System.IO.FileMode.Open))
        {

        }

        public Reader(Stream stream)
            : base(stream)
        {

        }

        public Reader(string Path, System.IO.FileMode FileMode)
            : base(new System.IO.FileStream(Path, FileMode))
        {

        }

        public override void Close()
        {
            base.Close();
        }

        public ushort ReadUInt16(bool LittleEndian)
        {
            if (!LittleEndian)
            {
                byte[] buffer = ReadBytes(0x2);
                Array.Reverse(buffer);
                return BitConverter.ToUInt16(buffer, 0x0);
            }
            return base.ReadUInt16();
        }

        public int ReadInt24(bool LittleEndian)
        {
            if (!LittleEndian)
            {
                byte[] Buffer = ReadBytes(0x3);
                Buffer = new byte[]
                {
                    Buffer[2], Buffer[1], Buffer[0], 0,
                };
                return (BitConverter.ToInt32(Buffer, 0x0) >> 8);
            }
            else
            {
                byte[] Buffer = ReadBytes(0x3);
                Buffer = new byte[]
            {
                Buffer[2], Buffer[1], Buffer[0], 0,
            };
                Array.Reverse(Buffer);
                return (BitConverter.ToInt32(Buffer, 0x0) << 8);
            }
        }

        public int ReadInt24()
        {
            byte[] Buffer = ReadBytes(0x3);
            // Reverse the array, add a zero
            Buffer = new byte[]
            {
                Buffer[2], Buffer[1], Buffer[0], 0,
            };
            return (BitConverter.ToInt32(Buffer, 0x0) >> 8);
        }

        public int ReadInt32(bool LittleEndian)
        {
            if (!LittleEndian)
            {
                byte[] buffer = ReadBytes(0x4);
                Array.Reverse(buffer);
                return BitConverter.ToInt32(buffer, 0x0);
            }
            return base.ReadInt32();
        }

        public uint ReadUInt32(bool LittleEndian)
        {
            if (!LittleEndian)
            {
                byte[] Buffer = ReadBytes(0x4);
                Array.Reverse(Buffer);
                return BitConverter.ToUInt32(Buffer, 0x0);
            }
            return base.ReadUInt32();
        }

        public override uint ReadUInt32()
        {
            byte[] Buffer = ReadBytes(0x4);
            Array.Reverse(Buffer);
            return BitConverter.ToUInt32(Buffer, 0x0);
        }

        public override ushort ReadUInt16()
        {
            byte[] buffer = ReadBytes(0x2);
            Array.Reverse(buffer);
            return BitConverter.ToUInt16(buffer, 0x0);
        }

        public override int ReadInt32()
        {
            byte[] Buffer = ReadBytes(0x4);
            Array.Reverse(Buffer);
            return BitConverter.ToInt32(Buffer, 0x0);
        }

        public string ReadUnicodeString(int length)
        {
            string ss = "";
            for (int i = 0; i < length; i += 2)
            {
                char c = (char)ReadUInt16();
                if (c != '\0')
                {
                    ss += c;
                }
            }
            return ss;
        }

        public string ReadCString()
        {
            string ss = "";
            for (int i = 0; ; i += 2)
            {
                char c = (char)ReadUInt16();
                if (c != '\0')
                {
                    ss += c;
                }
                else
                {
                    break;
                }
            }
            return ss;
        }

        public string ReadASCIIString(int length)
        {
            return Encoding.ASCII.GetString(ReadBytes(length));
        }
    }

    public class USBStream : System.IO.Stream
    {
        int Current = 0;
        System.IO.Stream[] Streams;
        public USBStream(string[] filePaths, System.IO.FileMode mode)
            : base()
        {
            Streams = new FileStream[filePaths.Length];
            for (int i = 0; i < Streams.Length; i++)
            {
                Streams[i] = new FileStream(filePaths[i], mode);
            }
        }

        public USBStream(System.IO.Stream[] Streams)
            : base()
        {
            this.Streams = Streams;
        }

        public override bool CanRead
        {
            get { return Streams[Current].CanRead; }
        }

        public override bool CanSeek
        {
            get { throw new NotImplementedException(); }
        }

        public override bool CanWrite
        {
            get { return Streams[Current].CanRead; }
        }

        public override void Flush()
        {
            Streams[Current].Flush();
        }

        public override long Length
        {
            get
            {
                long length = 0;
                for (int i = 0; i < Streams.Length; i++)
                {
                    length += Streams[i].Length;
                }
                return length;
            }
        }

        public override long Position
        {
            get
            {
                // Loop through each stream before this one, and add that
                // to the return position
                long r3 = 0;
                for (int i = 0; i < Current; i++)
                {
                    // Add the length
                    r3 += Streams[i].Length;
                }
                // Add the position in our current stream
                return r3 + Streams[Current].Position;
            }
            set
            {
                // Reset the position in each stream
                for (int i = 0; i < Streams.Length; i++)
                {
                    Streams[i].Position = 0;
                }

                // Determine which stream we need to be on...
                long Remaining = value;
                for (int i = 0; i < Streams.Length; i++)
                {
                    if (Streams[i].Length < Remaining)
                    {
                        Remaining -= Streams[i].Length;
                    }
                    else
                    {
                        Current = i;
                        break;
                    }
                }

                // Check to see if we're at the end of a file...
                if (Remaining == Streams[Current].Length)
                {
                    // We were, so let's bump the current stream up one
                    Current++;
                    return;
                }
                Streams[Current].Position = Remaining;
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            // If the amount of data they're wanting to be read can be
            // read within the current stream...

            // Oh yeah, this is the position before hand... we add the
            // count to this, just to make sure that all of our position are aligned
            long bPos = Position;

            if (Streams[Current].Length - Streams[Current].Position >= count)
            {
                // We can read the data, yaaayyyy
                Streams[Current].Read(buffer, offset, count);
            }
            // We can't read it... OH NO! Gotta do some trickery
            else
            {
                // Let's declare out ints here.  First, the data we have to read,
                // then the streams that we have to read from (count), then
                // the amount of data that we can read from this current stream
                long DataLeft = count, streams = 0, DataCurrent = Streams[Current].Length - Streams[Current].Position;

                // Loop through each higher stream, getting the amount of data we can read
                // from each, and if the amount of data is still higher than the data left,
                // then loop again
                for (long i = Current + 1, Remaining = DataLeft - DataCurrent; i < Streams.Length; i++)
                {
                    // Bump up our streams
                    streams++;

                    // If the stream length is smaller than the remaining data...
                    if (Streams[i].Length >= Remaining)
                    {
                        // We can break!
                        break;
                    }
                }

                // Read our beginning data
                DataLeft -= Streams[Current].Read(buffer, offset, count);

                // Loop through each stream, reading the rest of the data
                for (int i = 0, cS = (Current + 1); i < streams; i++, cS++)
                {
                    byte[] Temp = new byte[0];
                    if (i == streams - 1)
                    {
                        Temp = new byte[DataLeft];
                    }
                    else
                    {
                        Temp = new byte[Streams[cS].Length];
                    }

                    // Read the data in to our temp array
                    Streams[cS].Read(Temp, 0, Temp.Length);

                    // Copy that in to the pointed array
                    Array.Copy(Temp, 0, buffer, count - DataLeft, Temp.Length);

                    DataLeft -= Streams[cS].Length;
                }
            }

            Position = bPos + count;

            // Return count.  Hax.
            return count;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            /* COPYPASTA FROM READ FUNCTION! */


            // If the amount of data they're wanting to be read can be
            // read within the current stream...

            // Oh yeah, this is the position before hand... we add the
            // count to this, just to make sure that all of our position are aligned
            long bPos = Position;

            if (Streams[Current].Length - Streams[Current].Position >= count)
            {
                // We can write the data, yaaayyyy
                Streams[Current].Write(buffer, offset, count);
            }
            // We can't read it... OH NO! Gotta do some trickery
            else
            {
                // Let's declare out ints here.  First, the data we have to read,
                // then the streams that we have to read from (count), then
                // the amount of data that we can read from this current stream
                long DataLeft = count, streams = 0, DataCurrent = Streams[Current].Length - Streams[Current].Position;

                // Loop through each higher stream, getting the amount of data we can read
                // from each, and if the amount of data is still higher than the data left,
                // then loop again
                for (long i = Current + 1, Remaining = DataLeft - DataCurrent; i < Streams.Length; i++)
                {
                    // Bump up our streams
                    streams++;

                    // If the stream length is smaller than the remaining data...
                    if (Streams[i].Length >= Remaining)
                    {
                        // We can break!
                        break;
                    }
                }

                // Copy the first wave of data in to a temp array
                byte[] Temp = new byte[DataCurrent];
                Array.Copy(buffer, 0, Temp, 0, Temp.Length);

                // Write our beginning data
                Streams[Current].Write(buffer, 0, Temp.Length);
                DataLeft -= Temp.Length;

                // Loop through each stream, reading the rest of the data
                for (int i = 0, cS = (Current + 1); i < streams; i++, cS++)
                {
                    Temp = new byte[0];
                    if (i == streams - 1)
                    {
                        Temp = new byte[DataLeft];
                    }
                    else
                    {
                        Temp = new byte[Streams[cS].Length];
                    }

                    Array.Copy(buffer, count - DataLeft, Temp, 0, Temp.Length);

                    // Read the data in to our temp array
                    Streams[cS].Write(Temp, 0, Temp.Length);

                    DataLeft -= Streams[cS].Length;
                }
            }

            Position = bPos + count;
        }

        public override void Close()
        {
            for (int i = 0; i < Streams.Length; i++)
            {
                Streams[i].Close();
            }
        }
    }

    public class FATXFileStream : Stream
    {
        File xFile;
        long xPositionInFile = 0;
        Stream Underlying;
        byte[] PreviouslyRead = new byte[0];
        long PreviouslyReadOffset = -1;

        public FATXFileStream(string[] InPaths, File file)
        {
            if (file.Size == 0)
            {
                throw new Exception("Null files not supported");
            }
            xFile = file;
            // Set our position to the beginning of the file
            long off = VariousFunctions.GetBlockOffset(xFile.BlocksOccupied[0], xFile);
            Underlying = new USBStream(InPaths, FileMode.Open);
            //Underlying.Position = off;
            Position = 0;
        }

        public FATXFileStream(int DeviceIndex, System.IO.FileAccess fa, File file)
        {
            //Underlying.Close();
            if (file.Size == 0)
            {
                throw new Exception("Null files not supported");
            }
            xFile = file;
            // Set our position to the beginning of the file
            long off = VariousFunctions.GetBlockOffset(xFile.BlocksOccupied[0], xFile);
            Underlying = new FileStream(VariousFunctions.CreateHandle(DeviceIndex), fa);
            //Underlying.Position = off;
            Position = 0;
        }

        public FATXFileStream(string Path, System.IO.FileMode fmode, File file)
        {
            //Underlying.Close();
            if (file.Size == 0)
            {
                throw new Exception("Null files not supported");
            }
            xFile = file;
            // Set our position
            long off = VariousFunctions.GetBlockOffset(xFile.BlocksOccupied[0], xFile);
            Underlying = new FileStream(Path, fmode);
            //Underlying.Position = off;
            Position = 0;
        }

        public FATXFileStream(System.IO.Stream Stream, File file)
        {
            if (file.Size == 0)
            {
                throw new Exception("Null files not supported");
            }
            xFile = file;
            // Set our position
            long off = VariousFunctions.GetBlockOffset(xFile.BlocksOccupied[0], xFile);
            Underlying = Stream;
            //Underlying.Position = off;
            Position = 0;
        }

        public override long Position
        {
            get
            {
                /* If we return the Underlying position, then we're returning the offset
                 * for the entire thing, not just the individual file we're trying
                 * to read*/
                return xPositionInFile;
            }
            set
            {
                if (value > xFile.Size)
                {
                    return;
                    throw new Exception("Can not read beyond end of file. Tard");
                }
                xPositionInFile = value;
                Underlying.Position = GetRealSectorOffset(value);
            }
        }

        public override void WriteByte(byte value)
        {
            Underlying.WriteByte(value);
        }

        public override void Write(byte[] array, int offset, int count)
        {
            Underlying.Write(array, offset, count);
        }

        public override long Length
        {
            get
            {
                return xFile.Size;
            }
        }

        // I dont' know why I'VariousFunctions even going to bother with this function.
        // I don't think I'VariousFunctions going to ever use it.
        public override int ReadByte()
        {
            // Check if we're at the edge of a cluster...
            if (RealOffset == VariousFunctions.UpToNearestClusterForce(RealSectorOffset, xFile.PartitionInfo.ClusterSize))
            {
                Underlying.Position = VariousFunctions.GetBlockOffset(xFile.BlocksOccupied[DetermineBlockIndex(VariousFunctions.UpToNearestClusterForce(RealSectorOffset, xFile.PartitionInfo.ClusterSize))], xFile);
                xPositionInFile++;
                return Underlying.ReadByte();
            }
            // Check if we're at the beginning of a sector...
            if (RealOffset == VariousFunctions.DownToNearest200(RealOffset))
            {
                xPositionInFile++;
                return Underlying.ReadByte();
            }
            // We aren't at the beginning of a sector, and we're not at the end of a cluster
            // We must be somewhere in-between, so we've got to do some hax.
            byte[] b = new byte[1];
            Read(b, 0, 1);
            return (int)b[0];
            // I think I made it return that first byte for some reason, but idk
            // oh yeeeuh, I wanted it to read from the nearest 0x200 byte boundary
            // so if we keep calling .ReadByte() it would have that shit cached
            // idk why i didn't do that
            int index = (int)(RealOffset - RealSectorOffset);
            if (Position.DownToNearest200() == PreviouslyReadOffset && index < PreviouslyRead.Length)
            {
                xPositionInFile++;
                return (int)PreviouslyRead[index];
            }
            else
            {
                byte[] buffer = new byte[0];
                // Read the buffer
                if (Length - Position >= 0x200)
                {
                    buffer = new byte[0x200];
                }
                else
                {
                    buffer = new byte[(Length - Position)];
                }
                index = (int)(RealOffset - RealSectorOffset);
                Read(buffer, 0, buffer.Length);
                try
                {
                    Position -= buffer.Length - 1;
                }
                catch { }
                // Set the previously read to thissssssssss
                PreviouslyRead = buffer;
                PreviouslyReadOffset = Position.DownToNearest200();
                // Return the value at the index we should be at
                return (int)buffer[index];
            }
        }

        #region Notes on these read functions

        /* So Microsoft likes to be weird and return an int for how many
         * bytes were read, vs. the array that they read... so basically
         * the array comes in as all null bytes, then comes back filled
         * in (sort of like how when you leave your drink around at a bar
         * and come back, you find it filled up by some nice guy who wants
         * to drug you and take you back to his apartment).  SO, instead of
         * loading the array at the beginning of reading, we need to load
         * everything in to a different array each time, add that to a list,
         * then finally set the byte array.  I'VariousFunctions on drugs.*/

        #endregion

        public override int Read(byte[] array, int offset, int count)
        {
            if (this.Position == this.Length)
            {
                return 0;
            }
            long p = xPositionInFile;
            // Before we do anything, we're going to check our cached buffer
            // to see if we can do anything with our previous buffer

            uint CurrentIndex = 0;
            byte[] b_Return;
            // If the number of bytes they're reading is smaller than
            // the cluster size...
            // AND
            // If they want to read a small enough amount of data to where we can
            // read the data without any trickery...

            //Creatively named WHAT becuase I have no idea what I was doing here.
            long what = VariousFunctions.UpToNearestCluster(RealOffset, xFile.PartitionInfo.ClusterSize) - RealSectorOffset;
            if (count <= xFile.PartitionInfo.ClusterSize && what >= count)
            {
                // Get the amount to remove off of the beginning of our list...
                long v_bToRemove = RealOffset - RealSectorOffset;
                // Get the amount to remove off the the end of our list
                long up = VariousFunctions.UpToNearest200(RealOffset + count);
                long v_eToRemove = up - (RealOffset + count);
                // Get the total amount of data we have to read
                long v_ToRead = VariousFunctions.UpToNearest200(v_bToRemove + v_eToRemove + count);
                // Set our return value's length
                b_Return = new byte[v_ToRead];
                // Read our shit
                Underlying.Read(b_Return, offset, (int)v_ToRead);
                // Copy our return to the original array
                Array.Copy(b_Return, v_bToRemove, array, 0x0, b_Return.Length - (v_bToRemove + v_eToRemove));
                // Clear the b_Return array
                Array.Clear(b_Return, 0, b_Return.Length);
            }
            // Else, the data they want to read spans across multiple clusters,
            // yet is less than the cluster size itself
            else
            {
                long DataRead = 0;
                /* TODO:
                 * 1.) Get the amount of data we have to read total
                 * 2.) Get the amount of data we have to read for the beginning
                 * and the end of our read
                 * 3.) Get the amount of data we have to remove off of the
                 * beginning and end of our buffer
                 * 4.) Remove that.
                 * 5.) ????*/
                // Data to remove off of the beginning
                long v_bToRemove = RealOffset - RealSectorOffset;
                //long v_eToRemove = VariousFunctions.UpToNearest200(xPositionInFile + count + v_bToRemove) - (xPositionInFile + count + v_bToRemove);
                // Data total to read...
                // Get the amount of data we can read for this beginning cluster
                long v_Cluster = VariousFunctions.UpToNearestCluster(RealSectorOffset, xFile.PartitionInfo.ClusterSize) - RealSectorOffset;
                // Get the amount of data to skim off of the end.  By doing the number rounded up to the nearest 0x200 byte boundary
                // subtracted by the non-rounded number, we are efficiently getting the difference.  What.  Why did I say efficiently
                long v_eToRemove = VariousFunctions.UpToNearest200((count - v_Cluster) + v_bToRemove) - ((count - v_Cluster) + v_bToRemove);
                // This gets the number of bytes we have to read in the final cluster.
                long v_eToReadNotRounded = ((count - v_Cluster) + v_bToRemove) - VariousFunctions.DownToNearestCluster(count - v_Cluster + v_bToRemove, xFile.PartitionInfo.ClusterSize);
                // The amount of data to read for each other cluster...
                long v_ToReadBetween = ((count - v_Cluster) + v_bToRemove) - v_eToReadNotRounded;

                b_Return = new byte[v_Cluster];
                // Read the first bit of data...
                Underlying.Read(b_Return, 0, (int)v_Cluster);
                // Copy the return buffer to the actual array
                Array.Copy(b_Return, v_bToRemove, array, 0, b_Return.Length - v_bToRemove);
                // Clear the return
                b_Return = new byte[0];
                DataRead += v_Cluster - v_bToRemove;
                Position += DataRead;

                // Loop for each cluster inbetween
                if (((count - v_Cluster) + v_bToRemove - v_eToReadNotRounded) != 0)
                {
                    for (int i = 0; i < ClusterSpanned((count - v_Cluster) + v_bToRemove - v_eToReadNotRounded); i++)
                    {
                        b_Return = new byte[xFile.PartitionInfo.ClusterSize];
                        Underlying.Read(b_Return, 0, b_Return.Length);
                        // Copy the return buffer to the actual array
                        Array.Copy(b_Return, 0, array, DataRead, b_Return.Length);
                        DataRead += xFile.PartitionInfo.ClusterSize;
                        Position += xFile.PartitionInfo.ClusterSize;
                    }
                }

                // Read our final data...
                //Underlying.Position = VariousFunctions.GetBlockOffset(xFile.BlocksOccupied[CurrentIndex + 1], xFile);
                b_Return = new byte[VariousFunctions.UpToNearest200(count - DataRead)];
                // Read the data for this final cluster
                Underlying.Read(b_Return, 0, (int)VariousFunctions.UpToNearest200(count - DataRead));
                // Copy that to the array
                Array.Copy(b_Return, 0x0, array, DataRead, b_Return.Length - v_eToRemove);
                // Clear the buffer
                b_Return = new byte[0];
            }
            //PreviouslyRead = array;
            //PreviouslyReadOffset = xPositionInFile;
            Position = p + count;
            // Just return the count.  Assholes.
            return array.Length;
        }

        long ClusterOffset
        {
            get
            {
                long cluster = VariousFunctions.DownToNearestCluster(xPositionInFile, xFile.PartitionInfo.ClusterSize);
                // Return the actual block offset + difference
                return VariousFunctions.GetBlockOffset(xFile.BlocksOccupied[DetermineBlockIndex(cluster)], xFile);
            }
        }

        long RealOffset
        {
            get
            {
                // Round the number down to the nearest cluster so that we
                // can easily get the cluster index
                long cluster = VariousFunctions.DownToNearestCluster(xPositionInFile, xFile.PartitionInfo.ClusterSize);
                uint index = (uint)(cluster / xFile.PartitionInfo.ClusterSize);
                // Get the difference so we can add it later...
                long dif = xPositionInFile - cluster;
                cluster = VariousFunctions.GetBlockOffset((uint)xFile.BlocksOccupied[index], xFile) + dif;
                // Return the actual block offset + difference
                return cluster;
            }
        }

        long RealSectorOffset
        {
            get
            {
                return GetRealSectorOffset(xPositionInFile);
            }
        }

        long GetRealSectorOffset(long off)
        {
            // Get the size up to the nearest cluster
            // Divide by cluster size
            // That is the block index.
            long SizeInCluster = VariousFunctions.DownToNearest200(off - VariousFunctions.DownToNearestCluster(off, xFile.PartitionInfo.ClusterSize));//VariousFunctions.GetBlockOffset(xFile.StartingCluster) + 0x4000;            long SizeInCluster = VariousFunctions.DownToNearestCluster(off, xFile.PartitionInfo.ClusterSize) / xFile.PartitionInfo.ClusterSize)
            uint Cluster = (uint)(VariousFunctions.DownToNearestCluster(off, xFile.PartitionInfo.ClusterSize) / xFile.PartitionInfo.ClusterSize);
            //Cluster = (Cluster == 0) ? 0 : Cluster - 1;
            try
            {
                long Underlying = VariousFunctions.GetBlockOffset(xFile.BlocksOccupied[Cluster], xFile);
                return Underlying + SizeInCluster;
            }
            catch { return VariousFunctions.GetBlockOffset(xFile.BlocksOccupied[Cluster - 1], xFile); }
        }

        uint DetermineBlockIndex(long Off)
        {
            // Pre-planning... I need to figure ref the rounded offset in order
            // to determine the cluster that this bitch is in
            // So now that we have the rounded number, we can 
            long rounded = VariousFunctions.DownToNearestCluster(Off, xFile.PartitionInfo.ClusterSize);
            // Loop for each cluster, determining if the sizes match
            for (uint i = 0; i < xFile.BlocksOccupied.Length; i++)
            {
                long off = VariousFunctions.GetBlockOffset(xFile.BlocksOccupied[i], xFile);
                if (off == rounded)
                {
                    return i;
                }
            }
            throw new Exception("Block not allocated to this file!");
        }

        // Returns the number of clusters that the value (size?) will span across
        uint ClusterSpanned(long value)
        {
            // Add the cluster size because if we don't, then upon doing this math we
            // will get the actual number - 1
            // EXAMPLE: number = 0x689 or something, and we round it down.  That number
            // is now 0, and 0/x == 0.
            long rounded = VariousFunctions.DownToNearestCluster(value, xFile.PartitionInfo.ClusterSize) + xFile.PartitionInfo.ClusterSize;
            // Divide rounded by cluster size to see how many clusters it spans across...
            return (uint)(rounded / xFile.PartitionInfo.ClusterSize);
        }

        public override void Close()
        {
            Underlying.Close();
            base.Close();
        }

        public override bool CanRead
        {
            get { return Underlying.CanRead; }
        }

        public override bool CanSeek
        {
            get { throw new NotImplementedException(); }
        }

        public override bool CanWrite
        {
            get { throw new NotImplementedException(); }
        }

        public override void Flush()
        {
            throw new NotImplementedException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }
    }
}
