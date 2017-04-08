using System;
using System.Windows;
using System.Windows.Controls;

namespace PlatformClient.PageDesignTime.Controls
{
    /// <summary>
    /// 
    /// </summary>
    public partial class AddPageOrDirectory : UserControl, IDisposable
    {
        /// <summary>
        /// 点击事件
        /// </summary>
        public event EventHandler<RoutedEventArgs> Click;

        /// <summary>
        /// 获取和设计节点的名称
        /// </summary>
        public string GetNodeName { get { return this.txtName.Text; } set { this.txtName.Text = value; } }

        /// <summary>
        /// 
        /// </summary>
        public AddPageOrDirectory()
        {
            InitializeComponent();
        }

        void _ClickMethod(object sender, RoutedEventArgs e)
        {
            if (null != Click)
            {
                Click(sender, e);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            this.txtName.Text = string.Empty;
        }
    }
}
