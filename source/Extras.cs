using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;

namespace CleanDesk
{
    public static class FileFilter
    {
        public static bool Test(string filter, string extension)
        {
            string ext = extension.ToLower();
            string[] filters = filter.ToLower().Split(' ');

            foreach (var f in filters)
                if (f == ext)
                    return true;

            return false;
        }

        /// <summary>
        /// Test the filter is correct for file
        /// </summary>
        /// <param name="filter">Filter (.ext1 .ext2 ...)</param>
        /// <param name="path">File path</param>
        /// <returns>True if filter is correct for file</returns>
        public static bool TestFile(string filter, string path)
        {
            if (filter == "<folder>")
            {
                return Directory.Exists(path);
            }
            else
                return Test(filter, Path.GetExtension(path));
        }
    }

    [Serializable]
    public struct FileCategory
    {
        public string Filter;
        public string Name;

        public static FileCategory Create(string filter, string name)
        {
            return new FileCategory
            {
                Filter = filter,
                Name = name
            };
        }

        public bool Test(string path)
        {
            return FileFilter.TestFile(Filter, path);
        }
    }

    [Serializable]
    public class FileCategories : List<FileCategory> {

        public const string General = "General";

        public static FileCategories CreateDefault()
        {
            var instance = new FileCategories();
            instance.Default();
            return instance;
        }

        public void Default()
        {
            Clear();
            Add(FileCategory.Create("<folder>", "Folders"));
            Add(FileCategory.Create(".png .jpg .jpeg .psd .ai .cdr .gif", "Pictures"));
            Add(FileCategory.Create(".doc .docx .rtf .pptx .ppx .pub .txt .html .htm", "Documents"));
            Add(FileCategory.Create(".mp3 .mp4 .wav .m4p .webm .flv", "Media"));
            Add(FileCategory.Create(".zip .7z .rar .7zip .tar .iso", "Archives"));
            Add(FileCategory.Create(".exe .com .lnk", "Apps"));
        }

        public bool IsGeneral(string path)
        {
            foreach (var category in this)
                if (category.Test(path))
                    return false;

            return true;
        }

        public string GetDirectory(string path)
        {
            foreach (var category in this)
                if (category.Test(path))
                    return category.Name;

            return General;
        }

        public string[] GetFolderNames()
        {
            string[] folders = new string[Count + 1];
            folders[Count] = General;
            for (int i = 0; i < Count; i++)
                folders[i] = this[i].Name;
            return folders;
        }

        public void Delete(string name)
        {
            for (int i = 0; i < Count; i++)
                if (this[i].Name == name)
                    RemoveAt(i);
        }
    }

    [Serializable]
    public class Settings
    {
        public FileCategories Categories = FileCategories.CreateDefault();
        public int DirtyLimit = 15;

        public void Save(string path)
        {
            string dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

            using (Stream file = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                var bf = new BinaryFormatter();
                bf.Serialize(file, this);
            }
        }

        public static Settings Load(string path, Settings settings = null)
        {
            if (!File.Exists(path)) return settings ?? new Settings();

            try
            {
                using (Stream file = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    var bf = new BinaryFormatter();
                    return bf.Deserialize(file) as Settings;
                }
            }
            catch
            {
                return settings ?? new Settings();
            }
        }
    }

    static class ArrayHelper
    {
        public static bool Contains(this string[] @this, string item, bool caseSensitive = true)
        {
            foreach (var str in @this)
            {
                if (caseSensitive)
                {
                    if (str == item)
                        return true;
                }
                else
                {
                    if (str.ToLower() == item.ToLower())
                        return true;
                }
            }

            return false;
        }

        public static bool InArray<T>(this T[] @this, T item)
        {
            Comparer<T> comparer = Comparer<T>.Default;

            foreach (var itm in @this)
                if (comparer.Compare(item, itm) == 0)
                    return true;

            return false;
        }

        public static T[] Merge<T>(T[] array1, T[] array2)
        {
            T[] result = new T[array1.Length + array2.Length];
            array1.CopyTo(result, 0);
            array2.CopyTo(result, array1.Length);
            return result;
        }

    }

    class Debug : IDisposable
    {
        public static Debug Instance;

        private Stream file;

        public Debug(string path)
        {
            file = new FileStream(path, File.Exists(path) ? FileMode.Append : FileMode.Create);
        }

        public void CreateInstance()
        {
            Instance = this;
        }

        public void Log(string message)
        {
            WriteLog(message, "Debug");
        }

        public void Error(string message)
        {
            WriteLog(message, "Error");
        }

        private void WriteLog(string message, string type, string time = "")
        {
            if (time == "")
                time = DateTime.Now.ToString();
            string line = $"{time} [{type}] {message}\n";
            byte[] bytes = Encoding.UTF8.GetBytes(line);
            file.Write(bytes, 0, bytes.Length);
        }

        public void Dispose()
        {
            file.Dispose();
        }
    }

    static class StringHelper
    {
        public static string Multiply(this string @this, int count)
        {
            string result = "";
            for (int i = 0; i < count; i++)
                result += @this;
            return result;
        }
    }
}