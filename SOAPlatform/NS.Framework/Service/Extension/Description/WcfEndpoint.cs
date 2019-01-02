/****************************************************************************************************************
*                                                                                                               *
* Copyright (C) 2011 5173.com                                                                                   *
* This project may be copied only under the terms of the Apache License 2.0.                                    *
* Please visit the project Home Page http://wcfextension.codeplex.com/ for more detail.                         *
*                                                                                                               *
****************************************************************************************************************/

namespace WcfExtension
{
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Text;

    [DataContract(Namespace = "WcfExtension")]
    public class WcfEndpoint
    {
        [DataMember]
        public string ServiceContractType { get; set; }

        [DataMember]
        public string EndpointBindingXml { get; set; }

        [DataMember]
        public string EndpointBindingType { get; set; }

        [DataMember]
        public string EndpointName { get; set; }

        [DataMember]
        public string EndpointProtocol { get; set; }

        [DataMember]
        public int EndpointPort { get; set; }

        [DataMember]
        public string EndpointBehaviorXml { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            this.GetType().GetProperties().ToList().ForEach(p =>
            {
                var o = p.GetValue(this, null);
                sb.AppendLine(p.Name + ": " + o);
            });

            return sb.ToString();
        }
    }
}
