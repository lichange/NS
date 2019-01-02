using NS.Framework.Global;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace NS.Framework.Utility.Xml
{
    /// <summary>
    /// XML文件的操作辅助类
    /// </summary>
    public class XmlHelper
    {
        private static readonly object lock_flag = new object();
        public static XmlHelper context = null;
        private static ConcurrentDictionary<Type, XmlSerializer> _cache;
        private static XmlSerializerNamespaces _defaultNamespace;

        //配置文件存储路径
        private static string ConfigPath;
        private static string BinPath;

        public static XmlHelper Instance
        {
            get
            {
                if (context == null)
                {
                    lock (lock_flag)
                    {
                        if (context == null)
                        {
                            context = new XmlHelper();
                        }
                    }
                }
                return context;
            }
        }


        static XmlHelper()
        {
            BinPath = System.AppDomain.CurrentDomain.BaseDirectory;

            if (BinPath.LastIndexOf("bin") > 0)
                ConfigPath = System.IO.Path.Combine(BinPath, "Config");
            else if (BinPath.LastIndexOf("Out") > 0 && BinPath.LastIndexOf("TestResults") > 0)
            {
                var tempPath = BinPath.Substring(0, BinPath.LastIndexOf("TestResults"));
                var tempParentPath = System.IO.Path.GetDirectoryName(tempPath);
                tempParentPath = tempParentPath.Substring(0, tempParentPath.LastIndexOf("\\"));
                tempParentPath = System.IO.Path.Combine(tempParentPath, "Publish");
                ConfigPath = System.IO.Path.Combine(tempParentPath, "Config");
                //TestResults\Deploy_Administrator 2013-12-18 09_36_41\Out
            }
            else if (BinPath.LastIndexOf("Out") > 0 && BinPath.LastIndexOf("TeamCity") > 0)
            {
                var tempPath = BinPath.Substring(0, BinPath.LastIndexOf("TeamCity"));
                var tempParentPath = System.IO.Path.Combine(tempPath, "TeamCity");
                tempParentPath = System.IO.Path.Combine(tempParentPath, "Publish");
                ConfigPath = System.IO.Path.Combine(tempParentPath, "Config");
                //TestResults\Deploy_Administrator 2013-12-18 09_36_41\Out
            }
            else if (BinPath.LastIndexOf("WebApplication") > 0)
            {
                var tempPath = System.IO.Path.Combine(BinPath, "bin");
                ConfigPath = System.IO.Path.Combine(tempPath, "Config");
            }
            else if (BinPath.LastIndexOf("Publish") < 0)
                ConfigPath = System.IO.Path.Combine(BinPath, "bin");
            else
                ConfigPath = System.IO.Path.Combine(BinPath, "Config");

            if (NSHttpContext.Current != null && !ConfigPath.EndsWith("bin"))
                ConfigPath = Path.Combine(ConfigPath, "bin", "Config");

            if (ConfigPath.EndsWith("bin"))
                ConfigPath = Path.Combine(ConfigPath, "Config");

            _defaultNamespace = new XmlSerializerNamespaces();
            _defaultNamespace.Add(string.Empty, string.Empty);

            _cache = new ConcurrentDictionary<Type, XmlSerializer>();
        }

        public static string CurrentConfigPath
        {
            get
            {
                return ConfigPath;
            }
        }

        private static XmlSerializer GetSerializer<T>()
        {
            var type = typeof(T);
            return _cache.GetOrAdd(type, XmlSerializer.FromTypes(new[] { type }).FirstOrDefault());
        }

        private static XmlSerializer GetSerializer(Type type)
        {
            return _cache.GetOrAdd(type, XmlSerializer.FromTypes(new[] { type }).FirstOrDefault());
        }

        #region 存取配置文件

        /// <summary>
        /// 加载配置
        /// </summary>
        /// <typeparam name="T">配置类型</typeparam>
        /// <returns></returns>
        public T LoadConfig<T>() where T : class, new()
        {
            Type t = typeof(T);

            string path = System.IO.Path.Combine(ConfigPath, t.Name + ".xml");
            try
            {
                if (!System.IO.File.Exists(path))
                {
                    path = System.IO.Path.Combine(ConfigPath.Replace("\\Config", ""), t.Name + ".xml");
                }

                if (!System.IO.File.Exists(path))
                    return null;
                else
                {
                    T obj = null;

                    using (System.IO.StreamReader sr = new System.IO.StreamReader(path, System.Text.Encoding.Unicode))
                    {
                        obj = GetSerializer<T>().Deserialize(sr) as T;
                        sr.Close();
                    }
                    return obj;
                }
            }
            catch (Exception ex)
            {
                return null;
            }

            return null;
        }

        /// <summary>
        /// 加载配置
        /// </summary>
        /// <typeparam name="T">配置类型</typeparam>
        /// <returns></returns>
        public object LoadConfig(Type t, string configName)
        {
            string path = System.IO.Path.Combine(ConfigPath, configName + ".xml");
            if (!System.IO.File.Exists(path))
            {
                path = System.IO.Path.Combine(ConfigPath.Replace("\\Config", ""), t.Name + ".xml");
            }
            if (!System.IO.File.Exists(path))
                return null;
            else
            {
                object obj = null;

                using (System.IO.StreamReader sr = new System.IO.StreamReader(path, System.Text.Encoding.Unicode))
                {
                    obj = GetSerializer(t).Deserialize(sr);
                    sr.Close();
                }
                return obj;
            }
            return null;
        }

        /// <summary>
        /// 保存配置
        /// </summary>
        /// <param name="cfg">要保存的配置对象</param>
        public void SaveConfig(object cfg)
        {
            if (cfg == null)
                throw new ArgumentNullException();

            try
            {
                Type t = cfg.GetType();

                var tempDirectoryPath = ConfigPath;

                if (!Directory.Exists(ConfigPath))
                {
                    tempDirectoryPath = ConfigPath.Replace("\\Config", "");
                }

                string path = System.IO.Path.Combine(tempDirectoryPath, t.Name + ".xml");
                System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(path));

                using (System.IO.StreamWriter sr = new System.IO.StreamWriter(path, false, System.Text.Encoding.Unicode))
                {
                    GetSerializer(t).Serialize(sr, cfg);
                    sr.Flush();
                    sr.Close();
                }
            }
            catch (Exception ex)
            {
            }
        }

        /// <summary>
        /// 保存配置
        /// </summary>
        /// <param name="cfg">要保存的配置对象</param>
        public void SaveConfig(string configName, object cfg)
        {
            if (cfg == null)
                throw new ArgumentNullException();

            Type t = cfg.GetType();

            var tempDirectoryPath = ConfigPath;

            if (!Directory.Exists(ConfigPath))
            {
                tempDirectoryPath = ConfigPath.Replace("\\Config", "");
            }

            string path = System.IO.Path.Combine(tempDirectoryPath, configName + ".xml");
            System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(path));

            using (System.IO.StreamWriter sr = new System.IO.StreamWriter(path, false, System.Text.Encoding.Unicode))
            {
                GetSerializer(t).Serialize(sr, cfg);
                sr.Close();
            }
        }

        #endregion

        #region 修改指定XML节的信息
        public void SetNhibernateXmlAttributeValue(string filePath, string attributeName, string value)
        {
            try
            {
                if (!System.IO.File.Exists(filePath))
                    return;

                System.Xml.XmlDocument document = new System.Xml.XmlDocument();
                document.Load(filePath);

                //hibernate-configuration
                var rootNode = document.DocumentElement;

                //session-factory
                var firstNode = rootNode.FirstChild;

                //propertys
                var propertyNodes = firstNode.ChildNodes;

                bool isBreak = false;
                foreach (System.Xml.XmlNode propertyNode in propertyNodes)
                {
                    var attributes = propertyNode.Attributes;

                    if (attributes.Count > 0)
                    {
                        foreach (System.Xml.XmlAttribute attribute in attributes)
                        {
                            if (attribute.Value == attributeName)
                            {
                                isBreak = true;
                                break;
                            }
                        }
                    }

                    if (isBreak)
                    {
                        var connectionStringNode = propertyNode.FirstChild;
                        connectionStringNode.Value = value;
                        break;
                    }
                }

                document.Save(filePath);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public string GetNhibernateXmlAttributeValue(string filePath, string attributeName)
        {
            string returnValue = string.Empty;
            try
            {
                if (!System.IO.File.Exists(filePath))
                    return returnValue;

                System.Xml.XmlDocument document = new System.Xml.XmlDocument();
                document.Load(filePath);

                //hibernate-configuration
                var rootNode = document.DocumentElement;

                //session-factory
                var firstNode = rootNode.FirstChild;

                //propertys
                var propertyNodes = firstNode.ChildNodes;

                bool isBreak = false;
                foreach (System.Xml.XmlNode propertyNode in propertyNodes)
                {
                    var attributes = propertyNode.Attributes;

                    if (attributes.Count > 0)
                    {
                        foreach (System.Xml.XmlAttribute attribute in attributes)
                        {
                            if (attribute.Value == attributeName)
                            {
                                isBreak = true;
                                break;
                            }
                        }
                    }

                    if (isBreak)
                    {
                        var connectionStringNode = propertyNode.FirstChild;
                        returnValue = connectionStringNode.Value;
                        break;
                    }
                }

                return returnValue;
            }
            catch (Exception)
            {
                return returnValue;
            }
        }
        #endregion

        #region 将Object通过xml序列化的方式，序列化为xml字符串

        /**/
        /// <summary> 
        /// 序列化对象 
        /// </summary> 
        /// <typeparam name=\"T\">对象类型</typeparam> 
        /// <param name=\"t\">对象</param> 
        /// <returns></returns> 
        public string SerializeToString(object t)
        {
            if (t == null)
                throw new ArgumentNullException("待保存的配置文件信息不能为null");

            using (StringWriter sw = new StringWriter())
            {
                GetSerializer(t.GetType()).Serialize(sw, t);
                return sw.ToString();
            }
        }

        /**/
        /// <summary> 
        /// 反序列化为对象 
        /// </summary> 
        /// <param name=\"type\">对象类型</param> 
        /// <param name=\"s\">对象序列化后的Xml字符串</param> 
        /// <returns></returns> 
        public object DeserializeStringToObject(Type type, string s)
        {
            if (string.IsNullOrEmpty(s))
                throw new ArgumentNullException("配置文件内容为null，无法读取配置文件内容");

            using (StringReader sr = new StringReader(s))
            {
                return GetSerializer(type).Deserialize(sr);
            }
        }

        #endregion

        #region XmlDocument 操作

        /// <summary>
        /// 加载配置
        /// </summary>
        /// <typeparam name="T">配置类型</typeparam>
        /// <returns></returns>
        public T LoadConfigFile<T>(string filePath) where T : class, new()
        {
            Type t = typeof(T);
            try
            {
                if (!System.IO.File.Exists(filePath))
                    filePath = Path.Combine(filePath, t.Name + ".xml");

                if (!System.IO.File.Exists(filePath))
                    return null;
                else
                {
                    T obj = null;

                    using (System.IO.StreamReader sr = new System.IO.StreamReader(filePath, System.Text.Encoding.Unicode))
                    {
                        obj = GetSerializer<T>().Deserialize(sr) as T;
                        sr.Close();
                    }
                    return obj;
                }
            }
            catch (Exception ex)
            {
                return null;
            }

            return null;
        }

        /// <summary>
        /// 加载配置
        /// </summary>
        /// <typeparam name="T">配置类型</typeparam>
        /// <returns></returns>
        public object LoadConfigFile(Type t, string filePath)
        {
            if (!System.IO.File.Exists(filePath))
                return null;
            else
            {
                object obj = null;

                using (System.IO.StreamReader sr = new System.IO.StreamReader(filePath, System.Text.Encoding.Unicode))
                {
                    obj = GetSerializer(t).Deserialize(sr);
                    sr.Close();
                }
                return obj;
            }
            return null;
        }

        /// <summary>
        /// 保存配置
        /// </summary>
        /// <param name="cfg">要保存的配置对象</param>
        /// <param name="dirPath">文件夹路径</param>
        public void SaveConfigFile(object cfg, string dirPath)
        {
            if (cfg == null)
                throw new ArgumentNullException();

            try
            {
                Type t = cfg.GetType();

                string path = System.IO.Path.Combine(dirPath, t.Name + ".xml");
                System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(path));

                using (System.IO.StreamWriter sr = new System.IO.StreamWriter(path, false, System.Text.Encoding.Unicode))
                {
                    GetSerializer(t).Serialize(sr, cfg);
                    sr.Close();
                }
            }
            catch (Exception ex)
            {
            }
        }

        /// <summary>
        /// 保存配置
        /// </summary>
        /// <param name="cfg">要保存的配置对象</param>
        public void SaveConfigFile(string dirPath, object cfg)
        {
            if (cfg == null)
                throw new ArgumentNullException();

            Type t = cfg.GetType();

            string path = System.IO.Path.Combine(dirPath, t.Name + ".xml");

            if (!System.IO.Directory.Exists(dirPath))
                System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(path));

            using (System.IO.StreamWriter sr = new System.IO.StreamWriter(path, false, System.Text.Encoding.Unicode))
            {
                GetSerializer(t).Serialize(sr, cfg);
                sr.Close();
            }
        }

        #endregion
    }
}
