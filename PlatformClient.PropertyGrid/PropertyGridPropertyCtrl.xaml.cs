using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;
using PlatformClient.Extend.Core;
using PlatformClient.Common;
using PlatformClient.Common.Interfaces;
using PlatformClient.Model.Method;
using PlatformClient.PropertyGrid.Events;
using PlatformClient.Utility;
using PlatformClient.Utility.Events;

namespace PlatformClient.PropertyGrid
{
    /// <summary>
    /// 
    /// </summary>
    public partial class PropertyGridPropertyCtrl : UserControl, IUserControl
    {
        /// <summary>
        /// 属性失去焦点事件
        /// </summary>
        public event EventHandler<EvaluationCtrlLostFocusEventArgs> PropertyLostFocus;
        List<PropertyInfoAttribute> item = new List<PropertyInfoAttribute>();
        private UIElement _ReadUIElement;
        private Dictionary<string, PropertyInfoAttribute> _DictControlProperty;
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
        public PropertyGridPropertyCtrl()
        {
            InitializeComponent();

            pgSearch.InputSearch += pgSearch_InputSearch;
            _DictControlProperty = new Dictionary<String, PropertyInfoAttribute>();
        }

        void _PropertyLostFocusMethod(object sender, EvaluationCtrlLostFocusEventArgs e)
        {
            if (null != PropertyLostFocus)
            {
                PropertyLostFocus(sender, e);
            }
        }
        /// <summary>
        /// 属性
        /// </summary>
        /// <param name="ui"></param>
        internal void ReadProperty(UIElement ui)
        {
            DisposePValue();
            if (null == ui)
            {
                spPValue.Children.Clear();
            }
            else
            {
                _ReadUIElement = ui;
                item = Wrapper.ReadControlAttribute<PropertyInfoAttribute>(ui);
                item.InsertRange(0, Wrapper.CommonPropertyItem(ui));
                foreach (var child in item)
                {
                    if (null == child || child.Name.IsNullOrEmpty() || _DictControlProperty.ContainsKey(child.Name))
                    {
                        continue;
                    }
                    _DictControlProperty.Add(child.Name, child);
                }
                AddPropertyToStackPanel(_DictControlProperty.Values.ToList());
            }
        }
        /// <summary>
        /// 更新属性
        /// </summary>
        /// <param name="e"></param>
        internal void UpdateProperty(ControlModifyPropertyEventArgs e)
        {
            if (null == item || 0 == item.Count)
            {
                return;
            }
            //-->更新属性操作
            foreach (var v in item)
            {
                if (null == v || v.Name.IsNullOrEmpty())
                {
                    continue;
                }
                if (e.DictProperty.ContainsKey(v.Name))
                {
                    object obj = spPValue.FindName("pgec_" + v.Name);
                    PropertyGridEvaluationCtrl pgec = obj as PropertyGridEvaluationCtrl;
                    if (null == pgec)
                    {
                        continue;
                    }
                    pgec.EvaluationValue = e.DictProperty[v.Name];
                }
            }
        }
        /// <summary>
        /// 读取控件属性
        /// </summary>
        /// <param name="ui"></param>
        /// <param name="iPageDesignFramework"></param>
        internal void ReadProperty(UIElement ui, IPageDesignFramework iPageDesignFramework)
        {
            this.IDesignFramework = iPageDesignFramework;
            ReadProperty(ui);
        }

