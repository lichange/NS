using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace NS.Framework.Communication
{
    /// <summary>
    /// 大文件发送传输接口，要求支持断点续传，同时要求是异步传输，能够多线程处理。
    /// </summary>
    public class SocketBigFileSender : IBigFileSender
    {
        /// <summary>
        /// 计时器
        /// </summary>
        private Stopwatch watcher;

#if TransmitLog
        private static StreamWriter transmitLoger;
#endif
#if BreakpointLog
        private static StreamWriter breakpointLoger;
#endif

        public SocketBigFileSender()
        {
            watcher = new Stopwatch();

            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            //TestIP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 520);
#if TransmitLog
            transmitLoger = new StreamWriter(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "transmit.log"), true, Encoding.Default);
#endif
#if BreakpointLog
            breakpointLoger = new StreamWriter(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "breakpoint.log"), true, Encoding.Default);
#endif
        }

        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            StreamWriter writer = new StreamWriter(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "exec.log"), true, Encoding.Default);
            writer.Write("Time:");
            writer.Write(DateTime.Now.ToShortTimeString());
            writer.Write(". ");
            writer.WriteLine(e.ExceptionObject);
            writer.Dispose();
        }

        /// <summary>
        /// 实时通知当前传输的进度情况-以便在前台显示进度条情况。
        /// </summary>
        public event Action<int, int> ReportProcessState;

        public void SendFile(string filePath)
        {
            //监视器
            watcher.Start();
            Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            sock.Connect(BigFileTransferDescription.ServiceIP);
            byte[] buffer = new byte[BigFileTransferDescription.BufferSize];

            //打开要传输的文件
            using (FileStream reader = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.None))
            {
                long send, length = reader.Length;
                Buffer.BlockCopy(BitConverter.GetBytes(length), 0, buffer, 0, BigFileTransferDescription.PerLongCount);
                string fileName = Path.GetFileName(filePath);

                //发送文件名称来获取服务器上该文件已经发送的文件分包大小及位置
                sock.Send(buffer, 0, BigFileTransferDescription.PerLongCount + Encoding.Default.GetBytes(fileName, 0, fileName.Length, buffer, BigFileTransferDescription.PerLongCount), SocketFlags.None);

                Console.WriteLine("Sending file:" + fileName + ".Plz wait...");

                //接收文件包大小及位置。
                sock.Receive(buffer);

                //读取文件流的具体位置
                reader.Position = send = BitConverter.ToInt64(buffer, 0);

#if BreakpointLog
                Console.WriteLine("Breakpoint " + reader.Position);
#endif
                int read, sent;
                bool flag;
                //从指定的文件位置读取文件流的数据信息
                while ((read = reader.Read(buffer, 0, BigFileTransferDescription.BufferSize)) != 0)
                {
                    sent = 0;
                    flag = true;

                    //分包传输文件内容-每次发送缓冲区大小的内容
                    while ((sent += sock.Send(buffer, sent, read, SocketFlags.None)) < read)
                    {
                        flag = false;
                        send += (long)sent;
#if TransmitLog
                        Console.WriteLine("Sent " + send + "/" + length + ".");
#endif
#if Sleep
                        Thread.Sleep(200);
#endif
                    }
                    if (flag)
                    {
                        send += (long)read;
#if TransmitLog
                        Console.WriteLine("Sent " + send + "/" + length + ".");
#endif
#if Sleep
                        Thread.Sleep(200);
#endif
                    }
                }
            }

            //关闭socket链接
            sock.Shutdown(SocketShutdown.Both);
            sock.Close();

            //停止计时器
            watcher.Stop();

            Console.WriteLine("Send finish.Span Time:" + watcher.Elapsed.TotalMilliseconds + " ms.");
        }
    }
}