﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NS.Framework.Config
{
    /// <summary>
    /// 平台配置信息
    /// </summary>
    [Serializable]
    public class PlatformConfig
    {
        #region 平台单一实例

        /// <summary>
        /// 静态单例-客户端配置
        /// </summary>
        private static ClientSettings clientInstance;

        /// <summary>
        /// 静态单例-服务端配置
        /// </summary>
        private static ServerSettings serverInstance;

        /// <summary>
        /// 线程安全的锁
        /// </summary>
        private static Object syncLock = new Object();


        /// <summary>
        /// 客户端配置
        /// </summary>
        public static ClientSettings ClientConfig
        {
            get
            {
                lock (syncLock)
                {
                    if (clientInstance == null)
                    {
                        clientInstance = NS.Framework.Utility.Xml.XmlHelper.Instance.LoadConfig<ClientSettings>();

                        if (clientInstance == null)
                        {
                            clientInstance = new ClientSettings();
                            NS.Framework.Utility.Xml.XmlHelper.Instance.SaveConfig(clientInstance);
                        }
                    }
                    return clientInstance;
                }
            }
        }

        /// <summary>
        /// 服务器端配置
        /// </summary>
        public static ServerSettings ServerConfig
        {
            get
            {
                lock (syncLock)
                {
                    if (serverInstance == null)
                    {
                        serverInstance = NS.Framework.Utility.Xml.XmlHelper.Instance.LoadConfig<ServerSettings>();

                        if (serverInstance == null)
                        {
                            serverInstance = new ServerSettings();
                            NS.Framework.Utility.Xml.XmlHelper.Instance.SaveConfig(serverInstance);
                        }
                    }

                    if (serverInstance != null)
                        serverInstance.ConfigFilePath = NS.Framework.Utility.Xml.XmlHelper.CurrentConfigPath;

                    return serverInstance;
                }
            }
        }

        public static T GetConfig<T>() where T : class, new()
        {
            return NS.Framework.Utility.Xml.XmlHelper.Instance.LoadConfig<T>();
        }

        public static void UpdateServerConfig(ServerSettings serverConfig)
        {
            lock (syncLock)
                serverInstance = serverConfig;
        }

        public static void SaveConfig(object config)
        {
            lock (syncLock)
                NS.Framework.Utility.Xml.XmlHelper.Instance.SaveConfig(config);
        }

        public static void UpdateClientConfig(ClientSettings clientConfig)
        {
            lock (syncLock)
                clientInstance = clientConfig;
        }

        #endregion
    }
}
