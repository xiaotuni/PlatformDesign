using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using PlatformClient.Common.Interfaces;
using PlatformClient.Common.Lib;
using PlatformClient.Extend.Core;
using PlatformClient.Model.Method;
using PlatformClient.Utility;

namespace PlatformClient.Selectors.Core.ApiSelectors
{
    /// <summary>
    /// 设置属性值
    /// </summary>
    public partial class ApiSetPropertyValue : UserControl, IApiSelector
    {
        /// <summary>
        /// 
        /// </summary>
        public FrameworkElement CurrentSelectedSetEventControl { get; set; }
        XmlTemplate _XmlTemplate;
        ParameterSelected _CurrentParamterSelected;
        EventLinkInfo _CurrentControlEventInfo;
        ControlActivityInfo _CurrentActivityInfo;
        /// <summary>
        /// 当前控件事件所在的索引号
        /// </summary>
        int _CurrentControlEventIndex = -1;
        /// <summary>
        /// 当前事件行为控件所在索引号
        /// </summary>
        int _CurrentActivityInfoIndex = -1;
        /// <summary>
        /// 
        /// </summary>
        public EventInfoAttribute AttributeInfo { get; set; }
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
            this.Loaded -= ApiSetPropertyValue_Loaded;
            this.ct.SelectedItemChanged -= ct_SelectedItemChanged;
        }

        /// <summary>
        /// 
        /// </summary>
        public ApiSetPropertyValue()
        {
            InitializeComponent();

            this.Loaded += ApiSetPropertyValue_Loaded;
            this.ct.SelectedItemChanged += ct_SelectedItemChanged;
            this.pfm.SelectionControlProperty += pfm_SelectionControlProperty;
            this.btnOK.Click += btnOK_Click;
        }

        void btnOK_Click(object sender, RoutedEventArgs e)
        {
            UpdateDesignPage();
            MessageBox.Show("成功");
        }

        void UpdateDesignPage()
        {
            string eventName = string.Format("{0}", CurrentApiSelector.Name);
            string controlName = string.Format("{0}", ct.SelectedControlInfo.Name);

            //-->保存操作。
            _CurrentActivityInfo.ActivityName = this.ActivityName;
            _CurrentActivityInfo.ActivityType = this.ActivityType;
            _CurrentActivityInfo.ControlName = controlName;
            _CurrentActivityInfo.EventName = eventName;
            _CurrentActivityInfo.Description = CurrentApiSelector.Description;
            _CurrentActivityInfo.Param = new List<ParamInfo>();
            foreach (var v in this.sp_Children.Children)
            {
                ParameterSelected select = v as ParameterSelected;
                if (null == select || null == select.ParamForamt)
                {
                    continue;
                }
                _CurrentActivityInfo.Param.Add(select.ParamForamt);
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
                _XmlTemplate.EventLinkItem.RemoveAt(_CurrentControlEventIndex);
            }
            else
            {
                _CurrentControlEventIndex = 0;
            }
            _XmlTemplate.EventLinkItem.Add(_CurrentControlEventInfo);

            IDesignFramework.GetCurrentXmlTemplate().EventLinkItem = _XmlTemplate.EventLinkItem;
            IDesignFramework.UpdateCurrentTemplate();
        }

        void pfm_SelectionControlProperty(object sender, SelectionChangedEventArgs e)
        {
            var cp = this.pfm.SelectedControlProperty;
            var ci = this.pfm.SelectedControlInfo;
            //-->将选中的属性设置到 _CurrentParamterSelected 上里去了。
            var datas = _GetControlParameter(ci.MetaData, ci.ColumnName);
            if (null == datas || 0 == datas.Count())
            {
                return;
            }
            var data = datas[0];
            _CurrentParamterSelected.Text = data.column_comment;
            _CurrentParamterSelected.TargetMetaDataColumn = data;
            _CurrentParamterSelected.OrgCtrlInfo = ci;
            _CurrentParamterSelected.TargetCtrlName = string.Format("{0}", ci.Name);
        }

        void ApiSetPropertyValue_Loaded(object sender, RoutedEventArgs e)
        {
            _XmlTemplate = this.IDesignFramework.GetCurrentXmlTemplate();
            this.ct.ShowTree(_XmlTemplate.ControlItem);

            InitParameter();
        }

