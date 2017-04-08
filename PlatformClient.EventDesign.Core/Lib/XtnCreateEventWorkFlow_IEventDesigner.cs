using System.Windows;
using System.Xml.Linq;
using PlatformClient.Common.Interfaces;

namespace PlatformClient.EventDesign.Core
{
    public partial class XtnCreateEventWorkFlow : IEventDesigner
    {
        /// <summary>
        /// 控件名称 事件名称 调用控件的名称，调用控件的事件名称，传入的参数
        /// </summary>
        public XElement EvaluationContent { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public IPageDesignFramework IDesignFramework { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public FrameworkElement CurrentSelectedControl { get; set; }

        bool _IsFullscreen = true;
        /// <summary>
        /// 是否全屏
        /// </summary>
        public bool IsFullscreen { get { return _IsFullscreen; } }

        /// <summary>
        /// 控件名称
        /// </summary>
        public string EventControlName { get; set; }

        /// <summary>
        /// 事件名称
        /// </summary>
        public string EventName { get; set; }
        /// <summary>
        /// 当前选中控件的名称
        /// </summary>
        public string CurrentSelectedControlName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public override void Dispose()
        {
            btnClear_Click(null, null);

            this.MouseRightButtonDown -= EventDesignMain_MouseRightButtonDown;

            this.print.MouseLeftButtonDown -= print_MouseLeftButtonDown;
            this.print.MouseLeftButtonUp -= print_MouseLeftButtonUp;
            this.print.MouseMove -= print_MouseMove;
            this.Loaded -= EventDesignMain_Loaded;

            this.btnClear.Click -= btnClear_Click;
            this.btnSave.Click -= btnSave_Click;
            this.btnRevoke.Click -= btnRevoke_Click;
            this.btnExportXml.Click -= btnExportXml_Click;
            this.btnInportXml.Click -= btnImportXml_Click;

            this.s_ChangeWidth.ValueChanged -= s_ChangeWidth_ValueChanged;
            this.s_ChangeHeight.ValueChanged -= s_ChangeHeight_ValueChanged;
            this._Cm.MouseLeave -= _Cm_MouseLeave;
            this._txtActivityContent.LostFocus -= _txtActivityContent_LostFocus;
        }
    }
}
