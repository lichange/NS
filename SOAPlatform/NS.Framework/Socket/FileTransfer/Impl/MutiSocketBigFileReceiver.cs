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
    /// 大文件接收传输实现，要求支持断点续传，同时要求是异步传输，能够多线程处理。
    /// </summary>
    [Export(typeof(IBigFileReceiver))]
    public class MutiSocketBigFileReceiver : IBigFileReceiver
    {
        /// <summary>
        /// 计时器
        /// </summary>
        private Stopwatch watcher;

        /// <summary>
        /// 实时通知当前传输的进度情况-以便在前台显示进度条情况。
        /// </summary>
        public event Action<int, int> ReportProcessState;

        public MutiSocketBigFileReceiver()
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
            listener.Bind(BigFileTransferDescription.ServiceIP);
            listener.Listen(BigFileTransferDescription.MaxThreadCount);
            var receiveWorker = new BigFileReceiveWorker(listener.Accept());

            //计时器
            watcher.Start();

            //接收相关流到缓冲区
            int recv = receiveWorker.Client.Receive(receiveWorker.Buffer);
            long fileLength = BitConverter.ToInt64(receiveWorker.Buffer, 0);
            string fileName = Encoding.Default.GetString(receiveWorker.Buffer, BigFileTransferDescription.PerLongCount, recv - BigFileTransferDescription.PerLongCount);
            Console.WriteLine("Receiveing file:" + fileName + ".Plz wait...");

            //获取当前文件接收线程数
            int threadCount = SocketFileTransferUtility.GetThreadCount(fileLength);
            BigFileReceiveWorker[] workers = new BigFileReceiveWorker[threadCount];
            try
            {
                for (int i = 0; i < threadCount; i++)
                {
                    workers[i] = i == 0 ? receiveWorker : new BigFileReceiveWorker(listener.Accept());
                }

                #region Breakpoint 断点续传

                int perPairCount = BigFileTransferDescription.PerLongCount * 2, count = perPairCount * threadCount;
                byte[] bufferInfo = new byte[count];
                string filePath = Path.Combine(saveFilePath, fileName), pointFilePath = Path.ChangeExtension(filePath, BigFileTransferDescription.PointExtension), tempFilePath = Path.ChangeExtension(filePath, BigFileTransferDescription.TempExtension);
                FileStream pointStream;
                long oddSize, avgSize = Math.DivRem(fileLength, (long)threadCount, out oddSize);
                if (File.Exists(pointFilePath) && File.Exists(tempFilePath))
                {
                    pointStream = new FileStream(pointFilePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
                    pointStream.Read(bufferInfo, 0, count);
                    long w, t;
                    for (int i = 0; i < threadCount; i++)
                    {
                        SocketFileTransferUtility.ReadFile(out w, bufferInfo, i * perPairCount);
                        SocketFileTransferUtility.ReadFile(out t, bufferInfo, i * perPairCount + BigFileTransferDescription.PerLongCount);
                        workers[i].Initialize(tempFilePath, i * avgSize, i == threadCount - 1 ? avgSize + oddSize : avgSize, w, t);
#if BreakpointLog
                    AppendTransmitLog(LogType.Breakpoint, i + " read:" + w + "/" + t + ".");
#endif
                    }
                    receiveWorker.Client.Send(bufferInfo);
                }
                else
                {
                    pointStream = new FileStream(pointFilePath, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.None);
                    FileStream stream = new FileStream(tempFilePath, FileMode.CreateNew, FileAccess.Write, FileShare.Write);
                    stream.SetLength(fileLength);
                    stream.Flush();
                    stream.Dispose();
                    for (int i = 0; i < threadCount; i++)
                    {
                        workers[i].Initialize(tempFilePath, i * avgSize, i == threadCount - 1 ? avgSize + oddSize : avgSize);
                    }
                    receiveWorker.Client.Send(bufferInfo, 0, 4, SocketFlags.None);
                }

                //计时器--异步执行
                Timer timer = new Timer(state =>
                {
                    long w, t;
                    for (int i = 0; i < threadCount; i++)
                    {
                        workers[i].ReportProgress(out w, out t);
                        SocketFileTransferUtility.WriteFile(w, bufferInfo, i * perPairCount);
                        SocketFileTransferUtility.WriteFile(t, bufferInfo, i * perPairCount + BigFileTransferDescription.PerLongCount);
#if BreakpointLog
                    AppendTransmitLog(LogType.Breakpoint, i + " write:" + w + "/" + t + ".");
#endif
                    }
                    pointStream.Position = 0L;
                    pointStream.Write(bufferInfo, 0, count);
                    pointStream.Flush();

                }, null, TimeSpan.Zero, TimeSpan.FromSeconds(2));

                #endregion

                //线程同步锁
                AutoResetEvent reset = new AutoResetEvent(true);
                for (int i = 0; i < threadCount; i++)
                {
                    workers[i].RunWork(i == threadCount - 1 ? reset : null);
                }
                reset.WaitOne();

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
                timer.Dispose();
                pointStream.Dispose();
                File.Delete(pointFilePath);
                File.Move(tempFilePath, filePath);
                #endregion
            }
            catch (Exception ex)
            {
                Console.WriteLine("文件传输过程中出现错误：" + ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }

            watcher.Stop();
            Console.WriteLine("Receive finish.Span Time:" + watcher.Elapsed.TotalMilliseconds + " ms.");
        }
    }
}