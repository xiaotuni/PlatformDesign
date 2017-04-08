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
    public partial class XtnApiSwitchForm : UserControl, IApiSelector
    {
        private string _PageGuid;
        private string _CurrentSelectedGuid;
        public XtnApiSwitchForm()
        {
            InitializeComponent();
            this.Loaded += XtnApiSwitchForm_Loaded;
            this.pft.OpenPageInfo += pft_OpenPageInfo;
        }

        void pft_OpenPageInfo(object sender, Model.Events.OpenPageInfoEventArgs e)
        {
            var pds = e.PageDirectorySub;
            _CurrentSelectedGuid = pds.PageGuid;
            if (_CurrentSelectedGuid.Equals(_PageGuid))
            {
                Wrapper.ShowDialog("自己不能切换到自己，你是否要重置刷新一下页面的内容？是的话请设置页面刷新即可");
                return;
            }
            else
            {
                this.txtSwitchForm.Text = string.Format("要从当前【{0}】页面切换到【{1}】页面", _PageGuid, _CurrentSelectedGuid);
            }
        }

        void XtnApiSwitchForm_Loaded(object sender, RoutedEventArgs e)
        {
            var xml = this.IDesignFramework.GetCurrentXmlTemplate();
            _PageGuid = xml.BaseInfo.PageGUID;
            this.txtSwitchForm.Text = string.Format("要从当前【{0}】页面切换到【{1}】页面", xml.BaseInfo.PageName, "");
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
            if (_CurrentSelectedGuid.IsNullOrEmpty())
            {
                Wrapper.ShowDialog("请选择要切换的窗体");
                return false;
            }
            if (_CurrentSelectedGuid.Equals(_PageGuid))
            {
                Wrapper.ShowDialog("自己不能切换到自己，你是否要重置刷新一下页面的内容？是的话请设置页面刷新即可");
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
            var cai = eli.Item.Where(p => p.ControlName.IsNotEmpty() &&
                                            p.ControlName.Equals(_controlName) &&
                                            p.EventName.IsNotEmpty() &&
                                            p.EventName.Equals(eventName)).GetFirst<ControlActivityInfo>();
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
            cai.Description = "切换窗体" + _guid;
            cai.PageGuid = string.Format("{0},{1}", _CurrentSelectedGuid, _guid);
            IDesignFramework.UpdateCurrentTemplate();
            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            this.Loaded -= XtnApiSwitchForm_Loaded;
            this.pft.OpenPageInfo -= pft_OpenPageInfo;
            this.pft.Dispose();
        }
    }
}
