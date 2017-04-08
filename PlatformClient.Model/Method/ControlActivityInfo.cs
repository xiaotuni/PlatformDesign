using System;
using System.Collections.Generic;

namespace PlatformClient.Model.Method
{
    /// <summary>
    /// 
    /// </summary>
    public class ControlActivityInfo
    {
        /// <summary>
        /// 控件名称
        /// </summary>
        public String ControlName { get; set; }
        /// <summary>
        /// 事件名称
        /// </summary>
        public String EventName { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public String Description { get; set; }

        /// <summary>
        /// 行为类型
        /// </summary>
        public string ActivityType { get; set; }

        /// <summary>
        /// 行为名称
        /// </summary>
        public string ActivityName { get; set; }

        /// <summary>
        /// 参数
        /// </summary>
        public List<ParamInfo> Param { get; set; }
        /// <summary>
        /// 弹出窗体的GUID
        /// </summary>
        public string PageGuid { get; set; }
    }
}
