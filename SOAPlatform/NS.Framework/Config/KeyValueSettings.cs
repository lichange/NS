using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace NS.Framework.Config
{
    /// <summary>
    /// 服务配置信息-数据库服务器配置
    /// </summary>
    [Serializable]
    public class KeyValueSettings
    {
        private List<KeyValueItem> keyValueItems = new List<KeyValueItem>();

        /// <summary>
        /// 当前IOC的配置项信息
        /// </summary>
        [System.Xml.Serialization.XmlArray("KeyValueItems")]
        public List<KeyValueItem> KeyValueItems
        {
            get
            {
                return this.keyValueItems;
            }
            set
            {
                this.keyValueItems = value;
            }
        }

        /// <summary>
        /// 根据关键字获取key对应的配置项
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public KeyValueItem this[string key]
        {
            get
            {
                var tempItem= this.KeyValueItems.Where(pre => pre.Key.ToLower() == key.ToLower()).FirstOrDefault();
                return tempItem == null ? new KeyValueItem() : tempItem;
            }
        }
    }

    public class KeyValueItem
    {
        private string key = "";
        /// <summary>
        /// 配置项名称
        /// </summary>
        [System.Xml.Serialization.XmlAttribute("Key")]
        public string Key
        {
            get
            {
                return this.key;
            }
            set
            {
                this.key = value;
            }
        }

        private string value = "";
        /// <summary>
        /// 配置项的值
        /// </summary>
        [System.Xml.Serialization.XmlAttribute("Value")]
        public string Value
        {
            get
            {
                return this.value;
            }
            set
            {
                this.value = value;
            }
        }
    }
}
