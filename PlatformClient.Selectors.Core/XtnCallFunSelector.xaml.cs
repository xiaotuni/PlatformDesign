using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using PlatformClient.Common.Interfaces;
using PlatformClient.Common.Lib;
using PlatformClient.Extend.Core;
using PlatformClient.Model.Method;
using PlatformClient.Utility;
using PlatformClient.Utility.ParseXml;

namespace PlatformClient.Selectors.Core
{
    /// <summary>
    /// 调用函数
    /// </summary>
    public partial class XtnCallFunSelector : UserControl, ISelectors
    {
        /// <summary>
        /// 是否全屏
        /// </summary>
        bool _IsFullscreen = false;
        private ParameterSelected _CurrentParamterSelected;
        private ControlInfo _CurrentControlInfo;
        private List<MetaDataColumnInfo> _CurrentControlParams;
        private XmlTemplate _xml;
        private Dictionary<ControlInfo, List<MetaDataColumnInfo>> _DictCtrlParam = new Dictionary<ControlInfo, List<MetaDataColumnInfo>>();

        /// <summary>
        /// 事件、方法属性
        /// </summary>
        public EventInfoAttribute AttributeInfo { get; set; }
        /// <summary>
        /// 设计时接口
        /// </summary>
        public IPageDesignFramework IDesignFramework { get; set; }
        /// <summary>
        /// 当前菜单信息
        /// </summary>
        public ContextMenuInfo CurrentContextMenuInfo { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ActivityName { get; set; }
        /// <summary>
        /// 当前选择Activity的IActivity接口
        /// </summary>
        public IActivity IActivity { get; set; }
        /// <summary>
        /// 控件的名称
        /// </summary>
        public string EventControlName { get; set; }
        /// <summary>
        /// 行为类型
        /// </summary>
        public string ActivityType { get; set; }
        /// <summary>
        /// 属性框里选中的事件名称
        /// </summary>
        public string EventName { get; set; }
        /// <summary>
        /// 在设计时选中的控件
        /// </summary>
        public FrameworkElement CurrentSelectedSetEventControl { get; set; }

        /// <summary>
        /// 是否全屏
        /// </summary>
        public bool IsFullscreen { get { return _IsFullscreen; } }
        /// <summary>
        /// 是否保存
        /// </summary>
        /// <returns></returns>
        public bool CheckSave()
        {
            return Save();
        }
        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            this.Loaded -= XtnCallFunSelector_Loaded;
            this.lbCtrls.SelectionChanged -= lbCtrls_SelectionChanged;
            this.lbSelectCtrlParams.SelectionChanged -= lbSelectCtrlParams_SelectionChanged;
            this.btnAutoBindingParam.Click -= btnAutoBindingParam_Click;
            this.btnReset.Click -= btnReset_Click;
            this.cbbCtrl.SelectionChanged -= cbbCtrl_SelectionChanged;
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        public XtnCallFunSelector()
        {
            InitializeComponent();
            this.cbbCtrl.SelectionChanged += cbbCtrl_SelectionChanged;
            this.Loaded += XtnCallFunSelector_Loaded;
            this.lbCtrls.SelectionChanged += lbCtrls_SelectionChanged;
            this.lbSelectCtrlParams.SelectionChanged += lbSelectCtrlParams_SelectionChanged;
            this.btnAutoBindingParam.Click += btnAutoBindingParam_Click;
            this.btnReset.Click += btnReset_Click;
        }

        void btnReset_Click(object sender, RoutedEventArgs e)
        {
            foreach (var child in this.spCtrlParams.Children)
            {
                var ps = child as ParameterSelected;
                if (null == ps)
                {
                    continue;
                }
                ps.Text = string.Empty;
                ps.TargetCtrlName = string.Empty;
                ps.TargetMetaDataColumn = null;
                ps.InitLoadValue = null;
            }
            this.lbSelectCtrlParams.SelectedIndex = -1;
        }

        void btnAutoBindingParam_Click(object sender, RoutedEventArgs e)
        {
            if (null == _CurrentControlParams || 0 == _CurrentControlParams.Count)
            {
                return;
            }
            foreach (var child in this.spCtrlParams.Children)
            {
                var ps = child as ParameterSelected;
                if (null == ps)
                {
                    continue;
                }
                //ps.FieldName
                var mdci = _CurrentControlParams.Where(p => p.column_name.IsNotEmpty() &&
                                                            p.column_name.Equals(ps.FieldName)
                                                            ).GetFirst<MetaDataColumnInfo>();
                SetParametSelected(ps, mdci);
            }
        }

        void lbSelectCtrlParams_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (null == _CurrentParamterSelected)
            {
                Wrapper.ShowDialog("请选择参数");
                return;
            }
            //-->将参数赋值到控件上去。
            ListBox lb = sender as ListBox;
            if (null == lb || null == lb.SelectedItem)
            {
                return;
            }
            ListBoxItem item = lb.SelectedItem as ListBoxItem;
            var metaDataInfo = item.Tag as MetaDataColumnInfo;

            SetParametSelected(_CurrentParamterSelected, metaDataInfo);
        }

