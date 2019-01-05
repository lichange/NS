
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Quartz;
using NS.Framework.Quartz;
using NS.Framework.IOC;
using System.Threading;

namespace NS.Component.Quartz
{
    /// <summary>
    /// Custom TriggerListener
    /// </summary>
    public class QuartzListener : ITriggerListener
    {

        public string Name
        {
            get
            {
                return "All_TriggerListener";
            }
        }

        /// <summary>
        /// Job execution time call
        /// </summary>
        /// <param name="trigger">trigger</param>
        /// <param name="context">context</param>
        public Task TriggerFired(ITrigger trigger, IJobExecutionContext context, CancellationToken cancellationToken = default(CancellationToken))
        {
           return Task.Run(() => { context.JobDetail.JobDataMap.Get("TaskParam"); });
        }
       

        /// <summary>
        /// When Trigger is triggered, the method is called when the job is executed. True that is rejected, job does not perform behind
        /// </summary>
        /// <param name="trigger">trigger</param>
        /// <param name="context">context</param>
        /// <returns></returns>
        public Task<bool> VetoJobExecution(ITrigger trigger, IJobExecutionContext context, CancellationToken cancellationToken = default(CancellationToken))
        {
           
            return Task.Run(() => {

                TaskUtil task = (TaskUtil)context.JobDetail.JobDataMap.Get("TaskParam");
                if (task.VetoJobExecution != null)
                {
                    task.RecentRunTime = TimeZoneInfo.ConvertTimeFromUtc(context.FireTimeUtc.DateTime, TimeZoneInfo.Local);
                    object[] obj = { trigger, context, task };
                    task.VetoJobExecution.Invoke(null, obj);
                }
                return false;
            });
           
        }
       

        /// <summary>
        /// Job completion time call
        /// </summary>
        /// <param name="trigger">trigger</param>
        /// <param name="context">context</param>
        /// <param name="triggerInstructionCode"></param>
        
        public Task TriggerComplete(ITrigger trigger, IJobExecutionContext context, SchedulerInstruction triggerInstructionCode, CancellationToken cancellationToken = default(CancellationToken))
        {
           return Task.Run(() =>
            {
                TaskUtil task = (TaskUtil)context.JobDetail.JobDataMap.Get("TaskParam");
                if (task.TriggerComplete != null)
                {
                    task.NextRunTime = TimeZoneInfo.ConvertTimeFromUtc(context.NextFireTimeUtc.Value.DateTime, TimeZoneInfo.Local);
                    object[] obj = { trigger, context, task };
                    task.TriggerComplete.Invoke(null, obj);
                }
            });
        }
        

        /// <summary>
        /// Missed call
        /// </summary>
        /// <param name="trigger">trigger</param>
        public Task TriggerMisfired(ITrigger trigger, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.Run(() => { });
        }

      
    }
}
