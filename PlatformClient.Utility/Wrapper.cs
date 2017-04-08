using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Xml.Linq;
using PlatformClient.Extend.Core;
using PlatformClient.Model.Method;
using PlatformClient.Utility.ParseXml;

namespace PlatformClient.Utility
{

    /// <summary>
    /// 封装类
    /// </summary>
    public class Wrapper
    {
        /// <summary>
        /// 结构函数
        /// </summary>
        static Wrapper()
        {
            LoadDll();
        }

        /// <summary>
        /// 生成验证码
        /// </summary>
        /// <param name="autoCodeLength">生成几位验证码</param>
        /// <param name="source">图片控件</param>
        /// <param name="imageHeight">高</param>
        /// <param name="imageWidth">宽</param>
        public static string BuildAuthenticode(Image source, int autoCodeLength, int imageHeight, int imageWidth)
        {
            if (null == source || 0 == imageWidth || 0 == imageHeight)
            {
                return string.Empty;
            }
            AuthenticodeHelper icc = new AuthenticodeHelper();
            var autoCode = icc.CreateIndentifyCode(autoCodeLength);
            icc.CreatIndentifyCodeImage(autoCode, source, imageWidth, imageHeight);
            return autoCode;
        }
        /// <summary>
        /// 生成验证码(默认生成4位验证码)
        /// </summary>
        /// <param name="source">图片控件</param>
        /// <param name="imageHeight">高</param>
        /// <param name="imageWidth">宽</param>
        public static string BuildAuthenticode(Image source, int imageHeight, int imageWidth)
        {
            return BuildAuthenticode(source, 4, imageHeight, imageWidth);
        }

