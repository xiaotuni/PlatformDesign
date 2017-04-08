using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Linq;
using PlatformService.Lib;

namespace PlatformService
{
    /// <summary>
    /// 平台命令对象类
    /// </summary>
    [DataContract]
    public class PlatformCommandInfo
    {
        //List<string> _SqlSentence;
        string _TableName;
        string _CommandName;
        //XElement _DataTable;
        string _ExecuteNonQueryResult;
        XElement _TempValue;
        XElement _PublicValue;

        /// <summary>
        /// 参数集合
        /// </summary>
        [DataMember]
        public XElement ParamCollection { get; set; }

        /// <summary>
        /// 公共值
        /// </summary>
        [DataMember]
        public XElement PublicValue { get { return _PublicValue; } set { _PublicValue = value; } }
        /// <summary>
        /// 临时存放的值
        /// </summary>
        [DataMember]
        public XElement TempValue { get { return _TempValue; } set { _TempValue = value; } }
        ///// <summary>
        ///// SQL语句
        ///// </summary>
        //[DataMember]
        //public List<string> SqlSentence { get { return _SqlSentence; } set { _SqlSentence = value; } }
        /// <summary>
        /// 执行SQL语句集合[insert,update,select,delete and so on ...]
        /// </summary>
        [DataMember]
        public List<ExecSQLCmd> ExecSql { get; set; }
        //public List<
        /// <summary>
        /// 表的名称
        /// </summary>
        [DataMember]
        public string TableName { get { return _TableName; } set { _TableName = value; } }
        /// <summary>
        /// 命令名称
        /// </summary>
        [DataMember]
        public string CommandName { get { return _CommandName; } set { _CommandName = value; } }
        ///// <summary>
        ///// 表的信息
        ///// </summary>
        //[DataMember]
        //public XElement DataTable { get { return _DataTable; } set { _DataTable = value; } }
        /// <summary>
        /// 返回执行的结果
        /// </summary>
        [DataMember]
        public string ExecuteNonQueryResult { get { return _ExecuteNonQueryResult; } set { _ExecuteNonQueryResult = value; } }
        /// <summary>
        /// 错误信息
        /// </summary>
        [DataMember]
        public string ErrorMessage { get; set; }

        /// <summary>
        /// 到哪个模块
        /// </summary>
        [DataMember]
        public CommandDirectionType ToModel { get; set; }

        /// <summary>
        /// 来到于哪个模块
        /// </summary>
        [DataMember]
        public CommandDirectionType FromModel { get; set; }

        /// <summary>
        /// 命令类型
        /// </summary>
        [DataMember]
        public CommandDirectionType CommandType { get; set; }
    }

    /// <summary>
    /// 执行SQL类型
    /// </summary>
    public enum ExecSqlCmdType
    {
        /// <summary>
        /// 插入,更新,删除
        /// </summary>
        [DataMember]
        ExecuteNonQuery,
        /// <summary>
        /// 查询
        /// </summary>
        [DataMember]
        Query,
        /// <summary>
        /// 首行首例的值
        /// </summary>
        [DataMember]
        ExecuteScalar
    }
    /// <summary>
    /// 命令类型
    /// </summary>
    public enum CommandDirectionType
    {
        /// <summary>
        /// 复合控件
        /// </summary>
        [DataMember]
        CompositeControl,
        /// <summary>
        /// 数据类型
        /// </summary>
        [DataMember]
        DB,
        /// <summary>
        /// 消息
        /// </summary>
        [DataMember]
        Message,
        /// <summary>
        /// 客户UI界面
        /// </summary>
        ClientUI,
    }
}
