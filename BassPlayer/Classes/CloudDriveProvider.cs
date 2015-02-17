using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace BassPlayer.Classes
{
    internal enum CloudDrives
    {
        Google,
        Dropbox,
        OneDrive
    }

    internal static class CloudDriveProvider
    {
        private static string GetDropboxPath()
        {
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string dbPath = System.IO.Path.Combine(appDataPath, "Dropbox\\host.db"); string[] lines = System.IO.File.ReadAllLines(dbPath);
            byte[] dbBase64Text = Convert.FromBase64String(lines[1]);
            return System.Text.ASCIIEncoding.ASCII.GetString(dbBase64Text);
        }
        private static string GetGoogleDrive()
        {
            string dbFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Google\\Drive\\sync_config.db");
            File.Copy(dbFilePath, "temp.db", true);
            string text = File.ReadAllText("temp.db", Encoding.ASCII);
            // The "29" refers to the end position of the keyword plus a few extra chars
            string trim = text.Substring(text.IndexOf("local_sync_root_pathvalue") + 29);
            // The "30" is the ASCII code for the record separator
            return trim.Substring(0, trim.IndexOf(char.ConvertFromUtf32(30)));
        }
        private static string GetOneDrivePath()
        {
            return Registry.GetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\SkyDrive", "UserFolder", null).ToString();
        }

        public static string GetPath(CloudDrives drive)
        {
            try
            {
                switch (drive)
                {
                    case CloudDrives.Google:
                        return GetGoogleDrive();
                    case CloudDrives.Dropbox:
                        return GetDropboxPath();
                    case CloudDrives.OneDrive:
                        return GetOneDrivePath();
                    default:
                        return string.Empty;
                }
            }
            catch (Exception) { return string.Empty; }
        }

        public static BitmapImage GetIcon(CloudDrives drive)
        {
            switch (drive)
            {
                case CloudDrives.Dropbox:
                    return new BitmapImage(new Uri("pack://application:,,,/BassPlayer;component/Images/filemanager/dropbox-50.png"));
                case CloudDrives.Google:
                    return new BitmapImage(new Uri("pack://application:,,,/BassPlayer;component/Images/filemanager/google_drive-50.png"));
                case CloudDrives.OneDrive:
                    return new BitmapImage(new Uri("pack://application:,,,/BassPlayer;component/Images/filemanager/skydrive-50.png"));
                default:
                    return null;
            }
        }

        public static CloudDrives[] AvailableDrives
        {
            get
            {
                List<CloudDrives> _list = new List<CloudDrives>();
                if (!string.IsNullOrEmpty(GetPath(CloudDrives.Dropbox))) _list.Add(CloudDrives.Dropbox);
                if (!string.IsNullOrEmpty(GetPath(CloudDrives.Google))) _list.Add(CloudDrives.Google);
                if (!string.IsNullOrEmpty(GetPath(CloudDrives.OneDrive))) _list.Add(CloudDrives.OneDrive);
                return _list.ToArray();
            }
        }
    }
}
