using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using PlatformClient.Common;
using PlatformClient.Common.Interfaces;
using PlatformClient.Common.Lib;
using PlatformClient.Utility.ParseXml;
using PlatformClient.Model.Method;
using PlatformClient.Utility;
using System.Windows.Media;
using PlatformClient.Extend.Core;

namespace PlatformClient.Selectors.Core
{
    /// <summary>
    /// 判断当前控件是否是以Form开头的，如果是的话，说明是文件控件
    /// 得把它当前虚拟控件的来进行处理
    /// 
    /// 调用控件方法：
    ///     选择控件某个方法的时候，列表控件方法所需要的参数，并设置参数从什么地方取值
    /// </summary>
    public partial class CallFunctionSelectors : UserControl, ISelectors
    {
        /// <summary>
        /// 控件2集合
        /// </summary>
        List<ListBoxItemExtend> _Controls2Item = new List<ListBoxItemExtend>();
        /// <summary>
        /// 当前选中的参数控件
        /// </summary>
        ParameterSelected _CurrentParamterSelected;
        /// <summary>
        /// 当前选中的控件信息
        /// </summary>
        ControlInfo _CurrentControlInfo;
        /// <summary>
        /// 是否全屏
        /// </summary>
        bool _IsFullscreen = false;

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
        /// 函数选择器
        /// </summary>
        public CallFunctionSelectors()
        {
            InitializeComponent();
            this.lb_Controls1.SelectionChanged += lb_Controls1_SelectionChanged;
            this.lb_Controls1Function.SelectionChanged += lb_Controls1Function_SelectionChanged;
            this.lb_Controls2.SelectionChanged += lb_Controls2_SelectionChanged;
            this.lb_Controls2Parameter.SelectionChanged += lb_Controls2Parameter_SelectionChanged;
            //this.btnOK.Click += btnOK_Click;
            this.Loaded += CallFunctionSelectors_Loaded;

            this.lb_Controls2.Visibility = System.Windows.Visibility.Collapsed;
            this.lb_Controls2Parameter.Visibility = System.Windows.Visibility.Collapsed;
        }

        void lb_Controls2Parameter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox lb = sender as ListBox;
            if (null == lb || null == lb.SelectedItem)
            {
                return;
            }
            ListBoxItem item = lb.SelectedItem as ListBoxItem;
            var metaDataInfo = item.Tag as MetaDataColumnInfo;
            _CurrentParamterSelected.Text = metaDataInfo.column_comment;
            _CurrentParamterSelected.TargetMetaDataColumn = metaDataInfo;
            _CurrentParamterSelected.OrgCtrlInfo = _CurrentControlInfo;
            _CurrentParamterSelected.TargetCtrlName = string.Format("{0}", lb.Tag);
            this.lb_Controls2.Visibility = System.Windows.Visibility.Collapsed;
            this.lb_Controls2Parameter.Visibility = System.Windows.Visibility.Collapsed;
        }

        void lb_Controls2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox lb = sender as ListBox;
            if (null == lb || null == lb.SelectedItem)
            {
                return;
            }
            lb_Controls2Parameter.Items.Clear();
            ListBoxItemExtend item = lb.SelectedItem as ListBoxItemExtend;
            var paramItem = item.Tag as List<MetaDataColumnInfo>;
            if (null == paramItem)
            {
                return;
            }
            foreach (var v in paramItem)
            {
                ListBoxItemExtend data = new ListBoxItemExtend();
                data.Tag = v;
                data.Content = string.Format("{0}【{1}】 ", v.column_comment, v.column_name);
                lb_Controls2Parameter.Items.Add(data);
            }

