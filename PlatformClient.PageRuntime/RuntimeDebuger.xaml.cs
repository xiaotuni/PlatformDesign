using System.Windows;
using System.Windows.Controls;
using PlatformClient.Common.Interfaces;

namespace PlatformClient.PageRuntime
{
    /// <summary>
    /// 运行时管理页面
    /// </summary>
    public partial class RuntimeDebuger : UserControl
    {
        /// <summary>
        /// 
        /// </summary>
        public IRuntimeManagerPage IRuntimeManager;
        /// <summary>
        /// 
        /// </summary>
        public RuntimeDebuger()
        {
            InitializeComponent();
        }

        void btnOpenPage_Click(object sender, RoutedEventArgs e)
        {
            IRuntimeManager.OpenBizPageInNewTab(this.txtGUID.Text.Trim());
        }

        void btnCloseCurrentPage_Click(object sender, RoutedEventArgs e)
        {
            IRuntimeManager.CloseCurrentBizPage();
        }

        void btnCloseAllPage_Click(object sender, RoutedEventArgs e)
        {
            IRuntimeManager.CloseAllBizPage();
        }
    }
}
