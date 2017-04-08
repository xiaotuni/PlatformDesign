using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace PlatformClient.Model.Method
{
    /// <summary>
    /// SQL语句描述类
    /// </summary>
    public class CtrlExecSQLCmd
    {
        /// <summary>
        /// 执行SQL类型
        /// </summary>
        public CtrlExecSqlCmdType ExecType { get; set; }
        /// <summary>
        /// SQL语句,可以是 insert,update,select,delete and so on ...
        /// </summary>
        public string Sql { get; set; }
        /// <summary>
        /// 结果
        /// </summary>
        public XElement Result { get; set; }
        /// <summary>
        /// 出错信息
        /// </summary>
        public string Error { get; set; }
        /// <summary>
        /// 表的名称
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// 控件名称
        /// </summary>
        public string CtrlName { get; set; }
        /// <summary>
        /// 方法名称
        /// </summary>
        public string MethodName { get; set; }
        /// <summary>
        /// 构造函数
        /// </summary>
        public CtrlExecSQLCmd()
            : this(CtrlExecSqlCmdType.Query, "", "", "", "")
        {
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="execType">执行SQL类型</param>
        /// <param name="sql">sql语句</param>
        /// <param name="tableName">表的名称</param>
        /// <param name="ctrlName">控件名称</param>
        /// <param name="methodName">方法名称</param>
        public CtrlExecSQLCmd(CtrlExecSqlCmdType execType, string sql, string tableName, string ctrlName, string methodName)
        {
            this.ExecType = execType;
            this.Sql = sql;
            this.TableName = tableName;
            this.CtrlName = ctrlName;
            this.MethodName = methodName;
        }
    }
}
