using PlatformClient.Model.Method;

namespace PlatformClient.Common.Interfaces
{
    /// <summary>
    /// 运行时接口
    /// </summary>
    public interface IApiRuntimePage
    { /// <summary>
        /// 弹出窗体、页面
        /// </summary>
        /// <param name="cmd"></param>
        void PopupForm(CtrlPlatformCommandInfo cmd);
        /// <summary>
        /// 关闭窗体、页面
        /// </summary>
        /// <param name="cmd"></param>
        void CloseForm(CtrlPlatformCommandInfo cmd);
        /// <summary>
        /// 切换窗体、页面
        /// </summary>
        /// <param name="cmd"></param>
        void SwitchForm(CtrlPlatformCommandInfo cmd);
        /// <summary>
        /// 刷新窗体、页面
        /// </summary>
        void RefreshForm();
    }
}
