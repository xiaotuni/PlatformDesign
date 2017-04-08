using System;

namespace PlatformClient.Common.Interfaces
{
    /// <summary>
    /// 平台弹出窗体接口
    /// </summary>
    public interface IPlatformClientChildWindow : IDisposable
    {
        /// <summary>
        /// 是否全屏
        /// </summary>
        bool IsFullscreen { get; }
        /// <summary>
        /// 检查是否保存
        /// </summary>
        bool CheckSave();
    }
}
