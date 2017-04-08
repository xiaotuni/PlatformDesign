using System.Collections.Generic;

namespace PlatformClient.Model.Method
{
    /// <summary>
    /// 事件绑定类
    /// </summary>
    public class EventBindInfo
    {
        /// <summary>
        /// 控件名称
        /// </summary>
        public string ControlName { get; set; }
        /// <summary>
        /// 控件绑定事件集合
        /// </summary>
        public List<CtrlBindEventInfo> Item { get; set; }

    }
}
