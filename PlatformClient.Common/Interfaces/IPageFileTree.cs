using System;
using PlatformClient.Common.Events;
using PlatformClient.Model.Events;

namespace PlatformClient.Common.Interfaces
{
    /// <summary>
    /// 页面资源文件树接口
    /// </summary>
    public interface IPageFileTree:IDisposable
    {
        /// <summary>
        /// 打开资源文件事件
        /// </summary>
        event EventHandler<OpenPageInfoEventArgs> OpenPageInfo;
    }
}
