using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace PlatformClient.Extend.Core
{
    public static class ExtendModel
    {
        /// <summary>
        /// 获取一个泛型T
        /// </summary>
        /// <typeparam name="T">泛型类</typeparam>
        /// <param name="i"></param>
        /// <returns>返回一个泛型T</returns>
        public static T GetFirst<T>(this IEnumerable @i) where T : class
        {
            IEnumerator ie = @i.GetEnumerator();
            while (ie.MoveNext())
            {
                return (T)ie.Current;
            }
            return default(T);
        }

        /// <summary>
        /// 获取一个泛型T的List T 集合
        /// </summary>
        /// <typeparam name="T">泛型类</typeparam>
        /// <param name="i">输入的</param>
        /// <returns>返回一个泛型T的List T 集合</returns>
        public static List<T> GetTList<T>(this IEnumerable @i) where T : class
        {
            List<T> tItem = new List<T>();
            IEnumerator ie = @i.GetEnumerator();
            while (ie.MoveNext())
            {
                T temp = (T)ie.Current;
                if (null == temp)
                {
                    continue;
                }
                tItem.Add(temp);
            }
            return tItem;
        }

        /// <summary>
        /// 获取一个泛型T的Array[T]集合
        /// </summary>
        /// <typeparam name="T">泛型类</typeparam>
        /// <param name="i">输入的</param>
        /// <returns>返回一个泛型T的Array[T]集合</returns>
        public static T[] GetTArray<T>(this IEnumerable @i) where T : class
        {
            List<T> tItem = GetTList<T>(@i);
            if (tItem != null && tItem.Count > 0)
            {
                return GetTList<T>(@i).ToArray();
            }
            return new T[0];
        }

    }
}
