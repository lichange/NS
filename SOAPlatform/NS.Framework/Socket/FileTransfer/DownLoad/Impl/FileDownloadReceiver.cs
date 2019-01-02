using NS.Framework.Attributes;
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
    [Export(typeof(IFileDownloadReceiver))]
    public class FileDownloadReceiver : IFileDownloadReceiver
    {
        private NS.Framework.Communication.FileDownloadHelper.FileReceiver task;

        public FileDownloadReceiver()
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(BigFileTransferDescription.ServiceIP);

            task = new NS.Framework.Communication.FileDownloadHelper.FileReceiver();
            task.Socket = socket;
            task.EnabledIOBuffer = true;
            task.BlockFinished += new NS.Framework.Communication.FileDownloadHelper.BlockFinishedEventHandler(task_BlockFinished);
            task.ConnectLost += new EventHandler(task_ConnectLost);
            task.AllFinished += new EventHandler(task_AllFinished);
            task.BlockHashed += new NS.Framework.Communication.FileDownloadHelper.BlockFinishedEventHandler(task_BlockHashed);
            task.ErrorOccurred += new NS.Framework.Communication.FileDownloadHelper.FileTransmissionErrorOccurEventHandler(task_ErrorOccurred);
            //task.FilePath = @"A:";
        }

        public void Start()
        {
            task.Start();
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
        /// 传输过程中出现错误是执行的事件通知
        /// </summary>
        public event Action<string> OnAllTaskFinishedEvent;

        void task_AllFinished(object sender, EventArgs e)
        {
            if (OnAllTaskFinishedEvent != null)
                OnAllTaskFinishedEvent("完成");
        }

        /// <summary>
        /// 传输过程中出现错误是执行的事件通知
        /// </summary>
        public event Action<string> OnConnectLostedEvent;

        void task_ConnectLost(object sender, EventArgs e)
        {
            if (OnConnectLostedEvent != null)
                OnConnectLostedEvent("连接中断");
        }
        /// <summary>
        /// 文件块发送完成时执行的事件通知
        /// </summary>
        public event Action<NS.Framework.Communication.FileDownloadHelper.FileTransmission> OnProgressChangedEvent;

        void task_BlockFinished(object sender, NS.Framework.Communication.FileDownloadHelper.BlockFinishedEventArgs e)
        {
            NS.Framework.Communication.FileDownloadHelper.FileTransmission task = (NS.Framework.Communication.FileDownloadHelper.FileTransmission)sender;

            if (OnProgressChangedEvent != null)
                OnProgressChangedEvent(task);
        }
        void task_BlockHashed(object sender, NS.Framework.Communication.FileDownloadHelper.BlockFinishedEventArgs e)
        {
            NS.Framework.Communication.FileDownloadHelper.FileTransmission task = (NS.Framework.Communication.FileDownloadHelper.FileTransmission)sender;

            if (OnProgressChangedEvent != null)
                OnProgressChangedEvent(task);
        }
        //void SetProgressBar(NS.Framework.Communication.FileDownloadHelper.FileTransmission task)
        //{
        //    this.progressBar1.Maximum = task.Blocks.Count;
        //    this.progressBar1.Value = task.Blocks.CountValid;
        //}

        //void SetProgress(NS.Framework.Communication.FileDownloadHelper.FileTransmission task)
        //{
        //    this.Text = "接收端 下载中";
        //    SetProgressBar(task);
        //    this.label3.Text = string.Format("进度:{0:N2}%   总长度:{1}   已完成:{2}", task.Progress, task.TotalSize, task.FinishedSize);
        //    this.label1.Text = string.Format("平均速度:{0:N2}KB/s   已用时:{1}   估计剩余时间:{2}", task.KByteAverSpeed, task.TimePast, task.TimeRemaining);
        //    this.label2.Text = string.Format("瞬时速度:{0:N2}KB/s   丢弃的区块:{1}", task.KByteSpeed, task.Blocks.Cast.Count);
        //}
        #endregion
    }
}