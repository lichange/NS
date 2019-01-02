using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NS.Framework.IOC
{
    [Serializable]
    [System.Xml.Serialization.XmlRoot("Object")]
    public class ObjectConfiguration
    {
        [System.Xml.Serialization.XmlAttribute("Name")]
        public string Name
        {
            get;
            set;
        }

        [System.Xml.Serialization.XmlAttribute("Value")]
        public string Value
        {
            get;
            set;
        }

        [System.Xml.Serialization.XmlAttribute("LifeCycle")]
        public string LifeCycle
        {
            get;
            set;
        }
    }
}
