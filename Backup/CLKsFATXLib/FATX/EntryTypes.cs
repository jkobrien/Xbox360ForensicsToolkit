using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CLKsFATXLib.Structs;
using System.Runtime.CompilerServices;

namespace CLKsFATXLib
{
    public class Entry
    {
        private List<uint> blocks;
        public Entry(PartitionInfo Partition, EntryData EntryData, Drive Drive)
        {
            this.PartitionInfo = Partition;
            this.Drive = Drive;
            this.EntryData = EntryData;
        }

        public uint[] BlocksOccupied
        {
            get
            {
                if (blocks == null)
                {
                    if (!IsDeleted)
                    {
                        EntryFunctions e = new EntryFunctions(this);
                        blocks = e.GetBlocksOccupied().ToList();
                    }
                    else
                    {
                        if (IsFolder)
                        {
                            blocks = new List<uint>();
                            blocks.Add(this.EntryData.StartingCluster);
                        }
                        else
                        {
                            long size = ((File)this).Size;
                            size = CLKsFATXLib.VariousFunctions.UpToNearestCluster(size, this.PartitionInfo.ClusterSize);
                            blocks = new List<uint>();
                            blocks.Add(StartingCluster);
                            for (uint i = 1; i < size / PartitionInfo.ClusterSize; i++)
                            {
                                blocks.Add(blocks[0] + i);
                            }
                        }
                    }
                }
                return blocks.ToArray();
            }
            internal set
            {
                blocks = value.ToList();
            }
        }

        public Drive Drive { get; private set; }

        public EntryData EntryData { get; internal set;}

        public DateTime CreationDate
        {
            get { return VariousFunctions.DateTimeFromFATInt(EntryData.CreationDate, EntryData.CreationTime); }
        }

        public DateTime ModifiedDate
        {
            get { return VariousFunctions.DateTimeFromFATInt(EntryData.ModifiedDate, EntryData.ModifiedTime); }
        }

        public DateTime AccessedDate
        {
            get { return VariousFunctions.DateTimeFromFATInt(EntryData.AccessDate, EntryData.AccessTime); }
        }

        public PartitionInfo PartitionInfo
        {
            get;
            private set;
        }

        public string Name
        {
            get
            {
                return EntryData.Name;
            }
        }

        public long StartingOffset
        {
            get
            {
                return VariousFunctions.GetBlockOffset(EntryData.StartingCluster, this);
            }
        }

        public uint StartingCluster
        {
            get
            {
                return EntryData.StartingCluster;
            }
        }

        public long EntryOffset
        {
            get
            {
                return EntryData.EntryOffset;
            }
        }

        public string EntryType
        {
            get
            {
                if (IsFolder)
                {
                    if (IsDeleted)
                    {
                        return "Deleted Folder";
                    }
                    return "Folder";
                }
                else
                {
                    if (Name.ToLower().EndsWith(".xbe"))
                    {
                        return "Xbox Executable";
                    }
                    else if (Name.ToLower().EndsWith(".xex"))
                    {
                        return "Xenon Executable";
                    }
                    else if (((File)this).IsSTFSPackage())
                    {
                        string magic = ((File)this).STFSMagic();
                        switch (magic)
                        {
                            case "CON":
                                return "Console-Signed Package";
                            case "LIVE":
                                return "LIVE-Signed Package";
                            case "PIRS":
                                return "PIRS-Signed Package";
                            default:
                                throw new Exception("File magic not valid");
                        }
                    }
                    else if (Name.ToLower().EndsWith(".jpg") || Name.ToLower().EndsWith(".png"))
                    {
                        return "Image File";
                    }
                    else if (IsDeleted)
                    {
                        return "Deleted File";
                    }
                    else
                    {
                        return "File";
                    }
                }
            }
        }

        public bool IsDeleted
        {
            get;
            internal set;
        }

        public bool IsFolder
        {
            get
            {
                if (Flags.Contains(CLKsFATXLib.Geometry.Flags.Directory))
                {
                    return true;
                }
                return false;
            }
        }

        // Returns true for good, false for bad
        public bool Rename(string NewName)
        {
            if (VariousFunctions.CheckFileName(NewName))
            {
                if (Name.ToLower() != NewName.ToLower())
                {
                    foreach (Folder f in Parent.Folders())
                    {
                        if (f.Name.ToLower() == NewName.ToLower() && !f.IsDeleted)
                        {
                            throw new Exception("A folder with the desired name already exists!");
                        }
                    }
                    foreach (File f in Parent.Files())
                    {
                        if (f.Name.ToLower() == NewName.ToLower() && !f.IsDeleted)
                        {
                            throw new Exception("A file with the desired name already exists!");
                        }
                    }
                }
                EntryData ed = this.EntryData;
                ed.Name = NewName;
                ed.NameSize = (byte)NewName.Length;
                this.EntryData = ed;
                EntryFunctions ef = new EntryFunctions(this);
                ef.CreateNewEntry(EntryData);

                // Change the path to match that of the new name
                this.FullPath = Parent.FullPath + "\\" + NewName;
                return true;
            }
            return false;
        }

        public Folder Parent
        {
            get;
            internal set;
        }

        public Geometry.Flags[] Flags
        {
            get
            {
                List<Geometry.Flags> FL = new List<Geometry.Flags>();
                // Read bit zero, mask the rest of that shit
                for (short i = 1, j = 0; i <= 0x80; i <<= 1, j++)
                {
                    if (((EntryData.Flags & i) >> j) == 1)
                    {
                        FL.Add((Geometry.Flags)Enum.Parse(typeof(Geometry.Flags), Enum.GetName(typeof(Geometry.Flags), j)));
                    }
                }
                return FL.ToArray();
            }
        }

        public string FullPath
        {
            get;
            internal set;
        }

