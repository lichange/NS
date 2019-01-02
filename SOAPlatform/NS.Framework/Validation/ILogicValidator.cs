using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NS.Framework.Validation;

namespace NS.DDD.Core.Validation
{
    /// <summary>
    /// 业务逻辑验证 --该验证器负责验证对象上的特定的业务逻辑
    /// </summary>
    public interface ILogicValidator
    {
        /// <summary>
        /// 待验证的对象
        /// </summary>
        /// <param name="parameter">对象信息</param>
        /// <returns></returns>
        LogicValidationResult Validate(object parameter);
    }
}
