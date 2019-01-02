using System.Collections.Generic;
using System.Net;

namespace NS.Framework.RocketSocket.SocketBase.Utils
{
    /// <summary>
    /// IP 操作辅助类
    /// </summary>
    public static class IPUtility
    {
        #region Private Members
        /// <summary>
        /// A类: 10.0.0.0-10.255.255.255
        /// </summary>
        static private long ipABegin, ipAEnd;
        /// <summary>
        /// B类: 172.16.0.0-172.31.255.255   
        /// </summary>
        static private long ipBBegin, ipBEnd;
        /// <summary>
        /// C类: 192.168.0.0-192.168.255.255
        /// </summary>
        static private long ipCBegin, ipCEnd;
        #endregion

        #region 构造函数
        /// <summary>
        /// 静态构造函数
        /// </summary>
        static IPUtility()
        {
            ipABegin = ConvertToNumber("10.0.0.0");
            ipAEnd = ConvertToNumber("10.255.255.255");

            ipBBegin = ConvertToNumber("172.16.0.0");
            ipBEnd = ConvertToNumber("172.31.255.255");

            ipCBegin = ConvertToNumber("192.168.0.0");
            ipCEnd = ConvertToNumber("192.168.255.255");
        }
        #endregion

        #region 公有方法
        /// <summary>
        /// 将IP地址转换为长整数
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <returns></returns>
        static public long ConvertToNumber(string ipAddress)
        {
            return ConvertToNumber(IPAddress.Parse(ipAddress));
        }
        /// <summary>
        /// 将IP地址转换为长整数
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <returns></returns>
        static public long ConvertToNumber(IPAddress ipAddress)
        {
            var bytes = ipAddress.GetAddressBytes();
            return bytes[0] * 256 * 256 * 256 + bytes[1] * 256 * 256 + bytes[2] * 256 + bytes[3];
        }
        /// <summary>
        /// true表示为内网IP
        /// </summary>
        /// <param name="ipAddress">ip地址</param>
        /// <returns></returns>
        static public bool IsIntranet(string ipAddress)
        {
            return IsIntranet(ConvertToNumber(ipAddress));
        }

        /// <summary>
        /// true表示为内网IP
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <returns></returns>
        static public bool IsIntranet(IPAddress ipAddress)
        {
            return IsIntranet(ConvertToNumber(ipAddress));
        }

        /// <summary>
        /// true表示为内网IP
        /// </summary>
        /// <param name="longIP"></param>
        /// <returns></returns>
        static public bool IsIntranet(long longIP)
        {
            return ((longIP >= ipABegin) && (longIP <= ipAEnd) ||
                    (longIP >= ipBBegin) && (longIP <= ipBEnd) ||
                    (longIP >= ipCBegin) && (longIP <= ipCEnd));
        }

        /// <summary>
        /// 获取本机内网IP
        /// </summary>
        /// <returns></returns>
        static public IPAddress GetLocalIntranetIP()
        {
            var list = Dns.GetHostEntry(Dns.GetHostName()).AddressList;
            foreach (var child in list)
            {
                if (IsIntranet(child)) return child;
            }

            return null;
        }

        /// <summary>
        /// 获取本机内网IP列表
        /// </summary>
        /// <returns></returns>
        static public List<IPAddress> GetLocalIntranetIPList()
        {
            var list = Dns.GetHostEntry(Dns.GetHostName()).AddressList;
            var result = new List<IPAddress>();
            foreach (var child in list)
            {
                if (IsIntranet(child)) result.Add(child);
            }

            return result;
        }
        #endregion
    }
}