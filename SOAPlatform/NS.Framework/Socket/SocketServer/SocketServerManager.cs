using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;

namespace NS.Framework.RocketSocket.Server
{
    /// <summary>
    /// socket服务器管理器控制台
    /// </summary>
    public class SocketServerManager
    {
        #region 私有成员
        /// <summary>
        /// 服务端宿主集合
        /// </summary>
        static private readonly List<SocketBase.IHost> _listHosts = new List<SocketBase.IHost>();
        #endregion

        #region 静态方法

        /// <summary>
        /// 初始化Socket Server
        /// </summary>
        static public void Init()
        {
            Init("socketServer");
        }

        /// <summary>
        /// 初始化Socket Server
        /// </summary>
        /// <param name="sectionName">配置文件中德sectionName</param>
        static public void Init(string sectionName)
        {
            if (string.IsNullOrEmpty(sectionName)) throw new ArgumentNullException("sectionName");
            Init(ConfigurationManager.GetSection(sectionName) as Config.SocketServerConfig);
        }

        /// <summary>
        /// 初始化Socket Server
        /// </summary>
        /// <param name="config"></param>
        static public void Init(Config.SocketServerConfig config)
        {
            if (config == null) throw new ArgumentNullException("config");
            if (config.Servers == null) return;

            foreach (Config.Server serverConfig in config.Servers)
            {
                //inti protocol
                var objProtocol = GetProtocol(serverConfig.Protocol);
                if (objProtocol == null) throw new InvalidOperationException("protocol");

                //init custom service
                var tService = Type.GetType(serverConfig.ServiceType, false);
                if (tService == null) throw new InvalidOperationException("serviceType");

                var serviceFace = tService.GetInterface(typeof(ISocketService<>).Name);
                if (serviceFace == null) throw new InvalidOperationException("serviceType");

                var objService = Activator.CreateInstance(tService);
                if (objService == null) throw new InvalidOperationException("serviceType");

                //init host.
                var host = Activator.CreateInstance(typeof(SocketServer<>).MakeGenericType(
                    serviceFace.GetGenericArguments()),
                    objService,
                    objProtocol,
                    serverConfig.SocketBufferSize,
                    serverConfig.MessageBufferSize,
                    serverConfig.MaxMessageSize,
                    serverConfig.MaxConnections) as BaseSocketServer;

                host.AddListener(serverConfig.Name, new IPEndPoint(IPAddress.Any, serverConfig.Port));

                _listHosts.Add(host);
            }
        }

        /// <summary>
        /// 获取当前协议
        /// </summary>
        /// <param name="protocol">协议类型</param>
        /// <returns></returns>
        static public object GetProtocol(string protocol)
        {
            switch (protocol)
            {
                case Protocol.ProtocolNames.AsyncBinary: return new Protocol.AsyncBinaryProtocol();
                case Protocol.ProtocolNames.Thrift: return new Protocol.ThriftProtocol();
                case Protocol.ProtocolNames.CommandLine: return new Protocol.CommandLineProtocol();
            }
            return Activator.CreateInstance(Type.GetType(protocol, false));
        }

        /// <summary>
        /// 启动服务
        /// </summary>
        static public void Start()
        {
            foreach (var server in _listHosts) server.Start();
        }
        /// <summary>
        /// 停止服务
        /// </summary>
        static public void Stop()
        {
            foreach (var server in _listHosts) server.Stop();
        }
        #endregion
    }
}