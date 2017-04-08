using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml.Linq;
using PlatformClient.Common;
using PlatformClient.Common.Interfaces;
using PlatformClient.EventDesign.Core.Activitys;
using PlatformClient.EventDesign.Core.Lib;
using PlatformClient.Extend.Core;
using PlatformClient.Model.Method;
using PlatformClient.Utility;
using PlatformClient.Utility.ParseXml;

namespace PlatformClient.EventDesign.Core
{
    /// <summary>
    /// 事件设计器
    /// </summary>
    public partial class EventDesignMain : BaseControl
    {
        ContextMenu _Cm = new ContextMenu();
        TextBox _txtActivityContent = new TextBox();
        Dictionary<String, FrameworkElement> DictControls = new Dictionary<string, FrameworkElement>();
        /// <summary>
        /// 
        /// </summary>
        public EventInfoAttribute AttributeInfo { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public EventDesignMain()
        {
            InitializeComponent();

            this._txtActivityContent.LostFocus += _txtActivityContent_LostFocus;
            this.xtnEvents.SelectionChanged += xtnEvents_SelectionChanged;
            this.xtnEvents.OnDelete += xtnEvents_OnDelete;
            this.xtnEvents.OnModify += xtnEvents_OnModify;
            this.Loaded += EventDesignMain_Loaded;
        }

        void xtnEvents_OnModify(object sender, EventArgs e)
        {
            var _event = sender as XtnEventTree;
            if (null == _event)
            {
                return;
            }
            var edc = _event.SelectedItem;
            if (null == edc)
            {
                return;
            }
            ImportWorkFlow(edc.EventContent);
        }

        void xtnEvents_OnDelete(object sender, EventArgs e)
        {
            this.ClearChilds();
        }

        void EventDesignMain_Loaded(object sender, RoutedEventArgs e)
        {
            this.xtnEvents.IDesignFramework = this.IDesignFramework;
            LoadingControlEventDesinger();
        }

        void xtnEvents_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var _event = sender as XtnEventTree;
            if (null == _event)
            {
                return;
            }
            var edc = _event.SelectedItem;
            if (null == edc)
            {
                return;
            }
            ImportWorkFlow(edc.EventContent);
        }

        void ClearChilds()
        {
            try
            {
                foreach (var v in this.print.Children)
                {
                    if (v is IBaseActivity)
                    {
                        DisposeActivity(v as XtnBaseActivity);
                    }
                    else if (v is IArrowLine)
                    {
                        DisposeArrowLine(v as IArrowLine);
                    }
                }
                DictControls.Clear();
                this.print.Children.Clear();
            }
            catch { }
        }

