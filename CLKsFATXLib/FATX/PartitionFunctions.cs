using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CLKsFATXLib.Structs;
using CLKsFATXLib.Extensions;

namespace CLKsFATXLib
{
    /* Port from Party Buffalo 1.0 -- It worked for the most part;
     * too lazy to recode */
    internal class PartitionFunctions
    {
        Drive FATXDrive;
        int entrySize = 0;
        PartitionInfo Partition;

        /// <summary>
        /// Provides partition/FAT information
        /// </summary>
        public PartitionFunctions(Drive Drive, Folder partition)
        {
            FATXDrive = Drive;
            Partition = partition.PartitionInfo;
            ProcessBootSector();
        }

        public PartitionFunctions(Drive Drive, long partitionOffset)
        {
            FATXDrive = Drive;
            Partition.Name = "Root";
            Partition.Offset = partitionOffset;
            ProcessBootSector();
        }

        public PartitionFunctions(Drive Drive, long partitionOffset, long partitionSize)
        {
            FATXDrive = Drive;
            Partition.Name = "Root";
            Partition.Offset = partitionOffset;
            Partition.Size = partitionSize;
            ProcessBootSector();
        }

        public PartitionFunctions(Drive Drive, Geometry.HDDOffsets p)
        {
            FATXDrive = Drive;
            Partition.Name = p.ToString();
            Partition.Offset = (long)p;
            ProcessBootSector();
        }

        public PartitionFunctions(Drive Drive, Geometry.DevOffsets p)
        {
            FATXDrive = Drive;
            Partition.Name = p.ToString();
            Partition.Offset = (long)p;
            ProcessBootSector();
        }

        /// <summary>
        /// Provides partition/FAT information
        /// </summary>
        public PartitionFunctions(Entry entry)
        {
            FATXDrive = (Drive)entry.Drive;
            Partition = entry.PartitionInfo;
            //ProcessBootSector();
        }

        long cntlzw(uint val)
        {
            List<int> Consecutives = new List<int>();
            uint consec = 0;
            for (uint i = 1; i <= 0x80000000; i = i << 1)
            {
                if ((val & i) == 0)
                {
                    consec++;
                }
                else
                {
                    Consecutives.Add((int)consec);
                    consec = 0;
                }
                if (i == 0x80000000)
                {
                    break;
                }
            }
            Consecutives.Add((int)consec);

            uint highest = 0;
            for (int i = 0; i < Consecutives.Count; i++)
            {
                if (Consecutives[i] > highest)
                {
                    highest = (uint)Consecutives[i];
                }
            }
            return highest;
        }