        public Structs.WriteResult Move(string Destination)
        {
            WriteResult Result = new WriteResult();
            // Get the destination folder
            Folder D = Drive.FolderFromPath(Destination);
            if (D.FullPath == this.Parent.FullPath || D.FullPath.Contains(this.FullPath))
            {
                Result.Entry = this;
                return Result;
            }
            for (int i = 0; i < D.Folders().Length; i++)
            {
                if (D.Folders()[i].Name.ToLower() == Name.ToLower())
                {
                    Result.CouldNotWrite = true;
                    Result.Entry = D.Folders()[i];
                    Result.ConflictingEntryPath = Result.Entry.FullPath;
                    Result.AttemptedEntryToMove = this;
                    return Result;
                }
            }

            for (int i = 0; i < D.Files().Length; i++)
            {
                if (D.Files()[i].Name.ToLower() == Name.ToLower())
                {
                    Result.CouldNotWrite = true;
                    Result.Entry = D.Files()[i];
                    Result.ConflictingEntryPath = Result.Entry.FullPath;
                    Result.AttemptedEntryToMove = this;
                    return Result;
                }
            }
            // Get the offset for a new folder in the destination
            EntryFunctions ef = new EntryFunctions(D);
            long Offset = ef.GetNewEntryOffset(D);
            EntryData f = this.EntryData;
            byte NameSize = this.EntryData.NameSize;
            // Mark this entry as deleted and write it
            f.NameSize = 0xE5;
            ef.CreateNewEntry(f);
            // Reset the size of that name...
            f.NameSize = NameSize;
            // Set the offset to that of the new folder
            f.EntryOffset = Offset;
            // Write the entry
            ef.CreateNewEntry(f);
            this.EntryData = f;
            if (f.EntryOffset >= VariousFunctions.GetBlockOffset(this.BlocksOccupied[BlocksOccupied.Length - 1], this) + this.PartitionInfo.ClusterSize)
            {
                List<uint> blocks = this.BlocksOccupied.ToList();
                blocks.Add(VariousFunctions.GetBlockFromOffset(f.EntryOffset, this.PartitionInfo));
                this.BlocksOccupied = blocks.ToArray();
            }

            if (Parent != null)
            {
                if (!this.IsFolder)
                {
                    Parent.cachedFiles.Remove((File)this);
                    Parent.cachedDeletedFiles.Add((File)this);
                }
                else
                {
                    Parent.cachedFolders.Remove((Folder)this);
                    Parent.cachedDeletedFolders.Add((Folder)this);
                }
            }

            Structs.EntryEventArgs ea = new EntryEventArgs();
            ea.Deleting = true;
            ea.FullParentPath = this.Parent.FullPath;
            ea.ModifiedEntry = this;
            ea.ParentFolder = this.Parent;

            if (Parent != null)
            {
                if (Parent.EntryEventNull)
                {
                    if (this.IsFolder)
                    {
                        if (!((Folder)this).EntryEventNull)
                        {
                            ((Folder)this).OnEntryEvent(ref ea);
                        }
                    }
                }
                else
                {
                    Parent.OnEntryEvent(ref ea);
                }
            }
            else if (this.IsFolder)
            {
                if (!((Folder)this).EntryEventNull)
                {
                    ((Folder)this).OnEntryEvent(ref ea);
                }
            }

            this.Parent = D;
            this.FullPath = this.Parent.FullPath + "\\" + Name;

            Structs.EntryEventArgs MovedEA = new EntryEventArgs();
            MovedEA.Deleting = false;
            MovedEA.FullParentPath = this.Parent.FullPath;
            MovedEA.ModifiedEntry = this;
            MovedEA.ParentFolder = this.Parent;

            if (Parent != null)
            {
                if (Parent.EntryEventNull)
                {
                    if (this.IsFolder)
                    {
                        if (!((Folder)this).EntryEventNull)
                        {
                            ((Folder)this).OnEntryEvent(ref MovedEA);
                        }
                    }
                }
                else
                {
                    Parent.OnEntryEvent(ref MovedEA);
                }

                if (!this.IsFolder)
                {
                    Parent.cachedFiles.Add((File)this);
                }
                else
                {
                    Parent.cachedFolders.Add((Folder)this);
                }
            }
            else if (this.IsFolder)
            {
                if (!((Folder)this).EntryEventNull)
                {
                    ((Folder)this).OnEntryEvent(ref MovedEA);
                }
            }

            Result.Entry = this;
            return Result;
        }
    }

    public class File : Entry
    {
        System.IO.Stream stream;
        public File(PartitionInfo Partition, EntryData EntryData, Drive Drive)
            : base(Partition, EntryData, Drive)
        {

        }

        public long Size
        {
            get
            {
                return EntryData.Size;
            }
        }

        public string SizeFriendly
        {
            get
            {
                return VariousFunctions.ByteConversion(Size);
            }
        }

        /// <summary>
        /// Gets System.IO.Stream for the file to read directly from the drive
        /// </summary>
        /// <remarks>May have bugs.</remarks>
        /// <returns>type of System.IO.Stream</returns>
        public System.IO.Stream GetStream()
        {
            if (stream == null || !stream.CanRead)
            {
                // Re-intialize the stream
                stream = new Streams.FATXFileStream(Drive.Stream(), this);
            }
            stream.Position = 0;
            return stream;
        }

        public event FileActionChanged FileAction;

        protected virtual void OnFileAction(ref FileAction a)
        {
            if (FileAction != null)
            {
                FileAction(ref a);
            }
        }

        /// <summary>
        /// Writes the file to a given directory
        /// </summary>
        public void Extract(string Destination)
        {
            // Check to see if this file is null...
            if (this.Size == 0)
            {
                System.IO.File.Create(Destination);
                return;
            }

            // Before doing anything else, check to see if hte user has
            // enough space on their current drive for the file
            try
            {
                if (new System.IO.DriveInfo(System.IO.Path.GetPathRoot(Destination)).AvailableFreeSpace < Size)
                {
                    throw new Exception("Not enough free space on destination device to extract file \"" + Name + "\" to");
                }
            }
            catch { }

            // Create a new instance of the blocks occupied
            uint[] Blocks = BlocksOccupied;
            // FileAction to be used
            Structs.FileAction Fa = new FileAction();
            Fa.MaxValue = Blocks.Length;
            Fa.FullPath = this.FullPath;

            // The IO to read the file
            Streams.Reader FileReader = Drive.Reader();
            // The IO to write to the destination file
            Streams.Writer FileWriter = new CLKsFATXLib.Streams.Writer(new System.IO.FileStream(Destination, System.IO.FileMode.Create));
            // Loop for each block...
            for (int i = 0; i < Blocks.Length - 1; i++)
            {
                if (Fa.Cancel)
                {
                    Fa.Cancel = false;
                    FileWriter.Close();
                    return;
                }
                // Set the position to the beginning of the block
                FileReader.BaseStream.Position = VariousFunctions.GetBlockOffset(Blocks[i], this);
                for (int j = 1, k=0; j <= 0x100; j++, k++)
                {
                    if (i + k == Blocks.Length - 1)
                    {
                        FileWriter.Write(FileReader.ReadBytes((int)PartitionInfo.ClusterSize * k));
                        i += k;
                        Fa.Progress += k;
                        break;
                    }
                    else if (Blocks[i + k] == Blocks.Length - 2 || Blocks[i + k] + 1 != Blocks[i + j] || j == 10)
                    {
                        FileWriter.Write(FileReader.ReadBytes((int)PartitionInfo.ClusterSize * j));
                        i += k;
                        Fa.Progress += j;
                        break;
                    }
                }
                OnFileAction(ref Fa);
            }
            // For the last cluster, we don't know how long it is... so we use
            // this nifty function I made to do that for us
            FileReader.BaseStream.Position = VariousFunctions.GetBlockOffset(Blocks[Blocks.Length - 1], this);
            // Read/write data
            FileWriter.Write(VariousFunctions.ReadBytes(ref FileReader, VariousFunctions.RemainingData(this)));
            Fa.Progress++;
            OnFileAction(ref Fa);
            FileWriter.Close();
        }