        void SetParametSelected(ParameterSelected ps, MetaDataColumnInfo mdci)
        {
            if (null == ps || null == mdci)
            {
                return;
            }
            ps.Text = mdci.column_comment;
            ps.TargetMetaDataColumn = mdci;
            if (null == cbbCtrl.SelectedItem)
            {
                ps.OrgCtrlInfo = _CurrentControlInfo;
                ps.TargetCtrlName = this.EventControlName;
            }
            else
            {
                var ci = cbbCtrl.SelectedItem as ControlInfo;
                ps.OrgCtrlInfo = ci;
                ps.TargetCtrlName = ci.Name;
            }
            //-->这个还要进一步确定一下。
            var item = this.lbCtrls.SelectedItem as ListBoxItemExtend;
            ps.OrgIsDataSource = item.ControlInfo.ControlDataSourceTableName.IsNotEmpty() ? true : false;
        }

        void lbCtrls_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var otherCtrl = sender as ListBox;
            if (null == otherCtrl || null == otherCtrl.SelectedItem)
            {
                return;
            }
            var selectedItem = otherCtrl.SelectedItem as ListBoxItemExtend;
            //-->1、将控件的事件显示出来。
            LoadCtrlFunction(selectedItem);
            //-->2、将控件的参数显示出来。
            LoadCtrlParam(selectedItem);
            LoadComboBoxCtrl(selectedItem);
        }

        void LoadComboBoxCtrl(ListBoxItemExtend selectedItem)
        {
            string ctrlName = string.Format("{0}", selectedItem.Content);
            var item = _DictCtrlParam.Keys.Where(p => !p.Name.Equals(ctrlName)).GetTList<ControlInfo>();
            this.cbbCtrl.ItemsSource = item;
        }

        void LoadCtrlFunction(ListBoxItemExtend selectedItem)
        {
            var controlInfo = selectedItem.ControlInfo;
            if (null == controlInfo)
            {
                return;
            }
            this.lbCtrlsFuns.Items.Clear();
            var functionConfig = ParseControlFunctionConfig.GetControlFunctionConfig(controlInfo.Type);
            if (null != functionConfig && null != functionConfig.Functions && 0 < functionConfig.Functions.Count)
            {
                foreach (var function in functionConfig.Functions)
                {
                    ListBoxItemExtend _functItem = new ListBoxItemExtend();
                    _functItem.Tag = function;
                    _functItem.Content = function.Description;
                    this.lbCtrlsFuns.Items.Add(_functItem);
                }
            }
        }

        void LoadCtrlParam(ListBoxItemExtend selectedItem)
        {
            spCtrlParams.Children.Clear();
            var ci = selectedItem.ControlInfo;
            var paramItem = selectedItem.Tag as List<MetaDataColumnInfo>;
            LoadCtrlParam(ci, paramItem);
        }

