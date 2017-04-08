using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace PlatformClient.Model.Method
{
    /// <summary>
    /// 参数信息
    /// </summary>
    public class ParamInfo
    {
        /// <summary>
        /// 源控件
        /// </summary>
        public string OrgFieldName { get; set; }
        /// <summary>
        /// 源字段
        /// </summary>
        public string OrgFieldType { get; set; }
        /// <summary>
        /// OrgFieldType是否是来到源数据[ControlDataSourceTableName 所对应表里的字段]里的。
        /// </summary>
        public bool OrgIsDataSource { get; set; }

        /// <summary>
        /// 目标控件
        /// </summary>
        public string TargetCtrlName { get; set; }
        /// <summary>
        /// 目标字段
        /// </summary>
        public string TargetFieldName { get; set; }
        /// <summary>
        /// 目标类型
        /// </summary>
        public string TargetFieldType { get; set; }
    }
}
