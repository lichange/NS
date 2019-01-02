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
    public class WcfLogSetting
    {
        [DataMember]
        public bool Enabled { get; set; }

        [DataMember]
        public InvokeInfoSetting InvokeInfoSetting { get; set; }

        [DataMember]
        public MessageInfoSetting MessageInfoSetting { get; set; }

        [DataMember]
        public StartInfoSetting StartInfoSetting { get; set; }

        [DataMember]
        public ExceptionInfoSetting ExceptionInfoSetting { get; set; }
    }
}
