using System;
using System.Xml.Linq;

namespace PlatformClient.Model.Table
{
    /// <summary>
    /// 事件设计器设计出来的内容类
    /// </summary>
    public class EventDesignerContent
    {
        /// <summary>
        /// 主键
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// 事件名称
        /// </summary>
        public string EventName { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        public XElement EventContent { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0}、{1}-{2}", this.ID, this.EventName, this.Description);
        }
    }
}
