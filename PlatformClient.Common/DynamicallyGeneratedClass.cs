using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Xml.Linq;
using PlatformClient.Model.Method;
using PlatformClient.Extend.Core;

namespace PlatformClient.Common
{
    /// <summary>
    /// 动态生成类
    /// </summary>
    public class DynamicallyGeneratedClass
    {
        static AssemblyName DynamicallyAssemblyName;
        static AssemblyBuilder DynamicallyAssemblyBuilder;
        static TypeBuilder DynamicallyTypeBuilder;
        static ModuleBuilder DynamicallyModuleBuilder;
        /// <summary>
        /// 动态创建类对象类型集合
        /// </summary>
        static Dictionary<String, Type> DictType = new Dictionary<string, Type>();
        /// <summary>
        /// 创建实例
        /// </summary>
        /// <param name="item">实例属性信息</param>
        /// <param name="tableName">实例名称</param>
        /// <returns></returns>
        public static object CreateInstance(List<MetaDataColumnInfo> item, string tableName)
        {
            var tt = typeof(DynamicallyGeneratedClass);
            var instanceName = string.Format("{0}.{1}.{2}", ConstantCollection.DYNAMICALLY_ASSEMBLY_NAME_PREFIX, tt.Namespace, tableName);

            if (DictType.ContainsKey(instanceName))
            {
                return Activator.CreateInstance(DictType[instanceName]);
            }
            TypeBuilder tb = GetTypeBuilder(instanceName);
            //-->动态类的构造函数
            ConstructorBuilder constructor = tb.DefineDefaultConstructor(MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName);
            foreach (var pair in item)
            {
                if (pair.column_name.IsNullOrEmpty())
                {
                    continue;
                }
                //-->创建属性
                CreateProperty(tb, pair.column_name.ToLower(), GetPropertyType(pair));
            }
            Type objT = tb.CreateType();
            object tempObj = Activator.CreateInstance(objT);
            return tempObj;
        }

        /// <summary>
        /// 将取 xml 表列结构转成MetaDataColumnInfo集合
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public static List<MetaDataColumnInfo> GetDataTableColumnCollection(XElement result)
        {
            if (null == result)
            {
                return null;
            }
            //-->我首先要得到 表的结构 column里的信息
            var column_xml = result.Element("columns");
            if (null == column_xml)
            {
                return RowConvertToColumn(result);
            }
            //-->如果列（column）的行中属性的个数不一致，时得添加进来。
            var columns = column_xml.Elements("column");
            List<MetaDataColumnInfo> mdciItem = new List<MetaDataColumnInfo>();
            foreach (var column in columns)
            {
                var mdci = column.ToModel<MetaDataColumnInfo>();
                mdciItem.Add(mdci);
            }
            var rows_count = GetRowsAttributesCount(result);
            if (rows_count != columns.Count())
            {
                var rows = RowConvertToColumn(result);
                if (null != rows)
                {
                    foreach (var row in rows)
                    {
                        var col = mdciItem.Where(p => p.column_name.ToLower().Equals(row.column_name.ToLower())).GetFirst<MetaDataColumnInfo>();
                        if (null != col)
                        {
                            continue;
                        }
                        mdciItem.Add(row);
                    }
                }
            }

            return mdciItem;
        }

        /// <summary>
        /// 将取 xml 表列结构转成MetaDataColumnInfo集合
        /// </summary>
        /// <param name="result"></param>
        /// <param name="elementName"></param>
        /// <returns></returns>
        public static List<MetaDataColumnInfo> GetDataTableColumnCollection(XElement result, string elementName)
        {
            XElement dir = result.Element(elementName);
            if (null == dir)
            {
                return null;
            }

            //-->我首先要得到 表的结构 column里的信息
            var column_xml = dir.Element("columns");
            if (null == column_xml)
            {
                return RowConvertToColumn(result, elementName);
            }
            //-->如果列（column）的行中属性的个数不一致，时得添加进来。
            var columns = column_xml.Elements("column");
            List<MetaDataColumnInfo> mdciItem = new List<MetaDataColumnInfo>();
            foreach (var column in columns)
            {
                var mdci = column.ToModel<MetaDataColumnInfo>();
                mdciItem.Add(mdci);
            }
            var rows_count = GetRowsAttributesCount(result, elementName);
            if (rows_count != columns.Count())
            {
                var rows = RowConvertToColumn(result, elementName);
                if (null != rows)
                {
                    foreach (var row in rows)
                    {
                        var col = mdciItem.Where(p => p.column_name.ToLower().Equals(row.column_name.ToLower())).GetFirst<MetaDataColumnInfo>();
                        if (null != col)
                        {
                            continue;
                        }
                        mdciItem.Add(row);
                    }
                }
            }

            return mdciItem;
        }