        public System.Drawing.Image ti;
        public System.Drawing.Image gi;
        /// <summary>
        /// If the file is an STFS package, it will get the icon for the game
        /// </summary>
        public System.Drawing.Image TitleIcon()
        {
            if (IsSTFSPackage())
            {
                if (ti == null && !tiattempted)
                {
                    // Get a new reader for this file
                    Streams.Reader r = new CLKsFATXLib.Streams.Reader(GetStream());
                    r.BaseStream.Position = (long)Geometry.STFSOffsets.TitleImageSize;
                    int Size = r.ReadInt32();
                    if (Size == 0)
                    {
                        giattempted = true;
                        return null;
                    }
                    r.BaseStream.Position = (long)Geometry.STFSOffsets.TitleImage;
                    try
                    {
                        ti = System.Drawing.Image.FromStream(new System.IO.MemoryStream(r.ReadBytes(Size)));
                    }
                    catch { }
                    tiattempted = true;
                }
                return ti;
            }
            throw new Exception("File is not a valid STFS package!");
        }

        bool giattempted = false;
        bool tiattempted = false;
        /// <summary>
        /// If the file is an STFS package, it will get the package's icon (non-game icon)
        /// </summary>
        /// <returns>STFS package content icon</returns>
        public System.Drawing.Image ContentIcon()
        {
            if (IsSTFSPackage())
            {
                if (gi == null && !giattempted)
                {
                    // Get a new reader for this file
                    Streams.Reader r = new CLKsFATXLib.Streams.Reader(GetStream());
                    r.BaseStream.Position = (long)Geometry.STFSOffsets.ContentImageSize;
                    int Size = r.ReadInt32();
                    if (Size == 0)
                    {
                        giattempted = true;
                        return null;
                    }
                    r.BaseStream.Position = (long)Geometry.STFSOffsets.ContentImage;
                    try
                    {
                        gi = System.Drawing.Image.FromStream(new System.IO.MemoryStream(r.ReadBytes(Size)));
                    }
                    catch(Exception e) { }
                    giattempted = true;
                }
                return gi;
            }
            throw new Exception("File is not a valid STFS package!");
        }

        bool isstfs;
        bool stfschecked;
        /// <summary>
        /// Checks the file magic to check if it's a valid CON/LIVE/PIRS package
        /// </summary>
        public bool IsSTFSPackage()
        {
            if (!isstfs && !stfschecked)
            {
                if (Size >= 0xA000)
                {
                    Streams.Reader r = new CLKsFATXLib.Streams.Reader(Drive.Stream());
                    r.BaseStream.Position = StartingOffset;
                    byte[] Buffer = null;
                    try
                    {
                        Buffer = r.ReadBytes(0x200);
                    }
                    catch { stfschecked = false; return isstfs = false; }
                    r = new CLKsFATXLib.Streams.Reader(new System.IO.MemoryStream(Buffer));
                    uint val = 0;
                    try
                    {
                        val = r.ReadUInt32();
                    }
                    catch { stfschecked = false; return isstfs = false; }
                    r.Close();
                    switch (val)
                    {
                        // CON
                        case 0x434F4E20:
                            stfschecked = true;
                            stfsmagic = "CON";
                            return isstfs = true;
                        // LIVE
                        case 0x4C495645:
                            stfschecked = true;
                            stfsmagic = "LIVE";
                            return isstfs = true;
                        // PIRS
                        case 0x50495253:
                            stfschecked = true;
                            stfsmagic = "PIRS";
                            return isstfs = true;
                        // NIGR
                        default:
                            stfschecked = true;
                            stfsmagic = "";
                            return isstfs = false;
                    }
                }
                else
                {
                    stfschecked = true;
                    return isstfs = false;
                }
            }
            return isstfs;
        }

        string stfsmagic = null;
        public string STFSMagic()
        {
            if (stfsmagic != null)
            {
                return stfsmagic;
            }
            else if (IsSTFSPackage())
            {
                return stfsmagic;
            }
            return stfsmagic;
        }

        string name = null;
        /// <summary>
        /// If the file is an STFS package, it will get the package's display (content) name
        /// </summary>
        /// <returns>Package display/content name</returns>
        public string ContentName()
        {
            if (IsSTFSPackage())
            {
                if (name == null)
                {
                    Streams.Reader r = new CLKsFATXLib.Streams.Reader(GetStream());
                    r.BaseStream.Position = (long)Geometry.STFSOffsets.DisplayName;
                    name = r.ReadUnicodeString(0x80);
                }
                return name;
            }
            throw new Exception("File is not a valid STFS package!");
        }

        string tname = null;
        /// <summary>
        /// If the file is an STFS package, it will get the name of the game that the package belongs to
        /// </summary>
        public string TitleName()
        {
            if (IsSTFSPackage())
            {
                if (tname == null)
                {
                    Streams.Reader r = new CLKsFATXLib.Streams.Reader(GetStream());
                    r.BaseStream.Position = (long)Geometry.STFSOffsets.TitleName;
                    tname = r.ReadUnicodeString(0x80);
                }
                return tname;
            }
            throw new Exception("File is not a valid STFS package!");
        }

        uint tid;
        /// <summary>
        /// If the file is an STFS package, it will get the game's unique identifier
        /// </summary>
        public uint TitleID()
        {
            if (IsSTFSPackage())
            {
                if (tid == 0)
                {
                    Streams.Reader r = new CLKsFATXLib.Streams.Reader(GetStream());
                    r.BaseStream.Position = (long)Geometry.STFSOffsets.TitleID;
                    tid = r.ReadUInt32();
                }
                return tid;
            }
            throw new Exception("File is not a valid STFS package!");
        }

