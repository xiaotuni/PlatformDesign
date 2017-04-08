using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlatformClient.Model.Method
{
    /// <summary>
    /// 变量信息类
    /// </summary>
    public class VariableInfo
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 值
        /// </summary>
        public string Value { get; set; }
    }
}
