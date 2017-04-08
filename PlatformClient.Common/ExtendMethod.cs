using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace PlatformClient.Common
{
    ///// <summary>
    ///// 扩展方法类
    ///// </summary>
    //public static class ExtendMethod
    //{
    //    /// <summary>
    //    /// 将对象的属性转成 XElement,只转属性部份，数组、泛型不转
    //    /// </summary>
    //    /// <param name="s"></param>
    //    /// <returns></returns>
    //    public static XElement ToXElement(this object @s)
    //    {
    //        if (null == @s)
    //        {
    //            return null;
    //        }
    //        return ToXElement(@s, "");
    //    }

    //    /// <summary>
    //    /// 将对象的属性转成 XElement,只转属性部份，数组、泛型不转
    //    /// </summary>
    //    /// <param name="elementName"></param>
    //    /// <param name="source"></param>
    //    /// <returns></returns>
    //    public static XElement ToXElement(this object @source, string elementName)
    //    {
    //        if (null == @source)
    //        {
    //            return null;
    //        }

    //        Type tt = @source.GetType();
    //        //-->得到所有属性
    //        PropertyInfo[] piItem = tt.GetProperties();
    //        XElement _result = new XElement(elementName.IsNullOrEmpty() ? tt.Name : elementName);
    //        foreach (var pi in piItem)
    //        {
    //            if (pi.PropertyType.IsArray)
    //            {
    //                continue;
    //            }
    //            else if (pi.PropertyType.FullName.Equals("System.Xml.Linq.XElement"))
    //            {
    //                object _value = pi.GetValue(@source, null);
    //                if (null != _value)
    //                {
    //                    _result.Add(new XElement(pi.Name, new XCData(_value.ToString())));
    //                }
    //                continue;
    //            }
    //            else if (pi.PropertyType.IsGenericType)
    //            {
    //                IList _Ilist = pi.GetValue(@source, null) as IList;
    //                if (null == _Ilist || 0 == _Ilist.Count)
    //                {
    //                    continue;
    //                }
    //                XElement _ChildXElement = new XElement(pi.Name, new XAttribute("Name", _Ilist[0].GetType().Name));
    //                foreach (var v in _Ilist)
    //                {
    //                    XElement _bb = v.ToXElement();
    //                    if (null == _bb)
    //                    {
    //                        continue;
    //                    }
    //                    _ChildXElement.Add(_bb);
    //                }
    //                _result.Add(_ChildXElement);
    //            }
    //            else if (pi.PropertyType.Name.Equals("Char"))
    //            {
    //                _result.Add(new XAttribute(pi.Name, string.Format("{0}", @source)));
    //            }
    //            else
    //            {
    //                object _value = pi.GetValue(@source, null);
    //                if (null == _value)
    //                {
    //                    continue;
    //                }
    //                if (pi.PropertyType.IsClass && pi.PropertyType.IsPublic
    //                    && !pi.PropertyType.IsPrimitive && !pi.PropertyType.IsValueType
    //                    && !(_value is IConvertible))
    //                {
    //                    var _Class_XElement = _value.ToXElement();
    //                    _result.Add(_Class_XElement);
    //                }
    //                else
    //                {
    //                    if (_value.ToString().IsNullOrEmpty())
    //                    {
    //                        continue;
    //                    }
    //                    _result.Add(new XAttribute(pi.Name, _value));
    //                }
    //            }
    //        }
    //        return _result;
    //    }
    //    /// <summary>
    //    /// 将XElement转成 T 对象
    //    /// </summary>
    //    /// <typeparam name="T"></typeparam>
    //    /// <param name="xElement"></param>
    //    /// <returns></returns>
    //    public static T ToModel<T>(this XElement @xElement) where T : new()
    //    {
    //        try
    //        {
    //            T _new = new T();
    //            _new = (T)@xElement.ToModel(_new);
    //            return _new;
    //        }
    //        catch
    //        {
    //            return default(T);
    //        }
    //    }
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <typeparam name="T"></typeparam>
    //    /// <param name="xElement"></param>
    //    /// <returns></returns>
    //    public static List<T> ToModelList<T>(this XElement @xElement) where T : new()
    //    {
    //        if (null == @xElement)
    //        {
    //            return null;
    //        }
    //        //-->获取字项是以什么开头的
    //        string subkeyName = @xElement.GetAttributeValue("Name");
    //        if (subkeyName.IsNullOrEmpty())
    //        {
    //            return null;
    //        }
    //        var subkeyItem = @xElement.Elements(subkeyName);
    //        if (null == subkeyItem || 0 == subkeyItem.Count())
    //        {
    //            return null;
    //        }
    //        List<T> item = new List<T>();
    //        foreach (var subkey in subkeyItem)
    //        {
    //            var data = subkey.ToModel<T>();
    //            if (null == data)
    //            {
    //                continue;
    //            }
    //            item.Add(data);
    //        }
    //        return item;
    //    }

    //    static object ToModel(this XElement @xElement, object setValue)
    //    {
    //        if (null == @xElement || null == setValue)
    //        {
    //            return null;
    //        }
    //        Type tt = setValue.GetType();
    //        PropertyInfo[] piItem = tt.GetProperties();
    //        foreach (var pi in piItem)
    //        {
    //            var pt = pi.PropertyType;
    //            if (pt.IsArray)
    //            {
    //                continue;
    //            }
    //            //-->判断是不是xml类型
    //            if (pt.FullName.Equals("System.Xml.Linq.XElement"))
    //            {
    //                pi.SetValue(setValue, xElement, null);
    //                continue;
    //            }
    //            //-->判断是不是集合
    //            if (pt.IsGenericType)
    //            {
    //                var GenericItem = @xElement.Element(pi.Name);
    //                if (null == GenericItem)
    //                {
    //                    continue;
    //                }
    //                //-->创建一个泛型
    //                object list = pt.InvokeMember(null,
    //                    BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.CreateInstance,
    //                    null, null, new object[] { });

    //                //-->获取Add方法
    //                MethodInfo mi = pt.GetMethod("Add");
    //                var _singleType = pt.GetGenericArguments()[0];
    //                string childName = GenericItem.GetAttributeValue("Name");
    //                var childs = GenericItem.Elements(childName);
    //                if (null == childs || 0 == childs.Count())
    //                {
    //                    continue;
    //                }
    //                foreach (var v_child in childs)
    //                {
    //                    //-->创建一个泛型类
    //                    object newC = Activator.CreateInstance(_singleType);
    //                    //-->给实例赋值
    //                    newC = v_child.ToModel(newC);
    //                    //-->将实例添加到泛型集合里去
    //                    mi.Invoke(list, new object[] { newC });
    //                }
    //                pi.SetValue(setValue, list, null);

    //                continue;
    //            }
    //            string value = @xElement.GetAttributeValue(pi.Name);
    //            if (value.IsNullOrEmpty())
    //            {
    //                value = @xElement.GetElementValue(pi.Name);
    //                if (value.IsNullOrEmpty())
    //                {
    //                    //-->判断是不是类
    //                    //if (pt.IsAnsiClass && pt.IsAutoLayout && !pt.IsPrimitive && pt.IsPublic && !pt.IsSealed && !pt.IsValueType && pt.IsVisible)
    //                    if (pt.IsClass && pt.IsPublic && !pt.IsPrimitive && !pt.IsValueType && pt.IsAutoLayout)
    //                    {
    //                        XElement _Element = @xElement.Element(pi.Name);
    //                        if (null == _Element)
    //                        {
    //                            _Element = @xElement.Element(pt.Name);
    //                        }
    //                        if (null == _Element)
    //                        {
    //                            continue;
    //                        }
    //                        //-->创建一个实例
    //                        var _NewClass = Activator.CreateInstance(pt);
    //                        _NewClass = _Element.ToModel(_NewClass);
    //                        try
    //                        {
    //                            pi.SetValue(setValue, _NewClass, null);
    //                        }
    //                        catch { }
    //                    }
    //                    continue;
    //                }//End if; 不为空是，则里面的值一定是字符串类型
    //            }
    //            pi.SetValue(setValue, Convert.ChangeType(value, pt, null), null);
    //        }
    //        return setValue;
    //    }

    //    /// <summary>
    //    /// 删除属性
    //    /// </summary>
    //    /// <param name="s"></param>
    //    /// <param name="list"></param>
    //    internal static void RemoveAttribute(this XElement @s, params string[] list)
    //    {
    //        foreach (var str in list)
    //        {
    //            var att = @s.Attribute(str);
    //            if (null == att)
    //            {
    //                continue;
    //            }
    //            att.Remove();
    //        }
    //    }

    //    /// <summary>
    //    /// 泛型类型转换
    //    /// </summary>
    //    /// <typeparam name="T">要转换的类型</typeparam>
    //    /// <param name="s">要转的内容</param>
    //    /// <returns>返回转换后的数据类型值</returns>
    //    public static T ConvertTo<T>(this IConvertible @s)
    //    {
    //        try
    //        {
    //            if (String.IsNullOrEmpty(string.Format("{0}", @s)))
    //            {
    //                return default(T);
    //            }
    //            Type t = typeof(T);
    //            if (t.IsGenericType)
    //            {
    //                Type gt = t.GetGenericTypeDefinition();
    //                if (typeof(Nullable<>) == gt)
    //                {
    //                    return (T)Convert.ChangeType(@s, Nullable.GetUnderlyingType(t), null);
    //                }
    //            }
    //            return (T)Convert.ChangeType(@s, t, null);
    //        }
    //        catch
    //        {
    //            return default(T);
    //        }
    //    }

    //    /// <summary>
    //    /// 获取一个泛型T
    //    /// </summary>
    //    /// <typeparam name="T">泛型类</typeparam>
    //    /// <param name="i"></param>
    //    /// <returns>返回一个泛型T</returns>
    //    public static T GetFirst<T>(this IEnumerable @i) where T : class
    //    {
    //        IEnumerator ie = @i.GetEnumerator();
    //        while (ie.MoveNext())
    //        {
    //            return (T)ie.Current;
    //        }
    //        return default(T);
    //    }

    //    /// <summary>
    //    /// 获取一个泛型T的List T 集合
    //    /// </summary>
    //    /// <typeparam name="T">泛型类</typeparam>
    //    /// <param name="i">输入的</param>
    //    /// <returns>返回一个泛型T的List T 集合</returns>
    //    public static List<T> GetTList<T>(this IEnumerable @i) where T : class
    //    {
    //        List<T> tItem = new List<T>();
    //        IEnumerator ie = @i.GetEnumerator();
    //        while (ie.MoveNext())
    //        {
    //            T temp = (T)ie.Current;
    //            if (null == temp)
    //            {
    //                continue;
    //            }
    //            tItem.Add(temp);
    //        }
    //        return tItem;
    //    }

    //    /// <summary>
    //    /// 获取一个泛型T的Array[T]集合
    //    /// </summary>
    //    /// <typeparam name="T">泛型类</typeparam>
    //    /// <param name="i">输入的</param>
    //    /// <returns>返回一个泛型T的Array[T]集合</returns>
    //    public static T[] GetTArray<T>(this IEnumerable @i) where T : class
    //    {
    //        List<T> tItem = GetTList<T>(@i);
    //        if (tItem != null && tItem.Count > 0)
    //        {
    //            return GetTList<T>(@i).ToArray();
    //        }
    //        return new T[0];
    //    }

    //    /// <summary>
    //    /// 判断字符是否为空
    //    /// </summary>
    //    /// <param name="s"></param>
    //    /// <returns>true-为空，false-非空</returns>
    //    public static bool IsNullOrEmpty(this string @s)
    //    {
    //        if (@s == "" || String.IsNullOrEmpty(@s))
    //        {
    //            return true;
    //        }
    //        return false;
    //    }

    //    /// <summary>
    //    /// 判断字符不为空
    //    /// </summary>
    //    /// <param name="s"></param>
    //    /// <returns>true-为空，false-非空</returns>
    //    public static bool IsNotEmpty(this string @s)
    //    {
    //        if (@s == "" || String.IsNullOrEmpty(@s))
    //        {
    //            return false;
    //        }
    //        return true;
    //    }

    //    /// <summary>
    //    /// 获取属性的值
    //    /// </summary>
    //    /// <param name="source">源</param>
    //    /// <param name="propertyName">属性名称</param>
    //    /// <returns>返回属性的值</returns>
    //    public static object GetPropertyValue(this object @source, string propertyName)
    //    {
    //        try
    //        {
    //            if (String.IsNullOrEmpty(propertyName) || null == @source)
    //            {
    //                return null;
    //            }
    //            var pi = @source.GetType().GetProperty(propertyName);
    //            if (null == pi)
    //            {
    //                return null;
    //            }
    //            return pi.GetValue(@source, null);
    //        }
    //        catch
    //        {
    //            return null;
    //        }
    //    }

    //    /// <summary>
    //    /// 设置属性的值
    //    /// </summary>
    //    /// <param name="source">源</param>
    //    /// <param name="propertyName">属性名称</param>
    //    /// <param name="value">要设置的值</param>
    //    public static void SetPropertyValue(this object @source, string propertyName, object value)
    //    {
    //        try
    //        {
    //            if (String.IsNullOrEmpty(propertyName) || null == @source)
    //            {
    //                return;
    //            }
    //            var pi = @source.GetType().GetProperty(propertyName);
    //            if (null == pi)
    //            {
    //                return;
    //            }
    //            if (pi.PropertyType.IsEnum)     //-->是否是枚举值
    //            {
    //                pi.SetValue(@source, Enum.Parse(pi.PropertyType, string.Format("{0}", value), true), null);
    //            }
    //            else
    //            {
    //                pi.SetValue(@source, Convert.ChangeType(value, pi.PropertyType, null), null);
    //            }
    //        }
    //        catch { }
    //    }

    //    /// <summary>
    //    /// 克隆一个类,自己克隆自己,值也被克隆了。
    //    /// </summary>
    //    /// <typeparam name="T"></typeparam>
    //    /// <param name="source"></param>
    //    /// <returns></returns>
    //    public static T Clone<T>(this T @source) where T : new()
    //    {
    //        return @source.CloneTo<T>();
    //    }

    //    /// <summary>
    //    /// 克隆对象方法【此克隆类名不同，但必须类的属性名称都一样，才行，否则克隆出来的数据为空】
    //    /// </summary>
    //    /// <typeparam name="T"></typeparam>
    //    /// <param name="Source"></param>
    //    /// <returns></returns>
    //    public static T CloneTo<T>(this object @Source)
    //    {
    //        try
    //        {
    //            if (null == @Source)
    //            {
    //                return default(T);
    //            }

    //            //-->先获取@Source的属性
    //            Type old_tt = @Source.GetType();
    //            PropertyInfo[] old_pi_array = old_tt.GetProperties();

    //            Type clone_tt = typeof(T);
    //            Object _CloneObj = Activator.CreateInstance<T>();
    //            foreach (var v in old_pi_array)
    //            {
    //                object value = v.GetValue(@Source, null);
    //                PropertyInfo pi = clone_tt.GetProperty(v.Name, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public);
    //                if (null == pi)
    //                {
    //                    continue;
    //                }
    //                try
    //                {
    //                    pi.SetValue(_CloneObj, Convert.ChangeType(value, pi.PropertyType, null), null);
    //                }
    //                catch
    //                {
    //                }
    //            }
    //            return (T)_CloneObj;
    //        }
    //        catch
    //        {
    //            return default(T);
    //        }
    //    }

    //    /// <summary>
    //    /// 克隆对象方法【此克隆类名不同，但必须类的属性名称都一样，才行，否则克隆出来的数据为空】
    //    /// </summary>
    //    /// <typeparam name="T"></typeparam>
    //    /// <param name="Source"></param>
    //    /// <returns></returns>
    //    public static List<T> CloneTo<T>(this IList @Source)
    //    {
    //        if (null == @Source)
    //        {
    //            return null;
    //        }
    //        List<T> item = new List<T>();
    //        foreach (var v in @Source)
    //        {
    //            item.Add(v.CloneTo<T>());
    //        }
    //        return item;
    //    }

    //    /// <summary>
    //    /// 克隆数组
    //    /// </summary>
    //    /// <typeparam name="T"></typeparam>
    //    /// <param name="Source"></param>
    //    /// <returns></returns>
    //    public static IList<T> CloneTo<T>(this IEnumerable @Source)
    //    {
    //        if (null == @Source)
    //        {
    //            return null;
    //        }
    //        IList<T> item = new List<T>();
    //        var ie = @Source.GetEnumerator();
    //        while (ie.MoveNext())
    //        {
    //            object current = ie.Current;
    //            if (null == current)
    //            {
    //                continue;
    //            }
    //            T temp = current.CloneTo<T>();
    //            item.Add(temp);
    //        }
    //        return item;
    //    }

    //    /// <summary>
    //    /// 对象克隆自己
    //    /// </summary>
    //    /// <param name="Source">要克隆的对象</param>
    //    /// <returns>返回克隆后的对象</returns>
    //    public static object CloneSelf(this object @Source)
    //    {
    //        return CloneSelf(@Source, true);
    //    }

    //    /// <summary>
    //    /// 对象克隆自己
    //    /// </summary>
    //    /// <param name="Source">要克隆的对象</param>
    //    /// <param name="isSaveValue">克隆对象是，是否把数据同时也保存起来</param>
    //    /// <returns>返回克隆后的对象</returns>
    //    public static object CloneSelf(this object @Source, bool isSaveValue)
    //    {
    //        if (null == @Source)
    //        {
    //            return null;
    //        }
    //        if (@Source is IConvertible)
    //        {
    //            return @Source;
    //        }

    //        Type tt = @Source.GetType();
    //        //-->获取所有属性
    //        PropertyInfo[] piItem = tt.GetProperties();
    //        TypeBuilder tb = GetTypeBuilder(DateTime.Now.ToString("yyyyMMddHHmmss"));
    //        //-->动态类的构造函数
    //        ConstructorBuilder constructor = tb.DefineDefaultConstructor(MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName);
    //        foreach (var pair in piItem)
    //        {
    //            //-->创建属性
    //            CreateProperty(tb, pair.Name, pair.PropertyType);
    //        }
    //        Type objT = tb.CreateType();
    //        object tempObj = Activator.CreateInstance(objT);
    //        if (isSaveValue == true && tempObj != null)
    //        {
    //            PropertyInfo[] tempPIItem = objT.GetProperties();
    //            foreach (var v in tempPIItem)
    //            {
    //                v.SetValue(tempObj, tt.GetProperty(v.Name).GetValue(@Source, null), null);
    //            }
    //        }
    //        return tempObj;
    //    }

    //    /// <summary>
    //    /// 克隆对象方法【此克隆类名不同，但必须类的属性名称都一样，才行，否则克隆出来的数据为空】
    //    /// </summary>
    //    /// <typeparam name="T"></typeparam>
    //    /// <param name="Source"></param>
    //    /// <returns></returns>
    //    public static ObservableCollection<T> ObservableCollectionCloneTo<T>(this @IList Source)
    //    {
    //        if (null == @Source)
    //        {
    //            return null;
    //        }
    //        ObservableCollection<T> item = new ObservableCollection<T>();
    //        foreach (var v in @Source)
    //        {
    //            if (null == v)
    //            {
    //                continue;
    //            }
    //            T temp = v.CloneTo<T>();
    //            if (null == temp)
    //            {
    //                continue;
    //            }
    //            item.Add(temp);
    //        }
    //        return item;
    //    }

    //    /// <summary>
    //    /// 克隆到一个集合
    //    /// </summary>
    //    /// <typeparam name="T"></typeparam>
    //    /// <param name="source"></param>
    //    /// <returns></returns>
    //    public static List<T> CloneList<T>(this IEnumerable @source) where T : new()
    //    {
    //        try
    //        {
    //            if (@source == null)
    //            {
    //                return null;
    //            }
    //            List<T> tItem = new List<T>();
    //            IEnumerator ie = @source.GetEnumerator();
    //            while (ie.MoveNext())
    //            {
    //                T temp = ie.Current.CloneTo<T>();
    //                if (null == temp)
    //                {
    //                    continue;
    //                }
    //                tItem.Add(temp);
    //            }
    //            return tItem;
    //        }
    //        catch
    //        {
    //            return null;
    //        }
    //    }

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <typeparam name="T"></typeparam>
    //    /// <param name="source"></param>
    //    /// <returns></returns>
    //    public static Collection<T> CloneCollection<T>(this IEnumerable @source) where T : new()
    //    {
    //        try
    //        {
    //            if (@source == null)
    //            {
    //                return null;
    //            }
    //            Collection<T> oCollection = new Collection<T>();
    //            IEnumerator ie = @source.GetEnumerator();
    //            while (ie.MoveNext())
    //            {
    //                T result = ie.Current.CloneTo<T>();
    //                if (null == result)
    //                {
    //                    continue;
    //                }
    //                oCollection.Add(result);
    //            }
    //            return oCollection;
    //        }
    //        catch
    //        {
    //            return null;
    //        }
    //    }

    //    /// <summary>
    //    /// 自已克隆自己
    //    /// </summary>
    //    /// <typeparam name="T"></typeparam>
    //    /// <param name="source"></param>
    //    /// <returns></returns>
    //    public static ObservableCollection<T> CloneObservableCollection<T>(this IEnumerable @source) where T : new()
    //    {
    //        try
    //        {
    //            if (@source == null)
    //            {
    //                return null;
    //            }
    //            ObservableCollection<T> oCollection = new ObservableCollection<T>();
    //            IEnumerator ie = @source.GetEnumerator();
    //            while (ie.MoveNext())
    //            {
    //                T result = ie.Current.CloneTo<T>();
    //                if (null == result)
    //                {
    //                    continue;
    //                }
    //                oCollection.Add(result);
    //            }
    //            return oCollection;
    //        }
    //        catch
    //        {
    //            return null;
    //        }
    //    }

    //    /// <summary>
    //    /// 克隆到一个数组
    //    /// </summary>
    //    /// <typeparam name="T"></typeparam>
    //    /// <param name="source"></param>
    //    /// <returns></returns>
    //    public static T[] CloneArray<T>(this IEnumerable @source) where T : new()
    //    {
    //        List<T> tItem = CloneList<T>(@source);

    //        if (tItem != null && tItem.Count > 0)
    //        {
    //            return tItem.ToArray();
    //        }
    //        return null;
    //    }

    //    #region --> 将 IEnumerable<IDictionary> 转成 IEnumerable 类型
    //    /// <summary>
    //    /// 表达试
    //    /// </summary>
    //    private static readonly Regex PropertNameRegex = new Regex(@"^[A-Za-z]+[A-Za-z0-9_]*$", RegexOptions.Singleline);
    //    /// <summary>
    //    /// 签名类型字典
    //    /// </summary>
    //    private static readonly Dictionary<string, Type> _typeBySigniture = new Dictionary<string, Type>();

    //    /// <summary>
    //    /// 转换成DataSource
    //    /// </summary>
    //    /// <param name="list">字典列表</param>
    //    /// <returns>返回一个IEnumerable对象</returns>
    //    public static IEnumerable ToDataSource(this IEnumerable<IDictionary> @list)
    //    {
    //        if (@list == null)
    //        {
    //            return null;
    //        }

    //        IDictionary firstDict = null;
    //        bool hasData = false;

    //        //firstDict = list.GetFirst<IDictionary>();
    //        foreach (IDictionary currentDict in list)
    //        {
    //            hasData = true;
    //            firstDict = currentDict;
    //            break;
    //        }

    //        if (!hasData)
    //        {
    //            return new object[] { };
    //        }

    //        if (firstDict == null)
    //        {
    //            return null;
    //        }

    //        string typeSigniture = GetTypeSigniture(firstDict);
    //        Type objectType = GetTypeByTypeSigniture(typeSigniture);
    //        if (objectType == null)
    //        {
    //            //-->
    //            TypeBuilder tb = GetTypeBuilder(typeSigniture);
    //            //-->动态类的构造函数
    //            ConstructorBuilder constructor = tb.DefineDefaultConstructor(MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName);
    //            //-->
    //            foreach (DictionaryEntry pair in firstDict)
    //            {
    //                //-->判断属性列名是否合法
    //                if (PropertNameRegex.IsMatch(Convert.ToString(pair.Key), 0))
    //                {
    //                    //-->创建属性
    //                    CreateProperty(tb, Convert.ToString(pair.Key), GetValueType(pair.Value));
    //                }
    //                else
    //                {
    //                    throw new ArgumentException(@"IDictionary的每个键必须是字母数字和字符开始。");
    //                }
    //            }
    //            objectType = tb.CreateType();

    //            _typeBySigniture.Add(typeSigniture, objectType);
    //        }

    //        return GenerateEnumerable(objectType, list, firstDict);
    //    }

    //    /// <summary>
    //    /// 根据类型签名获取类型
    //    /// </summary>
    //    /// <param name="typeSigniture">类型签名</param>
    //    /// <returns></returns>
    //    private static Type GetTypeByTypeSigniture(string typeSigniture)
    //    {
    //        Type type;
    //        return _typeBySigniture.TryGetValue(typeSigniture, out type) ? type : null;
    //    }

    //    /// <summary>
    //    /// 获取值类型
    //    /// </summary>
    //    /// <param name="value"></param>
    //    /// <returns></returns>
    //    private static Type GetValueType(object value)
    //    {
    //        return value == null ? typeof(object) : value.GetType();
    //    }

    //    /// <summary>
    //    /// 获取类型签名
    //    /// </summary>
    //    /// <param name="firstDict"></param>
    //    /// <returns></returns>
    //    private static string GetTypeSigniture(IDictionary firstDict)
    //    {
    //        StringBuilder sb = new StringBuilder();
    //        foreach (DictionaryEntry pair in firstDict)
    //        {
    //            sb.AppendFormat("_{0}_{1}", pair.Key, GetValueType(pair.Value));
    //        }
    //        return sb.ToString().GetHashCode().ToString().Replace("-", "Minus");
    //    }

    //    /// <summary>
    //    /// 生成 IEnumerable
    //    /// </summary>
    //    /// <param name="objectType"></param>
    //    /// <param name="list"></param>
    //    /// <param name="firstDict"></param>
    //    /// <returns></returns>
    //    private static IEnumerable GenerateEnumerable(Type objectType, IEnumerable<IDictionary> list, IDictionary firstDict)
    //    {
    //        var listType = typeof(List<>).MakeGenericType(new[] { objectType });
    //        var listOfCustom = Activator.CreateInstance(listType);
    //        foreach (var currentDict in list)
    //        {
    //            if (null == currentDict)
    //            {
    //                //throw new ArgumentException("IDictionary entry cannot be null");
    //                continue;
    //            }
    //            var row = Activator.CreateInstance(objectType);
    //            foreach (DictionaryEntry pair in firstDict)
    //            {
    //                if (currentDict.Contains(pair.Key))
    //                {
    //                    PropertyInfo property = objectType.GetProperty(Convert.ToString(pair.Key));
    //                    property.SetValue(row, Convert.ChangeType(currentDict[pair.Key], property.PropertyType, null), null);
    //                }
    //            }
    //            listType.GetMethod("Add").Invoke(listOfCustom, new[] { row });
    //        }
    //        return listOfCustom as IEnumerable;
    //    }

    //    /// <summary>
    //    /// 在运行时定义并创建类的新实例
    //    /// </summary>
    //    /// <param name="typeSigniture"></param>
    //    /// <returns></returns>
    //    private static TypeBuilder GetTypeBuilder(string typeSigniture)
    //    {
    //        AssemblyName an = new AssemblyName("TempAssembly" + typeSigniture);
    //        AssemblyBuilder assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(an, AssemblyBuilderAccess.Run);
    //        ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("MainModule");
    //        TypeBuilder tb = moduleBuilder.DefineType("TempType" + typeSigniture, TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.AutoClass | TypeAttributes.AnsiClass | TypeAttributes.BeforeFieldInit | TypeAttributes.AutoLayout, typeof(object));
    //        return tb;
    //    }

    //    /// <summary>
    //    /// 创建属性
    //    /// </summary>
    //    /// <param name="tb"></param>
    //    /// <param name="propertyName"></param>
    //    /// <param name="propertyType"></param>
    //    private static void CreateProperty(TypeBuilder tb, string propertyName, Type propertyType)
    //    {
    //        FieldBuilder fieldBuilder = tb.DefineField("_" + propertyName, propertyType, FieldAttributes.Private);
    //        PropertyBuilder propertyBuilder = tb.DefineProperty(propertyName, PropertyAttributes.HasDefault, propertyType, null);
    //        MethodBuilder getPropMthdBldr = tb.DefineMethod("get_" + propertyName, MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, propertyType, Type.EmptyTypes);
    //        ILGenerator getIL = getPropMthdBldr.GetILGenerator();
    //        getIL.Emit(OpCodes.Ldarg_0);
    //        getIL.Emit(OpCodes.Ldfld, fieldBuilder);
    //        getIL.Emit(OpCodes.Ret);
    //        MethodBuilder setPropMthdBldr = tb.DefineMethod("set_" + propertyName, MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, null, new Type[] { propertyType });
    //        ILGenerator setIL = setPropMthdBldr.GetILGenerator();
    //        setIL.Emit(OpCodes.Ldarg_0);
    //        setIL.Emit(OpCodes.Ldarg_1);
    //        setIL.Emit(OpCodes.Stfld, fieldBuilder);
    //        setIL.Emit(OpCodes.Ret);
    //        propertyBuilder.SetGetMethod(getPropMthdBldr);
    //        propertyBuilder.SetSetMethod(setPropMthdBldr);
    //    }

    //    #endregion --> 将 IEnumerable<IDictionary> 转成 IEnumerable 类型

    //    /// <summary>
    //    /// 获取属性值
    //    /// </summary>
    //    /// <param name="xe"></param>
    //    /// <param name="name"></param>
    //    /// <returns></returns>
    //    public static string GetAttributeValue(this XElement @xe, string name)
    //    {
    //        try
    //        {
    //            name = name.Trim();
    //            string value = xe == null ? null : (xe.Attribute(name) != null ? xe.Attribute(name).Value : null);
    //            return value;
    //        }
    //        catch
    //        {
    //            return "";
    //        }
    //    }

    //    /// <summary>
    //    /// 获取元素值
    //    /// </summary>
    //    /// <param name="xe"></param>
    //    /// <param name="name"></param>
    //    /// <returns></returns>
    //    public static string GetElementValue(this XElement @xe, string name)
    //    {
    //        string value = xe == null ? null : (xe.Element(name) != null ? xe.Element(name).Value.Trim() : null);
    //        return value;
    //    }

    //    /// <summary>
    //    /// 序列化
    //    /// </summary>
    //    /// <param name="obj">序列化对象</param>
    //    /// <returns>返回序列化后的字符串</returns>
    //    public static string Serializer(this object obj)
    //    {
    //        if (obj == null)
    //        {
    //            return "";
    //        }
    //        try
    //        {
    //            StringBuilder xml = new StringBuilder();
    //            Type t = obj.GetType();
    //            XmlSerializer xmlSerializer = new XmlSerializer(t);
    //            XmlWriterSettings xs = new XmlWriterSettings();
    //            xs.Encoding = Encoding.UTF8;
    //            XmlWriter xw = XmlWriter.Create(xml, xs);
    //            try
    //            {
    //                xmlSerializer.Serialize(xw, obj);
    //                xw.Close();
    //            }
    //            catch
    //            {
    //                try
    //                {
    //                    xw.Close();
    //                }
    //                catch
    //                {
    //                }
    //            }
    //            return XDocument.Parse(xml.ToString()).ToString();
    //        }
    //        catch { }
    //        return "";
    //    }

    //    /// <summary>
    //    /// 反序列化
    //    /// </summary>
    //    /// <typeparam name="T">要转换成的对象</typeparam>
    //    /// <param name="xmlString">xml内容</param>
    //    /// <returns>返回T对象</returns>
    //    public static T Deserialize<T>(this string xmlString) //where T : object
    //    {
    //        try
    //        {
    //            //xml反序列化                     
    //            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(xmlString));
    //            XmlSerializer xml = new XmlSerializer(typeof(T));
    //            T pTest = (T)xml.Deserialize(ms);                   //xml反序列化的关键代码
    //            ms.Close();
    //            return pTest;
    //        }
    //        catch
    //        {
    //            return default(T);
    //        }
    //    }

    //    /// <summary>
    //    /// 只能执行公开的方法，私有方法不行。
    //    /// </summary>
    //    /// <param name="obj"></param>
    //    /// <param name="methodName"></param>
    //    /// <param name="param"></param>
    //    /// <returns></returns>
    //    public static object InvokeMethod(this object obj, string methodName, object param)
    //    {
    //        Type tt = obj.GetType();
    //        MethodInfo method = tt.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.CreateInstance);
    //        if (null == method)
    //        {
    //            return null;
    //        }
    //        return method.Invoke(obj, new object[] { param });
    //    }

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <typeparam name="T"></typeparam>
    //    /// <param name="value"></param>
    //    /// <param name="fieldName"></param>
    //    /// <param name="fieldValue"></param>
    //    public static void Remove<T>(this List<T> @value, string fieldName, string fieldValue) where T : new()
    //    {
    //        List<T> item = new List<T>();
    //        foreach (var v in @value)
    //        {
    //            if (IsExists(v, fieldName, fieldValue))
    //            {
    //                item.Add(v);
    //            }
    //        }
    //        if (0 == item.Count())
    //        {
    //            return;
    //        }
    //        foreach (var a in item)
    //        {
    //            @value.Remove(a);
    //        }
    //    }

    //    static bool IsExists(object p, string fieldName, string fieldValue)
    //    {
    //        Type tt = p.GetType();
    //        PropertyInfo pi = tt.GetProperty(fieldName);
    //        if (null == pi)
    //        {
    //            return false;
    //        }
    //        string value = string.Format("{0}", pi.GetValue(p, null));
    //        if (value.Equals(fieldValue))
    //        {
    //            return true;
    //        }
    //        return false;
    //    }
    //}
}
