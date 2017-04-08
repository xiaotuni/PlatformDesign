using System;

namespace PlatformClient.Model.Method
{
    /// <summary>
    /// 全局变量信息
    /// </summary>
    public class GlobalVariableInfo
    {
        /// <summary>
        /// 变量值
        /// </summary>
        public string VariableValue { get; set; }
        /// <summary>
        /// 变量名称
        /// </summary>
        public string VariableName { get; set; }
        /// <summary>
        /// 变量类型
        /// </summary>
        public Type VariableType { get; set; }
        /// <summary>
        /// 变量描述
        /// </summary>
        public string Description { get; set; }
    }
}