        /// <summary>
        /// 将xaml转控件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xaml"></param>
        /// <returns></returns>
        public static T LoadXaml<T>(string xaml) where T : UIElement
        {
            try
            {
                return XamlReader.Load(xaml) as T;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 创建控件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="controlType"></param>
        /// <returns></returns>
        public static T CreateCtrl<T>(string controlType) where T : FrameworkElement
        {
            List<String> assItem = new List<string>();
            assItem.AddRange(ParseControlTemplate.GetControlTempletConfig("UserControl").AssemblyCollection);
            ControlConfig config = ParseControlConfig.GetControlConfig(controlType);
            //-->判断是不是复合控件
            if (config.IsCompositeCtrl && !config.AssemblyQualifiedName.IsNullOrEmpty())
            {
                assItem.Add(config.AssemblyQualifiedName.Trim());
            }
            string ass = string.Join(" ", assItem);
            ass = string.Format(config.xaml, ass);
            ass = ass.Replace("??", config.Name + DateTime.Now.ToString("ffffff"));
            ass = ass.Trim();

            T tt = LoadXaml<T>(ass);
            tt.Tag = config;
            return tt;
        }

        /// <summary>
        /// 修改xaml属性值
        /// </summary>
        /// <param name="modify"></param>
        /// <param name="controlXaml"></param>
        /// <returns></returns>
        public static string ModifyXamlAttribute(Dictionary<string, object> modify, string controlXaml)
        {
            string split = "_1_2_3_4_";
            controlXaml = controlXaml.Replace(":", split);
            XElement xaml = XElement.Parse(controlXaml);
            foreach (var v in modify)
            {
                if (null == xaml.Attribute(v.Key))
                {
                    xaml.Add(new XAttribute(v.Key, v.Value));
                    continue;
                }
                xaml.Attribute(v.Key).Value = string.Format("{0}", v.Value == null ? "" : v.Value);
            }
            controlXaml = xaml.ToString();
            controlXaml = controlXaml.Replace(split, ":");
            return controlXaml;
        }

        /// <summary>
        /// 修改xaml属性值
        /// </summary>
        /// <param name="modify"></param>
        /// <param name="controlXaml"></param>
        /// <returns></returns>
        public static string ModifyAttribute(string controlXaml, params KeyValuePair<String, Object>[] modify)
        {
            if (null == modify || 0 == modify.Length)
            {
                return controlXaml;
            }
            string split = "_1_2_3_4_";
            controlXaml = controlXaml.Replace(":", split);
            XElement xaml = XElement.Parse(controlXaml);
            foreach (var v in modify)
            {
                var att = xaml.Attribute(v.Key);
                if (null == att)
                {
                    xaml.Add(new XAttribute(v.Key, v.Value));
                    continue;
                }
                att.Value = string.Format("{0}", v.Value == null ? "" : v.Value);
            }
            controlXaml = xaml.ToString();
            controlXaml = controlXaml.Replace(split, ":");
            return controlXaml;
        }
        /// <summary>
        /// 修改xaml属性值如果属性没有就创建属性
        /// </summary>
        /// <param name="modify"></param>
        /// <param name="controlXaml"></param>
        /// <returns></returns>
        public static void ModifyAttribute(XElement controlXaml, params KeyValuePair<String, Object>[] modify)
        {
            if (null == modify || 0 == modify.Length)
            {
                return;
            }
            XElement xaml = controlXaml;
            foreach (var v in modify)
            {
                if (null == xaml.Attribute(v.Key))
                {
                    continue;
                }
                xaml.Attribute(v.Key).Value = string.Format("{0}", v.Value == null ? "" : v.Value);
            }
        }

        ///// <summary>
        ///// 将所有行转成T类型。
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="elementName">获取元素节点的名称</param>
        ///// <param name="result"></param>
        ///// <returns></returns>
        //public static List<T> ConvertToList<T>(XElement result, string elementName) where T : new()
        //{
        //    if (null == result || String.IsNullOrEmpty(elementName))
        //    {
        //        return null;
        //    }

        //    XElement dir = result.Element(elementName);
        //    if (null == dir)
        //    {
        //        return null;
        //    }
        //    XElement _table = dir.Element("rows");
        //    if (null == _table)
        //    {
        //        return null;
        //    }
        //    List<T> item = new List<T>();
        //    Type tt = typeof(T);
        //    PropertyInfo[] piItem = tt.GetProperties();
        //    foreach (var row in _table.Elements("row"))
        //    {
        //        T _new = new T();
        //        foreach (var pi in piItem)
        //        {
        //            object value = "";
        //            if (pi.PropertyType.FullName.Equals("System.Xml.Linq.XElement"))
        //            {
        //                var _ElementValue = row.GetElementValue(pi.Name.ToLower());
        //                if (_ElementValue.IsNullOrEmpty())
        //                {
        //                    continue;
        //                }
        //                try
        //                {
        //                    value = XElement.Parse(_ElementValue);
        //                }
        //                catch
        //                {
        //                    continue;
        //                }
        //            }
        //            else
        //            {
        //                string _ElementValue = row.GetAttributeValue(pi.Name.ToLower());
        //                if (_ElementValue.IsNullOrEmpty())
        //                {
        //                    _ElementValue = row.GetElementValue(pi.Name.ToLower());
        //                    if (_ElementValue.IsNullOrEmpty())
        //                    {
        //                        continue;
        //                    }
        //                }
        //                value = Convert.ChangeType(_ElementValue, pi.PropertyType, null);
        //            }
        //            try
        //            {
        //                pi.SetValue(_new, Convert.ChangeType(value, pi.PropertyType, null), null);
        //            }
        //            catch
        //            {
        //                continue;
        //            }
        //        }
        //        item.Add(_new);
        //    }
        //    return item;
        //}

        /// <summary>
        /// 将所有行转成T类型。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="result"></param>
        /// <returns></returns>
        public static List<T> ConvertToList<T>(XElement result) where T : new()
        {
            if (null == result)
            {
                return null;
            }

            XElement _Rows = result.Element("rows");
            if (null == _Rows)
            {
                return null;
            }
            List<T> item = new List<T>();
            Type tt = typeof(T);
            PropertyInfo[] piItem = tt.GetProperties();
            foreach (var row in _Rows.Elements("row"))
            {
                T _new = new T();
                foreach (var pi in piItem)
                {
                    object value = "";
                    if (pi.PropertyType.FullName.Equals("System.Xml.Linq.XElement"))
                    {
                        var _ElementValue = row.GetElementValue(pi.Name.ToLower());
                        if (_ElementValue.IsNullOrEmpty())
                        {
                            continue;
                        }
                        try
                        {
                            value = XElement.Parse(_ElementValue);
                        }
                        catch
                        {
                            continue;
                        }
                    }
                    else
                    {
                        string _ElementValue = row.GetAttributeValue(pi.Name.ToLower());
                        if (_ElementValue.IsNullOrEmpty())
                        {
                            _ElementValue = row.GetElementValue(pi.Name.ToLower());
                            if (_ElementValue.IsNullOrEmpty())
                            {
                                continue;
                            }
                        }
                        value = Convert.ChangeType(_ElementValue, pi.PropertyType, null);
                    }
                    try
                    {
                        pi.SetValue(_new, Convert.ChangeType(value, pi.PropertyType, null), null);
                    }
                    catch
                    {
                        continue;
                    }
                }
                item.Add(_new);
            }
            return item;
        }

        ///// <summary>
        ///// 将第一条记录转为T类型
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="elementName">获取元素节点的名称</param>
        ///// <param name="result"></param>
        ///// <returns></returns>
        //public static T ConvertToModel<T>(XElement result, string elementName) where T : new()
        //{
        //    List<T> item = ConvertToList<T>(result, elementName);
        //    if (null == item || 0 == item.Count)
        //    {
        //        return default(T);
        //    }
        //    return item[0];
        //}

        /// <summary>
        /// 将第一条记录转为T类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="result"></param>
        /// <returns></returns>
        public static T ConvertToModel<T>(XElement result) where T : new()
        {
            List<T> item = ConvertToList<T>(result);
            if (null == item || 0 == item.Count)
            {
                return default(T);
            }
            return item[0];
        }
        /// <summary>
        /// 格式是：
        /// sql;table
        /// {
        ///     sql = select * from table;
        ///     table = user_info;
        /// }
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static string ParseSqlSentence(string sql, string tableName)
        {
            return string.Format("{0};{1}", sql, tableName);
        }

        /// <summary>
        /// 调用Invoke方法，来执行事件
        /// </summary>
        /// <param name="cmd">命令对象</param>
        /// <param name="methodName">方法名称</param>
        /// <param name="obj">执行方法所在类</param>
        /// <returns></returns>
        public static object InvokeMethod(object cmd, string methodName, object obj)
        {
            Type tt = obj.GetType();
            MethodInfo method = tt.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.CreateInstance);
            if (null == method)
            {
                return null;
            }
            return method.Invoke(obj, new object[] { cmd });
        }

        /// <summary>
        /// GUID的值
        /// </summary>
        public static string GuidValue { get { return Guid.NewGuid().ToString("N").ToUpper(); } }

        /// <summary>
        /// 设置xml的值
        /// </summary>
        /// <param name="nodeName">节点名称</param>
        /// <param name="functionName">方法名称(可以为空)</param>
        /// <param name="dictAttribute">属性</param>
        /// <returns></returns>
        public static XElement SetXmlValue(string nodeName, string functionName, params KeyValuePair<String, Object>[] dictAttribute)
        {
            XElement xml = new XElement(string.Format("{0}", nodeName.IsNotEmpty() ? nodeName : "TempNode"));
            if (!functionName.IsNullOrEmpty())
            {
                xml.Add(new XAttribute("FunctionName", functionName));
            }
            if (null != dictAttribute && 0 < dictAttribute.Length)
            {
                foreach (var v in dictAttribute)
                {
                    xml.Add(new XAttribute(v.Key, v.Value));
                }
            }
            return xml;
        }

        /// <summary>
        /// 设置xml的值
        /// </summary>
        /// <param name="nodeName">节点名称</param>
        /// <param name="functionName">方法名称(可以为空)</param>
        /// <param name="dictAttribute">属性</param>
        /// <param name="content">内容 会用 XCData 存放</param>
        /// <returns></returns>
        public static XElement SetXmlValue(string nodeName, string functionName, List<object> content, params KeyValuePair<String, Object>[] dictAttribute)
        {
            XElement xml = SetXmlValue(nodeName, functionName, dictAttribute);
            foreach (var v in content)
            {
                xml.Add(v is XElement ? v : new XCData(string.Format("{0}", v)));
            }
            return xml;
        }

        /// <summary>
        /// 弹出信息
        /// </summary>
        /// <param name="message"></param>
        public static void ShowDialog(string message)
        {
            MessageBox.Show(message);
        }

        /// <summary>
        /// 删除属性
        /// </summary>
        /// <param name="result"></param>
        /// <param name="list"></param>
        public static void RemoveAttribute(XElement result, params string[] list)
        {
            foreach (var str in list)
            {
                var att = result.Attribute(str);
                if (null == att)
                {
                    continue;
                }
                att.Remove();
            }
        }

        /// <summary>
        /// 调用事件
        /// </summary>
        /// <param name="eventControl">事件处理的控件对象</param>
        /// <param name="methodName">方法名称</param>
        /// <param name="nameSpace">名称空间</param>
        /// <param name="eventName">事件名称</param>
        public static void ControlRegisterEvent(object eventControl, string eventName, string nameSpace, string methodName)
        {
            try
            {
                if (null == eventControl || eventName.IsNullOrEmpty() || methodName.IsNullOrEmpty())
                {
                    return;
                }
                //创建一个类
                object classType = Wrapper.CreateInstance(nameSpace);
                if (null == classType)
                {
                    return;
                }
                //获取控件上的事件信息
                EventInfo evenInfo = eventControl.GetType().GetEvent(eventName);
                if (null == evenInfo)
                {
                    return;
                }
                MethodInfo methodHandler = classType.GetType().GetMethod(methodName, BindingFlags.Public | BindingFlags.Static);
                if (null == methodHandler)
                {
                    return;
                }//获取方法失败。

                //获取事件类型
                Type delegateType = evenInfo.EventHandlerType;
                //创建一个委托
                Delegate createDelegate = Delegate.CreateDelegate(delegateType, methodHandler);
                //事件要委托的方法
                MethodInfo miAddHandler = evenInfo.GetAddMethod(); //添加方法
                //事件句柄参数
                object[] addHandlerArgs = { createDelegate };
                //调用控件事件
                miAddHandler.Invoke(eventControl, addHandlerArgs);
                //获取委托返回类型
                Type returnType = GetDelegateReturnType(delegateType);
                if (null == returnType || returnType != typeof(void))
                {
                    return;
                }
                //动态方法
                DynamicMethod handler = new DynamicMethod("", null, GetDelegateParameterTypes(delegateType));
                ILGenerator ilgen = handler.GetILGenerator();
                Type[] showParameters = { typeof(string) };
                ilgen.Emit(OpCodes.Ldstr, ""/*parms*/);
                ilgen.Emit(OpCodes.Pop);
                ilgen.Emit(OpCodes.Ret);
                Delegate dEmitted = handler.CreateDelegate(delegateType);
                evenInfo.AddEventHandler(eventControl, dEmitted);
            }
            catch { }
        }

        /// <summary>
        /// 获取委托返回类型
        /// </summary>
        /// <param name="d">委托类型</param>
        /// <returns></returns>
        private static Type GetDelegateReturnType(Type d)
        {
            //是否是多播委托
            if (d.BaseType != typeof(MulticastDelegate))
            {
                Console.WriteLine("不是一个委托。");
                return null;
                //throw new InvalidOperationException("Not a delegate.");
            }
            //获取d里是否有Invoer方法。
            MethodInfo invoke = d.GetMethod("Invoke");
            if (null == invoke)
            {
                Console.WriteLine("不是一个委托。");
                return null;
                //throw new InvalidOperationException("Not a delegate.");
            }
            return invoke.ReturnType;
        }

        /// <summary>
        /// 获取委托参数
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        static Type[] GetDelegateParameterTypes(Type d)
        {
            if (d.BaseType != typeof(MulticastDelegate))
            {
                throw new InvalidOperationException("Not a delegate.");
            }

            MethodInfo invoke = d.GetMethod("Invoke");
            if (invoke == null)
            {
                throw new InvalidOperationException("Not a delegate.");
            }

            ParameterInfo[] parameters = invoke.GetParameters();
            Type[] typeParameters = new Type[parameters.Length];
            for (int i = 0; i < parameters.Length; i++)
            {
                typeParameters[i] = parameters[i].ParameterType;
            }

            return typeParameters;
        }

        static Dictionary<String, Assembly> DynLoadingAssemblyDict = new Dictionary<string, Assembly>();
        /// <summary>
        /// 加载所有程序集合
        /// </summary>
        public static void LoadDll()
        {
            AssemblyPartCollection coll = Deployment.Current.Parts;
            foreach (var v in coll)
            {
                if (!DynLoadingAssemblyDict.ContainsKey(v.Source))
                {
                    System.Windows.Resources.StreamResourceInfo sri = Application.GetResourceStream(new Uri(v.Source, UriKind.RelativeOrAbsolute));
                    if (null == sri)
                    {
                        continue;
                    }
                    Assembly ass = v.Load(sri.Stream);
                    DynLoadingAssemblyDict.Add(v.Source, ass);
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="assemblyName"></param>
        /// <returns></returns>
        public static Assembly AssemblyByName(string assemblyName)
        {
            if (DynLoadingAssemblyDict.ContainsKey(assemblyName))
            {
                return DynLoadingAssemblyDict[assemblyName];
            }
            return null;
        }
        /// <summary>
        /// 创建一个UIElemenet实例
        /// </summary>
        /// <param name="xaml"></param>
        /// <returns></returns>
        public static UIElement CreateUIElement(string xaml)
        {
            return CreateUIElement<UIElement>(xaml);
        }
        /// <summary>
        /// 创建一个UIElemenet实例
        /// </summary>
        /// <param name="xaml"></param>
        /// <returns></returns>
        public static T CreateUIElement<T>(string xaml) where T : UIElement
        {
            try
            {
                T ui = (T)XamlReader.Load(xaml);
                return ui;
            }
            catch { }
            return null;
        }

        /// <summary>
        /// 创建控件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="AssemblyQualifiedName">程序集;控件的命令空间</param>
        /// <returns></returns>
        public static T CreateControl<T>(string AssemblyQualifiedName) where T : UIElement
        {
            return (T)CreateInstance(AssemblyQualifiedName);
        }

        /// <summary>
        /// 从程序集中查找指定的类型，然后使用系统激活器创建它的实例
        /// </summary>
        /// <param name="AssemblyQualifiedName">程序集;控件的命令空间</param>
        /// <returns></returns>
        public static object CreateInstance(string AssemblyQualifiedName)
        {
            if (AssemblyQualifiedName.IsNullOrEmpty())
            {
                return null;
            }
            string[] assArray = AssemblyQualifiedName.Split(';');
            string assemblyName = assArray[0] + ".dll";
            string nameSpace = assArray[1];
            var ass = AssemblyByName(assemblyName);
            if (null == ass)
            {
                return null;
            }
            return ass.CreateInstance(nameSpace);
        }

        /// <summary>
        /// 读取控件属性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ui"></param>
        /// <returns></returns>
        public static List<T> ReadControlAttribute<T>(UIElement ui) where T : BaseAttribute
        {
            if (null == ui)
            {
                return new List<T>();
            }
            MemberInfo[] eventItem = ui.GetType().GetMembers();
            var result = from p in eventItem
                         where p.GetCustomAttributes(true).Length > 0 &&
                          (
                             from p1 in p.GetCustomAttributes(true)
                             where p1 is T
                             select p1
                         ).Count() > 0
                         select GetControlAttribute<T>(ui, p);
            return result.ToList();
        }
        /// <summary>
        /// 读取控件属性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ui"></param>
        /// <param name="ei"></param>
        /// <returns></returns>
        public static T GetControlAttribute<T>(UIElement ui, MemberInfo ei) where T : BaseAttribute
        {
            T eia = default(T);
            foreach (var v in ei.GetCustomAttributes(true))
            {
                if (!(v is T))
                {
                    continue;
                }
                eia = (T)v;
                eia.Source = ui;
                eia.Name = ei.Name;
                if (v is PropertyInfoAttribute)
                {
                    PropertyInfoAttribute pia = v as PropertyInfoAttribute;
                    PropertyInfo pi = ui.GetType().GetProperty(ei.Name);
                    eia.DefaultValue = pi.GetValue(ui, null);
                    pia.DataType = pi.PropertyType;
                }
                break;
            }
            return eia;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ui"></param>
        /// <returns></returns>
        public static List<PropertyInfoAttribute> CommonPropertyItem(UIElement ui)
        {
            List<PropertyInfoAttribute> item = new List<PropertyInfoAttribute>();

            Type tt = ui.GetType();
            ControlConfig cc = ParseControlConfig.GetControlConfig(tt.Name);
            if (null == cc)
            {
                return item;
            }

            FrameworkElement fe = ui as FrameworkElement;
            foreach (var v in cc.DictProperty)
            {
                //string[] a = v.Split(';');
                PropertyInfoAttribute pia = GetPropertyInfoAttribute(ui, tt, v.Key, v.Value);
                item.Add(pia);
            }
            return item;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ui"></param>
        /// <returns></returns>
        public static List<EventInfoAttribute> CommonEventItem(UIElement ui)
        {
            var result = new List<EventInfoAttribute>();
            if (null == ui)
            {
                return result;
            }
            Type tt = ui.GetType();
            ControlConfig cc = ParseControlConfig.GetControlConfig(tt.Name);
            if (null == cc || null == cc.Events)
            {
                return result;
            }
            foreach (var e in cc.Events)
            {
                EventInfoAttribute ea = new EventInfoAttribute(e.Name, e.AssemblyName, e.Description, e.EventDesigner);
                ea.Source = ui;
                result.Add(ea);
            }
            return result;
        }

        static PropertyInfoAttribute GetPropertyInfoAttribute(UIElement ui, Type tt, string propertyName, string description)
        {
            PropertyInfoAttribute pia = new PropertyInfoAttribute();
            PropertyInfo pi = tt.GetProperty(propertyName);
            if (null == pi)
            {
                return null;
            }
            pia.Name = propertyName;
            pia.Description = description;
            pia.DataType = pi.PropertyType;
            pia.DefaultValue = pi.GetValue(ui, null);
            return pia;
        }

        static PropertyInfoAttribute GetDependencyProperty(UIElement ui, DependencyProperty dp, string description)
        {
            PropertyInfoAttribute pia = new PropertyInfoAttribute();
            Type tt = dp.GetType();
            pia.Name = tt.Name;
            pia.Source = ui;
            pia.Description = description;
            pia.DataType = tt;
            pia.DefaultValue = ui.GetValue(dp);
            return pia;
        }

        /// <summary>
        /// 分析控件名称
        /// 如果是以Form开头的话，返回FormX控件来
        /// </summary>
        /// <param name="controlName"></param>
        /// <returns></returns>
        public static string ParseControlName(string controlName)
        {
            return controlName.Length > 4 &&
                    controlName.Substring(0, 4).Equals(ConstantCollection.FORM_PREFIX) &&
                    0 < controlName.Split('_').Length
                    ?
                    controlName.Split('_')[0]
                    : controlName;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="control"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static FrameworkElement FindControl(FrameworkElement control, string name)
        {
            if (null == control)
            {
                return null;
            }
            if (control.Name.ToLower().Equals(name.ToLower()))
            {
                return control;
            }
            object result = control.FindName(name);
            if (null != result)
            {
                return result as FrameworkElement;
            }

            Panel p = control as Panel;
            if (p != null)
            {
                foreach (var v in p.Children)
                {
                    result = ReturnFrameworkElement(v as object, name);
                    if (result != null)
                    {
                        return result as FrameworkElement;
                    }
                }
            }
            UserControl uc = control as UserControl;
            if (uc != null)
            {
                result = ReturnFrameworkElement(uc.Content, name);  // FindName(fe, name);
                if (result != null)
                {
                    return result as FrameworkElement;
                }
            }
            ScrollViewer uv = control as ScrollViewer;
            if (uv != null)
            {
                result = ReturnFrameworkElement(uv.Content, name);
                if (result != null)
                {
                    return result as FrameworkElement;
                }
            }
            Border b = control as Border;
            if (b != null)
            {
                result = ReturnFrameworkElement(b.Child, name);
                if (result != null)
                {
                    return result as FrameworkElement;
                }
            }
            return null;
        }
        static object ReturnFrameworkElement(object control, string name)
        {
            FrameworkElement fe = control as FrameworkElement;
            if (fe != null)
            {
                object result = FindControl(fe, name);
                if (result != null)
                {
                    return result;
                }
            }
            return null;
        }
        /// <summary>
        /// 将数据转成数据类型[值带'='号]
        /// </summary>
        /// <param name="mdci"></param>
        /// <returns></returns>
        public static string ConvertToDBTypeWhereUsing(MetaDataColumnInfo mdci)
        {
            if (null == mdci || mdci.data_type.IsNullOrEmpty())
            {
                return "";
            }
            switch (mdci.data_type.ToLower())
            {
                case "datetime":
                case "varchar":
                    if (mdci.column_value.IsNotEmpty())
                    {
                        //BETWEEN
                        if (mdci.column_value.Trim().Length > 7)
                        {
                            var subStr = mdci.column_value.Trim().Substring(0, 7).ToLower();
                            if (subStr.Equals("BETWEEN".ToLower()))
                            {
                                return mdci.column_value;
                            }
                        }
                        return string.Format("='{0}'", mdci.column_value);
                    }
                    //return string.Format("='{0}'", mdci.column_value);
                    return "";
                case "int":
                case "double":
                case "decimal":
                    //return string.Format("={0}", mdci.column_value);
                    if (mdci.column_value.IsNotEmpty())
                    {
                        //BETWEEN
                        if (mdci.column_value.Trim().Length > 7)
                        {
                            var subStr = mdci.column_value.Trim().Substring(0, 7).ToLower();
                            if (subStr.Equals("BETWEEN".ToLower()))
                            {
                                return mdci.column_value;
                            }
                        }
                        return string.Format("={0}", mdci.column_value);
                    }
                    return "0";
                //return string.Format("str_to_date('{0}','%Y-%m-%d %h:%i:%s')", mdci.column_value);
                default:
                    //return string.Format("='{0}'", mdci.column_value);
                    if (mdci.column_value.IsNotEmpty())
                    {
                        //BETWEEN
                        if (mdci.column_value.Trim().Length > 7)
                        {
                            var subStr = mdci.column_value.Trim().Substring(0, 7).ToLower();
                            if (subStr.Equals("BETWEEN".ToLower()))
                            {
                                return mdci.column_value;
                            }
                        }
                        return string.Format("='{0}'", mdci.column_value);
                    }
                    //return string.Format("='{0}'", mdci.column_value);
                    return "";
            }
        }

        /// <summary>
        /// 将数据转成数据类型
        /// </summary>
        /// <param name="mdci"></param>
        /// <returns></returns>
        public static string ConvertToDBType(MetaDataColumnInfo mdci)
        {
            if (null == mdci || mdci.data_type.IsNullOrEmpty())
            {
                return "";
            }
            switch (mdci.data_type.ToLower())
            {
                case "datetime":
                case "varchar":
                    return string.Format("'{0}'", mdci.column_value);
                case "int":
                case "double":
                case "decimal":
                    return mdci.column_value.ToString();
                //return string.Format("str_to_date('{0}','%Y-%m-%d %h:%i:%s')", mdci.column_value);
                default:
                    return string.Format("'{0}'", mdci.column_value);
            }
        }
    }
}
