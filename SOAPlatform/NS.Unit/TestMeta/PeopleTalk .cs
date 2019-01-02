using System;
using System.Collections.Generic;
using System.Text;

namespace NS.Unit.TestMeta
{
    public class PeopleTalk : ITalk
    {
        private string username;

        private int age;

        public string UserName
        {
            get { return username; }
        }

        public int Age
        {
            get { return age; }
        }

        public PeopleTalk(string userName, int age)
        {
            this.username = userName;
            this.age = age;
        }

        public bool talk(string msg)
        {
            Console.WriteLine(msg + "!你好，我是" + username + "，我的年龄" + age);
            return true;
        }

    }
}

