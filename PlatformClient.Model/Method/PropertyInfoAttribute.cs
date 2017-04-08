using System;

namespace PlatformClient.Model.Method
{
    /// <summary>
    /// 属性信息
    /// </summary>
    public class PropertyInfoAttribute : BaseAttribute
    {
        /// <summary>
        /// 数据类型
        /// </summary>
        public Type DataType { get; set; }
        /// <summary>
        /// 构造函数
        /// </summary>
        public PropertyInfoAttribute() : this(null, null) { }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="assemblyQualifiedName">反射用的程序集名称</param>
        /// <param name="description">描述</param>
        public PropertyInfoAttribute(string assemblyQualifiedName, string description)
        {
            this.AssemblyQualifiedName = assemblyQualifiedName;
            this.Description = description;
        }
    }
}
