using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace CLKsFATXLib.Structs
{
        public struct EntryData
        {
            /// <summary>
            /// Size of the name
            /// </summary>
            public byte NameSize;
            /// <summary>
            /// Entry flags
            /// </summary>
            public byte Flags;
            /// <summary>
            /// Entry name
            /// </summary>
            public string Name;
            /// <summary>
            /// Beginning cluster (block) for the entry
            /// </summary>
            public UInt32 StartingCluster;
            /// <summary>
            /// Size of the entry (0x00 for folders)
            /// </summary>
            public UInt32 Size;
            public UInt16 CreationDate;
            public UInt16 CreationTime;
            public UInt16 AccessDate;
            public UInt16 AccessTime;
            public UInt16 ModifiedDate;
            public UInt16 ModifiedTime;
            /// <summary>
            /// Offset for the entry
            /// </summary>
            public long EntryOffset;
        }

        public struct STFSInfo
        {
            // public System.Drawing.Image ContentImage;
            // public System.Drawing.Image TitleImage;
            /// <summary>
            /// The identifier for the console the package was created one
            /// </summary>
            public byte[] ConsoleID;
            /// <summary>
            /// Identifier for the device that the package was assigned to
            /// </summary>
            public byte[] DeviceID;
            /// <summary>
            /// Package header magic
            /// </summary>
            public byte[] Magic;
            /// <summary>
            /// Identifier for the profile which the package is tied to (depending on transfer flags); the profile which the package was created with
            /// </summary>
            public byte[] ProfileID;
            /// <summary>
            /// Title (game/application) name
            /// </summary>
            public string TitleName;
            /// <summary>
            /// Local package name
            /// </summary>
            public string ContentName;
            /// <summary>
            /// Title (game/application) identifier
            /// </summary>
            public uint TitleID;
        }

        /// <summary>
        /// Provides information about a given partition
        /// </summary>
        public struct PartitionInfo
        {
            [Category("Header"), DisplayName("Partition Magic"), Description("The partition magic displayed in decimal")]
            /// <summary>
            /// Partition magic
            /// </summary>
            public uint Magic
            {
                get;
                internal set;
            }
            [Category("Header"), DisplayName("Partition Magic"), Description("The partition magic displayed in hex")]
            public string MagicAsString
            {
                get
                {
                    return "0x" + Magic.ToString("X2");
                }
            }

            [Category("Header"), DisplayName("Cluster Size"), Description("The size of each cluster in the partition in decimal")]
            /// <summary>
            /// Cluster size
            /// </summary>
            public long ClusterSize
            {
                get;
                internal set;
            }

            [Category("Header"), DisplayName("Cluster Size"), Description("The size of each cluster in the partition in hex")]
            public string ClusterSizeAsString
            {
                get
                {
                    return "0x" + ClusterSize.ToString("X");
                }
            }

            [Category("Header"), DisplayName("Partition ID"), Description("The partition identifier in decimal")]
            /// <summary>
            /// Partition ID
            /// </summary>
            public uint ID
            {
                get;
                internal set;
            }

            [Category("Header"), DisplayName("Partition ID"), Description("The partition identifier in hex")]
            public string IDAsString
            {
                get
                {
                    return "0x" + ID.ToString("X2");
                }
            }

            [Category("Header"), DisplayName("Sectors Per Cluster"), Description("The number of sectors per cluster in decimal")]
            /// <summary>
            /// Sectors per cluster
            /// </summary>
            public uint SectorsPerCluster
            {
                get;
                internal set;
            }

            [Category("Header"), DisplayName("Sectors Per Cluster"), Description("The number of sectors per cluster in hex")]
            public string SectorsPerClusterAsString
            {
                get
                {
                    return "0x" + SectorsPerCluster.ToString("X");
                }
            }


            [Category("FAT"), DisplayName("FAT Copies"), Description("The number of file allocation table copies")]
            /// <summary>
            /// Number of FAT copies for the partition
            /// </summary>
            public uint FATCopies
            {
                get;
                internal set;
            }

            [Category("FAT"), DisplayName("FAT Size"), Description("The file allocation table size in decimal")]
            /// <summary>
            /// FAT size for the partition
            /// </summary>
            public long FATSize
            {
                get;
                internal set;
            }

            [Category("FAT"), DisplayName("FAT Size"), Description("The file allocation table size in hex")]
            public string FATSizeAsString
            {
                get
                {
                    return "0x" + FATSize.ToString("X");
                }
            }

            [Category("FAT"), DisplayName("FAT Size"), Description("The file allocation table size in bytes/KB/GB")]
            public string FATSizeAsFriendly
            {
                get
                {
                    return VariousFunctions.ByteConversion(FATSize);
                }
            }

            [Category("Data"), DisplayName("Data Region Start"), Description("The location in which data starts in decimal")]
            /// <summary>
            /// Data offset for the partition
            /// </summary>
            public long DataOffset
            {
                get;
                internal set;
            }

            [Category("Data"), DisplayName("Data Region Start"), Description("The location in which data starts in hex")]
            public string DataOffsetAsString
            {
                get
                {
                    return "0x" + DataOffset.ToString("X");
                }
            }

            [Category("FAT"), DisplayName("FAT Offset"), Description("The file allocation table starting offset in decimal")]
            /// <summary>
            /// FAT offset for the partition
            /// </summary>
            public long FATOffset
            {
                get;
                internal set;
            }

            [Category("FAT"), DisplayName("FAT Offset"), Description("The file allocation table starting offset in hex")]
            public string FATOffsetAsString
            {
                get
                {
                    return "0x" + FATOffset.ToString("X");
                }
            }

            [Category("Partition"), DisplayName("Partition Size"), Description("The partition size in decimal")]
            /// <summary>
            /// Size of the partition
            /// </summary>
            public long Size
            {
                get;
                internal set;
            }

            [Category("Partition"), DisplayName("Partition Size"), Description("The partition size in hex")]
            public string SizeAsString
            {
                get
                {
                    return "0x" + Size.ToString("X");
                }
            }

            [Category("Partition"), DisplayName("Partition Size"), Description("The partition size in bytes/KB/MB/GB")]
            public string SizeFriendly
            {
                get
                {
                    return VariousFunctions.ByteConversion(Size);
                }
            }

            [Category("Partition"), DisplayName("Partition Offset"), Description("The partition offset in decimal")]
            /// <summary>
            /// Partition offset
            /// </summary>
            public long Offset
            {
                get;
                internal set;
            }

            [Category("Partition"), DisplayName("Partition Offset"), Description("The partition offset in hex")]
            public string OffsetAsString
            {
                get
                {
                    return "0x" + Offset.ToString("X");
                }
            }

            [Category("Partition"), DisplayName("Partition Name"), Description("The partition name")]
            /// <summary>
            /// Partition name
            /// </summary>
            public string Name
            {
                get;
                internal set;
            }

            [Category("FAT"), DisplayName("Chainmap Size"), Description("The size (in bytes) of a chainmap entry")]
            /// <summary>
            /// Entry size
            /// </summary>
            public int EntrySize
            {
                get;
                internal set;
            }

            [Category("Partition"), DisplayName("Clusters"), Description("The number of clusters in this partition in decimal")]
            /// <summary>
            /// Number of clusters in the partition
            /// </summary>
            public uint Clusters
            {
                get;
                internal set;
            }

            [Category("Partition"), DisplayName("Clusters"), Description("The number of clusters in this partition in hex")]
            public string ClustersAsString
            {
                get
                {
                    return "0x" + Clusters.ToString("X");
                }
            }


            [Category("FAT"), DisplayName("Don't mind this"), Description("It's dumb.")]
            /// <summary>
            /// The REAL size of the file allocation table
            /// </summary>
            public long RealFATSize
            {
                get;
                internal set;
            }
        }

        public struct Queue
        {
            public Folder Folder;
            public string Path;
            public bool Writing;
        }

        public struct ExistingEntry
        {
            // The entry that already exists
            public Entry Existing;
            // The path to the entry they tried writing
            public string NewPath;
        }

        public struct FreeEntry
        {
            public bool UsingDeletedEntry;
            public EntryData NewEntryData;
        }

        public struct CachedTitleName
        {
            public uint ID;
            public string Name;
        }

        public struct WriteResult
        {
            // The entry that either exists, or was written
            public Entry Entry;
            public bool CouldNotWrite;
            public string ConflictingEntryPath;
            public Entry AttemptedEntryToMove;
        }

    public struct FileAction
    {
        public int Progress { get; internal set; }
        public int MaxValue { get; internal set; }
        public string FullPath { get; internal set; }
        public bool Cancel;
    }
    
    public delegate void FileActionChanged(ref FileAction Progress);

    public struct FolderAction
    {
        public int Progress { get; internal set; }
        public int MaxValue { get; internal set; }
        public bool Cancel { get; set; }
        public string CurrentFile { get; internal set; }
        public string CurrentFilePath { get; internal set; }
    }

    public delegate void FolderActionChanged(ref FolderAction Progress);

    public struct EntryEventArgs
    {
        public string FullParentPath { get; internal set; }
        public Folder ParentFolder { get; internal set; }
        public Entry ModifiedEntry { get; internal set; }
        public bool Deleting { get; internal set; }
    }

    public delegate void OnEntryEvent(ref EntryEventArgs e);
}
