using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml.Linq;
using PlatformClient.Common;
using PlatformClient.Common.Interfaces;
using PlatformClient.Common.Lib;
using PlatformClient.DataBaseDesign;
using PlatformClient.Extend.Core;
using PlatformClient.Model.Events;
using PlatformClient.Model.Method;
using PlatformClient.Model.Table;
using PlatformClient.PropertyGrid.Events;
using PlatformClient.Utility;
using PlatformClient.Utility.Events;
using PlatformClient.Utility.ParseXml;

namespace PlatformClient.PageDesignTime
{
    /// <summary>
    /// 
    /// </summary>
    public partial class PageWorkArea : BaseControl, IDisposable
    {
        /// <summary>
        /// 控件位置发生变发
        /// </summary>
        public event EventHandler<ControlModifyPropertyEventArgs> ControlPosition;
        /// <summary>
        /// 增加或删除控件【主要是用来对设计时，更新左边那树里控件结构用】
        /// </summary>
        public event EventHandler<ControlTimesUpdateEventArgs> ControlTimesUpdate;

        /// <summary>
        /// 内容修改
        /// </summary>
        public event RoutedEventHandler ContentUpdate;
        /// <summary>
        /// 页面信息
        /// </summary>
        internal PageDirectorySub PageInfo { get; set; }
        /// <summary>
        /// 是否更新
        /// </summary>
        bool _IsUpdate;
        /// <summary>
        /// 是否有更新
        /// </summary>
        internal bool IsUpdate
        {
            get { return _IsUpdate; }
            set
            {
                var result = value;
                if (result != _IsUpdate)
                {
                    _IsUpdate = value;
                    _ContentUpdateMethod(this, null);
                }
            }
        }
        /// <summary>
        /// 父级控件
        /// </summary>
        internal UIElement ParentControl { get; set; }
        /// <summary>
        /// 框架接口
        /// </summary>
        IPageDesignFramework _IDesignFramework;
        /// <summary>
        /// xml模板类
        /// </summary>
        public XmlTemplate _XmlTemplate;
        /// <summary>
        /// xaml模板类
        /// </summary>
        public XamlTemplate _XamlTemplate;
        string _Xml_Template;
        string _Xaml_Template;
        private bool _IsOpenPage;
        /// <summary>
        /// 最后xaml字符内容
        /// </summary>
        string Xaml_Template
        {
            get { return _Xaml_Template; }
            set
            {
                string newValue = value;
                if (!newValue.Equals(_Xaml_Template))
                {
                    _Xaml_Template = value;
                    this.txtXaml.Text = value;
                    this.IsUpdate = true;
                }
            }
        }
        /// <summary>
        /// 最后xml字符内容
        /// </summary>
        string Xml_Template
        {
            get { return _Xml_Template; }
            set
            {
                string newValue = value;
                if (!newValue.Equals(_Xml_Template))
                {
                    _Xml_Template = value;
                    this.txtXml.Text = value;
                    this.IsUpdate = true;
                }
            }
        }
        /// <summary>
        /// 页面的xaml
        /// </summary>
        public string PageXaml { get { return this.txtXaml.Text.Trim(); } }
        /// <summary>
        /// 页面的xml
        /// </summary>
        public string PageXml { get { return this.txtXml.Text.Trim(); } }
        /// <summary>
        /// 更新左边树控件结构
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _ControlTimesUpdateMethod(object sender, ControlTimesUpdateEventArgs e)
        {
            if (null != ControlTimesUpdate)
            {
                ControlTimesUpdate(sender, e);
            }
        }
        /// <summary>
        /// 控件位置更新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _ControlPositionMethod(object sender, ControlModifyPropertyEventArgs e)
        {
            if (null != ControlPosition)
            {
                ControlPosition(sender, e);
            }
        }
        /// <summary>
        /// 内容更新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _ContentUpdateMethod(object sender, RoutedEventArgs e)
        {
            if (null != ContentUpdate)
            {
                ContentUpdate(sender, e);
            }
        }

