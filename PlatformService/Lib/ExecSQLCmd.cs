using System.Runtime.Serialization;
using System.Xml.Linq;

namespace PlatformService
{
    /// <summary>
    /// SQL语句描述类
    /// </summary>
    [DataContract]
    public class ExecSQLCmd
    {
        /// <summary>
        /// 执行SQL类型[insert,update,select,delete and so on ...]
        /// </summary>
        [DataMember]
        public ExecSqlCmdType ExecType { get; set; }
        /// <summary>
        /// SQL语句,可以是 insert,update,select,delete and so on ...
        /// </summary>
        [DataMember]
        public string Sql { get; set; }
        /// <summary>
        /// 结果
        /// </summary>
        [DataMember]
        public XElement Result { get; set; }
        /// <summary>
        /// 出错信息
        /// </summary>
        [DataMember]
        public string Error { get; set; }
        /// <summary>
        /// 表的名称 default = _tablename
        /// </summary>
        [DataMember]
        public string TableName { get; set; }
        /// <summary>
        /// 控件名称
        /// </summary>
        [DataMember]
        public string CtrlName { get; set; }
        /// <summary>
        /// 方法名称
        /// </summary>
        [DataMember]
        public string MethodName { get; set; }
    }
}