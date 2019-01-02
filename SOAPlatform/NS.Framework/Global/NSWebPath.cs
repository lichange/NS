using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NS.Framework.Global
{
    public static class NSWebPath
    {
        public static IHostingEnvironment HostEnv;

        public static string GetServerPath(string fileName)
        {
            return Path.Combine(HostEnv.ContentRootPath, fileName);
        }
    }

    /// 在启动文件Startup中注入
    //public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider svp)
    //{
    //    NSWebPath.ServiceProvider = svp;

    //    NSWebPath.HostEnv = env;
    //}
}
