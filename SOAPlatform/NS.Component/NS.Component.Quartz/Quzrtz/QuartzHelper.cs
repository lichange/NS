using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Web;
using System.Collections.Specialized;
using System.Reflection;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Triggers;
using Quartz.Impl.Matchers;
using Quartz.Spi;
using NS.Framework.Bus;
using NS.Framework.Quartz;
using NS.Framework.Global;

namespace NS.Component.Quartz
{
    /// <summary>
    /// Task processing help class
    /// </summary>
    public class QuartzHelper
    {
        #region initialization
        public QuartzHelper()
        {
            //Initial task scheduling.
            InitScheduler();

            //Add event subscription
            EventBus.Subscribe<AddTaskUtilEvent>(AddJob);
            EventBus.Subscribe<AddTaskUtilsEvent>(AddJobs);

            //Delete event subscription
            EventBus.Subscribe<DeleteTaskUtilEvent>(DeleteJob);
            EventBus.Subscribe<DeleteTaskUtilsEvent>(DeleteJobs);

            //Update event subscription
            EventBus.Subscribe<UpdateTaskUtilEvent>(UpdateJob);
            EventBus.Subscribe<UpdateTaskUtilsEvent>(UpdateJobs);

            //Pause event subscription
            EventBus.Subscribe<PauseJobEvent>(PauseJob);
            EventBus.Subscribe<PauseJobsEvent>(PauseJobs);
            EventBus.Subscribe<PauseAllJobEvent>(PauseAllJob);

            //Resume event subscription
            EventBus.Subscribe<ResumeJobEvent>(ResumeJob);
            EventBus.Subscribe<ResumeJobsEvent>(ResumeJobs);
            EventBus.Subscribe<ResumeAllJobEvent>(ResumeAllJob);

        }

        private static object obj = new object();

        /// <summary>
        /// Cache task assembly information
        /// </summary>
        private static Dictionary<string, Assembly> AssemblyDict = new Dictionary<string, Assembly>();

        private static IScheduler scheduler = null;

