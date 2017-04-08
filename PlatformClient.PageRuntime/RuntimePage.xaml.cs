using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;
using PlatformClient.Common.Interfaces;
using PlatformClient.Common.Lib;
using PlatformClient.CompositeCtrl;
using PlatformClient.Extend.Core;
using PlatformClient.Model.Method;
using PlatformClient.Utility;
using PlatformClient.Utility.ParseXml;

namespace PlatformClient.PageRuntime
{
    /// <summary>
    /// 运行时页面
    /// </summary>
    public partial class RuntimePage : UserControl
    {
        XmlTemplate _XmlTemplate;
        XamlTemplate _XamlTemplate;
        /// <summary>
        /// 当前界面所有控件
        /// </summary>
        Dictionary<string, FrameworkElement> _DictControl = new Dictionary<string, FrameworkElement>();
        /// <summary>
        /// 是否开始初始化控件
        /// </summary>
        bool IsBeginInitControl = false;

        /// <summary>
        /// 运行时管理接口
        /// </summary>
        public IRuntimeManagerPage IRuntimePageManager { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public RuntimePage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmd"></param>
        internal void DoAction(CtrlPlatformCommandInfo cmd)
        {
            //-->判断是否是复合控件了。
            if (cmd.CommandType == CtrlCommandDirectionType.CompositeControl)
            {
                var compositeControlName = cmd.TempValue.GetAttributeValue(ConstantCollection.COMPOSITE_CONTROL_NAME);
                var icc = this.FindControl(compositeControlName) as ICompositeCtrl;
                if (null == icc)
                {
                    return;
                }
                icc.DoAction(cmd);
            }
            else
            {
                //-->当前
                try
                {
                    string mn = cmd.TempValue.GetAttributeValue(ConstantCollection.METHOD_NAME);
                    string _MethodName = string.Format("Process{0}", mn);
                    var mi = this.GetType().GetMethod(_MethodName, BindingFlags.Instance | BindingFlags.NonPublic);
                    if (null == mi)
                    {
                        AlertMessage("没有找着【" + _MethodName + "】");
                        return;
                    }
                    mi.Invoke(this, new object[] { cmd });
                }
                catch (Exception ee)
                {
                    AlertMessage(ee);
                }
            }
        }

        /// <summary>
        /// 重绘
        /// </summary>
        internal void Redraw()
        {
            if (null == PageInfo || this.PageInfo.PageContent.IsNullOrEmpty())
            {
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
            _XmlTemplate = XmlTemplate.DecodeXml(xml);
            _XamlTemplate = XamlTemplate.DecodeXml(xaml);

            UserControl uc = Wrapper.CreateUIElement(xaml) as UserControl;
            if (null == uc)
            {
                return;
            }
            this.LayoutRoot.Children.Add(uc);

            //-->给复合控件设置标题内容,和运行时接口。
            SetCompositeCtrlTitleOrInterface();

            //-->给控件注册事件
            RegisterControl();

            //-->加载初化操作。
            LoadInitControl();
        }


        /// <summary>
        /// 给控件注册事件
        /// </summary>
        void RegisterControl()
        {
            if (null == _XmlTemplate || null == _XmlTemplate.EventBindItem)
            {
                return;
            }
            var ebis = _XmlTemplate.EventBindItem;
            foreach (var ebi in ebis)
            {
                //-->找出控件
                var control = FindControl(ebi.ControlName);
                var cc = ParseControlConfig.GetControlConfig(control.GetType().Name);
                foreach (var cebi in ebi.Item)
                {
                    var _be = cc.Events.Where(p => p.Name.Equals(cebi.EventName)).GetFirst<BindEvent>();
                    if (null == _be)
                    {
                        continue;
                    }
                    Wrapper.ControlRegisterEvent(control, _be.Name, _be.AssemblyName, _be.BindFunctionName);
                    control.Tag = this;
                }
            }
        }

        /// <summary>
        /// 设置控件标题以及运行时接口
        /// </summary>
        void SetCompositeCtrlTitleOrInterface()
        {
            if (null == _XmlTemplate || null == _XmlTemplate.ControlItem || null == _XmlTemplate.MetaDataItem)
            {
                return;
            }

            foreach (var ci in _XmlTemplate.ControlItem)
            {
                FrameworkElement fe = null;
                string controlName = Wrapper.ParseControlName(ci.Name);

                MetaDataInfo _mdi = new MetaDataInfo();
                _mdi.Item = new List<MetaDataColumnInfo>();

                if (ci.ControlType.Equals(ConstantCollection.FORM_PREFIX))
                {
                    fe = new XtnVirtualFormCtrl();
                    fe.Name = ci.Name;
                    _DictControl.Add(fe.Name, fe);
                    //-->查找 metadata 里 row里的信息
                    var mdi = GetMetaDataInfoByTableName(ci.MetaData);
                    if (null != mdi)
                    {
                        var arr = ci.ColumnName.Split('|');
                        foreach (var cn in arr)
                        {
                            var mdci = mdi.Item.Where(p => p.column_name.Equals(cn)).GetFirst<MetaDataColumnInfo>();
                            if (null == mdci)
                            {
                                continue;
                            }
                            _mdi.Item.Add(mdci);
                        }
                        _mdi.table_comment = mdi.table_comment;
                        _mdi.table_name = mdi.table_name;
                        _mdi.table_type = mdi.table_type;
                    }
                    else
                    {
                        _mdi.table_name = ci.MetaData;
                        _mdi.table_comment = ci.ColumnName;
                    }
                }
                else
                {
                    fe = FindControl(ci.Name);
                    if (null == fe)
                    {
                        continue;
                    }
                    if (!(fe is ICompositeCtrl))
                    {
                        continue;
                    }
                    //-->查找 metadata 里 row里的信息
                    var mdi = GetMetaDataInfoByTableName(ci.MetaData);
                    if (null != mdi)
                    {
                        var mdci = mdi.Item.Where(p => p.column_name.Equals(ci.ColumnName)).GetFirst<MetaDataColumnInfo>();
                        if (null != mdci)
                        {
                            _mdi.Item.Add(mdci);
                        }
                        _mdi.table_comment = mdi.table_comment;
                        _mdi.table_name = mdi.table_name;
                        _mdi.table_type = mdi.table_type;
                    }
                    else
                    {
                        _mdi.table_name = ci.MetaData;
                        _mdi.table_comment = ci.ColumnName;
                    }
                }
                var icc = fe as ICompositeCtrl;
                CtrlPlatformCommandInfo ctrlCmd = new CtrlPlatformCommandInfo();
                ctrlCmd.ControlInfo = ci;
                ctrlCmd.MetaDataInfo = _mdi;
                fe.Tag = ctrlCmd;
                icc.InitTitle(ctrlCmd);
                icc.SetPageRuntimeInterface(this);

            }
        }
    }
}