        void ProcessBootSector()
        {
            Exception InvalidSectorsPerCluster = new Exception("FATX: found invalid sectors per cluster");
            Exception InvalidBytesPerSector = new Exception("FATX: invalid bytes per cluster for growable file partition");
            Exception TooSmallClusterSize = new Exception("FATX: found too small of cluster size");
            Exception VolumeTooSmall = new Exception("FATX: volume too small to hold the FAT");
            Exception TooManyClusters = new Exception("FATX: too many clusters");

            // r31 = r3 + 0xa8
            long r31 = 0x3A1E69E8;
            // move r5 in to r28
            long r28 = PartitionSize(); //0x0000000004000000;
            // Set r10
            long r10 = 0x1000;
            // Set r30 to sp + 0x50
            long r30 = 0;
            long SP50 = r30;
            // Set r11
            long r11 = 0x200; // Drive sector size?
            // Set r9
            long r9 = r31;
            r11 += 0xFFF;
            // Set r29
            long r29 = (r11 & 0xFFFFF000);
            r11 = 0;
            long r26 = 0;
            if (!(r29 <= 0x1000))
            {
                // Move r29
                r10 = r29;
            }
            // Load long at r28
            r11 = r28; // partition size
            long r25 = (r10 & 0xFFFFFFFF);
            if (r25 < r11)
            {
                // BRANCH
                //long r7 = 0 /* Should be SP + 0x50, but that's 0. */, r5 = 0, r6 = 0;
                long r3 = r31;
                // Call fsc map buffer here, but we're not going to
                r10 = 0;
                r9 = 0;
                r3 = 0;
                if (r3 == 0)
                {
                    r9 = 0x58544146; // FATX ("XTAF") magic
                    r11 = Magic();
                    if ((uint)r9 == Magic())
                    {
                        // Read sectors per cluster
                        r11 = SectorsPerCluster();
                        if (r11 == 0)
                        {
                            throw InvalidSectorsPerCluster;
                            return;
                        }
                        if(!(r11 <= 2) && (r11 != 0x4) && (r11 != 0x8) && (r11 != 0x20) && (r11 != 0x40) && (r11 != 0x80))
                        {
                            throw InvalidSectorsPerCluster;
                        }
                        r9 = 9; // 9 because 0x1000 / 8 = 0x200, 0x200 = sector size
                        // Shift
                        r11 = r11 << (int)r9; // cluster size
                        long r31CC = r11;
                        if (r11 == 0)
                        {
                            // what
                            r11 = 0;//r30;
                        }
                        else // Usually branches here
                        {
                            r11 = cntlzw((uint)r11);
                            r11 = 0x1F - r11;
                        }
                        long r31D1 = r11;
                        if (r26 != 0) // Usually skips this
                        {
                            r11 = r31CC;
                            if (r11 != 0x4000)
                            {
                                throw InvalidBytesPerSector;
                            }
                        }
                        r11 = PartitionID();
                        long r31160 = r11;
                        r3 = SP50; // becomes pointer to FATX partition
                        long r27 = RootDirectoryCluster();
                        // FscUnmapBuffer
                        SP50 = r30;
                        r11 = r31CC;
                        if (r11 < r29)
                        {
                            throw TooSmallClusterSize;
                        }
                        r9 = (byte)r31D1;
                        r11 = r28;
                        r11 = (r11 >> (int)r9);
                        r11++;
                        // Entry size?
                        if (r11 < 0x0000FFF0 && r26 == 0)
                        {
                            // Should branch here
                            r10 = 1;
                            r11 = ((long)r11 << 1);
                        }
                        else if (r11 >= 0x0000FFF0)
                        {
                            r10 = 2;
                            r11 = ((long)r11 << 2);
                        }
                        r11 += r29;
                        long r31D2 = r10;
                        long r8 = r29 - 1;
                        r10 = (long)r28;
                        r11--;
                        r10 = r10 - r25;
                        // And with complement
                        r11 = r11 & ~r8;
                        r11 &= 0xFFFFFFFF;
                        if (r11 >= r10)
                        {
                            throw VolumeTooSmall;
                        }
                        r10 -= r11;
                        r9 &= 0xFFFFFFFFFFFFFF;
                        r8 = 0x000000000FFFFFFF;
                        r10 >>= (int)r9;
                        long r31C8 = clusters = (uint)r10;
                        if (r10 > r8)
                        {
                            throw TooManyClusters;
                        }
                        Console.WriteLine(Partition.Offset.ToString("X") + " Clusters " + r10.ToString("X"));
                        fatsize = r11;
                        Console.WriteLine("r11 " + r11.ToString("X"));
                        r11 += r25;
                        Console.WriteLine("r11 " + r11.ToString("X"));
                        long r31D4 = r25;
                        r10 = r25 & 0xFFF;
                        Console.WriteLine("r25 " + r25.ToString("X") + "\r\nr10 " + r10.ToString("X"));
                        r11 &= 0xFFF;
                        Console.WriteLine("r11 " + r11.ToString("X"));
                        //fatsize = r11.UpToNearestCluster(r31CC);
                        r11 = r31CC;
                        Console.WriteLine("r11 " + r11.ToString("X"));
                        r11 &= 0xFFF;
                        Console.WriteLine("r11 " + r11.ToString("X"));
                    }
                }
                else
                {
                    // BRANCH
                }
            }
        }

        uint clusters
        {
            get
            {
                return Partition.Clusters;
            }
            set
            {
                Partition.Clusters = value;
            }
        }
        public uint Clusters()
        {
            //Take the partition size - data offset because that's the true amount of blocks we have
            //(otherwise we are assuming that the entire file is able to have data written to it)

            //FUCK
            //THAT
            //SHIT
            //get an infinite loop, ya dig?
            if (clusters == 0)
            {
                clusters = (uint)(PartitionSize() / (ClusterSize()));
            }
            return clusters;
        }

        public uint RootDirectoryCluster()
        {
            // Open our IO
            Streams.Reader io = FATXDrive.Reader();
            // Set the IO position...
            io.BaseStream.Position = Partition.Offset;
            // Read our buffer
            byte[] buffer = io.ReadBytes(0x200);
            // Re-open the IO in to a memory stream
            io = new Streams.Reader(new System.IO.MemoryStream(buffer));
            // Go to the offset that the root dir is located; 0xC
            io.BaseStream.Position = 0xC;
            // Read and return the int there
            uint rVal = io.ReadUInt32();
            io.Close();
            return rVal;
        }

        public int EntrySize
        {
            get
            {
                if (entrySize == 0)
                {
                    if (Partition.Offset == 0x20000000 && FATXDrive.DriveType == DriveType.USB)
                    {
                        entrySize = 4;
                    }
                    else if (Partition.Offset == (long)Geometry.HDDOffsets.System_Cache || Partition.Offset == (long)Geometry.HDDOffsets.System_Extended)
                    {
                        entrySize = 2;
                    }
                    else
                    {
                        uint blocks = Clusters();
                        if (blocks < 0xFFF0)
                        {
                            entrySize = 2;
                        }
                        else
                        {
                            entrySize = 4;
                        }
                    }
                }
                return entrySize;
            }
        }

        public long RealFATSize()
        {
            long ValToReturn = (((Partition.Offset + PartitionSize()) - DataOffset()) / ClusterSize()) * EntrySize;
            return ValToReturn;
        }