        byte[] pid;
        /// <summary>
        /// If the file is an STFS package, it will get the package owner's profile ID
        /// </summary>
        public byte[] ProfileID()
        {
            if (IsSTFSPackage())
            {
                if (pid == null)
                {
                    Streams.Reader r = new CLKsFATXLib.Streams.Reader(GetStream());
                    r.BaseStream.Position = (long)Geometry.STFSOffsets.ProfileID;
                    pid = r.ReadBytes(0x8);
                }
                return pid;
            }
            throw new Exception("File is not a valid STFS package!");
        }

        byte[] did;
        /// <summary>
        /// If the file is an STFS package, it will get the device ID that the package was created on (HDD/USB)
        /// </summary>
        /// <returns>Package device ID</returns>
        public byte[] DeviceID()
        {
            if (IsSTFSPackage())
            {
                if (did == null)
                {
                    Streams.Reader r = new CLKsFATXLib.Streams.Reader(GetStream());
                    r.BaseStream.Position = (long)Geometry.STFSOffsets.DeviceID;
                    did = r.ReadBytes(0x14);
                }
                return did;
            }
            throw new Exception("File is not a valid STFS package!");
        }

        byte[] cid;
        /// <summary>
        /// If the file is an STFS package, it will get the identifier that the package was created on.
        /// </summary>
        /// <returns>Console ID for STFS package</returns>
        public byte[] ConsoleID()
        {
            if (IsSTFSPackage())
            {
                if (cid == null)
                {
                    Streams.Reader r = new CLKsFATXLib.Streams.Reader(GetStream());
                    r.BaseStream.Position = (long)Geometry.STFSOffsets.ConsoleID;
                    cid = r.ReadBytes(0x5);
                }
                return cid;
            }
            throw new Exception("File is not a valid STFS package!");
        }

        /// <summary>
        /// Deletes the file from the parent folder, clears the FAT chain, etc.
        /// </summary>
        public void Delete()
        {
            if (this.IsDeleted)
            {
                return;
            }
            // File action (event)
            Structs.FileAction fa = new FileAction();
            fa.FullPath = this.FullPath;
            fa.MaxValue = 2;
            OnFileAction(ref fa);

            // Create the entry functions so we can clear the FAT chain
            EntryFunctions ef = new EntryFunctions(this);
            // Get the FAT chain
            uint[] FatChain = this.BlocksOccupied;
            // Clear the FAT chain
            ef.ClearFATChain(FatChain);
            // Dispose of the FatChain
            FatChain = null;
            
            // Change the progress
            fa.Progress = 1;
            OnFileAction(ref fa);

            // Get the new entry data that we're going to write
            EntryData ed = this.EntryData;
            // Mark it as deleted
            ed.NameSize = 0xE5;
            // "Create" a new entry (writing the old one with the size of 0xE5
            ef.CreateNewEntry(ed);
            
            // Change progress
            fa.Progress = 2;
            OnFileAction(ref fa);

            // Reset the entry data
            this.EntryData = ed;

            // Other event
            EntryEventArgs eea = new EntryEventArgs();
            eea.FullParentPath = Parent.FullPath;
            eea.ModifiedEntry = this;
            eea.ParentFolder = this.Parent;
            eea.Deleting = true;
            if (Parent != null)
            {
                Parent.cachedFiles.Remove(this);
                Parent.cachedDeletedFiles.Add(this);
                try
                {
                    Parent.OnEntryEvent(ref eea);
                }
                catch { }
            }
        }
    }

    public class Folder : Entry
    {
        internal List<Folder> cachedFolders;
        internal List<Folder> cachedDeletedFolders;
        internal List<File> cachedFiles;
        internal List<File> cachedDeletedFiles;
        internal FolderAction fa;
        /// <summary>
        /// Default constructor for the folder class
        /// </summary>
        /// <param name="Partition">Partition information to which this folder belongs to</param>
        /// <param name="EntryData">The entry data for this folder</param>
        /// <param name="Drive">Drive which this folder belongs to</param>
        public Folder(PartitionInfo Partition, EntryData EntryData, Drive Drive)
            : base(Partition, EntryData, Drive)
        {
            ReturnDeletedEntries = false;
        }

        /// <summary>
        /// Gets or sets whether to return deleted entries (files/folders)
        /// </summary>
        public bool ReturnDeletedEntries
        {
            get;
            set;
        }

        /// <summary>
        /// Reloads the current folder (files and folders)
        /// </summary>
        public void Reload()
        {
            cachedFolders = null;
            cachedFiles = null;
            cachedDeletedFiles = null;
            cachedDeletedFolders = null;
        }

        public Folder[] Folders()
        {
            // If all of those guys are null, that means we have to re-load them.
            if (cachedFolders == null && cachedFiles == null && cachedDeletedFiles == null && cachedDeletedFolders == null)
            {
                object[] entries = new EntryFunctions(this).GetEntries(this);
                cachedFiles = (List<File>)entries[0];
                cachedFolders = (List<Folder>)entries[1];
                cachedDeletedFolders = (List<Folder>)entries[3];
                cachedDeletedFiles = (List<File>)entries[2];
            }
            if (ReturnDeletedEntries)
            {
                foreach (Folder f in cachedDeletedFolders)
                {
                    f.ReturnDeletedEntries = this.ReturnDeletedEntries;
                }
            }
            if (ReturnDeletedEntries)
            {
                Folder[] list = new Folder[cachedFolders.Count + cachedDeletedFolders.Count];
                Array.Copy(cachedFolders.ToArray(), 0, list, 0, cachedFolders.Count);
                Array.Copy(cachedDeletedFolders.ToArray(), 0, list, cachedFolders.Count, cachedDeletedFolders.Count);
                return list;
            }
            return cachedFolders.ToArray();
        }

        /// <summary>
        /// Gets the files contained in this folder
        /// </summary>
        /// <returns>File[] Files</returns>
        public File[] Files()
        {
            // If all of those guys are null, that means we have to re-load them.
            if (cachedFolders == null && cachedFiles == null && cachedDeletedFiles == null && cachedDeletedFolders == null)
            {
                object[] entries = new EntryFunctions(this).GetEntries(this);
                cachedFiles = (List<File>)entries[0];
                cachedFolders = (List<Folder>)entries[1];
                cachedDeletedFolders = (List<Folder>)entries[3];
                cachedDeletedFiles = (List<File>)entries[2];
            }
            if (ReturnDeletedEntries)
            {
                File[] list = new File[cachedFiles.Count + cachedDeletedFiles.Count];
                Array.Copy(cachedFiles.ToArray(), 0, list, 0, cachedFiles.Count);
                Array.Copy(cachedDeletedFiles.ToArray(), 0, list, cachedFiles.Count, cachedDeletedFiles.Count);
                return list;
            }
            return cachedFiles.ToArray();
        }

