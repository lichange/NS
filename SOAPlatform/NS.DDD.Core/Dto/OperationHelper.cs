//using NS.Component.Utility;
//using NS.Framework.Utility;

//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace NS.DDD.Core
//{

//    public static class OperationAnts
//    {
//        public static readonly string CreatePerson = "CreatePerson";
//        public static readonly string CreateDate = "CreateDate";
//        public static readonly string UpdatePerson = "UpdatePerson";
//        public static readonly string UpdateDate = "UpdateDate";
//    }

//    public static class OperationHelper
//    {
//        public static IList<T> RefreshDtoOperationInfo<T>(this IList<T> list)
//        {
//            if (list.Count == 0)
//                return list;

//            var currentUser = FormsAuth.GetUserData<CurrentUser>();

//            var tempCreatePersonProperty = typeof(T).GetProperty(OperationAnts.CreatePerson);
//            var tempCreateDateProperty = typeof(T).GetProperty(OperationAnts.CreateDate);
//            var tempUpdatePersonProperty = typeof(T).GetProperty(OperationAnts.UpdatePerson);
//            var tempUpdateDateProperty = typeof(T).GetProperty(OperationAnts.UpdateDate);

//            var tempCreatePersonLmdGet = DynamicMethodFactory.CreatePropertyGetter(tempCreatePersonProperty);
//            var tempCreateDateLmdGet = DynamicMethodFactory.CreatePropertyGetter(tempCreateDateProperty);
//            var tempUpdatePersonLmdGet = DynamicMethodFactory.CreatePropertyGetter(tempUpdatePersonProperty);
//            var tempUpdateDateLmdGet = DynamicMethodFactory.CreatePropertyGetter(tempUpdateDateProperty);

//            var tempCreatePersonLmdSet = DynamicMethodFactory.CreatePropertySetter(tempCreatePersonProperty);
//            var tempCreateDateLmdSet = DynamicMethodFactory.CreatePropertySetter(tempCreateDateProperty);
//            var tempUpdatePersonLmdSet = DynamicMethodFactory.CreatePropertySetter(tempUpdatePersonProperty);
//            var tempUpdateDateLmdSet = DynamicMethodFactory.CreatePropertySetter(tempUpdateDateProperty);

//            foreach (var item in list)
//            {
//                var tempCreator = tempCreatePersonLmdGet(item);
//                if (tempCreator == null || string.IsNullOrEmpty(tempCreator.ToString()))
//                    tempCreatePersonLmdSet(item, currentUser.UserCode);

//                var tempTime = (DateTime)tempCreateDateLmdGet(item);
//                if (tempTime <= DateTime.MinValue || tempTime <= DateTime.Parse("1900-01-01 00:00:00"))
//                    tempCreateDateLmdSet(item, DateTime.Now);

//                //if (tempUpdatePersonLmdGet(item) == null)
//                tempUpdatePersonLmdSet(item, currentUser.UserCode);

//                //if ((DateTime)tempUpdateDateLmdGet.GetValue(item) <= DateTime.MinValue)
//                tempUpdateDateLmdSet(item, DateTime.Now);
//            }

//            return list;
//        }
//    }
//}
