//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.ServiceModel;
//using System.ServiceModel.Dispatcher;
//using System.ServiceModel.Description;
//using System.ServiceModel.Channels;
//using System.ServiceModel.Configuration;

//namespace NS.Framework.Service
//{
//    public class AttachUserNamePasswordBehavior : IClientMessageInspector, IDispatchMessageInspector, IEndpointBehavior
//    {
//        private static string UserName = "neusoft";
//        private static string Password = "1qaz!QAZ";

//        public AttachUserNamePasswordBehavior()
//        {
//        }

//        #region IClientMessageInspector 成员

//        public void AfterReceiveReply(ref System.ServiceModel.Channels.Message reply, object correlationState)
//        {

//        }

//        public object BeforeSendRequest(ref System.ServiceModel.Channels.Message request, System.ServiceModel.IClientChannel channel)
//        {
//            MessageHeader userNameHeader = MessageHeader.CreateHeader("OperationUserName", "http://neusoft.org", UserName, false, "");
//            MessageHeader pwdNameHeader = MessageHeader.CreateHeader("OperationPwd", "http://neusoft.org", Password, false, "");
//            request.Headers.Add(userNameHeader);
//            request.Headers.Add(pwdNameHeader);
//            Console.WriteLine(request);
//            return null;
//        }

//        #endregion

//        #region IDispatchMessageInspector 成员

//        string GetHeaderValue(string key)
//        {
//            int index = OperationContext.Current.IncomingMessageHeaders.FindHeader(key, "http://neusoft.org");
//            if (index >= 0)
//            {
//                return OperationContext.Current.IncomingMessageHeaders.GetHeader<string>(index).ToString();
//            }
//            return null;
//        }

//        public object AfterReceiveRequest(ref System.ServiceModel.Channels.Message request, System.ServiceModel.IClientChannel channel, System.ServiceModel.InstanceContext instanceContext)
//        {
//            //Console.WriteLine(request);
//            string username = GetHeaderValue("OperationUserName");
//            string pwd = GetHeaderValue("OperationPwd");
//            if (username == "neusoft" && pwd == "1qaz!QAZ")
//            {
//            }
//            else
//            {
//                throw new Exception("操作中的用户名，密码不正确！");
//            }
//            return null;
//        }

//        public void BeforeSendReply(ref System.ServiceModel.Channels.Message reply, object correlationState)
//        {

//        }

//        #endregion

//        #region IEndpointBehavior 成员

//        public void AddBindingParameters(ServiceEndpoint endpoint, System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
//        {

//        }

//        public void ApplyClientBehavior(ServiceEndpoint endpoint, System.ServiceModel.Dispatcher.ClientRuntime clientRuntime)
//        {
//            clientRuntime.MessageInspectors.Add(new AttachUserNamePasswordBehavior());
//        }

//        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, System.ServiceModel.Dispatcher.EndpointDispatcher endpointDispatcher)
//        {
//            endpointDispatcher.DispatchRuntime.MessageInspectors.Add(new AttachUserNamePasswordBehavior());
//        }

//        public void Validate(ServiceEndpoint endpoint)
//        {

//        }

//        #endregion
//    }
//}
