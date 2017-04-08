using System.Windows;

namespace PlatformClient.Common.Interfaces
{
    /// <summary>
    /// 箭头线接口
    /// </summary>
    public interface IArrowLine : IImportExport
    {
        /// <summary>
        /// 删除线
        /// </summary>
        event RoutedEventHandler DeleteLine;
        /// <summary>
        /// 修改线的方向
        /// </summary>
        event RoutedEventHandler ResetDirection;
        /// <summary>
        /// 线的GUID
        /// </summary>
        string LineGuid { get; set; }
        /// <summary>
        /// 线尾所在的控件
        /// </summary>
        UIElement ArrowFootControl { get; set; }
        /// <summary>
        /// 线头所在的控件
        /// </summary>
        UIElement ArrowCapControl { get; set; }
        /// <summary>
        /// 开始坐标
        /// </summary>
        Point StartPoint { get; set; }
        /// <summary>
        /// 结束坐标
        /// </summary>
        Point EndPoint { get; set; }
        /// <summary>
        /// 控件名称
        /// </summary>
        string CtrName { get; set; }
    }
}
