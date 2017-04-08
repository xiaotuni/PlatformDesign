using System.Xml.Linq;
using System.Collections.Generic;
using System.Linq;

namespace PlatformClient.Model.Method
{
    /// <summary>
    /// 页面加载信息
    /// </summary>
    public class PageLoadingInfo
    {
        /// <summary>
        /// 控件名称
        /// </summary>
        public string ControlName { get; set; }
        /// <summary>
        /// 调用方法名称
        /// </summary>
        public string CallFunctionName { get; set; }
        /// <summary>
        /// 返回控件名称
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return ControlName;
        }
    }
}