        /// <summary>
        /// Extracts the subfolders and files to the given paht
        /// </summary>
        /// <param name="Path">Path to extract the folder to</param>
        /// <param name="FoldersToSkip">Folder names to skip when extracting (folders will not be created, files will not be extracted)</param>
        public void Extract(string Path, string[] FoldersToSkip)
        {
            for (int i = 0; i < FoldersToSkip.Length; i++)
            {
                if (Name.ToLower() == FoldersToSkip[i].ToLower())
                {
                    return;
                }
            }
            // Create our new folder action
            fa = new FolderAction();
            OnFolderAction(ref fa);
            if (fa.Cancel)
            {
                fa.CurrentFile = "Canceling...";
                OnFolderAction(ref fa);
                fa.Cancel = false;
                return;
            }
            // Let's start off by creating this directory in the path defined
            if (!System.IO.Directory.Exists(Path + "\\" + Name))
            {
                System.IO.Directory.CreateDirectory(Path + "\\" + Name);
            }

            // Now, extract all the files in this directory to that one...
            foreach (File f in Files())
            {
                if (fa.Cancel)
                {
                    fa.Cancel = false;
                    return;
                }
                fa.CurrentFile = f.Name;
                f.FileAction += new FileActionChanged(f_FileAction);
                f.Extract(Path + "\\" + Name + "\\" + f.Name);
            }

            foreach (Folder f in Folders())
            {
                if (fa.Cancel)
                {
                    fa.Cancel = false;
                    return;
                }
                f.FolderAction += new FolderActionChanged(f_FolderAction);
                f.Extract(Path + "\\" + Name, FoldersToSkip);
            }
        }

        void f_FolderAction(ref FolderAction Progress)
        {
            FolderActionInternal(ref Progress);
        }

        void f_FileAction(ref FileAction Progress)
        {
            fa.MaxValue = Progress.MaxValue;
            fa.Progress = Progress.Progress;
            fa.CurrentFilePath = Progress.FullPath;
            if (fa.Cancel)
            {
                fa.Cancel = false;
                Progress.Cancel = true;
            }
            OnFolderAction(ref fa);
        }

        private event FolderActionChanged FolderActionInternal;
        public event FolderActionChanged FolderAction
        {
            add
            {
                fa = new FolderAction();
                if (FolderActionInternal == null || FolderActionInternal.GetInvocationList().Contains(value))
                {
                    FolderActionInternal += value;
                }
            }
            remove
            {
                FolderActionInternal -= value;
            }
        }

        internal virtual void OnFolderAction(ref FolderAction a)
        {
            if (FolderActionInternal != null)
            {
                FolderActionInternal(ref a);
            }
        }

        internal virtual void OnEntryEvent(ref EntryEventArgs e)
        {
            Drive.OnEntryEvent(ref e);
        }

        /// <summary>
        /// Checks if the folder name is a standard Xbox 360 title ID name (e.g. 00000001 for save games)
        /// </summary>
        public bool IsKnownFolder
        {
            get
            {
                if (IsTitleIDFolder && VariousFunctions.Known.Contains(Name))
                {
                    return true;
                }
                return false;
            }
        }

        // Returns the profile, otherwise returns null
        public File IsProfileFolder()
        {
            // Loop for each subdirectory
            foreach (Folder XboxDashboard in Folders())
            {
                // If the folder name is that of the Xbox Dashboard title ID
                if (XboxDashboard.IsTitleIDFolder && XboxDashboard.Name.ToLower() == "fffe07d1")
                {
                    foreach (Folder ProfileFolder in XboxDashboard.Folders())
                    {
                        // If the folder name is that of the profile ID
                        if (ProfileFolder.IsKnownFolder && ProfileFolder.Name == "00010000")
                        {
                            // Loop for each file in this directory
                            foreach (File f in ProfileFolder.Files())
                            {
                                if (f.Name == f.ContentName())
                                {
                                    Streams.Reader r = new CLKsFATXLib.Streams.Reader(f.GetStream());
                                    r.BaseStream.Position = 0x344;
                                    if (r.ReadUInt32() == 0x00010000)
                                    {
                                        return f;
                                    }
                                }
                            }
                            break;
                        }
                    }
                    break;
                }
            }
            return null;
        }

        string name = null;
        bool namechecked = false;
        /// <summary>
        /// Gets the title name from the STFS packages contained in this folder's subdirectories
        /// </summary>
        /// <returns>First non-null/string.Empty game name from an STFS package, otherwise will return the first non-null/string.Empty content name</returns>
        public string GameName()
        {
            if (!namechecked || ForceGameName)
            {
                if (IsTitleIDFolder && VariousFunctions.Known.Contains(Name))
                {
                    for (int i = 0; i < VariousFunctions.Known.Length; i++)
                    {
                        if (VariousFunctions.Known[i] == Name)
                        {
                            return name = VariousFunctions.KnownEquivilent[i];
                        }
                    }
                }
                else if (IsTitleIDFolder)
                {
                    if (name == null)
                    {
                        foreach (Folder f in Folders())
                        {
                            foreach (File fi in f.Files())
                            {
                                if (fi.IsSTFSPackage())
                                {
                                    if (fi.TitleName() != "")
                                    {
                                        namechecked = true;
                                        return name = fi.TitleName();
                                    }
                                }
                            }
                        }

                        // If nothing was returned, let's just return the first
                        // content display name
                        foreach (Folder f in Folders())
                        {
                            foreach (File fi in f.Files())
                            {
                                if (fi.IsSTFSPackage())
                                {
                                    if (fi.ContentName() != "")
                                    {
                                        namechecked = true;
                                        return name = fi.ContentName();
                                    }
                                }
                            }
                        }
                        namechecked = true;
                    }
                }
            }
            return name;
        }

        public bool ForceGameName
        {
            get;
            set;
        }

