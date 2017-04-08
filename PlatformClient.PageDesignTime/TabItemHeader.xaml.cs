using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PlatformClient.PageDesignTime
{
    /// <summary>
    /// 
    /// </summary>
    public partial class TabItemHeader : UserControl
    {
        /// <summary>
        /// 关闭Tab标签
        /// </summary>
        public event RoutedEventHandler OnCloseTabItem;
        /// <summary>
        /// 
        /// </summary>
        public TabItemHeader()
        {
            InitializeComponent();
            this.btnDelete.Click += btnDelete_Click;
        }

        void _DeleteMethod(object sender, RoutedEventArgs e)
        {
            if (null != OnCloseTabItem)
            {
                OnCloseTabItem(sender, e);
            }
        }

        void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            _DeleteMethod(this, e);
        }

        /// <summary>
        /// 标题
        /// </summary>
        internal string Header
        {
            get
            {
                return this.txtTitle.Text;
            }
            set
            {
                this.txtTitle.Text = string.Format("{0}", value);
            }
        }

        /// <summary>
        /// 设计是否有更新
        /// </summary>
        /// <param name="value"></param>
        internal void SetUpdateImage(bool value)
        {
            if (value)
            {
                this.imageIsUpdate.Visibility = Visibility.Visible;
            }
            else
            {
                this.imageIsUpdate.Visibility = Visibility.Collapsed;
            }
        }
    }
}
