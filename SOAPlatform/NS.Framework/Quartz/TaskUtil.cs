using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NS.Framework.Quartz
{
    /// <summary>
    /// Task entity
    /// </summary>
    [Serializable]
    public class TaskUtil
    {
        /// <summary>
        /// TaskID
        /// </summary>
        public string TaskID { get; set; }

        /// <summary>
        /// TaskName
        /// </summary>
        public string JobName { get; set; }

        /// <summary>
        /// TaskParam
        /// </summary>
        public string Param { get; set; }

        /// <summary>
        /// CronExpressionString
        /// </summary>
        public string CronExpressionString { get; set; }

        /// <summary>
        /// CronRemark
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Assembly
        /// </summary>
        public string Assembly { get; set; }

        /// <summary>
        /// Class
        /// </summary>
        public string Class { get; set; }

        /// <summary>
        /// Method
        /// </summary>
        public string Method { get; set; }

        /// <summary>
        /// task Status，0：run，1：stop
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// CreatePerson
        /// </summary>
        public string CreatePerson { get; set; }

        /// <summary>
        /// createDatetime
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// CreatePerson
        /// </summary>
        public string UpdatePerson { get; set; }

        /// <summary>
        /// Modification time
        /// </summary>
        public DateTime UpdateDate { get; set; }

        /// <summary>
        /// Task recently run time
        /// </summary>
        public DateTime RecentRunTime { get; set; }

        /// <summary>
        /// Task Last running time
        /// </summary>
        public DateTime NextRunTime { get; set; }

        /// <summary>
        /// Results
        /// </summary>
        public string Result { get; set; }

        /// <summary>
        /// job method
        /// </summary>
        public MethodInfo Execute;

        /// <summary>
        /// listener, When Trigger is triggered, the method is called when the job is executed. True that is rejected, job does not perform behind
        /// </summary>
        public MethodInfo VetoJobExecution { get; set; }

        /// <summary>
        /// listener, Job completion time call
        /// </summary>
        public MethodInfo TriggerComplete { get; set; }
    }

}
