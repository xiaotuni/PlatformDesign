using System;
using System.Windows;
using PlatformClient.Common.Lib;
using PlatformClient.Model.Method;

namespace PlatformClient.Common.Interfaces
{
    /// <summary>
    /// 选择器接口
    /// </summary>
    public interface ISelectors : IPlatformClientChildWindow
    {
        /// <summary>
        /// 设计时接口
        /// </summary>
        IPageDesignFramework IDesignFramework { get; set; }
        /// <summary>
        /// 当前菜单信息
        /// </summary>
        ContextMenuInfo CurrentContextMenuInfo { get; set; }
        /// <summary>
        /// 当前选择Activity的IActivity接口
        /// </summary>
        IActivity IActivity { get; set; }
        /// <summary>
        /// 控件的名称
        /// </summary>
        string EventControlName { get; set; }
        /// <summary>
        /// 属性框里选中的事件名称
        /// </summary>
        string EventName { get; set; }
        /// <summary>
        /// 在设计时选中的控件
        /// </summary>
        FrameworkElement CurrentSelectedSetEventControl { get; set; }
        /// <summary>
        /// 当前选的Activity名称
        /// </summary>
        string ActivityName { get; set; }
        /// <summary>
        /// 当前的类型
        /// </summary>
        string ActivityType { get; set; }
        /// <summary>
        /// 事件信息属性
        /// </summary>
        EventInfoAttribute AttributeInfo { get; set; }
    }
}
