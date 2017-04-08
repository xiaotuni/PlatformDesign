
namespace PlatformClient.PropertyGrid.Lib
{
    /// <summary>
    /// 枚举描述类
    /// </summary>
    internal class EnumsTypeDescription
    {
        /// <summary>
        /// 序号
        /// </summary>
        internal int Index { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        internal string Name { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        internal string Description { get; set; }
        /// <summary>
        /// 枚举描述类构造函数
        /// </summary>
        internal EnumsTypeDescription() : this(0, null, null) { }
        /// <summary>
        /// 枚举描述类构造函数
        /// </summary>
        /// <param name="index">序号</param>
        /// <param name="name">名称</param>
        /// <param name="description">描述</param>
        internal EnumsTypeDescription(int index, string name, string description)
        {
            this.Index = index;
            this.Name = name;
            this.Description = description;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Name;
        }
    }
}