        /// <summary>
        /// Initial task scheduling
        /// </summary>
        public static void InitScheduler()
        {
            try
            {
                lock (obj)
                {
                    if (scheduler == null)
                    {
                        NameValueCollection properties = new NameValueCollection();

                        properties["quartz.scheduler.instanceName"] = "ExampleDefaultQuartzScheduler";

                        properties["quartz.threadPool.type"] = "Quartz.Simpl.SimpleThreadPool, Quartz";

                        properties["quartz.threadPool.threadCount"] = "10";

                        properties["quartz.threadPool.threadPriority"] = "Normal";

                        properties["quartz.jobStore.misfireThreshold"] = "60000";

                        properties["quartz.jobStore.type"] = "Quartz.Simpl.RAMJobStore, Quartz";

                        ISchedulerFactory factory = new StdSchedulerFactory(properties);

                        scheduler = factory.GetScheduler().Result;
                        scheduler.Clear();

                        //Add global listener
                        scheduler.ListenerManager.AddTriggerListener(new QuartzListener(), GroupMatcher<TriggerKey>.AnyGroup());
                        scheduler.Start();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region interface

        #region add

        /// <summary>
        /// Add Task
        /// <param name="TaskUtilEvent">task information</param>
        /// </summary>
        public static void AddJob(AddTaskUtilEvent TaskUtilEvent)
        {
            TaskManager(TaskUtilEvent.taskUtil);
        }

        /// <summary>
        /// Batch add task.
        /// </summary>
        /// <param name="listTask">listTask</param>
        public static void AddJobs(AddTaskUtilsEvent TaskUtilEvent)
        {
            foreach (TaskUtil taskUtil in TaskUtilEvent.listTaskUtil)
            {
                TaskManager(taskUtil);
            }
        }

        #endregion

        #region delete
        /// <summary>
        /// Delete an existing task
        /// </summary>
        /// <param name="JobKey"></param>
        public static void DeleteJob(DeleteTaskUtilEvent TaskUtilEvent)
        {
            JobKey jk = new JobKey(TaskUtilEvent.JobKey);
            if (scheduler.CheckExists(jk).Result)
            {
                //Task already exists to delete
                scheduler.DeleteJob(jk);
            }
        }

        /// <summary>
        /// Delete existing tasks
        /// </summary>
        /// <param name="JobKey"></param>
        public static void DeleteJobs(DeleteTaskUtilsEvent taskUtilEvent)
        {
            foreach (string item in taskUtilEvent.listJobKey)
            {
                JobKey jk = new JobKey(item);
                if (scheduler.CheckExists(jk).Result)
                {
                    //Task already exists to delete
                    scheduler.DeleteJob(jk).Start();
                }
            }

        }

        #endregion

        #region update
        /// <summary>
        /// Update existing tasks
        /// <param name="taskUtil">task information</param>
        /// </summary>
        public static void UpdateJob(UpdateTaskUtilEvent TaskUtilEvent)
        {
            TaskManager(TaskUtilEvent.taskUtil);
        }
        /// <summary>
        /// Update existing tasks
        /// <param name="taskUtil">task information</param>
        /// </summary>
        public static void UpdateJobs(UpdateTaskUtilsEvent TaskUtilEvent)
        {
            foreach (TaskUtil taskUtil in TaskUtilEvent.listTaskUtil)
            {
                TaskManager(taskUtil);
            }
        }

        #endregion

        #region pause

        /// <summary>
        /// Pause task
        /// </summary>
        /// <param name="JobKey"></param>
        public static void PauseJob(PauseJobEvent JobEvent)
        {
            JobKey jk = new JobKey(JobEvent.JobKey);
            if (scheduler.CheckExists(jk).Result)
            {
                scheduler.PauseJob(jk).Start();
            }
        }

        /// <summary>
        /// Pause task
        /// </summary>
        /// <param name="JobKey"></param>
        public static void PauseJobs(PauseJobsEvent JobEvent)
        {
            foreach (string item in JobEvent.listJobKey)
            {
                JobKey jk = new JobKey(item);
                if (scheduler.CheckExists(jk).Result)
                {
                    scheduler.PauseJob(jk).Start();
                }
            }
        }

        /// <summary>
        /// Pause task
        /// </summary>
        /// <param name="JobKey"></param>
        public static void PauseAllJob(PauseAllJobEvent JobEvent)
        {
            scheduler.PauseAll();
        }
        #endregion

        #region resume
        /// <summary>
        /// Resume running suspended task
        /// </summary>
        /// <param name="JobKey">task key</param>
        public static void ResumeJob(ResumeJobEvent JobEvent)
        {
            JobKey jk = new JobKey(JobEvent.JobKey);
            if (scheduler.CheckExists(jk).Result)
            {
                scheduler.ResumeJob(jk).Start();
            }
        }

        /// <summary>
        /// Resume running suspended task
        /// </summary>
        /// <param name="JobKey">task key</param>
        public static void ResumeJobs(ResumeJobsEvent JobEvent)
        {
            foreach (string item in JobEvent.listJobKey)
            {
                JobKey jk = new JobKey(item);
                if (scheduler.CheckExists(jk).Result)
                {
                    scheduler.ResumeJob(jk).Start();
                }
            }
        }

        /// <summary>
        /// Resume running suspended task
        /// </summary>
        /// <param name="JobKey">task key</param>
        public static void ResumeAllJob(ResumeAllJobEvent JobEvent)
        {
            scheduler.ResumeAll();
        }
        #endregion

        /// <summary>
        /// task Manager
        /// </summary>
        /// <param name="taskUtil">task information</param>
        /// <returns></returns>
        public static bool TaskManager(TaskUtil taskUtil)
        {
            try
            {
                //First delete existing tasks by same ID
                DeleteJob(taskUtil.TaskID);

                //IJobDetail job = new JobDetailImpl(taskUtil.TaskID, GetClassInfo(taskUtil.Assembly, taskUtil.Class));
                IJobDetail job = new JobDetailImpl(taskUtil.TaskID, typeof(TaskJob));
                CronTriggerImpl trigger = new CronTriggerImpl();
                trigger.CronExpressionString = taskUtil.CronExpressionString;
                trigger.Name = taskUtil.TaskID;
                trigger.Description = taskUtil.JobName;
                //Add task execution parameters
                job.JobDataMap.Add("TaskParam", taskUtil);
                scheduler.ScheduleJob(job, trigger);

                if (taskUtil.Status == 1)
                {
                    JobKey jk = new JobKey(taskUtil.TaskID.ToString());
                    scheduler.PauseJob(jk);
                }
            }
            catch
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Acquisition tasks which will run in the next cycle
        /// </summary>
        /// <param name="CronExpressionString">Cron Expressions</param>
        /// <param name="numTimes">Running times</param>
        /// <returns>Run time segment</returns>
        public static DateTime GetNextRunTime(string CronExpressionString)
        {
            DateTime dt = DateTime.MinValue;
            try
            {
                ITrigger trigger = TriggerBuilder.Create().WithCronSchedule(CronExpressionString).Build();
                IReadOnlyList<DateTimeOffset> dates = TriggerUtils.ComputeFireTimes(trigger as IOperableTrigger, null, 1);

                foreach (DateTimeOffset dtf in dates)
                {
                    dt = TimeZoneInfo.ConvertTimeFromUtc(dtf.DateTime, TimeZoneInfo.Local);
                }
                return dt;
            }
            catch (Exception)
            {
                return dt;
            }
        }



        #endregion

        #region Other Methods


        /// <summary>
        /// Delete an existing task
        /// </summary>
        /// <param name="JobKey"></param>
        public static void DeleteJob(string JobKey)
        {
            JobKey jk = new JobKey(JobKey);
            if (scheduler.CheckExists(jk).Result)
            {
                //Task already exists to delete
                scheduler.DeleteJob(jk).Start();
            }
        }

        /// <summary>
        /// Task scheduling
        /// </summary>
        public static void StopSchedule()
        {
            //To determine whether the scheduling has been closed
            if (!scheduler.IsShutdown)
            {
                //Wait for the task to run complete
                scheduler.Shutdown(true);
            }
        }

        /// Attribute and method of obtaining a class  
        /// </summary>  
        /// <param name="assemblyName">assemblyName</param>  
        /// <param name="className">className</param>  
        private static Type GetClassInfo(string assemblyName, string className)
        {
            try
            {
                assemblyName = GetAbsolutePath(assemblyName + ".dll");
                Assembly assembly = null;
                if (!AssemblyDict.TryGetValue(assemblyName, out assembly))
                {
                    assembly = Assembly.LoadFrom(assemblyName);
                    AssemblyDict[assemblyName] = assembly;
                }
                Type type = assembly.GetType(className, true, true);
                return type;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Get the absolute path of the file, for the window program and web program can be used
        /// </summary>
        /// <param name="relativePath">Relative path address</param>
        /// <returns>Absolute path address</returns>
        public static string GetAbsolutePath(string relativePath)
        {
            if (string.IsNullOrEmpty(relativePath))
            {
                throw new ArgumentNullException("para relativePath is not null！");
            }
            relativePath = relativePath.Replace("/", "\\");
            if (relativePath[0] == '\\')
            {
                relativePath = relativePath.Remove(0, 1);
            }
            //Judgment is the Web program or window program
            if (NSHttpContext.Current != null)
            {
                return NSWebPath.GetServerPath(relativePath);//Path.Combine(NSHttpRuntime.AppDomainAppPath, relativePath);
            }
            else
            {
                return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath);
            }
        }

        /// <summary>
        /// Verify that the string is correct for the Cron expression
        /// </summary>
        /// <param name="cronExpression">Band check expression</param>
        /// <returns></returns>
        public static bool ValidExpression(string cronExpression)
        {
            return CronExpression.IsValidExpression(cronExpression);
        }

        /// <summary>
        /// Acquisition tasks which will run in the next cycle
        /// </summary>
        /// <param name="CronExpressionString">Cron Expressions</param>
        /// <param name="numTimes">Running times</param>
        /// <returns>Run time segment</returns>
        public static List<DateTime> GetTaskeFireTime(string CronExpressionString, int numTimes)
        {
            List<DateTime> list = new List<DateTime>();
            try
            {
                if (numTimes < 0)
                {
                    return list;
                }
                //Time expression
                ITrigger trigger = TriggerBuilder.Create().WithCronSchedule(CronExpressionString).Build();
                IReadOnlyList<DateTimeOffset> dates = TriggerUtils.ComputeFireTimes(trigger as IOperableTrigger, null, numTimes);

                foreach (DateTimeOffset dtf in dates)
                {
                    list.Add(TimeZoneInfo.ConvertTimeFromUtc(dtf.DateTime, TimeZoneInfo.Local));
                }
                return list;
            }
            catch (Exception)
            {
                return list;
            }
        }

        #endregion

    }
}
