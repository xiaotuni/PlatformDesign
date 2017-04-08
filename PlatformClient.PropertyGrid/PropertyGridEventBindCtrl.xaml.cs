using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;
using PlatformClient.Extend.Core;
using PlatformClient.Common;
using PlatformClient.Common.Interfaces;
using PlatformClient.Model.Method;
using PlatformClient.Utility;

namespace PlatformClient.PropertyGrid
{
    /// <summary>
    /// 
    /// </summary>
    public partial class PropertyGridEventBindCtrl : UserControl, IUserControl
    {
        XElement _EvaluationContent;
        /// <summary>
        /// 当前选中控件名称
        /// </summary>
        public string CurrentSelectedControlName { get; set; }
        /// <summary>
        /// 事件用的,用来对控件关连事件
        /// </summary>
        public string AssemblyQualifiedName { get; set; }
        /// <summary>
        /// 方法名称
        /// </summary>
        public string MethodName { get; set; }
        /// <summary>
        /// 控件
        /// </summary>
        public UIElement ParentControl { get; set; }
        /// <summary>
        /// 设计时接口
        /// </summary>
        public IPageDesignFramework IDesignFramework { get; set; }
        /// <summary>
        /// 事件属性信息
        /// </summary>
        public EventInfoAttribute AttributeInfo { get; set; }
        /// <summary>
        /// 显示的名称 
        /// </summary>
        public string ShowName { get { return this.txtTitle.Text; } set { this.txtTitle.Text = value; } }

        /// <summary>
        /// 
        /// </summary>
        FrameworkElement _EventDesigner;
        /// <summary>
        /// 赋值内容
        /// </summary>
        public XElement EvaluationContent { get { return _EvaluationContent; } set { _EvaluationContent = value; } }

        /// <summary>
        /// 构造函数
        /// </summary>
        public PropertyGridEventBindCtrl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 加载信息
        /// </summary>
        /// <param name="events"></param>
        /// <param name="_messageTip"></param>
        public void LoadInitValue(string events, string _messageTip)
        {
            this.txtValue.Text = events;
            ToolTipService.SetToolTip(this, _messageTip);
            ToolTipService.SetToolTip(this.btnOK, _messageTip);
            ToolTipService.SetToolTip(this.txtTitle, _messageTip);
            ToolTipService.SetToolTip(this.txtValue, _messageTip);
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            _EvaluationContent = null;
            AssemblyQualifiedName = string.Empty;
            MethodName = string.Empty;
            ShowName = string.Empty;
            ParentControl = null;
            IDesignFramework = null;
            AttributeInfo = null;
            _EventDesigner = null;
            EvaluationContent = null;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //-->弹出页面
            _EventDesigner = Wrapper.CreateControl<FrameworkElement>(AttributeInfo.EventDesigner);
            if (null == _EventDesigner)
            {
                string msg = string.Format("请查看【{0}】复合控件\r\n的【{1}】属性事件配置是否正确", this.ParentControl.GetType().FullName, AttributeInfo.Name);
                Wrapper.ShowDialog(msg);
                return;
            }
            string title = "";
            if (_EventDesigner is IEventDesigner)
            {
                var ed = (_EventDesigner as IEventDesigner);
                ed.CurrentSelectedControl = ParentControl as FrameworkElement;
                ed.IDesignFramework = IDesignFramework;
                ed.EventName = MethodName;
                ed.EventControlName = ed.CurrentSelectedControl.Name;
                ed.AttributeInfo = this.AttributeInfo;
                title = string.Format("现在对【{0}】控件的【{1}】事件设置", ed.EventControlName, ed.EventName);
            }
            XtnChildWindow child = new XtnChildWindow(this.IDesignFramework, _EventDesigner);
            child.Closed += child_Closed;
            child.SetTitle(title);
            child.Show();
        }

        void child_Closed(object sender, System.EventArgs e)
        {
            //-->如果是事件的话，得注册事件
            XtnChildWindow child = sender as XtnChildWindow;
            if (null != child)
            {
                child.Closed -= child_Closed;
                child.Dispose();
            }
            GetTextBoxValue();
        }

        void GetTextBoxValue()
        {
            var _xml = IDesignFramework.GetCurrentXmlTemplate();
            var eli = _xml.EventLinkItem.Where(
                               p => p.ControlName.Equals((ParentControl as FrameworkElement).Name) &&
                               p.EventName.Equals(MethodName)).GetFirst<EventLinkInfo>();
            if (null != eli && 0 < eli.Item.Count)
            {
                var _eventItem = from p in eli.Item
                                 select p.Description.IsNullOrEmpty() ? p.EventName : p.Description;
                string _value = string.Join(",", _eventItem);
                string _messageTip = string.Format("[{0}]调用[{1}]方法;{2}", MethodName, _eventItem.Count(), _value);

                LoadInitValue(_value, _messageTip);
            }
        }
    }
}
