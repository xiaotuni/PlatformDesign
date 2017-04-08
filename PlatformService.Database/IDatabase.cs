using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace PlatformService.Database
{
    /// <summary>
    /// 数据库操作类
    /// </summary>
    public interface IDatabase
    {
        /// <summary>
        /// 出错信息
        /// </summary>
        string ErrorMessage { get; set; }
        /// <summary>
        /// 执行 SQL 语句并返回受影响的行数。
        /// </summary>
        /// <param name="executeSql"></param>
        /// <returns></returns>
        int ExecuteNonQuery(string executeSql);
        /// <summary>
        /// 执行带参数的sql语句或存储过程;0-成功，其它失败
        /// </summary>
        /// <param name="executeSql"></param>
        /// <param name="oParams"></param>
        /// <returns></returns>
        int ExecuteNonQuery(string executeSql, params DbParameter[] oParams);
        /// <summary>
        /// 批处理操作【0-成功；其它值失败】
        /// </summary>
        /// <param name="sqlItem">批量处理操作</param>
        /// <returns>0-成功；其它值失败</returns>
        int ExecuteNonQuery(List<String> sqlItem);
        /// <summary>
        /// 返回首行首列值
        /// </summary>
        /// <param name="executeSql"></param>
        /// <returns></returns>
        object ExecuteScalar(string executeSql);
        /// <summary>
        /// 获取DataSet
        /// </summary>
        /// <param name="executeSql"></param>
        /// <returns></returns>
        DataSet GetDataSet(string executeSql);
        /// <summary>
        /// 获取指定的DataSet
        /// </summary>
        /// <param name="executeSql"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        DataSet GetDataSet(string executeSql, string name);
        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="procedureName"></param>
        /// <returns></returns>
        DataSet ExecuteProcedure(string procedureName);
        /// <summary>
        /// 返回一个DataTable
        /// </summary>
        /// <param name="executeSql"></param>
        /// <returns></returns>
        DataTable GetDataTable(string executeSql);
    }
}