        /// <summary>
        /// 结构函数
        /// </summary>
        public PageWorkArea()
        {
            InitializeComponent();

            IsUpdate = false;
            _XmlTemplate = new XmlTemplate();
            _XamlTemplate = new XamlTemplate();

            DesignPage.ControlPosition += PageDesign_ControlPosition;
            DesignPage.ControlDelete += DesignPage_ControlDelete;
            txtXaml.LostFocus += txtXaml_LostFocus;
            txtXml.LostFocus += txtXml_LostFocus;
        }

        void txtXml_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                string _xml = this.txtXml.Text.Trim();
                if (Xml_Template.Equals(_xml))
                {
                    return;
                }
                var xml = XmlTemplate.DecodeXml(this.txtXml.Text.Trim());
                double width = xml.BaseInfo.PageWidth;
                double height = xml.BaseInfo.PageHeight;
                //-->xml里的宽与高
                this.DesignPage.Width = width;
                this.DesignPage.Height = height;

                _XmlTemplate = xml;
                Xml_Template = _xml;

                //-->xaml里面的高与宽
                this._XamlTemplate.Height = height;
                this._XamlTemplate.Width = width;
                Xaml_Template = _XamlTemplate.ParseXml().ToString();
            }
            catch
            {
                double width = _XmlTemplate.BaseInfo.PageWidth;
                double height = _XmlTemplate.BaseInfo.PageHeight;
                this.DesignPage.Width = width;
                this.DesignPage.Height = height;
            }
        }

        void txtXaml_LostFocus(object sender, RoutedEventArgs e)
        {
            _IsOpenPage = false;
            Xaml_LostFocus(Xaml_Template, this.txtXaml.Text.Trim());
        }

        void Xaml_LostFocus(string orgValue, string changeXaml)
        {
            if (changeXaml.IsNullOrEmpty() || changeXaml.Equals(orgValue))
            {
                return;
            }
            //-->判断控件是否可用。
            FrameworkElement fe = Wrapper.LoadXaml<FrameworkElement>(changeXaml);
            if (null == fe)
            {
                _XamlTemplate = new XamlTemplate();
                this.Xaml_Template = _XamlTemplate.ParseXml().ToString();
                return;
            }
            XamlTemplate new_xamlTemplate = XamlTemplate.DecodeXml(changeXaml);
            //-->判断程序集是否有改变
            this._XamlTemplate.AssemblyCollection.AddRange(new_xamlTemplate.AssemblyCollection);

            XamlCanvasTemplate old_Xaml_CanvasTemplate = this._XamlTemplate.CanvasTemplate;
            //-->判断画布的大小，背景色是否不同
            foreach (var cp in new_xamlTemplate.CanvasTemplate.CanvasPropertyDict)
            {
                if (old_Xaml_CanvasTemplate.CanvasPropertyDict.ContainsKey(cp.Key))
                {
                    if (!old_Xaml_CanvasTemplate.CanvasPropertyDict[cp.Key].Equals(cp.Value))
                    {
                        //-->内容有改变
                        old_Xaml_CanvasTemplate.CanvasPropertyDict[cp.Key] = cp.Value;
                    }
                }
                else
                {
                    old_Xaml_CanvasTemplate.CanvasPropertyDict.Add(cp.Key, cp.Value);
                }
            }
            //-->判断控件
            foreach (var child in new_xamlTemplate.CanvasTemplate.Children)
            {
                FrameworkElement _news_ctrl = fe.FindName(child.Key) as FrameworkElement;
                ((fe as UserControl).Content as Panel).Children.Remove(_news_ctrl);
                if (old_Xaml_CanvasTemplate.Children.ContainsKey(child.Key))
                {
                    if (!old_Xaml_CanvasTemplate.Children[child.Key].Equals(child.Value))
                    {
                        //-->删除原来的控件
                        this.DesignPage.DeleteChilren(child.Key);
                        //-->2、增加新的控件
                        this.DesignPage.AddChild(_news_ctrl, !_IsOpenPage);
                        DesignPage.UpdateDesignRectangle(_news_ctrl);
                        old_Xaml_CanvasTemplate.Children[child.Key] = child.Value;
                    }
                    continue;
                }
                this.DesignPage.AddChild(_news_ctrl, !_IsOpenPage);
                old_Xaml_CanvasTemplate.Children.Add(child.Key, child.Value);
                //-->初始一下内容
                if (_news_ctrl is ICompositeCtrl)
                {
                    var ci = _IDesignFramework.GetControlInfoByControlName(_news_ctrl.Name);
                    if (null == ci)
                    {
                        continue;
                    }
                    //-->查找 metadata 里 row里的信息
                    var ctrlMetaData = _IDesignFramework.GetMetaDataInfoByTableName(ci.MetaData);
                    if (null == ctrlMetaData || null == ctrlMetaData.Item || 0 == ctrlMetaData.Item.Count)
                    {
                        continue;
                    }
                    //--> metaData Column
                    var colXml = ctrlMetaData.Item.Where(p => p.column_name.Equals(ci.ColumnName)).GetFirst<MetaDataColumnInfo>();
                    if (null == colXml)
                    {
                        continue;
                    }
                    var _newCtrlMetaData = new MetaDataInfo();
                    _newCtrlMetaData.table_type = ctrlMetaData.table_type;
                    _newCtrlMetaData.table_name = ctrlMetaData.table_name;
                    _newCtrlMetaData.table_comment = ctrlMetaData.table_comment;
                    _newCtrlMetaData.Item = new List<MetaDataColumnInfo>();
                    _newCtrlMetaData.Item.Add(colXml);

                    CtrlPlatformCommandInfo ctrlCmd = new CtrlPlatformCommandInfo();
                    ctrlCmd.ControlInfo = ci;
                    ctrlCmd.MetaDataInfo = _newCtrlMetaData;
                    (_news_ctrl as ICompositeCtrl).InitTitle(ctrlCmd);
                }
            }
            //-->如果是高、宽改变了，得修改xml文件信息
            this._XmlTemplate.BaseInfo.PageHeight = new_xamlTemplate.Height;
            this._XmlTemplate.BaseInfo.PageWidth = new_xamlTemplate.Width;
            _XamlTemplate.Height = new_xamlTemplate.Height;
            _XamlTemplate.Width = new_xamlTemplate.Width;
            Xml_Template = _XmlTemplate.ParseXml().ToString();
            _XamlTemplate = new_xamlTemplate;

            this.Xaml_Template = changeXaml;
            DesignPage.UpdatePropertyGrid();
        }

        /// <summary>
        /// 删除操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void DesignPage_ControlDelete(object sender, ControlDeleteEventArgs e)
        {
            this.IsUpdate = true;
            string controlName = e.ControlName;
            this._IDesignFramework.UpdateCurrentSelectedCtrl(null);
            DeleteCurrentSelectedControl(controlName);
            //-->如果是Form控件的话，删除最后一个控件的时候，要把虚拟的那个控件也要删除掉
            string v_control = Wrapper.ParseControlName(controlName);
            if (!v_control.Equals(controlName))
            {
                int controls = _XmlTemplate.ControlItem.Where(p => p.Name.Contains(v_control)).Count();
                if (1 == controls)
                {
                    //-->最后一个控件了，现在开始进行删除操作
                    DeleteCurrentSelectedControl(v_control);
                }
            }

            this.Xaml_Template = this._XamlTemplate.ParseXml().ToString();
            this.Xml_Template = _XmlTemplate.ParseXml().ToString();

            UpdateControlTree();
        }

        void DeleteCurrentSelectedControl(string controlName)
        {
            if (_XamlTemplate.CanvasTemplate.Children.ContainsKey(controlName))
            {
                _XamlTemplate.CanvasTemplate.Children.Remove(controlName);
            }
            _XmlTemplate.ControlItem.Remove("Name", controlName);
            _XmlTemplate.EventLinkItem.Remove("ControlName", controlName);
            _XmlTemplate.EventBindItem.Remove("ControlName", controlName);
            _XmlTemplate.PageLoadingItem.Remove("ControlName", controlName);
            //-->还要就是删除数据库里与之相关的记录
            //-->如果是最后一个控件了，请空MetaData里的信息
            if (0 == _XamlTemplate.CanvasTemplate.Children.Count ||
                0 == _XmlTemplate.ControlItem.Count)
            {
                _XmlTemplate.Clear();
            }
            cmd = new CtrlPlatformCommandInfo();
            cmd.CommandName = ConstantCollection.CommandName_MixedCommand;
            cmd.ExecSql = new List<CtrlExecSQLCmd>();
            string deleteSql = string.Format(" delete from EventDesigner where PageGuid = '{0}' and ControlName = '{1}'",
                this.PageInfo.PageGuid,
                controlName);
            var exec = new CtrlExecSQLCmd(CtrlExecSqlCmdType.ExecuteNonQuery, deleteSql, "EventDesigner", this.Name, "DeleteCurrentSelectedControl");
            cmd.ExecSql.Add(exec);

            cmd.TempValue = Wrapper.SetXmlValue("EventDesigner", "DeleteCurrentSelectedControl");

            this.SendToService(cmd, this);
        }

        /// <summary>
        /// 处理删除当前选中控件
        /// </summary>
        /// <param name="cmd"></param>
        public void ProcessDeleteCurrentSelectedControl(CtrlPlatformCommandInfo cmd)
        {
            if (!cmd.ExecuteNonQueryResult.IsNullOrEmpty())
            {
                Wrapper.ShowDialog(cmd.ExecuteNonQueryResult.ToString());
                return;
            }
        }

        /// <summary>
        /// 控件位置改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void PageDesign_ControlPosition(object sender, ControlModifyPropertyEventArgs e)
        {
            string controlName = e.ControlName;
            if (!_XamlTemplate.CanvasTemplate.Children.ContainsKey(controlName))
            {
                return;
            }
            this.IsUpdate = true;
            string controlXaml = _XamlTemplate.CanvasTemplate.Children[controlName];
            controlXaml = Wrapper.ModifyXamlAttribute(e.DictProperty, controlXaml);
            if (_XamlTemplate.CanvasTemplate.Children.ContainsKey(controlName))
            {
                _XamlTemplate.CanvasTemplate.Children.Remove(controlName);
            }
            _XamlTemplate.CanvasTemplate.Children.Add(controlName, controlXaml);
            _ControlPositionMethod(sender, e);
            Xaml_Template = _XamlTemplate.ParseXml().ToString();
        }

        /// <summary>
        /// 增加控件
        /// </summary>
        /// <param name="fe"></param>
        internal void AddChild(FrameworkElement fe)
        {
            this.IsUpdate = true;
            _IsOpenPage = false;
            //-->获取当前最大的控件序号
            _XmlTemplate.BaseInfo.SerialNum++;
            int SerialNum = _XmlTemplate.BaseInfo.SerialNum;
            string controlTypeName = string.Format("{0}", fe.GetType().Name);
            string controlName = string.Format("{0}{1}", controlTypeName, SerialNum);
            fe.Name = controlName;
            UpdateXaml(fe);
            string controlType = fe is ICompositeCtrl ? "CompositeCtrl" : "BaseControl";
            UpdateXml(fe, null, null, null, null, null);
            this.Xaml_Template = _XamlTemplate.ParseXml().ToString();
            this.Xml_Template = _XmlTemplate.ParseXml().ToString();
            this.DesignPage.AddChild(fe, true);

            UpdateControlTree();
        }

        /// <summary>
        /// 更新xml文件内容
        /// </summary>
        /// <param name="ui"></param>
        /// <param name="columnName"></param>
        /// <param name="comment"></param>
        /// <param name="tableName"></param>
        /// <param name="parentCtrlName"></param>
        /// <param name="mdi"></param>
        void UpdateXml(FrameworkElement ui, string columnName, string comment, string tableName, string parentCtrlName, MetaDataInfo mdi)
        {
            ControlInfo ci = new ControlInfo();
            ci.Name = ui.Name;
            ci.ColumnName = columnName;
            ci.MetaData = tableName;
            ci.Comment = comment;
            ci.Type = ui.GetType().Name;
            if (!parentCtrlName.IsNullOrEmpty())
            {
                ci.ParentCtrlName = parentCtrlName;
            }
            if (ui is ICompositeCtrl)
            {
                //-->查找 metadata 里 row里的信息
                ci.ControlType = ConstantCollection.COMPOSITE_CONTROL;
                CtrlPlatformCommandInfo ctrlCmd = new CtrlPlatformCommandInfo();
                ctrlCmd.MetaDataInfo = mdi;
                ctrlCmd.ControlInfo = ci;
                (ui as ICompositeCtrl).InitTitle(ctrlCmd);
            }
            _XmlTemplate.ControlItem.Add(ci);
        }

        /// <summary>
        /// 更新Xaml信息
        /// </summary>
        /// <param name="fe"></param>
        void UpdateXaml(FrameworkElement fe)
        {
            //-->判断是否是复合控件。
            ControlConfig cc = fe.Tag as ControlConfig;
            if (cc.IsCompositeCtrl && !cc.AssemblyQualifiedName.IsNullOrEmpty())
            {
                _XamlTemplate.AssemblyCollection.Add(cc.AssemblyQualifiedName.Trim());
            }
            string controlName = fe.Name;
            string controlXaml = cc.xaml.Replace("{0}", "").Replace("??", controlName).Trim();
            if (_XamlTemplate.CanvasTemplate.Children.ContainsKey(controlName))
            {
                _XamlTemplate.CanvasTemplate.Children.Remove(controlName);
            }
            _XamlTemplate.CanvasTemplate.Children.Add(controlName, controlXaml);
        }

        /// <summary>
        /// 属性失去焦点事件 
        /// </summary>
        /// <param name="e"></param>
        internal void PropertyLostFoucs(EvaluationCtrlLostFocusEventArgs e)
        {
            string controlName = e.ControlName;
            if (!_XamlTemplate.CanvasTemplate.Children.ContainsKey(controlName))
            {
                return;
            }
            this.IsUpdate = true;
            string controlXaml = _XamlTemplate.CanvasTemplate.Children[controlName];
            Dictionary<String, object> modify = new Dictionary<string, object>();
            modify.Add(e.PropertyName, e.Value);
            controlXaml = Wrapper.ModifyXamlAttribute(modify, controlXaml);
            if (_XamlTemplate.CanvasTemplate.Children.ContainsKey(controlName))
            {
                _XamlTemplate.CanvasTemplate.Children.Remove(controlName);
            }
            _XamlTemplate.CanvasTemplate.Children.Add(controlName, controlXaml);
            this.Xaml_Template = _XamlTemplate.ParseXml().ToString();
        }

        /// <summary>
        /// 显示
        /// </summary>
        internal void PageView()
        {
            var ui = Wrapper.LoadXaml<FrameworkElement>(this.txtXaml.Text);
            var childDisplay = new XtnChildWindow(this._IDesignFramework, ui);
            childDisplay.Closed += childDisplay_Closed;
            childDisplay.Show();
        }

        void childDisplay_Closed(object sender, EventArgs e)
        {
            var childDisplay = sender as XtnChildWindow;
            if (null != childDisplay)
            {
                childDisplay.Closed -= childDisplay_Closed;
                childDisplay.Dispose();
            }
        }

        /// <summary>
        /// 打开数据库框架
        /// </summary>
        internal void OpenDBFramework()
        {
            DBFramework dbf = new DBFramework();
            var child = new XtnChildWindow(this._IDesignFramework, dbf);
            child.Closed += child_Closed;
            child.Show();
        }

        void child_Closed(object sender, EventArgs e)
        {
            var child = (sender as XtnChildWindow);
            if (true != child.DialogResult)
            {
                return;
            }
            DBFramework dbf = child.GetControl as DBFramework;
            var mdi = dbf.GetMetaDataInfo;
            if (null == mdi || 0 == mdi.Item.Count)
            {
                return;
            }
            //-->判断tableName是否已经增加了
            MetaDataInfo old_mdi = GetMetaDataInfoByTableName(mdi.table_name);
            if (null == old_mdi)
            {
                _XmlTemplate.MetaDataItem.Add(mdi);
            }
            //-->增加控件
            _XmlTemplate.BaseInfo.SerialNum++;
            int SerialNum = _XmlTemplate.BaseInfo.SerialNum;
            double top = 0;
            double left = 30;
            double maxWidth = 0;
            List<String> columns = new List<string>();
            List<FrameworkElement> feItem = new List<FrameworkElement>();
            Dictionary<String, String> dictTopLeft = new Dictionary<string, string>();

            MetaDataInfo ctrlMetaData = new MetaDataInfo();
            ctrlMetaData.table_name = mdi.table_name;
            ctrlMetaData.table_comment = mdi.table_comment;
            ctrlMetaData.table_type = mdi.table_type;

            foreach (var ctrl in mdi.Item)
            {
                if (!ctrl.IsChoose)
                {
                    continue;
                }
                ctrlMetaData.Item = new List<MetaDataColumnInfo>();
                ctrlMetaData.Item.Add(ctrl);
                var fe = Wrapper.CreateCtrl<FrameworkElement>(ctrl.control_type);
                string controlName = string.Format("Form{0}_{1}", SerialNum, ctrl.column_name);
                fe.Name = controlName;
                this.DesignPage.AddChild(fe, false);
                feItem.Add(fe);
                UpdateXaml(fe);
                UpdateXml(fe, ctrl.column_name, ctrl.column_comment, mdi.table_name, string.Format("Form{0}", SerialNum), ctrlMetaData);
                double _maxWidth = GetCharWidth(ctrl.column_comment, this.FontSize, this.FontFamily, this.FontWeight);
                maxWidth = _maxWidth > maxWidth ? _maxWidth : maxWidth;
                columns.Add(ctrl.column_name);
                //-->修改期坐标位置
                if ((top + 30 + fe.Height + 30) < DesignPage.ActualHeight)
                {
                    top += 30;
                }
                else
                {
                    top = 30;
                    if ((left + 190) < DesignPage.ActualWidth)
                    {
                        left += 190;
                    }
                }
                dictTopLeft.Add(controlName, string.Format("{0};{1}", top, left));
                fe.SetValue(Canvas.LeftProperty, left);
                fe.SetValue(Canvas.TopProperty, top);
            }
            foreach (var v in feItem)
            {
                if (v is ICompositeCtrl)
                {
                    (v as ICompositeCtrl).TitleWidth = (int)maxWidth + 1;
                    v.Width = (int)maxWidth + 100 + 1;
                    //-->修改xaml的位置坐标
                    string t_l = dictTopLeft[v.Name];
                    string new_value = Wrapper.ModifyAttribute(_XamlTemplate.CanvasTemplate.Children[v.Name],
                        new KeyValuePair<String, Object>[] { 
                        new KeyValuePair<String,Object>("Canvas.Top",t_l.Split(';')[0]),
                        new KeyValuePair<String,Object>("Canvas.Left",t_l.Split(';')[1]),
                        new KeyValuePair<String,Object>("Width",v.Width),
                        new KeyValuePair<String,Object>("TitleWidth",(int)maxWidth + 1),
                    });
                    _XamlTemplate.CanvasTemplate.Children[v.Name] = new_value;
                }
            }
            feItem.Clear();
            dictTopLeft.Clear();
            //-->创建一个虚拟控件
            ControlInfo ci = new ControlInfo();
            ci.Name = string.Format("Form{0}", SerialNum);
            ci.ColumnName = string.Join("|", columns);
            ci.MetaData = mdi.table_name;
            ci.Comment = mdi.table_comment;
            ci.ControlType = ConstantCollection.FORM_PREFIX;
            ci.ParentCtrlName = "LayoutRoot";
            ci.Type = ConstantCollection.FORM_PREFIX;//
            //ci.ControlDataSourceTableName = mdi.table_name;
            _XmlTemplate.ControlItem.Add(ci);

            this.Xaml_Template = _XamlTemplate.ParseXml().ToString();
            this.Xml_Template = _XmlTemplate.ParseXml().ToString();

            UpdateControlTree();

            //-->释放窗体里的资源
            child.Closed -= child_Closed;
            child.Dispose();
        }

        double GetCharWidth(string value, double fontSize, FontFamily fontFamily, FontWeight fontWeight)
        {
            TextBlock tb = new TextBlock();
            if (0 < fontSize)
            {
                tb.FontSize = fontSize;
            }
            if (null != fontFamily)
            {
                tb.FontFamily = fontFamily;
            }
            if (null != fontWeight)
            {
                tb.FontWeight = fontWeight;
            }
            tb.Text = value;// "宽";
            TextBlock tb2 = tb;
            return new Size(tb2.ActualWidth, tb2.ActualHeight).Width;
        }

        /// <summary>
        /// 打开页面
        /// </summary>
        /// <param name="iPage"></param>
        /// <param name="e"></param>
        internal void OpenPageInfo(OpenPageInfoEventArgs e, IPageDesignFramework iPage)
        {
            _IsOpenPage = true;
            this._IDesignFramework = iPage;
            this.PageInfo = e.PageDirectorySub;
            this.DesignPage.SetDesigntimeInterface(iPage);
            //-->判断 this.PageInfo.PageContent 是否为空。
            //-->如果不为空说就把里面的信息分开，然后分别放到 txtXaml,txtXml控件里去
            this.txtXaml.Text = string.Empty;
            this.txtXml.Text = string.Empty;
            if (this.PageInfo.PageContent.IsNullOrEmpty())
            {
                this.IsUpdate = true;
                //-->说明刚创建的一个新的页面。
                CreateNewPageInfo(e);
                return;
            }
            XElement _xe_page = XElement.Parse(this.PageInfo.PageContent.Trim());
            if (null == _xe_page)
            {
                return;
            }
            XElement _xe_xaml = _xe_page.Element("xaml");
            XElement _xe_xml = _xe_page.Element("xml");

            string xaml = _xe_xaml != null && _xe_xaml.FirstNode != null ? _xe_xaml.FirstNode.ToString() : "";// _xe_xaml.GetElementValue("xaml");
            string xml = _xe_xml != null && _xe_xml.FirstNode != null ? _xe_xml.FirstNode.ToString() : "";
            //-->xml操作
            DecodeXml(xml);
            //-->获取xaml
            Xaml_LostFocus("", xaml);
            DesignPage.Height = _XmlTemplate.BaseInfo.PageHeight;
            DesignPage.Width = _XmlTemplate.BaseInfo.PageWidth;

            UpdateControlTree();
            _IsOpenPage = false;
            this.IsUpdate = false;
        }

        void DecodeXml(string xml)
        {
            if (xml.IsNullOrEmpty())
            {
                BuilderXml(this.PageInfo);
                return;
            }
            _XmlTemplate = XmlTemplate.DecodeXml(xml);
            this.Xml_Template = xml;
        }

        /// <summary>
        /// 创建一个新的页面
        /// </summary>
        /// <param name="e"></param>
        void CreateNewPageInfo(OpenPageInfoEventArgs e)
        {
            this.Xaml_Template = _XamlTemplate.ParseXml().ToString();
            BuilderXml(e.PageDirectorySub);
        }

        /// <summary>
        /// 生成一个xml文件
        /// </summary>
        /// <param name="e"></param>
        void BuilderXml(PageDirectorySub e)
        {
            _XmlTemplate.BaseInfo.PageName = e.PageName;
            _XmlTemplate.BaseInfo.PageGUID = e.PageGuid;

            this.Xml_Template = _XmlTemplate.ParseXml().ToString();
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public override void Dispose()
        {
            DesignPage.Dispose();
            DesignPage.ControlPosition -= PageDesign_ControlPosition;
            DesignPage.ControlDelete -= DesignPage_ControlDelete;
            txtXaml.LostFocus -= txtXaml_LostFocus;
            txtXml.LostFocus -= txtXml_LostFocus;
            this._IDesignFramework = null;
        }

        /// <summary>
        /// 更新xml PageName的值
        /// </summary>
        /// <param name="newName"></param>
        internal void UpdateXmlPageNameValue(string newName)
        {
            XElement result = XElement.Parse(Xml_Template);
            Wrapper.ModifyAttribute(result.Element("PageBaseInfo"),
                new KeyValuePair<String, Object>[] { 
                    new KeyValuePair<String,object>("PageName",newName)
                });
            this.Xml_Template = result.ToString();
        }

        /// <summary>
        /// 更新控件数
        /// </summary>
        internal void UpdateControlTree()
        {
            var e = new ControlTimesUpdateEventArgs();
            e.Controls = _XmlTemplate.ControlItem;
            _ControlTimesUpdateMethod(this, e);
        }

        /// <summary>
        /// 在切换tableControl控件的时候,更新属性以及控件树
        /// </summary>
        internal void UpdatePropertyGridAndControlTree()
        {
            UpdateControlTree();
            DesignPage.UpdatePropertyGrid();
        }
        /// <summary>
        /// 更新当前模板
        /// </summary>
        internal void UpdateCurrentTemplate()
        {
            this.DesignPage.Width = _XmlTemplate.BaseInfo.PageWidth;
            this.DesignPage.Height = _XmlTemplate.BaseInfo.PageHeight;
            this.Xml_Template = this._XmlTemplate.ParseXml().ToString();
            this.Xaml_Template = this._XamlTemplate.ParseXml().ToString();
        }
        /// <summary>
        /// 查找控件
        /// </summary>
        /// <param name="controlName"></param>
        internal UIElement FindControl(string controlName)
        {
            return this.DesignPage.FindControl(controlName);
        }

        /// <summary>
        /// 获取控件信息
        /// </summary>
        /// <param name="controlName">控件名称</param>
        /// <returns>返回控件信息</returns>
        internal ControlInfo GetControlInfoByControlName(string controlName)
        {
            //if (controlName.IsNullOrEmpty() || null == _XmlTemplate || null == _XmlTemplate.MetaDataItem || 0 == _XmlTemplate.MetaDataItem.Count)
            if (controlName.IsNullOrEmpty() || null == _XmlTemplate || null == _XmlTemplate.ControlItem || 0 == _XmlTemplate.ControlItem.Count)
            {
                return null;
            }
            return _XmlTemplate.ControlItem.Where(p => p.Name.Equals(controlName)).GetFirst<ControlInfo>();
        }

        /// <summary>
        /// 获取MetaData信息
        /// </summary>
        /// <param name="tableName">表的名称</param>
        /// <returns>返回一个MetaData对象</returns>
        internal MetaDataInfo GetMetaDataInfoByTableName(string tableName)
        {
            if (tableName.IsNullOrEmpty() || null == _XmlTemplate || null == _XmlTemplate.MetaDataItem || 0 == _XmlTemplate.MetaDataItem.Count)
            {
                return null;
            }
            return _XmlTemplate.MetaDataItem.Where(p => p.table_name.Equals(tableName)).GetFirst<MetaDataInfo>();
        }

        /// <summary>
        /// 设置当前选择的控件
        /// </summary>
        /// <param name="controlName"></param>
        internal void ControlTreeSelectedEventUpdateCurrentSelectedControl(string controlName)
        {
            this.DesignPage.UpdateSelectedControl(controlName);
        }
    }
}
