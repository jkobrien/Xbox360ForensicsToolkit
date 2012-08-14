using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CLKsFATXLib.Geometry
{
    class I_Sell_Cocaine_and_Cocaine_Accessories
    {
        public string orly
        {
            get
            {
                return "yarly";
            }
        }
    }

    /* Some important kernel functions that i'll keep here...
     * -> FatxProcessBootSector (processes the partition header, determines if the partition is stable
     * -> SataDiskAuthenticateDevice (checks the security sector to make sure it's an OEM drive)
     * -> SataCreateDevkitPartitions (formats a drive for dev use)
     */

    public enum STFSOffsets
    {
        TitleID = 0x360,
        ConsoleID = 0x36c,
        DeviceID = 0x3FD,
        DisplayName = 0x411,
        TitleName = 0x1691,
        ProfileID = 0x371,
        ContentImage = 0x171A,
        ContentImageSize = 0x1712,
        TitleImageSize = 0x1716,
        TitleImage = 0x571A,
    }

    public enum USBOffsets
    {
        aSystem_Aux = 0x8115200,
        aSystem_Extended = 0x12000400,
        Cache = 0x8000400,
        Data = 0x20000000,
    }

    public enum USBPartitionSizes
    {
        Cache = 0x47FF000,//0x4000000,//0x12000400,//0x47FF000,
        Cache_NoSystem = 0x4000000,
        System_Aux = 0x8000000,
        System_Extended = 0xDFFFC00,
    }

    public enum NANDOffsets
    {
        System = 0x3190800,
        Cache = 0x3CE8800,
        Data = 0x1E600300,
    }

    public enum Flags : byte
    {
        /// <summary>
        /// The file or directory is read-only. Applications can read the file but cannot write to it or delete it. In the case of a directory, applications cannot delete it. FATX does not support read-only files.
        /// </summary>
        ReadOnly,
        Hidden,
        System,
        Volume,
        /// <summary>
        /// This attribute identifies a directory.
        /// </summary>
        Directory,
        /// <summary>
        ///  The file or directory is an archive file or directory. Applications use this attribute to mark files for backup or removal. 
        /// </summary>
        Archive,
        /// <summary>
        /// Reserved; do not use.
        /// </summary>
        Device,
        Unused,
        Deleted = 0xE5,
    }

    public enum EntryOffsets : int
    {
        /// <summary>
        /// Offset for the size of the file name
        /// </summary>
        NameSize = 0x0,
        /// <summary>
        /// Offset for file flags
        /// </summary>
        Flags = 0x1,
        /// <summary>
        /// Offset for file name
        /// </summary>
        FileName = 0x2,
        /// <summary>
        /// Offset for starting cluster
        /// </summary>
        StartingCluster = 0x2C,
        /// <summary>
        /// Offset for the file size
        /// </summary>
        Size = 0x30,
        CreationDate = 0x34,
        CreationTime = 0x36,
        AccessDate = 0x38,
        AccessTime = 0x3A,
        ModifiedDate = 0x3C,
        ModifiedTime = 0x3E,
    }

    public enum HDDLengths : long
    {
        /// <summary>
        /// Length of the System Cache partition
        /// </summary>
        SystemCache = 0x80000000,
        /// <summary>
        /// Length of the Game Cache partition
        /// </summary>
        GameCache = 0xA0E30000,
        /// <summary>
        /// Length of the Compatibility partition
        /// </summary>
        Compatibility = 0x10000000,
        System_Cache = 0xCE30000,
        System_Extended = 0x8000000,
        /// <summary>
        /// The maximum file size allowed by FATX
        /// </summary>
        MaxFileSize = 0x100000000,
        /// <summary>
        /// Sector size
        /// </summary>
        SectorSize = 0x200,
    }

    public enum DevOffsets : long
    {
        DEVKIT_ = 0xB6600000,
        Content = 0xC6600000,
    }

    public enum HDDOffsets : long
    {
        /// <summary>
        /// Offset of the data partition
        /// </summary>
        Data = 0x130EB0000,
        /// <summary>
        /// Offset of the Josh partition
        /// </summary>
        Josh = 0x800,
        /// <summary>
        /// Offset of the Security Sector
        /// </summary>
        SecuritySector = 0x2000,
        /// <summary>
        /// Offset of the System Cache partition
        /// </summary>
        SystemCache = 0x80000,
        /// <summary>
        /// Offset of the Game Cache partition
        /// </summary>
        GameCache = 0x80080000,
        /// <summary>
        /// ????????????
        /// </summary>
        System_Cache = 0x10C080000,
        /// <summary>
        ///  ??????
        /// </summary>
        System_Extended = 0x118EB0000,
        /// <summary>
        /// Offset of the Compatibility partition
        /// </summary>
        Compatibility = 0x120EB0000,
    }

    public enum Prefixes
    {
        XlfsUploader,
        NuiHiveSetting,
        NuiTroubleShooter,
        NuiBiometric,
        NuiSession,
        ValidCert,
        CertStorage,
        AvatarGamerTile,
        ProfileSettings,
        QosHistory,
        MessengerBuddies,
        GameTile,
        TitleUpdate,
        TitleName,
        Tickets,
        SystemUpdate,
        SPA,
        GamerTile,
        GameInvite,
        GamerTag,
        FriendMuteList,
        DashboardApp,
        CustomGamerTile,
        AchievementTile,
    }

    public static class CacheFilePrefixes
    {
        public static string[] CachePrefixes = 
        {
            "XL", "NH", "TS", "NB", "NS", "VC",
            "CA", "AV", "PS", "QH", "MB", "TT",
            "TU", "TN", "TK", "SU", "SP", "GT",
            "GI", "GA", "FM", "DA", "CT", "AT",
        };

        public static string[] PrefixNames =
        {
            "XLFS Uploader", "NUI Hive Setting", "NUI Troubleshooter", "NUI Biometric", "NUI Session", "Valid Cert",
            "Cert Storage", "Avatar Gamer Tile", "Profile Settings", "QoS History", "Messenger Buddies", "Game Tile",
            "Title Update", "Title Name", "Tickets", "System Update", "SPA", "Gamer Tile",
            "Game Invite", "Gamertag", "Friend Mute List", "Dashboard App", "Custom Gamer Tile", "Achievement Tile",
        };

        public static string GetPrefix(Prefixes Prefix)
        {
            return CachePrefixes[(int)Prefix];
        }

        public static string GetPrefixName(Prefixes Prefix)
        {
            return PrefixNames[(int)Prefix];
        }
    }
}
