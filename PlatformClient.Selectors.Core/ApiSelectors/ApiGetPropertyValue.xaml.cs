using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using PlatformClient.Common.Interfaces;
using PlatformClient.Common.Lib;
using PlatformClient.Extend.Core;
using PlatformClient.Model.Events;
using PlatformClient.Model.Method;
using PlatformClient.Utility;

namespace PlatformClient.Selectors.Core.ApiSelectors
{
    /// <summary>
    /// 获取属性值
    /// </summary>
    public partial class ApiGetPropertyValue : UserControl, IApiSelector
    {
        /// <summary>
        /// 
        /// </summary>
        public FrameworkElement CurrentSelectedSetEventControl { get; set; }
        Dictionary<String, String> _DictResult = new Dictionary<string, String>();
        IPageDesignFramework _IDesignFramework;
        XmlTemplate _XmlTemplate_Source;
        XmlTemplate _XmlTemplate_CurrentPage;
        ControlActivityInfo _CurrentActivityInfo;
        int _CurrentActivityInfoIndex = -1;
        int _CurrentControlEventIndex = -1;
        EventLinkInfo _CurrentControlEventInfo;
        UserControl _CurrentPageControl;
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
        public EventInfoAttribute AttributeInfo { get; set; }
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
        public ApiGetPropertyValue()
        {
            InitializeComponent();

            this.btnRight.Click += btnRight_Click;
            this.btnLeft.Click += btnLeft_Click;
            this.btnLeftAll.Click += btnLeftAll_Click;
            this.btnRightAll.Click += btnRightAll_Click;
            this.btnOK.Click += btnOK_Click;
            this.pftm.OpenPageInfo += pftm_OpenPageInfo;
            this.Loaded += ApiGetPropertyValue_Loaded;
        }

        void pftm_OpenPageInfo(object sender, OpenPageInfoEventArgs e)
        {
            _XmlTemplate_CurrentPage = this.pftm.SelectedPageInfo;
            _CurrentPageControl = this.pftm.PageControl;
        }

        void ApiGetPropertyValue_Loaded(object sender, RoutedEventArgs e)
        {
            string _controlName = Wrapper.ParseControlName(EventControlName);

            _CurrentControlEventInfo = _XmlTemplate_Source.EventLinkItem.Where(p => p.ControlName == _controlName && p.EventName == EventName).GetFirst<EventLinkInfo>();
            if (null != _CurrentControlEventInfo)
            {
                _CurrentControlEventIndex = _XmlTemplate_Source.EventLinkItem.IndexOf(_CurrentControlEventInfo);
                _CurrentActivityInfo = _CurrentControlEventInfo.Item.Where(p => p.ActivityName == this.ActivityName).GetFirst<ControlActivityInfo>();
                if (null == _CurrentActivityInfo)
                {
                    _CurrentActivityInfo = new ControlActivityInfo();
                    _CurrentActivityInfo.Param = new List<ParamInfo>();
                }
                else
                {
                    _CurrentActivityInfoIndex = _CurrentControlEventInfo.Item.IndexOf(_CurrentActivityInfo);
                    //--> init page info
                    //--> fill lb_result control;
                    if (null != _CurrentActivityInfo.Param)
                    {
                        foreach (var v in _CurrentActivityInfo.Param)
                        {
                            string _key = string.Format("{0}|{1}", v.TargetCtrlName, v.TargetFieldName);
                            _DictResult.Add(_key, v.TargetFieldType);
                        }
                        BindResultControl();
                    }
                }//End if;
            }
            else
            {
                _CurrentControlEventInfo = new EventLinkInfo();
                _CurrentControlEventInfo.EventName = this.EventName;
                _CurrentControlEventInfo.ControlName = _controlName;
                _CurrentControlEventInfo.Item = new List<ControlActivityInfo>();

                _CurrentActivityInfo = new ControlActivityInfo();
                _CurrentActivityInfo.Param = new List<ParamInfo>();
            }
        }

        void btnOK_Click(object sender, RoutedEventArgs e)
        {
            UpdateDesignPage();
            MessageBox.Show("成功");
        }

