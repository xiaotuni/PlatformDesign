
namespace PlatformClient.Model.Method
{
    /// <summary>
    /// 事件名称
    /// </summary>
    public class EventInfoAttribute : BaseAttribute
    {
        /// <summary>
        /// 事件设计器
        /// </summary>
        public string EventDesigner { get; set; }
        /// <summary>
        /// 构造函数
        /// </summary>
        public EventInfoAttribute() : this(null, null, null) { }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="assemblyQualifiedName">反射用的程序集名称</param>
        /// <param name="description">描述</param>
        public EventInfoAttribute(string name, string assemblyQualifiedName, string description)
            : this(name, assemblyQualifiedName, description, null)
        {
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="assemblyQualifiedName">反射用的程序集名称</param>
        /// <param name="description">描述</param>
        /// <param name="eventDesigner">事件设计器</param>
        public EventInfoAttribute(string name, string assemblyQualifiedName, string description,
            string eventDesigner)
        {
            this.Name = name;
            this.AssemblyQualifiedName = assemblyQualifiedName;
            this.Description = description;
            this.EventDesigner = eventDesigner;
        }

    }
}
