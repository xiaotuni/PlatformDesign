
namespace PlatformClient.Model.Method
{
    /// <summary>
    /// 
    /// </summary>
    public class ContextMenuInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public string Header { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ContextMenuType Type { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public object Source { get; set; }
        /// <summary>
        /// 程序集名称, 要弹出窗体，创建控件
        /// </summary>
        public string AssemblyQualifiedName { get; set; }
    }
}
