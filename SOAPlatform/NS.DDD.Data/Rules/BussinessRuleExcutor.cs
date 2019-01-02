using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Unity;

using NS.DDD.Core.Extensions;
using NS.DDD.Core;
using NS.DDD.Core.Repository;
using NS.Framework.IOC;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;
using System.Reflection;
using System.Collections;

namespace NS.DDD.Data.Rules
{
    /// <summary>
    /// 业务规则执行器
    /// </summary>
    public class BussinessRuleExcutor
    {
        /// <summary>
        /// 传入DTO，然后执行转换操作，转换完成后，自动执行领域上的业务操作，只执行具有业务规则的相关方法。
        /// </summary>
        /// <param name="dto">目标Dto类型</param>
        /// <param name="domainObj">转换后的领域对象</param>
        /// <param name="contextObj">数据库中根据主键查询出来的主键</param>
        /// <returns></returns>
        public static bool Excute(object dto, object domainObj, object contextObj, IRepositoryContext context)
        {
            //0、先调用Dto转换为Domain
            if (dto == null)
                throw new AppServiceException("待操作的对象不能为空");

            //if (domainObj == null)
            //    throw new AppServiceException("待操作的对象不能为空");

            //1、转换完成后，只想相应的操作。
            //object domainObj = dto.GetMapTarget(dto.GetMapTargetType());

            if (domainObj == null)
                throw new AppServiceException("转换完成后的领域对象不能为空");

            var primaryKeyValue = domainObj.GetType().GetProperties().Where(pre => pre.Name == "AggregateID").FirstOrDefault().GetValue(domainObj, null);

            //2、将领域内部的所有打了特殊标记的Rule 执行。

            var allPropertys = domainObj.GetType().GetProperties();

            if (allPropertys == null || allPropertys.Length == 0)
                return true;

            var allMthods = domainObj.GetType().GetMethods();

            if (allMthods == null || allMthods.Length == 0)
                return true;

            foreach (var tempProperty in allPropertys)
            {
                var attributes = (DependencyAttribute[])tempProperty.GetCustomAttributes(typeof(DependencyAttribute), false);

                if (attributes == null || attributes.Length == 0)
                    continue;

                //根据参数类型获取参数的实例
                var tempRule = ObjectContainer.CreateInstance(tempProperty.PropertyType);

                if (tempRule == null)
                    continue;

                //设置领域中的属性
                tempProperty.SetValue(domainObj, tempRule, null);

                //要求执行规则的方法与规则名称差别不大-按照去除规则名称的Rule四个字母
                var tempMethodName = tempProperty.Name.Replace("Rule", "");
                if (string.IsNullOrEmpty(tempMethodName))
                    continue;

                var parameterName = string.Empty;
                var tempName = string.Empty;

                #region 判断执行的操作
                //(primaryKeyValue == null || (contextObj == null)) &&
                if ((tempMethodName.StartsWith("Add")))
                {
                    tempName = tempMethodName.Replace("Add", "");
                    if (tempName == domainObj.GetType().Name)
                        parameterName = "Childrens";
                    else
                        parameterName = tempName;
                }
                else if ((tempMethodName.StartsWith("Set")))
                {
                    tempName = tempMethodName.Replace("Set", "");
                    parameterName = tempName;
                }
                else if (contextObj != null && tempMethodName.StartsWith("Remove"))
                {
                    tempName = tempMethodName.Replace("Remove", "");
                    parameterName = tempName;
                }
                else
                    continue;

                #endregion

                if (string.IsNullOrEmpty(parameterName))
                    continue;

                var tempSelectProperty = dto.GetType().GetProperty(parameterName);

                if (tempSelectProperty == null)
                {
                    if (tempMethodName.StartsWith("Set"))
                    {
                        parameterName = parameterName + "Id";

                        tempSelectProperty = dto.GetType().GetProperty(parameterName);

                        if (tempSelectProperty == null)
                            continue;
                    }
                    else if (tempMethodName.StartsWith("Add"))
                    {
                        parameterName = parameterName + "s";

                        tempSelectProperty = dto.GetType().GetProperty(parameterName);

                        if (tempSelectProperty == null)
                            continue;
                    }
                }

                if (tempSelectProperty == null)
                    continue;

                //获取参数值
                var propertyValue = tempSelectProperty.GetValue(dto, null);
                if (propertyValue == null)
                    continue;

                //判定指定名称的方法是否存在。
                if (allMthods.Where(pre => pre.Name.ToLower().StartsWith(tempMethodName.ToLower())).Count() == 0)
                    continue;

                //查询当前类型中与制定名称匹配的方法集合
                var tempMethods = allMthods.Where(pre => pre.Name.ToLower().StartsWith(tempMethodName.ToLower())).ToList();

                //循环方法列表
                foreach (var tempMethod in tempMethods)
                {
                    if (tempMethod.GetParameters().Length == 0)
                        continue;

                    //判定当前方法是否是泛型方法
                    if (propertyValue.GetType().GetGenericArguments().Count() > 0)
                    {
                        //判定当前泛型方法集合于当前的获取的参数值是否匹配-如果匹配则执行批量操作。
                        if (tempMethod.GetParameters().Where(pre => pre.ParameterType == propertyValue.GetType()).Count() > 0)
                        {
                            tempMethod.Invoke(domainObj, (propertyValue as IEnumerable<object>).ToArray());

                            break;
                        }
                    }

                    //判断当前属性值是否是泛型集合
                    if (propertyValue is IEnumerable<object>)
                    {
                        var tempList = propertyValue as IEnumerable<object>;
                        foreach (var tempItem in tempList)
                        {
                            if (tempItem == null)
                                continue;

                            var tempTargetValue = tempItem.GetMapTarget(tempMethod.GetParameters()[0].ParameterType);

                            if (tempTargetValue == null)
                                continue;

                            tempMethod.Invoke(domainObj, new object[] { tempTargetValue });
                        }
                        break;
                    }
                    else
                    {
                        var parameterType = tempMethod.GetParameters()[0].ParameterType;

                        if (parameterType == null)
                            continue;

                        if (propertyValue.GetType() != parameterType)
                        {
                            var querierName = string.Format("{0}Querier", parameterType.Name);

                            var searchIocItems = ObjectContainer.GetRegisterItems().Where(pre => pre.MappedToType.Name == querierName).ToList();

                            object tempFindValue = null;

                            if (searchIocItems.Count > 0)
                            {
                                //先从仓储集合中查找指定类型的仓储。
                                var searchQuerier = searchIocItems.FirstOrDefault();
                                var searchQuerierInstance = ObjectContainer.CreateInstance(searchQuerier.RegisteredType);

                                var methodName = string.Format("Get{0}ById", parameterType.Name);

                                var tempQuerierMethod = searchQuerierInstance.GetType().GetMethod(methodName);

                                if (tempQuerierMethod != null)
                                {
                                    tempFindValue = tempQuerierMethod.Invoke(searchQuerierInstance, new object[] { propertyValue });

                                    if (tempFindValue == null)
                                        continue;

                                    tempMethod.Invoke(domainObj, new object[] { tempFindValue });

                                    break;
                                }
                            }

                            //如果没有找到，再执行后续的代码。
                            var dbContext = (DbContext)context.GetType().GetProperty("Context").GetValue(context, null);

                            if (dbContext == null)
                                throw new ApplicationException("没有找到持久化上下文");
                            
                            //暂无好的方案
                            //tempFindValue = dbContext.Set(parameterType).Find(propertyValue);
                            if (tempFindValue == null)
                                continue;

                            tempMethod.Invoke(domainObj, new object[] { tempFindValue });
                            break;
                        }

                        tempMethod.Invoke(domainObj, new object[] { propertyValue });

                        break;
                    }
                }
            }

            return true;
        }
    }

    public static partial class CustomExtensions
    {
        public static IQueryable Query(this DbContext context, string entityName) =>
            context.Query(context.Model.FindEntityType(entityName).ClrType);

        static readonly MethodInfo SetMethod = typeof(DbContext).GetMethod(nameof(DbContext.Set));

        public static IQueryable Query(this DbContext context, Type entityType) =>
            (IQueryable)SetMethod.MakeGenericMethod(entityType).Invoke(context, null);
    }
}
