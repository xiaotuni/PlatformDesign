using System;
using System.Collections.Generic;

namespace PlatformClient.Model.Method
{
    /// <summary>
    /// 控件参数信息类
    /// </summary>
    public class CtrlFuctionInfo
    {
        /// <summary>
        /// 方法名称
        /// </summary>
        public String Name { get; set; }
        /// <summary>
        /// 方法描述
        /// </summary>
        public String Description { get; set; }
        /// <summary>
        /// 方法参数
        /// </summary>
        public List<ParamInfo> Item { get; set; }
    }
}