        void UpdateDesignPage()
        {
            string eventName = string.Format("{0}", CurrentApiSelector.Name);
            string controlName = string.Format("{0}", this.EventControlName);

            //-->保存操作。
            _CurrentActivityInfo.ActivityName = this.ActivityName;
            _CurrentActivityInfo.ActivityType = this.ActivityType;
            _CurrentActivityInfo.ControlName = controlName;
            _CurrentActivityInfo.EventName = eventName;
            _CurrentActivityInfo.Description = CurrentApiSelector.Description;
            _CurrentActivityInfo.Param = new List<ParamInfo>();
            foreach (var v in _DictResult)
            {
                string[] arrayItem = v.Key.Split('|');
                ParamInfo pi = new ParamInfo();
                pi.TargetCtrlName = arrayItem[0];
                pi.TargetFieldName = arrayItem[1];
                pi.TargetFieldType = v.Value;

                _CurrentActivityInfo.Param.Add(pi);
            }
            if (0 <= _CurrentActivityInfoIndex)
            {
                _CurrentControlEventInfo.Item.RemoveAt(_CurrentActivityInfoIndex);
            }
            else
            {
                _CurrentActivityInfoIndex = 0;
            }
            _CurrentControlEventInfo.Item.Add(_CurrentActivityInfo);
            if (0 <= _CurrentControlEventIndex)
            {
                _XmlTemplate_Source.EventLinkItem.RemoveAt(_CurrentControlEventIndex);
            }
            else
            {
                _CurrentActivityInfoIndex = 0;
            }
            _XmlTemplate_Source.EventLinkItem.Add(_CurrentControlEventInfo);

            IDesignFramework.GetCurrentXmlTemplate().EventLinkItem = _XmlTemplate_Source.EventLinkItem;
            IDesignFramework.UpdateCurrentTemplate();
        }

        void btnRightAll_Click(object sender, RoutedEventArgs e)
        {
            var _CurrentControlInfo = pftm.SelectedControlInfo;
            foreach (var v in this.pftm.SelectedControlPropertyItem)
            {
                GetControlProperty(_CurrentControlInfo, v);
            }
            BindResultControl();
        }

        void GetControlProperty(ControlInfo _CurrentControlInfo, PropertyInfoAttribute pia)
        {
            //-->哪个控件，哪个属性
            //PropertyInfoAttribute pia = v as PropertyInfoAttribute;
            if (null == pia)
            {
                return;
            }
            //-->得到选中的控件
            string controlName = string.Format("{0}|{1}", _CurrentControlInfo.Name, pia.Name);
            if (_DictResult.ContainsKey(controlName))
            {
                return;
            }
            string dataType = "varchar";
            MetaDataInfo mdi = this.IDesignFramework.GetMetaDataInfoByTableName(_CurrentControlInfo.MetaData);
            if (null == mdi)
            {
                var controlObj = _CurrentPageControl.FindName(_CurrentControlInfo.Name);
                if (null != controlObj)
                {
                    var pi = controlObj.GetType().GetProperty(pia.Name);
                    dataType = pi != null ? pi.PropertyType.FullName : dataType;
                }
            }
            else
            {
                MetaDataColumnInfo mdci = mdi.Item.Where(p => p.column_name.Equals(_CurrentControlInfo.ColumnName)).GetFirst<MetaDataColumnInfo>();
            }
            _DictResult.Add(controlName, dataType);
        }

        void btnRight_Click(object sender, RoutedEventArgs e)
        {
            //-->哪个控件，哪个属性
            PropertyInfoAttribute pia = pftm.SelectedControlProperty;
            var _CurrentControlInfo = pftm.SelectedControlInfo;
            GetControlProperty(_CurrentControlInfo, pia);
            BindResultControl();
        }

        void btnLeftAll_Click(object sender, RoutedEventArgs e)
        {
            _DictResult.Clear();
            BindResultControl();
        }

        void btnLeft_Click(object sender, RoutedEventArgs e)
        {
            if (null == this.lb_Result.SelectedItem)
            {
                return;
            }
            string controlName = string.Format("{0}", this.lb_Result.SelectedItem);
            _DictResult.Remove(controlName);
            BindResultControl();
        }

        void BindResultControl()
        {
            this.lb_Result.ItemsSource = null;
            this.lb_Result.ItemsSource = _DictResult.Keys;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
        }

        /// <summary>
        /// 弹出窗体
        /// </summary>
        /// <returns></returns>
        public bool CheckSave()
        {
            return true;
        }
    }
}
