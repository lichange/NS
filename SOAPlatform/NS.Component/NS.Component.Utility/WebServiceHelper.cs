//namespace NS.Component.Utility
//{
//    using Microsoft.CSharp;
//    using System;
//    using System.CodeDom;
//    using System.CodeDom.Compiler;
//    using System.Collections.Generic;
//    using System.Net;
//    using System.Reflection;
//    using System.ServiceModel;
//    using System.Text;
//    using System.Web.Services.Description;

//    public static class WebServiceHelper
//    {
//        private static IDictionary<string, Type> WSProxyTypeDictionary = new Dictionary<string, Type>();

//        private static string GetWsClassName(string wsUrl)
//        {
//            string[] strArray = wsUrl.Split(new char[] { '/' });
//            return strArray[strArray.Length - 1].Split(new char[] { '.' })[0];
//        }

//        public static Type GetWsProxyType(string wsUrl, string classname)
//        {
//            string name = "NS.WebService.DynamicWebCalling";
//            if ((classname == null) || (classname == ""))
//            {
//                classname = GetWsClassName(wsUrl);
//            }
//            string key = wsUrl + "@" + classname;
//            if (WSProxyTypeDictionary.ContainsKey(key))
//            {
//                return WSProxyTypeDictionary[key];
//            }
//            WebClient client = new WebClient();
//            ServiceDescription serviceDescription = ServiceDescription.Read(client.OpenRead(wsUrl + "?WSDL"));
//            ServiceDescriptionImporter importer = new ServiceDescriptionImporter();
//            importer.AddServiceDescription(serviceDescription, "", "");
//            CodeNamespace namespace2 = new CodeNamespace(name);
//            CodeCompileUnit codeCompileUnit = new CodeCompileUnit();
//            codeCompileUnit.Namespaces.Add(namespace2);
//            importer.Import(namespace2, codeCompileUnit);
//            ICodeCompiler compiler = new CSharpCodeProvider().CreateCompiler();
//            CompilerParameters options = new CompilerParameters
//            {
//                GenerateExecutable = false,
//                GenerateInMemory = true
//            };
//            options.ReferencedAssemblies.Add("System.dll");
//            options.ReferencedAssemblies.Add("System.XML.dll");
//            options.ReferencedAssemblies.Add("System.Web.Services.dll");
//            options.ReferencedAssemblies.Add("System.Data.dll");
//            CompilerResults results = compiler.CompileAssemblyFromDom(options, codeCompileUnit);
//            if (results.Errors.HasErrors)
//            {
//                StringBuilder builder = new StringBuilder();
//                foreach (CompilerError error in results.Errors)
//                {
//                    builder.Append(error.ToString());
//                    builder.Append(Environment.NewLine);
//                }
//                throw new Exception(builder.ToString());
//            }
//            Assembly compiledAssembly = results.CompiledAssembly;
//            compiledAssembly.GetTypes();
//            Type type2 = compiledAssembly.GetType(name + "." + classname, true, true);
//            lock (WSProxyTypeDictionary)
//            {
//                if (!WSProxyTypeDictionary.ContainsKey(key))
//                {
//                    WSProxyTypeDictionary.Add(key, type2);
//                }
//            }
//            return type2;
//        }

//        public static object InvokeWebService(string wsUrl, string methodname, object[] args)
//        {
//            return InvokeWebService(wsUrl, null, methodname, args);
//        }

//        public static object InvokeWebService(string wsUrl, string classname, string methodname, object[] args)
//        {
//            object obj3;
//            try
//            {
//                Type wsProxyType = GetWsProxyType(wsUrl, classname);
//                object obj2 = Activator.CreateInstance(wsProxyType);
//                obj3 = wsProxyType.GetMethod(methodname).Invoke(obj2, args);
//            }
//            catch (Exception exception)
//            {
//                throw exception;
//            }
//            return obj3;
//        }
//    }

