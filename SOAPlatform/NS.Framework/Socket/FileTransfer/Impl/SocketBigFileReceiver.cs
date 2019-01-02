using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace NS.Framework.Communication
{
    /// <summary>
    /// 大文件接收传输实现，要求支持断点续传，同时要求是异步传输，能够多线程处理。
    /// </summary>
    public class SocketBigFileReceiver : IBigFileReceiver
    {
        /// <summary>
        /// 计时器
        /// </summary>
        private Stopwatch watcher;

        /// <summary>
        /// 实时通知当前传输的进度情况-以便在前台显示进度条情况。
        /// </summary>
        public event Action<int, int> ReportProcessState;

        public SocketBigFileReceiver()
        {
            watcher = new Stopwatch();
        }

        /// <summary>
        /// 接收文件
        /// </summary>
        /// <param name="saveFilePath">文件完整路径</param>
        public void ReceiverFile(string saveFilePath)
        {
            Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            listener.Bind(BigFileTransferDescription.LocalIP);
            listener.Listen(BigFileTransferDescription.MinThreadCount);
            Socket client = listener.Accept();

            watcher.Start();

            //设定缓冲区大小
            byte[] buffer = new byte[BigFileTransferDescription.BufferSize];
            int received = client.Receive(buffer);
            long receive, length = BitConverter.ToInt64(buffer, 0);
            string fileName = Encoding.Default.GetString(buffer, BigFileTransferDescription.PerLongCount, received - BigFileTransferDescription.PerLongCount);
            
            Console.WriteLine("Receiveing file:" + fileName + ".Plz wait...");
            
            FileInfo file = new FileInfo(Path.Combine(saveFilePath, fileName));
            using (FileStream writer = file.Open(file.Exists ? FileMode.Append : FileMode.CreateNew, FileAccess.Write, FileShare.None))
            {
                receive = writer.Length;

                //向客户端发送已接收的文件长度信息
                client.Send(BitConverter.GetBytes(receive));

#if BreakpointLog
                Console.WriteLine("Breakpoint " + receive);
#endif
                //如果没有接收文件完毕-持续从客户端接收文件流
                while (receive < length)
                {
                    if ((received = client.Receive(buffer)) == 0)
                    {
                        Console.WriteLine("Send Stop.");
                        return;
                    }

                    //将接收后的字节数据，写入到文件流
                    writer.Write(buffer, 0, received);
                    //清空缓冲区
                    writer.Flush();
                    //已接收的文件长度信息
                    receive += (long)received;
#if TransmitLog
                    Console.WriteLine("Received " + receive + "/" + length + ".");
#endif
#if Sleep
                    Thread.Sleep(200);
#endif
                }
            }
            //关闭客户端连接
            client.Shutdown(SocketShutdown.Both);
            client.Close();

            //停止计时器
            watcher.Stop();
            Console.WriteLine("Receive finish.Span Time:" + watcher.Elapsed.TotalMilliseconds + " ms.");
        }
    }
}