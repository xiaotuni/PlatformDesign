using System;
using System.Collections.Generic;
using System.Windows;

namespace PlatformClient.Common.Interfaces
{
    /// <summary>
    /// Base控件接口
    /// </summary>
    public interface IBaseActivity : IImportExport
    {
        ///// <summary>
        ///// 所有箭头的坐标
        ///// </summary>
        //Dictionary<ArrowLine, ArrowLine> DictArrowCapPoint { get; set; }
        ///// <summary>
        ///// 所有箭尾的坐标
        ///// </summary>
        //Dictionary<ArrowLine, ArrowLine> DictArrowFootPoint { get; set; }

        /// <summary>
        /// 所有箭头的坐标
        /// </summary>
        Dictionary<String, IArrowLine> DictArrowCapPoint { get; set; }
        /// <summary>
        /// 所有箭尾的坐标
        /// </summary>
        Dictionary<String, IArrowLine> DictArrowFootPoint { get; set; }

        /// <summary>
        /// 当前左键单的坐标
        /// </summary>
        Point CurrentLeftButtonDownPoint { get; set; }
        /// <summary>
        /// 节点的标题
        /// </summary>
        String LabelContent { get; set; }
        /// <summary>
        /// 节点的GUID
        /// </summary>
        String ActivityGUID { get; set; }
        /// <summary>
        /// 删除线
        /// </summary>
        void RemoveLine(IArrowLine line);
        /// <summary>
        /// 更新所有箭头坐标
        /// </summary>
        /// <param name="point"></param>
        void UpdateArrowCapPoint(Point point);
        /// <summary>
        /// 更新所有箭尾坐标
        /// </summary>
        /// <param name="point"></param>
        void UpdateArrowFootPoint(Point point);

        /// <summary>
        /// 判断线是否存在了。
        /// </summary>
        /// <param name="iact"></param>
        /// <returns></returns>
        bool CheckedArrowIsExists(IActivity iact);
        /// <summary>
        /// 控件的与线之间的关系
        /// </summary>
        /// <returns></returns>
        string ExportControlRelationship();

        //void Set
    }
}