        void LoadCtrlParam(ControlInfo ci, List<MetaDataColumnInfo> paramItem)
        {
            if (ci.MetaData.IsNotEmpty())
            {

                foreach (var v in paramItem)
                {
                    var ps = new ParameterSelected();
                    ps.txtParamName.Text = string.Format("{0}【{1}】", v.column_name, v.column_comment);
                    ps.OrgMetaDataColumn = v;
                    ps.FieldName = v.column_name;
                    ps.DeleteClick += ps_DeleteClick;
                    ps.ModifyClick += ps_ModifyClick;
                    spCtrlParams.Children.Add(ps);
                }
            }
            /******************************************************************************
             * 
             * 控件的参数就是控件MetaData里的row信息,
             * 
             * ControlDataSourceTable 所处在的 MetaData信息也要取出来。
             * 
             * *****************************************************************************/
            if (ci.ControlDataSourceTableName.IsNotEmpty())
            {
                var mdi = IDesignFramework.GetMetaDataInfoByTableName(ci.ControlDataSourceTableName);
                if (null != mdi && null != mdi.Item && 0 < mdi.Item.Count)
                {
                    Label lbl = new Label();
                    lbl.Height = 2;
                    lbl.Background = new SolidColorBrush(Colors.Black);
                    spCtrlParams.Children.Add(lbl);
                    foreach (var item in mdi.Item)
                    {
                        ParameterSelected ps = new ParameterSelected();
                        ps.txtParamName.Text = string.Format("{0}【{1}】", item.column_name, item.column_comment);
                        ps.OrgMetaDataColumn = item;
                        ps.FieldName = item.column_name;
                        ps.OrgIsDataSource = true;
                        ps.DeleteClick += ps_DeleteClick;
                        ps.ModifyClick += ps_ModifyClick;
                        spCtrlParams.Children.Add(ps);
                    }
                }
            }
        }

        void ps_ModifyClick(object sender, RoutedEventArgs e)
        {
            //-->将值显示到text上去
            var ps = sender as ParameterSelected;
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
            var ps = sender as ParameterSelected;
            if (null == ps)
            {
                return;
            }
            ps.Text = string.Empty;
            ps.TargetCtrlName = string.Empty;
            ps.TargetMetaDataColumn = null;
            ps.InitLoadValue = null;
        }

        void XtnCallFunSelector_Loaded(object sender, RoutedEventArgs e)
        {
            //-->显示标题
            lblSelectCtrl.Text = string.Format("【{0}】控件参数", EventControlName);
            //-->加载其它控件
            _xml = IDesignFramework.GetCurrentXmlTemplate();
            InitLoadingOtherCtrl(_xml);
            BindControlEvent(_xml);
            //-->
            ReappearParam(_xml);
        }
        /// <summary>
        /// 数据重现操作
        /// </summary>
        /// <param name="xml"></param>
        void ReappearParam(XmlTemplate xml)
        {
            string _controlName = CoreHelper.GetControlName(this.CurrentSelectedSetEventControl, this.EventName);
            //-->获取事件
            var cei = xml.EventLinkItem.Where(p => p.ControlName.Equals(_controlName) &&
                                                    p.EventName.Equals(EventName)
                                                    ).GetFirst<EventLinkInfo>();
            if (null == cei)
            {
                return;
            }
            //-->获取行为
            var ai = cei.Item.Where(p => p.ActivityName.Equals(this.ActivityName)).GetFirst<ControlActivityInfo>();
            if (null == ai || null == ai.Param || 0 == ai.Param.Count)
            {
                return;
            }
            //-->获取控件信息
            var ci = this.IDesignFramework.GetControlInfoByControlName(ai.ControlName);
            //-->1、查找选择控件信息
            var _SelectedItem = lbCtrls.Items.Where(p => (p as ListBoxItemExtend).Content.Equals(ai.ControlName)).GetFirst<ListBoxItemExtend>();
            _SelectedItem.ControlInfo = ci;
            lbCtrls.SelectedItem = _SelectedItem;
            //-->2、查找选择控件方法
            var _SelectedItemFunc = lbCtrlsFuns.Items.Where(p => (p as ListBoxItemExtend).Content.Equals(ai.Description)).GetFirst<ListBoxItemExtend>();
            _SelectedItemFunc.ControlInfo = ci;
            lbCtrlsFuns.SelectedItem = _SelectedItemFunc;
            //-->3、查找选择控件的参数
            foreach (var pi in ai.Param)
            {
                var ps = this.spCtrlParams.Children.Where(p => p is ParameterSelected && (p as ParameterSelected).FieldName.Equals(pi.OrgFieldName)).GetFirst<ParameterSelected>();
                if (null == ps)
                {
                    continue;
                }
                var _target_ci = this.IDesignFramework.GetControlInfoByControlName(pi.TargetCtrlName);
                if (null == _target_ci)
                {
                    continue;
                }
                var _target_mdi = this.IDesignFramework.GetMetaDataInfoByTableName(_target_ci.MetaData);
                if (null == _target_mdi)
                {
                    continue;
                }
                var _target_mdc = _target_mdi.Item.Where(p => p.column_name.ToLower().Equals(pi.TargetFieldName.ToLower())).GetFirst<MetaDataColumnInfo>();
                if (null == _target_mdc)
                {
                    continue;
                }
                ps.Text = _target_mdc.column_comment;
                ps.TargetCtrlName = pi.TargetCtrlName;
                ps.TargetMetaDataColumn = _target_mdc;
                ps.OrgCtrlInfo = _SelectedItemFunc.ControlInfo;
                ps.InitLoadValue = pi;
            }//End foreach;
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

        void InitLoadingOtherCtrl(XmlTemplate xml)
        {
            foreach (var ctrl in xml.ControlItem)
            {
                var paramsCtrl = CoreHelper.GetControlParameter(xml, ctrl.MetaData.IsNullOrEmpty() ? ctrl.ControlDataSourceTableName : ctrl.MetaData, ctrl.ColumnName);
                if (null == paramsCtrl)
                {
                    continue;
                }
                if (ctrl.Name.Equals(this.EventControlName))
                {
                    InitloadingSelectCtrl(ctrl, paramsCtrl);
                }
                else
                {
                    ListBoxItemExtend item = new ListBoxItemExtend();
                    item.Tag = paramsCtrl;
                    item.Content = ctrl.Name;
                    item.ControlInfo = ctrl;
                    this.lbCtrls.Items.Add(item);
                }
                _DictCtrlParam.Add(ctrl, paramsCtrl);
            }
        }

        void InitloadingSelectCtrl(ControlInfo ctrl, List<MetaDataColumnInfo> paramsCtrl)
        {
            _CurrentControlInfo = ctrl;
            _CurrentControlParams = paramsCtrl;
            lbSelectCtrlParams.Items.Clear();
            foreach (var v in paramsCtrl)
            {
                ListBoxItemExtend data = new ListBoxItemExtend();
                data.Tag = v;
                data.Content = string.Format("{0}【{1}】 ", v.column_comment, v.column_name);
                data.ControlInfo = ctrl;
                lbSelectCtrlParams.Items.Add(data);
            }
            lbSelectCtrlParams.Tag = paramsCtrl;
        }

        void cbbCtrl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cbb = sender as ComboBox;
            ControlInfo ci = cbb.SelectedItem as ControlInfo;
            var paramItem = _DictCtrlParam[ci];
            InitloadingSelectCtrl(ci, paramItem);
        }

