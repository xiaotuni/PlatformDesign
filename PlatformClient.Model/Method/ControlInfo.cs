using System;
using System.Collections.Generic;

namespace PlatformClient.Model.Method
{
    /// <summary>
    /// 控件信息类
    /// </summary>
    public class ControlInfo
    {
        string _ControlType = "BaseControl";
        string _ParentCtrlName = "LayoutRoot";

        /// <summary>
        /// 控件名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 列的名称
        /// </summary>
        public string ColumnName { get; set; }
        /// <summary>
        /// default value is "BaseControl" 还是复合控件
        /// </summary>
        public string ControlType { get { return _ControlType; } set { _ControlType = value; } }
        /// <summary>
        /// 备注
        /// </summary>
        public string Comment { get; set; }
        /// <summary>
        /// 表的名称
        /// </summary>
        public string MetaData { get; set; }
        /// <summary>
        /// 控件类型【Button、Lable、TextBoxEx and so on】
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// default value is 'LayoutRoot'
        /// </summary>
        public string ParentCtrlName
        {
            get
            {
                if (!String.IsNullOrEmpty(Name) && Name.Substring(0, 4).Equals("Form") && Name.Split('_').Length > 1)
                {
                    _ParentCtrlName = Name.Split('_')[0];
                }
                return _ParentCtrlName;
            }
            set
            {
                if (!String.IsNullOrEmpty(Name) && Name.Substring(0, 4).Equals("Form") && Name.Split('_').Length > 1)
                {
                    _ParentCtrlName = Name.Split('_')[0];
                }
                else
                {
                    _ParentCtrlName = value;
                }
            }
        }

        ///// <summary>
        ///// 控件事件
        ///// </summary>
        //public List<ControlEventInfo> Events { get; set; }

        ///// <summary>
        ///// 控件的数据源
        ///// </summary>
        //public MetaDataInfo ControlDataSource { get; set; }
        /// <summary>
        /// 控件的数据源名称
        /// </summary>
        public string ControlDataSourceTableName { get; set; }

        /// <summary>
        /// 列数据转换类
        /// </summary>
        public List<ColumnDBConvertInfo> Convert { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (String.IsNullOrEmpty(Comment) && String.IsNullOrEmpty(ColumnName))
            {
                return this.Name;
            }
            return string.Format("{0}【{1}】", this.Comment, this.ColumnName);
        }

    }
}
