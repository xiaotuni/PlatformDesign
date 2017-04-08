using System.Windows.Controls;
using System.Xml.Linq;
using PlatformClient.Common.Interfaces;

namespace PlatformClient.CompositeCtrl
{
    /// <summary>
    /// 
    /// </summary>
    public partial class TestShowDialog : UserControl, IUserControl
    {
        /// <summary>
        /// 当前选中控件名称
        /// </summary>
        public string CurrentSelectedControlName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public TestShowDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 设计时框架接口
        /// </summary>
        public IPageDesignFramework IDesignFramework { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public XElement EvaluationContent
        {
            get
            {
                XElement xe = new XElement("Name");
                xe.Add("this.txtContent.Text");
                return xe;
            }
            set
            {
                this.Tag = value;
                this.txtContent.Text = value.ToString();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
         
        }
    }
}
