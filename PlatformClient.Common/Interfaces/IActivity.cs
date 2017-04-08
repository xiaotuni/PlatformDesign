using System.Collections.Generic;
using PlatformClient.Common.Lib;
using PlatformClient.Model.Method;

namespace PlatformClient.Common.Interfaces
{
    /// <summary>
    /// 活动控件[圆形、矩形等]
    /// </summary>
    public interface IActivity : IBaseActivity
    {
        /// <summary>
        /// 是否可以连接上
        /// </summary>
        /// <returns></returns>
        bool IsConnection();
        /// <summary>
        /// 是否开始
        /// </summary>
        /// <returns></returns>
        bool IsStart();
        /// <summary>
        /// 右键菜单
        /// </summary>
        /// <returns></returns>
        List<ContextMenuInfo> ContextMenu();
        /// <summary>
        /// 判断当前Activity是否合法
        /// </summary>
        /// <returns></returns>
        bool IsCheck();
    }
}
