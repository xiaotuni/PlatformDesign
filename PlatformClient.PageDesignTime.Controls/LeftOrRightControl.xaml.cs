using System.Windows;
using System.Windows.Controls;

namespace PlatformClient.PageDesignTime.Controls
{
    /// <summary>
    /// 
    /// </summary>
    public partial class LeftOrRightControl : UserControl
    {
        /// <summary>
        /// 到左边
        /// </summary>
        public event RoutedEventHandler ButtonLeft;
        /// <summary>
        /// 将所有都到左边
        /// </summary>
        public event RoutedEventHandler ButtonLeftAll;
        /// <summary>
        /// 到右边
        /// </summary>
        public event RoutedEventHandler ButtonRight;
        /// <summary>
        /// 将所有的都到右边
        /// </summary>
        public event RoutedEventHandler ButtonRightAll;

        Button _CurrentButton;

        /// <summary>
        /// 当前选择的Button
        /// </summary>
        public Button CurrentButton { get { return _CurrentButton; } }
        /// <summary>
        /// 
        /// </summary>
        public LeftOrRightControl()
        {
            InitializeComponent();

            this.btnLeft.Click += new RoutedEventHandler(btnLeft_Click);
            this.btnLeftAll.Click += new RoutedEventHandler(btnLeftAll_Click);
            this.btnRight.Click += new RoutedEventHandler(btnRight_Click);
            this.btnRightAll.Click += new RoutedEventHandler(btnRightAll_Click);
        }

        void btnRightAll_Click(object sender, RoutedEventArgs e)
        {
            _CurrentButton = sender as Button;
            if (null != ButtonRightAll)
            {
                ButtonRightAll(this, e);
            }
        }

        void btnRight_Click(object sender, RoutedEventArgs e)
        {
            _CurrentButton = sender as Button;
            if (null != ButtonRight)
            {
                ButtonRight(this, e);
            }
        }

        void btnLeftAll_Click(object sender, RoutedEventArgs e)
        {
            _CurrentButton = sender as Button;
            if (null != ButtonLeftAll)
            {
                ButtonLeftAll(this, e);
            }
        }

        void btnLeft_Click(object sender, RoutedEventArgs e)
        {
            _CurrentButton = sender as Button;
            if (null != ButtonLeft)
            {
                ButtonLeft(this, e);
            }
        }
    }
}
