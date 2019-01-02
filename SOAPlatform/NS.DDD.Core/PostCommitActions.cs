using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace NS.DDD.Core
{
    /// <summary>
    /// 提交执行的操作
    /// </summary>
    public static class PostCommitActions
    {
        static ThreadLocal<List<Action>> _actions = new ThreadLocal<List<Action>>(() => new List<Action>());

        public static void Enqueue(Action action)
        {
            //RequireValidate.NotNull(action, "action");
            _actions.Value.Add(action);
        }

        public static int Count()
        {
            return _actions.Value.Count;
        }

        public static IEnumerable<Action> GetQueuedActions()
        {
            return _actions.Value;
        }

        public static void Clear()
        {
            _actions.Value.Clear();
        }
    }
}
