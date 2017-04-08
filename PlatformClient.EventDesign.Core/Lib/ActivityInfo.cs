using System.Windows;

namespace PlatformClient.EventDesign.Core.Lib
{
    /// <summary>
    /// 
    /// </summary>
    public class ActivityInfo
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
        public double Width { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public double Height { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public double Left { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public double Top { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public double EnterX { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public double EnterY { get; set; }
        internal Point CurrentLeftButtonDownPoint { get { return new Point(Left, Top); } }
        internal Point CurrentEnterPoint { get { return new Point(EnterX, EnterY); } }
    }
}
