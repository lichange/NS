using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NS.Framework.Quartz
{
    /// <summary>
    /// Indicates that the type of implementation of the interface is a type of timing task that can be provided for the application
    /// </summary>
    public interface IQuartzProvider
    {
        #region Methods
        /// <summary>
        /// Add job
        /// </summary>
        /// <param name="task">task</param>
        bool AddJob(TaskUtil task);

        /// <summary>
        /// Batch add task
        /// </summary>
        /// <param name="listTask">list task</param>
        bool AddJob(List<TaskUtil> listTask);

        /// <summary>
        /// Delete an existing task
        /// </summary>
        /// <param name="JobKey"></param>
        bool DeleteJob(string jobKey);

        /// <summary>
        /// Delete existing tasks
        /// </summary>
        /// <param name="JobKey"></param>
        bool DeleteJob(List<string> listJobKey);

        /// <summary>
        /// Update existing tasks
        /// </summary>
        /// <param name="task">task</param>
        bool UpdateJob(TaskUtil task);

        /// <summary>
        /// Update existing tasks
        /// </summary>
        /// <param name="listTask">list task</param>
        bool UpdateJob(List<TaskUtil> listTask);

        /// <summary>
        /// Pause task
        /// </summary>
        /// <param name="JobKey">task key</param>
        bool PauseJob(string jobKey);

        /// <summary>
        /// Pause task
        /// </summary>
        /// <param name="JobKey">task key</param>
        bool PauseJob(List<string> listJobKey);

        /// <summary>
        /// Pause all task
        /// </summary>
        /// <param name="JobKey">task key</param>
        bool PauseJob();

        /// <summary>
        /// Resume running suspended task
        /// </summary>
        /// <param name="JobKey">task key</param>
        bool ResumeJob(string jobKey);

        /// <summary>
        /// Resume running suspended task
        /// </summary>
        /// <param name="JobKey">task key</param>
        bool ResumeJob(List<string> listJobKey);

        /// <summary>
        /// Resume all running suspended task
        /// </summary>
        /// <param name="JobKey">task key</param>
        bool ResumeJob();

        /// <summary>
        /// Acquisition tasks which will run in the next cycle
        /// </summary>
        /// <param name="CronExpressionString">Cron Expressions</param>
        /// <param name="numTimes">Running times</param>
        /// <returns>Run time segment</returns>
        DateTime GetNextRunTime(string CronExpressionString);

        #endregion
    }
}