        System.Drawing.Image cIcon = null;
        /// <summary>
        /// Gets the icon for the game (if it's a title ID folder, and not a known folder)
        /// </summary>
        /// <returns>Image from the first STFS package contained in the subdirectories.</returns>
        public System.Drawing.Image GameIcon()
        {
            if ((!IsTitleIDFolder || VariousFunctions.Known.Contains(Name)) && Name.ToLower() != "FFFE07D1".ToLower())
            {
                return null;
            }
            if (cIcon == null)
            {
                foreach (Folder f in Folders())
                {
                    foreach (File fi in f.Files())
                    {
                        if (fi.IsSTFSPackage())
                        {
                            try
                            {
                                cIcon = fi.TitleIcon();
                                if (cIcon != null)
                                {
                                    return cIcon;
                                }
                            }
                            catch(Exception e) { }
                        }
                    }
                }
            }
            return cIcon;
        }

        /// <summary>
        /// Checks if the folder name is of valid title ID format
        /// </summary>
        public bool IsTitleIDFolder
        {
            get
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
        }

        void UpdateModifiedTime()
        {
            if (this.EntryData.EntryOffset != 0)
            {
                EntryFunctions ef = new EntryFunctions(this);
                EntryData ed = this.EntryData;
                ed.ModifiedDate = VariousFunctions.DateTimeToFATShort(DateTime.Now, true);
                ed.ModifiedTime = VariousFunctions.DateTimeToFATShort(DateTime.Now, false);

                this.EntryData = ed;
                ef.CreateNewEntry(ed);
            }
        }

        /// <summary>
        /// Creates a new folder
        /// </summary>
        /// <param name="Name">New folder name</param>
        /// <returns>The existing file or folder will be returned if applicable, otherwise returns null (no problems encountered)</returns>
        public Structs.WriteResult CreateNewFolder(string Name)
        {
            WriteResult Result = new WriteResult();
            Folders();
            Console.WriteLine("Getting folders... comparing names");
            for (int i = 0; i < cachedFolders.Count; i++)
            {
                if (cachedFolders[i].Name.ToLower() == Name.ToLower())
                {
                    Result.CouldNotWrite = true;
                    Result.Entry = cachedFolders[i];
                    Result.ConflictingEntryPath = Result.Entry.FullPath;
                    return Result;
                }
            }

            Console.WriteLine("Getting files... comparing names");
            for (int i = 0; i < cachedFiles.Count; i++)
            {
                if (cachedFiles[i].Name.ToLower() == Name.ToLower())
                {
                    Result.CouldNotWrite = true;
                    Result.Entry = cachedFiles[i];
                    Result.ConflictingEntryPath = Result.Entry.FullPath;
                    return Result;
                }
            }

            EntryFunctions ef = new EntryFunctions(this);
            Console.WriteLine("Creating new folder");
            Folder f = new Folder(this.PartitionInfo,
                ef.GetNewEntry(this, 0, new CLKsFATXLib.Geometry.Flags[] { Geometry.Flags.Directory }, Name),
                this.Drive);
            // Check if the new folder was created on a new cluster
            if (f.EntryOffset >= VariousFunctions.GetBlockOffset(this.BlocksOccupied[BlocksOccupied.Length - 1], this) + this.PartitionInfo.ClusterSize)
            {
                List<uint> blocks = this.BlocksOccupied.ToList();
                blocks.Add(VariousFunctions.GetBlockFromOffset(f.EntryOffset, this.PartitionInfo));
                this.BlocksOccupied = blocks.ToArray();
            }
            Console.WriteLine("Writing FAT chain");
            ef.WriteFATChain(new uint[] { f.EntryData.StartingCluster });
            f.FullPath = this.FullPath + "\\" + f.Name;
            f.Parent = this;
            Streams.Writer w = this.Drive.Writer();
            w.BaseStream.Position = f.StartingOffset;
            byte[] FF = new byte[PartitionInfo.ClusterSize];
            for (int i = 0; i < FF.Length; i++)
            {
                FF[i] = 0xFF;
            }
            Console.WriteLine("Writing FF");
            w.Write(FF);
            //Console.WriteLine("Closing stream");
            //w.Close();
            this.cachedFolders.Add(f);

            EntryEventArgs eea = new EntryEventArgs();
            eea.FullParentPath = FullPath;
            eea.ModifiedEntry = f;
            eea.ParentFolder = this;
            OnEntryEvent(ref eea);

            Console.WriteLine("Updating entry modified time");
            UpdateModifiedTime();

            Result.Entry = f;
            return Result;
        }

        /// <summary>
        /// Deletes all files and subfolders contained in this, and the folder itself
        /// </summary>
        public void Delete()
        {
            /* We're setting return deleted entries to false because:
             * 1.) We're deleting these folders, so the property doesn't matter anyway
             * 2.) We don't want to delete a deleted folder
             * 3.) Pickles
             */
            ReturnDeletedEntries = false;
            // Loop for each folder in this
            foreach (Folder f in Folders())
            {
                // While we're looping, we want to check if the user wants
                // to cancel the current action.  If they do, then we stop and return
                if (fa.Cancel)
                {
                    fa.Cancel = false;
                    return;
                }
                // Set the folder action event so we can update stuff
                f.FolderAction += new FolderActionChanged(f_FolderAction);
                // Call to delete the folder
                f.Delete();
            }
            // Now, loop through each file
            foreach (File f in Files())
            {
                // If they want us to cancel...
                if (fa.Cancel)
                {
                    // Set the cancel to false, then return
                    fa.Cancel = false;
                    return;
                }
                // Set the file action so we can update stuff again
                f.FileAction +=new FileActionChanged(f_FileAction);
                // Delete the file
                f.Delete();
            }

            // Update our progress
            fa.CurrentFile = this.Name;
            fa.CurrentFilePath = this.FullPath;
            fa.MaxValue = 2;
            fa.Progress = 0;
            OnFolderAction(ref fa);

            // Clear the FAT chain of this folder
            EntryFunctions ef = new EntryFunctions(this);
            ef.ClearFATChain(this.BlocksOccupied);

            // Update our progress again
            fa.Progress = 1;
            OnFolderAction(ref fa);

            // Create a new instance of the entry data
            EntryData ed = this.EntryData;
            // Set the size to 0xE5 (mark for deletion)
            ed.NameSize = 0xE5;
            // Re-write the entry data
            ef.CreateNewEntry(ed);
            // Reset this guy's entry data
            this.EntryData = ed;
            
            // Update our progress
            fa.Progress = 2;
            OnFolderAction(ref fa);

            // Remove this folder from the parent's cached stuff
            if (this.Parent != null)
            {
                this.Parent.cachedFolders.Remove(this);
                this.Parent.cachedDeletedFolders.Add(this);
            }

            // Update the FINAL progress
            EntryEventArgs eea = new EntryEventArgs();
            eea.FullParentPath = Parent.FullPath;
            eea.ModifiedEntry = this;
            eea.ParentFolder = this.Parent;
            eea.Deleting = true;
            if (this.Parent != null && !this.Parent.EntryEventNull)
            {
                Parent.OnEntryEvent(ref eea);
            }
            else if (!this.EntryEventNull)
            {
                this.OnEntryEvent(ref eea);
            }
        }

