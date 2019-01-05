using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NS.Framework.Attributes;
using NS.Framework.Quartz;
using NS.Framework.Bus;

namespace NS.Component.Quartz
{
    [Export(typeof(IQuartzProvider))]
    public class QuartzProvider : IQuartzProvider
    {
        public QuartzProvider()
        {
            QuartzHelper quartzHelper = new QuartzHelper();
        }

        #region Methods

        #region add job
        /// <summary>
        /// Add job
        /// </summary>
        /// <param name="task">task</param>
        public bool AddJob(TaskUtil task)
        {
            EventBus.Publish<AddTaskUtilEvent>(new AddTaskUtilEvent()
            {
                taskUtil = task
            });
            return true;
        }

        /// <summary>
        /// Batch add task
        /// </summary>
        /// <param name="listTask">list task</param>
        public bool AddJob(List<TaskUtil> listTask)
        {
            EventBus.Publish<AddTaskUtilsEvent>(new AddTaskUtilsEvent()
            {
                listTaskUtil = listTask
            });
            return true;
        }

        #endregion

        #region delete Job

        /// <summary>
        /// Delete an existing task
        /// </summary>
        /// <param name="JobKey"></param>
        public bool DeleteJob(string jobKey)
        {
            EventBus.Publish<DeleteTaskUtilEvent>(new DeleteTaskUtilEvent()
            {
                JobKey = jobKey
            });
            return true;
        }

        /// <summary>
        /// Delete existing tasks
        /// </summary>
        /// <param name="JobKey"></param>
        public bool DeleteJob(List<string> listJobKey)
        {
            EventBus.Publish<DeleteTaskUtilsEvent>(new DeleteTaskUtilsEvent()
            {
                listJobKey = listJobKey
            });
            return true;
        }

        #endregion

        #region update job

        /// <summary>
        /// Update existing tasks
        /// </summary>
        /// <param name="task">task</param>
        public bool UpdateJob(TaskUtil task)
        {
            EventBus.Publish<UpdateTaskUtilEvent>(new UpdateTaskUtilEvent()
            {
                taskUtil = task
            });
            return true;
        }

        /// <summary>
        /// Update existing tasks
        /// </summary>
        /// <param name="listTask">list task</param>
        public bool UpdateJob(List<TaskUtil> listTask)
        {
            EventBus.Publish<UpdateTaskUtilsEvent>(new UpdateTaskUtilsEvent()
            {
                listTaskUtil = listTask
            });
            return true;
        }
        #endregion

        #region Pause job

        /// <summary>
        /// Pause task
        /// </summary>
        /// <param name="JobKey">task key</param>
        public bool PauseJob(string jobKey)
        {
            EventBus.Publish<PauseJobEvent>(new PauseJobEvent()
            {
                JobKey = jobKey
            });
            return true;
        }

        /// <summary>
        /// Pause task
        /// </summary>
        /// <param name="JobKey">task key</param>
        public bool PauseJob(List<string> listJobKey)
        {
            EventBus.Publish<PauseJobsEvent>(new PauseJobsEvent()
            {
                listJobKey = listJobKey
            });
            return true;
        }

        /// <summary>
        /// Pause task
        /// </summary>
        /// <param name="JobKey">task key</param>
        public bool PauseJob()
        {
            EventBus.Publish<PauseAllJobEvent>(new PauseAllJobEvent());
            return true;
        }

        #endregion

        #region resume job

        /// <summary>
        /// Resume running suspended task
        /// </summary>
        /// <param name="JobKey">task key</param>
        public bool ResumeJob(string jobKey)
        {
            EventBus.Publish<ResumeJobEvent>(new ResumeJobEvent()
            {
                JobKey = jobKey
            });
            return true;
        }

        /// <summary>
        /// Resume running suspended task
        /// </summary>
        /// <param name="JobKey">task key</param>
        public bool ResumeJob(List<string> listJobKey)
        {
            EventBus.Publish<ResumeJobsEvent>(new ResumeJobsEvent()
            {
                listJobKey = listJobKey
            });
            return true;
        }

        /// <summary>
        /// Resume running suspended task
        /// </summary>
        /// <param name="JobKey">task key</param>
        public bool ResumeJob()
        {
            EventBus.Publish<ResumeAllJobEvent>(new ResumeAllJobEvent());
            return true;
        }

        #endregion

        /// <summary>
        /// Acquisition tasks which will run in the next cycle
        /// </summary>
        /// <param name="CronExpressionString">Cron Expressions</param>
        /// <param name="numTimes">Running times</param>
        /// <returns>Run time segment</returns>
        public DateTime GetNextRunTime(string CronExpressionString)
        {
            return QuartzHelper.GetNextRunTime(CronExpressionString);
        }

        #endregion
    }
}