        void DisposeArrowLine(IArrowLine _newLine)
        {
            if (null == _newLine)
            {
                return;
            }
            _newLine.Dispose();
        }
        void DisposeActivity(XtnBaseActivity act)
        {
            if (null == act)
            {
                return;
            }
            act.MouseRightButtonDown -= act_MouseRightButtonDown;
            act.Dispose();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="root"></param>
        void ImportWorkFlow(XElement root)
        {
            if (null == root)
            {
                return;
            }
            ClearChilds();
            _Cm.Visibility = System.Windows.Visibility.Collapsed;
            _txtActivityContent.Visibility = System.Windows.Visibility.Collapsed;
            this.print.Children.Add(_Cm);
            this.print.Children.Add(_txtActivityContent);
            var controls = root.Element("Controls");
            var Width = controls.GetAttributeValue("Width").ConvertTo<double>();
            var Height = controls.GetAttributeValue("Height").ConvertTo<double>();
            if (0d < Height)
            {
                this.print.Height = Height;
            }
            if (0d < Width)
            {
                this.print.Width = Width;
            }
            var activitys = controls.Elements("Activity");
            var arrowlines = controls.Elements("ArrowLine");
            foreach (var child in arrowlines)
            {
                ArrowLineInfo ali = child.ToModel<ArrowLineInfo>();
                if (null == ali)
                {
                    continue;
                }
                ArrowLine al = new ArrowLine(ali.StartPoint, ali.EndPoint);
                al.Name = ali.Name;
                al.LineGuid = ali.Guid;
                CreateArrowLine(al);
            }
            foreach (var act in activitys)
            {
                ActivityInfo ai = act.ToModel<ActivityInfo>();
                if (null == ai)
                {
                    continue;
                }

                var config = ParseActivityConfig.GetActivityConfig(ai.Type);
                if (null == config)
                {
                    continue;
                }
                var ba = Wrapper.CreateControl<XtnBaseActivity>(config.AssemblyQualifiedName);
                ba.Name = ai.Name;
                ba.Height = ai.Height;
                ba.Width = ai.Width;
                ba.ActivityGUID = ai.Guid;
                ba.LabelContent = ai.Content.IsNullOrEmpty() ? config.Description : ai.Content;
                ba.CurrentLeftButtonDownPoint = ai.CurrentLeftButtonDownPoint;
                ba.CurrentEnterPoint = ai.CurrentEnterPoint;
                CreateActivity(ba);
                Canvas.SetTop(ba, ai.Top);
                Canvas.SetLeft(ba, ai.Left);
            }

            //-->规则
            var rules = root.Element("Relationship");
            var rulesControl = rules.Elements("Control");
            foreach (var rule in rulesControl)
            {
                string _r_name = rule.GetAttributeValue("Name");
                XtnBaseActivity iact = GetControlByName<XtnBaseActivity>(_r_name);
                if (null == iact)
                {
                    continue;
                }
                var caps = rule.GetAttributeValue("Cap");
                if (!caps.IsNullOrEmpty())
                {
                    var capArray = caps.Split('|');
                    foreach (var cap in capArray)
                    {
                        ArrowLine _al = GetControlByName<ArrowLine>(cap);
                        if (null == _al)
                        {
                            continue;
                        }
                        _al.ArrowFootControl = iact;
                        iact.DictArrowCapPoint.Add(_al.Name, _al);
                    }
                }
                var foots = rule.GetAttributeValue("Foot");
                if (!foots.IsNullOrEmpty())
                {
                    var footArray = foots.Split('|');
                    foreach (var foot in footArray)
                    {
                        ArrowLine _al = GetControlByName<ArrowLine>(foot);
                        if (null == _al)
                        {
                            continue;
                        }
                        _al.ArrowCapControl = iact;
                        iact.DictArrowFootPoint.Add(_al.CtrName, _al);
                    }
                }
            }
        }

        /// <summary>
        /// 获取控件名称
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="controlName"></param>
        /// <returns></returns>
        T GetControlByName<T>(string controlName) where T : FrameworkElement
        {
            if (!DictControls.ContainsKey(controlName))
            {
                return default(T);
            }
            return (T)DictControls[controlName];
        }
        void CreateArrowLine(ArrowLine _newLine)
        {
            if (null == _newLine)
            {
                return;
            }
            print.Children.Add(_newLine);
            DictControls.Add(_newLine.Name, _newLine);
        }
        void CreateActivity(XtnBaseActivity act)
        {
            act.MouseRightButtonDown += act_MouseRightButtonDown;
            Canvas.SetZIndex(act, 100);
            Canvas.SetLeft(act, 30);
            Canvas.SetTop(act, 30);

            DictControls.Add(act.Name, act);
            print.Children.Add(act);
        }
        void act_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            IActivity iact = sender as IActivity;
            if (null == iact)
            {
                return;
            }
            _Cm.Visibility = System.Windows.Visibility.Visible;
            _Cm.Items.Clear();
            var cm = iact.ContextMenu();
            foreach (var v in cm)
            {
                MenuItem mi = new MenuItem();
                mi.Header = v.Header;
                mi.Tag = v;
                mi.Click += mi_Click;
                _Cm.Items.Add(mi);
            }
            var p = e.GetPosition(print);
            Canvas.SetLeft(_Cm, p.X);
            Canvas.SetTop(_Cm, p.Y);
            Canvas.SetZIndex(_Cm, 1000);
        }

