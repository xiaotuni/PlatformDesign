
namespace PlatformClient.Model.Method
{
    /// <summary>
    /// 控件绑定事件
    /// </summary>
    public class CtrlBindEventInfo
    {
        /// <summary>
        /// 事件名称
        /// </summary>
        public string EventName { get; set; }
        /// <summary>
        /// 绑定事件名称
        /// </summary>
        public string BindFunctionName { get; set; }
        /// <summary>
        /// 绑定所在的程序集名称【程序集名；表的名称空间】
        /// </summary>
        public string AssemblyName { get; set; }
    }
}
