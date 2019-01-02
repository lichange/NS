using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//using Microsoft.Practices.Unity;
//using NS.Framework.Exceptions;
//using System.ServiceModel;
//using System.ServiceModel.Web;
//using System.ServiceModel.Description;
//using NS.Framework.Utility.Collections;
//using System.ServiceModel.Channels;
//using System.Reflection;

//using NS.Framework.Service.Implement;

namespace NS.Framework.Service
{
    /// <summary>
    /// WCF辅助类
    /// </summary>
    //public class WCFHelper
    //{
    //    #region 单例

    //    private static readonly object flag = new object();
    //    private static WCFHelper instance;

    //    public static WCFHelper Instance
    //    {
    //        get
    //        {
    //            if (instance == null)
    //            {
    //                lock (flag)
    //                {
    //                    if (instance == null)
    //                    {
    //                        instance = new WCFHelper();
    //                    }
    //                }
    //            }
    //            return instance;
    //        }
    //    }

    //    #endregion

    //    #region 服务发现
    //    public IList<ServiceMetaData> GetServiceMetaDatas(string filePath)
    //    {
    //        IServiceDiscovery discovery = new ServiceDiscovery();

    //        return discovery.GetServiceMetaDatas(filePath);
    //    }

    //    public IList<ServiceMetaData> GetServiceMetaDatas(string directoryPath, string filter, params string[] ignoredfileNames)
    //    {
    //        IServiceDiscovery discovery = new ServiceDiscovery();

    //        return discovery.GetServiceMetaDatas(directoryPath, filter, ignoredfileNames);
    //    }

    //    #endregion

    //    #region 服务发布

    //    /// <summary>
    //    /// 根据服务元数据获取服务发布后的URL地址
    //    /// </summary>
    //    /// <param name="metaData"></param>
    //    /// <returns></returns>
    //    public string GetServiceUrl(ServiceMetaData metaData)
    //    {
    //        System.ServiceModel.Channels.Binding tempBinding = ServicePublishHelper.PublishHelper.GetBinding(NS.Framework.Config.PlatformConfig.ServerConfig.WCFSetting.Binding);
    //        var baseUri = string.Format(ServicePublishHelper.PublishHelper.GetServiceUrlFormat(tempBinding), GetWCFServiceHostName(), this.GetPort(metaData), metaData.ServiceContract);
    //        return baseUri;
    //    }

    //    /// <summary>
    //    /// 获取WCF服务宿主
    //    /// </summary>
    //    /// <returns></returns>
    //    private string GetWCFServiceHostName()
    //    {
    //        if (string.IsNullOrEmpty(NS.Framework.Config.PlatformConfig.ServerConfig.WCFSetting.ServiceName))
    //            return ServicePublishHelper.PublishHelper.GetHostName();
    //        else
    //            return NS.Framework.Config.PlatformConfig.ServerConfig.WCFSetting.ServiceName;
    //    }

    //    /// <summary>
    //    /// 获取端口号
    //    /// </summary>
    //    /// <param name="description"></param>
    //    /// <returns></returns>
    //    private string GetPort(ServiceMetaData description)
    //    {
    //        return (description.Port == null ? (NS.Framework.Config.PlatformConfig.ServerConfig.WCFSetting.Port).ToString() : description.Port);
    //    }

    //    /// <summary>
    //    /// 根据服务元数据集合获取服务发布后的URL地址集合
    //    /// </summary>
    //    /// <param name="metaData"></param>
    //    /// <returns></returns>
    //    public IList<string> GetServiceUrls(IList<ServiceMetaData> metaDatas)
    //    {
    //        var tempList = new List<string>();

    //        if (metaDatas == null || metaDatas.Count == 0)
    //            return tempList;

    //        foreach (var metaData in metaDatas)
    //        {
    //            tempList.Add(GetServiceUrl(metaData));
    //        }

    //        return tempList;
    //    }

    //    #endregion

    //    #region 服务发布1
        
    //    public bool Publish(params ServiceMetaData[] serviceDescriptions)
    //    {
    //        bool flag = false;
    //        if (serviceDescriptions == null)
    //            return flag;

    //        var platform= NS.Framework.Service.WCFEngine.Instance.MakeSureEngineIsStarted();

    //        flag = platform.GetServicePublisher().Publish(serviceDescriptions);

    //        return flag;
    //    }

    //    #endregion
    //}
}
