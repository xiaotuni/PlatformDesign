using System;
using System.Windows;
using PlatformClient.Common.Lib;
using PlatformClient.Model.Method;

namespace PlatformClient.Common.Interfaces
{
    /// <summary>
    /// 事件设计器接口
    /// </summary>
    public interface IEventDesigner : IUserControl, IPlatformClientChildWindow
    {
        /// <summary>
        /// 当前选中控件
        /// </summary>
        FrameworkElement CurrentSelectedControl { get; set; }
        /// <summary>
        /// 控件名称
        /// </summary>
        String EventControlName { get; set; }
        /// <summary>
        /// 事件名称
        /// </summary>
        String EventName { get; set; }
        /// <summary>
        /// 事件信息属性
        /// </summary>
        EventInfoAttribute AttributeInfo { get; set; }
    }
}
