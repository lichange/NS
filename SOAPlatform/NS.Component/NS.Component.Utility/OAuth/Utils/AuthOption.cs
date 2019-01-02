using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NS.Component.Utility.OAuth
{
    /// <summary>
    /// OAuth通用参数类
    /// </summary>
    public class AuthOption
    {
        #region //属性
        public String ClientId
        {
            get;
            set;
        }
        public String ClientSecret
        {
            get;
            set;
        }

        public String ApiUrlBase
        {
            get;
            set;
        }
        public String AuthorizeUrl
        {
            get;
            set;
        }
        public String AccessTokenUrl
        {
            get;
            set;
        }
        public String CallbackUrl
        {
            get;
            set;
        }

        public String Display
        {
            get;
            set;
        }
        public String State
        {
            get;
            set;
        }
        public String Scope
        {
            get;
            set;
        }

        public IDictionary<String, String> Urls
        {
            get;
            set;
        }
        #endregion

        #region //构造方法
        public AuthOption()
        {
            this.Urls = new Dictionary<String, String>();
            this.Display = "default ";
        }
        #endregion

    }
}