        /// <summary>
        /// Partition magicF
        /// </summary>
        uint magic = 0;
        public uint Magic()
        {
            if (magic != 0)
            {
                return magic;
            }
            Streams.Reader br = FATXDrive.Reader();
            br.BaseStream.Position = Partition.Offset;
            //Read the header
            magic = br.ReadUInt32();
            return magic;
        }

        /// <summary>
        /// Sector size (in bytes)
        /// </summary>
        public long SectorSize
        {
            get { return 0x200; }
        }

        /// <summary>
        /// Cluster size (in bytes)
        /// </summary>
        public long ClusterSize()
        {
            return SectorSize * SectorsPerCluster();
        }

        /// <summary>
        /// Partition ID
        /// </summary>
        public uint PartitionID()
        {
            uint rVal = 0;
            //Open our binary reader
            Streams.Reader br = FATXDrive.Reader();
            //Seek to the data partition offset
            br.BaseStream.Position = Partition.Offset;
            //Read our buffer
            Streams.Reader mem = new Streams.Reader(new System.IO.MemoryStream(br.ReadBytes(0x200)));
            mem.BaseStream.Position = 0x4;
            rVal = mem.ReadUInt32();
            mem.Close();
            return rVal;
        }

        /// <summary>
        /// Sectors per cluster
        /// </summary>
        public uint SectorsPerCluster()
        {
            uint rVal = 0;
            //Open our binary reader
            Streams.Reader br = FATXDrive.Reader();
            //Seek to the data partition offset + 0x8 (cluster size location)
            br.BaseStream.Position = Partition.Offset;
            //Create our mem reader / buffer
            Streams.Reader mem = new Streams.Reader(new System.IO.MemoryStream(br.ReadBytes(0x200)));
            mem.BaseStream.Position = 0x8;
            //Get our value (uses outside class for bigendian)
            rVal = mem.ReadUInt32();
                mem.Close();
            return rVal;
        }

        /// <summary>
        /// Number of File Allocation Tables
        /// </summary>
        public uint FATCopies()
        {
            uint rVal = 0;
            //Open our binary reader
            Streams.Reader br = FATXDrive.Reader();
            //Seek to the data partition offset + 0xC (where the FATCopies int is)
            br.BaseStream.Position = Partition.Offset;
            //Create our mem reader / buffer
            Streams.Reader mem = new Streams.Reader(new System.IO.MemoryStream(br.ReadBytes(0x200)));
            mem.BaseStream.Position = 0xC;
            //Get our value (uses outside class for bigendian)
            rVal = mem.ReadUInt32();
            mem.Close();
            return rVal;
        }


        long fatsize
        {
            get
            {
                return Partition.FATSize;
            }
            set
            {
                Partition.FATSize = value;
            }
        }
        bool SizeChecked = false;
        /// <summary>
        /// TOTAL size (includes padding) of the File Allocation Table (in bytes)
        /// FOR REAL SIZE, CALL TO RealFATSize();
        /// </summary>
        public long FATSize()
        {
            if (fatsize != 0)
            {
                if (!SizeChecked)
                {
                    Streams.Reader r = FATXDrive.Reader();
                    r.BaseStream.Position = Partition.Offset + fatsize + 0x1000;
                    while (true)
                    {
                        if (r.ReadUInt32() == 0x0)
                        {
                            fatsize += 0x1000;
                            r.BaseStream.Position += 0x1000 - 0x4;
                        }
                        else
                        {
                            break;
                        }
                    }
                    SizeChecked = true;
                }
                return fatsize;
            }
            if (Partition.Offset == (long)Geometry.HDDOffsets.System_Extended)
            {
                return 0x5000;
            }
            else if (Partition.Offset == (long)Geometry.HDDOffsets.System_Cache)
            {
                return 0x7000;
            }
            #region old
            //long size = 0;
            //if (Partition.Offset == 0x20000000 && FATXDrive.IsUSB)
            //{
            //    System.IO.FileInfo fi = new System.IO.FileInfo(FATXDrive.DumpPath + "\\Data0001");
            //    size = fi.Length - 0x1000;
            //    //Return the size.
            //    return size;
            //}
            //else
            //{
            //    //This gets the size
            //    size = ((PartitionSize() / ClusterSize()) * EntrySize);
            //    //We need to round up to the nearest 0x1000 byte boundary.
            //    long sizeToAdd = (0x1000 - (size % 0x1000));
            //    if (!FATXDrive.IsUSB)
            //    {
            //        size += sizeToAdd;
            //    }
            //    //Return the size.
            //    return size;
            //}
            #endregion
            //Code that rounds up to nearest cluster...
            long size = 0;
            #region shit
            //if (Partition.Offset == 0x20000000 && FATXDrive.IsUSB)
            //{
            //    System.IO.FileInfo fi = new System.IO.FileInfo(FATXDrive.DumpPath + "\\Data0001");
            //    size = fi.Length - 0x1000;
            //    //Ghetto
            //    Streams.Reader ir = FATXDrive.Reader();
            //    //Return the size.
            //    return size;
            //}
            //else
            //{
            #endregion
            //This gets the size
            size = (((PartitionSize() / ClusterSize())) * EntrySize);
            //We need to round up to the nearest blabhlabhalkhdflkasdf byte boundary.
            size = VariousFunctions.UpToNearestCluster(size + 0x1000, ClusterSize() / EntrySize) - 0x1000;
            //long sizeToAdd = (0x1000 - (size % 0x1000));
            //size += sizeToAdd;
            //Return the size.
            return size;
            //uint r24 = 0, r3, r6, r23, r10, r11, r27, r22, r25, r26, r28, r29, r30, r31;
            //if (Partition.Size == 0)
            //{

            //}
            //else
            //{
            //    r11 = r27 & 0xFF;
            //    if (r11 == 2)
            //    {
            //        r11 = r22;
            //        r10 = r11 + r29;
            //        r11--;
            //        r10--;
            //        r29 = r10 & (~r11);
            //        r30 = r29;
            //    }
            //    else
            //    {
            //        // break here
            //    }
            //    r10 = 1;
            //    r3 = dicks(ref r28, ref r26, ref r27, ref r25, ref r31, ref r24, ref r10);
            //}
        }