        public void ResetFolderAction()
        {
            fa = new FolderAction();
        }

        /// <summary>
        /// Creates a new file from the given path
        /// </summary>
        /// <param name="Path">Path to the file</param>
        /// <returns>Returns a WriteResult with a bool indicating whether or not there were problems writing the file.  If true, then the Entry property will be the conflicting entry</returns>
        public Structs.WriteResult CreateNewFile(string Path)
        {
            if (fa.Cancel)
            {
                return new WriteResult()
                {
                    CouldNotWrite = false,
                };
            }
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            EntryEventArgs eea = new EntryEventArgs();
            string NewName = System.IO.Path.GetFileName(Path);
            fa.CurrentFilePath = this.FullPath + "\\" + NewName;
            fa.CurrentFile = NewName;
            fa.MaxValue = 1;
            fa.Progress = 0;
            OnFolderAction(ref fa);
            WriteResult Return = new WriteResult();
            Console.WriteLine("Comparing folder/file names");
            foreach (Folder f in Folders())
            {
                if (f.Name.ToLower() == NewName.ToLower())
                {
                    Return.Entry = f;
                    Return.CouldNotWrite = true;
                    Return.ConflictingEntryPath = f.FullPath;
                    return Return;
                }
            }

            foreach (File f in Files())
            {
                if (f.Name.ToLower() == NewName.ToLower())
                {
                    Return.Entry = f;
                    Return.CouldNotWrite = true;
                    Return.ConflictingEntryPath = f.FullPath;
                    return Return;
                }
            }
            if (new System.IO.FileInfo(Path).Length == 0)
            {
                Console.WriteLine("Creating null file");
                File f = new File(this.PartitionInfo, new EntryFunctions(this).GetNewEntry(this, 0, new CLKsFATXLib.Geometry.Flags[0], NewName), this.Drive);
                f.Parent = this;
                f.FullPath = this.FullPath + "\\" + f.Name;
                Return.Entry = f;
                eea.ModifiedEntry = f;
            }
            else
            {
                Console.WriteLine(sw.Elapsed.ToString());
                Console.WriteLine("Getting blocks needed");
                int BlocksNeeded = (int)(VariousFunctions.UpToNearestCluster(new System.IO.FileInfo(Path).Length, PartitionInfo.ClusterSize) / PartitionInfo.ClusterSize);
                Console.WriteLine(sw.Elapsed.ToString());
                List<uint> Blocks = new List<uint>();
                Console.WriteLine("Getting new entry");
                EntryData newE = new EntryFunctions(this).GetNewEntry(this, (uint)new System.IO.FileInfo(Path).Length, new CLKsFATXLib.Geometry.Flags[0], NewName);
                if (newE.EntryOffset >= VariousFunctions.GetBlockOffset(this.BlocksOccupied[BlocksOccupied.Length - 1], this) + this.PartitionInfo.ClusterSize)
                {
                    List<uint> blocks = this.BlocksOccupied.ToList();
                    blocks.Add(VariousFunctions.GetBlockFromOffset(newE.EntryOffset, this.PartitionInfo));
                    this.BlocksOccupied = blocks.ToArray();
                }
                Console.WriteLine(sw.Elapsed.ToString());
                Blocks.Add(newE.StartingCluster);
                try
                {
                    Console.WriteLine("Getting free blocks");
                    Blocks.AddRange(Drive.GetFreeBlocks(this, BlocksNeeded - 1, newE.StartingCluster, 0, false));
                    Console.WriteLine(sw.Elapsed.ToString());
                }
                // This excpetion here is going to be that we're out of space...
                catch (Exception x)
                {
                    Console.WriteLine("Exception thrown, deleting written entry");
                    // Delete this entry
                    newE.NameSize = 0xE5;
                    // Create a new entry functions class so we can get rid of this entry
                    EntryFunctions ef = new EntryFunctions(this);
                    // Clear the FAT chain
                    Console.WriteLine("Clearing FAT chain");
                    ef.ClearFATChain(new uint[] { newE.StartingCluster });
                    // Mark this entry as deleted
                    ef.CreateNewEntry(newE);
                    throw x;
                }

                Console.WriteLine("Writing FAT chain");
                // Write that FAT chain
                new EntryFunctions(this).WriteFATChain(Blocks.ToArray());
                Console.WriteLine(sw.Elapsed.ToString());

                // Write the file data...
                // FileAction to be used
                fa.MaxValue = Blocks.Count;

                // The IO to read the file
                Streams.Reader FileReader = null;
                try
                {
                    FileReader = new CLKsFATXLib.Streams.Reader(new System.IO.FileStream(Path, System.IO.FileMode.Open));
                }
                catch
                {
                    System.Threading.Thread.Sleep(1000);
                    try
                    {
                        FileReader = new CLKsFATXLib.Streams.Reader(new System.IO.FileStream(Path, System.IO.FileMode.Open));
                    }
                    catch (Exception x)
                    {
                        Console.WriteLine("Exception thrown, deleting written entry");
                        // Delete this entry
                        newE.NameSize = 0xE5;
                        // Create a new entry functions class so we can get rid of this entry
                        EntryFunctions ef = new EntryFunctions(this);
                        // Clear the FAT chain
                        Console.WriteLine("Clearing FAT chain");
                        ef.ClearFATChain(Blocks.ToArray());
                        // Mark this entry as deleted
                        ef.CreateNewEntry(newE);
                        throw x;
                    }
                }
                // The IO to write to the destination file
                Streams.Writer FileWriter = Drive.Writer();
                // Loop for each block...
                for (int i = 0; i < Blocks.Count - 1; i++)
                {
                    if (fa.Cancel)
                    {
                        Console.WriteLine("Cancel engaged");
                        FileReader.Close();
                        File newfile = new File(this.PartitionInfo, newE, this.Drive);
                        newfile.Parent = this;
                        newfile.FullPath = this.FullPath + "\\" + newfile.Name;
                        this.cachedFiles.Add(newfile);

                        eea.FullParentPath = FullPath;
                        eea.ModifiedEntry = newfile;
                        eea.ParentFolder = this;
                        OnEntryEvent(ref eea);
                        UpdateModifiedTime();

                        Return.Entry = newfile;
                        return Return;
                    }
                    // Set the position to the beginning of the block
                    FileWriter.BaseStream.Position = VariousFunctions.GetBlockOffset(Blocks[i], this);
                    for (int j = 1, k = 0; j <= 0x100; j++, k++)
                    {
                        if (i + k == Blocks.Count - 1)
                        {
                            //Console.WriteLine("Writing part of file");
                            FileWriter.Write(FileReader.ReadBytes((int)PartitionInfo.ClusterSize * k));
                            i += k;
                            fa.Progress += k;
                            break;
                        }
                        else if (Blocks[i + k] == Blocks.Count - 2 || Blocks[i + k] + 1 != Blocks[i + j] || j == 10)
                        {
                            //Console.WriteLine("Writing part of file");
                            FileWriter.Write(FileReader.ReadBytes((int)PartitionInfo.ClusterSize * j));
                            i += k;
                            fa.Progress += j;
                            break;
                        }
                    }
                    OnFolderAction(ref fa);
                }
                Console.WriteLine(sw.Elapsed.ToString());
                // For the last cluster, we don't know how long it is... so we use
                // this nifty function I made to do that for us
                Console.WriteLine("Seeking to final cluster");
                FileWriter.BaseStream.Position = VariousFunctions.GetBlockOffset(Blocks[Blocks.Count - 1], this);
                // Read/write data
                byte[] ToWrite = new byte[(int)VariousFunctions.UpToNearest200(VariousFunctions.RemainingData(BlocksNeeded, new System.IO.FileInfo(Path).Length, this))];
                if ((int)VariousFunctions.RemainingData(BlocksNeeded, new System.IO.FileInfo(Path).Length, this) < (int)VariousFunctions.UpToNearest200((int)VariousFunctions.RemainingData(BlocksNeeded, new System.IO.FileInfo(Path).Length, this)))
                {
                    for (int i = (int)VariousFunctions.RemainingData(BlocksNeeded, new System.IO.FileInfo(Path).Length, this) + 1; i < ToWrite.Length; i++)
                    {
                        ToWrite[i] = 0xFF;
                    }
                }
                byte[] Buffer = FileReader.ReadBytes((int)VariousFunctions.RemainingData(BlocksNeeded, new System.IO.FileInfo(Path).Length, this));
                Array.Copy(Buffer, 0, ToWrite, 0, Buffer.Length);
                Buffer = null;
                Console.WriteLine("Writing final cluster");
                FileWriter.Write(ToWrite);
                fa.Progress++;
                OnFolderAction(ref fa);
                Console.WriteLine("Closing streams");
                FileReader.Close();

                Console.WriteLine(sw.Elapsed.ToString());

                File newF = new File(this.PartitionInfo, newE, this.Drive);
                newF.Parent = this;
                newF.FullPath = this.FullPath + "\\" + newF.Name;
                this.cachedFiles.Add(newF);
                Return.Entry = newF;
                eea.ModifiedEntry = newF;
            }
            eea.FullParentPath = FullPath;
            eea.ParentFolder = this;
            OnEntryEvent(ref eea);
            UpdateModifiedTime();
            return Return;
        }