        static List<MetaDataColumnInfo> RowConvertToColumn(XElement result)
        {
            XElement _table = result.Element("rows");
            if (null == _table)
            {
                return null;
            }

            List<MetaDataColumnInfo> datas = new List<MetaDataColumnInfo>();
            var rows = _table.FirstNode as XElement;
            if (null == rows)
            {
                return null;
            }
            foreach (var data in rows.Attributes())
            {
                var col = new MetaDataColumnInfo();
                col.column_comment = col.column_name = data.Name.LocalName;
                col.data_type = "varchar";
                datas.Add(col);
            }
            return datas;
        }

        static List<MetaDataColumnInfo> RowConvertToColumn(XElement result, string tableName)
        {
            XElement root = result.Element(tableName);
            XElement _table = root.Element("rows");
            if (null == _table)
            {
                return null;
            }
            //return null;

            List<MetaDataColumnInfo> datas = new List<MetaDataColumnInfo>();
            var rows = _table.FirstNode as XElement;
            if (null == rows)
            {
                return null;
            }
            foreach (var data in rows.Attributes())
            {
                var col = new MetaDataColumnInfo();
                col.column_comment = col.column_name = data.Name.LocalName;
                col.data_type = "varchar";
                datas.Add(col);
            }
            return datas;
        }


        static int GetRowsAttributesCount(XElement result)
        {
            XElement _table = result.Element("rows");
            if (null == _table)
            {
                return 0;
            }

            List<MetaDataColumnInfo> datas = new List<MetaDataColumnInfo>();
            var rows = _table.FirstNode as XElement;
            if (null == rows)
            {
                return 0;
            }
            return rows.Attributes().Count();
        }

        static int GetRowsAttributesCount(XElement result, string tableName)
        {
            XElement root = result.Element(tableName);
            XElement _table = root.Element("rows");
            if (null == _table)
            {
                return 0;
            }
            //return null;

            List<MetaDataColumnInfo> datas = new List<MetaDataColumnInfo>();
            var rows = _table.FirstNode as XElement;
            if (null == rows)
            {
                return 0;
            }
            return rows.Attributes().Count();
        }


        /// <summary>
        /// 将Xml表数据，转成对象集合
        /// </summary>
        /// <param name="result">xml数据</param>
        /// <returns>返回数据集</returns>
        public static List<Object> ToList(XElement result)
        {
            //-->我首先要得到 表的结构 column里的信息
            List<MetaDataColumnInfo> mdciItem = GetDataTableColumnCollection(result);
            return ToList(result, mdciItem);
        }
        /// <summary>
        /// 获取第一条记录
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public static Object ToFirst(XElement result)
        {
            List<MetaDataColumnInfo> mdciItem = GetDataTableColumnCollection(result);
            return ToFirst(result, mdciItem);
        }

        /// <summary>
        /// 获取第一条记录
        /// </summary>
        /// <param name="result">xml数据</param>
        /// <param name="mdciItem">列的集合</param>
        /// <returns>返回数据集</returns>
        public static Object ToFirst(XElement result, List<MetaDataColumnInfo> mdciItem)
        {
            //-->我首先要得到 表的结构 column里的信息
            if (null == mdciItem || null == result)
            {
                return null;
            }
            XElement _table = result.Element("rows");
            if (null == _table)
            {
                return null;
            }
            var rows = _table.Elements("row");
            if (null == rows || 0 == rows.Count())
            {
                return null;
            }
            return ConvertRowToObject(rows.First(), result.Name.LocalName, mdciItem);
        }

