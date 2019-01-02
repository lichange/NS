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
    [Export(typeof(IFileDownloadSender))]
    public class FileDownloadSender : IFileDownloadSender
    {
        private string FullFileName;
        private NS.Framework.Communication.FileDownloadHelper.FileSender task;
        private Socket listensocket;
        private Thread ListenThread;
        private bool isStart = false;

        public FileDownloadSender()
        {
            ListenThread = new Thread(BeginListen);
            ListenThread.IsBackground = true;
            isStart = false;
        }

        public void Start()
        {
            ListenThread.Start();
            isStart = true;
        }

        public void SendFile(string filePath)
        {
            this.Start();
            FullFileName = filePath;
        }

        public void Stop()
        {
            ListenThread.Abort();

            if (listensocket != null)
                listensocket.Close();

            isStart = false;
        }

        private void BeginListen()
        {
            try
            {
                listensocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPEndPoint ep = BigFileTransferDescription.ServiceIP;
                listensocket.Bind(ep);
                listensocket.Listen(20);
                while (true)
                {
                    Socket newsocket = listensocket.Accept();
                    //task = new FileTransmission(FileTransmission.TransmissionMode.Send);
                    task = new NS.Framework.Communication.FileDownloadHelper.FileSender();
                    task.FullFileName = FullFileName;
                    task.Socket = newsocket;
                    task.EnabledIOBuffer = true;
                    task.BlockFinished += new NS.Framework.Communication.FileDownloadHelper.BlockFinishedEventHandler(task_BlockFinished);
                    task.CommandReceived += new NS.Framework.Communication.FileDownloadHelper.CommandReceivedEventHandler(task_CommandReceived);
                    task.ConnectLost += new EventHandler(task_ConnectLost);
                    task.ErrorOccurred += new NS.Framework.Communication.FileDownloadHelper.FileTransmissionErrorOccurEventHandler(task_ErrorOccurred);
                    task.Start();
                }
            }
            catch (ThreadAbortException)
            {
                if (task != null && task.IsAlive)
                    task.Stop(true);
            }
        }

        #region 事件处理

        /// <summary>
        /// 传输过程中出现错误是执行的事件通知
        /// </summary>
        public event Action<string> OnErrorOccurredEvent;

        void task_ErrorOccurred(object sender, NS.Framework.Communication.FileDownloadHelper.FileTransmissionErrorOccurEventArgs e)
        {
            if (OnErrorOccurredEvent != null)
                OnErrorOccurredEvent(e.InnerException.ToString());
        }

        /// <summary>
        /// 文件块发送完成时执行的事件通知
        /// </summary>
        public event Action<string, string> OnBlockFinishedEvent;

        void task_BlockFinished(object sender, NS.Framework.Communication.FileDownloadHelper.BlockFinishedEventArgs e)
        {
            NS.Framework.Communication.FileDownloadHelper.FileTransmission task = (NS.Framework.Communication.FileDownloadHelper.FileTransmission)sender;

            OnBlockFinishedEvent(string.Format("已发送:{0:N2}%", task.Progress), string.Format("{0:N2}KB/s", task.KByteAverSpeed));
        }

        /// <summary>
        /// 连接丢失或断开时的事件通知
        /// </summary>
        public event Action<string> OnConnectLostedEvent;

        void task_ConnectLost(object sender, EventArgs e)
        {
            if (OnConnectLostedEvent != null)
                OnConnectLostedEvent("连接丢失");
        }

        /// <summary>
        /// 收到socket相关命令时发生
        /// </summary>
        public event Action<string> OnCommandReceivedEvent;

        void task_CommandReceived(object sender, NS.Framework.Communication.FileDownloadHelper.CommandReceivedEventArgs e)
        {
            if (OnCommandReceivedEvent != null)
                OnCommandReceivedEvent(e.CommandLine);
        }

        #endregion
    }
}