        void mi_Click(object sender, RoutedEventArgs e)
        {
            _Cm.Visibility = System.Windows.Visibility.Collapsed;
            MenuItem mi = sender as MenuItem;
            if (null == mi)
            {
                return;
            }
            ContextMenuInfo cmi = mi.Tag as ContextMenuInfo;
            XtnBaseActivity bac = cmi.Source as XtnBaseActivity;
            switch (cmi.Type)
            {
                case ContextMenuType.Delete:
                    RemoveActivity(bac);
                    break;
                case ContextMenuType.ModifyContent:
                    _ModifyActivityContent(cmi);
                    break;
                case ContextMenuType.ParameterSettings:
                    _ParameterSettings(cmi, bac);
                    break;
            }
        }

        private void RemoveActivity(XtnBaseActivity bac)
        {
            Wrapper.ShowDialog("不能删除。");
        }

        /// <summary>
        /// 进行参数设置
        /// </summary>
        /// <param name="cmi"></param>
        /// <param name="bac"></param>
        void _ParameterSettings(ContextMenuInfo cmi, XtnBaseActivity bac)
        {
            if (cmi.AssemblyQualifiedName.IsNullOrEmpty())
            {
                return;
            }
            FrameworkElement ui = Wrapper.CreateControl<FrameworkElement>(cmi.AssemblyQualifiedName);
            ISelectors select = ui as ISelectors;
            if (null == select)
            {
                return;
            }
            select.IDesignFramework = this.IDesignFramework;
            select.CurrentContextMenuInfo = cmi;
            select.IActivity = bac as IActivity;
            select.ActivityName = bac.Name;
            select.EventControlName = this.EventControlName;
            select.EventName = this.EventName;
            select.ActivityType = bac.GetType().Name;
            select.AttributeInfo = this.AttributeInfo;
            select.CurrentSelectedSetEventControl = this.CurrentSelectedControl;

            //-->弹出窗体来
            XtnChildWindow pcc = new XtnChildWindow(this.IDesignFramework, ui);
            pcc.Closed += pcc_Closed;
            pcc.SetTitle(string.Format("正在对【{0}】控件【{1}】事件参数进行设置...", this.CurrentSelectedControl.Name, this.EventName));
            pcc.Show();
        }

        void pcc_Closed(object sender, EventArgs e)
        {
            XtnChildWindow pcc = sender as XtnChildWindow;
            if (null != pcc)
            {
                pcc.Closed -= pcc_Closed;
                pcc.Dispose();
            }
        }

        void _ModifyActivityContent(ContextMenuInfo cmi)
        {
            //Wrapper.ShowDialog("不能移动。");
            _txtActivityContent.Tag = cmi;
            _txtActivityContent.Visibility = System.Windows.Visibility.Visible;
            XtnBaseActivity fe = cmi.Source as XtnBaseActivity;
            _txtActivityContent.Text = fe.LabelContent;
            //_txtActivityContent.Height = 30;
            //_txtActivityContent.Width = 120;
            Canvas.SetLeft(_txtActivityContent, Canvas.GetLeft(fe) + fe.Width);
            Canvas.SetTop(_txtActivityContent, Canvas.GetTop(fe));
            Canvas.SetZIndex(_txtActivityContent, 1000);
        }

        void _txtActivityContent_LostFocus(object sender, RoutedEventArgs e)
        {
            _txtActivityContent.Visibility = System.Windows.Visibility.Collapsed;
            ContextMenuInfo cmi = _txtActivityContent.Tag as ContextMenuInfo;
            IActivity iact = cmi.Source as IActivity;
            iact.LabelContent = _txtActivityContent.Text;
        }
    }
}
