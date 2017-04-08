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
    /// 克隆
    /// </summary>
    public static class ExtendClone
    {
        /// <summary>
        /// 泛型类型转换
        /// </summary>
        /// <typeparam name="T">要转换的类型</typeparam>
        /// <param name="s">要转的内容</param>
        /// <returns>返回转换后的数据类型值</returns>
        public static T ConvertTo<T>(this IConvertible @s)
        {
            try
            {
                if (String.IsNullOrEmpty(string.Format("{0}", @s)))
                {
                    return default(T);
                }
                Type t = typeof(T);
                if (t.IsGenericType)
                {
                    Type gt = t.GetGenericTypeDefinition();
                    if (typeof(Nullable<>) == gt)
                    {
                        return (T)Convert.ChangeType(@s, Nullable.GetUnderlyingType(t), null);
                    }
                }
                return (T)Convert.ChangeType(@s, t, null);
            }
            catch
            {
                return default(T);
            }
        }


        /// <summary>
        /// 克隆一个类,自己克隆自己,值也被克隆了。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static T Clone<T>(this T @source) where T : new()
        {
            return @source.CloneTo<T>();
        }
        /// <summary>
        /// 克隆枚举
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static T CloneEnum<T>(this Enum @source)
        {
            return (T)Enum.Parse(typeof(T), string.Format("{0}", @source), true);
        }

        /// <summary>
        /// 克隆对象方法【此克隆类名不同，但必须类的属性名称都一样，才行，否则克隆出来的数据为空】
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Source"></param>
        /// <returns></returns>
        public static T CloneTo<T>(this object @Source)
        {
            try
            {
                if (null == @Source)
                {
                    return default(T);
                }

                //-->先获取@Source的属性
                Type old_tt = @Source.GetType();
                PropertyInfo[] old_pi_array = old_tt.GetProperties();

                Type clone_tt = typeof(T);
                Object _CloneObj = Activator.CreateInstance<T>();
                foreach (var v in old_pi_array)
                {
                    object value = v.GetValue(@Source, null);
                    PropertyInfo pi = clone_tt.GetProperty(v.Name, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public);
                    if (null == pi)
                    {
                        continue;
                    }
                    try
                    {
                        if (pi.PropertyType.IsEnum)
                        {
                            pi.SetValue(_CloneObj, Enum.Parse(pi.PropertyType, string.Format("{0}", value), true), null);
                        }
                        else
                        {
                            pi.SetValue(_CloneObj, Convert.ChangeType(value, pi.PropertyType, null), null);
                        }
                        //pi.SetValue(_CloneObj, Convert.ChangeType(value, pi.PropertyType, null), null);
                    }
                    catch
                    {
                    }
                }
                return (T)_CloneObj;
            }
            catch
            {
                return default(T);
            }
        }

        /// <summary>
        /// 克隆对象方法【此克隆类名不同，但必须类的属性名称都一样，才行，否则克隆出来的数据为空】
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Source"></param>
        /// <returns></returns>
        public static List<T> CloneTo<T>(this IList @Source)
        {
            if (null == @Source)
            {
                return null;
            }
            List<T> item = new List<T>();
            foreach (var v in @Source)
            {
                item.Add(v.CloneTo<T>());
            }
            return item;
        }

        /// <summary>
        /// 克隆数组
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Source"></param>
        /// <returns></returns>
        public static IList<T> CloneTo<T>(this IEnumerable @Source)
        {
            if (null == @Source)
            {
                return null;
            }
            IList<T> item = new List<T>();
            var ie = @Source.GetEnumerator();
            while (ie.MoveNext())
            {
                object current = ie.Current;
                if (null == current)
                {
                    continue;
                }
                T temp = current.CloneTo<T>();
                item.Add(temp);
            }
            return item;
        }

        /// <summary>
        /// 对象克隆自己
        /// </summary>
        /// <param name="Source">要克隆的对象</param>
        /// <returns>返回克隆后的对象</returns>
        public static object CloneSelf(this object @Source)
        {
            return CloneSelf(@Source, true);
        }

        /// <summary>
        /// 对象克隆自己
        /// </summary>
        /// <param name="Source">要克隆的对象</param>
        /// <param name="isSaveValue">克隆对象是，是否把数据同时也保存起来</param>
        /// <returns>返回克隆后的对象</returns>
        public static object CloneSelf(this object @Source, bool isSaveValue)
        {
            if (null == @Source)
            {
                return null;
            }
            if (@Source is IConvertible)
            {
                return @Source;
            }

            Type tt = @Source.GetType();
            //-->获取所有属性
            PropertyInfo[] piItem = tt.GetProperties();
            TypeBuilder tb = GetTypeBuilder(DateTime.Now.ToString("yyyyMMddHHmmss"));
            //-->动态类的构造函数
            ConstructorBuilder constructor = tb.DefineDefaultConstructor(MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName);
            foreach (var pair in piItem)
            {
                //-->创建属性
                CreateProperty(tb, pair.Name, pair.PropertyType);
            }
            Type objT = tb.CreateType();
            object tempObj = Activator.CreateInstance(objT);
            if (isSaveValue == true && tempObj != null)
            {
                PropertyInfo[] tempPIItem = objT.GetProperties();
                foreach (var v in tempPIItem)
                {
                    v.SetValue(tempObj, tt.GetProperty(v.Name).GetValue(@Source, null), null);
                }
            }
            return tempObj;
        }

        /// <summary>
        /// 克隆对象方法【此克隆类名不同，但必须类的属性名称都一样，才行，否则克隆出来的数据为空】
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Source"></param>
        /// <returns></returns>
        public static ObservableCollection<T> ObservableCollectionCloneTo<T>(this @IList Source)
        {
            if (null == @Source)
            {
                return null;
            }
            ObservableCollection<T> item = new ObservableCollection<T>();
            foreach (var v in @Source)
            {
                if (null == v)
                {
                    continue;
                }
                T temp = v.CloneTo<T>();
                if (null == temp)
                {
                    continue;
                }
                item.Add(temp);
            }
            return item;
        }

        /// <summary>
        /// 克隆到一个集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static List<T> CloneList<T>(this IEnumerable @source) where T : new()
        {
            try
            {
                if (@source == null)
                {
                    return null;
                }
                List<T> tItem = new List<T>();
                IEnumerator ie = @source.GetEnumerator();
                while (ie.MoveNext())
                {
                    T temp = ie.Current.CloneTo<T>();
                    if (null == temp)
                    {
                        continue;
                    }
                    tItem.Add(temp);
                }
                return tItem;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static Collection<T> CloneCollection<T>(this IEnumerable @source) where T : new()
        {
            try
            {
                if (@source == null)
                {
                    return null;
                }
                Collection<T> oCollection = new Collection<T>();
                IEnumerator ie = @source.GetEnumerator();
                while (ie.MoveNext())
                {
                    T result = ie.Current.CloneTo<T>();
                    if (null == result)
                    {
                        continue;
                    }
                    oCollection.Add(result);
                }
                return oCollection;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 自已克隆自己
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static ObservableCollection<T> CloneObservableCollection<T>(this IEnumerable @source) where T : new()
        {
            try
            {
                if (@source == null)
                {
                    return null;
                }
                ObservableCollection<T> oCollection = new ObservableCollection<T>();
                IEnumerator ie = @source.GetEnumerator();
                while (ie.MoveNext())
                {
                    T result = ie.Current.CloneTo<T>();
                    if (null == result)
                    {
                        continue;
                    }
                    oCollection.Add(result);
                }
                return oCollection;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 克隆到一个数组
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static T[] CloneArray<T>(this IEnumerable @source) where T : new()
        {
            List<T> tItem = CloneList<T>(@source);

            if (tItem != null && tItem.Count > 0)
            {
                return tItem.ToArray();
            }
            return null;
        }


        #region --> 将 IEnumerable<IDictionary> 转成 IEnumerable 类型
        /// <summary>
        /// 表达试
        /// </summary>
        private static readonly Regex PropertNameRegex = new Regex(@"^[A-Za-z]+[A-Za-z0-9_]*$", RegexOptions.Singleline);
        /// <summary>
        /// 签名类型字典
        /// </summary>
        private static readonly Dictionary<string, Type> _typeBySigniture = new Dictionary<string, Type>();

        /// <summary>
        /// 转换成DataSource
        /// </summary>
        /// <param name="list">字典列表</param>
        /// <returns>返回一个IEnumerable对象</returns>
        public static IEnumerable ToDataSource(this IEnumerable<IDictionary> @list)
        {
            if (@list == null)
            {
                return null;
            }

            IDictionary firstDict = null;
            bool hasData = false;

            //firstDict = list.GetFirst<IDictionary>();
            foreach (IDictionary currentDict in list)
            {
                hasData = true;
                firstDict = currentDict;
                break;
            }

            if (!hasData)
            {
                return new object[] { };
            }

            if (firstDict == null)
            {
                return null;
            }

            string typeSigniture = GetTypeSigniture(firstDict);
            Type objectType = GetTypeByTypeSigniture(typeSigniture);
            if (objectType == null)
            {
                //-->
                TypeBuilder tb = GetTypeBuilder(typeSigniture);
                //-->动态类的构造函数
                ConstructorBuilder constructor = tb.DefineDefaultConstructor(MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName);
                //-->
                foreach (DictionaryEntry pair in firstDict)
                {
                    //-->判断属性列名是否合法
                    if (PropertNameRegex.IsMatch(Convert.ToString(pair.Key), 0))
                    {
                        //-->创建属性
                        CreateProperty(tb, Convert.ToString(pair.Key), GetValueType(pair.Value));
                    }
                    else
                    {
                        throw new ArgumentException(@"IDictionary的每个键必须是字母数字和字符开始。");
                    }
                }
                objectType = tb.CreateType();

                _typeBySigniture.Add(typeSigniture, objectType);
            }

            return GenerateEnumerable(objectType, list, firstDict);
        }

        /// <summary>
        /// 根据类型签名获取类型
        /// </summary>
        /// <param name="typeSigniture">类型签名</param>
        /// <returns></returns>
        private static Type GetTypeByTypeSigniture(string typeSigniture)
        {
            Type type;
            return _typeBySigniture.TryGetValue(typeSigniture, out type) ? type : null;
        }

        /// <summary>
        /// 获取值类型
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static Type GetValueType(object value)
        {
            return value == null ? typeof(object) : value.GetType();
        }

        /// <summary>
        /// 获取类型签名
        /// </summary>
        /// <param name="firstDict"></param>
        /// <returns></returns>
        private static string GetTypeSigniture(IDictionary firstDict)
        {
            StringBuilder sb = new StringBuilder();
            foreach (DictionaryEntry pair in firstDict)
            {
                sb.AppendFormat("_{0}_{1}", pair.Key, GetValueType(pair.Value));
            }
            return sb.ToString().GetHashCode().ToString().Replace("-", "Minus");
        }

        /// <summary>
        /// 生成 IEnumerable
        /// </summary>
        /// <param name="objectType"></param>
        /// <param name="list"></param>
        /// <param name="firstDict"></param>
        /// <returns></returns>
        private static IEnumerable GenerateEnumerable(Type objectType, IEnumerable<IDictionary> list, IDictionary firstDict)
        {
            var listType = typeof(List<>).MakeGenericType(new[] { objectType });
            var listOfCustom = Activator.CreateInstance(listType);
            foreach (var currentDict in list)
            {
                if (null == currentDict)
                {
                    //throw new ArgumentException("IDictionary entry cannot be null");
                    continue;
                }
                var row = Activator.CreateInstance(objectType);
                foreach (DictionaryEntry pair in firstDict)
                {
                    if (currentDict.Contains(pair.Key))
                    {
                        PropertyInfo property = objectType.GetProperty(Convert.ToString(pair.Key));
                        property.SetValue(row, Convert.ChangeType(currentDict[pair.Key], property.PropertyType, null), null);
                    }
                }
                listType.GetMethod("Add").Invoke(listOfCustom, new[] { row });
            }
            return listOfCustom as IEnumerable;
        }

        /// <summary>
        /// 在运行时定义并创建类的新实例
        /// </summary>
        /// <param name="typeSigniture"></param>
        /// <returns></returns>
        private static TypeBuilder GetTypeBuilder(string typeSigniture)
        {
            AssemblyName an = new AssemblyName("TempAssembly" + typeSigniture);
            AssemblyBuilder assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(an, AssemblyBuilderAccess.Run);
            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("MainModule");
            TypeBuilder tb = moduleBuilder.DefineType("TempType" + typeSigniture, TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.AutoClass | TypeAttributes.AnsiClass | TypeAttributes.BeforeFieldInit | TypeAttributes.AutoLayout, typeof(object));
            return tb;
        }

        /// <summary>
        /// 创建属性
        /// </summary>
        /// <param name="tb"></param>
        /// <param name="propertyName"></param>
        /// <param name="propertyType"></param>
        private static void CreateProperty(TypeBuilder tb, string propertyName, Type propertyType)
        {
            FieldBuilder fieldBuilder = tb.DefineField("_" + propertyName, propertyType, FieldAttributes.Private);
            PropertyBuilder propertyBuilder = tb.DefineProperty(propertyName, PropertyAttributes.HasDefault, propertyType, null);
            MethodBuilder getPropMthdBldr = tb.DefineMethod("get_" + propertyName, MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, propertyType, Type.EmptyTypes);
            ILGenerator getIL = getPropMthdBldr.GetILGenerator();
            getIL.Emit(OpCodes.Ldarg_0);
            getIL.Emit(OpCodes.Ldfld, fieldBuilder);
            getIL.Emit(OpCodes.Ret);
            MethodBuilder setPropMthdBldr = tb.DefineMethod("set_" + propertyName, MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, null, new Type[] { propertyType });
            ILGenerator setIL = setPropMthdBldr.GetILGenerator();
            setIL.Emit(OpCodes.Ldarg_0);
            setIL.Emit(OpCodes.Ldarg_1);
            setIL.Emit(OpCodes.Stfld, fieldBuilder);
            setIL.Emit(OpCodes.Ret);
            propertyBuilder.SetGetMethod(getPropMthdBldr);
            propertyBuilder.SetSetMethod(setPropMthdBldr);
        }

        #endregion --> 将 IEnumerable<IDictionary> 转成 IEnumerable 类型
    }
}
