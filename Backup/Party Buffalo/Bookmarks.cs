using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Party_Buffalo
{
    public class Bookmarks
    {
        string FilePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Party Buffalo Drive Explorer\\Bookmarks.xml";
        string FolderPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Party Buffalo Drive Explorer";
        public struct BookmarkData
        {
            public string Name;
            public string Path;
        }

        public void CreateNewBookmark(string Name, string Path)
        {
            XmlDocument doc = CheckFile(FilePath, FolderPath);
            XmlNode Root = doc.ChildNodes[1];

            XmlNode Bookmark = doc.CreateElement("Bookmark");
            XmlAttribute at = doc.CreateAttribute("name");
            at.Value = Name;
            Bookmark.Attributes.Append(at);
            Root.AppendChild(Bookmark);

            // Create our bookmark path
            XmlNode path = doc.CreateElement("Path");
            path.InnerText = Path;
            Bookmark.AppendChild(path);
            doc.Save(FilePath);
        }

        public BookmarkData[] GetBookmarks()
        {
            XmlDocument doc = CheckFile(FilePath, FolderPath);
            List<BookmarkData> toReturn = new List<BookmarkData>();
            for (int i = 0; i < doc.ChildNodes[1].ChildNodes.Count; i++)
            {
                BookmarkData bd = new BookmarkData();
                bd.Name = doc.ChildNodes[1].ChildNodes[i].Attributes[0].Value;
                bd.Path = doc.ChildNodes[1].ChildNodes[i].ChildNodes[0].InnerText;
                toReturn.Add(bd);
            }
            return toReturn.ToArray();
        }

        public void DeleteBookmark(BookmarkData Bookmark)
        {
            XmlDocument doc = CheckFile(FilePath, FolderPath);
            for (int i = 0; i < doc.ChildNodes[1].ChildNodes.Count; i++)
            {
                BookmarkData bd = new BookmarkData();
                bd.Name = doc.ChildNodes[1].ChildNodes[i].Attributes[0].Value;
                bd.Path = doc.ChildNodes[1].ChildNodes[i].ChildNodes[0].InnerText;
                if (bd.Name == Bookmark.Name && bd.Path == Bookmark.Path)
                {
                    doc.ChildNodes[1].RemoveChild(doc.ChildNodes[1].ChildNodes[i]);
                    doc.Save(FilePath);
                    break;
                }
            }
        }

        public bool CheckIfBookmarkExists(string Name, string Path)
        {
            XmlDocument doc = CheckFile(FilePath, FolderPath);
            for (int i = 0; i < doc.ChildNodes[1].ChildNodes.Count; i++)
            {
                BookmarkData bd = new BookmarkData();
                bd.Name = doc.ChildNodes[1].ChildNodes[i].Attributes[0].Value;
                bd.Path = doc.ChildNodes[1].ChildNodes[i].ChildNodes[0].InnerText;
                if (bd.Name == Name && bd.Path == Path)
                {
                    doc.ChildNodes[1].ChildNodes[i].RemoveChild(doc.ChildNodes[1].ChildNodes[i]);
                    return true;
                }
            }
            return false;
        }

        private XmlDocument CheckFile(string FilePath, string FolderPath)
        {
            System.Xml.XmlDocument doc = new XmlDocument();
            if (!System.IO.Directory.Exists(FolderPath))
            {
                System.IO.Directory.CreateDirectory(FolderPath);
            }
            if (!System.IO.File.Exists(FilePath))
            {
                // Create the file if it doesn't exist
                doc.AppendChild(doc.CreateXmlDeclaration("1.0", "UTF-8", null));
                XmlNode BookMarks = doc.CreateElement("Bookmarks");
                doc.AppendChild(BookMarks);

                XmlNode Bookmark = doc.CreateElement("Bookmark");
                XmlAttribute at = doc.CreateAttribute("name");
                at.Value = "Content";
                Bookmark.Attributes.Append(at);
                BookMarks.AppendChild(Bookmark);

                // Create our bookmark path
                XmlNode Path = doc.CreateElement("Path");
                Path.InnerText = "Data\\Content";
                Bookmark.AppendChild(Path);

                Bookmark = doc.CreateElement("Bookmark");
                at = doc.CreateAttribute("name");
                at.Value = "LIVE/Global Content";
                Bookmark.Attributes.Append(at);
                BookMarks.AppendChild(Bookmark);

                // Create our bookmark path
                Path = doc.CreateElement("Path");
                Path.InnerText = "Data\\Content\\0000000000000000";
                Bookmark.AppendChild(Path);

                doc.Save(FilePath);
            }
            else
            {
                doc.Load(FilePath);
            }
            return doc;
        }
    }
}