        void AddPropertyToStackPanel(List<PropertyInfoAttribute> item)
        {
            double height = ConstantCollection.HEIGHT;
            spPValue.Children.Clear();
            foreach (var v in item)
            {
                if (null == v)
                {
                    continue;
                }
                var pgec = new PropertyGridEvaluationCtrl();
                pgec.Name = string.Format("pgec_{0}", v.Name);
                pgec.Height = height;
                pgec.ShowName = v.Description;
                pgec.DefaultValue = v.DefaultValue;
                pgec.AssemblyQualifiedName = v.AssemblyQualifiedName;
                pgec.DataType = v.DataType;
                pgec.MethodName = v.Name;
                pgec.PropertyName = v.Name;
                pgec.ParentControl = _ReadUIElement;
                pgec.IDesignFramework = this.IDesignFramework;
                pgec.LostFocus += pgec_LostFocus;
                pgec.ContentChange += pgec_ContentChange;
                pgec.CurrentSelectedControlName = this.CurrentSelectedControlName;
                try
                {
                    string dv = string.Format("{0}", v.DefaultValue);
                    if (!dv.IsNullOrEmpty())
                    {
                        pgec.EvaluationContent = new XElement(v.Name, dv);
                    }
                }
                catch { }
                spPValue.Children.Add(pgec);
            }
        }

        void pgec_ContentChange(object sender, EvaluationCtrlTextChangeEventArgs e)
        {
            var iuc = sender as PropertyGridEvaluationCtrl;
            XElement xe = iuc.EvaluationContent;
            string pName = xe.Name.ToString();
            PropertyInfo pi = _ReadUIElement.GetType().GetProperty(pName);
            string _PropertyName = iuc.PropertyName;
            if (null == pi || _PropertyName.IsNullOrEmpty())
            {
                pi = _ReadUIElement.GetType().GetProperty(_PropertyName);
                if (null == pi)
                {
                    return;
                }
            }
            var pt = pi.PropertyType;
            if (pt.IsEnum)
            {
                pi.SetValue(_ReadUIElement, Enum.Parse(pi.PropertyType, string.Format("{0}", xe.Value), true), null);
            }
            else
            {
                pi.SetValue(_ReadUIElement, Convert.ChangeType(xe.Value, pt, null), null);
            }
            var ecf = new EvaluationCtrlLostFocusEventArgs(sender, _PropertyName, xe.Value.ToString(), this.CurrentSelectedControlName);
            _PropertyLostFocusMethod(this, ecf);
        }

        private void pgSearch_InputSearch(object sender, InputSearchEventArgs e)
        {
            if (String.IsNullOrEmpty(e.InputValue))
            {
                AddPropertyToStackPanel(item);
            }
            else
            {
                var result = from p in item
                             where p.Name.ToUpper().StartsWith(e.InputValue.ToUpper())
                             select p;
                List<PropertyInfoAttribute> _item = result.ToList();
                AddPropertyToStackPanel(_item);
            }
        }

        void pgec_LostFocus(object sender, EvaluationCtrlLostFocusEventArgs e)
        {
            var iuc = sender as PropertyGridEvaluationCtrl;
            _ReadUIElement.SetPropertyValue(e.PropertyName, e.Value);
            _PropertyLostFocusMethod(this, e);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            try
            {
                DisposePValue();
                item.Clear();
                item = null;
            }
            catch { }
        }

        void DisposePValue()
        {
            foreach (var child in spPValue.Children)
            {
                var pgec = child as PropertyGridEvaluationCtrl;
                pgec.LostFocus -= pgec_LostFocus;
                pgec.ContentChange -= pgec_ContentChange;
                pgec.Dispose();
            }
            spPValue.Children.Clear();
            _DictControlProperty.Clear();
        }
        /// <summary>
        /// 赋值内容
        /// </summary>
        public XElement EvaluationContent { get { return GetPropertyValue(); } set { this.Tag = value; } }
        /// <summary>
        /// 属性值
        /// </summary>
        /// <returns></returns>
        XElement GetPropertyValue()
        {
            List<XElement> _item = new List<XElement>();
            foreach (var ui in spPValue.Children)
            {
                if (ui is IUserControl)
                {
                    XElement value = (ui as IUserControl).EvaluationContent;
                    _item.Add(value);
                }
            }
            XElement result = new XElement("PropertyItem", _item);
            return result;
        }
    }
}
