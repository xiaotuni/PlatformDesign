using System;

namespace PlatformClient.Model.Table
{
    /// <summary>
    /// 目录下节点结构
    /// </summary>
    public class PageDirectorySub
    {
        /// <summary>
        /// ID号
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// 节点所在目录ID
        /// </summary>
        public int PageDirectoryId { get; set; }
        /// <summary>
        /// 页面的GUID
        /// </summary>
        public string PageGuid { get; set; }
        /// <summary>
        /// 页面名称
        /// </summary>
        public string PageName { get; set; }
        /// <summary>
        /// 页面内容
        /// </summary>
        public string PageContent { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }
    }
}
