using System;
using System.Text;

namespace NS.Framework.RocketSocket.Server
{
    /// <summary>
    /// <see cref="SocketBase.Packet"/> 数据包构建器
    /// </summary>
    public static class PacketBuilder
    {
        #region 转换为异步二进制流形式数据包
        /// <summary>
        /// 待发送的数据内容转换为异步二进制流形式数据包<see cref="SocketBase.Packet"/>
        /// </summary>
        /// <param name="responseFlag"></param>
        /// <param name="seqID"></param>
        /// <param name="buffer"></param>
        /// <returns></returns>
        static public SocketBase.Packet ToAsyncBinary(string responseFlag, int seqID, byte[] buffer)
        {
            var messageLength = (buffer == null ? 0 : buffer.Length) + responseFlag.Length + 6;
            var sendBuffer = new byte[messageLength + 4];

            //write message length
            Buffer.BlockCopy(SocketBase.Utils.NetworkBitConverter.GetBytes(messageLength), 0, sendBuffer, 0, 4);
            //write seqID.
            Buffer.BlockCopy(SocketBase.Utils.NetworkBitConverter.GetBytes(seqID), 0, sendBuffer, 4, 4);
            //write response flag length.
            Buffer.BlockCopy(SocketBase.Utils.NetworkBitConverter.GetBytes((short)responseFlag.Length), 0, sendBuffer, 8, 2);
            //write response flag
            Buffer.BlockCopy(Encoding.ASCII.GetBytes(responseFlag), 0, sendBuffer, 10, responseFlag.Length);
            //write body buffer
            if (buffer != null && buffer.Length > 0) Buffer.BlockCopy(buffer, 0, sendBuffer, 10 + responseFlag.Length, buffer.Length);

            return new SocketBase.Packet(sendBuffer);
        }
        #endregion

        #region 待发送的字符串内容转化为命令
        /// <summary>
        /// 转化为命令 <see cref="SocketBase.Packet"/>
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        static public SocketBase.Packet ToCommandLine(string value)
        {
            return new SocketBase.Packet(Encoding.UTF8.GetBytes(string.Concat(value, Environment.NewLine)));
        }
        #endregion
    }
}