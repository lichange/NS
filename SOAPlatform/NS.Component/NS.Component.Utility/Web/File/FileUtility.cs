using System;
using System.IO;

namespace NS.Component.Utility
{
    public class FileUtility
    {
        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="path"></param>
        public static void DeleteFile(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        /// <summary>
        /// 删除文件夹
        /// </summary>
        /// <param name="path"></param>
        public static void DeleteFolder(string path)
        {
            if (Directory.Exists(path))
            {
                foreach (string d in Directory.GetFileSystemEntries(path))
                {
                    if (File.Exists(d))
                    {
                        File.Delete(d);
                    }
                    else
                    {
                        DeleteFolder(d);
                    }
                }
            }
        }

        /// <summary>
        /// 创建以时间命名的文件名
        /// </summary>
        public static string FileNewName
        {
            get { return DateTime.Now.ToString("yyyyMMddHHmmss"); }
        }

        /// <summary>
        /// 获取扩展名
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static string GetFileExtendName(string file)
        {
            int lastIndex = file.LastIndexOf('.');
            if (lastIndex != -1)
            {
                return file.Substring(lastIndex + 1);
            }
            return string.Empty;
        }

        /// <summary>
        /// 返回与 Web 服务器上的指定虚拟路径相对应的物理文件路径
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetPhysicalPath(string path)
        {
            string basePath = AppDomain.CurrentDomain.BaseDirectory;
            if (path.IsEmpty() || path.Length == 1)
            {
                return basePath;
            }
            if (path.IndexOf('~') == 0)
            {
                path = path.Substring(1);
            }
            path = path.Replace('/', '\\');
            if (path.IndexOf('\\') == 0)
            {
                return basePath.Substring(0, basePath.Length - 1) + path;
            }
            else
            {
                return basePath.Substring(0, basePath.Length) + path;
            }
        }
    }
}
