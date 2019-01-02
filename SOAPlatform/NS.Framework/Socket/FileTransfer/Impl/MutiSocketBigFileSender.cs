using NS.Framework.Attributes;
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
    [Export(typeof(IBigFileSender))]
    public class MutiSocketBigFileSender : IBigFileSender
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

        public MutiSocketBigFileSender()
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
            //启动监视器
            watcher.Start();

            //获取文件信息
            FileInfo file = new FileInfo(filePath);

#if DEBUG
            if (!file.Exists) //Debug 模式下抛出异常
            {
                throw new FileNotFoundException();
            }
#endif
            //多线程处理器
            BigFileSendWorker worker = new BigFileSendWorker(BigFileTransferDescription.ServiceIP);
            long fileLength = file.Length;
            Buffer.BlockCopy(BitConverter.GetBytes(fileLength), 0, worker.Buffer, 0, BigFileTransferDescription.PerLongCount);
            string fileName = file.Name;
            worker.Client.Send(worker.Buffer, 0, BigFileTransferDescription.PerLongCount + Encoding.Default.GetBytes(fileName, 0, fileName.Length, worker.Buffer, BigFileTransferDescription.PerLongCount), SocketFlags.None);
            Console.WriteLine("Sending file:" + fileName + ".Plz wait...");

            int threadCount = SocketFileTransferUtility.GetThreadCount(fileLength);

            BigFileSendWorker[] workers = new BigFileSendWorker[threadCount];

            try
            {
                for (int i = 0; i < threadCount; i++)
                {
                    workers[i] = i == 0 ? worker : new BigFileSendWorker(BigFileTransferDescription.ServiceIP);
                }

                #region Breakpoint 断点续传

                int perPairCount = BigFileTransferDescription.PerLongCount * 2, count = perPairCount * threadCount;
                byte[] bufferInfo = new byte[count];
                long oddSize, avgSize = Math.DivRem(fileLength, (long)threadCount, out oddSize);
                if (worker.Client.Receive(bufferInfo) == 4)
                {
                    for (int i = 0; i < threadCount; i++)
                    {
                        workers[i].Initialize(filePath, i * avgSize, i == threadCount - 1 ? avgSize + oddSize : avgSize);
                    }
                }
                else
                {
                    long w, t;
                    for (int i = 0; i < threadCount; i++)
                    {
                        SocketFileTransferUtility.ReadFile(out w, bufferInfo, i * perPairCount);
                        SocketFileTransferUtility.ReadFile(out t, bufferInfo, i * perPairCount + BigFileTransferDescription.PerLongCount);
                        workers[i].Initialize(filePath, i * avgSize, i == threadCount - 1 ? avgSize + oddSize : avgSize, w, t);
#if BreakpointLog
                    AppendTransmitLog(LogType.Breakpoint, i + " read:" + w + "/" + t + ".");
#endif
                    }
                }

                Thread.Sleep(100);

                #endregion

                //线程同步锁
                AutoResetEvent reset = new AutoResetEvent(true);
                for (int i = 0; i < threadCount; i++)
                {
                    workers[i].RunWork(i == threadCount - 1 ? reset : null);
                }
                reset.WaitOne();
            }
            catch (Exception ex)
            {
                Console.WriteLine("文件传输过程中出现错误：" + ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }

            #region Breakpoint 断点续传

            int speed;
            long value = 0L;
            do
            {
                speed = workers.ReportSpeed(ref value);
                Console.WriteLine("waiting for other threads. Progress:" + value + "/" + fileLength + ";Speed:" + speed + "kb/s.");
                Thread.Sleep(500);
            }
            while (!workers.IsAllFinished());
            speed = workers.ReportSpeed(ref value);
            Console.WriteLine("waiting for other threads. Progress:" + value + "/" + fileLength + ";Speed:" + speed + "kb/s.");

            #endregion

            watcher.Stop();
            Console.WriteLine("Send finish.Span Time:" + watcher.Elapsed.TotalMilliseconds + " ms.");
        }
    }
}