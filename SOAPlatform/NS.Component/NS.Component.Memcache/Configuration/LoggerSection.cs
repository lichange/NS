using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using NS.Component.Memcache.Caching.Configuration;
using System.ComponentModel;

namespace NS.Component.Memcache.Caching.Configuration
{
	public class LoggerSection : ConfigurationSection
	{
		[ConfigurationProperty("factory", IsRequired = true)]
		[InterfaceValidator(typeof(ILogFactory)), TypeConverter(typeof(TypeNameConverter))]
		public Type LogFactory
		{
			get { return (Type)base["factory"]; }
			set { base["factory"] = value; }
		}
	}
}
