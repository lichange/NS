using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NS.Component.Utility
{
    public class CodeCompileContext
    {
        #region Reference Assemblys

        private IList<string> tempReferenceAssemblys = new List<string>();
        public IList<string> ReferenceAssemblys
        {
            get
            {
                return tempReferenceAssemblys;
            }
        }

        #endregion

        #region AssemblyName

        public string AssemblyName
        {
            get;
            set;
        }

        #endregion

        #region NameSpace

        public string NameSpace
        {
            get;
            set;
        }

        #endregion

        #region

        public string CodeDomTemplate
        {
            get;
            set;
        }

        public string ExecuteCode
        {
            get;
            set;
        }

        public string ClassName
        {
            get;
            set;
        }

        public string MethodName
        {
            get;
            set;
        }

        private IList<object> tempParameters = new List<object>();
        public IList<object> parameters
        {
            get
            {
                return tempParameters;
            }
        }
        #endregion
    }
}
