using System;

namespace NS.Component.Utility
{
    [Flags]
    public enum Authority : int
    {
        /// <summary>
        /// 浏览权限
        /// </summary>
        Index = 1,
        /// <summary>
        /// 添加权限
        /// </summary>
        Add = 2,
        /// <summary>
        /// 编辑权限
        /// </summary>
        Edit = 4,
        /// <summary>
        /// 删除权限
        /// </summary>
        Delete = 8,
        /// <summary>
        /// 搜索权限
        /// </summary>
        Search = 16,
        /// <summary>
        /// 审核权限
        /// </summary>
        Verify = 32,
        /// <summary>
        /// 移动权限
        /// </summary>
        Move = 64,
        /// <summary>
        /// 打印权限
        /// </summary>
        Print = 128,
        /// <summary>
        /// 下载权限
        /// </summary>
        Download = 256,
        /// <summary>
        /// 备份权限
        /// </summary>
        Backup = 512,
        /// <summary>
        /// 授权权限
        /// </summary>
        Grant = 1024,
        /// <summary>
        /// 查看详细权限
        /// </summary>
        View = 2048,
        /// <summary>
        /// 导出权限
        /// </summary>
        Export = 4096
    }
}
