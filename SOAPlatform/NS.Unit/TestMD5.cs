using NS.Component.Utility;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace NS.Unit
{
   public class TestMD5
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test_MD5()
        {
            string test = "test";
            string ret = StringHelper.MD5Hash(test);
            //098F6BCD4621D373CADE4E832627B4F6
            Assert.AreEqual("098F6BCD4621D373CADE4E832627B4F6", ret);
        }
    }
}
