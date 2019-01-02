using System.ComponentModel.DataAnnotations;
using Microsoft.Practices.Unity.InterceptionExtension;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;

using NS.Framework.Utility.Reflection;
using NS.DDD.Core.Validation;
using NS.DDD.Core.Internal.Validation;
using NS.Framework.IOC;
using NS.Framework.Log;

using ValidationAttribute = NS.Framework.Attributes.ValidationAttribute;
using ValidationException = NS.Framework.Exceptions.ValidationException;
using Unity.Interception.InterceptionBehaviors;
using Unity.Interception.PolicyInjection.Pipeline;
using Unity.Interception.Interceptors.TypeInterceptors.VirtualMethodInterception;

namespace NS.DDD.Core.AOP.Interceptors
{
    /// <summary>
    /// 离线处理拦截器。-实现服务调用检测，当服务不可用时-则自动转为本地存储
    /// </summary>
    public class OfflineInterceptionBehavior : IInterceptionBehavior
    {
        #region IInterceptionBehavior Members
        /// <summary>
        /// 获取当前行为需要拦截的对象类型接口。
        /// </summary>
        /// <returns>所有需要拦截的对象类型接口。</returns>
        public IEnumerable<Type> GetRequiredInterfaces()
        {
            return Type.EmptyTypes;
        }

        /// <summary>
        /// 通过实现此方法来拦截调用并执行所需的拦截行为。
        /// </summary>
        /// <param name="input">调用拦截目标时的输入信息。</param>
        /// <param name="getNext">通过行为链来获取下一个拦截行为的委托。</param>
        /// <returns>从拦截目标获得的返回信息。</returns>
        public IMethodReturn Invoke(IMethodInvocation input, GetNextInterceptionBehaviorDelegate getNext)
        {
            if (input == null)
                throw new ArgumentNullException("input");
            if (getNext == null)
                throw new ArgumentNullException("getNext");

            //获取当前操作的方法
            var tempMethod = input.MethodBase as System.Reflection.MethodInfo;

            //方法参数
            var inputParameterCollection = input.Inputs;

            //根据反射获取对象的成员信息
            var realArguments = (object[])ReflectHelper.GetInstanceProperty(inputParameterCollection, "arguments");

            //获取当前方法所属的对象
            var tempTarget = input.Target;

            var methodReturn = getNext().Invoke(input, getNext);
            if (methodReturn.Exception != null && (methodReturn.Exception is TimeoutException))
            {
                //将当前操作的参数序列化到对象中
                var tempResult = this.PreDealOfflineDto(input, tempTarget, tempMethod, realArguments);

                return new VirtualMethodReturn(input, tempResult, realArguments);
            }
            else
            {
                //离线数据同步事件委托
                Action tempSyncAction = new Action(() =>
                {
                    //同步离线数据到数据中心
                    IOfflineSyncService syncService = ObjectContainer.CreateInstance<IOfflineSyncService>();
                    syncService.Sync();
                });

                //异步调用
                tempSyncAction.BeginInvoke(null, null);
            }

            return methodReturn;
        }

        private object PreDealOfflineDto(IMethodInvocation input, object tempTarget, MethodInfo tempMethod, IList<object> parameterInfos)
        {
            if (tempTarget == null)
                throw new ArgumentNullException("方法请求调用的服务对象不能为空");

            if (tempMethod == null)
                throw new ArgumentNullException("请求调用的方法不能为空");

            //获取相关接口
            var targetType = tempTarget.GetType();

            //离线数据操控器--从离线的IOC中加载
            var targetOfflineService = OfflineObjectContainer.CreateInstance(targetType);

            var tempOfflineMethod = targetOfflineService.GetType().GetMethod(tempMethod.Name, BindingFlags.Public);

            object returenValue = null;

            if (tempOfflineMethod != null)
            {
                returenValue = tempOfflineMethod.Invoke(targetOfflineService, parameterInfos.ToArray());
            }

            return returenValue;
        }

        /// <summary>
        /// 获取一个<see cref="Boolean"/>值，该值表示当前拦截行为被调用时，是否真的需要执行
        /// 某些操作。
        /// </summary>
        public bool WillExecute
        {
            get
            {
                return true;
            }
        }

        #endregion
    }
}
