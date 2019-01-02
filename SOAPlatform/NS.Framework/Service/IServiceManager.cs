//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.ServiceModel;

//namespace NS.Framework.Service
//{
//    /// <summary>
//    /// 服务管理-负责管理当前平台内的所有的服务生命周期的管理。
//    /// </summary>
//    public interface IServiceManager
//    {
//        /// <summary>
//        /// 初始化-主要是加载默认服务
//        /// </summary>
//        void InitializationServiceManager();

//        /// <summary>
//        /// 将指定名称的服务状态打开，提供给服务调用者
//        /// </summary>
//        /// <param name="serviceName">服务实现名称</param>
//        /// <returns>返回服务是否启动成功</returns>
//        bool StartService(string serviceName);
//        /// <summary>
//        /// 将指定名称的服务状态打开，提供给服务调用者
//        /// </summary>
//        /// <param name="implementType">服务类型</param>
//        /// <returns>返回服务是否启动成功</returns>
//        bool StartService(Type implementType);

//        /// <summary>
//        /// 将指定名称的服务的状态设置为关闭状态，停止对外界提供服务。
//        /// </summary>
//        /// <param name="serviceName">服务实现名称</param>
//        /// <returns>返回服务是否关闭</returns>
//        bool CloseService(string serviceName);
//        /// <summary>
//        /// 将指定名称的服务的状态设置为关闭状态，停止对外界提供服务。
//        /// </summary>
//        /// <param name="implementType">服务实现类型</param>
//        /// <returns>返回服务是否关闭</returns>
//        bool CloseService(Type implementType);

//        /// <summary>
//        /// 更新宿主信息，将原来的宿主信息进行更新，更新操作执行了关闭旧宿主，打开新宿主。
//        /// </summary>
//        /// <param name="implementType">服务实现类型</param>
//        /// <param name="newImplementType">新宿主</param>
//        /// <returns></returns>
//        bool UpdateService(Type implementType, ServiceMetaData newImplementType);
//        bool UpdateService(string serviceName, ServiceMetaData newServiceName);

//        ///// <summary>
//        ///// 将新的宿主添加到当前的服务宿主管理集合中
//        ///// </summary>
//        ///// <param name="implementType">服务实现类型</param>
//        ///// <param name="addServiceHost">服务宿主</param>
//        ///// <returns>返回操作结果</returns>
//        //bool AddService(Type implementType); 

//        /// <summary>
//        /// 将新的宿主添加到当前的服务宿主管理集合中
//        /// </summary>
//        /// <param name="metaData">服务元数据</param>
//        /// <returns>返回操作结果</returns>
//        bool AddService(ServiceMetaData metaData);

//        /// <summary>
//        /// 获取服务的元数据信息
//        /// </summary>
//        /// <param name="serviceName">服务名称</param>
//        /// <returns></returns>
//        ServiceMetaData GetServiceMetaData(string serviceName);

//        /// <summary>
//        /// 获取服务的元数据信息
//        /// </summary>
//        /// <param name="serviceNames">服务名称集合</param>
//        /// <returns></returns>
//        IList<ServiceMetaData> GetServiceMetaDatas(params string[] serviceName);

//        /// <summary>
//        /// 获取服务的元数据信息
//        /// </summary>
//        /// <param name="serviceName">服务名称</param>
//        /// <returns></returns>
//        ServiceMetaData GetServiceMetaData(Type implementType);

//        /// <summary>
//        /// 获取服务的元数据信息
//        /// </summary>
//        /// <param name="serviceNames">服务名称集合</param>
//        /// <returns></returns>
//        IList<ServiceMetaData> GetServiceMetaDatas(params Type[] implementTypes);

//        /// <summary>
//        /// 获取所有服务的元数据信息
//        /// </summary>
//        /// <returns></returns>
//        IList<ServiceMetaData> GetAllServiceMetaDatas();
//    }
//}
