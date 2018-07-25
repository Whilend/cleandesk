using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CleanDesk
{

    class UserDesktop
    {
        private FileCategories categories;
        private string desktop;

        public UserDesktop(FileCategories categories)
        {
            this.categories = categories;
            this.desktop = GetDesktopDirectory() + "\\";
        }

        public void Clean()
        {
            string[] folders = categories.GetFolderNames();
            string[] files = GetDesktopIcons();

            foreach (var folder in folders)
                if (!Directory.Exists(desktop + folder))
                    Directory.CreateDirectory(desktop + folder);

            foreach (var file in GetDesktopIcons())
            {
                try
                {
                    string name = Path.GetFileName(file);
                    string directory = categories.GetDirectory(file) + "\\";

                    if (folders.Contains(name, false)) continue;

                    if (Directory.Exists(file))
                    {
                        Directory.Move(file, desktop + directory + name);
                    }
                    else if (File.Exists(file))
                    {
                        File.Move(file, desktop + directory + name);
                    }
                }
                catch (Exception ex)
                {
                    Debug.Instance.Error(ex.Message);
                }
            }

        }

        public static string GetDesktopDirectory(bool user = true)
        {
            return Environment.GetFolderPath(user ? Environment.SpecialFolder.DesktopDirectory : Environment.SpecialFolder.CommonDesktopDirectory);
        }

        public static string[] GetDesktopIcons()
        {
            string[] user = Directory.GetFileSystemEntries(GetDesktopDirectory());
            string[] common = Directory.GetFileSystemEntries(GetDesktopDirectory(false), "*.lnk");

            return ArrayHelper.Merge(user, common);
        }
    }
}
