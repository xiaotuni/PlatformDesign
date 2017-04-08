using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace PlatformService.Lib
{
    /// <summary>
    /// 列的信息
    /// </summary>
    public class ColumnInfo
    {
        /// <summary>
        /// 表的名称
        /// </summary>
        public string table_name { get; set; }
        /// <summary>
        /// 列的命令
        /// </summary>
        public string column_name { get; set; }
        /// <summary>
        /// 是否可以为空
        /// </summary>
        public string is_nullable { get; set; }
        /// <summary>
        /// 数据类型
        /// </summary>
        public string data_type { get; set; }
        /// <summary>
        /// 列的备注
        /// </summary>
        public string column_comment { get; set; }
        /// <summary>
        /// 列的默认值
        /// </summary>
        public string column_default { get; set; }
        /// <summary>
        /// 最大字符长度
        /// </summary>
        public long character_maximum_length { get; set; }
        /// <summary>
        /// 主键、外键描述
        /// </summary>
        public string column_key { get; set; }
        /// <summary>
        /// 列的序号
        /// </summary>
        public int ordinal_position { get; set; }
    }
}
