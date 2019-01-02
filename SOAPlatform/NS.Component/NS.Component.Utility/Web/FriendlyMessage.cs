using System;
using System.Text;

namespace NS.Component.Utility
{
    public static class FriendlyMessage
    {
        /// <summary>
        /// 获取消息(根据操作类型和状态)
        /// </summary>
        /// <param name="sysOp">操作类型</param>
        /// <param name="status">执行状态</param>
        /// <returns></returns>
        public static string ToMessage(this SysOperate sysOp, bool status)
        {
            string message = "";
            //根据操作类型和执行状态返回消息
            switch (sysOp)
            {
                case SysOperate.Add:
                    message = status ? SysMessage.AddSuccess.ToMessage() : SysMessage.AddError.ToMessage();
                    break;
                case SysOperate.Load:
                    message = status ? SysMessage.LoadSuccess.ToMessage() : SysMessage.LoadError.ToMessage();
                    break;
                case SysOperate.Update:
                    message = status ? SysMessage.UpdateSuccess.ToMessage() : SysMessage.UpdateError.ToMessage();
                    break;

                case SysOperate.Delete:
                    message = status ? SysMessage.DeleteSuccess.ToMessage() : SysMessage.DeleteError.ToMessage();
                    break;
                case SysOperate.Operate:
                    message = status ? SysMessage.OperateSuccess.ToMessage() : SysMessage.OperateError.ToMessage();
                    break;
                case SysOperate.UnkownError:
                    message = SysMessage.UnkownError.ToMessage();
                    break;
                case SysOperate.ParamError:
                    message = SysMessage.ParamError.ToMessage();
                    break;
            }
            return message;
        }
        /// <summary>
        /// 获取系统管理模块友好提示信息
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static string ToMessage(this SysMessage code)
        {
            string message = "";
            switch (code) 
            {
                case SysMessage.AddSuccess:
                    message = "添加成功!";
                    break;
                case SysMessage.AddError:
                    message = "添加失败!";
                    break;
                case SysMessage.DeleteSuccess:
                    message = "删除成功!";
                    break;
                case SysMessage.DeleteError:
                    message = "删除失败!";
                    break;
                case SysMessage.LoadSuccess:
                    message = "加载成功!";
                    break;
                case SysMessage.LoadError:
                    message = "加载失败!";
                    break;
                case SysMessage.OperateSuccess:
                    message = "操作成功!";
                    break;
                case SysMessage.OperateError:
                    message = "操作失败!";
                    break;
                case SysMessage.UpdateSuccess:
                    message = "更新成功!";
                    break;
                case SysMessage.UpdateError:
                    message = "更新失败!";
                    break;
                case SysMessage.UnkownError:
                    message = "未知错误!";
                    break;
                case SysMessage.ParamError:
                    message = "参数错误!";
                    break;
                default:
                    message = "错误";
                    break;
            }
            return message;
        }
    }
}
