using System;

namespace PlatformClient.PropertyGrid.Events
{
    /// <summary>
    /// 
    /// </summary>
    public class EvaluationCtrlTextChangeEventArgs : EventArgs
    {
        /// <summary>
        /// 
        /// </summary>
        internal Object Source { get; set; }
        /// <summary>
        /// 
        /// </summary>
        internal String Text { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public EvaluationCtrlTextChangeEventArgs(Object source, string text)
        {
            this.Source = source;
            this.Text = text;
        }
    }
}
