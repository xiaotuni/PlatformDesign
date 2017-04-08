using System.Windows.Controls;
using System.Windows.Input;

namespace PlatformClient.PageRuntime
{
    /// <summary>
    /// 
    /// </summary>
    public partial class XtnChildRunPage : ChildWindow
    {
        /// <summary>
        /// 
        /// </summary>
        public XtnChildRunPage()
        {
            InitializeComponent();
            this.KeyUp += XtnChildRunPage_KeyUp;
        }

        void XtnChildRunPage_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.Close();
            }
        }
    }
}

