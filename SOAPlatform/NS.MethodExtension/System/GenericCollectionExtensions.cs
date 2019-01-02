using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Collections.Generic
{
    public static class GenericCollectionExtensions
    {
        public static void AddRange<T>(this ICollection<T> source, IEnumerable<T> objects)
        {
            if (source != null && objects!=null)
            {
                foreach (var item in objects)
                {
                    source.Add(item);
                }
            }
        }

        public static ArrayList GetCollectionInfo<T>(this ICollection<T> source)
        {
            ArrayList newList = new ArrayList();

            if (source != null )
            {
                System.Reflection.MethodInfo methodInfo = typeof(T).GetMethod("GetInfo");

                if (methodInfo == null)
                    return null;

                foreach (var item in source)
                {
                    newList.Add(methodInfo.Invoke(item, null));
                }
            }

            return newList;
        }

        public static void SetCollectionInfo<T>(this ICollection<T> source, ArrayList objects)
        {
            if (source != null && objects != null)
            {
                System.Reflection.MethodInfo methodInfo = typeof(T).GetMethod("SetInfo");

                if (methodInfo == null)
                    return;

                foreach (var item in objects)
                {
                    if (item == null)
                        continue;

                    T instance = Activator.CreateInstance<T>();
                    methodInfo.Invoke(instance, new object[] { item });

                    source.Add(instance);
                }
            }
        }

        /// <summary>
        /// 遍历执行action
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="func"></param>
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> func)
        {
            foreach (var item in source)
                func(item);
        }
    }
}
