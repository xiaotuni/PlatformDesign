using System;
using System.Windows;
using PlatformClient.Common.Interfaces;
using PlatformClient.Model.Method;

namespace PlatformClient.PageDesignTime.Lib
{
    /// <summary>
    /// 
    /// </summary>
    public class ResizeEventArgs : EventArgs
    {
        /// <summary>
        /// 
        /// </summary>
        public Point CurPoint { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public AuchorLocation DragDirection { get; set; }
    }
}