        //uint dicks(ref uint r3, ref uint r4, ref uint r6, ref uint r7, ref uint r8, ref uint r9, ref uint r10)
        //{
            
        //}

        public long GetFreeSpace()
        {
            // Our return
            long Return = 0;
            long ClusterSize = this.ClusterSize();

            // Get our position
            long positionya = FATOffset;

            // Get our end point
            long toBeLessThan = FATOffset + RealFATSize();

            // Get our IO
            Streams.Reader io = FATXDrive.Reader();
            // Set the position
            io.BaseStream.Position = positionya;

            // Start reading!
            for (long dick = io.BaseStream.Position; dick < toBeLessThan; dick += 0x200)
            {
                bool BreakAndShit = false;
                // Set the position
                io.BaseStream.Position = dick;
                // Read our buffer
                byte[] Buffer = null;
                if ((dick - FATOffset).DownToNearest200() == (toBeLessThan - FATOffset).DownToNearest200())
                {
                    byte[] Temp = io.ReadBytes(0x200);
                    Buffer = new byte[(toBeLessThan - FATOffset) - (dick - FATOffset).DownToNearest200()];
                    Array.Copy(Temp, 0, Buffer, 0, Buffer.Length);
                }
                else
                {
                    Buffer = io.ReadBytes(0x200);
                }
                // Length to loop for (used for the end so we can read ONLY usable partitions)
                long Length = Buffer.Length;
                if (dick == VariousFunctions.DownToNearest200(toBeLessThan))
                {
                    Length = toBeLessThan - VariousFunctions.DownToNearest200(toBeLessThan);
                    BreakAndShit = true;
                }
                // Check the values
                Streams.Reader ioya = new Streams.Reader(new System.IO.MemoryStream(Buffer));
                for (int i = 0; i < Length; i+= EntrySize)
                {
                    // This size will be off by a few megabytes, no big deal in my opinion
                    if (EntrySize == 2)
                    {
                        ushort Value = ioya.ReadUInt16();
                        if (Value == 0)
                        {
                            Return += ClusterSize;
                        }
                    }
                    else
                    {
                        if (ioya.ReadUInt32() == 0)
                        {
                            Return += ClusterSize;
                        }
                    }
                }
                ioya.Close();
                if (BreakAndShit)
                {
                    break;
                }
            }

             return Return;
        }


        /// <summary>
        /// Returns the root offset for where data begins in the partition
        /// </summary>
        public long DataOffset()
        {
            //System.Diagnostics.Debug.Assert(Partition.Offset != (long)Geometry.HDDOffsets.Data, "ye");
            //if (Partition.Offset == (long)Geometry.HDDOffsets.Data)
            //{
            //    return 0x13296F000;
            //}
            //We take the FAT start + the FAT size to get the data offset
            long off = (Partition.Offset + 0x1000 + FATSize());
            return off;
        }

        /// <summary>
        /// Offset of the File Allocation Table
        /// </summary>
        public long FATOffset
        {
            get { return Partition.Offset + 0x1000; }
        }

