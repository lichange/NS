using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity.Registration;

namespace NS.Framework.IOC
{
    /// <summary>
    /// 规则处理Handle注册容器
    /// </summary>
    public class RuleHandlerContainer
    {
        public static IList<object> GetRuleHandlers(Type modelType, string ruleType, string targetName)
        {
            return InternaRuleHandlerFactory.Instance.GetRuleHandlers(modelType, ruleType, targetName);
        }

        public static IList<object> GetRuleHandlers<T>()
        {
            return InternaRuleHandlerFactory.Instance.GetRuleHandlers<T>();
        }

        /// <summary>
        /// 注册规则
        /// </summary>
        ///<param name="handlerType">处理器类型</param>
        /// <returns>返回操作的结果</returns>
        public static bool RegisterRuleHandler(Type handlerType)
        {
            return InternaRuleHandlerFactory.Instance.RegisterHandler(handlerType);
        }

        /// <summary>
        /// 取消注册规则
        /// </summary>
        /// <param name="handlerType">处理器类型</param>
        /// <returns></returns>
        public static bool UnregisterRuleHandler(Type handlerType)
        {
            return InternaRuleHandlerFactory.Instance.UnregisterHandler(handlerType);
        }

        /// <summary>
        /// 取消注册规则
        /// </summary>
        /// <param name="handlerTypes"></param>
        public static void UnregisterRuleHandler(params Type[] handlerTypes)
        {
            if(handlerTypes.Length==0)
                return;

            foreach (var handlerType in handlerTypes)
            {
                if (handlerType==null)
                    continue;

                InternaRuleHandlerFactory.Instance.UnregisterHandlers(handlerType);
            }
        }

        public static int GetRegisterCount()
        {
            return InternaRuleHandlerFactory.Instance.GetRegisterCount();
        }

        public static IEnumerable<IContainerRegistration> GetRegisterItems()
        {
            return InternaRuleHandlerFactory.Instance.GetRegisterItems();
        }
    }
}
