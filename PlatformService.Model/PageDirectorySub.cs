using System;

namespace PlatformService.Model
{
    /// <summary>
    /// 
    /// </summary>
    public class PageDirectorySub
    {
        /// <summary>
        /// 
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int PageDirectoryId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PageGuid { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PageName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PageContent { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime CreateDate { get; set; }
    }
}
