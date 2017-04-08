using System;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;
using PlatformClient.Common;
using PlatformClient.Common.Interfaces;
using PlatformClient.PropertyGrid.Events;
using PlatformClient.Utility.Events;
using PlatformClient.Extend.Core;

namespace PlatformClient.PropertyGrid
{
    /// <summary>
    /// 
    /// </summary>
    public partial class PropertyGridCtrl : UserControl, IUserControl
    {
        /// <summary>
        /// 属性失去焦点事件
        /// </summary>
        public event EventHandler<EvaluationCtrlLostFocusEventArgs> PropertyLostFocus;
        UIElement _ReadAttributeCtrl;
        IPageDesignFramework _IDesignFramework;
        /// <summary>
        /// 当前选中控件名称
        /// </summary>
        public string CurrentSelectedControlName { get; set; }
        /// <summary>
        /// 设计时框架接口
        /// </summary>
        public IPageDesignFramework IDesignFramework
        {
            get { return _IDesignFramework; }
            set
            {
                _IDesignFramework = value;
                pgFunction.IDesignFramework = value;
                pgProperty.IDesignFramework = value;
                pgEvent.IDesignFramework = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public PropertyGridCtrl()
        {
            InitializeComponent();
            pgProperty.PropertyLostFocus += pgProperty_PropertyLostFocus;
            txtControlName.LostFocus += txtControlName_LostFocus;
        }

        private void txtControlName_LostFocus(object sender, RoutedEventArgs e)
        {
            var ce = new EvaluationCtrlLostFocusEventArgs();
            ce.ControlName = _ReadAttributeCtrl.GetPropertyValue("Name").ToString();
            ce.Source = sender;
            ce.PropertyName = "Name";
            ce.Value = this.txtControlName.Text.Trim();
            _PropertyLostFocusMethod(this, ce);
        }

        void pgProperty_PropertyLostFocus(object sender, EvaluationCtrlLostFocusEventArgs e)
        {
            _PropertyLostFocusMethod(this, e);
        }

        void _PropertyLostFocusMethod(object sender, EvaluationCtrlLostFocusEventArgs e)
        {
            if (null != PropertyLostFocus)
            {
                PropertyLostFocus(sender, e);
            }
        }
        /// <summary>
        /// 读取属性
        /// </summary>
        /// <param name="ui"></param>
        public void ReadAttribute(UIElement ui)
        {
            _ReadAttributeCtrl = ui;
            string controlName = string.Empty;
            string controlType = string.Empty;
            if (null != ui)
            {
                Type tt = ui.GetType();
                controlName = string.Format("{0}", tt.GetProperty("Name").GetValue(ui, null));
                controlType = tt.Name;
            }
            this.lblControlType.Content = controlType;
            this.txtControlName.Text = controlName;
            //-->事件
            pgEvent.CurrentSelectedControlName = controlName;
            pgEvent.ReadEvent(ui);
            //-->属性
            pgProperty.CurrentSelectedControlName = controlName;
            pgProperty.ReadProperty(ui);
            //-->方法
            pgFunction.CurrentSelectedControlName = controlName;
            pgFunction.ReadFunctions(ui);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            pgEvent.Dispose();
            pgProperty.Dispose();
            pgFunction.Dispose();
        }
        /// <summary>
        /// 赋值内容
        /// </summary>
        public XElement EvaluationContent
        {
            get
            {
                return GetPropertyGridValue();
            }
            set
            {
                this.Tag = value;
            }
        }

        XElement GetPropertyGridValue()
        {
            var iuc = pgProperty as IUserControl;
            var _PropertyValue = iuc.EvaluationContent;
            return _PropertyValue;
        }
        /// <summary>
        /// 更新属性
        /// </summary>
        /// <param name="e"></param>
        public void UpdateProperty(ControlModifyPropertyEventArgs e)
        {
            pgProperty.UpdateProperty(e);
        }
    }
}
