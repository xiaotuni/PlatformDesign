using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
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
    /// 创建事件工作流
    /// </summary>
    public partial class XtnCreateEventWorkFlow : BaseControl
    {
        bool isMove;
        ArrowLine _newLine = null;
        UIElement _CurrentUIElement;
        bool _UI_IsMoveing;
        double _UI_X;
        double _UI_Y;
        Color _B_C = Color.FromArgb(0x9B, 0xEF, 0xEE, 0xE5);
        bool _IsActivity;
        private int Index;
        Dictionary<String, FrameworkElement> DictControls = new Dictionary<string, FrameworkElement>();
        private ContextMenu _Cm = new ContextMenu();
        private TextBox _txtActivityContent = new TextBox();
        List<String> _OperatorHistory = new List<string>();
        private string _OldWorkFlowStr;
        /// <summary>
        /// 
        /// </summary>
        public bool IsActivity { get { return _IsActivity; } set { _IsActivity = value; } }
        /// <summary>
        /// 
        /// </summary>
        public EventInfoAttribute AttributeInfo { get; set; }

        /// <summary>
        /// 创建事件工作流构造函数
        /// </summary>
        public XtnCreateEventWorkFlow()
        {
            InitializeComponent();
            //-->订阅事件
            SubscribeEvent();
            //-->
            _Cm.Visibility = System.Windows.Visibility.Collapsed;
            _txtActivityContent.Visibility = System.Windows.Visibility.Collapsed;
            _txtActivityContent.AcceptsReturn = true;
            _txtActivityContent.TextWrapping = TextWrapping.Wrap;
            _txtActivityContent.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            _txtActivityContent.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            //-->
            this.print.Children.Add(_Cm);
            this.print.Children.Add(_txtActivityContent);
            //-->
            CreateActivityControl();
            this.MouseRightButtonDown += EventDesignMain_MouseRightButtonDown;
        }

        void EventDesignMain_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        /// <summary>
        /// 创建有哪些Activity控件，从配置文件里读取出来
        /// </summary>
        void CreateActivityControl()
        {
            //-->获取所有Activity
            var acItem = ParseActivityConfig.ActivityItem;
            if (null == acItem)
            {
                return;
            }
            sp_Activitys.Children.Clear();
            foreach (var v in acItem)
            {
                Button btn = new Button();
                btn.Name = string.Format("btn_{0}", v.Name);
                btn.Tag = v;
                btn.Content = v.Description;
                btn.Click += btn_Click;
                sp_Activitys.Children.Add(btn);
            }
        }

        /// <summary>
        /// 订阅事件
        /// </summary>
        void SubscribeEvent()
        {
            this.print.MouseLeftButtonDown += print_MouseLeftButtonDown;
            this.print.MouseLeftButtonUp += print_MouseLeftButtonUp;
            this.print.MouseMove += print_MouseMove;
            this.Loaded += EventDesignMain_Loaded;

            this.btnClear.Click += btnClear_Click;
            this.btnSave.Click += btnSave_Click;
            this.btnRevoke.Click += btnRevoke_Click;
            this.btnExportXml.Click += btnExportXml_Click;
            this.btnInportXml.Click += btnImportXml_Click;

            this.s_ChangeWidth.ValueChanged += s_ChangeWidth_ValueChanged;
            this.s_ChangeHeight.ValueChanged += s_ChangeHeight_ValueChanged;
            this._Cm.MouseLeave += _Cm_MouseLeave;
            this._txtActivityContent.LostFocus += _txtActivityContent_LostFocus;
        }

        void _Cm_MouseLeave(object sender, MouseEventArgs e)
        {
            _Cm.Visibility = System.Windows.Visibility.Collapsed;
        }

        void s_ChangeHeight_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            this.print.Height = e.NewValue < this.print.ActualHeight ? this.print.ActualHeight : e.NewValue;
        }

        void s_ChangeWidth_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            this.print.Width = e.NewValue < this.print.ActualWidth ? this.print.ActualWidth : e.NewValue;
        }

        void btnClear_Click(object sender, RoutedEventArgs e)
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
                DisposeActivity(_CurrentUIElement as XtnBaseActivity);
                DisposeArrowLine(_newLine);
                _IsActivity = false;
                _UI_IsMoveing = false;
                _UI_X = 0;
                _UI_Y = 0;
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
            _newLine.DeleteLine -= _newLine_DeleteLine;
            _newLine.ResetDirection -= _newLine_ResetDirection;
            _newLine.Dispose();
        }

        void DisposeActivity(XtnBaseActivity act)
        {
            if (null == act)
            {
                return;
            }
            act.DotMouseMove -= act_DotMouseMove;
            act.DotMouseLeftButtonUp -= act_DotMouseLeftButtonUp;
            act.DotMouseLeftButtonDown -= act_DotMouseLeftButtonDown;
            act.DotMouseEnter -= act_DotMouseEnter;
            act.DotMouseLeave -= act_DotMouseLeave;
            act.MouseLeftButtonDown -= act_MouseLeftButtonDown;
            act.MouseLeftButtonUp -= act_MouseLeftButtonUp;
            act.MouseMove -= act_MouseMove;
            act.MouseRightButtonDown -= act_MouseRightButtonDown;
            act.Dispose();
        }

        void btnImportXml_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //-->先清除然后再进行导入
                ImportWorkFlow(XElement.Parse(this.txtXml.Text.Trim()));
            }
            catch { }
        }
        /// <summary>
        /// 导入数据
        /// </summary>
        /// <param name="root"></param>
        void ImportWorkFlow(XElement root)
        {
            if (null == root)
            {
                return;
            }
            _OldWorkFlowStr = root.ToString();
            btnClear_Click(null, null);
            this.print.Children.Add(_Cm);
            this.print.Children.Add(_txtActivityContent);
            var controls = root.Element("Controls");
            Index = controls.GetAttributeValue("Index").ConvertTo<Int32>();
            var activitys = controls.Elements("Activity");
            var arrowlines = controls.Elements("ArrowLine");
            foreach (var v in arrowlines)
            {
                ArrowLineInfo ali = v.ToModel<ArrowLineInfo>();
                if (null == ali)
                {
                    continue;
                }
                ArrowLine al = new ArrowLine(ali.StartPoint, ali.EndPoint);
                al.Name = ali.Name;
                al.LineGuid = ali.Guid;
                CreateArrowLine(al);
            }
            foreach (var v in activitys)
            {
                ActivityInfo ai = v.ToModel<ActivityInfo>();
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

        void btnExportXml_Click(object sender, RoutedEventArgs e)
        {
            txtXml.Text = GetWorkFlowXml();
        }

        string GetWorkFlowXml()
        {
            List<String> points = new List<string>();
            List<String> activitys = new List<string>();
            foreach (var v in print.Children)
            {
                IImportExport iie = v as IImportExport;
                if (null == iie)
                {
                    continue;
                }
                points.Add(iie.ExportLocation());
                if (v is IActivity)
                {
                    activitys.Add((v as IActivity).ExportControlRelationship());
                }
            }
            string _xmlValue = string.Format("<Root>\r\n{0}\r\n{1}\r\n</Root>",
               string.Format("<Controls Index=\"{0}\" Height=\"{1}\" Width=\"{2}\">{3}</Controls>",
               Index,
               this.print.Height,
               this.print.Width,
               string.Join("\r\n", points)),
               string.Format("<Relationship>{0}</Relationship>", string.Join("\r\n", activitys)));
            return XElement.Parse(_xmlValue).ToString();
        }

        void btnRevoke_Click(object sender, RoutedEventArgs e)
        {
            if (0 < _OperatorHistory.Count)
            {
                string aa = _OperatorHistory[0];
                ImportWorkFlow(XElement.Parse(aa));

                _OperatorHistory.RemoveAt(0);
            }
        }

        void btnSave_Click(object sender, RoutedEventArgs e)
        {
            this.SaveXmlToDB();
        }

        void EventDesignMain_Loaded(object sender, RoutedEventArgs e)
        {
            //LoadingControlEventDesinger();
        }

        void btn_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (null == btn)
            {
                return;
            }
            ActivityConfig ac = btn.Tag as ActivityConfig;
            XtnBaseActivity act = Wrapper.CreateControl<XtnBaseActivity>(ac.AssemblyQualifiedName);
            if (null == act)
            {
                return;
            }
            act.Name = string.Format("Activity_{0}_{1}", ac.Name, ++Index);
            act.LabelContent = ac.Description;
            CreateActivity(act);
        }

        void CreateActivity(XtnBaseActivity act)
        {
            act.DotMouseMove += act_DotMouseMove;
            act.DotMouseLeftButtonUp += act_DotMouseLeftButtonUp;
            act.DotMouseLeftButtonDown += act_DotMouseLeftButtonDown;
            act.DotMouseEnter += act_DotMouseEnter;
            act.DotMouseLeave += act_DotMouseLeave;
            act.MouseRightButtonDown += act_MouseRightButtonDown;
            act.MouseLeftButtonDown += act_MouseLeftButtonDown;
            act.MouseLeftButtonUp += act_MouseLeftButtonUp;
            act.MouseMove += act_MouseMove;

            Canvas.SetZIndex(act, 100);
            Canvas.SetLeft(act, 30);
            Canvas.SetTop(act, 30);

            DictControls.Add(act.Name, act);
            print.Children.Add(act);
            _CurrentUIElement = act;
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
            _txtActivityContent.Tag = cmi;
            _txtActivityContent.Visibility = System.Windows.Visibility.Visible;
            XtnBaseActivity fe = cmi.Source as XtnBaseActivity;
            _txtActivityContent.Text = fe.LabelContent;
            Canvas.SetLeft(_txtActivityContent, Canvas.GetLeft(fe) + fe.Width);
            Canvas.SetTop(_txtActivityContent, Canvas.GetTop(fe));
        }

        void _txtActivityContent_LostFocus(object sender, RoutedEventArgs e)
        {
            _txtActivityContent.Visibility = System.Windows.Visibility.Collapsed;
            ContextMenuInfo cmi = _txtActivityContent.Tag as ContextMenuInfo;
            IActivity iact = cmi.Source as IActivity;
            iact.LabelContent = _txtActivityContent.Text;
        }

        void print_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            isMove = true;
            Point p = e.GetPosition(print);
            if (_newLine != null && p == _newLine.StartPoint)
            {
                return;
            }

            FrameworkElement element = sender as FrameworkElement;
            element.CaptureMouse();
            double top = p.Y;
            double left = p.X;

            _newLine = new ArrowLine(p, p);
            _newLine.Name = string.Format("ArrowLine_{0}", ++Index);
            _newLine.LineGuid = Wrapper.GuidValue;
            CreateArrowLine(_newLine);
        }

        void CreateArrowLine(ArrowLine _newLine)
        {
            if (null == _newLine)
            {
                return;
            }
            _newLine.DeleteLine += new RoutedEventHandler(_newLine_DeleteLine);
            _newLine.ResetDirection += new RoutedEventHandler(_newLine_ResetDirection);
            _newLine.Background = new SolidColorBrush(Colors.Blue);
            _newLine.MouseEnter += new MouseEventHandler(_newLine_MouseEnter);
            _newLine.MouseLeave += new MouseEventHandler(_newLine_MouseLeave);
            print.Children.Add(_newLine);
            DictControls.Add(_newLine.Name, _newLine);
        }

        void _newLine_MouseLeave(object sender, MouseEventArgs e)
        {
            ArrowLine al = sender as ArrowLine;
            if (null == al)
            {
                return;
            }
            al.Connector.Stroke = new SolidColorBrush(Colors.Black);
            al.Connector.StrokeThickness = 1;
        }

        void _newLine_MouseEnter(object sender, MouseEventArgs e)
        {
            ArrowLine al = sender as ArrowLine;
            if (null == al)
            {
                return;
            }
            al.Connector.Stroke = new SolidColorBrush(Colors.Black);
            al.Connector.StrokeThickness = 5;
        }

        void print_MouseMove(object sender, MouseEventArgs e)
        {
            if (isMove)
            {
                Point p = e.GetPosition(print);
                if (p == _newLine.StartPoint)
                {
                    RemoveArrowLine(_newLine);
                }
                else
                {
                    _newLine.EndPoint = p;
                }
            }
        }

        void print_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            isMove = false;
            IsActivity = false;
            //e.Handled = false;
            FrameworkElement element = sender as FrameworkElement;
            element.ReleaseMouseCapture();
            if (_newLine == null)
            {
                return;
            }
            Point p = e.GetPosition(print);
            if (p == _newLine.StartPoint)
            {
                RemoveArrowLine(_newLine);
            }
            else
            {
                //-->如果当前的箭头，所连接的控件，与移动进去的控件不是同一控件的话。
                foreach (var v in DictControls)
                {
                    IActivity iact = v.Value as IActivity;
                    if (null == iact)
                    {
                        continue;
                    }
                    //-->获取当前控件的
                    double top = Canvas.GetTop(v.Value);
                    double left = Canvas.GetLeft(v.Value);
                    //-->判断当前的箭头进入Activity控件区域
                    if ((p.X >= left && p.X <= (left + v.Value.Width)) && (p.Y >= top && p.Y <= (top + v.Value.Height)))
                    {
                        //-->判断当前的箭头是否可以连接到Activity控件上去。
                        if (!iact.IsConnection())
                        {
                            MessageBox.Show("不能连接");
                            RemoveArrowLine(_newLine);
                            continue;
                        }
                        //-->当前线的尾部控件与当前进入的控件相同，不进行操作
                        if (null != _newLine.ArrowFootControl && _newLine.ArrowFootControl.Equals(v.Value))
                        {
                            continue;
                        }
                        IActivity arrowFoot = _newLine.ArrowFootControl as IActivity;
                        if (arrowFoot != null && arrowFoot.CheckedArrowIsExists(iact))
                        {
                            MessageBox.Show("已经存在相同的线了");
                            RemoveArrowLine(_newLine);
                            continue;
                        }
                        string _point = string.Format("X_{0}_Y_{1}", _newLine.StartPoint.X, _newLine.StartPoint.Y);
                        if (!iact.DictArrowFootPoint.ContainsKey(_newLine.Name))
                        {
                            iact.DictArrowCapPoint.Add(_newLine.Name, _newLine);
                            _newLine.ArrowCapControl = v.Value;
                        }
                        break;
                    }
                }
            }
            _newLine = null;
        }

        /// <summary>
        /// 删除线
        /// </summary>
        /// <param name="line"></param>
        void RemoveArrowLine(IArrowLine line)
        {
            if (null == line)
            {
                return;
            }
            DisposeArrowLine(line);
            if (line.ArrowCapControl is IActivity)
            {
                (line.ArrowCapControl as IActivity).RemoveLine(line);
            }
            if (line.ArrowFootControl is IActivity)
            {
                (line.ArrowFootControl as IActivity).RemoveLine(line);
            }
            var ctl = this.print.FindName(line.CtrName) as UIElement;
            this.print.Children.Remove(ctl);
        }

        /// <summary>
        /// 删除行为控件
        /// </summary>
        /// <param name="bac"></param>
        void RemoveActivity(XtnBaseActivity bac)
        {
            if (null == bac)
            {
                return;
            }
            List<IArrowLine> items = new List<IArrowLine>();
            foreach (var v in bac.DictArrowCapPoint)
            {
                items.Add(v.Value);
            }
            foreach (var v in bac.DictArrowFootPoint)
            {
                items.Add(v.Value);
            }
            foreach (var v in items)
            {
                RemoveArrowLine(v);
            }
            DisposeActivity(bac);
            DictControls.Remove(bac.Name);
            this.print.Children.Remove(bac);

            //-->删除Activity时，同时删除页面设计时事件链里的连接
            var elItem = this.IDesignFramework.GetCurrentXmlTemplate().EventLinkItem;
            var _eli = elItem.Where(p => p.ControlName.Equals(this.EventControlName) && p.EventName.Equals(this.EventName)).GetFirst<EventLinkInfo>();
            if (null != _eli)
            {
                var _cai = _eli.Item.Where(p => p.ActivityName.Equals(bac.Name)).GetFirst<ControlActivityInfo>();
                if (null != _cai)
                {
                    _eli.Item.Remove(_cai);
                    this.IDesignFramework.UpdateCurrentTemplate();
                }
            }
        }
        void _newLine_ResetDirection(object sender, RoutedEventArgs e)
        {
            //-->获取鼠标的坐标
            //isMove = true;
        }

        void _newLine_DeleteLine(object sender, RoutedEventArgs e)
        {
            if (sender is ArrowLine)
            {
                ArrowLine line = sender as ArrowLine;
                RemoveArrowLine(line);
            }
        }

        void act_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            UIElement uie = sender as UIElement;
            if (null == uie)
            {
                return;
            }
            uie.CaptureMouse();
            var p = e.GetPosition(print);
            double _UI_Left = Canvas.GetLeft(uie);
            double _UI_Top = Canvas.GetTop(uie);
            IActivity act = sender as IActivity;
            if (IsActivity && null != act && act.IsStart())
            {
                print_MouseLeftButtonDown(print, e);
            }
            else
            {
                _UI_X = p.X - _UI_Left;
                _UI_Y = p.Y - _UI_Top;
                _UI_IsMoveing = true;
                e.Handled = true;
            }
            if (null != act && act.IsStart())
            {
                p = new Point(_UI_Left, _UI_Top);
                (sender as IActivity).CurrentLeftButtonDownPoint = p;
            }
        }

        void act_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_UI_IsMoveing)
            {
                return;
            }
            var p = e.GetPosition(print);
            var p1 = new Point();
            if (IsActivity)
            {
                print_MouseMove(print, e);
            }
            else
            {
                UIElement uie = sender as UIElement;
                if (null == uie || _UI_IsMoveing == false)
                {
                    return;
                }
                double newLeft = p.X;
                double newTop = p.Y;
                p1.X = newLeft - _UI_X;
                p1.Y = newTop - _UI_Y;
                Canvas.SetLeft(uie, newLeft - _UI_X);
                Canvas.SetTop(uie, newTop - _UI_Y);
            }
            //-->当箭头移动到控件上时
            if (sender is IActivity)
            {
                (sender as IActivity).UpdateArrowCapPoint(p1);
                (sender as IActivity).UpdateArrowFootPoint(p1);
            }
        }

        void act_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = false;
            _UI_IsMoveing = false;
            IsActivity = false;
            var p = e.GetPosition(print);
            if (IsActivity)
            {
                print_MouseLeftButtonUp(print, e);
            }
            else
            {
                UIElement uie = sender as UIElement;
                if (null == uie)
                {
                    return;
                }
                double newLeft = p.X;
                double newTop = p.Y;
                Canvas.SetLeft(uie, newLeft - _UI_X);
                Canvas.SetTop(uie, newTop - _UI_Y);
                uie.ReleaseMouseCapture();
            }
            if (sender is IActivity)
            {
                p = new Point(p.X - _UI_X, p.Y - _UI_Y);
                (sender as IActivity).UpdateArrowCapPoint(p);
                (sender as IActivity).UpdateArrowFootPoint(p);
            }
            _newLine = null;
            string a = GetWorkFlowXml();
            if (_OperatorHistory.Count > 50)
            {
                _OperatorHistory.RemoveAt(_OperatorHistory.Count - 1);
            }
            _OperatorHistory.Insert(0, a);
        }

        void act_DotMouseLeave(object sender, MouseEventArgs e)
        {
            IsActivity = false;
        }

        void act_DotMouseEnter(object sender, MouseEventArgs e)
        {
            _IsActivity = true;
        }

        void act_DotMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point p = e.GetPosition(print);
            if (_newLine != null && p == _newLine.StartPoint)
            {
                return;
            }
            print_MouseLeftButtonDown(print, e);
            IActivity lb = sender as IActivity;
            if (!lb.IsStart())
            {
                MessageBox.Show("不能开始");
                if (null != _newLine)
                {
                    RemoveArrowLine(_newLine);
                }
                return;
            }
            //-->记住当前的节点开始坐标
            lb.DictArrowFootPoint.Add(_newLine.Name, _newLine);
            _newLine.ArrowFootControl = sender as UIElement;
        }

        void act_DotMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (IsActivity && null != _newLine)
            {
                //-->又到里面去了，不用创建此箭头
                RemoveArrowLine(_newLine);
            }
            print_MouseLeftButtonUp(print, e);

            var p = e.GetPosition(print);
            //-->更新所有箭头坐标。
            IActivity lb = sender as IActivity;
            lb.UpdateArrowCapPoint(p);
            lb.UpdateArrowFootPoint(p);
            _newLine = null;
        }

        void act_DotMouseMove(object sender, MouseEventArgs e)
        {
            print_MouseMove(print, e);
        }

        /// <summary>
        /// 
        /// </summary>
        public bool CheckSave()
        {
            //-->判断是否有开始和结束Activity控件。
            bool _StartAct = false;
            bool _EndAct = false;
            foreach (var act in print.Children)
            {
                if (act.GetType().Name.Equals("BeginActivity"))
                {
                    _StartAct = true;
                }
                if (act.GetType().Name.Equals("EndActivity"))
                {
                    _EndAct = true;
                }
                var iact = act as IActivity;
                if (null == iact)
                {
                    continue;
                }
                if (!iact.IsCheck())
                {
                    Wrapper.ShowDialog(string.Format("【{0}】控件配置不正确。", (act as IBaseActivity).LabelContent));
                    return false;
                }
            }
            if (!_StartAct)
            {
                Wrapper.ShowDialog("没有开始控件。");
                return false;
            }
            if (!_EndAct)
            {
                Wrapper.ShowDialog("没有结束控件。");
                return false;
            }
            btnSave_Click(null, null);
            return true;
        }
    }
}
