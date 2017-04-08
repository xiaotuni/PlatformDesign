using System;

namespace PlatformClient.Utility.Events
{
    /// <summary>
    /// 删除控件事件类
    /// </summary>
    public class ControlDeleteEventArgs : EventArgs
    {
        /// <summary>
        /// 控件名称
        /// </summary>
        public string ControlName { get; set; }
    }
}
