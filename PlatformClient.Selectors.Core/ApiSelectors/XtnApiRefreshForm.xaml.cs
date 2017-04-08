using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using PlatformClient.Common.Interfaces;
using PlatformClient.Extend.Core;
using PlatformClient.Model.Method;
using PlatformClient.Utility;

namespace PlatformClient.Selectors.Core.ApiSelectors
{
    public partial class XtnApiRefreshForm : UserControl, IApiSelector
    {
        public XtnApiRefreshForm()
        {
            InitializeComponent();

            this.Loaded += XtnApiSwitchForm_Loaded;
        }

        void XtnApiSwitchForm_Loaded(object sender, RoutedEventArgs e)
        {
            var xml = this.IDesignFramework.GetCurrentXmlTemplate();
            this.txtRelreshForm.Text = string.Format("刷新当前页面", xml.BaseInfo.PageName);
        }

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
        public EventInfoAttribute AttributeInfo { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public FrameworkElement CurrentSelectedSetEventControl { get; set; }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool CheckSave()
        {
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
            var _guid = this.IDesignFramework.GetCurrentXmlTemplate().BaseInfo.PageGUID;
            cai.Description = "刷新窗体" + _guid;
            cai.PageGuid = _guid;
            IDesignFramework.UpdateCurrentTemplate();
            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            this.Loaded -= XtnApiSwitchForm_Loaded;
        }
    }
}