        /// <summary>
        /// Total size of the partition (in bytes)
        /// </summary>
        public long PartitionSize()
        {
            if (Partition.Size == 0)
            {
                long psize = 0;
                if (FATXDrive.DriveType == DriveType.Backup || FATXDrive.DriveType == DriveType.HardDisk)
                {
                    // If we're working with a dev drive
                    if (FATXDrive.IsDev)
                    {
                        // If we're on the DEVKIT? partition...
                        if (Partition.Offset == (long)Geometry.DevOffsets.DEVKIT_)
                        {
                            // This is simply for some testing purposes... shit's not permanent yet
                            psize = 0x11FFD000;
                        }
                        else
                        {
                            psize = 0x2E22DF000;
                        }
                    }
                    else
                    {
                        switch (Partition.Offset)
                        {
                            case (long)Geometry.HDDOffsets.Compatibility:
                                psize = (long)Geometry.HDDLengths.Compatibility;
                                break;
                            case (long)Geometry.HDDOffsets.System_Cache:
                                psize = (long)Geometry.HDDLengths.System_Cache;
                                break;
                            case (long)Geometry.HDDOffsets.System_Extended:
                                psize = (long)Geometry.HDDLengths.System_Extended;
                                break;
                            case (long)Geometry.HDDOffsets.Data:
                                if (FATXDrive.DriveType == DriveType.HardDisk)
                                {
                                    psize = (FATXDrive.Length - Partition.Offset);
                                }
                                else
                                {
                                    psize = (FATXDrive.Length - Partition.Offset);
                                }
                                break;
                            case (long)Geometry.HDDOffsets.GameCache:
                                psize = (long)Geometry.HDDLengths.GameCache;
                                break;
                            case (long)Geometry.HDDOffsets.SystemCache:
                                psize = (long)Geometry.HDDLengths.SystemCache;
                                break;
                        }
                    }
                }
                else
                {
                    switch (Partition.Offset)
                    {
                        case (long)Geometry.USBOffsets.Cache:
                            psize = (long)Geometry.USBPartitionSizes.Cache;
                            break;
                        case (long)Geometry.USBOffsets.aSystem_Aux:
                            psize = (long)Geometry.USBPartitionSizes.System_Aux;
                            break;
                        case (long)Geometry.USBOffsets.aSystem_Extended:
                            psize = (long)Geometry.USBPartitionSizes.System_Extended;
                            break;
                        case (long)Geometry.USBOffsets.Data:
                            psize = FATXDrive.Length - (long)Geometry.USBOffsets.Data;
                            break;
                    }
                }
                if (psize == 0)
                {
                    psize = FATXDrive.Reader().BaseStream.Length;
                }
                Partition.Size = psize;
            }
                return Partition.Size;
        }
    }

    internal class EntryFunctions
    {
        Entry Parent;
        public EntryFunctions(Entry DesiredEntry)
        {
            this.Parent = DesiredEntry;
        }

        public object[] GetEntries(Folder Parent)
        {
            List<Folder> cachedFolders = new List<Folder>();
            List<Folder> cachedDeletedFolders = new List<Folder>();
            List<File> cachedFiles = new List<File>();
            List<File> cachedDeletedFiles = new List<File>();
            EntryData[] Entries = GetEntryData(Parent);

            for (int i = 0; i < Entries.Length; i++)
            {
                EntryData newEdata = Entries[i];
                bool Deleted = false;
                if (newEdata.NameSize == 0xE5)
                {
                    // Remove the question marks and shit
                    try
                    {
                        newEdata.Name = newEdata.Name.Remove(newEdata.Name.IndexOf('?'));
                        newEdata.NameSize = (byte)newEdata.Name.Length;
                    }
                    catch { newEdata.NameSize = 0x2A; }
                    Deleted = true;
                }
                List<Geometry.Flags> FL = new List<Geometry.Flags>();
                // Read bit zero, mask the rest of that shit
                for (short s = 1, j = 0; s <= 80; s <<= 1, j++)
                {
                    if (((newEdata.Flags & s) >> j) == 1)
                    {
                        FL.Add((Geometry.Flags)Enum.Parse(typeof(Geometry.Flags), Enum.GetName(typeof(Geometry.Flags), j)));
                    }
                }
                // Folder
                if (newEdata.StartingCluster != 0 && newEdata.Size == 0 && FL.Contains(Geometry.Flags.Directory))
                {
                    Folder f = new Folder(Parent.PartitionInfo, newEdata, Parent.Drive);
                    f.FullPath = Parent.FullPath + "\\" + f.Name;
                    f.Parent = Parent;
                    if (Deleted)
                    {
                        f.IsDeleted = true;
                        cachedDeletedFolders.Add(f);
                    }
                    else
                    {
                        cachedFolders.Add(f);
                    }
                }
                    // File
                else
                {
                    File f = new File(Parent.PartitionInfo, newEdata, Parent.Drive);
                    f.FullPath = Parent.FullPath + "\\" + f.Name;
                    f.Parent = Parent;
                    if (Deleted)
                    {
                        f.IsDeleted = true;
                        cachedDeletedFiles.Add(f);
                    }
                    else
                    {
                        cachedFiles.Add(f);
                    }
                }
            }
            return new object[] { cachedFiles, cachedFolders, cachedDeletedFiles, cachedDeletedFolders };
        }

        public EntryData[] GetEntryData(Folder Parent)
        {
            List<EntryData> eList = new List<EntryData>();
            for (int i = 0; i < Parent.BlocksOccupied.Length; i++)
            {
                eList.AddRange(EntryDataFromBlock(Parent.BlocksOccupied[i]));
            }
            return eList.ToArray();
        }

