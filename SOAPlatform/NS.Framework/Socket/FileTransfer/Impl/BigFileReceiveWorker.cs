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
    /// 大文件接收传输实现，要求支持断点续传，同时要求是异步传输，能够多线程处理。
    /// </summary>
    public class BigFileReceiveWorker : IBigFileReceiveWorker
    {
        /// <summary>
        /// 当前位置；已接收字节总数；总接收字节内容
        /// </summary>
        private long offset, totalReceived, totalReceive;

        /// <summary>
        /// 缓冲区
        /// </summary>
        private byte[] buffer;

        /// <summary>
        /// socket通信套接字
        /// </summary>
        private Socket sock;

        /// <summary>
        /// 文件流Writer
        /// </summary>
        private FileStream writer;

        /// <summary>
        /// 工作线程
        /// </summary>
        private Thread thread;

        /// <summary>
        /// 任务是否完成
        /// </summary>
        private bool isFinished;

        /// <summary>
        /// 已接收的文件字节流总数
        /// </summary>
        public long TotalReceived
        {
            get
            {
                return totalReceived;
            }
        }

        /// <summary>
        /// 计划接收字节流总数
        /// </summary>
        public long TotalReceive
        {
            get
            {
                return totalReceive;
            }
        }

        /// <summary>
        /// 缓冲区大小
        /// </summary>
        public byte[] Buffer
        {
            get
            {
                return buffer;
            }
        }

        /// <summary>
        /// 客户端连接对象
        /// </summary>
        public Socket Client
        {
            get
            {
                return sock;
            }
        }

        /// <summary>
        /// 是否传输完成
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
        /// <param name="client">客户端传输Socket对象</param>
        public BigFileReceiveWorker(Socket client)
        {
            sock = client;
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
            writer = new FileStream(path, FileMode.Open, FileAccess.Write, FileShare.Write);
            writer.Position = position + worked;
            writer.Lock(position, length);
            offset = position;
            totalReceived = worked;
            totalReceive = total;
            thread = new Thread(new ParameterizedThreadStart(Work));
            thread.IsBackground = true;
#if TransmitLog
                thread.Name = position.ToString() + length.ToString();
                AppendTransmitLog(LogType.Transmit, thread.Name + " Initialized:" + totalReceived + "/" + totalReceive + ".");
#endif
        }

        /// <summary>
        /// 线程执行任务过程
        /// </summary>
        /// <param name="obj"></param>
        private void Work(object obj)
        {
            int received;
            while (totalReceived < totalReceive)
            {
                if ((received = sock.Receive(buffer)) == 0)
                {
                    break;
                }
                writer.Write(buffer, 0, received);
                writer.Flush();
                totalReceived += (long)received;
#if TransmitLog
                    AppendTransmitLog(LogType.Transmit, thread.Name + ":" + totalReceived + "/" + totalReceive + ".");
#endif
#if Sleep
                    Thread.Sleep(200);
#endif
            }
            writer.Unlock(offset, totalReceive);
            writer.Dispose();
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
        /// 通知线程当前处理的进度信息
        /// </summary>
        /// <param name="worked"></param>
        /// <param name="total"></param>
        public void ReportProgress(out long worked, out long total)
        {
            worked = totalReceived;
            total = totalReceive;
        }

        /// <summary>
        /// 启动任务
        /// </summary>
        /// <param name="waitHandle"></param>
        public void RunWork(EventWaitHandle waitHandle)
        {
            thread.Start(waitHandle);
        }
    }
}