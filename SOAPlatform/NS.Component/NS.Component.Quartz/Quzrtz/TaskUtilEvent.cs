using NS.Framework.Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NS.Component.Quartz
{
    public class AddTaskUtilEvent
    {
        public TaskUtil taskUtil
        {
            get;
            set;
        }
    }
    public class AddTaskUtilsEvent
    {
        public List<TaskUtil> listTaskUtil
        {
            get;
            set;
        }
    }

    public class DeleteTaskUtilEvent
    {
        public string JobKey
        {
            get;
            set;
        }
    }

    public class DeleteTaskUtilsEvent
    {
        public List<string> listJobKey
        {
            get;
            set;
        }
    }

    public class UpdateTaskUtilEvent
    {
        public TaskUtil taskUtil
        {
            get;
            set;
        }
    }

    public class UpdateTaskUtilsEvent
    {
        public List<TaskUtil> listTaskUtil
        {
            get;
            set;
        }
    }

    public class PauseJobEvent
    {
        public string JobKey
        {
            get;
            set;
        }
    }

    public class PauseJobsEvent
    {
        public List<string> listJobKey
        {
            get;
            set;
        }
    }
    public class PauseAllJobEvent
    {
    }

    public class ResumeJobEvent
    {
        public string JobKey
        {
            get;
            set;
        }
    }
    public class ResumeJobsEvent
    {
        public List<string> listJobKey
        {
            get;
            set;
        }
    }
    public class ResumeAllJobEvent
    {
    }

   
}