        public EntryData[] EntryDataFromBlock(uint Block)
        {
            bool Break = false;
            List<EntryData> eList = new List<EntryData>();
            // Get our binary reader
            Streams.Reader r1 = Parent.Drive.Reader();
            r1.BaseStream.Position = VariousFunctions.GetBlockOffset(Block, Parent);
            /* Parent.PartitionInfo.Clusters / 0x40 / 0x8 because if each
             * entry is 0x40 in length and the cluster is filled to the
             * max with cluster entries, then we can do division to get
             * the number of entries that would be in that cluster 
             * the 0x8 part is because on drives we have to read in intervals
             * of 0x200 right?  So if Parent.PartitionInfo.Clusters / 0x40 = 0x100,
             * then that means that there are 0x100 entries per cluster...
             * divide that by 8 (the number of clusters within a 0x200 interval) and
             * that's how many shits we have to go forward */
            for (int j = 0; j < Parent.PartitionInfo.ClusterSize / 0x1000; j++)
            {
                // Increment our position
                // Open another reader using a memory stream
                long r1Position = r1.BaseStream.Position;
                Streams.Reader r = new CLKsFATXLib.Streams.Reader(new System.IO.MemoryStream(r1.ReadBytes(0x1000)));
                for (int k = 0; k < (0x1000 / 0x40); k++)
                {
                    // Check to see if we've passed the last entry...
                    uint val = r.ReadUInt32();
                    if (val == 0x0 || val == 0xFFFFFFFF)
                    {
                        Break = true;
                        break;
                    }
                    // Go back four bytes because we just checked the next four...
                    r.BaseStream.Position -= 4;
                    long StartOffset = r.BaseStream.Position;
                    EntryData e = new EntryData();
                    e.EntryOffset = r.BaseStream.Position + r1Position;
                    e.NameSize = r.ReadByte();
                    e.Flags = r.ReadByte();
                    /* Because some fucking smart guy decided to put the
                     * deleted flag in the name size field, we have to check
                     * if it's deleted or not...*/
                    if (e.NameSize == 0xE5)
                    {
                        // Fuckers
                        e.Name = Encoding.ASCII.GetString(r.ReadBytes(0x2A));
                    }
                    else
                    {
                        e.Name = Encoding.ASCII.GetString(r.ReadBytes(e.NameSize));
                    }
                    r.BaseStream.Position = StartOffset + 0x2C;
                    e.StartingCluster = r.ReadUInt32();
                    e.Size = r.ReadUInt32();
                    e.CreationDate = r.ReadUInt16();
                    e.CreationTime = r.ReadUInt16();
                    e.AccessDate = r.ReadUInt16();
                    e.AccessTime = r.ReadUInt16();
                    e.ModifiedDate = r.ReadUInt16();
                    e.ModifiedTime = r.ReadUInt16();
                    eList.Add(e);
                }
                r.Close();
                if (Break)
                {
                    break;
                }
            }
            return eList.ToArray();
        }

        public uint[] GetBlocksOccupied()
        {
            List<uint> Blocks = new List<uint>();
            Streams.Reader r = Parent.Drive.Reader();
            Blocks.Add(Parent.StartingCluster);
            byte[] Buffer = new byte[0x1000];
            int buffersize = 0x1000;
            long lastoffset = 0;
            for (int i = 0; i < Blocks.Count; i++)
            {
                r.BaseStream.Position = VariousFunctions.BlockToFATOffset(Blocks[i], Parent).DownToNearestCluster(0x1000);
                // We use this so that we aren't reading the same buffer
                // a zillion times
                if (r.BaseStream.Position != lastoffset)
                {
                    lastoffset = r.BaseStream.Position;
                    Buffer = r.ReadBytes(buffersize);
                }

                Streams.Reader r1 = new CLKsFATXLib.Streams.Reader(new System.IO.MemoryStream(Buffer));
                int OffsetInBuffer = (int)(VariousFunctions.BlockToFATOffset(Blocks[i], Parent) - VariousFunctions.BlockToFATOffset(Blocks[i], Parent).DownToNearestCluster(0x1000));
                r1.BaseStream.Position = OffsetInBuffer;
                switch (Parent.PartitionInfo.EntrySize)
                {
                    case 2:
                        ushort Value = r1.ReadUInt16();
                        if (Value != 0xFFFF && Value != 0xFFF8)
                        {
                            if (Value == 0)
                            {
                                throw new Exception("Bad FAT chain in file or folder " + Parent.FullPath + "\r\nEntry Offset: 0x" + Parent.EntryOffset.ToString("X"));
                            }
                            Blocks.Add(Value);
                        }
                        break;
                    case 4:
                        uint Value2 = r1.ReadUInt32();
                        if (Value2 != 0xFFFFFFFF && Value2 != 0xFFFFFFF8)
                        {
                            if (Value2 == 0)
                            {
                                throw new Exception("Bad FAT chain in file or folder " + Parent.FullPath + "\r\nEntry Offset: 0x" + Parent.EntryOffset.ToString("X"));
                            }
                            Blocks.Add(Value2);
                        }
                        break;
                }
                r1.Close();
            }
            return Blocks.ToArray();
        }

