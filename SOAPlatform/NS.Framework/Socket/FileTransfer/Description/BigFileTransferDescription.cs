using System;
using System.Net;
using System.Linq;

namespace NS.Framework.Communication
{
    /// <summary>
    /// 大文件传输相关参数定义说明。
    /// </summary>
    public class BigFileTransferDescription
    {
        /// <summary>
        /// 传输数据缓冲区大小
        /// </summary>
        public readonly static int BufferSize = 4096;
        /// <summary>
        /// 每次传输的文件长度
        /// </summary>
        public static int PerLongCount = sizeof(long);
        /// <summary>
        /// 最小线程数
        /// </summary>
        public readonly static int MinThreadCount = 10;
        /// <summary>
        /// 最大线程数
        /// </summary>
        public readonly static int MaxThreadCount = 50;
        /// <summary>
        /// 断点续传的临时文件存储格式
        /// </summary>
        public readonly static string PointExtension = ".dat";
        /// <summary>
        /// 接收时临时文件存储信息格式
        /// </summary>
        public readonly static string TempExtension = ".temp";
        /// <summary>
        /// 分包大小
        /// </summary>
        public readonly static long SplitSize = 1024L * 1024L * 100L;
        /// <summary>
        /// 文件服务器的地址信息
        /// </summary>
        public static readonly IPEndPoint ServiceIP;

        /// <summary>
        /// 文件服务器的地址信息
        /// </summary>
        public static readonly IPEndPoint LocalIP;

        static BigFileTransferDescription()
        {
            //相关信息
            LocalIP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 520);
            var fileServerKeyValuePair = Config.PlatformConfig.ServerConfig.KeyValueSettings.KeyValueItems.Where(pre => pre.Key == "FileServer").FirstOrDefault();
            ServiceIP = new IPEndPoint(IPAddress.Parse(string.IsNullOrEmpty(fileServerKeyValuePair.Value) ? "127.0.0.1" : fileServerKeyValuePair.Value), 520);
        }
    }
}