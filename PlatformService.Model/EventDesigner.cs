using System.Xml.Linq;

namespace PlatformService.Model
{
    /// <summary>
    /// 
    /// </summary>
    public class EventDesigner
    {
        /// <summary>
        /// 外键：事件内容ID
        /// </summary>
        public int ContentID { get; set; }
        /// <summary>
        /// 页面的GUID
        /// </summary>
        public string PageGuid { get; set; }
        /// <summary>
        /// 控件名称
        /// </summary>
        public string ControlName { get; set; }
        /// <summary>
        /// 事件名称
        /// </summary>
        public string EventName { get; set; }
        ///// <summary>
        ///// 内容
        ///// </summary>
        //public XElement EventDesignerContent { get; set; }

    }
}