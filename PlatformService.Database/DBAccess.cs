using System;
using System.Data;
using System.Data.Common;
using System.Collections.Generic;
using System.Diagnostics;

namespace PlatformService.Database
{
    /// <summary>
    /// 
    /// </summary>
    public class DBAccess
    {
        static IDatabase _DbAccess;

        /// <summary>
        /// 数据库访问
        /// </summary>
        static IDatabase DBHelper
        {
            get
            {
                if (null == _DbAccess)
                {
                    _DbAccess = new MySQLHelper();
                }
                return _DbAccess;
            }
        }

        /// <summary>
        /// 出错信息
        /// </summary>
        public static string ErrorMessage { get; set; }

        /// <summary>
        /// 执行 SQL 语句并返回受影响的行数。
        /// </summary>
        /// <param name="executeSql"></param>
        /// <returns></returns>
        public static int ExecuteNonQuery(string executeSql)
        {
            printf(string.Format("返回受影响的行数ExecuteNonQuery执行【\r\n{0}\r\n】Sql语句", executeSql));
            if (null != DBHelper)
            {
                var result = DBHelper.ExecuteNonQuery(executeSql);
                ErrorMessage = DBHelper.ErrorMessage;
                return result;
            }
            else
            {
                ErrorMessage = string.Format("创建访问数据库对象失败");
                return -100;
            }
        }
        /// <summary>
        /// 执行带参数的sql语句或存储过程;0-成功，其它失败
        /// </summary>
        /// <param name="executeSql"></param>
        /// <param name="oParams"></param>
        /// <returns></returns>
        public static int ExecuteNonQuery(string executeSql, params DbParameter[] oParams)
        {
            printf(string.Format("执行带参数的sql语句或存储过程ExecuteNonQuery执行【\r\n{0}\r\n】Sql语句,参数个数【{1}】", executeSql, oParams.Length));
            if (null != DBHelper)
            {
                var result = DBHelper.ExecuteNonQuery(executeSql, oParams);
                ErrorMessage = DBHelper.ErrorMessage;
                return result;
            }
            else
            {
                ErrorMessage = string.Format("创建访问数据库对象失败");
                return -100;
            }
        }
        /// <summary>
        /// 批处理操作【0-成功；其它值失败】
        /// </summary>
        /// <param name="sqlItem">批量处理操作</param>
        /// <returns>0-成功；其它值失败</returns>
        public static int ExecuteNonQuery(List<String> sqlItem)
        {
            printf(string.Format("批处理操作ExecuteNonQuery执行【\r\n{0}\r\n】Sql语句集合",
                string.Join("\r\n", sqlItem)));
            if (null != DBHelper)
            {
                var result = DBHelper.ExecuteNonQuery(sqlItem);
                ErrorMessage = DBHelper.ErrorMessage;
                return result;
            }
            else
            {
                ErrorMessage = string.Format("创建访问数据库对象失败");
                return -100;
            }
        }
        /// <summary>
        /// 返回首行首列值
        /// </summary>
        /// <param name="executeSql"></param>
        /// <returns></returns>
        public static object ExecuteScalar(string executeSql)
        {
            printf(string.Format("返回首行首列值ExecuteScalar执行【\r\n{0}\r\n】Sql语句", executeSql));
            if (null != DBHelper)
            {
                var result = DBHelper.ExecuteScalar(executeSql);
                ErrorMessage = DBHelper.ErrorMessage;
                return result;
            }
            else
            {
                ErrorMessage = string.Format("创建访问数据库对象失败");
                return null;
            }
        }
        /// <summary>
        /// 获取DataSet
        /// </summary>
        /// <param name="executeSql"></param>
        /// <returns></returns>
        public static DataSet GetDataSet(string executeSql)
        {
            printf(string.Format("GetDataSet执行【\r\n{0}\r\n】Sql语句", executeSql));
            if (null != DBHelper)
            {
                var result = DBHelper.GetDataSet(executeSql);
                ErrorMessage = DBHelper.ErrorMessage;
                return result;
            }
            else
            {
                ErrorMessage = string.Format("创建访问数据库对象失败");
                return null;
            }
        }
        /// <summary>
        /// 获取指定的DataSet
        /// </summary>
        /// <param name="executeSql"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static DataSet GetDataSet(string executeSql, string name)
        {
            printf(string.Format("GetDataSet【\r\n{0}\r\n】【{1}】", executeSql, name));
            if (null != DBHelper)
            {
                var result = DBHelper.GetDataSet(executeSql, name);
                ErrorMessage = DBHelper.ErrorMessage;
                return result;
            }
            else
            {
                ErrorMessage = string.Format("创建访问数据库对象失败");
                return null;
            }
        }
        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="procedureName"></param>
        /// <returns></returns>
        public static DataSet ExecuteProcedure(string procedureName)
        {
            printf(string.Format("ExecuteProcedure执行【\r\n{0}\r\n】存储过程", procedureName));
            if (null != DBHelper)
            {
                var result = DBHelper.ExecuteProcedure(procedureName);
                ErrorMessage = DBHelper.ErrorMessage;
                return result;
            }
            else
            {
                ErrorMessage = string.Format("创建访问数据库对象失败");
                return null;
            }
        }
        /// <summary>
        /// 返回一个DataTable
        /// </summary>
        /// <param name="executeSql"></param>
        /// <returns></returns>
        public static DataTable GetDataTable(string executeSql)
        {
            printf(string.Format("GetDataTable执行【\r\n{0}\r\n】Sql语句", executeSql));
            if (null != DBHelper)
            {
                var result = DBHelper.GetDataTable(executeSql);
                ErrorMessage = DBHelper.ErrorMessage;
                return result;
            }
            else
            {
                ErrorMessage = string.Format("创建访问数据库对象失败");
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        internal static void printf(object value)
        {
            try
            {
                Debug.WriteLine("{0}-->{1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:s"), value);
                //Console.WriteLine("{0}-->{1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:s.fffffff"), value);
            }
            catch
            {
                return;
            }
        }
    }
}
