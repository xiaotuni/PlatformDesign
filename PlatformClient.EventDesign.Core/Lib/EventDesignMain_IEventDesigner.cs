using System.Windows;
using System.Xml.Linq;
using PlatformClient.Common.Interfaces;
using PlatformClient.EventDesign.Core.Activitys;

namespace PlatformClient.EventDesign.Core
{
    /// <summary>
    /// 
    /// </summary>
    public partial class EventDesignMain : IEventDesigner
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
        /// 释放资源
        /// </summary>
        public override void Dispose()
        {
            this._txtActivityContent.LostFocus -= _txtActivityContent_LostFocus;
            this.xtnEvents.SelectionChanged -= xtnEvents_SelectionChanged;
            this.xtnEvents.OnDelete -= xtnEvents_OnDelete;
            this.xtnEvents.OnModify -= xtnEvents_OnModify;
            this.Loaded -= EventDesignMain_Loaded;
            this.xtnEvents.Dispose();
            this.ClearChilds();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool CheckSave()
        {
            return SaveXmlToDB();
        }
    }
}
