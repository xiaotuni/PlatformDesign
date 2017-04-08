using System;
using System.Collections.Generic;

namespace PlatformClient.Model.Method
{
    /// <summary>
    /// 
    /// </summary>
    public class CtrlFuctionTypeInfo
    {
        /// <summary>
        /// 类型:Button、Label、TextBoxEx、and so on
        /// </summary>
        public String Type { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public MetaDataInfo Init { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<CtrlFuctionInfo> Functions { get; set; }
    }
}
