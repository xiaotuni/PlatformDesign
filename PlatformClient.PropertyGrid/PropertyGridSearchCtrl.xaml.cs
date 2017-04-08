using System;
using System.Windows;
using System.Windows.Controls;
using PlatformClient.PropertyGrid.Events;

namespace PlatformClient.PropertyGrid
{
    /// <summary>
    /// 
    /// </summary>
    public partial class PropertyGridSearchCtrl : UserControl
    {
        /// <summary>
        /// 输入要搜索的内容
        /// </summary>
        internal event EventHandler<InputSearchEventArgs> InputSearch;
        /// <summary>
        /// 
        /// </summary>
        public PropertyGridSearchCtrl()
        {
            InitializeComponent();
        }

        void _InputSearchMethod(object sender, InputSearchEventArgs e)
        {
            if (null == InputSearch)
            {
                return;
            }
            InputSearch(sender, e);
        }

        private void txtInputValue_TextChanged(object sender, TextChangedEventArgs e)
        {
            _InputSearchMethod(this, new InputSearchEventArgs(this, this.txtInputValue.Text.Trim()));
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            this.txtInputValue.Text = string.Empty;
            txtInputValue_TextChanged(null, null);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            txtInputValue.TextChanged -= txtInputValue_TextChanged;
            btnClear.Click -= btnClear_Click;
        }
    }
}
