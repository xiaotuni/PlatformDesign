using System.Windows.Controls;
using PlatformClient.Common.Interfaces;
using PlatformClient.Common.Lib;
using PlatformClient.Model.Method;

namespace PlatformClient.PageDesignTime
{
    /// <summary>
    /// 
    /// </summary>
    public partial class PCCanvas : Canvas
    {
        internal IPageDesignFramework IPageDesignFramework;
        private double _PageHeight;
        private double _PageWidth;
        /// <summary>
        /// 
        /// </summary>
        [PropertyInfoAttribute("PlatformClient.PageDesignTime.Controls;PlatformClient.PageDesignTime.Controls.PageLoadingControlCollection", "加载控件")]
        public object PageLadingItem { get; set; }
        /// <summary>
        /// 页面高
        /// </summary>
        [PropertyInfoAttribute("", "页面高")]
        public double PageHeight
        {
            get { return IPageDesignFramework.GetCurrentXmlTemplate().BaseInfo.PageHeight; }
            set
            {
                if (value.Equals(_PageHeight))
                {
                    return;
                }
                _PageHeight = value;
                IPageDesignFramework.GetCurrentXmlTemplate().BaseInfo.PageHeight = value;
                IPageDesignFramework.GetCurrentXamlTemplate().Height = value;
                IPageDesignFramework.UpdateCurrentTemplate();
            }
        }
        /// <summary>
        /// 页面宽
        /// </summary>
        [PropertyInfoAttribute("", "页面宽")]
        public double PageWidth
        {
            get { return IPageDesignFramework.GetCurrentXmlTemplate().BaseInfo.PageWidth; }
            set
            {
                if (value.Equals(_PageWidth))
                {
                    return;
                }
                _PageWidth = value;
                IPageDesignFramework.GetCurrentXmlTemplate().BaseInfo.PageWidth = value;
                IPageDesignFramework.GetCurrentXamlTemplate().Width = value;
                IPageDesignFramework.UpdateCurrentTemplate();
            }
        }
        /// <summary>
        /// 页面GUID
        /// </summary>
        [PropertyInfoAttribute("", "页面GUID")]
        public string PageGuid
        {
            get
            {
                return IPageDesignFramework.GetCurrentXmlTemplate().BaseInfo.PageGUID;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public PCCanvas()
        {
            InitializeComponent();
        }
    }
}