        void InitParameter()
        {
            //-->获取出所有控件的方法来，来至两个地方，第一个接口，第二是控件自己的方法
            _XmlTemplate = IDesignFramework.GetCurrentXmlTemplate();
            string _controlName = Wrapper.ParseControlName(EventControlName);
            _CurrentControlEventInfo = _XmlTemplate.EventLinkItem.Where(p => p.ControlName == _controlName && p.EventName == EventName).GetFirst<EventLinkInfo>();
            if (null != _CurrentControlEventInfo)
            {
                _CurrentControlEventIndex = _XmlTemplate.EventLinkItem.IndexOf(_CurrentControlEventInfo);
                _CurrentActivityInfo = _CurrentControlEventInfo.Item.Where(p => p.ActivityName == this.ActivityName).GetFirst<ControlActivityInfo>();
                if (null == _CurrentActivityInfo)
                {
                    _CurrentActivityInfo = new ControlActivityInfo();
                    _CurrentActivityInfo.Param = new List<ParamInfo>();
                }
                else
                {
                    _CurrentActivityInfoIndex = _CurrentControlEventInfo.Item.IndexOf(_CurrentActivityInfo);
                    //-->1、初始化界面信息
                    if (null == _CurrentActivityInfo.Param || 0 == _CurrentActivityInfo.Param.Count)
                    {
                        _CurrentActivityInfo.Param = new List<ParamInfo>();
                        return;
                    }
                    //-->获取出是哪个控件。
                    ct.SetSelectionControl(_CurrentActivityInfo.ControlName);
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

        void ct_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            sp_Children.Children.Clear();
            var ci = ct.SelectedControlInfo;
            if (null == ci)
            {
                return;
            }
            var parameterItem = _GetControlParameter(ci.MetaData, ci.ColumnName);
            //-->控件的参数就是控件MetaData里的row信息
            //-->得行的参数了
            if (null == parameterItem)
            {
                return;
            }

            string _controlName = Wrapper.ParseControlName(ci.Name);
            foreach (var v in parameterItem)
            {
                ParameterSelected ps = new ParameterSelected();
                ps.txtParamName.Text = string.Format("{0}【{1}】", v.column_name, v.column_comment);
                ps.OrgMetaDataColumn = v;
                ps.FieldName = v.column_name;
                ps.DeleteClick += new RoutedEventHandler(ps_DeleteClick);
                ps.ModifyClick += new RoutedEventHandler(ps_ModifyClick);
                sp_Children.Children.Add(ps);

                if (!_controlName.Equals(_CurrentActivityInfo.ControlName))
                {
                    continue;
                }
                var pi = _CurrentActivityInfo.Param.Where(p => p.OrgFieldName.Equals(v.column_name)).GetFirst<ParamInfo>();
                if (null == pi)
                {
                    continue;
                }

                SetParameterSelectedValue(ps, pi);
            }
            _CurrentParamterSelected = sp_Children.Children[0] as ParameterSelected;
        }

        void SetParameterSelectedValue(ParameterSelected ps, ParamInfo pi)
        {
            var _target_ci = IDesignFramework.GetControlInfoByControlName(pi.TargetCtrlName);
            if (null == _target_ci)
            {
                return;
            }
            var _target_mdi = IDesignFramework.GetMetaDataInfoByTableName(_target_ci.MetaData);
            if (null == _target_mdi)
            {
                return;
            }
            var _target_mdc = _target_mdi.Item.Where(p => p.column_name.Equals(pi.TargetFieldName)).GetFirst<MetaDataColumnInfo>();
            if (null == _target_mdc)
            {
                return;
            }
            ps.InitLoadValue = pi;
            ps.Text = _target_mdc.column_comment;
        }

        void ps_ModifyClick(object sender, RoutedEventArgs e)
        {
            //-->将值显示到text上去
            ParameterSelected ps = sender as ParameterSelected;
            if (null == ps)
            {
                return;
            }
            if (null != _CurrentParamterSelected)
            {
                _CurrentParamterSelected.BackgroundReset();
            }
            //-->获取所有其它控件
            _CurrentParamterSelected = ps;
            _CurrentParamterSelected.BackgroundSet();
        }

        void ps_DeleteClick(object sender, RoutedEventArgs e)
        {
            ParameterSelected ps = sender as ParameterSelected;
            if (null == ps)
            {
                return;
            }
            ps.Text = string.Empty;
            ps.TargetCtrlName = string.Empty;
            ps.TargetMetaDataColumn = null;
            ps.InitLoadValue = null;
        }

        List<MetaDataColumnInfo> _GetControlParameter(string MetaData, string ColumnName)
        {
            if (MetaData.IsNullOrEmpty() || ColumnName.IsNullOrEmpty())
            {
                return null;
            }
            //-->查找 metadata 里 row里的信息
            var row = IDesignFramework.GetMetaDataInfoByTableName(MetaData);
            if (null == row)
            {
                return null;
            }
            var items = ColumnName.Split('|');
            if (1 < items.Length)
            {
                List<MetaDataColumnInfo> columns = new List<MetaDataColumnInfo>();
                foreach (var item in items)
                {
                    foreach (var column in row.Item)
                    {
                        if (column.column_name.Equals(item))
                        {
                            columns.Add(column);
                            break;
                        }
                    }
                }
                return columns;
            }
            else
            {
                return row.Item.Where(p => p.column_name.Equals(ColumnName)).GetTList<MetaDataColumnInfo>();
            }
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
