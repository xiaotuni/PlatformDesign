using System;
using System.Windows;
using System.Windows.Controls;
using PlatformClient.Common.Interfaces;
using PlatformClient.Model.Method;

namespace PlatformClient.CompositeCtrl.DataTypeCtrl
{
    /// <summary>
    /// 时间控件
    /// </summary>
    public partial class XtnCtrlPickerTime : XtnCtrlBaseType, ICompositeCtrl
    {
        TimePicker time = null;
        /// <summary>
        /// 文件框值
        /// </summary>
        [PropertyInfoAttribute("", "文件框值")]
        public new String TextValue
        {
            get { return time.Value.ToString(); }
            set
            {
                var dt = value;
                time.Value = Convert.ToDateTime(value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public XtnCtrlPickerTime()
        {
            time = new TimePicker();
            time.LostFocus += time_LostFocus;
            cControl.Content = time;
        }

        void time_LostFocus(object sender, RoutedEventArgs e)
        {
            this._NotifyRuntimePage(ConstantCollection.COMPOSITE_CONTROL_LOST_FOCUS );
        }
    }
}
