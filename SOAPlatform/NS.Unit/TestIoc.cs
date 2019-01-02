using EmitMapper;
using NS.Framework.IOC;
using NS.NS.Unit.TestMeta;
using NUnit.Framework;
using Unity.Registration;

namespace Tests
{
    public class TestIoc
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test_CreateInstance()
        {
            object studentObj = ObjectContainer.CreateInstance<Student>();
            Assert.NotNull(studentObj);

            Student student = studentObj as Student;
            Assert.NotNull(student);

            //Assert.Pass();
        }

        [Test]
        public void Test_RegisterInstance()
        {
            object studentObj = ObjectContainer.CreateInstance<Student>();
            Assert.NotNull(studentObj);

            Student student = studentObj as Student;
            Assert.NotNull(student);

            int oldCount = ObjectContainer.GetRegisterCount();
            ObjectContainer.RegisterInstance<Student>(student);
            int count = ObjectContainer.GetRegisterCount();
            Assert.AreEqual(1, count - oldCount);
        }

        [Test]
        public void Test_test()
        {
           
        }
    }
}