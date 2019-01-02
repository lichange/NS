//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace NS.Framework.Service
//{
//    /// <summary>
//    /// 服务动态发布器接口定义
//    /// </summary>
//    public interface IServicePublisher
//    {
//        /// <summary>
//        /// 发布指定的服务-根据服务名称来发布服务。
//        /// </summary>
//        /// <param name="serviceContractName">服务名称</param>
//        /// <returns>返回是否发布成功</returns>
//        bool Publish(string serviceImplementName);

//        /// <summary>
//        /// 批量发布指定的服务-根据服务名称来发布服务。
//        /// </summary>
//        /// <param name="serviceContractNames">服务名称集合</param>
//        /// <returns>返回是否发布成功</returns>
//        bool Publish(params string[] serviceImplementNames);

//        /// <summary>
//        /// 发布指定的服务-根据服务名称来发布服务。
//        /// </summary>
//        /// <param name="serviceContractName">服务名称</param>
//        /// <param name="binding">绑定方式支持以下几种：basehttpbinding，wshttpbinding，tcp绑定</param>
//        /// <param name="address">服务地址-服务发布的URL地址：根据不同的绑定协议，服务的URL地址不同</param>
//        /// <returns>返回是否发布成功</returns>
//        bool Publish(string serviceImplementName, string binding, string address);

//        /// <summary>
//        /// 发布指定的服务-根据服务名称来发布服务。
//        /// </summary>
//        /// <param name="serviceContractName">服务名称</param>
//        /// <param name="binding">绑定方式支持以下几种：basehttpbinding，wshttpbinding，tcp绑定</param>
//        /// <returns>返回是否发布成功</returns>
//        bool Publish(string serviceImplementName, string binding);

//        /// <summary>
//        /// 判定指定服务是否已经发布。
//        /// </summary>
//        /// <param name="serviceContractName">服务名称</param>
//        /// <returns>返回是否该服务已经发布成功</returns>
//        bool IsPublish(string serviceImplementName);

//        /// <summary>
//        /// 根据应用服务的描述信息进行应用服务的发布。
//        /// </summary>
//        /// <param name="serviceDescription">应用服务描述信息</param>
//        /// <returns>返回是否该服务已经发布成功</returns>
//        bool Publish(ServiceMetaData serviceDescription);

//        /// <summary>
//        /// 根据应用服务的描述信息集合进行批量应用服务的发布。
//        /// </summary>
//        /// <param name="serviceDescriptions">应用服务描述信息集合</param>
//        /// <returns>返回是否该服务已经发布成功</returns>
//        bool Publish(params ServiceMetaData[] serviceDescriptions);

//        /// <summary>
//        /// 根据服务契约名称返回服务发布后的详细信息
//        /// </summary>
//        /// <param name="serviceContractName">服务名称</param>
//        /// <returns>如果不存在该服务发布信息，动态发布该服务,并返回发布后的服务信息，否则返回查找后的目标对象</returns>
//        ServiceMetaData GetServiceMetaData(string serviceImplementName);

//        /// <summary>
//        /// 返回当前应用中已发布的所有服务说明信息
//        /// </summary>
//        /// <returns>返回已发布的服务集合</returns>
//        IList<ServiceMetaData> GetServiceMetaDatas();

//        /// <summary>
//        /// 停止指定的服务-根据服务名称来停止服务。
//        /// </summary>
//        /// <param name="serviceContractName">服务名称</param>
//        /// <returns>返回是否停止成功</returns>
//        bool UnPublish(string serviceImplementName);

//        /// <summary>
//        /// 批量发布指定的服务-根据服务名称来停止服务。
//        /// </summary>
//        /// <param name="serviceContractNames">服务名称集合</param>
//        /// <returns>返回是否停止成功</returns>
//        bool UnPublish(params string[] serviceImplementNames);

//        /// <summary>
//        /// 根据应用服务的描述信息进行应用服务的停止。
//        /// </summary>
//        /// <param name="serviceDescription">应用服务描述信息</param>
//        /// <returns>返回是否停止成功</returns>
//        bool UnPublish(ServiceMetaData serviceDescription);

//        /// <summary>
//        /// 根据应用服务的描述信息集合进行批量应用服务的停止。
//        /// </summary>
//        /// <param name="serviceDescriptions">应用服务描述信息集合</param>
//        /// <returns>返回是否停止成功</returns>
//        bool UnPublish(params ServiceMetaData[] serviceDescriptions);
//    }
//}
