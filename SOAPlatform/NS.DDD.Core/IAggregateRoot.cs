using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NS.DDD.Core
{
    /// <summary>
    /// 表示继承于该接口的类型是聚合根类型。
    /// </summary>
    public interface IAggregateRoot
    {
        #region Properties
        /// <summary>
        /// 获取当前领域实体类的全局唯一标识。
        /// </summary>
        object GetAggregateID();

        #endregion

        #region
        //IList<Column> Columns
        //{
        //    get;
        //}


        //Column this[string columnName]
        //{
        //    get;
        //}

        //object GetValue(string columnName);

        //void SetValue(string columnName, object value);

        Type GetRealType();

        #endregion
    }
}