        /* AUTOMATICALLY CREATES SHIT */
        public EntryData GetNewEntry(Folder Destination, uint Size, Geometry.Flags[] Flags, string EntryName)
        {
            if (!VariousFunctions.CheckFileName(EntryName))
            {
                throw new ArgumentException("Invalid name: \"" + EntryName + "\"", "EntryName");
            }

            EntryData newEntryData = new EntryData();
            newEntryData.EntryOffset = GetNewEntryOffset(Destination);

            ushort Date = VariousFunctions.DateTimeToFATShort(DateTime.Now, true);
            ushort Time = VariousFunctions.DateTimeToFATShort(DateTime.Now, false);
            newEntryData.CreationDate = Date;
            newEntryData.CreationTime = Time;
            newEntryData.ModifiedDate = Date;
            newEntryData.ModifiedTime = Time;
            newEntryData.AccessDate = Date;
            newEntryData.AccessTime = Time;
            if (Flags.Length != 0)
            {
                newEntryData.Flags = VariousFunctions.FlagsToByte(Flags);
            }
            else
            {
                newEntryData.Flags = 0;
            }
            newEntryData.Size = Size;
            newEntryData.Name = EntryName;
            newEntryData.NameSize = (byte)EntryName.Length;
            if ((Size == 0 && Flags.Contains(Geometry.Flags.Directory)) || (Size != 0 && Flags.Length == 0))
            {
                newEntryData.StartingCluster = Destination.Drive.GetFreeBlocks(Destination, 1, 0, 0, false)[0];
            }
            else
            {
                newEntryData.StartingCluster = 0;
            }
            WriteFATChain(new uint[] { newEntryData.StartingCluster });
            CreateNewEntry(newEntryData);
            return newEntryData;
        }

        public long GetNewEntryOffset(Folder Destination)
        {
            // Go to the last block since we want to speed things up and not have to go
            // through all of them
            EntryData[] Entries = EntryDataFromBlock(Destination.BlocksOccupied[Destination.BlocksOccupied.Length - 1]);
            if (Entries.Length == 0)
            {
                return Destination.StartingOffset;
            }
            // If there isn't the maximum amount of entries for a cluster...
            if (Entries.Length < Destination.PartitionInfo.ClusterSize / 0x40)
            {
                foreach (EntryData e in Entries)
                {
                    if (e.NameSize == 0xE5)
                    {
                        return e.EntryOffset;
                    }
                }
                return Entries[Entries.Length - 1].EntryOffset + 0x40;
            }
            // Max amount of entries, let's add another cluster to our parent...
            else
            {
                uint[] NewBlocks = new uint[Destination.BlocksOccupied.Length + 1];
                Array.Copy(Destination.BlocksOccupied, NewBlocks, Destination.BlocksOccupied.Length);
                NewBlocks[NewBlocks.Length - 1] = Destination.Drive.GetFreeBlocks((Folder)Destination, 1, 0, 0, false)[0];
                byte[] FF = new byte[Destination.PartitionInfo.ClusterSize];
                for (int i = 0; i < FF.Length; i++)
                {
                    FF[i] = 0xFF;
                }
                Streams.Writer w = Destination.Drive.Writer();
                w.BaseStream.Position = VariousFunctions.BlockToFATOffset(NewBlocks[NewBlocks.Length - 1], Destination);
                w.Write(FF);
                Destination.BlocksOccupied = NewBlocks;
                return VariousFunctions.BlockToFATOffset(NewBlocks[NewBlocks.Length - 1], Destination);
            }
        }

        // Do not feel like recoding this function.
        public void CreateNewEntry(EntryData Edata)
        {
            Streams.Reader br = Parent.Drive.Reader();
            //Set our position so that we can read the entry location
            br.BaseStream.Position = VariousFunctions.DownToNearest200(Edata.EntryOffset);
            byte[] buffer = br.ReadBytes(0x200);
            //Create our binary writer
            Streams.Writer bw = new Streams.Writer(new System.IO.MemoryStream(buffer));
            //Set our position to where the entry is
            long EntryOffset = Edata.EntryOffset - VariousFunctions.DownToNearest200(Edata.EntryOffset);
            bw.BaseStream.Position = EntryOffset;
            //Write our entry
            bw.Write(Edata.NameSize);
            bw.Write(Edata.Flags);
            bw.Write(Encoding.ASCII.GetBytes(Edata.Name));
            if (Edata.NameSize != 0xE5)
            {
                int FFLength = 0x2A - Edata.NameSize;
                byte[] FF = new byte[FFLength];
                for (int i = 0; i < FFLength; i++)
                {
                    FF[i] = 0xFF;
                }
                bw.Write(FF);
            }
            else
            {
                bw.BaseStream.Position += 0x2A - Edata.Name.Length;
            }
            //Right here, we need to make everything a byte array, as it feels like writing
            //everything in little endian for some reason...
            byte[] StartingCluster = BitConverter.GetBytes(Edata.StartingCluster);
            Array.Reverse(StartingCluster);
            bw.Write(StartingCluster);
            byte[] Size = BitConverter.GetBytes(Edata.Size);
            Array.Reverse(Size);
            bw.Write(Size);
            //Write ref the creation date/time 3 times
            byte[] CreationDate = BitConverter.GetBytes(Edata.CreationDate);
            byte[] CreationTime = BitConverter.GetBytes(Edata.CreationTime);
            byte[] AccessDate = BitConverter.GetBytes(Edata.AccessDate);
            byte[] AccessTime = BitConverter.GetBytes(Edata.AccessTime);
            byte[] ModifiedDate = BitConverter.GetBytes(Edata.ModifiedDate);
            byte[] ModifiedTime = BitConverter.GetBytes(Edata.ModifiedTime);
            Array.Reverse(CreationDate);
            Array.Reverse(CreationTime);
            Array.Reverse(AccessDate);
            Array.Reverse(AccessTime);
            Array.Reverse(ModifiedDate);
            Array.Reverse(ModifiedTime);
            bw.Write(CreationDate);
            bw.Write(CreationTime);
            bw.Write(AccessDate);
            bw.Write(AccessTime);
            bw.Write(ModifiedDate);
            bw.Write(ModifiedTime);
            //Close our writer
            bw.Close();
            //Get our IO
            bw = Parent.Drive.Writer();
            bw.BaseStream.Position = VariousFunctions.DownToNearest200(Edata.EntryOffset);
            //Write ref our buffer
            bw.Write(buffer);
        }

