using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlatformClient.Model.Method
{
    /// <summary>
    /// 列数据转换类
    /// 如
    /// {
    /// user_info.cityid 字段的值，来源于 country_info.id.而显示的则是 country_info.country_name.
    /// }
    /// </summary>
    public class ColumnDBConvertInfo
    {
        /// <summary>
        /// 要转换列的名称
        /// </summary>
        public string ColumnName { get; set; }
        /// <summary>
        /// 关系表名称
        /// </summary>
        public string RelationTableName { get; set; }
        /// <summary>
        /// 关系表
        /// </summary>
        public string RelationField { get; set; }
        /// <summary>
        /// 关系表显示字段名称
        /// </summary>
        public string DisplayField { get; set; }
    }
}
