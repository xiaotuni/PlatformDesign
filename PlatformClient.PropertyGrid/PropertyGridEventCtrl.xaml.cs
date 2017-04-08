using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;
using PlatformClient.Common;
using PlatformClient.Common.Interfaces;
using PlatformClient.Model.Method;
using PlatformClient.PropertyGrid.Events;
using PlatformClient.Utility;
using PlatformClient.Extend.Core;

namespace PlatformClient.PropertyGrid
{
    /// <summary>
    /// 
    /// </summary>
    public partial class PropertyGridEventCtrl : UserControl, IUserControl
    {
        List<EventInfoAttribute> item = null;
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
        public PropertyGridEventCtrl()
        {
            InitializeComponent();

            this.pgSearch.InputSearch += pgSearch_InputSearch;
        }

        void pgSearch_InputSearch(object sender, InputSearchEventArgs e)
        {

        }

        /// <summary>
        /// 事件
        /// </summary>
        /// <param name="ui"></param>
        internal void ReadEvent(UIElement ui)
        {
            DisposePValue();
            spPValue.Children.Clear();
            if (null == ui)
            {
                return;
            }
            item = Wrapper.ReadControlAttribute<EventInfoAttribute>(ui);
            item.InsertRange(0, Wrapper.CommonEventItem(ui));
            _ReadUIElement = ui;
            AddPropertyToStackPanel(item);
        }

        void AddPropertyToStackPanel(List<EventInfoAttribute> item)
        {
            double height = ConstantCollection.HEIGHT;

            string controlName = string.Format("{0}", this._ReadUIElement.GetValue(Canvas.NameProperty));
            if (controlName.Length > 4 && controlName.Substring(0, 4).Equals("Form"))
            {
                controlName = "Form";
            }
            var _xml = IDesignFramework.GetCurrentXmlTemplate();

            foreach (var v in item)
            {
                PropertyGridEventBindCtrl pgec = new PropertyGridEventBindCtrl();
                pgec.Name = string.Format("pgec_{0}", v.Name);
                pgec.ShowName = v.Description;
                pgec.Height = height;
                pgec.AssemblyQualifiedName = v.AssemblyQualifiedName;
                pgec.MethodName = v.Name;
                pgec.ParentControl = _ReadUIElement;
                pgec.IDesignFramework = this.IDesignFramework;
                pgec.LostFocus += new RoutedEventHandler(pgec_LostFocus);
                pgec.AttributeInfo = v;
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

        void pgec_LostFocus(object sender, RoutedEventArgs e)
        {

        }

        void DisposePValue()
        {
            foreach (var v in spPValue.Children)
            {
                PropertyGridEventBindCtrl pgec = v as PropertyGridEventBindCtrl;
                pgec.LostFocus -= pgec_LostFocus;
                pgec.Dispose();
            }
            spPValue.Children.Clear();
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

        internal void ReadEvent(UIElement ui, IPageDesignFramework iPageDesignFramework)
        {
            this.IDesignFramework = iPageDesignFramework;
            this.ReadEvent(ui);
        }
    }
}
