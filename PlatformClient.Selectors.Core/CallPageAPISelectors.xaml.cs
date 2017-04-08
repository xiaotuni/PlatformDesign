using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Linq;
using System.Xml.Linq;
using PlatformClient.Common.Interfaces;
using PlatformClient.Common.Lib;
using PlatformClient.Model.Method;
using PlatformClient.Utility;
using PlatformClient.Utility.ParseXml;
using PlatformClient.Extend.Core;

namespace PlatformClient.Selectors.Core
{
    /// <summary>
    /// 
    /// </summary>
    public partial class CallPageAPISelectors : UserControl, ISelectors
    {
        IPageDesignFramework _IDesignFramework;
        /// <summary>
        /// 设计时，选中控件所处的xml信息
        /// </summary>
        XmlTemplate _XmlTemplate_Source;
        Dictionary<String, String> _DictResult = new Dictionary<string, String>();
        ApiSelector _CurrentApiSelector;
        UserControl _CurrentCreateControl;

        /// <summary>
        /// 是否全屏
        /// </summary>
        bool _IsFullscreen = true;
        private IApiSelector _ias;
        /// <summary>
        /// 是否全屏
        /// </summary>
        public bool IsFullscreen { get { return _IsFullscreen; } }

        /// <summary>
        /// 
        /// </summary>
        public IPageDesignFramework IDesignFramework
        {
            get { return _IDesignFramework; }
            set
            {
                _IDesignFramework = value;
                _XmlTemplate_Source = value.GetCurrentXmlTemplate();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public ContextMenuInfo CurrentContextMenuInfo { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public IActivity IActivity { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string EventControlName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string EventName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public FrameworkElement CurrentSelectedSetEventControl { get; set; }
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
        public EventInfoAttribute AttributeInfo { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            try
            {
                this.lb_Api.SelectionChanged -= lb_Api_SelectionChanged;
                this.Loaded -= CallPageAPISelectors_Loaded;
            }
            catch { }
        }

        /// <summary>
        /// 
        /// </summary>
        public CallPageAPISelectors()
        {
            InitializeComponent();

            this.lb_Api.SelectionChanged += lb_Api_SelectionChanged;
            this.Loaded += CallPageAPISelectors_Loaded;

        }

        void CallPageAPISelectors_Loaded(object sender, RoutedEventArgs e)
        {
            InitLoadApi();

            BindControlEvent(this.IDesignFramework.GetCurrentXmlTemplate());
        }

        /// <summary>
        /// 绑定控件事件
        /// </summary>
        /// <param name="xml"></param>
        void BindControlEvent(XmlTemplate xml)
        {
            if (AttributeInfo.AssemblyQualifiedName.IsNullOrEmpty())
            {
                return;
            }
            var _ebi = xml.EventBindItem.Where(p => p.ControlName.Equals(this.EventControlName)).GetFirst<EventBindInfo>();
            if (null == _ebi)
            {
                AddCtrlBindEvent(xml);
            }
            else
            {
                //-->获取指定的事件
                var _cbei = _ebi.Item.Where(p => p.EventName.Equals(this.EventName)).GetFirst<CtrlBindEventInfo>();
                if (null == _cbei)      //-->说明事件不存在，增加进去 
                {
                    AddCtrlBindEvent(xml);
                }
            }
        }

        /// <summary>
        /// 增加控件绑定事件
        /// </summary>
        void AddCtrlBindEvent(XmlTemplate xml)
        {
            var _cc = ParseControlConfig.GetControlConfig(CurrentSelectedSetEventControl.GetType().Name);
            var _be = _cc.Events.Where(p => p.Name.Equals(this.EventName)).GetFirst<BindEvent>();

            EventBindInfo _ebi = new EventBindInfo();
            _ebi.ControlName = this.EventControlName;
            _ebi.Item = new List<CtrlBindEventInfo>();

            CtrlBindEventInfo _cbei = new CtrlBindEventInfo();
            _cbei.EventName = this.EventName;
            _cbei.AssemblyName = _be.AssemblyName;
            _cbei.BindFunctionName = _be.BindFunctionName;
            _ebi.Item.Add(_cbei);

            xml.EventBindItem.Add(_ebi);
            IDesignFramework.UpdateCurrentTemplate();
        }

        /// <summary>
        /// 初始化加载Api参数设置控件
        /// </summary>
        void InitLoadApi()
        {
            var items = ParseActivityConfig.ApiSelectorItem;
            this.lb_Api.Items.Clear();
            this.lb_Api.ItemsSource = items;
        }

        void lb_Api_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.sp_Panel.Children.Clear();
            ListBox lb = sender as ListBox;
            _CurrentApiSelector = lb.SelectedItem as ApiSelector;
            _CurrentCreateControl = Wrapper.CreateControl<UserControl>(_CurrentApiSelector.AssemblyQualifiedName);
            _ias = _CurrentCreateControl as IApiSelector;
            if (null == _ias)
            {
                return;
            }
            _ias.ActivityName = this.ActivityName;
            _ias.ActivityType = this.ActivityType;
            _ias.EventControlName = this.EventControlName;
            _ias.EventName = this.EventName;
            _ias.CurrentApiSelector = _CurrentApiSelector;
            _ias.IDesignFramework = this.IDesignFramework;
            _ias.AttributeInfo = this.AttributeInfo;
            _ias.CurrentSelectedSetEventControl = this.CurrentSelectedSetEventControl;
            this.sp_Panel.Children.Add(_CurrentCreateControl);
        }

        /// <summary>
        /// 
        /// </summary>
        public bool CheckSave()
        {
            return _ias != null ? _ias.CheckSave() : true;
        }
    }
}
