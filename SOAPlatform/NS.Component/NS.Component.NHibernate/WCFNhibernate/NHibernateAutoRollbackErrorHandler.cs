//using System;
//using System.ServiceModel.Channels;
//using System.ServiceModel.Dispatcher;

//namespace NS.Component.NHibernate
//{
//    public class NHibernateAutoRollbackErrorHandler:IErrorHandler
//    {
//        public void ProvideFault(Exception error, MessageVersion version, ref Message fault)
//        {
//            //Log.For(this).Error("Error caught in service, NHibernate Session rolled back", error);
//            NHibernateContext.Current().Rollback();
//        }

//        public bool HandleError(Exception error)
//        {
//            return false;
//        }
//    }
//}