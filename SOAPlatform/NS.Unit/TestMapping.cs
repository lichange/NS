using NS.DDD.Data.BulkExtensions;
using NT.XUnit.TestMeta;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace NS.Unit
{
    public class TestMapping
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test_Mapping()
        {
            using (NT.XUnit.TestMeta.TestContext ctx = new NT.XUnit.TestMeta.TestContext(ContextUtil.GetOptions()))
            {
                DbMapping map = DbMapper.GetDbMapping(ctx);
            }
        }
    }
}
