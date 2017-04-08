using System.Windows;
using PlatformClient.Common.Interfaces;
using PlatformClient.Model.Method;

namespace PlatformClient.CompositeCtrl.DataTypeCtrl
{
    /// <summary>
    /// 
    /// </summary>
    public class XtnCtrlBinary : XtnCtrlBaseType, ICompositeCtrl
    {
        /// <summary>
        /// 
        /// </summary>
        public XtnCtrlBinary()
        {
            this.txtValue.LostFocus += txtValue_LostFocus;
        }

        void txtValue_LostFocus(object sender, RoutedEventArgs e)
        {
            //-->判断输入的值，是否是Double类型
            this._NotifyRuntimePage(ConstantCollection.COMPOSITE_CONTROL_LOST_FOCUS);
        }
    }
}
