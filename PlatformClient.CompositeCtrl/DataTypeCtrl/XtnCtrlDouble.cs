using System;
using System.Windows;
using PlatformClient.Common.Interfaces;
using PlatformClient.Model.Method;
using PlatformClient.Utility;

namespace PlatformClient.CompositeCtrl.DataTypeCtrl
{
    /// <summary>
    /// 
    /// </summary>
    public partial class XtnCtrlDouble : XtnCtrlBaseType, ICompositeCtrl
    {
        /// <summary>
        /// 
        /// </summary>
        public XtnCtrlDouble()
        {
            this.txtValue.LostFocus += txtValue_LostFocus;
        }

        void txtValue_LostFocus(object sender, RoutedEventArgs e)
        {
            //-->判断输入的值，是否是Double类型
            double result = 0;
            bool isDouble = Double.TryParse(this.txtValue.Text.Trim(), out result);
            if (!isDouble)
            {
                Wrapper.ShowDialog("数据类型输入错误");
                return;
            }
            this._NotifyRuntimePage(ConstantCollection.COMPOSITE_CONTROL_LOST_FOCUS);
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Dispose()
        {
            this.txtValue.LostFocus -= txtValue_LostFocus;
            base.Dispose();
        }
    }
}
