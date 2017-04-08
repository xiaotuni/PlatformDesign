using System;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;
using PlatformClient.Common;
using PlatformClient.Common.Interfaces;
using PlatformClient.Extend.Core;

namespace PlatformClient.PropertyGrid
{
    /// <summary>
    /// 
    /// </summary>
    public partial class PropertyGridDialogCtrl : UserControl, IUserControl
    {
        /// <summary>
        /// 关闭操作
        /// </summary>
        public event EventHandler Close;

        FrameworkElement _UIElementProperty;

        XElement _EvaluationContent;

        /// <summary>
        /// 当前选中控件名称
        /// </summary>
        public string CurrentSelectedControlName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string AssemblyQualifiedName { get; set; }

        /// <summary>
        /// 设计时框架接口
        /// </summary>
        public IPageDesignFramework IDesignFramework { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public FrameworkElement UIElementProperty
        {
            get { return _UIElementProperty; }
            set
            {
                _UIElementProperty = value;
                if (UIElementProperty is IUserControl)
                {
                    (UIElementProperty as IUserControl).IDesignFramework = this.IDesignFramework;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public XElement EvaluationContent
        {
            get
            {
                if (null == _EvaluationContent && UIElementProperty is IUserControl)
                {
                    _EvaluationContent = (UIElementProperty as IUserControl).EvaluationContent;
                    txtValue.Text = string.Format("{0}", _EvaluationContent == null ? "" : _EvaluationContent.Value);
                }
                return _EvaluationContent;
            }
            set
            {
                _EvaluationContent = value;

                this.txtValue.Text = string.Format("{0}", value == null ? "" : value.Value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public PropertyGridDialogCtrl()
        {
            InitializeComponent();
        }

        void _CloseMethod(object sender, EventArgs e)
        {
            if (null == Close)
            {
                return;
            }
            Close(sender, e);
        }

        void Button_Click(object sender, RoutedEventArgs e)
        {
            if (UIElementProperty is IUserControl)
            {
                var iuc = UIElementProperty as IUserControl;
                iuc.CurrentSelectedControlName = this.CurrentSelectedControlName;
                iuc.EvaluationContent = this.EvaluationContent;
            }
            var child = new XtnChildWindow(IDesignFramework, UIElementProperty);
            child.CurrentSelectedControlName = this.CurrentSelectedControlName;
            child.EvaluationContent = this.EvaluationContent;
            child.Closed += child_Closed;
            child.SetTitle(string.Format("正在对【{0}】控件进行操作...", this.CurrentSelectedControlName));
            child.Show();
        }

        void child_Closed(object sender, EventArgs e)
        {
            var child = sender as XtnChildWindow;
            if (null == child)
            {
                return;
            }
            child.Closed -= child_Closed;
            if (false == child.DialogResult)
            {
                child.Dispose();
                return;
            }
            child.Dispose();
            _EvaluationContent = child.EvaluationContent;
            txtValue.Text = string.Format("{0}", _EvaluationContent != null ? _EvaluationContent.Value : "");
            _CloseMethod(this, e);
        }

        void CheckTextValue()
        {
            var ui = UIElementProperty as IUserControl;
            if (null == ui)
            {
                return;
            }
            _EvaluationContent = ui.EvaluationContent;
            txtValue.Text = string.Format("{0}", _EvaluationContent.Value);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            if (UIElementProperty is IDisposable)
            {
                (UIElementProperty as IDisposable).Dispose();
            }
            UIElementProperty = null;
            EvaluationContent = null;
            AssemblyQualifiedName = string.Empty;
        }
    }
}
