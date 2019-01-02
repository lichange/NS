using EmitMapper;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace NS.Unit
{
   public class TestEmit
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test_DynamicAssemblyManager()
        {
            try
            {
                DynamicAssemblyManager.SaveAssembly();
            }
            catch (Exception e)
            {
                string message = e.Message;
                if (message == ".net standard 2.0 不支持assemblyBuilder.Save 方法")
                {
                    Assert.Pass();
                }
                else
                {
                    Assert.Fail();
                }
            }
        }
    }
}
