using System;
using System.Collections.Generic;

namespace PlatformClient.Model.Method
{
    /// <summary>
    /// 事件链信息
    /// </summary>
    public class EventLinkInfo
    {
        /// <summary>
        /// 控件名称,当前点击的控件
        /// </summary>
        public String ControlName { get; set; }
        /// <summary>
        /// 事件名称
        /// </summary>
        public string EventName { get; set; }
        /// <summary>
        /// 事件链集合
        /// </summary>
        public List<ControlActivityInfo> Item { get; set; }
    }
}
