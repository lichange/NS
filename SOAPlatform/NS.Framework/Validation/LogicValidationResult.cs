using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NS.Framework.Validation
{
    /// <summary>
    /// 验证结果
    /// </summary>
    [Serializable]
    public class LogicValidationResult
    {
        /// <summary>
        /// 错误提示信息
        /// </summary>
        public string Message
        {
            get;
            set;
        }

        /// <summary>
        /// 验证结果
        /// </summary>
        public bool IsSuccess
        {
            get;
            set;
        }
    }
}
