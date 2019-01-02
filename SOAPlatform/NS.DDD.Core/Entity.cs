using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NS.DDD.Core
{
    public abstract class Entity : IEntity
    {
        #region Protected Fields
        protected object id;
        #endregion

        #region Public Methods
        /// <summary>
        /// 确定指定的Object是否等于当前的Object。
        /// </summary>
        /// <param name="obj">要与当前对象进行比较的对象。</param>
        /// <returns>如果指定的Object与当前Object相等，则返回true，否则返回false。</returns>
        /// <remarks>有关此函数的更多信息，请参见：http://msdn.microsoft.com/zh-cn/library/system.object.equals。
        /// </remarks>
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            Entity ar = obj as Entity;
            if (ar == null)
                return false;
            return this.id.Equals(ar.GetBaseAggregateID());
        }
        /// <summary>
        /// 用作特定类型的哈希函数。
        /// </summary>
        /// <returns>当前Object的哈希代码。</returns>
        /// <remarks>有关此函数的更多信息，请参见：http://msdn.microsoft.com/zh-cn/library/system.object.gethashcode。
        /// </remarks>
        public override int GetHashCode()
        {
            return this.id == null ? Guid.NewGuid().GetHashCode() : this.id.GetHashCode();
        }
        #endregion

        /// <summary>
        /// Equal operator
        /// </summary>
        public static bool operator ==
            (Entity x, Entity y)
        {
            return Equals(x, y);
        }

        /// <summary>
        /// Not equal operator
        /// </summary>
        public static bool operator !=
            (Entity x, Entity y)
        {
            return !(x == y);
        }

        #region IEntity Members
        /// <summary>
        /// 获取当前领域实体类的全局唯一标识。
        /// </summary>
        public virtual object GetBaseAggregateID()
        {
            return id;
        }

        #endregion
    }
}
