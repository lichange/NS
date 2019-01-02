//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.ServiceModel.Activation;
//using System.ServiceModel.Web;
//using System.Text;
//using System.ServiceModel;
//using System.ServiceModel.Security;

//namespace NS.Framework.Service.Implement
//{
//    /// <summary>
//    /// WCF服务安全辅助类
//    /// </summary>
//    internal class ServiceSecurityHelper
//    {
//        #region 单例

//        private static readonly object flag = new object();
//        private static ServiceSecurityHelper iSecurityHelper;

//        public static ServiceSecurityHelper SecurityHelper
//        {
//            get
//            {
//                if (iSecurityHelper == null)
//                {
//                    lock (flag)
//                    {
//                        if (iSecurityHelper == null)
//                        {
//                            iSecurityHelper = new ServiceSecurityHelper();
//                        }
//                    }
//                }
//                return iSecurityHelper;
//            }
//        }

//        #endregion

//        #region Binging Setting

//        #region X509证书认证方式

//        private System.ServiceModel.Channels.Binding SetX509SecurityMode(System.ServiceModel.Channels.Binding binding)
//        {

//            if (binding is NetTcpBinding)
//            {
//                var tempBinding = binding as NetTcpBinding;

//                tempBinding.Security.Mode = SecurityMode.Transport;
//                //tempBinding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;
//                tempBinding.Security.Transport.ClientCredentialType = TcpClientCredentialType.None;
//                //tempBinding.Security.Message.ClientCredentialType = MessageCredentialType.UserName;
//            }
//            else if (binding is WebHttpBinding)
//            {
//                var tempBinding = binding as WebHttpBinding;

//                tempBinding.Security.Mode = WebHttpSecurityMode.Transport;
//                //tempBinding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;
//                tempBinding.Security.Transport.ClientCredentialType = HttpClientCredentialType.None;
//            }
//            else if (binding is WSHttpBinding)
//            {
//                var tempBinding = binding as WSHttpBinding;
//                tempBinding.Security.Mode = SecurityMode.Transport;
//                tempBinding.Security.Transport.ClientCredentialType = HttpClientCredentialType.None;
//                tempBinding.Security.Transport.ProxyCredentialType = HttpProxyCredentialType.None;
//                //tempBinding.Security.Message.ClientCredentialType = MessageCredentialType.UserName;
//            }
//            else if (binding is BasicHttpBinding)
//            {
//                var tempBinding = binding as BasicHttpBinding;
//                tempBinding.Security.Mode = BasicHttpSecurityMode.Transport;
//                tempBinding.Security.Transport.ClientCredentialType = HttpClientCredentialType.None;
//                tempBinding.Security.Transport.ProxyCredentialType = HttpProxyCredentialType.None;
//                //tempBinding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;
//                //tempBinding.Security.Message.ClientCredentialType = BasicHttpMessageCredentialType.UserName;
//            }
//            else if (binding is NetMsmqBinding)
//            {

//            }
//            else if (binding is NetNamedPipeBinding)
//            {

//            }
//            else if (binding is NetPeerTcpBinding)
//            {

//            }
//            return binding;
//        }

//        #endregion

//        #region Windows认证方式

//        public System.ServiceModel.Channels.Binding SetWindowsSecurityMode(System.ServiceModel.Channels.Binding binding)
//        {


//            return binding;
//        }

//        #endregion

//        #region 用户名密码认证方式

//        public System.ServiceModel.Channels.Binding SetUserPasswordSecurityMode(System.ServiceModel.Channels.Binding binding)
//        {


//            return binding;
//        }

//        #endregion

//        #region Issued Tokens认证方式

//        public System.ServiceModel.Channels.Binding SetTokenSecurityMode(System.ServiceModel.Channels.Binding binding)
//        {


//            return binding;
//        }

//        #endregion

//        #endregion

//        #region ServiceBehavior
//        private System.ServiceModel.ServiceHost SetX509SecurityMode(System.ServiceModel.ServiceHost host)
//        {
//            //host.Credentials.ServiceCertificate.SetCertificate(System.Security.Cryptography.X509Certificates.StoreLocation.LocalMachine, System.Security.Cryptography.X509Certificates.StoreName.My, System.Security.Cryptography.X509Certificates.X509FindType.FindByIssuerName, "CN=ejiyuan");
//            //<!--自定义对客户端进行证书认证方式 这里为 None-->
//            host.Credentials.ClientCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.None;

//            //< !--自定义用户名和密码验证的设置-- >
//            host.Credentials.UserNameAuthentication.UserNamePasswordValidationMode = System.ServiceModel.Security.UserNamePasswordValidationMode.Custom;
//            host.Credentials.UserNameAuthentication.CustomUserNamePasswordValidator = new UserNamePasswordValidator();

//            return host;
//        }
//        #endregion

//        #region Client Proxy

//        private ChannelFactory<T> CredentialsChannelFactory<T>(ChannelFactory<T> channelFactory) where T : class
//        {
//            channelFactory.Credentials.ServiceCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.None;
//            channelFactory.Credentials.UserName.UserName = "ejiyuan";
//            channelFactory.Credentials.UserName.Password = "123456";

//            return channelFactory;
//        }

//        #endregion
//    }

//    public class UserNamePasswordValidator : System.IdentityModel.Selectors.UserNamePasswordValidator
//    {
//        public override void Validate(string userName, string password)
//        {
//            if (userName != "ejiyuan" || password != "123456")
//            {
//                throw new System.IdentityModel.Tokens.SecurityTokenException("Unknown Username or Password");
//            }
//        }
//    }
//}
