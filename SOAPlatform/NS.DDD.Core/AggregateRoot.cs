using NS.Framework.Attributes;
using NS.Framework.Utility.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NS.DDD.Core
{
    /// <summary>
    /// 表示聚合根类型的基类型。
    /// </summary>
    public abstract class AggregateRoot : Entity, IAggregateRoot
    {
        #region IAggregateRoot 成员

        //[NotMapped]
        public virtual object GetAggregateID()
        {
            return base.GetBaseAggregateID();
        }

        //private IList<Column> tempColumns = new List<Column>();
        //public virtual IList<Column> Columns
        //{
        //    get
        //    {
        //        if (tempColumns.Count == 0)
        //        {
        //            var allPropertys = ReflectHelper.GetAllProperties(this, pre => pre.GetCustomAttributes(typeof(PropertyDescriptionAttribute), false).Length > 0);
        //           foreach (var item in allPropertys)
        //           {
        //               tempColumns.Add(new Column(this)
        //               {
        //                   Name = item.Name,
        //                   DataType = this.GetDataType(item.PropertyType),Caption= this.GetCaption(item)
        //               });
        //           }
        //        }

        //        return this.tempColumns;
        //    }
        //}

        //private System.Data.DbType GetDataType(Type type)
        //{
        //    return ReflectHelper.Type2Dbtype(type);
        //}

        //private string GetCaption(System.Reflection.PropertyInfo item)
        //{
        //    PropertyDescriptionAttribute propertyAttribute = item.GetCustomAttributes(typeof(PropertyDescriptionAttribute), false).FirstOrDefault() as PropertyDescriptionAttribute;
        //    return propertyAttribute==null ?  item.Name : propertyAttribute.Description;
        //}

        //public virtual Column this[string columnName]
        //{
        //    get
        //    {
        //        return this.Columns.Where(pre => pre.Name.Equals(columnName, StringComparison.CurrentCulture)).FirstOrDefault();
        //    }
        //}

        //public virtual object GetValue(string columnName)
        //{
        //    var property = this.GetType().GetProperties().Where(pre => pre.Name == columnName).FirstOrDefault();
        //    return property != null ? property.GetValue(this, null) : null;
        //}

        //public virtual void SetValue(string columnName, object value)
        //{
        //    var property = this.GetType().GetProperties().Where(pre => pre.Name == columnName).FirstOrDefault();
        //   if(property != null) 
        //       property.SetValue(this,value,null);
        //}

        public virtual Type GetRealType()
        {
            return this.GetType();
        }

        #endregion
    }

    //public class Column
    //{
    //    public Column()
    //    {
    //    }

    //    private IAggregateRoot obj;

    //    public Column(IAggregateRoot tempObj)
    //    {
    //        obj = tempObj;
    //    }

    //    public string Name
    //    {
    //        get;
    //        set;
    //    }

    //    public System.Data.DbType DataType
    //    {
    //        get;
    //        set;
    //    }

    //    public string Caption
    //    {
    //        get;
    //        set;
    //    }

    //    public object Value
    //    {
    //        get
    //        {
    //            return this.obj.GetValue(this.Name);
    //        }
    //        set
    //        {
    //            this.obj.SetValue(this.Name, value);
    //        }
    //    }
    //}
}
