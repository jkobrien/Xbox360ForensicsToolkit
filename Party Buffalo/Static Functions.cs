using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CLKsFATXLib;

namespace Party_Buffalo
{
    static class Cache
    {
        // Checks the cache to see if a game name for a title ID is available
        public static string CheckCache(string ID)
        {
            // Check the labels...
            if (Properties.Settings.Default.label != null)
            {
                for (int i = 0; i < Properties.Settings.Default.label.Count; i++)
                {
                    if (ID.ToLower() == Properties.Settings.Default.labelPath[i].ToLower())
                    {
                        return Properties.Settings.Default.label[i];
                    }
                }
            }
            if (Properties.Settings.Default.cachedID == null)
            {
                return null;
            }

            // We didn't return null (meaning that it's already been created, has entries)
            for (int i = 0; i < Properties.Settings.Default.cachedID.Count; i++)
            {
                // If we found it...
                if (Properties.Settings.Default.cachedID[i].ToLower() == ID.ToLower())
                {
                    // Return its corresponding value in the other array...
                    return Properties.Settings.Default.correspondingIDName[i];
                }
            }

            for (int i = 0; i < CLKsFATXLib.VariousFunctions.Known.Length; i++)
            {
                if (ID.ToLower() == CLKsFATXLib.VariousFunctions.Known[i].ToLower())
                {
                    return CLKsFATXLib.VariousFunctions.KnownEquivilent[i];
                }
            }

            // We didn't find it, return null
            return null;
        }

        // Adds an ID to our cache...
        public static void AddID(string ID, string GameName)
        {
            if (Properties.Settings.Default.cachedID == null)
            {
                Properties.Settings.Default.cachedID = new System.Collections.Specialized.StringCollection();
                Properties.Settings.Default.correspondingIDName = new System.Collections.Specialized.StringCollection();
            }
            Properties.Settings.Default.cachedID.Add(ID);
            Properties.Settings.Default.correspondingIDName.Add(GameName);
            Properties.Settings.Default.Save();
        }

        public static void AddLabel(string Label, string FullPath)
        {
            if (Properties.Settings.Default.label == null)
            {
                Properties.Settings.Default.label = new System.Collections.Specialized.StringCollection();
                Properties.Settings.Default.labelPath = new System.Collections.Specialized.StringCollection();
            }
            Properties.Settings.Default.label.Add(Label);
            Properties.Settings.Default.labelPath.Add(FullPath);
            Properties.Settings.Default.Save();
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

        public static string ImageCachePath
        {
            get
            {
                string FolderPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Party Buffalo Drive Explorer";
                if (!System.IO.Directory.Exists(FolderPath))
                {
                    System.IO.Directory.CreateDirectory(FolderPath);
                }

                FolderPath += "\\Cached Icons";
                return FolderPath;
            }
        }

        public static void AddIcon(System.Drawing.Image Icon, string Name)
        {
            if (Icon == null)
            {
                return;
            }
            string FolderPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Party Buffalo Drive Explorer";
            if (!System.IO.Directory.Exists(FolderPath))
            {
                System.IO.Directory.CreateDirectory(FolderPath);
            }

            FolderPath += "\\Cached Icons";

            if (!System.IO.Directory.Exists(FolderPath))
            {
                System.IO.Directory.CreateDirectory(FolderPath);
            }
            try
            {
                Icon.Save(FolderPath + "\\" + Name + ".png");
            }
            catch { }
        }

        public static System.Drawing.Image GetIcon(string Name)
        {
            try
            {
                string FolderPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Party Buffalo Drive Explorer\\Cached Icons";
                if (!System.IO.Directory.Exists(FolderPath))
                {
                    return null;
                }

                if (System.IO.File.Exists(FolderPath + "\\" + Name + ".png"))
                {
                    return System.Drawing.Image.FromFile(FolderPath + "\\" + Name + ".png");
                }
                return null;
            }
            catch { return null; }
        }
    }
}