            lb_Controls2Parameter.Tag = item.Content;
        }

        void CallFunctionSelectors_Loaded(object sender, RoutedEventArgs e)
        {
            //-->获取出所有控件的方法来，来至两个地方，第一个接口，第二是控件自己的方法
            var xml = IDesignFramework.GetCurrentXmlTemplate();
            InitLoadingControl(xml);
            //-----------------------------注册事件绑定---------------------------
            BindControlEvent(xml);
            //--------------------------------------------------------------------
            InitParameter(xml);
        }
        /// <summary>
        /// 初始化参数
        /// </summary>
        /// <param name="xml"></param>
        void InitParameter(XmlTemplate xml)
        {
            string _controlName = _GetControlName();

            var cei = xml.EventLinkItem.Where(p => p.ControlName == _controlName && p.EventName == EventName).GetFirst<EventLinkInfo>();
            if (null == cei)
            {
                return;
            }
            var ai = cei.Item.Where(p => p.ActivityName == this.ActivityName).GetFirst<ControlActivityInfo>();
            if (null == ai || null == ai.Param || 0 == ai.Param.Count)
            {
                return;
            }
            //-->1、初始化界面信息
            var _SelectedItem = lb_Controls1.Items.Where(p => (p as ListBoxItemExtend).Content.Equals(ai.ControlName)).GetFirst<ListBoxItemExtend>();
            lb_Controls1.SelectedItem = _SelectedItem;
            //-->2、初始化界面方法
            var _SelectedItemFunc = lb_Controls1Function.Items.Where(p => (p as ListBoxItemExtend).Content.Equals(ai.Description)).GetFirst<ListBoxItemExtend>();
            //-->3、初始化控件信息
            _SelectedItemFunc.ControlInfo = this.IDesignFramework.GetControlInfoByControlName(ai.ControlName);
            lb_Controls1Function.SelectedItem = _SelectedItemFunc;
            //-->给参数里面赋值。
            foreach (var pi in ai.Param)
            {
                var ps = this.sp_Children.Children.Where(p => p is ParameterSelected && (p as ParameterSelected).FieldName.Equals(pi.OrgFieldName)).GetFirst<ParameterSelected>();
                if (null == ps)
                {
                    continue;
                }

                var _target_ci = this.IDesignFramework.GetControlInfoByControlName(pi.TargetCtrlName);
                var _target_mdi = this.IDesignFramework.GetMetaDataInfoByTableName(_target_ci.MetaData);
                var _target_mdc = _target_mdi.Item.Where(p => p.column_name.Equals(pi.TargetFieldName)).GetFirst<MetaDataColumnInfo>();

                ps.Text = _target_mdc.column_comment;
                ps.InitLoadValue = pi;
                ps.TargetCtrlName = pi.TargetCtrlName;
                ps.TargetMetaDataColumn = _target_mdc;
                ps.OrgCtrlInfo = _SelectedItemFunc.ControlInfo;
            }//End foreach;
        }

        /// <summary>
        /// 获取控件名称
        /// </summary>
        /// <returns></returns>
        string _GetControlName()
        {
            /************************************************************************
             * 判断当前事件名称【EventName】它是来到于哪里，是Form还是自己所在控件里
             * 
             * 如果来到是Form里的话，此时的_ControlName就是用Wrapper.ParseControlName()得到
             * 
             * 否而的话，就不分析Control控件了
             * 
             ************************************************************************/
            var cc = ParseControlConfig.GetControlConfig(CurrentSelectedSetEventControl.GetType().Name);
            if (null == cc || null == cc.Functions || 0 == cc.Functions.Count)
            {
                return Wrapper.ParseControlName(EventControlName); ;
            }
            var fi = cc.Functions.Where(p => p.Name.Equals(EventName)).GetFirst<FunctionInfo>();
            if (null == fi)
            {
                return Wrapper.ParseControlName(EventControlName);
            }
            return EventControlName;
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
        /// 加载xml里的所有控件
        /// </summary>
        void InitLoadingControl(XmlTemplate xml)
        {
            lb_Controls1.Items.Clear();
            lb_Controls2.Items.Clear();
            _Controls2Item.Clear();
            foreach (var ci in xml.ControlItem)
            {
                var paramsCtrl = _GetControlParameter(xml, ci.MetaData, ci.ColumnName);
                try
                {
                    ListBoxItemExtend item = new ListBoxItemExtend();
                    item.Tag = paramsCtrl;
                    item.Content = ci.Name;
                    item.ControlInfo = ci;
                    lb_Controls1.Items.Add(item);

                    item = new ListBoxItemExtend();
                    item.Tag = paramsCtrl;
                    item.Content = ci.Name;
                    item.ControlInfo = ci;
                    _Controls2Item.Add(item);
                }
                catch { }
            }
            lb_Controls2.ItemsSource = _Controls2Item;
        }
        /// <summary>
        /// 获取控件参数
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="MetaData"></param>
        /// <param name="ColumnName"></param>
        /// <returns></returns>
        List<MetaDataColumnInfo> _GetControlParameter(XmlTemplate xml, string MetaData, string ColumnName)
        {
            if (MetaData.IsNullOrEmpty() || ColumnName.IsNullOrEmpty())
            {
                return null;
            }
            //-->查找 metadata 里 row里的信息
            var mdis = IDesignFramework.GetMetaDataInfoByTableName(MetaData);
            if (null == mdis)
            {
                return null;
            }
            var items = ColumnName.Split('|');
            if (1 < items.Length)
            {
                List<MetaDataColumnInfo> datas = new List<MetaDataColumnInfo>();
                foreach (var field in items)
                {
                    foreach (var mdci in mdis.Item)
                    {
                        if (mdci.column_name.Equals(field))
                        {
                            datas.Add(mdci);
                            break;
                        }
                    }
                }
                return datas;
            }
            else
            {
                return mdis.Item.Where(p => p.column_name.Equals(ColumnName)).GetTList<MetaDataColumnInfo>();
            }
        }

        void lb_Controls1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //-->当选中的时候，在lb_Control2中就得把当前的删除掉
            ListBox lb = sender as ListBox;
            if (null == lb || !(lb.SelectedItem is ListBoxItem))
            {
                return;
            }
            ListBoxItemExtend item = lb.SelectedItem as ListBoxItemExtend;
            string controlName = item.Content.ToString();
            //-->删除lb_controls2里的此控件
            var itemsource = _Controls2Item.Where(p => p != null).GetTList<ListBoxItemExtend>();
            ListBoxItemExtend removeItem = null;
            foreach (var v in itemsource)
            {
                ListBoxItemExtend item2 = v as ListBoxItemExtend;
                if (item2.Content.ToString().Equals(controlName))
                {
                    removeItem = v;
                    break;
                }
            }
            itemsource.Remove(removeItem);
            this.lb_Controls2.ItemsSource = itemsource;
            //-->当前选择控件
            _CurrentControlInfo = item.ControlInfo;
            //-->获取控件有多少函数
            this.lb_Controls1Function.Items.Clear();
            var ctrlFunction = ParseControlFunctionConfig.GetControlFunctionConfig(_CurrentControlInfo.Type);
            if (null != ctrlFunction && null != ctrlFunction.Functions && 0 < ctrlFunction.Functions.Count)
            {
                foreach (var function in ctrlFunction.Functions)
                {
                    ListBoxItemExtend _functItem = new ListBoxItemExtend();
                    _functItem.Tag = function;
                    _functItem.Content = function.Description;
                    this.lb_Controls1Function.Items.Add(_functItem);
                }
            }
        }

        void lb_Controls1Function_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            sp_Children.Children.Clear();
            ListBoxItemExtend controls = lb_Controls1.SelectedItem as ListBoxItemExtend;
            var parameterItem = controls.Tag as List<MetaDataColumnInfo>;
            if (null != parameterItem)
            {
                foreach (var v in parameterItem)
                {
                    ParameterSelected ps = new ParameterSelected();
                    ps.txtParamName.Text = string.Format("{0}【{1}】", v.column_name, v.column_comment);
                    ps.OrgMetaDataColumn = v;
                    ps.FieldName = v.column_name;
                    ps.DeleteClick += ps_DeleteClick;
                    ps.ModifyClick += ps_ModifyClick;
                    sp_Children.Children.Add(ps);
                }
            }

            var ci = controls.ControlInfo;
            /******************************************************************************
             * 
             * 控件的参数就是控件MetaData里的row信息,
             * 
             * ControlDataSourceTable 所处在的 MetaData信息也要取出来。
             * 
             * *****************************************************************************/
            if (!ci.ControlDataSourceTableName.IsNullOrEmpty())
            {
                var mdi = IDesignFramework.GetMetaDataInfoByTableName(ci.ControlDataSourceTableName);
                if (null != mdi && null != mdi.Item && 0 < mdi.Item.Count)
                {
                    Label lbl = new Label();
                    lbl.Height = 2;
                    lbl.Background = new SolidColorBrush(Colors.Black);
                    sp_Children.Children.Add(lbl);
                    foreach (var item in mdi.Item)
                    {
                        ParameterSelected ps = new ParameterSelected();
                        ps.txtParamName.Text = string.Format("{0}【{1}】", item.column_name, item.column_comment);
                        ps.OrgMetaDataColumn = item;
                        ps.FieldName = item.column_name;
                        ps.OrgIsDataSource = true;
                        ps.DeleteClick += ps_DeleteClick;
                        ps.ModifyClick += ps_ModifyClick;
                        sp_Children.Children.Add(ps);
                    }
                }
            }
            if (sp_Children.Children.Count > 0)
            {
                _CurrentParamterSelected = sp_Children.Children[0] as ParameterSelected;
            }
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
            this.lb_Controls2.Visibility = System.Windows.Visibility.Visible;
            this.lb_Controls2Parameter.Visibility = System.Windows.Visibility.Visible;
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

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            this.lb_Controls1.SelectionChanged -= lb_Controls1_SelectionChanged;
            this.lb_Controls1Function.SelectionChanged -= lb_Controls1Function_SelectionChanged;
            this.lb_Controls2.SelectionChanged -= lb_Controls2_SelectionChanged;
            this.lb_Controls2Parameter.SelectionChanged -= lb_Controls2Parameter_SelectionChanged;
            this.Loaded -= CallFunctionSelectors_Loaded;
            foreach (var v in this.sp_Children.Children)
            {
                var ps = v as ParameterSelected;
                if (null == ps)
                {
                    continue;
                }
                ps.DeleteClick -= ps_DeleteClick;
                ps.ModifyClick -= ps_ModifyClick;
                ps.Dispose();
            }
            this.sp_Children.Children.Clear();
            foreach (var v in this.lb_Controls1.Items)
            {
                var child = v as IDisposable;
                if (null == child)
                {
                    continue;
                }
                child.Dispose();
            }
            this.lb_Controls1.Items.Clear();
            foreach (var v in this.lb_Controls1Function.Items)
            {
                var child = v as IDisposable;
                if (null == child)
                {
                    continue;
                }
                child.Dispose();
            }
            this.lb_Controls1Function.Items.Clear();

            foreach (var v in this.lb_Controls2.Items)
            {
                var child = v as IDisposable;
                if (null == child)
                {
                    continue;
                }
                child.Dispose();
            }
            this.lb_Controls2.Items.Clear();

            foreach (var v in this.lb_Controls2Parameter.Items)
            {
                var child = v as IDisposable;
                if (null == child)
                {
                    continue;
                }
                child.Dispose();
            }
            this.lb_Controls2Parameter.Items.Clear();

            IDesignFramework = null;
            CurrentContextMenuInfo = null;
            ActivityName = string.Empty; IActivity = null;
            EventControlName = string.Empty;
            ActivityType = string.Empty;
            EventName = string.Empty;
            CurrentSelectedSetEventControl = null;
            _Controls2Item = null;
            _CurrentParamterSelected.Dispose();
            _CurrentParamterSelected = null;
            _CurrentControlInfo = null;

        }

        /// <summary>
        /// 检查是否保存
        /// </summary>
        public bool CheckSave()
        {
                return SaveOperator();
        }

        /// <summary>
        /// 保存
        /// </summary>
        bool SaveOperator()
        {
            if (null == lb_Controls1Function.SelectedItem)
            {
                Wrapper.ShowDialog("方法名称1没有选择");
                return false;
            }

            CtrlFuctionInfo cfi = (lb_Controls1Function.SelectedItem as ListBoxItem).Tag as CtrlFuctionInfo;
            string eventName = string.Format("{0}", cfi.Name);
            string controlName = string.Format("{0}", (lb_Controls1.SelectedItem as ListBoxItem).Content);

            var xml = IDesignFramework.GetCurrentXmlTemplate();
            if (null == xml.EventLinkItem)
            {
                xml.EventLinkItem = new List<EventLinkInfo>();
            }
            string _controlName = _GetControlName();
            //-->事件
            var eli = xml.EventLinkItem.Where(p => p.ControlName == _controlName && p.EventName == EventName).GetFirst<EventLinkInfo>();
            if (null == eli)
            {
                eli = new EventLinkInfo();
                xml.EventLinkItem.Add(eli);
            }
            eli.ControlName = _controlName;
            eli.EventName = this.EventName;
            eli.Item = new List<ControlActivityInfo>();
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
            foreach (var v in this.sp_Children.Children)
            {
                ParameterSelected select = v as ParameterSelected;
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