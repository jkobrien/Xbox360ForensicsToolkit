using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Net;
using System.IO;

namespace Party_Buffalo
{
    public class Update
    {
        [DllImport("wininet.dll")]
        private extern static bool InternetGetConnectedState( out int Description, int ReservedValue ) ;

        public struct UpdateInfo
        {
            public bool Update;
            public bool DLLNeeded;
            public string UpdateVersion;
            public string UpdateText;
            public string UpdatePath;
            public string UpdateDLLPath;
            public string QuickMessage;
        }

        /// <summary>
        /// Checks the current internet status of the computer
        /// </summary>
        public bool CheckInternetConnection()
        {
            int Desc;
            return InternetGetConnectedState(out Desc, 0);
        }


        /// <summary>
        /// Checks for new updates for the application
        /// </summary>
        public UpdateInfo CheckForUpdates(Uri relativeURL)
        {
            UpdateInfo info = new UpdateInfo();
            info.Update = false;
            string[] CurrentVersion = new string[4];
            // To see if the user is connected to the internet...
            if (!CheckInternetConnection())
            {
                // return they have no internet connection
                return info;
            }
            try
            {
                System.Net.HttpWebRequest wr = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(relativeURL.ToString());
                wr.UserAgent = "PARTY BUFFALO\r\n";
                System.Net.WebResponse response = wr.GetResponse();
                Stream responseStream = response.GetResponseStream();
                System.Xml.XmlTextReader r = new System.Xml.XmlTextReader(responseStream);
                while (r.Read())
                {
                    if (r.NodeType == System.Xml.XmlNodeType.Element)
                    {
                        // If we're getting the current version that's available
                        if (r.Name == "CurrentVersion")
                        {
                            // Set the update version to the inner text
                            info.UpdateVersion = r.ReadInnerXml();
                            // Split it at every instance of a period so that
                            // we can compare each number individually
                            CurrentVersion = info.UpdateVersion.Split('.');
                            // Get our current version, split that as well
                            string[] AppVersion = System.Windows.Forms.Application.ProductVersion.Split('.');
                            // Create new int arrays that contain the current &
                            // updated versions
                            int[] Current = new int[4];
                            int[] App = new int[4];
                            // Set their indexes accordingly
                            for (int i = 0; i < Current.Length; i++)
                            {
                                Current[i] = Convert.ToInt32(CurrentVersion[i]);
                                App[i] = Convert.ToInt32(AppVersion[i]);
                            }

                            // If the major is larger than current, there's an update
                            if (Current[0] > App[0])
                            {
                                info.Update = true;
                                return info;
                            }
                            else if (Current[0] < App[0])
                            {
                                return info;
                            }
                            // If the minor is larger than the current, there's an update :O
                            if (Current[1] > App[1])
                            {
                                info.Update = true;
                                return info;
                            }
                            else if (Current[1] < App[1])
                            {
                                return info;
                            }
                            // Same for build
                            if (Current[2] > App[2])
                            {
                                info.Update = true;
                                return info;
                            }
                            else if (Current[2] < App[2])
                            {
                                return info;
                            }
                            // Same for revision
                            if (Current[3] > App[3])
                            {
                                info.Update = true;
                                return info;
                            }
                            else if (Current[3] < App[3])
                            {
                                return info;
                            }
                            // None of those guys returned anything -- they must have
                            // the current version, return our info
                            return info;
                        }

                        if (r.Name == "UpdateFixes")
                        {
                            info.UpdateText = r.ReadInnerXml();
                        }

                        if (r.Name == "UpdatePath")
                        {
                            info.UpdatePath = r.ReadInnerXml();
                        }

                        if (r.Name == "UpdateDLLPath")
                        {
                            info.UpdateDLLPath = r.ReadInnerXml();
                        }

                        if (r.Name == "DLLNeeded")
                        {
                            if (r.ReadInnerXml() == "true")
                            {
                                info.DLLNeeded = true;
                            }
                            else
                            {
                                info.DLLNeeded = false;
                            }
                        }

                        if (r.Name == "QuickMessage")
                        {
                            info.QuickMessage = r.ReadInnerXml();
                        }
                    }
                }
            }
            catch { return info; }
            return info;
        }

        /// <summary>
        /// Downloads the update for the application using the specified update information
        /// </summary>
        public bool DownloadUpdate(UpdateInfo ui, Forms.UpdateDownloadForm sender, string DownloadPath)
        {
            try
            {
                // This will get the new exe's name
                string updateName = DownloadPath;
                // Create our new web client so we can download the file
                System.Net.WebClient wc = new System.Net.WebClient();
                //System.IO.File.Move(System.Windows.Forms.Application.ExecutablePath, System.Windows.Forms.Application.StartupPath + "\\Party Buffalo_old.exe");
                wc.DownloadProgressChanged += (o, f) =>
                    {
                        sender.ProgressBarMax = (int)f.TotalBytesToReceive;
                        sender.ProgressBarValue = (int)f.BytesReceived;
                    };
                wc.DownloadFileAsync(new Uri(ui.UpdatePath), updateName);
                return true;
            }
            catch (Exception e) { System.Diagnostics.Process.Start(ui.UpdatePath); throw e; }
        }

        private delegate void CloseForm();
    }
}
