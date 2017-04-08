using System;
using System.Windows.Controls;
using PlatformClient.Common.Lib;
using PlatformClient.Model.Method;

namespace PlatformClient.Selectors.Core
{
    /// <summary>
    /// 
    /// </summary>
    public class ListBoxItemExtend : ListBoxItem, IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        public ControlInfo ControlInfo { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            this.Tag = null;
            ControlInfo = null;
        }
    }
}
