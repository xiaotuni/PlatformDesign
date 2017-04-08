using System.Collections.Generic;

namespace PlatformClient.Model.Method
{
    /// <summary>
    /// 
    /// </summary>
    public class ActivityRoot
    {
        /// <summary>
        /// 
        /// </summary>
        public List<ActivityConfig> ActivityItem { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<ApiSelector> ApiItem { get; set; }
    }
}