        internal bool EntryEventNull
        {
            get
            {
                return Drive.EntryWatcherNull;
            }
        }

        /// <summary>
        /// Injects a directory (all child files and folders) in to the current directory
        /// </summary>
        /// <returns>List of entries that could not be written due to existing entries with the same name</returns>
        public ExistingEntry[] InjectFolder(string FolderPath, bool MergeFolders, bool OverwriteFiles)
        {
            List<ExistingEntry> ex = new List<ExistingEntry>();
            // Check the folder name
            bool FolderFound = false;
            Folder Wanted = null;
            foreach (Folder f in this.Folders())
            {
                if (fa.Cancel)
                {
                    fa.Cancel = false;
                    return ex.ToArray();
                }
                if (!f.IsDeleted && f.Name.ToLower() == new System.IO.DirectoryInfo(FolderPath).Name.ToLower())
                {
                    if (!MergeFolders)
                    {
                        ExistingEntry newex = new ExistingEntry();
                        newex.Existing = f;
                        newex.NewPath = FolderPath;
                        ex.Add(newex);
                    }
                    Wanted = f;
                    FolderFound = true;
                    break;
                }
            }

            if (!FolderFound || MergeFolders)
            {
                if (!FolderFound)
                {
                    CreateNewFolder(new System.IO.DirectoryInfo(FolderPath).Name);
                    Wanted = Folders()[Folders().Length - 1];
                }
                Wanted.fa = this.fa;
                Wanted.FolderAction += this.FolderActionInternal;
                foreach (System.IO.FileInfo fi in new System.IO.DirectoryInfo(FolderPath).GetFiles())
                {
                    if (fa.Cancel)
                    {
                        fa.Cancel = false;
                        return ex.ToArray();
                    }
                    Structs.WriteResult wr = Wanted.CreateNewFile(fi.FullName);
                    if (wr.CouldNotWrite && !OverwriteFiles)
                    {
                        ExistingEntry exe = new ExistingEntry();
                        exe.Existing = wr.Entry;
                        exe.NewPath = fi.FullName;
                        ex.Add(exe);
                    }
                    else if (wr.CouldNotWrite && OverwriteFiles)
                    {
                        File f = (File)wr.Entry;
                        f.Delete();
                        Wanted.CreateNewFile(fi.FullName);
                    }
                }
                foreach (System.IO.DirectoryInfo di in new System.IO.DirectoryInfo(FolderPath).GetDirectories())
                {
                    ex.AddRange(Wanted.InjectFolder(di.FullName, MergeFolders, OverwriteFiles));
                }
            }
            UpdateModifiedTime();
            return ex.ToArray();
        }

        /// <summary>
        /// Checks if the file exists in this folder and tries to return it
        /// </summary>
        /// <returns>File</returns>
        public File GetFile(string Name)
        {
            foreach (File f in Files())
            {
                if (f.Name.ToLower() == Name.ToLower())
                {
                    return f;
                }
            }
            throw new Exception("File not found!  Name: " + Name);
        }

        /// <summary>
        /// Checks if the folder exists in this folder and tries to return it
        /// </summary>
        /// <returns>Folder</returns>
        public Folder GetFolder(string Name)
        {
            foreach (Folder f in Folders())
            {
                if (f.Name.ToLower() == Name.ToLower())
                {
                    return f;
                }
            }
            throw new Exception("Folder not found!  Name: " + Name);
        }
    }
}
