using System;
using System.Windows.Controls;
using System.Windows.Input;
using PlatformClient.Common.Interfaces;
using PlatformClient.PageDesignTime.Lib;
using PlatformClient.Model.Method;

namespace PlatformClient.PageDesignTime
{
    /// <summary>
    /// 设计矩形
    /// </summary>
    public partial class DesignRectangle : UserControl
    {
        /// <summary>
        /// 大小改变事件
        /// </summary>
        public event EventHandler<ResizeEventArgs> ResizeChanged;
        /// <summary>
        /// 鼠标左键弹起事件
        /// </summary>
        public event MouseButtonEventHandler MouseLeftUp;
        void _ResizeChangedMethod(object sender, ResizeEventArgs e)
        {
            if (null != ResizeChanged)
            {
                ResizeChanged(sender, e);
            }
        }
        void _MouseLeftUpMethod(object sender, MouseButtonEventArgs e)
        {
            if (null != MouseLeftUp)
            {
                MouseLeftUp(sender, e);
            }
        }
        /// <summary>
        /// 右上方位尺寸变化
        /// </summary>
        ResizeHelper TRHelper = new ResizeHelper();
        /// <summary>
        /// 上中方位尺寸变化
        /// </summary>
        ResizeHelper TCHelper = new ResizeHelper();
        /// <summary>
        /// 左上方位尺寸变化
        /// </summary>
        ResizeHelper TLHelper = new ResizeHelper();
        ResizeHelper BRHelepr = new ResizeHelper();
        ResizeHelper BCHelper = new ResizeHelper();
        ResizeHelper BLHelper = new ResizeHelper();
        ResizeHelper CLHelper = new ResizeHelper();
        ResizeHelper CRHelper = new ResizeHelper();
        /// <summary>
        /// 结构函数
        /// </summary>
        public DesignRectangle()
        {
            InitializeComponent();

            ancTR.Location = AuchorLocation.上右;
            SetResizeEvent(ancTR, TRHelper);

            ancTC.Location = AuchorLocation.上中;
            SetResizeEvent(ancTC, TCHelper);

            ancTL.Location = AuchorLocation.上左;
            SetResizeEvent(ancTL, TLHelper);

            ancBC.Location = AuchorLocation.下中;
            SetResizeEvent(ancBC, BCHelper);

            ancBL.Location = AuchorLocation.下左;
            SetResizeEvent(ancBL, BLHelper);

            ancBR.Location = AuchorLocation.下右;
            SetResizeEvent(ancBR, BRHelepr);

            ancCL.Location = AuchorLocation.左中;
            SetResizeEvent(ancCL, CLHelper);

            ancCR.Location = AuchorLocation.右中;
            SetResizeEvent(ancCR, CRHelper);
        }

        void SetResizeEvent(Auchor anc, ResizeHelper rh)
        {
            rh.BindResizeFunction(anc, this);
            rh.MouseLeftButtonUp += new MouseButtonEventHandler(ResizeHelper_MouseLeftButtonUp);
            rh.ResizeChanged += new EventHandler<ResizeEventArgs>(ResizeHelper_ResizeChanged);
        }

        void ResizeHelper_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _MouseLeftUpMethod(this, e);
        }

        void ResizeHelper_ResizeChanged(object sender, ResizeEventArgs e)
        {
            _ResizeChangedMethod(this, e);
        }
    }
}
