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
    public class WcfClientEndpoint : WcfEndpoint
    {
        [DataMember]
        public string EndpointAddress { get; set; }

        public string ServerFarmName { get; set; }

        public string ServiceType { get; set; }
    }
}
