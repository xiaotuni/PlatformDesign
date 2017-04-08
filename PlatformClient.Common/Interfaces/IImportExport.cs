using System;

namespace PlatformClient.Common.Interfaces
{
    /// <summary>
    /// 导入/出接口
    /// </summary>
    public interface IImportExport : IDisposable
    {
        /// <summary>
        /// 导出位置
        /// </summary>
        /// <returns></returns>
        string ExportLocation();
    }
}