        static object ConvertRowToObject(XElement row, string tableName, List<MetaDataColumnInfo> mdciItem)
        {
            object _newInstance = CreateInstance(mdciItem, tableName);
            if (null == _newInstance)
            {
                return null;
            }
            var piItem = _newInstance.GetType().GetProperties();
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
                    pi.SetValue(_newInstance, Convert.ChangeType(value, pi.PropertyType, null), null);
                }
                catch
                {
                    continue;
                }
            }
            return _newInstance;
        }

        /// <summary>
        /// 将Xml表数据，转成对象集合
        /// </summary>
        /// <param name="result">xml数据</param>
        /// <param name="tableName">表的名称</param>
        /// <returns>返回数据集</returns>
        public static List<Object> ToList(XElement result, string tableName)
        {
            //-->我首先要得到 表的结构 column里的信息
            List<MetaDataColumnInfo> mdciItem = GetDataTableColumnCollection(result, tableName);
            return ToList(result, tableName, mdciItem);
        }

        /// <summary>
        /// 将Xml表数据，转成对象集合
        /// </summary>
        /// <param name="result">xml数据</param>
        /// <param name="mdciItem">列的集合</param>
        /// <returns>返回数据集</returns>
        public static List<Object> ToList(XElement result, List<MetaDataColumnInfo> mdciItem)
        {
            //-->我首先要得到 表的结构 column里的信息
            if (null == mdciItem || null == result)
            {
                return null;
            }
            XElement _table = result.Element("rows");
            if (null == _table)
            {
                return null;
            }

            List<Object> objItem = new List<object>();
            foreach (var row in _table.Elements("row"))
            {
                object _newInstance = CreateInstance(mdciItem, result.Name.LocalName + "_1");
                if (null == _newInstance)
                {
                    return null;
                }
                var piItem = _newInstance.GetType().GetProperties();
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
                        pi.SetValue(_newInstance, Convert.ChangeType(value, pi.PropertyType, null), null);
                    }
                    catch
                    {
                        continue;
                    }
                }
                objItem.Add(_newInstance);
            }
            return objItem;
        }

        /// <summary>
        /// 将Xml表数据，转成对象集合
        /// </summary>
        /// <param name="result">xml数据</param>
        /// <param name="tableName">表的名称,在xml里tableName就是节点名称</param>
        /// <param name="mdciItem">列的集合</param>
        /// <returns>返回数据集</returns>
        public static List<Object> ToList(XElement result, string tableName, List<MetaDataColumnInfo> mdciItem)
        {
            return ToList(result, tableName, tableName, mdciItem);
        }

        /// <summary>
        /// 将Xml表数据，转成对象集合
        /// </summary>
        /// <param name="result">xml数据</param>
        /// <param name="tableName">表的名称</param>
        /// <param name="mdciItem">列的集合</param>
        /// <param name="instanceName">实例名称</param>
        /// <returns>返回数据集</returns>
        public static List<Object> ToList(XElement result, string tableName, string instanceName, List<MetaDataColumnInfo> mdciItem)
        {
            //-->我首先要得到 表的结构 column里的信息
            if (null == mdciItem || null == result)
            {
                return null;
            }
            XElement dir = result.Element(tableName);
            if (null == dir)
            {
                return null;
            }
            XElement _table = dir.Element("rows");
            if (null == _table)
            {
                return null;
            }

            List<Object> objItem = new List<object>();
            foreach (var row in _table.Elements("row"))
            {
                var _newInstance = ConvertRowToObject(row, instanceName, mdciItem);
                if (null == _newInstance)
                {
                    continue;
                }
                objItem.Add(_newInstance);
            }
            return objItem;
        }

        /// <summary>
        /// 获取实例
        /// </summary>
        /// <param name="result"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static Object GetInstance(XElement result, string tableName)
        {
            XElement root = result.Element(tableName);
            if (null == root)
            {
                return null;
            }

            //-->我首先要得到 表的结构 column里的信息
            List<MetaDataColumnInfo> piItem = GetDataTableColumnCollection(result, tableName);
            if (null == piItem)
            {
                return null;
            }
            var rows = root.Element("rows");
            if (null == rows)
            {
                return null;
            }
            XElement row = rows.Elements("row").GetFirst<XElement>();
            if (null == row)
            {
                return null;
            }

            object _newInstance = CreateInstance(piItem, tableName);
            if (null == _newInstance)
            {
                return null;
            }
            SetInstanceValue(_newInstance, row);
            return _newInstance;
        }

        /// <summary>
        /// 在运行时定义并创建类的新实例
        /// </summary>
        /// <param name="builderClassName"></param>
        /// <returns></returns>
        static TypeBuilder GetTypeBuilder(string builderClassName)
        {
            if (null == DynamicallyAssemblyName)
            {
                string typeSigniture = ConstantCollection.DYNAMICALLY_ASSEMBLY_NAME;
                DynamicallyAssemblyName = new AssemblyName("TempAssembly" + typeSigniture);
            }
            if (null == DynamicallyAssemblyBuilder)
            {
                DynamicallyAssemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(DynamicallyAssemblyName, AssemblyBuilderAccess.Run);
            }
            if (null == DynamicallyModuleBuilder)
            {
                DynamicallyModuleBuilder = DynamicallyAssemblyBuilder.DefineDynamicModule("MainModule");
            }
            DynamicallyTypeBuilder = DynamicallyModuleBuilder.DefineType(builderClassName, TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.AutoClass | TypeAttributes.AnsiClass | TypeAttributes.BeforeFieldInit | TypeAttributes.AutoLayout, typeof(object));
            string key = DynamicallyTypeBuilder.FullName;
            if (!DictType.ContainsKey(key))
            {
                DictType.Add(key, DynamicallyTypeBuilder);
            }
            return DynamicallyTypeBuilder;
        }

        /// <summary>
        /// 获取属性类
        /// </summary>
        /// <param name="pair"></param>
        /// <returns></returns>
        static Type GetPropertyType(MetaDataColumnInfo pair)
        {
            if (null == pair || pair.data_type.IsNullOrEmpty())
            {
                return typeof(object);
            }

            switch (pair.data_type.ToLower())
            {
                case "varchar":
                case "char":
                    return typeof(String);

                //tinyint(size) 	-128 到 127 常规。0 到 255 无符号*。在括号中规定最大位数。
                //smallint(size) 	-32768 到 32767 常规。0 到 65535 无符号*。在括号中规定最大位数。
                //mediumint(size) 	-8388608 到 8388607 普通。0 to 16777215 无符号*。在括号中规定最大位数。
                //int(size) 	-2147483648 到 2147483647 常规。0 到 4294967295 无符号*。在括号中规定最大位数。
                //bigint(size) 	-9223372036854775808 到 9223372036854775807 常规。0 到 18446744073709551615 无符号*。在括号中规定最大位数。
                //float(size,d) 	带有浮动小数点的小数字。在括号中规定最大位数。在 d 参数中规定小数点右侧的最大位数。
                //double(size,d) 	带有浮动小数点的大数字。在括号中规定最大位数。在 d 参数中规定小数点右侧的最大位数。
                //decimal(size,d) 	作为字符串存储的 double 类型，允许固定的小数点。
                case "tinyint":
                    return typeof(byte);
                case "smallint":
                    return typeof(Int16);
                case "int":
                case "int32":
                case "bigint":
                case "mediumint":
                    return typeof(Int32);
                case "float":
                    return typeof(float);
                case "double":
                    return typeof(double);
                case "decimal":
                    return typeof(decimal);


                //11.4.1. CHAR和VARCHAR类型
                //11.4.2. BINARY和VARBINARY类型
                //11.4.3. BLOB和TEXT类型
                case "binary":
                case "varbinary":
                case "blob":
                case "text":
                    return typeof(byte[]);

                //11.3.1. DATETIME、DATE和TIMESTAMP类型
                //11.3.2. TIME类型
                //11.3.3. YEAR类型
                case "datetime":
                case "date":
                case "time":
                case "timestamp":
                case "year":
                    return typeof(DateTime);
                default:
                    return typeof(String);
            }
        }

        /// <summary>
        /// 创建属性
        /// </summary>
        /// <param name="tb"></param>
        /// <param name="propertyName"></param>
        /// <param name="propertyType"></param>
        static void CreateProperty(TypeBuilder tb, string propertyName, Type propertyType)
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

        /// <summary>
        /// 给实例赋值
        /// </summary>
        /// <param name="source"></param>
        /// <param name="xValue"></param>
        static void SetInstanceValue(object @source, XElement xValue)
        {
            if (null == xValue)
            {
                return;
            }
            var piItem = @source.GetType().GetProperties();
            foreach (var pi in piItem)
            {
                var _value = xValue.GetAttributeValue(pi.Name);
                if (_value.IsNullOrEmpty())
                {
                    continue;
                }
                pi.SetValue(@source, Convert.ChangeType(_value, pi.PropertyType, null), null);
            }
        }
    }
}
