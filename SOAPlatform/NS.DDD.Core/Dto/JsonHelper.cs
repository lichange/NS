using Newtonsoft.Json.Linq;
using NS.DDD.Core;
using NS.DDD.Core.Dto;
using NS.Framework.Json;
using NS.Framework.Utility;
using NS.Framework.Utility.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace Newtonsoft.Json.Linq
{
    public static class JsonHelper
    {
        public static IList<T> JSONToDtoList<T>(this JObject data) where T : new()
        {
            //var form = data["form"];
            var list = data["list"];

            IList<T> tempList = new List<T>();

            //var tempLmd = ReflectHelper.LmdSet(typeof(T), "OperationType");

            if (list != null)
            {
                var tempData = data["list"];
                foreach (JToken item in tempData)
                {
                    var tempRow = item.Deserialize<T>();
                    tempList.Add(tempRow);
                }
            }

            //if (form != null)
            //    tempList.Add(data.ToDto<T>());


            return tempList;
        }

        public static IList<T> ToDtoList<T>(this JObject data) where T : new()
        {
            var form = data["form"];
            var list = data["list"];

            IList<T> tempList = new List<T>();

            var tempLmd = ReflectHelper.LmdSet(typeof(T), "OperationType");

            if (list != null)
            {
                var tempData = data["list"].Children();
                foreach (JToken item in tempData)
                {
                    //var tempKey = GetOperationType(item["OperationType"].ToString());

                    //if (tempKey == OperationType.None)
                    //    continue;

                    //var tempRow = item.Deserialize<T>();
                    //tempLmd(tempRow, ConvertToDDDType(tempKey));
                    //tempList.Add(tempRow);
                    tempList.Add(item.ToDto<T>());
                }
            }

            if (form != null)
                tempList.Add(data.ToDto<T>());


            return tempList;
        }

        public static IList<T> ToDtoList<T>(this JObject data, OperationType type) where T : new()
        {
            var form = data["form"];
            var list = data["list"];

            IDictionary<OperationType, IList<T>> tempDictionary = new Dictionary<OperationType, IList<T>>();

            tempDictionary.Add(OperationType.Inserted, new List<T>());
            tempDictionary.Add(OperationType.Updated, new List<T>());
            tempDictionary.Add(OperationType.Deleted, new List<T>());

            foreach (JProperty item in data["list"].Children())
            {
                var tempKeyName = GetItemOperationType(item.Name);
                if (!tempDictionary.ContainsKey(tempKeyName))
                    continue;

                foreach (var row in item.Value.Children())
                {
                    var tempRow = row.Deserialize<T>();

                    tempDictionary[tempKeyName].Add(tempRow);
                }
            }

            return tempDictionary.Where(pre => pre.Key == type).First().Value;
        }

        public static IDictionary<OperationType, IList<T>> ToDtoDictionary<T>(this JObject data) where T : new()
        {
            var form = data["form"];
            var list = data["list"];

            IDictionary<OperationType, IList<T>> tempDictionary = new Dictionary<OperationType, IList<T>>();

            tempDictionary.Add(OperationType.Inserted, new List<T>());
            tempDictionary.Add(OperationType.Updated, new List<T>());
            tempDictionary.Add(OperationType.Deleted, new List<T>());

            var chilernList = data["list"].Children();

            foreach (JProperty item in chilernList)
            {
                var tempKeyName = GetItemOperationType(item.Name);
                if (!tempDictionary.ContainsKey(tempKeyName))
                    continue;

                foreach (var row in item.Value.Children())
                {
                    var tempRow = row.Deserialize<T>();

                    tempDictionary[tempKeyName].Add(tempRow);
                }
            }

            return tempDictionary;
        }

        public static T ToDto<T>(this JToken data) where T : new()
        {
            var tempResult = Activator.CreateInstance<T>(); ////创建指定类型的实例

            var propertypes = tempResult.GetType().GetProperties(); //得到类的属性

            if (data != null)
            {
                foreach (PropertyInfo pro in propertypes)
                {
                    var tempName = pro.Name.ToLower();
                    var value = data[pro.Name] != null ? data[pro.Name] : data[tempName];
                    var setDelegate = DynamicMethodFactory.CreatePropertySetter(pro);
                    if (value == null)
                    {
                        setDelegate(tempResult, GetDefaultValue(pro.PropertyType));
                    }
                    else
                        setDelegate(tempResult, GetValue(pro, value));
                }
            }
            else
                tempResult = data.Deserialize<T>();


            return tempResult;
        }

        public static T DeserializeToDto<T>(this JToken data) where T : new()
        {
            var tempResult = Activator.CreateInstance<T>(); ////创建指定类型的实例

            if (data != null)
                tempResult = data.Deserialize<T>();

            return tempResult;
        }

        public static T FastDeserializeToDto<T>(this JToken data) where T : new()
        {
            var tempResult = Activator.CreateInstance<T>(); ////创建指定类型的实例
            JsonParser jp = new JsonParser();
            var obj = jp.ToObject(typeof(T), data.ToString());

            //if (data != null)
            //    tempResult = data.Deserialize<T>();

            return tempResult;
        }

        private static object GetValue(PropertyInfo pro, object value)
        {
            if (pro.PropertyType == typeof(Int32) && value is JValue)
            {
                var tempValue = ((JValue)value).Value.ToString();
                return string.IsNullOrEmpty(tempValue) ? 0 : int.Parse(tempValue);
            }
            else if (pro.PropertyType == typeof(DateTime) && value is JValue)
            {
                return Convert.ToDateTime(((JValue)value).Value);
            }

            if (pro.PropertyType == typeof(DateTime) && value is JValue)
            {
                var tempValue = ((JValue)value).Value.ToString();
                return string.IsNullOrEmpty(tempValue) ? DateTime.Now : DateTime.Parse(tempValue);
            }

            if (pro.PropertyType == typeof(bool) && value is JValue)
            {
                return Convert.ToBoolean(((JValue)value).Value);
            }

            //updated by pengye 
            //time: 2016/05/10
            //reason: boolean to string raise error in SetValueDelegate method 
            if (pro.PropertyType == typeof(string) && value is JValue)
            {
                return Convert.ToString(((JValue)value).Value);
            }

            if (pro.PropertyType == typeof(decimal) && value is JValue)
            {
                return Convert.ToDecimal(((JValue)value).Value);
            }

            if (pro.PropertyType != typeof(OperatingType) && value is JValue)
                return ((JValue)value).Value;

            //if (pro.PropertyType != typeof(OperatingType) && value is JArray)
            //{
            //    var tempArray = (JArray)value;

            //    if (tempArray == null)
            //        return null;

            //    foreach (var arrayItem in tempArray)
            //    {

            //    }
            //}

            var tempResult = OperatingType.Query;
            switch (((JValue)value).Value.ToString().ToLower())
            {
                case "insert":
                    tempResult = OperatingType.Insert;
                    break;
                case "update":
                    tempResult = OperatingType.Update;
                    break;
                case "delete":
                    tempResult = OperatingType.Delete;
                    break;
                default:
                    tempResult = OperatingType.Query;
                    break;
            }

            return tempResult;
        }

        public static object GetDefaultValue(Type type)
        {
            object tempResult = null;
            switch (type.Name.ToLower())
            {
                case "string":
                    tempResult = "";
                    break;
                case "byte[]":
                    tempResult = new byte[0];
                    break;
                case "bool":
                case "boolean":
                    tempResult = false;
                    break;
                case "byte":
                    tempResult = new byte();
                    break;
                case "short":
                    tempResult = 0;
                    break;
                case "int":
                case "int32":
                    tempResult = 0;
                    break;
                case "long":
                    tempResult = 0;
                    break;
                case "ushort":
                    tempResult = 0;
                    break;
                case "uint":
                    tempResult = 0;
                    break;
                case "ulong":
                    tempResult = 0;
                    break;
                case "float":
                    tempResult = 0;
                    break;
                case "currency":
                case "decimal":
                    tempResult = decimal.Parse("0");
                    break;
                case "double":
                    tempResult = 0;
                    break;
                case "date":
                case "datetime":
                case "time":
                    tempResult = DateTime.Parse("1900-01-01 00:00:00");
                    break;
                case "guid":
                    tempResult = Guid.Empty;
                    break;
                case "object":
                case "numeric":
                    tempResult = null;
                    break;
                case "operatingtype":
                    tempResult = OperatingType.Query;
                    break;
                default:
                    tempResult = null;
                    break;
            }

            return tempResult;
        }

        public static IList<T> ConvertJsonToList<T>(IEnumerable<JToken> jtokens) where T : class, new()
        {
            IList<T> list = new List<T>(); //里氏替换原则
            T t = default(T);
            PropertyInfo[] propertypes = null;
            string tempName = string.Empty;
            propertypes = t.GetType().GetProperties(); //得到类的属性
            foreach (JToken row in jtokens)
            {
                t = Activator.CreateInstance<T>(); ////创建指定类型的实例

                foreach (PropertyInfo pro in propertypes)
                {
                    tempName = pro.Name;
                    object value = row[tempName];
                    if (value is System.DBNull)
                    {
                        value = "";
                    }

                    var setDelegate = DynamicMethodFactory.CreatePropertySetter(pro);

                    setDelegate(t, value);
                }
                list.Add(t);
            }
            return list;
        }

        private static OperatingType ConvertToDDDType(OperationType type)
        {
            var tempType = OperatingType.Query;

            switch (type)
            {
                case OperationType.Inserted:
                    tempType = OperatingType.Insert;
                    break;
                case OperationType.Updated:
                    tempType = OperatingType.Update;
                    break;
                case OperationType.Deleted:
                    tempType = OperatingType.Delete;
                    break;
                default:
                    break;
            }

            return tempType;
        }

        private static OperationType GetOperationType(string valuetype)
        {
            var tempDefaultType = OperationType.None;

            if (string.IsNullOrEmpty(valuetype))
                return tempDefaultType;

            switch (valuetype)
            {
                case "Insert":
                    tempDefaultType = OperationType.Inserted;
                    break;
                case "Update":
                    tempDefaultType = OperationType.Updated;
                    break;
                case "Delete":
                    tempDefaultType = OperationType.Deleted;
                    break;
                default:
                    tempDefaultType = OperationType.None;
                    break;
            }

            return tempDefaultType;
        }

        private static OperationType GetItemOperationType(string name)
        {
            var tempDefaultType = OperationType.None;

            if (string.IsNullOrEmpty(name))
                return tempDefaultType;

            switch (name)
            {
                case "inserted":
                    tempDefaultType = OperationType.Inserted;
                    break;
                case "updated":
                    tempDefaultType = OperationType.Updated;
                    break;
                case "deleted":
                    tempDefaultType = OperationType.Deleted;
                    break;
                default:
                    tempDefaultType = OperationType.None;
                    break;
            }

            return tempDefaultType;
        }

        #region     JArray to T

        public static IList<T> ToDtoList<T>(this JArray data) where T : new()
        {
            IList<T> tempList = new List<T>();

            if (data == null || data.Count == 0)
                return tempList;

            foreach (JToken item in data.Children())
            {
                //var tempRow = item.Deserialize<T>();
                //tempList.Add(tempRow);
                tempList.Add(item.ToDto<T>());
            }

            return tempList;
        }

        #endregion
    }

    /// <summary>
    /// Operation type
    /// </summary>
    public enum OperationType
    {
        Inserted,
        Updated,
        Deleted,
        None,
    }

}