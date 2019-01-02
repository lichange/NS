///****************************************************************************************************************
//*                                                                                                               *
//* Copyright (C) 2011 5173.com                                                                                   *
//* This project may be copied only under the terms of the Apache License 2.0.                                    *
//* Please visit the project Home Page http://wcfextension.codeplex.com/ for more detail.                         *
//*                                                                                                               *
//****************************************************************************************************************/

//namespace WcfExtension
//{
//    using System.ServiceModel.Channels;
//    using System.ServiceModel.Description;
//    using System.ServiceModel.Dispatcher;

//    /// <summary>
//    /// 扩展WCF服务终结点的操作行为
//    /// </summary>
//    internal class OperationBehavior : IOperationBehavior
//    {
//        protected OperationInvoker CreateInvoker(IOperationInvoker oldInvoker)
//        {
//            return new OperationInvoker(oldInvoker);
//        }

//        public void AddBindingParameters(OperationDescription operationDescription, BindingParameterCollection bindingParameters)
//        { }

//        public void ApplyClientBehavior(OperationDescription operationDescription, ClientOperation clientOperation)
//        { }

//        public void ApplyDispatchBehavior(OperationDescription operationDescription, DispatchOperation dispatchOperation)
//        {
//            IOperationInvoker oldInvoker = dispatchOperation.Invoker;
//            dispatchOperation.Invoker = CreateInvoker(oldInvoker);
//        }

//        public void Validate(OperationDescription operationDescription)
//        { }
//    }
//}
