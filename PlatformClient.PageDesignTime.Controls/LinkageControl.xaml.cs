using System;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;
using PlatformClient.Common.Interfaces;
using PlatformClient.Extend.Core;

namespace PlatformClient.PageDesignTime.Controls
{
    /// <summary>
    /// 联动控件
    /// </summary>
    public partial class LinkageControl : UserControl, IUserControl, IPlatformClientChildWindow
    {
        XElement _EvaluationContent;
        bool _IsFullscreen = false;
        /// <summary>
        /// 
        /// </summary>
        public bool IsFullscreen { get { return _IsFullscreen; } }
        /// <summary>
        /// 赋值内容
        /// </summary>
        public XElement EvaluationContent { get { return _EvaluationContent; } set { _EvaluationContent = value; } }

        /// <summary>
        /// 设计时框架接口
        /// </summary>
        public IPageDesignFramework IDesignFramework { get; set; }

        /// <summary>
        /// 当前选中的控件
        /// </summary>
        public String CurrentSelectedControlName { get; set; }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            this.Loaded -= LinkageControl_Loaded;
            this.cControls.SelectedItemChanged -= cControls_SelectedItemChanged;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public LinkageControl()
        {
            InitializeComponent();
            this.Loaded += LinkageControl_Loaded;
            this.cControls.SelectedItemChanged += cControls_SelectedItemChanged;
        }

        void cControls_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var ci = this.cControls.SelectedControlInfo;
            if (null == ci)
            {
                return;
            }
            this.txtControlName.Text = ci.Name;
        }

        void LinkageControl_Loaded(object sender, RoutedEventArgs e)
        {
            cControls.ShowTree(this.IDesignFramework.GetCurrentXmlTemplate().ControlItem);
            var xamlTemplate = this.IDesignFramework.GetCurrentXamlTemplate();
            var controlXaml = xamlTemplate.CanvasTemplate.Children[this.CurrentSelectedControlName];

            string split = "_1_2_3_4_";
            controlXaml = controlXaml.Replace(":", split);
            XElement xaml = XElement.Parse(controlXaml);
            var value = xaml.GetAttributeValue("LinkageControl");
            if (value.IsNullOrEmpty())
            {
                return;
            }
            this.txtControlName.Text = value;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool CheckSave()
        {
            string value = string.Format("{0}", this.txtControlName.Text);
            _EvaluationContent = new XElement("property", new XAttribute("Name", value));
            return true;
        }
    }
}
