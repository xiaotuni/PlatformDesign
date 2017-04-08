using System.Windows;
using PlatformClient.Common.Interfaces;

namespace PlatformClient.CompositeCtrl.DataTypeCtrl
{
    /// <summary>
    /// 
    /// </summary>
    public class XtnCtrlVarchar : XtnCtrlBaseType, ICompositeCtrl
    {
        /// <summary>
        /// 
        /// </summary>
        public XtnCtrlVarchar()
        {
            this.txtValue.LostFocus += txtValue_LostFocus;
        }

        void txtValue_LostFocus(object sender, RoutedEventArgs e)
        {
            this._NotifyRuntimePage("LostFocus");
        }
    }
}
