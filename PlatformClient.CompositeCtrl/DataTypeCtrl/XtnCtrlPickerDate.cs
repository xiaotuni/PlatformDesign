using System;
using System.Windows.Controls;
using PlatformClient.Common.Interfaces;
using PlatformClient.Model.Method;

namespace PlatformClient.CompositeCtrl.DataTypeCtrl
{
    /// <summary>
    /// 日期控件
    /// </summary>
    public partial class XtnCtrlPickerDate : XtnCtrlBaseType, ICompositeCtrl
    {
        DatePicker date = new DatePicker();
        /// <summary>
        /// 文件框值
        /// </summary>
        [PropertyInfoAttribute("", "文件框值")]
        public new String TextValue
        {
            get
            {
                return date.DisplayDate.ToString(ConstantCollection.DATE_FORMAT_YYYY_MM_DD);
            }
            set
            {
                if (String.IsNullOrEmpty(value))
                {
                    return;
                }
                var dt = value;
                date.DisplayDate = Convert.ToDateTime(value);
                date.SelectedDate = date.DisplayDate;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public XtnCtrlPickerDate()
        {
            date.SelectedDateChanged += date_SelectedDateChanged;
            date.LostFocus += date_LostFocus;
            cControl.Content = date;
            date.SelectedDate = DateTime.Now;
            GotFocus += XtnCtrlPickerDate_GotFocus;
        }

        void XtnCtrlPickerDate_GotFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            date.Focus();
        }

        void date_LostFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            this._NotifyRuntimePage("LostFocus");
        }

        void date_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            date.DisplayDate = (DateTime)date.SelectedDate;
            this._NotifyRuntimePage("SelectedDateChanged");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmd"></param>
        public override void InputParameter(CtrlPlatformCommandInfo cmd)
        {
            var mdci = cmd.MetaDataInfo.Item[0];
            if (null == mdci)
            {
                return;
            }
            TextValue = string.Format("{0}", mdci.column_value);
            this._NotifyRuntimePage(ConstantCollection.COMPOSITE_CONTROL_INPUT_PARAMETER);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override CtrlPlatformCommandInfo OutputParameter()
        {
            var opt = base.OutputParameter();
            opt.MetaDataInfo.Item[0].column_value = date.DisplayDate.ToString(ConstantCollection.DATE_FORMAT_YYYY_MM_DD_HH_MM_SS);
            return opt;
        }
        /// <summary>
        /// 清空控件内容
        /// </summary>
        public override void ClearCtrlValue()
        {
            base.ClearCtrlValue();
            date.SelectedDate = DateTime.Now;
        }
        /// <summary>
        /// 释放资源
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();
            date.SelectedDateChanged -= date_SelectedDateChanged;
            date.LostFocus -= date_LostFocus;
            GotFocus -= XtnCtrlPickerDate_GotFocus;
        }
    }
}
