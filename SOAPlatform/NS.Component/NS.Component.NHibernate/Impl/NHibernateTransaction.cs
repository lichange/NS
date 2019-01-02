using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate;

namespace NS.Component.Data.Impl
{
    /// <summary>
    /// 封装NHibernate.ITransaction,实现IKuafTransaction接口
    /// </summary>
    internal class NHibernateTransaction : IHMTransaction
    {
        private readonly ITransaction trans;
        private bool IsNotified;

        public NHibernateTransaction(ITransaction trans)
        {
            this.trans = trans;
        }

        public void Commit()
        {
            trans.Commit();
            if (OnTransactionEnd != null && !IsNotified)
            {
                IsNotified = true;
                OnTransactionEnd(this, EventArgs.Empty);
            }
        }

        public void Rollback()
        {
            trans.Rollback();
            if (OnTransactionEnd != null && !IsNotified)
            {
                IsNotified = true;
                OnTransactionEnd(this, EventArgs.Empty);
            }
        }

        public void Dispose()
        {
            trans.Dispose();
            if (OnTransactionEnd != null && !IsNotified)
            {
                IsNotified = true;
                OnTransactionEnd(this, EventArgs.Empty);
            }
        }

        public event EventHandler OnTransactionEnd;
    }
}
