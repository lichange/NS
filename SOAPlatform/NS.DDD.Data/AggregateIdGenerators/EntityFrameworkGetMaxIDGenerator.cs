using Microsoft.EntityFrameworkCore;
using NS.DDD.Core;
using NS.DDD.Core.AggregateIdGenerators;
using NS.Framework.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NS.DDD.Data.AggregateIdGenerators
{
    /// <summary>
    /// 根据现有的最大主键+1的方式来生成主键的-生成器
    /// </summary>
    [Export(typeof(IGetMaxIDGenerator))]
    public class EntityFrameworkGetMaxIDGenerator : IGetMaxIDGenerator
    {
        /// <summary>
        /// 上下文
        /// </summary>
        public object Context
        {
            get;
            set;
        }

        public object Generate<T>() where T : class,IAggregateRoot
        {
            //调用仓促接口= type.Name

            if (!(typeof(T).GetInterfaces().Where(pre => pre.Name == typeof(IAggregateRoot).Name || pre.Name == typeof(IEntity).Name).Count() > 0))
                throw new RepositoryException(string.Format("目标类型'{0}'不支持自动生成主键！", typeof(T).FullName));

            if (Context == null)
                throw new RepositoryException(string.Format("数据持久化上下文不能为空！调用目标类型：{0}", this.GetType().FullName));

            if (!(Context is DbContext))
                throw new RepositoryException("数据持久化上下文类型不匹配");

            var tempValue = ((DbContext)this.Context).Set<T>().Max(pre => pre.GetAggregateID());

            if (tempValue == null || string.IsNullOrEmpty(tempValue.ToString().Trim()))
                return 1;

            if (tempValue == null && tempValue is int)
                return 1;

            try
            {
                int i = Convert.ToInt32(tempValue);
            }
            catch (Exception ex)
            {
                throw new RepositoryException(string.Format("目标类型'{0}'不支持按照最大ID+1的方式生成主键！", typeof(T).FullName));
            }

            tempValue = Convert.ToInt32(tempValue) + 1;

            return null;
        }
    }
}
