using System;

namespace PlatformService.Model
{
    /// <summary>
    /// 
    /// </summary>
    public class PageDirectory
    {
        /// <summary>
        /// 
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int ParentID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string NodeName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime CreateDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool IsSendCommand { get; set; }
    }
}
