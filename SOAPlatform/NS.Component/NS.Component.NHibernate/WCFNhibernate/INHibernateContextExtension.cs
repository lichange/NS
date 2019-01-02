using System;
using NHibernate;

namespace NS.Component.NHibernate
{
    public interface INHibernateContextExtension
    {
        ISession Session { get; }
//        void InstanceContextFaulted(object sender, EventArgs e);
    }
}