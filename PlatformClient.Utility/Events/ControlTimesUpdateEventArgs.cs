using System;
using System.Collections.Generic;
using PlatformClient.Model.Method;

namespace PlatformClient.Utility.Events
{
    /// <summary>
    /// 控件数量发生变化事件类
    /// </summary>
    public class ControlTimesUpdateEventArgs : EventArgs
    {
        /// <summary>
        /// 控件集合
        /// </summary>
        public List<ControlInfo> Controls { get; set; }
    }
}