//    /// <summary>
//    /// 动态调用WCF的工具类库
//    /// </summary>
//    public class WCFInvokeContext
//    {

//        #region Wcf服务工厂
//        public static T CreateWCFServiceByURL<T>(string url)
//        {
//            return CreateWCFServiceByURL<T>(url, "wsHttpBinding");
//        }


//        public static T CreateWCFServiceByURL<T>(string url, string bing)
//        {
//            if (string.IsNullOrEmpty(url))
//                throw new NotSupportedException("this url isn`t Null or Empty!");
//            EndpointAddress address = new EndpointAddress(url);
//            System.ServiceModel.Channels.Binding binding = CreateBinding(bing);
//            ChannelFactory<T> factory = new ChannelFactory<T>(binding, address);
//            return factory.CreateChannel();
//        }
//        #endregion

//        #region 创建传输协议
//        /// <summary>
//        /// 创建传输协议
//        /// </summary>
//        /// <param name="binding">传输协议名称</param>
//        /// <returns></returns>
//        private static System.ServiceModel.Channels.Binding CreateBinding(string binding)
//        {
//            System.ServiceModel.Channels.Binding bindinginstance = null;
//            if (binding.ToLower() == "basichttpbinding")
//            {
//                BasicHttpBinding ws = new BasicHttpBinding();
//                ws.MaxBufferSize = 2147483647;
//                ws.MaxBufferPoolSize = 2147483647;
//                ws.MaxReceivedMessageSize = 2147483647;
//                ws.ReaderQuotas.MaxStringContentLength = 2147483647;
//                ws.CloseTimeout = new TimeSpan(0, 10, 0);
//                ws.OpenTimeout = new TimeSpan(0, 10, 0);
//                ws.ReceiveTimeout = new TimeSpan(0, 10, 0);
//                ws.SendTimeout = new TimeSpan(0, 10, 0);

//                bindinginstance = ws;
//            }
//            else if (binding.ToLower() == "netnamedpipebinding")
//            {
//                NetNamedPipeBinding ws = new NetNamedPipeBinding();
//                ws.MaxReceivedMessageSize = 65535000;
//                bindinginstance = ws;
//            }
//            else if (binding.ToLower() == "netpeertcpbinding")
//            {
//                NetPeerTcpBinding ws = new NetPeerTcpBinding();
//                ws.MaxReceivedMessageSize = 65535000;
//                bindinginstance = ws;
//            }
//            else if (binding.ToLower() == "nettcpbinding")
//            {
//                NetTcpBinding ws = new NetTcpBinding();
//                ws.MaxReceivedMessageSize = 65535000;
//                ws.Security.Mode = SecurityMode.None;
//                bindinginstance = ws;
//            }
//            else if (binding.ToLower() == "wsdualhttpbinding")
//            {
//                WSDualHttpBinding ws = new WSDualHttpBinding();
//                ws.MaxReceivedMessageSize = 65535000;

//                bindinginstance = ws;
//            }
//            else if (binding.ToLower() == "webhttpbinding")
//            {
//                //WebHttpBinding ws = new WebHttpBinding();
//                //ws.MaxReceivedMessageSize = 65535000;
//                //bindinginstance = ws;
//            }
//            else if (binding.ToLower() == "wsfederationhttpbinding")
//            {
//                WSFederationHttpBinding ws = new WSFederationHttpBinding();
//                ws.MaxReceivedMessageSize = 65535000;
//                bindinginstance = ws;
//            }
//            else if (binding.ToLower() == "wshttpbinding")
//            {
//                WSHttpBinding ws = new WSHttpBinding(SecurityMode.None);
//                ws.MaxReceivedMessageSize = 65535000;
//                ws.Security.Message.ClientCredentialType = System.ServiceModel.MessageCredentialType.Windows;
//                ws.Security.Transport.ClientCredentialType = System.ServiceModel.HttpClientCredentialType.Windows;
//                bindinginstance = ws;
//            }
//            return bindinginstance;

//        }
//        #endregion

//    }
//}
