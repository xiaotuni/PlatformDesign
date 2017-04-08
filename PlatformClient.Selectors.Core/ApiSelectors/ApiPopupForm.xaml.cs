using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using PlatformClient.Common.Interfaces;
using PlatformClient.Extend.Core;
using PlatformClient.Model.Events;
using PlatformClient.Model.Method;
using PlatformClient.Utility;

namespace PlatformClient.Selectors.Core.ApiSelectors
{
    /// <summary>
    /// 弹出窗体
    /// </summary>
    public partial class ApiPopupForm : UserControl, IApiSelector
    {
        /// <summary>
        /// 
        /// </summary>
        public FrameworkElement CurrentSelectedSetEventControl { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public EventInfoAttribute AttributeInfo { get; set; }
        private string _PageGuid;
        /// <summary>
        /// 
        /// </summary>
        public IPageDesignFramework IDesignFramework { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string EventControlName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ApiSelector CurrentApiSelector { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ActivityName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ActivityType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string EventName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public ApiPopupForm()
        {
            InitializeComponent();
            this.Loaded += ApiPopupForm_Loaded;
        }

        void ApiPopupForm_Loaded(object sender, RoutedEventArgs e)
        {
            pft.OpenPageInfo += pft_OpenPageInfo;
        }

        void pft_OpenPageInfo(object sender, OpenPageInfoEventArgs e)
        {
            if (null == e)
            {
                return;
            }
            _PageGuid = e.PageDirectorySub.PageGuid;
        }

        /// <summary>
        /// 弹出窗体
        /// </summary>
        /// <returns></returns>
        public bool CheckSave()
        {
            if (_PageGuid.IsNullOrEmpty())
            {
                Wrapper.ShowDialog("请选择要弹出的页面");
                return false;
            }
            //-->判断是否是当前的页面?
            if (this.IDesignFramework.GetCurrentXmlTemplate().BaseInfo.PageGUID.Equals(_PageGuid))
            {
                Wrapper.ShowDialog("不能弹出当前页面,请选择其它弹出页面。");
                return false;
            }

            string eventName = string.Format("{0}", CurrentApiSelector.Name);
            string _controlName = string.Format("{0}", this.EventControlName);
            var xml = IDesignFramework.GetCurrentXmlTemplate();
            if (null == xml.EventLinkItem)
            {
                xml.EventLinkItem = new List<EventLinkInfo>();
            }
            //-->事件
            var eli = xml.EventLinkItem.Where(p => p.ControlName == _controlName && p.EventName == EventName).GetFirst<EventLinkInfo>();
            if (null == eli)
            {
                eli = new EventLinkInfo();
                xml.EventLinkItem.Add(eli);
            }
            eli.ControlName = _controlName;
            eli.EventName = this.EventName;
            if (null == eli.Item)
            {
                eli.Item = new List<ControlActivityInfo>();
            }
            //-->行为
            var cai = eli.Item.Where(p => p.ControlName.Equals(_controlName) && p.EventName.Equals(eventName)).GetFirst<ControlActivityInfo>();
            if (null == cai)
            {
                cai = new ControlActivityInfo();
                eli.Item.Add(cai);
            }
            //-->保存操作。
            cai.ActivityName = this.ActivityName;
            cai.ActivityType = this.ActivityType;
            cai.ControlName = _controlName;
            cai.EventName = eventName;
            cai.Description = "弹出窗体" + _PageGuid;
            cai.PageGuid = _PageGuid;
            //-->参数，初始化弹出窗体的时候，要有参数

            IDesignFramework.UpdateCurrentTemplate();
            return true;
        }

    }
}
