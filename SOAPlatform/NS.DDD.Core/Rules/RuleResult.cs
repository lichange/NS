using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NS.DDD.Core
{
    public class RuleResult
    {
        /// <summary>
        /// 执行结果
        /// </summary>
        public bool ExcuteResult
        {
            get;
            set;
        }

        /// <summary>
        /// 如果执行失败，则该属性不为空
        /// </summary>
        public string ErrorMessage
        {
            get;
            set;
        }

        /// <summary>
        /// 出现的异常信息
        /// </summary>
        public Exception Exception
        {
            get;
            set;
        }
    }
}
