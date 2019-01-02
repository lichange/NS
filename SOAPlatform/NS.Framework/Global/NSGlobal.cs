using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace NS.Framework.Global
{
    public class NSGlobal
    {
        private static bool isUseSession;
        /// <summary>
        /// 执行初始化前必须将NSGlobal类中的注入方法调用完毕
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public static void Init(IApplicationBuilder app, IHostingEnvironment env)
        {
            //注入HttpContext.Current
            app.UseStaticHttpContext();

            //注入宿主环境
            NSWebPath.HostEnv = env;

            //启用session
            if (isUseSession)
            {
                app.UseSession();
            }
        }

        public static void InjectHttpContext(IServiceCollection services)
        {
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        }

        public static void InjectHttpSession(IServiceCollection services, int minutes)
        {
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(minutes);
            });
            isUseSession = true;
        }
    }
}
