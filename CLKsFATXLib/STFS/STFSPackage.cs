using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CLKsFATXLib.STFS
{
    class STFSPackage
    {
        public STFSPackage(System.IO.Stream InStream)
        {
            PackageStream = InStream;
            if (!IsSTFSPackage())
            {
                Close();
                throw new Exception("File is not a valid STFS package!");
            }
        }

        /// <summary>
        /// If the file is an STFS package, it will get the icon for the game
        /// </summary>
        public System.Drawing.Image TitleIcon()
        {
            // Get a new reader for this file
            Streams.Reader r = new CLKsFATXLib.Streams.Reader(PackageStream);
            r.BaseStream.Position = (long)Geometry.STFSOffsets.TitleImageSize;
            int Size = r.ReadInt32();
            r.BaseStream.Position = (long)Geometry.STFSOffsets.TitleImage;
            try
            {
                return System.Drawing.Image.FromStream(new System.IO.MemoryStream(r.ReadBytes(Size)));
            }
            catch { return null; }
        }

        /// <summary>
        /// If the file is an STFS package, it will get the package's icon (non-game icon)
        /// </summary>
        /// <returns>STFS package content icon</returns>
        public System.Drawing.Image ContentIcon()
        {
            // Get a new reader for this file
            Streams.Reader r = new CLKsFATXLib.Streams.Reader(PackageStream);
            r.BaseStream.Position = (long)Geometry.STFSOffsets.ContentImageSize;
            int Size = r.ReadInt32();
            r.BaseStream.Position = (long)Geometry.STFSOffsets.ContentImage;
            try
            {
                return System.Drawing.Image.FromStream(new System.IO.MemoryStream(r.ReadBytes(Size)));
            }
            catch { return null; }
        }

        /// <summary>
        /// Checks the file magic to check if it's a valid CON/LIVE/PIRS package
        /// </summary>
        public bool IsSTFSPackage()
        {
            if (PackageStream.Length >= 0xA000)
            {
                Streams.Reader r = new CLKsFATXLib.Streams.Reader(PackageStream);
                r.BaseStream.Position = 0;
                byte[] Buffer = r.ReadBytes(0x200);
                
                r = new CLKsFATXLib.Streams.Reader(new System.IO.MemoryStream(Buffer));
                uint val = r.ReadUInt32();
                
                switch (val)
                {
                    // CON
                    case 0x434F4E20:
                        return true;
                    // LIVE
                    case 0x4C495645:
                        return true;
                    // PIRS
                    case 0x50495253:
                        return true;
                    // NIGR
                    default:
                        return true;
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// If the file is an STFS package, it will get the package's display (content) name
        /// </summary>
        /// <returns>Package display/content name</returns>
        public string ContentName()
        {
            Streams.Reader r = new CLKsFATXLib.Streams.Reader(PackageStream);
            r.BaseStream.Position = (long)Geometry.STFSOffsets.DisplayName;
            return r.ReadUnicodeString(0x80);
        }

        /// <summary>
        /// If the file is an STFS package, it will get the name of the game that the package belongs to
        /// </summary>
        public string TitleName()
        {
            Streams.Reader r = new CLKsFATXLib.Streams.Reader(PackageStream);
            r.BaseStream.Position = (long)Geometry.STFSOffsets.TitleName;
            return r.ReadUnicodeString(0x80);
        }

        /// <summary>
        /// If the file is an STFS package, it will get the game's unique identifier
        /// </summary>
        public uint TitleID()
        {
            Streams.Reader r = new CLKsFATXLib.Streams.Reader(PackageStream);
            r.BaseStream.Position = (long)Geometry.STFSOffsets.TitleID;
            return r.ReadUInt32();
        }

        /// <summary>
        /// If the file is an STFS package, it will get the package owner's profile ID
        /// </summary>
        public byte[] ProfileID()
        {
            Streams.Reader r = new CLKsFATXLib.Streams.Reader(PackageStream);
            r.BaseStream.Position = (long)Geometry.STFSOffsets.ProfileID;
            return r.ReadBytes(0x8);
        }

        /// <summary>
        /// If the file is an STFS package, it will get the device ID that the package was created on (HDD/USB)
        /// </summary>
        /// <returns>Package device ID</returns>
        public byte[] DeviceID()
        {
            Streams.Reader r = new CLKsFATXLib.Streams.Reader(PackageStream);
            r.BaseStream.Position = (long)Geometry.STFSOffsets.DeviceID;
            return r.ReadBytes(0x14);
        }

        /// <summary>
        /// If the file is an STFS package, it will get the identifier that the package was created on.
        /// </summary>
        /// <returns>Console ID for STFS package</returns>
        public byte[] ConsoleID()
        {
            Streams.Reader r = new CLKsFATXLib.Streams.Reader(PackageStream);
            r.BaseStream.Position = (long)Geometry.STFSOffsets.ConsoleID;
            return r.ReadBytes(0x5);
        }


        private System.IO.Stream PackageStream
        {
            get;
            set;
        }

        public void Close()
        {
            PackageStream.Flush();
            PackageStream.Close();
        }
    }
}
