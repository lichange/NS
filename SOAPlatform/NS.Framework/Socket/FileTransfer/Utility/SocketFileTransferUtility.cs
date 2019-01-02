using System;
using System.Net;

namespace NS.Framework.Communication
{
    /// <summary>
    /// 文件传输处理类库
    /// </summary>
    public class SocketFileTransferUtility
    {
        /// <summary>
        /// 写文件
        /// </summary>
        /// <param name="value"></param>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        public static void WriteFile(long value, byte[] buffer, int offset)
        {
            buffer[offset++] = (byte)value;
            buffer[offset++] = (byte)(value >> 8);
            buffer[offset++] = (byte)(value >> 0x10);
            buffer[offset++] = (byte)(value >> 0x18);
            buffer[offset++] = (byte)(value >> 0x20);
            buffer[offset++] = (byte)(value >> 40);
            buffer[offset++] = (byte)(value >> 0x30);
            buffer[offset] = (byte)(value >> 0x38);
        }

        /// <summary>
        /// 读取文件
        /// </summary>
        /// <param name="value"></param>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        public static void ReadFile(out long value, byte[] buffer, int offset)
        {
            uint num = (uint)(((buffer[offset++] | (buffer[offset++] << 8)) | (buffer[offset++] << 0x10)) | (buffer[offset++] << 0x18));
            uint num2 = (uint)(((buffer[offset++] | (buffer[offset++] << 8)) | (buffer[offset++] << 0x10)) | (buffer[offset] << 0x18));
            value = (long)((num2 << 0x20) | num);
        }

        /// <summary>
        /// 根据文件大小获取传输的线程数
        /// </summary>
        /// <param name="fileSize">文件大小</param>
        /// <returns>返回线程数</returns>
        public static int GetThreadCount(long fileSize)
        {
            int count = (int)(fileSize / BigFileTransferDescription.SplitSize);

            if (count < BigFileTransferDescription.MinThreadCount)
            {
                count = BigFileTransferDescription.MinThreadCount;
            }
            else if (count > BigFileTransferDescription.MaxThreadCount)
            {
                count = BigFileTransferDescription.MaxThreadCount;
            }

            return count;
        }
    }

    public static class SocketFileTransferExtension
    {
        public static int ReportProgress(this IBigFileWorker[] workers, out long worked, out long total)
        {
            worked = total = 0L;
            long w, t;
            foreach (IBigFileWorker worker in workers)
            {
                worker.ReportProgress(out w, out t);
                worked += w;
                total += t;
            }
            return (int)(worked / total) * 100;
        }

        public static int ReportSpeed(this IBigFileWorker[] workers, ref long oldValue)
        {
            long w, t;
            workers.ReportProgress(out w, out t);
            int speed = (int)((w - oldValue) / 8L);
            oldValue = w;
            return speed;
        }

        public static bool IsAllFinished(this IBigFileWorker[] workers)
        {
            bool flag = true;
            foreach (IBigFileWorker worker in workers)
            {
                if (!worker.IsFinished)
                {
                    flag = false;
                    break;
                }
            }
            return flag;
        }
    }
}