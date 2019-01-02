using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NS.Framework.IOC
{
    [Serializable]
    [System.Xml.Serialization.XmlRoot("ObjectsConfiguration")]
    public class ObjectConfigurationCollection
    {
        private List<ObjectConfiguration> objects = new List<ObjectConfiguration>();
        [System.Xml.Serialization.XmlArray("Objects")]
        public List<ObjectConfiguration> Objects
        {
            get
            {
                return objects;
            }
        }
    }
}
