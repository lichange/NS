﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NS.DDD
{
    public interface IBussinessRule<T>
    {
        bool Excute(T instance);
    }
}