        bool Save()
        {
            if (lbCtrlsFuns.SelectedItem == null)
            {
                Wrapper.ShowDialog("请选择控件方法。");
                return false;
            }

            CtrlFuctionInfo cfi = (lbCtrlsFuns.SelectedItem as ListBoxItem).Tag as CtrlFuctionInfo;
            string eventName = string.Format("{0}", cfi.Name);
            string controlName = string.Format("{0}", (lbCtrls.SelectedItem as ListBoxItem).Content);

            var xml = IDesignFramework.GetCurrentXmlTemplate();
            if (null == xml.EventLinkItem)
            {
                xml.EventLinkItem = new List<EventLinkInfo>();
            }
            string _controlName = CoreHelper.GetControlName(this.CurrentSelectedSetEventControl, this.EventName);
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
            var cai = eli.Item.Where(p => p.ControlName.Equals(controlName) && p.EventName.Equals(eventName)).GetFirst<ControlActivityInfo>();
            if (null == cai)
            {
                cai = new ControlActivityInfo();
                eli.Item.Add(cai);
            }
            //-->保存操作。
            cai.ActivityName = this.ActivityName;
            cai.ActivityType = this.ActivityType;
            cai.ControlName = controlName;
            cai.EventName = eventName;
            cai.Description = cfi.Description;
            cai.Param = new List<ParamInfo>();
            foreach (var ps in this.spCtrlParams.Children)
            {
                var select = ps as ParameterSelected;
                if (null == select || null == select.ParamForamt)
                {
                    continue;
                }
                cai.Param.Add(select.ParamForamt);
            }
            IDesignFramework.UpdateCurrentTemplate();

            return true;
        }
    }
}
