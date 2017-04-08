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

namespace PlatformClient.Extend.Core
{
    /// <summary>
    /// 扩展方法类
    /// </summary>
    public static class ExtendMethod
    {
        /// <summary>
        /// 将对象的属性转成 XElement,只转属性部份，数组、泛型不转
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static XElement ToXElement(this object @s)
        {
            if (null == @s)
            {
                return null;
            }
            return ToXElement(@s, "");
        }

        /// <summary>
        /// 将对象的属性转成 XElement,只转属性部份，数组、泛型不转
        /// </summary>
        /// <param name="elementName"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public static XElement ToXElement(this object @source, string elementName)
        {
            if (null == @source)
            {
                return null;
            }

            Type tt = @source.GetType();
            //-->得到所有属性
            PropertyInfo[] piItem = tt.GetProperties();
            XElement _result = new XElement(elementName.IsNullOrEmpty() ? tt.Name : elementName);
            foreach (var pi in piItem)
            {
                if (pi.PropertyType.IsArray)
                {
                    continue;
                }
                else if (pi.PropertyType.FullName.Equals("System.Xml.Linq.XElement"))
                {
                    object _value = pi.GetValue(@source, null);
                    if (null != _value)
                    {
                        _result.Add(new XElement(pi.Name, new XCData(_value.ToString())));
                    }
                    continue;
                }
                else if (pi.PropertyType.IsGenericType)
                {
                    IList _Ilist = pi.GetValue(@source, null) as IList;
                    if (null == _Ilist || 0 == _Ilist.Count)
                    {
                        continue;
                    }
                    XElement _ChildXElement = new XElement(pi.Name, new XAttribute("Name", _Ilist[0].GetType().Name));
                    foreach (var v in _Ilist)
                    {
                        XElement _bb = v.ToXElement();
                        if (null == _bb)
                        {
                            continue;
                        }
                        _ChildXElement.Add(_bb);
                    }
                    _result.Add(_ChildXElement);
                }
                else if (pi.PropertyType.Name.Equals("Char"))
                {
                    _result.Add(new XAttribute(pi.Name, string.Format("{0}", @source)));
                }
                else
                {
                    object _value = pi.GetValue(@source, null);
                    if (null == _value)
                    {
                        continue;
                    }
                    if (pi.PropertyType.IsClass && pi.PropertyType.IsPublic
                        && !pi.PropertyType.IsPrimitive && !pi.PropertyType.IsValueType
                        && !(_value is IConvertible))
                    {
                        var _Class_XElement = _value.ToXElement();
                        _result.Add(_Class_XElement);
                    }
                    else
                    {
                        if (_value.ToString().IsNullOrEmpty())
                        {
                            continue;
                        }
                        _result.Add(new XAttribute(pi.Name, _value));
                    }
                }
            }
            return _result;
        }
        /// <summary>
        /// 将XElement转成 T 对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xElement"></param>
        /// <returns></returns>
        public static T ToModel<T>(this XElement @xElement) where T : new()
        {
            try
            {
                T _new = new T();
                _new = (T)@xElement.ToModel(_new);
                return _new;
            }
            catch
            {
                return default(T);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xElement"></param>
        /// <returns></returns>
        public static List<T> ToModelList<T>(this XElement @xElement) where T : new()
        {
            if (null == @xElement)
            {
                return null;
            }
            //-->获取字项是以什么开头的
            string subkeyName = @xElement.GetAttributeValue("Name");
            if (subkeyName.IsNullOrEmpty())
            {
                return null;
            }
            var subkeyItem = @xElement.Elements(subkeyName);
            if (null == subkeyItem || 0 == subkeyItem.Count())
            {
                return null;
            }
            List<T> item = new List<T>();
            foreach (var subkey in subkeyItem)
            {
                var data = subkey.ToModel<T>();
                if (null == data)
                {
                    continue;
                }
                item.Add(data);
            }
            return item;
        }

        static object ToModel(this XElement @xElement, object setValue)
        {
            if (null == @xElement || null == setValue)
            {
                return null;
            }
            Type tt = setValue.GetType();
            PropertyInfo[] piItem = tt.GetProperties();
            foreach (var pi in piItem)
            {
                var pt = pi.PropertyType;
                if (pt.IsArray)
                {
                    continue;
                }
                //-->判断是不是xml类型
                if (pt.FullName.Equals("System.Xml.Linq.XElement"))
                {
                    pi.SetValue(setValue, xElement, null);
                    continue;
                }
                //-->判断是不是集合
                if (pt.IsGenericType)
                {
                    var GenericItem = @xElement.Element(pi.Name);
                    if (null == GenericItem)
                    {
                        continue;
                    }
                    //-->创建一个泛型
                    object list = pt.InvokeMember(null,
                        BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.CreateInstance,
                        null, null, new object[] { });

                    //-->获取Add方法
                    MethodInfo mi = pt.GetMethod("Add");
                    var _singleType = pt.GetGenericArguments()[0];
                    string childName = GenericItem.GetAttributeValue("Name");
                    var childs = GenericItem.Elements(childName);
                    if (null == childs || 0 == childs.Count())
                    {
                        continue;
                    }
                    foreach (var v_child in childs)
                    {
                        //-->创建一个泛型类
                        object newC = Activator.CreateInstance(_singleType);
                        //-->给实例赋值
                        newC = v_child.ToModel(newC);
                        //-->将实例添加到泛型集合里去
                        mi.Invoke(list, new object[] { newC });
                    }
                    pi.SetValue(setValue, list, null);

                    continue;
                }
                string value = @xElement.GetAttributeValue(pi.Name);
                if (value.IsNullOrEmpty())
                {
                    value = @xElement.GetElementValue(pi.Name);
                    if (value.IsNullOrEmpty())
                    {
                        //-->判断是不是类
                        //if (pt.IsAnsiClass && pt.IsAutoLayout && !pt.IsPrimitive && pt.IsPublic && !pt.IsSealed && !pt.IsValueType && pt.IsVisible)
                        if (pt.IsClass && pt.IsPublic && !pt.IsPrimitive && !pt.IsValueType && pt.IsAutoLayout)
                        {
                            XElement _Element = @xElement.Element(pi.Name);
                            if (null == _Element)
                            {
                                _Element = @xElement.Element(pt.Name);
                            }
                            if (null == _Element)
                            {
                                continue;
                            }
                            //-->创建一个实例
                            var _NewClass = Activator.CreateInstance(pt);
                            _NewClass = _Element.ToModel(_NewClass);
                            try
                            {
                                pi.SetValue(setValue, _NewClass, null);
                            }
                            catch { }
                        }
                        continue;
                    }//End if; 不为空是，则里面的值一定是字符串类型
                }
                pi.SetValue(setValue, Convert.ChangeType(value, pt, null), null);
            }
            return setValue;
        }

        ///// <summary>
        ///// 删除属性
        ///// </summary>
        ///// <param name="s"></param>
        ///// <param name="list"></param>
        //internal static void RemoveAttribute(this XElement @s, params string[] list)
        //{
        //    foreach (var str in list)
        //    {
        //        var att = @s.Attribute(str);
        //        if (null == att)
        //        {
        //            continue;
        //        }
        //        att.Remove();
        //    }
        //}

        /// <summary>
        /// 判断字符是否为空
        /// </summary>
        /// <param name="s"></param>
        /// <returns>true-为空，false-非空</returns>
        public static bool IsNullOrEmpty(this string @s)
        {
            if (@s == "" || String.IsNullOrEmpty(@s))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 判断字符不为空
        /// </summary>
        /// <param name="s"></param>
        /// <returns>true-为空，false-非空</returns>
        public static bool IsNotEmpty(this string @s)
        {
            if (@s == "" || String.IsNullOrEmpty(@s))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 获取属性的值
        /// </summary>
        /// <param name="source">源</param>
        /// <param name="propertyName">属性名称</param>
        /// <returns>返回属性的值</returns>
        public static object GetPropertyValue(this object @source, string propertyName)
        {
            try
            {
                if (String.IsNullOrEmpty(propertyName) || null == @source)
                {
                    return null;
                }
                var pi = @source.GetType().GetProperty(propertyName);
                if (null == pi)
                {
                    return null;
                }
                return pi.GetValue(@source, null);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 设置属性的值
        /// </summary>
        /// <param name="source">源</param>
        /// <param name="propertyName">属性名称</param>
        /// <param name="value">要设置的值</param>
        public static void SetPropertyValue(this object @source, string propertyName, object value)
        {
            try
            {
                if (String.IsNullOrEmpty(propertyName) || null == @source)
                {
                    return;
                }
                var pi = @source.GetType().GetProperty(propertyName);
                if (null == pi)
                {
                    return;
                }
                if (pi.PropertyType.IsEnum)     //-->是否是枚举值
                {
                    pi.SetValue(@source, Enum.Parse(pi.PropertyType, string.Format("{0}", value), true), null);
                }
                else
                {
                    pi.SetValue(@source, Convert.ChangeType(value, pi.PropertyType, null), null);
                }
            }
            catch { }
        }

        /// <summary>
        /// 获取属性值
        /// </summary>
        /// <param name="xe"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetAttributeValue(this XElement @xe, string name)
        {
            try
            {
                name = name.Trim();
                string value = xe == null ? null : (xe.Attribute(name) != null ? xe.Attribute(name).Value : null);
                return value;
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// 获取元素值
        /// </summary>
        /// <param name="xe"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetElementValue(this XElement @xe, string name)
        {
            string value = xe == null ? null : (xe.Element(name) != null ? xe.Element(name).Value.Trim() : null);
            return value;
        }

        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name="obj">序列化对象</param>
        /// <returns>返回序列化后的字符串</returns>
        public static string Serializer(this object obj)
        {
            if (obj == null)
            {
                return "";
            }
            try
            {
                StringBuilder xml = new StringBuilder();
                Type t = obj.GetType();
                XmlSerializer xmlSerializer = new XmlSerializer(t);
                XmlWriterSettings xs = new XmlWriterSettings();
                xs.Encoding = Encoding.UTF8;
                XmlWriter xw = XmlWriter.Create(xml, xs);
                try
                {
                    xmlSerializer.Serialize(xw, obj);
                    xw.Close();
                }
                catch
                {
                    try
                    {
                        xw.Close();
                    }
                    catch
                    {
                    }
                }
                return XDocument.Parse(xml.ToString()).ToString();
            }
            catch { }
            return "";
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <typeparam name="T">要转换成的对象</typeparam>
        /// <param name="xmlString">xml内容</param>
        /// <returns>返回T对象</returns>
        public static T Deserialize<T>(this string xmlString) //where T : object
        {
            try
            {
                //xml反序列化                     
                MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(xmlString));
                XmlSerializer xml = new XmlSerializer(typeof(T));
                T pTest = (T)xml.Deserialize(ms);                   //xml反序列化的关键代码
                ms.Close();
                return pTest;
            }
            catch
            {
                return default(T);
            }
        }

        /// <summary>
        /// 只能执行公开的方法，私有方法不行。
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="methodName"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static object InvokeMethod(this object obj, string methodName, object param)
        {
            Type tt = obj.GetType();
            MethodInfo method = tt.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.CreateInstance);
            if (null == method)
            {
                return null;
            }
            return method.Invoke(obj, new object[] { param });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="fieldName"></param>
        /// <param name="fieldValue"></param>
        public static void Remove<T>(this List<T> @value, string fieldName, string fieldValue) where T : new()
        {
            List<T> item = new List<T>();
            foreach (var v in @value)
            {
                if (IsExists(v, fieldName, fieldValue))
                {
                    item.Add(v);
                }
            }
            if (0 == item.Count())
            {
                return;
            }
            foreach (var a in item)
            {
                @value.Remove(a);
            }
        }

        static bool IsExists(object p, string fieldName, string fieldValue)
        {
            Type tt = p.GetType();
            PropertyInfo pi = tt.GetProperty(fieldName);
            if (null == pi)
            {
                return false;
            }
            string value = string.Format("{0}", pi.GetValue(p, null));
            if (value.Equals(fieldValue))
            {
                return true;
            }
            return false;
        }
    }
}
