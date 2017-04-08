using System;
using PlatformClient.Model.Table;

namespace PlatformClient.Model.Events
{
    /// <summary>
    /// 打开新页面事件类
    /// </summary>
    public class OpenPageInfoEventArgs : EventArgs
    {
        /// <summary>
        /// 页面信息类
        /// </summary>
        public PageDirectorySub PageDirectorySub { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public OpenPageInfoEventArgs()
            : this(null)
        {
            this.PageDirectorySub = null;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        public OpenPageInfoEventArgs(PageDirectorySub node)
        {
            this.PageDirectorySub = node;
        }
    }
}
