using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Linq;
using System.Xml.Linq;
using PlatformService.Database;
using PlatformService.Utility;
using System.Text;

namespace PlatformService.Lib
{
    //※▲△■§№☆★○●◎◇◆□♀♂＃＆＠
    /// <summary>
    ///
    /// </summary>
    internal class PlatformHelper
    {
        static List<ColumnInfo> _Columns = new List<ColumnInfo>();

        internal static List<ColumnInfo> Columns
        {
            get
            {
                if (0 == _Columns.Count)
                {
                    string Sql = string.Format("select t.table_name,t.column_name,t.is_nullable,t.data_type,");
                    Sql += string.Format(" t.COLUMN_COMMENT,t.COLUMN_DEFAULT,t.CHARACTER_MAXIMUM_LENGTH,t.COLUMN_KEY,t.ordinal_position");
                    Sql += string.Format(" from information_schema.COLUMNS t");
                    Sql += string.Format(" where t.TABLE_SCHEMA = 'db_system_platform'");
                    Sql += string.Format(" order by t.TABLE_NAME ");

                    //DataTable dt = DBAccess.DBHelper.GetDataTable(Sql);
                    DataTable dt = DBAccess.GetDataTable(Sql);
                    _Columns = dt.GetModelItem<ColumnInfo>();
                }
                return _Columns;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        internal PlatformCommandInfo DoAction(string command, PlatformCommandInfo obj)
        {
            Type t = this.GetType();
            //-->命令名称
            string commandName = string.Format("Process{0}", command);
            //-->获取方法
            MethodInfo method = t.GetMethod(commandName, BindingFlags.NonPublic | BindingFlags.Instance);
            if (null == method)
            {
                return null;
            }
            object result = method.Invoke(this, new object[] { command, obj });
            return result as PlatformCommandInfo;
        }

        internal static ColumnInfo GetColumnInfo(string tableName, string columnName)
        {
            return Columns.Where(p => (
                                        !tableName.IsNullOrEmpty() &&
                                        p.table_name.ToLower().Equals(tableName.ToLower()) &&
                                        p.column_name.ToLower().Equals(columnName.ToLower())
                                        ) ||
                                        (
                                        tableName.IsNullOrEmpty() &&
                                        p.column_name.ToLower().Equals(columnName.ToLower())
                                        )
                                ).GetFirst<ColumnInfo>();

            //ColumnInfo _ci = null;
            //foreach (ColumnInfo ci in Columns)
            //{
            //    if (!String.IsNullOrEmpty(tableName))
            //    {
            //        if (ci.table_name.ToLower().Equals(tableName.ToLower()) &&
            //            ci.column_name.ToLower().Equals(columnName.ToLower()))
            //        {
            //            _ci = ci;
            //            break;
            //        }
            //    }
            //    else
            //    {
            //        if (ci.column_name.ToLower().Equals(columnName.ToLower()))
            //        {
            //            _ci = ci;
            //            break;
            //        }
            //    }
            //}

            //return _ci;
        }

        ///// <summary>
        ///// 将DataTable转成xml
        ///// </summary>
        ///// <param name="sqlSentence"></param>
        ///// <param name="tableName"></param>
        ///// <param name="dt"></param>
        ///// <returns></returns>
        //internal static XElement GetXml(DataTable dt, string sqlSentence, string tableName)
        //{
        //    if (null == dt)
        //    {
        //        return null;
        //    }
        //    ColumnInfo ci = new ColumnInfo();
        //    List<XElement> _heads = new List<XElement>();
        //    foreach (DataColumn dc in dt.Columns)
        //    {
        //        ci = GetColumnInfo(tableName, dc.ColumnName);
        //        if (null == ci)
        //        {
        //            continue;
        //        }
        //        XElement _xe_column = new XElement("column",
        //                new XAttribute("table_name", string.Format("{0}", ci.table_name)),
        //                new XAttribute("character_maximum_length", string.Format("{0}", ci.character_maximum_length)),
        //                new XAttribute("column_name", string.Format("{0}", ci.column_name)),
        //                new XAttribute("data_type", string.Format("{0}", ci.data_type)),
        //                new XAttribute("is_nullable", string.Format("{0}", ci.is_nullable)),
        //                new XAttribute("column_comment", string.Format("{0}", ci.column_comment)),
        //                new XAttribute("column_key", string.Format("{0}", ci.column_key)),
        //                new XAttribute("column_default", string.Format("{0}", ci.column_default))
        //            );
        //        _heads.Add(_xe_column);
        //    }
        //    XElement _xe_header = new XElement("columns", _heads);
        //    List<XElement> _rows = new List<XElement>();
        //    foreach (DataRow dr in dt.Rows)
        //    {
        //        //-->每一行的数据
        //        XElement _columns = new XElement("row");
        //        foreach (DataColumn dc in dt.Columns)
        //        {
        //            object value = dr[dc];
        //            if (dc.DataType.Name.Equals("Byte[]"))
        //            {
        //                byte[] buffer = value as byte[];
        //                if (null != buffer && 0 < buffer.Length)
        //                {
        //                    value = Encoding.Default.GetString(buffer);
        //                }
        //                else
        //                {
        //                    value = "";
        //                }
        //                _columns.Add(new XElement(dc.ColumnName.ToLower(), new XCData(value.ToString())));
        //            }
        //            else
        //            {
        //                _columns.Add(new XAttribute(dc.ColumnName.ToLower(), value));
        //            }
        //        }
        //        _rows.Add(_columns);
        //    }
        //    XElement _xe_rows = new XElement("rows", _rows);
        //    XElement _result = new XElement(string.Format("{0}", tableName), _xe_header, _xe_rows);
        //    _result.Add(new XAttribute("sql", string.Format("{0}", sqlSentence)));

        //    return _result;
        //}
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        internal static XElement ParseDataTable(DataTable dt, string tableName)
        {
            if (null == dt)
            {
                return null;
            }

            ColumnInfo ci = new ColumnInfo();
            List<XElement> _heads = new List<XElement>();
            foreach (DataColumn dc in dt.Columns)
            {
                //dt.TableName
                ci = GetColumnInfo(tableName, dc.ColumnName);
                if (null == ci)
                {
                    continue;
                }
                XElement _xe_column = new XElement("column",
                        new XAttribute("table_name", string.Format("{0}", ci.table_name)),
                        new XAttribute("character_maximum_length", string.Format("{0}", ci.character_maximum_length)),
                        new XAttribute("column_name", string.Format("{0}", ci.column_name)),
                        new XAttribute("data_type", string.Format("{0}", ci.data_type)),
                        new XAttribute("is_nullable", string.Format("{0}", ci.is_nullable)),
                        new XAttribute("column_comment", string.Format("{0}", ci.column_comment)),
                        new XAttribute("column_key", string.Format("{0}", ci.column_key)),
                        new XAttribute("column_default", string.Format("{0}", ci.column_default))
                    );
                _heads.Add(_xe_column);
            }
            XElement _xe_header = new XElement("columns", _heads);
            List<XElement> _rows = new List<XElement>();
            foreach (DataRow dr in dt.Rows)
            {
                //-->每一行的数据
                XElement _columns = new XElement("row");
                foreach (DataColumn dc in dt.Columns)
                {
                    object value = dr[dc];
                    if (dc.DataType.Name.Equals("Byte[]"))
                    {
                        byte[] buffer = value as byte[];
                        if (null != buffer && 0 < buffer.Length)
                        {
                            value = Encoding.Default.GetString(buffer);
                        }
                        else
                        {
                            value = "";
                        }
                        _columns.Add(new XElement(dc.ColumnName.ToLower(), new XCData(value.ToString())));
                    }
                    else
                    {
                        _columns.Add(new XAttribute(dc.ColumnName.ToLower(), value));
                    }
                }
                _rows.Add(_columns);
            }
            XElement _xe_rows = new XElement("rows", _rows);
            XElement _result = new XElement(string.Format("{0}", tableName), _xe_header, _xe_rows);
            //_result.Add(new XAttribute("sql", string.Format("{0}", sqlSentence)));
            return _result;
        }
    }
}
