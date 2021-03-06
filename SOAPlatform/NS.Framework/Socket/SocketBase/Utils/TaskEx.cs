﻿using System;
using System.Threading;

namespace NS.Framework.RocketSocket.SocketBase.Utils
{
    /// <summary>
    /// 任务Task辅助类
    /// </summary>
    public static class TaskEx
    {
        /// <summary>
        /// 延迟执行某个动作
        /// </summary>
        /// <param name="dueTime"></param>
        /// <param name="callback"></param>
        /// <exception cref="ArgumentOutOfRangeException">dueTime</exception>
        /// <exception cref="ArgumentNullException">callback is null</exception>
        public static void Delay(int dueTime, Action callback)
        {
            if (dueTime < -1)
                throw new ArgumentOutOfRangeException("dueTime");
            if (callback == null)
                throw new ArgumentNullException("callback");

            if (dueTime == 0)
            {
                try
                {
                    callback();
                }
                catch (Exception ex)
                {
                    Log.Trace.Error(ex.Message, ex);
                }
                return;
            }

            Timer timer = null;
            timer = new Timer(_ =>
            {
                timer.Dispose();
                try
                {
                    callback();
                }
                catch (Exception ex)
                {
                    Log.Trace.Error(ex.Message, ex);
                }
            }, null, dueTime, Timeout.Infinite);
        }
    }
}