using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.TestHost;
using Moq;
using NS.Framework.Global;
using NS.NS.Unit.TestMeta;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace NS.Unit
{
    public class TestAspNetCore
    {
        private IWebHost _server;
        [SetUp]
        public void Setup()
        {
            //已在Startup中mock
            var builder = WebHost.CreateDefaultBuilder()
                .UseUrls("http://localhost:8888")
                 .UseStartup<Startup>();

            _server = builder.Build();
            _server.Start();
        }

        [Test]
        public void Test_NSWebPath()
        {
            string path = NSWebPath.HostEnv.ContentRootPath;

            Assert.IsNotEmpty(path);
        }

        [Test]
        public void Test_HttpContextInjection()
        {
            HttpContext context = NSHttpContext.Current;

            Assert.IsNotNull(context);
        }

        [Test]
        public void Test_Controller()
        {
            ValuesController controller = new ValuesController();
            var result = controller.Get();
        }
    }
}
