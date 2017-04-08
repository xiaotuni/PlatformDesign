using System;
using PlatformClient.Model;

namespace PlatformClient.Common.Interfaces
{
    /// <summary>
    /// 运行时接口
    /// </summary>
    public interface IRuntimePage : IDisposable
    {
        /// <summary>
        /// 页面信息
        /// </summary>
        PageDirectorySub PageInfo { get; set; }
    }
}
