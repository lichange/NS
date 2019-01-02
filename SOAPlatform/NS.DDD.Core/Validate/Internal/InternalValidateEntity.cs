using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NS.DDD.Core.Internal.Validation
{
    public class InternalValidateEntity
    {
        private IAggregateRoot _instance;
        private IList<System.Reflection.PropertyInfo> propertyInfos = new List<PropertyInfo>();

        public InternalValidateEntity(IAggregateRoot instance)
        {
            this._instance = instance;
        }

        public Type EntityType
        {
            get
            {
                return this._instance == null ? null : _instance.GetType();
            }
        }

        public object EntityValue
        {
            get
            {
                return this._instance;
            }
        }

        public IList<System.Reflection.PropertyInfo> NavigationProperties
        {
            get
            {
                return this.propertyInfos;
            }
        }

        public InternalValidateProperty GetProperty(string propertyName)
        {
            return this._instance == null ? null : new InternalValidateProperty(this._instance.GetType().GetProperty(propertyName),this);
        }
    }
}
