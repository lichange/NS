//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using NS.Framework.IOC;
//using NS.Framework.Log;
//using Microsoft.Practices.Unity.InterceptionExtension;
//using System.ServiceModel;
//using NS.Framework.Faults;
//using NS.DDD.Core.Dto;
//using NS.Framework.Utility.Reflection;
//using Unity.Interception.InterceptionBehaviors;
//using Unity.Interception.Interceptors.TypeInterceptors.VirtualMethodInterception;
//using Unity.Interception.PolicyInjection.Pipeline;

//namespace NS.DDD.Core.AOP.Interceptors
//{
//    /// <summary>
//    /// 服务调用异常拦截器
//    /// </summary>
//    public class AppServiceExceptionInterceptionBehavior : IInterceptionBehavior
//    {
//        #region IInterceptionBehavior Members
//        /// <summary>
//        /// 获取当前行为需要拦截的对象类型接口。
//        /// </summary>
//        /// <returns>所有需要拦截的对象类型接口。</returns>
//        public IEnumerable<Type> GetRequiredInterfaces()
//        {
//            return Type.EmptyTypes;
//        }

//        /// <summary>
//        /// 通过实现此方法来拦截调用并执行所需的拦截行为。
//        /// </summary>
//        /// <param name="input">调用拦截目标时的输入信息。</param>
//        /// <param name="getNext">通过行为链来获取下一个拦截行为的委托。</param>
//        /// <returns>从拦截目标获得的返回信息。</returns>
//        public IMethodReturn Invoke(IMethodInvocation input, GetNextInterceptionBehaviorDelegate getNext)
//        {
//            var methodReturn = getNext().Invoke(input, getNext);
//            if (methodReturn.Exception != null)
//            {
//                BusinessLogicFault businessFault = new BusinessLogicFault();
//                businessFault.Operation = string.Format("{0}.{1}", input.Target.GetType().FullName, input.MethodBase.Name);
//                businessFault.Message = methodReturn.Exception.Message;

//                return new VirtualMethodReturn(input, new FaultException(businessFault.Message));
//                //BaseDto baseDto = new BaseDto();
//                //baseDto.IsSuccess=false;
//                //baseDto.Message=methodReturn.Exception.Message;
//                //baseDto.Operation = string.Format("{0}.{1}",input.Target.GetType().FullName, input.MethodBase.Name);

//                //var realArguments = (object[])ReflectHelper.GetInstanceProperty(input.Inputs, "arguments");

//                //methodReturn.ReturnValue = baseDto;

//                //AppServiceException appException = new AppServiceException(methodReturn.Exception.Message);
//                //appException.Source = string.Format("{0}.{1}", input.Target.GetType().FullName, input.MethodBase.Name);
//                //return new VirtualMethodReturn(input, appException);
//            }
//            return methodReturn;
//        }
//        /// <summary>
//        /// 获取一个<see cref="Boolean"/>值，该值表示当前拦截行为被调用时，是否真的需要执行
//        /// 某些操作。
//        /// </summary>
//        public bool WillExecute
//        {
//            get { return true; }
//        }

//        #endregion
//    }
//}
