using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NS.Framework.IOC;
using NS.Framework.Bus;

namespace NS.Framework.Quartz
{
    /// <summary>
    /// Manager that provides a timed task function to an application system
    /// </summary>
    public class QuartzManager
    {
        #region Private Fields
        private readonly IQuartzProvider quartzProvider;
        private static readonly QuartzManager instance = new QuartzManager();
        #endregion

        #region Ctor
        static QuartzManager() { }

        private QuartzManager()
        {
            quartzProvider = ObjectContainer.CreateInstance<IQuartzProvider>();
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// get<c>CacheManager</c>（Singleton）
        /// </summary>
        public static QuartzManager Instance
        {
            get { return instance; }
        }
        #endregion

        #region IQuartzProvider Members

        #region add

        /// <summary>
        /// Add job
        /// </summary>
        /// <param name="task">task</param>
        public bool AddJob(TaskUtil task)
        {
            return quartzProvider.AddJob(task);
        }

        /// <summary>
        /// Batch add task
        /// </summary>
        /// <param name="listTask">list task</param>
        public bool AddJob(List<TaskUtil> listTask)
        {
            return quartzProvider.AddJob(listTask);
        }
        #endregion

        #region delete

        /// <summary>
        /// Delete an existing task
        /// </summary>
        /// <param name="JobKey"></param>
        public bool DeleteJob(string jobKey)
        {
            return quartzProvider.DeleteJob(jobKey);
        }

        /// <summary>
        /// Delete existing tasks
        /// </summary>
        /// <param name="JobKey"></param>
        public bool DeleteJob(List<string> listJobKey)
        {
            return quartzProvider.DeleteJob(listJobKey);
        }
        #endregion

        #region update
        /// <summary>
        /// Update existing tasks
        /// </summary>
        /// <param name="task">task</param>
        public bool UpdateJob(TaskUtil task)
        {
            return quartzProvider.UpdateJob(task);
        }

        /// <summary>
        /// Update existing tasks
        /// </summary>
        /// <param name="listTask">list task</param>
        public bool UpdateJob(List<TaskUtil> listTask)
        {
            return quartzProvider.UpdateJob(listTask);
        }
        #endregion

        #region Pause job

        /// <summary>
        /// Pause task
        /// </summary>
        /// <param name="JobKey">task key</param>
        public bool PauseJob(string jobKey)
        {
            return quartzProvider.PauseJob(jobKey);
        }

        /// <summary>
        /// Pause task
        /// </summary>
        /// <param name="JobKey">task key</param>
        public bool PauseJob(List<string> listJobKey)
        {
            return quartzProvider.PauseJob(listJobKey);
        }

        /// <summary>
        /// Pause all task
        /// </summary>
        /// <param name="JobKey">task key</param>
        public bool PauseJob()
        {
            return quartzProvider.PauseJob();
        }

        #endregion

        #region resume job

        /// <summary>
        /// Resume running suspended task
        /// </summary>
        /// <param name="JobKey">task key</param>
        public bool ResumeJob(string jobKey)
        {
            return quartzProvider.ResumeJob(jobKey);
        }

        /// <summary>
        /// Resume running suspended task
        /// </summary>
        /// <param name="JobKey">task key</param>
        public bool ResumeJob(List<string> listJobKey)
        {
            return quartzProvider.ResumeJob(listJobKey);
        }

        /// <summary>
        /// Resume all running suspended task
        /// </summary>
        /// <param name="JobKey">task key</param>
        public bool ResumeJob()
        {
            return quartzProvider.ResumeJob();
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
            return quartzProvider.GetNextRunTime(CronExpressionString);
        }
        #endregion

    }
}
