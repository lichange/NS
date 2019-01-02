/****************************************************************************************************************
*                                                                                                               *
* Copyright (C) 2011 5173.com                                                                                   *
* This project may be copied only under the terms of the Apache License 2.0.                                    *
* Please visit the project Home Page http://wcfextension.codeplex.com/ for more detail.                         *
*                                                                                                               *
****************************************************************************************************************/

namespace WcfExtension
{
    using System.Runtime.Serialization;

    [DataContract(Namespace = "WcfExtension")]
    [KnownType(typeof(WcfClientSetting))]
    [KnownType(typeof(WcfServerSetting))]
    public abstract class WcfSetting
    {
        [DataMember]
        public WcfLogSetting WcfLogSetting { get; set; }

        [DataMember]
        public WcfSecuritySetting WcfSecuritySetting { get; set; }
    }
}