        public void WriteFATChain(uint[] Chain)
        {
            Streams.Reader r = Parent.Drive.Reader();
            Streams.Writer w = new CLKsFATXLib.Streams.Writer(r.BaseStream);
            long lastoffset = 0;
            long buffersize = 0x1000;
            byte[] Buffer = new byte[buffersize];
            for (int i = 0; i < Chain.Length; i++)
            {
                // Read the chain buffer
                if (lastoffset != VariousFunctions.BlockToFATOffset(Chain[i], Parent).DownToNearestCluster(0x1000))
                {
                    if (i != 0)
                    {
                        w.BaseStream.Position = lastoffset;
                        w.Write(Buffer);
                    }
                    lastoffset = VariousFunctions.BlockToFATOffset(Chain[i], Parent).DownToNearestCluster(0x1000);
                    r.BaseStream.Position = lastoffset;
                    Buffer = r.ReadBytes((int)buffersize);
                }

                // Write the chain
                Streams.Writer mem = new CLKsFATXLib.Streams.Writer(new System.IO.MemoryStream(Buffer));
                mem.BaseStream.Position = VariousFunctions.BlockToFATOffset(Chain[i], Parent) - VariousFunctions.BlockToFATOffset(Chain[i], Parent).DownToNearestCluster(0x1000);
                byte[] writing = new byte[0];
                switch (Parent.PartitionInfo.EntrySize)
                {
                    case 2:
                        if (i != Chain.Length - 1)
                        {
                            writing = BitConverter.GetBytes((ushort)Chain[i + 1]);
                        }
                        else
                        {
                            writing = BitConverter.GetBytes((ushort)0xFFFF);
                        }
                        break;
                    case 4:
                        if (i != Chain.Length - 1)
                        {
                            writing = BitConverter.GetBytes(Chain[i + 1]);
                        }
                        else
                        {
                            writing = BitConverter.GetBytes(0xFFFFFFFF);
                        }
                        break;
                }
                Array.Reverse(writing);
                mem.Write(writing);
                mem.Close();
                if (i == Chain.Length - 1)
                {
                    w.BaseStream.Position = lastoffset;
                    w.Write(Buffer);
                }
            }
        }

        public void ClearFATChain(uint[] Chain)
        {
            Streams.Reader r = Parent.Drive.Reader();
            Streams.Writer w = new CLKsFATXLib.Streams.Writer(r.BaseStream);
            long buffersize = 0x1000;
            long lastoffset = 0;//VariousFunctions.BlockToFATOffset(Chain[0], Parent).DownToNearestCluster(buffersize);
            byte[] Buffer = new byte[buffersize];
            for (int i = 0; i < Chain.Length; i++)
            {
                // Read the chain buffer
                if (lastoffset != VariousFunctions.BlockToFATOffset(Chain[i], Parent).DownToNearestCluster(0x1000))
                {
                    if (i != 0)
                    {
                        w.BaseStream.Position = lastoffset;
                        w.Write(Buffer);
                    }
                    lastoffset = VariousFunctions.BlockToFATOffset(Chain[i], Parent).DownToNearestCluster(0x1000);
                    r.BaseStream.Position = lastoffset;
                    Buffer = r.ReadBytes((int)buffersize);
                }

                // Write the chain
                Streams.Writer mem = new CLKsFATXLib.Streams.Writer(new System.IO.MemoryStream(Buffer));
                mem.BaseStream.Position = VariousFunctions.BlockToFATOffset(Chain[i], Parent) - VariousFunctions.BlockToFATOffset(Chain[i], Parent).DownToNearestCluster(0x1000);
                byte[] writing = new byte[0];
                switch (Parent.PartitionInfo.EntrySize)
                {
                    case 2:
                         writing = BitConverter.GetBytes((ushort)0);
                        break;
                    case 4:
                        writing = BitConverter.GetBytes(0);
                        break;
                }
                mem.Write(writing);
                mem.Close();
                if (i == Chain.Length - 1)
                {
                    w.BaseStream.Position = lastoffset;
                    w.Write(Buffer);
                }
            }
        }
    }
}
