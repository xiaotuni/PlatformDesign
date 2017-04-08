using System;
using System.Windows;
using PlatformClient.Common.Lib;
using PlatformClient.Model.Method;

namespace PlatformClient.Common.Interfaces
{
    /// <summary>
    /// 选择器接口
    /// </summary>
    public interface IApiSelector : IDisposable
    {
        /// <summary>
        /// 设计时框架接口
        /// </summary>
        IPageDesignFramework IDesignFramework { get; set; }

        /// <summary>
        /// 事件发生的控件名称
        /// </summary>
        string EventControlName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        ApiSelector CurrentApiSelector { get; set; }

        /// <summary>
        /// 行为名称
        /// </summary>
        string ActivityName { get; set; }

        /// <summary>
        /// 行为类型
        /// </summary>
        string ActivityType { get; set; }

        /// <summary>
        /// 事件名称
        /// </summary>
        string EventName { get; set; }

        /// <summary>
        /// 检查是否保存
        /// </summary>
        bool CheckSave();
        /// <summary>
        /// 事件名称
        /// </summary>
        EventInfoAttribute AttributeInfo { get; set; }
        /// <summary>
        /// 
        /// </summary>
        FrameworkElement CurrentSelectedSetEventControl { get; set; }
    }
}
