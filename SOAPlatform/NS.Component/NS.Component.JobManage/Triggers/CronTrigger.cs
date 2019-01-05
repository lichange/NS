using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Xml.Serialization;

using NS.Component.JobManage.Expressions;

using NS.Framework.Log;
using NS.Framework.IOC;

namespace NS.Component.JobManage.Triggers
{
  /// <summary>
  /// 时程触发器
  /// </summary>
  [Serializable]
  [DataContract]
  public class CronTrigger : Trigger
  {
      private ILogProvider _logProvider;
    /// <summary>
    /// 时程触发器
    /// </summary>
    public CronTrigger()
      : base()
      {
          _logProvider = ObjectContainer.CreateInstance<ILogProvider>();
    }

    /// <summary>
    /// 时程触发器
    /// </summary>
    /// <param name="cronExpressionString">时程表达式</param>
    /// <param name="job">触发的作业</param>
    public CronTrigger(string cronExpressionString, IJob job)
      : this()
    {
      CronExpressionString = cronExpressionString;
      TargetJob = job;
    }

    /// <summary>
    /// 时程表达式字符串
    /// </summary>
    [XmlAttribute]
    [DataMember]
    public string CronExpressionString
    {
      get
      {
        return this.CronExpression == null ? null : this.CronExpression.CronExpressionString;
      }
      set
      {
        this.CronExpression = new CronExpression(value);
      }
    }

    /// <summary>
    /// 时程表达式
    /// </summary>
    [XmlIgnore]
    [IgnoreDataMember]
    public CronExpression CronExpression { get; protected set; }

    /// <summary>
    /// 下一次触发时间
    /// </summary>
    [XmlIgnore]
    [IgnoreDataMember]
    public DateTime? NextFireTime { get; protected set; }

    /// <summary>
    /// 运行触发器之前的准备工作
    /// </summary>
    protected override void PrepareRun()
    {
      ComputeNextFireTime();
    }

    /// <summary>
    /// 检测触发时间
    /// </summary>
    /// <returns></returns>
    protected override bool CheckFireTime()
    {
      if (NextFireTime == null || !NextFireTime.HasValue)
      {
        return false;
      }

      if (DateTime.Now >= NextFireTime.Value)
      {
        return true;
      }

      return false;
    }

    /// <summary>
    /// 检测是否继续
    /// </summary>
    /// <returns></returns>
    protected override bool CheckContinue()
    {
      ComputeNextFireTime();

      if (NextFireTime == null || !NextFireTime.HasValue)
      {
          _logProvider.Debug(string.Format(CultureInfo.InvariantCulture, @"Is Continue : {0}", "False"));
        return false;
      }

      _logProvider.Debug(string.Format(CultureInfo.InvariantCulture, @"Is Continue : {0}", "True"));
      return true;
    }

    /// <summary>
    /// 获取等待时长
    /// </summary>
    /// <returns></returns>
    protected override int WaitingMilliseconds
    {
      get
      {
        DateTime now = DateTime.Now;

        if (NextFireTime == null || !NextFireTime.HasValue)
        {
            _logProvider.Debug(string.Format(CultureInfo.InvariantCulture, @"Waiting : 0 Millisecond when null."));
          return 0;
        }

        if (now > NextFireTime.Value)
        {
            _logProvider.Debug(string.Format(CultureInfo.InvariantCulture, @"Waiting : 0 Millisecond when over time."));
          return 0;
        }
        else
        {
          int sleep = (int)(Math.Round((NextFireTime.Value - now).TotalMilliseconds));
          _logProvider.Debug(string.Format(CultureInfo.InvariantCulture, @"Waiting : {0} Milliseconds until {1}.", sleep,
              NextFireTime.HasValue ? NextFireTime.Value.ToString(Constants.DateTimeFormat, CultureInfo.InvariantCulture) : "null"));
          return sleep;
        }
      }
    }

    /// <summary>
    /// 计算下一次触发时间
    /// </summary>
    private void ComputeNextFireTime()
    {
      NextFireTime = this.CronExpression.NextTime;

      _logProvider.Debug(string.Format(CultureInfo.InvariantCulture, @"Next Fire Time : {0} ", NextFireTime.HasValue ?
          NextFireTime.Value.ToString(Constants.DateTimeFormat, CultureInfo.InvariantCulture) : "null"));
    }
  }
}
