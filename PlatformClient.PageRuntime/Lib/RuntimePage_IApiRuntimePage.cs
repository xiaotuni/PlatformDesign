using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using PlatformClient.Common.Interfaces;
using PlatformClient.Model.Method;

namespace PlatformClient.PageRuntime
{
    /// <summary>
    /// 
    /// </summary>
    public partial class RuntimePage : IApiRuntimePage
    {
        /// <summary>
        /// 弹出窗体
        /// </summary>
        /// <param name="cmd"></param>
        public void PopupForm(CtrlPlatformCommandInfo cmd)
        {
            XtnChildRunPage child = new XtnChildRunPage();
            RuntimePageManager rp = new RuntimePageManager();
            rp.OpenBizPageInNewTab(cmd.PageGuid);
            child.LayoutRoot.Children.Clear();
            child.LayoutRoot.Children.Add(rp);
            child.Closed += child_Closed;
            child.Show();
        }

        void child_Closed(object sender, EventArgs e)
        {
            var child = sender as XtnChildRunPage;
            child.Closed -= child_Closed;

            var id = child.LayoutRoot.Children[0] as IDisposable;
            if (null != id)
            {
                id.Dispose();
            }
            child.LayoutRoot.Children.Clear();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmd"></param>
        public void CloseForm(CtrlPlatformCommandInfo cmd)
        {
            this.IRuntimePageManager.CloseBizPage(cmd.PageGuid);
        }
        /// <summary>
        /// 切换页面
        /// </summary>
        /// <param name="cmd"></param>
        public void SwitchForm(CtrlPlatformCommandInfo cmd)
        {
            this.IRuntimePageManager.SwitchForm(cmd);
        }
        /// <summary>
        /// 刷新页面
        /// </summary>
        public void RefreshForm()
        {
            LoadInitControl();
        }
    }
}
