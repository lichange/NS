using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace NS.Framework.Communication
{
    /// <summary>
    /// 大文件发送传输接口，要求支持断点续传，同时要求是异步传输，能够多线程处理。
    /// </summary>
    public class BigFileSendWorker : IBigFileSendWorker
    {
        /// <summary>
        /// 待发送文件流总数；已发送的文件流总数
        /// </summary>
        private long totalSent, totalSend;

        /// <summary>
        /// 缓冲区
        /// </summary>
        private byte[] buffer;

        /// <summary>
        /// 发送文件流socket
        /// </summary>
        private Socket sock;

        /// <summary>
        /// 文件流读取器
        /// </summary>
        private FileStream reader;

        /// <summary>
        /// 工作线程
        /// </summary>
        private Thread thread;

        /// <summary>
        /// 是否发送完成
        /// </summary>
        private bool isFinished;

        /// <summary>
        /// 已发送的文件流总数
        /// </summary>
        public long TotalSent
        {
            get
            {
                return totalSent;
            }
        }

        /// <summary>
        /// 待发送文件流总数
        /// </summary>
        public long TotalSend
        {
            get
            {
                return totalSend;
            }
        }

        /// <summary>
        /// 缓冲区
        /// </summary>
        public byte[] Buffer
        {
            get
            {
                return buffer;
            }
        }

        /// <summary>
        /// 发送文件流socket对象
        /// </summary>
        public Socket Client
        {
            get
            {
                return sock;
            }
        }

        /// <summary>
        /// 是否完成
        /// </summary>
        public bool IsFinished
        {
            get
            {
                return isFinished;
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="ip"></param>
        public BigFileSendWorker(IPEndPoint ip)
        {
            sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            sock.Connect(ip);
            buffer = new byte[BigFileTransferDescription.BufferSize];
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="path"></param>
        /// <param name="position"></param>
        /// <param name="length"></param>
        public void Initialize(string path, long position, long length)
        {
            Initialize(path, position, length, 0L, length);
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="path"></param>
        /// <param name="position"></param>
        /// <param name="length"></param>
        /// <param name="worked"></param>
        /// <param name="total"></param>
        public void Initialize(string path, long position, long length, long worked, long total)
        {
            reader = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            reader.Position = position + worked;
            totalSent = worked;
            totalSend = total;
            thread = new Thread(new ParameterizedThreadStart(Work));
            thread.IsBackground = true;
#if TransmitLog
                thread.Name = position.ToString() + length.ToString();
                AppendTransmitLog(LogType.Transmit, thread.Name + " Initialized:" + totalSent + "/" + totalSend + ".");
#endif
        }

        /// <summary>
        /// 工作线程任务过程
        /// </summary>
        /// <param name="obj"></param>
        private void Work(object obj)
        {
            int read, sent;
            bool flag;
            while (totalSent < totalSend)
            {
                read = reader.Read(buffer, 0, Math.Min(BigFileTransferDescription.BufferSize, (int)(totalSend - totalSent)));
                sent = 0;
                flag = true;
                while ((sent += sock.Send(buffer, sent, read, SocketFlags.None)) < read)
                {
                    flag = false;
                    totalSent += (long)sent;
#if TransmitLog
                        AppendTransmitLog(LogType.Transmit, thread.Name + ":" + totalSent + "/" + totalSend + ".");
#endif
#if Sleep
                        Thread.Sleep(200);
#endif
                }
                if (flag)
                {
                    totalSent += (long)read;
#if TransmitLog
                        AppendTransmitLog(LogType.Transmit, thread.Name + ":" + totalSent + "/" + totalSend + ".");
#endif
#if Sleep
                        Thread.Sleep(200);
#endif
                }
            }
            reader.Dispose();
            sock.Shutdown(SocketShutdown.Both);
            sock.Close();
            EventWaitHandle waitHandle = obj as EventWaitHandle;
            if (waitHandle != null)
            {
                waitHandle.Set();
            }
            isFinished = true;
        }

        /// <summary>
        /// 通知工作线程当前进度
        /// </summary>
        /// <param name="worked"></param>
        /// <param name="total"></param>
        public void ReportProgress(out long worked, out long total)
        {
            worked = totalSent;
            total = totalSend;
        }

        /// <summary>
        /// 运行线程
        /// </summary>
        /// <param name="waitHandle"></param>
        public void RunWork(EventWaitHandle waitHandle)
        {
            thread.Start(waitHandle);
        }
    }
}