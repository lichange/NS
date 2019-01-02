using System;
using System.Net;

namespace NS.Framework.RocketSocket.SocketBase
{
    /// <summary>
    /// 与服务器链接的接口定义
    /// </summary>
    public interface IConnection
    {
        /// <summary>
        /// 开始发送数据包的事件定义
        /// </summary>
        event StartSendingHandler StartSending;

        /// <summary>
        /// 数据包发送完成的回掉事件
        /// </summary>
        event SendCallbackHandler SendCallback;

        /// <summary>
        /// 收到消息的事件处理
        /// </summary>
        event MessageReceivedHandler MessageReceived;

        /// <summary>
        /// 断开连接的事件处理
        /// </summary>
        event DisconnectedHandler Disconnected;

        /// <summary>
        /// 链接错误的事件处理
        /// </summary>
        event ErrorHandler Error;

        /// <summary>
        /// 返回当前链接是否还处于活动状态--为心跳检测提供服务
        /// </summary>
        bool Active { get; }

        /// <summary>
        /// 当前链接的唯一标识
        /// </summary>
        long ConnectionID { get; }

        /// <summary>
        /// 获取本地IP地址
        /// </summary>
        IPEndPoint LocalEndPoint { get; }

        /// <summary>
        /// 获取远程IP地址
        /// </summary>
        IPEndPoint RemoteEndPoint { get; }

        /// <summary>
        /// 获取或设置与用户数据
        /// </summary>
        object UserData { get; set; }

        /// <summary>
        /// 异步发送数据
        /// </summary>
        /// <param name="packet"></param>
        void BeginSend(Packet packet);

        /// <summary>
        /// 异步接收数据
        /// </summary>
        void BeginReceive();

        /// <summary>
        /// 异步断开连接
        /// </summary>
        /// <param name="ex">异常信息</param>
        void BeginDisconnect(Exception ex = null);
    }
}