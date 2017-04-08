using System.Windows;

namespace PlatformClient.EventDesign.Core.Lib
{
    /// <summary>
    /// 
    /// </summary>
    public class ArrowLineInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Guid { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public double StartX { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public double StartY { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public double EndX { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public double EndY { get; set; }
        /// <summary>
        /// 
        /// </summary>
        internal Point StartPoint { get { return new Point(StartX, StartY); } }
        /// <summary>
        /// 
        /// </summary>
        internal Point EndPoint { get { return new Point(EndX, EndY); } }
    }
}
