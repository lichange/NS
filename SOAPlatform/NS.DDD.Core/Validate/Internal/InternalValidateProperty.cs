using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NS.DDD.Core.Internal.Validation
{
    public class InternalValidateProperty
    {
        private System.Reflection.PropertyInfo _propertyInfo;
        private InternalValidateEntity _instance;
        public InternalValidateProperty(System.Reflection.PropertyInfo propertyInfo,InternalValidateEntity instance)
        {
            this._propertyInfo = propertyInfo;
            this._instance = instance;
        }

        public string PropertyName
        {
            get
            {
                return this._propertyInfo == null ? string.Empty : this._propertyInfo.Name;
            }
        }

        public object PropertyValue
        {
            get
            {
                return this._propertyInfo == null ? null : this._propertyInfo.GetValue(this._instance.EntityValue,null);
            }
        }
    }
}
