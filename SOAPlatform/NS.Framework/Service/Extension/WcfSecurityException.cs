/****************************************************************************************************************
*                                                                                                               *
* Copyright (C) 2011 5173.com                                                                                   *
* This project may be copied only under the terms of the Apache License 2.0.                                    *
* Please visit the project Home Page http://wcfextension.codeplex.com/ for more detail.                         *
*                                                                                                               *
****************************************************************************************************************/

namespace WcfExtension
{
    using System;

    /// <summary>
    /// 异常信息----身份验证异常信息定义
    /// </summary>
    internal class WcfSecurityException : Exception
    {
        internal WcfSecurityException(string message) : base(message) {}
    }
}
