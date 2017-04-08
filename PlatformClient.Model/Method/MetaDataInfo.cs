using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace PlatformClient.Model.Method
{
    /// <summary>
    /// 表元素信息
    /// </summary>
    public class MetaDataInfo
    {
        /// <summary>
        /// 表的名称
        /// </summary>
        public string table_name { get; set; }
        /// <summary>
        /// 表的备注
        /// </summary>
        public string table_comment { get; set; }
        /// <summary>
        /// 表的类型
        /// </summary>
        public string table_type { get; set; }
        /// <summary>
        /// 列集合
        /// </summary>
        public List<MetaDataColumnInfo> Item { get; set; }
    }
}
