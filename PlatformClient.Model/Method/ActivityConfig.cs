
namespace PlatformClient.Model.Method
{
    /// <summary>
    /// 行为配置文件类
    /// </summary>
    public class ActivityConfig
    {
        /// <summary>
        /// 行为控件名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string AssemblyQualifiedName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Description;
        }
    }
}
