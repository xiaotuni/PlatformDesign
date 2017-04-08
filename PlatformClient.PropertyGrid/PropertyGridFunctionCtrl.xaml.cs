using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;
using PlatformClient.Extend.Core;
using PlatformClient.Common;
using PlatformClient.Common.Interfaces;
using PlatformClient.Common.Lib;
using PlatformClient.Model.Method;
using PlatformClient.PropertyGrid.Events;
using PlatformClient.Utility;
using PlatformClient.Utility.ParseXml;

namespace PlatformClient.PropertyGrid
{
    /// <summary>
    /// 
    /// </summary>
    public partial class PropertyGridFunctionCtrl : UserControl, IUserControl
    {
        UIElement _ReadUIElement;
        /// <summary>
        /// 当前选中控件名称
        /// </summary>
        public string CurrentSelectedControlName { get; set; }

        /// <summary>
        /// 设计时框架接口
        /// </summary>
        public IPageDesignFramework IDesignFramework { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public XElement EvaluationContent { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public PropertyGridFunctionCtrl()
        {
            InitializeComponent();

            this.pgSearch.InputSearch += pgSearch_InputSearch;
        }

        void pgSearch_InputSearch(object sender, InputSearchEventArgs e)
        {

        }

        /// <summary>
        /// 方法
        /// </summary>
        /// <param name="ui"></param>
        internal void ReadFunctions(UIElement ui)
        {
            spPValue.Children.Clear();
            DisposePValue();
            _ReadUIElement = ui;
            if (null == ui)
            {
                return;
            }
            var cc = ParseControlConfig.GetControlConfig(ui.GetType().Name);
            if (null == cc)
            {
                return;
            }
            //-->如果是Form里的子控件话吧，还得把Form里的函数取出来。
            string controlName = string.Format("{0}", this._ReadUIElement.GetValue(Canvas.NameProperty));

            var _xml = IDesignFramework.GetCurrentXmlTemplate();
            AddFunctionToStackPanel(controlName, cc.Functions, _xml);

            string _newControlName = Wrapper.ParseControlName(controlName);
            if (_newControlName.Equals(controlName))
            {
                return;
            }
            //-->说明此控件是Form控件里的一个子项
            var _formCC = ParseControlConfig.GetControlConfig(ConstantCollection.FORM_PREFIX);
            if (null == _formCC || null == _formCC.Functions || 0 == _formCC.Functions.Count)
            {
                return;
            }
            AddFunctionToStackPanel(_newControlName, _formCC.Functions, _xml);
        }

        void AddFunctionToStackPanel(string controlName, List<FunctionInfo> item, XmlTemplate _xml)
        {
            if (null == item || 0 == item.Count)
            {
                return;
            }
            double height = ConstantCollection.HEIGHT;

            foreach (var v in item)
            {
                PropertyGridEventBindCtrl pgec = new PropertyGridEventBindCtrl();
                pgec.Name = string.Format("pgec_Fun_{0}", v.Name);
                pgec.ShowName = v.Description;
                pgec.Height = height;
                pgec.MethodName = v.Name;
                pgec.ParentControl = _ReadUIElement;
                pgec.IDesignFramework = this.IDesignFramework;
                pgec.LostFocus += pgec_LostFocus;
                pgec.AttributeInfo = new EventInfoAttribute(v.Name, null, v.Description, v.EventDesigner);
                pgec.CurrentSelectedControlName = this.CurrentSelectedControlName;

                var eli = _xml.EventLinkItem.Where(
                          p => p.ControlName.Equals(controlName) &&
                          p.EventName.Equals(pgec.MethodName)).GetFirst<EventLinkInfo>();
                if (null != eli && 0 < eli.Item.Count)
                {
                    var _eventItem = from p in eli.Item
                                     select p.Description.IsNullOrEmpty() ? p.EventName : p.Description;
                    string _value = string.Join(",", _eventItem);
                    string _messageTip = string.Format("[{0}]调用[{1}]方法;{2}", pgec.MethodName, _eventItem.Count(), _value);

                    pgec.LoadInitValue(_value, _messageTip);
                }

                spPValue.Children.Add(pgec);
            }
        }

        void DisposePValue()
        {
            foreach (var child in spPValue.Children)
            {
                PropertyGridEventBindCtrl pgec = child as PropertyGridEventBindCtrl;
                pgec.LostFocus -= pgec_LostFocus;
                pgec.Dispose();
            }
            spPValue.Children.Clear();
        }

        void pgec_LostFocus(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            DisposePValue();
            pgSearch.InputSearch -= pgSearch_InputSearch;
            if (pgSearch is IDisposable)
            {
                (pgSearch as IDisposable).Dispose();
            }
        }
    }
}
