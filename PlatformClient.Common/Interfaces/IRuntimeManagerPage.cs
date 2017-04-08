using System;
using PlatformClient.Model.Method;

namespace PlatformClient.Common.Interfaces
{
    /// <summary>
    /// 运行时管理接口
    /// </summary>
    public interface IRuntimeManagerPage : IDisposable
    {
        /// <summary>
        /// 父控件
        /// </summary>
        object ParentControl { get; set; }
        /// <summary>
        /// 打开一个新的页面
        /// </summary>
        /// <param name="bizPageGUID">页面GUID</param>
        void OpenBizPageInNewTab(string bizPageGUID);

        /// <summary>
        /// 关闭当前打开页面
        /// </summary>
        void CloseCurrentBizPage();

        /// <summary>
        /// 关闭当前打开的页面
        /// </summary>
        /// <param name="bizPageGUID">页面GUID</param>
        void CloseBizPage(string bizPageGUID);

        /// <summary>
        /// 关闭全部页面
        /// </summary>
        void CloseAllBizPage();

        ///// <summary>
        ///// 批量打开界面
        ///// </summary>
        ///// <param name="item">打开页面时的参数</param>
        ////void BatchOpenPage(List<UIPageParam> item);

        ///// <summary>
        ///// 刷新当前页面
        ///// </summary>
        ///// <param name="param">打开页面的参数</param>
        ///// <param name="currentPageGuid">当前页面的GUID</param>
        ////void RefreshPage(UIPageParam param, string currentPageGuid);

        /// <summary>
        /// 调用复合控件方法
        /// </summary>
        /// <param name="cmd"></param>
        void CallControlFunction(CtrlPlatformCommandInfo cmd);

        /// <summary>
        /// 批量打开页面
        /// </summary>
        /// <param name="cmd"></param>
        void BatchOpenPage(CtrlPlatformCommandInfo cmd);

        /// <summary>
        /// 切换窗体、页面
        /// </summary>
        /// <param name="cmd"></param>
        void SwitchForm(CtrlPlatformCommandInfo cmd);

        /// <summary>
        /// 刷新窗体、页面
        /// </summary>
        /// <param name="cmd"></param>
        void RefreshForm(CtrlPlatformCommandInfo cmd);

        /// <summary>
        /// 关闭所有窗体。
        /// </summary>
        /// <param name="cmd"></param>
        void RefreshAllForm(CtrlPlatformCommandInfo cmd);
    }
}
