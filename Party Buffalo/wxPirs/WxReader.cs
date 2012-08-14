/**
*   Title:          wxPIRS.
*   Description:    The wxPIRS application will disassamble STFS packages and see their contents.
*
*   Original Author: wxPIRS - gael360
*   License:        http://www.gnu.org/licenses/gpl.html GNU GENERAL PUBLIC LICENSE
*   Link:           http://gael360.free.fr/
*   Note:           This comment will appear before any of the code developed by obriej62@mail.dcu.ie
*/
namespace WxTools
{
    using System;
    using System.Globalization;
    using System.IO;

    internal class WxReader
    {
        public string extractFileName(string fullName)
        {
            try
            {
                return fullName.Substring(fullName.LastIndexOf(@"\") + 1);
            }
            catch (ArgumentOutOfRangeException)
            {
                return "";
            }
        }

        public string extractFolderName(string fullName)
        {
            try
            {
                return fullName.Substring(0, fullName.LastIndexOf(@"\"));
            }
            catch (ArgumentOutOfRangeException)
            {
                return "";
            }
        }

        public int HexToInt(string hex)
        {
            if (hex.Substring(0, 2).ToLower() == "0x")
            {
                hex = hex.Substring(2);
            }
            try
            {
                return int.Parse(hex, NumberStyles.HexNumber);
            }
            catch (FormatException)
            {
                return 0;
            }
        }

        public uint HexToUInt(string hex)
        {
            if (hex.Substring(0, 2).ToLower() == "0x")
            {
                hex = hex.Substring(2);
            }
            try
            {
                return uint.Parse(hex, NumberStyles.HexNumber);
            }
            catch (FormatException)
            {
                return 0;
            }
        }

        public short readInt16(BinaryReader br)
        {
            short num = 0;
            for (int i = 0; i < 2; i++)
            {
                num = (short) (num << 8);
                num = (short) (num + Convert.ToInt16(br.ReadByte()));
            }
            return num;
        }

        public int readInt32(BinaryReader br)
        {
            int num = 0;
            for (int i = 0; i < 4; i++)
            {
                num *= 0x100;
                num += br.ReadByte();
            }
            return num;
        }

        public string readString(BinaryReader br, uint nbchar)
        {
            string str = "";
            for (uint i = 0; i < nbchar; i++)
            {
                char ch = br.ReadChar();
                if (ch != '\0')
                {
                    str = str + Convert.ToString(ch);
                }
            }
            return str;
        }

        public ushort readUInt16(BinaryReader br)
        {
            ushort num = 0;
            for (int i = 0; i < 2; i++)
            {
                num = (ushort) (Convert.ToUInt16((int) (num * 0x100)) + Convert.ToUInt16(br.ReadByte()));
            }
            return num;
        }

        public uint readUInt32(BinaryReader br)
        {
            uint num = 0;
            for (int i = 0; i < 4; i++)
            {
                num *= 0x100;
                num += br.ReadByte();
            }
            return num;
        }

        public string readWideString(BinaryReader br, uint nbchar)
        {
            string str = "";
            for (uint i = 0; i < nbchar; i++)
            {
                br.ReadChar();
                str = str + Convert.ToString(br.ReadChar());
            }
            return str;
        }

        public string unicodeToStr(byte[] data)
        {
            return this.unicodeToStr(data, 0, data.Length);
        }

        public string unicodeToStr(byte[] data, int start)
        {
            return this.unicodeToStr(data, start, data.Length);
        }

        public string unicodeToStr(byte[] data, int start, int length)
        {
            string str = "";
            for (int i = start; i < data.Length; i += 2)
            {
                if (data[i] == 0)
                {
                    return str;
                }
                str = str + Convert.ToChar(data[i]);
            }
            return str;
        }
    }
}

