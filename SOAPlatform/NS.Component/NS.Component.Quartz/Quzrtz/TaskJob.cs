using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;
using NS.Framework.Quartz;

namespace NS.Component.Quartz
{
    [DisallowConcurrentExecution]
    public class TaskJob : IJob
    {
        TaskUtil task;
      
        Task IJob.Execute(IJobExecutionContext context)
        {
            return Task.Run(() => {

                try
                {
                    task = (TaskUtil)context.JobDetail.JobDataMap.Get("TaskParam");
                    if (task.Execute != null)
                    {
                        task.Execute.Invoke(null, null);
                        task.Result = "success";
                    }
                }
                catch (Exception ex)
                {
                    task.Result = ex.Message;
                    JobExecutionException e2 = new JobExecutionException(ex);
                    //1.立即重新执行任务 
                    e2.RefireImmediately = true;
                    //2 立即停止所有相关这个任务的触发器
                    //e2.UnscheduleAllTriggers=true; 
                }

            });
           
        }
    }
}
