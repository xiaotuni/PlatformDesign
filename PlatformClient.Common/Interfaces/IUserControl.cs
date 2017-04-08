using System;
using System.Xml.Linq;

namespace PlatformClient.Common.Interfaces
{
    /// <summary>
    /// 用户操作接口
    /// </summary>
    public interface IUserControl : IDisposable
    {
        /// <summary>
        /// 属性值 
        ///   {name= '属性名称' value='值' }
        /// 或
        ///   {name='属性名称'}
        ///      内容
        ///   {/name}
        /// </summary>
        /// <returns></returns>
        XElement EvaluationContent { get; set; }

        /// <summary>
        /// 设计时框架接口
        /// </summary>
        IPageDesignFramework IDesignFramework { get; set; }

        /// <summary>
        /// 当前选中的控件
        /// </summary>
        string CurrentSelectedControlName { get; set; }
    }

    /// <summary>
    /// 设计时用户控件，主要是用于弹出窗体时控件使用此接口
    /// </summary>
    public interface IPageDesignTimeUserControl : IUserControl, IPlatformClientChildWindow
    {
    }
}
