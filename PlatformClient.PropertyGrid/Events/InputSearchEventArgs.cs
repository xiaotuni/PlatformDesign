using System;

namespace PlatformClient.PropertyGrid.Events
{
    /// <summary>
    /// 
    /// </summary>
    internal class InputSearchEventArgs : EventArgs
    {
        internal object Source { get; set; }
        internal string InputValue { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="inputValue"></param>
        public InputSearchEventArgs(object source, string inputValue)
        {
            this.Source = source;
            this.InputValue = inputValue;
        }
    }
}
