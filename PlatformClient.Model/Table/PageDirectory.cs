using System;

namespace PlatformClient.Model.Table
{
    /// <summary>
    /// 目录结构
    /// </summary>
    public class PageDirectory
    {
        /// <summary>
        /// 序号
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// 目录父节点ID
        /// </summary>
        public int ParentID { get; set; }
        /// <summary>
        /// 目录节点名称
        /// </summary>
        public string NodeName { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }
        /// <summary>
        /// true-说明此节点已经从服务器上把数据取回来了，不用再发命令了。
        /// </summary>
        public bool IsSendCommand { get; set; }
    }
}
