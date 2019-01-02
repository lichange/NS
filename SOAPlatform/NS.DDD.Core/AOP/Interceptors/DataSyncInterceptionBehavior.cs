using System.ComponentModel.DataAnnotations;
using Microsoft.Practices.Unity.InterceptionExtension;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using NS.DDD.Core.Validation;
using NS.Framework.Utility.Reflection;
using NS.Framework.IOC;
using NS.Framework.Log;
using NS.Framework.Attributes;
using NS.DDD.Core.Internal.Validation;
using ValidationAttribute = NS.Framework.Attributes.ValidationAttribute;
using ValidationException = NS.Framework.Exceptions.ValidationException;

using NS.DDD.Core.Dto;
using Unity.Interception.InterceptionBehaviors;
using Unity.Interception.PolicyInjection.Pipeline;

namespace NS.DDD.Core.AOP.Interceptors
{
    /// <summary>
    /// 离线处理拦截器。-实现服务调用检测，当服务不可用时-则自动转为本地存储
    /// </summary>
    public class DataSyncInterceptionBehavior : IInterceptionBehavior
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

            var methodReturn = getNext().Invoke(input, getNext);

            //获取当前操作的方法
            var tempMethod = input.MethodBase as System.Reflection.MethodInfo;

            var tempDataSyncAttributes = tempMethod.GetCustomAttributes(typeof(DataSyncAttribute), false);
            var tempDataSyncCloudAttributes = tempMethod.GetCustomAttributes(typeof(DataSyncCloudAttribute), false);

            //如果该方法上有标记
            if (tempDataSyncAttributes.Length > 0)
            {
                //方法参数
                var inputParameterCollection = input.Inputs;

                //根据反射获取对象的成员信息
                var realArguments = (object[])ReflectHelper.GetInstanceProperty(inputParameterCollection, "arguments");

                //获取当前方法所属的对象
                var tempTarget = input.Target;

                //生成对应的信息-并保存到数据库中
                DataSyncContract tempDataSyncContract = new DataSyncContract();

                if (realArguments.Length > 0)
                {
                    tempDataSyncContract.Data = NS.Framework.Utility.Xml.XmlHelper.Instance.SerializeToString(realArguments[0]);
                    tempDataSyncContract.DataType = string.Format("{0};{1}", realArguments[0].GetType().Assembly.GetName().Name + ".dll", realArguments[0].GetType().FullName);
                }

                tempDataSyncContract.Id = Guid.NewGuid().ToString();
                tempDataSyncContract.MethodName = tempMethod.Name;
                tempDataSyncContract.ServicePath = string.Format("{0};{1}", input.MethodBase.ReflectedType.Assembly.GetName().Name + ".dll", input.Target.GetType().FullName);
                tempDataSyncContract.OperationTime = DateTime.Now;
                tempDataSyncContract.SyncFlag = "0";
                var tempDataSyncAttribute = tempDataSyncAttributes[0] as DataSyncAttribute;

                //如果要求实时同步
                if (tempDataSyncAttribute.IsRealTimeSync)
                    this.RemoteCall(tempDataSyncContract);
                else //不要求实时同步的数据，就通过离线同步版本的方式来传输
                    this.SaveUnlineData(tempDataSyncContract);
            }
            //如果该方法上有标记
            if (tempDataSyncCloudAttributes.Length > 0)
            {
                //方法参数
                var inputParameterCollection = input.Inputs;

                //根据反射获取对象的成员信息
                var realArguments = (object[])ReflectHelper.GetInstanceProperty(inputParameterCollection, "arguments");

                //获取当前方法所属的对象
                var tempTarget = input.Target;

                //生成对应的信息-并保存到数据库中
                DataSyncContract tempDataSyncContract = new DataSyncContract();

                if (realArguments.Length > 0)
                {
                    tempDataSyncContract.Data = NS.Framework.Utility.Xml.XmlHelper.Instance.SerializeToString(realArguments[0]);
                    tempDataSyncContract.DataType = string.Format("{0};{1}", realArguments[0].GetType().Assembly.GetName().Name + ".dll", realArguments[0].GetType().FullName);
                }

                tempDataSyncContract.Id = Guid.NewGuid().ToString();
                tempDataSyncContract.MethodName = tempMethod.Name;
                tempDataSyncContract.ServicePath = string.Format("{0};{1}", input.MethodBase.ReflectedType.Assembly.GetName().Name + ".dll", input.Target.GetType().FullName);
                tempDataSyncContract.OperationTime = DateTime.Now;
                tempDataSyncContract.SyncFlag = "1";
                var tempDataSyncAttribute = tempDataSyncCloudAttributes[0] as DataSyncCloudAttribute;

                //如果要求实时同步
                if (tempDataSyncAttribute.IsRealTimeSync)
                    this.RemoteCall(tempDataSyncContract);
                else //不要求实时同步的数据，就通过离线同步版本的方式来传输
                    this.SaveUnlineData(tempDataSyncContract);
            }
            return methodReturn;
        }

        private void RemoteCall(DataSyncContract tempDataSyncContract)
        {
            //离线数据同步事件委托
            Action tempSyncAction = new Action(() =>
            {
                //同步离线数据到数据中心
                IRemoteDataSyncService remoteSyncService = ObjectContainer.CreateInstance<IRemoteDataSyncService>();
                if (!remoteSyncService.Sync(tempDataSyncContract))
                    this.SaveUnlineData(tempDataSyncContract);
            });

            //异步调用
            tempSyncAction.BeginInvoke(null, null);
        }

        private void SaveUnlineData(DataSyncContract tempDataSyncContract)
        {
            //离线数据同步事件委托
            Action tempSyncAction = new Action(() =>
            {
                //同步离线数据到数据中心
                ILocalDataSyncService syncService = ObjectContainer.CreateInstance<ILocalDataSyncService>();
                syncService.Sync(tempDataSyncContract);
            });

            //异步调用
            tempSyncAction.BeginInvoke(null, null);
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
