using System;
using System.Windows;
using System.Windows.Controls;
using PlatformClient.Common;
using PlatformClient.Common.Interfaces;
using PlatformClient.Model.Method;
using PlatformClient.Extend.Core;

namespace PlatformClient.CompositeCtrl.DataTypeCtrl
{
    /// <summary>
    /// 日期时间控件
    /// </summary>
    public partial class XtnCtrlPickerDateTime : XtnCtrlBaseType, ICompositeCtrl
    {
        DatePicker date = new DatePicker();
        Grid gRoot = null;
        TimePicker time = null;

        DateTimeType _Format = DateTimeType.To_YYYY_MM_DD_HH_MM_SS;
        /// <summary>
        /// 日期类型
        /// </summary>
        [PropertyInfoAttribute("", "日期类型")]
        public DateTimeType Format
        {
            get { return _Format; }
            set
            {
                _Format = value;
                UpdateCurrentFormatDate(date.DisplayDate.ToString());
            }
        }

        /// <summary>
        /// 文件框值
        /// </summary>
        [PropertyInfoAttribute("", "文件框值")]
        public new String TextValue
        {
            get
            {
                //string result = date.DisplayDate.ToString(ConstantCollection.DATE_FORMAT_YYYY_MM_DD_HH_MM_SS);
                //var _time = time == null ? DateTime.Now : Convert.ToDateTime(time.Value);
                //var _dd = date.DisplayDate;
                //var _tempDT = new DateTime(_dd.Year, _dd.Month, _dd.Day, _time.Hour, _time.Minute, _time.Second);

                //switch (_Format)
                //{
                //    case DateTimeType.To_HH_MM_SS:
                //        result = _tempDT.ToString(ConstantCollection.DATE_FORMAT_HH_MM_SS);
                //        break;
                //    case DateTimeType.To_YYYY:
                //        result = _tempDT.ToString(ConstantCollection.DATE_FORMAT_YYYY);
                //        break;
                //    case DateTimeType.To_YYYY_MM:
                //        result = _tempDT.ToString(ConstantCollection.DATE_FORMAT_YYYY_MM);
                //        break;
                //    case DateTimeType.Default:
                //    case DateTimeType.To_YYYY_MM_DD:
                //        result = _tempDT.ToString(ConstantCollection.DATE_FORMAT_YYYY_MM_DD);
                //        break;
                //    case DateTimeType.To_YYYY_MM_DD_HH: ;
                //        result = _tempDT.ToString(ConstantCollection.DATE_FORMAT_YYYY_MM_DD_HH);
                //        break;
                //    case DateTimeType.To_YYYY_MM_DD_HH_MM:
                //        result = _tempDT.ToString(ConstantCollection.DATE_FORMAT_YYYY_MM_DD_HH_MM);
                //        break;
                //    case DateTimeType.To_YYYY_MM_DD_HH_MM_SS:
                //        result = _tempDT.ToString(ConstantCollection.DATE_FORMAT_YYYY_MM_DD_HH_MM_SS);
                //        break;
                //    default:
                //        result = _tempDT.ToString(ConstantCollection.DATE_FORMAT_YYYY_MM_DD_HH_MM_SS);
                //        break;
                //}
                //return result;
                return date.DisplayDate.ToString(ConstantCollection.DATE_FORMAT_YYYY_MM_DD);
            }
            set
            {
                var dt = value;
                //UpdateCurrentFormatDate(dt);
                date.DisplayDate = Convert.ToDateTime(value);
            }
        }

        void UpdateCurrentFormatDate(string value)
        {
            var dt = Convert.ToDateTime(value);
            date.DisplayDate = dt;
            time.Value = dt;
            date.Visibility = System.Windows.Visibility.Visible;
            time.Visibility = System.Windows.Visibility.Visible;
            switch (_Format)
            {
                case DateTimeType.To_HH_MM_SS:
                    date.Visibility = System.Windows.Visibility.Collapsed;
                    time.Format = new CustomTimeFormat(ConstantCollection.DATE_FORMAT_HH_MM_SS);
                    break;
                case DateTimeType.To_YYYY:
                    time.Visibility = System.Windows.Visibility.Collapsed;
                    date.DisplayDate = dt.ToString(ConstantCollection.DATE_FORMAT_YYYY).ConvertTo<DateTime>();
                    break;
                case DateTimeType.To_YYYY_MM:
                    time.Visibility = System.Windows.Visibility.Collapsed;
                    date.DisplayDate = dt.ToString(ConstantCollection.DATE_FORMAT_YYYY_MM).ConvertTo<DateTime>();
                    break;
                case DateTimeType.Default:
                case DateTimeType.To_YYYY_MM_DD:
                    time.Visibility = System.Windows.Visibility.Collapsed;
                    date.DisplayDate = dt;
                    break;
                case DateTimeType.To_YYYY_MM_DD_HH: ;
                    time.Format = new CustomTimeFormat(ConstantCollection.DATE_FORMAT_HH);
                    break;
                case DateTimeType.To_YYYY_MM_DD_HH_MM:
                    time.Format = new CustomTimeFormat(ConstantCollection.DATE_FORMAT_HH_MM);
                    break;
                case DateTimeType.To_YYYY_MM_DD_HH_MM_SS:
                    time.Format = new CustomTimeFormat(ConstantCollection.DATE_FORMAT_HH_MM_SS);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public XtnCtrlPickerDateTime()
        {
            InitControl();

            this.LostFocus += ControlPickerDateTime_LostFocus;
        }

        void ControlPickerDateTime_LostFocus(object sender, RoutedEventArgs e)
        {
            this._NotifyRuntimePage("LostFocus");
        }

        void InitControl()
        {
            gRoot = new Grid();
            gRoot.ColumnDefinitions.Add(new ColumnDefinition());

            ColumnDefinition col = new ColumnDefinition();
            col.Width = new GridLength(0, GridUnitType.Auto);
            gRoot.ColumnDefinitions.Add(col);

            date = new DatePicker();
            Grid.SetColumn(date, 0);

            time = new TimePicker();
            time.Format = new CustomTimeFormat(ConstantCollection.DATE_FORMAT_HH_MM_SS);
            Grid.SetColumn(time, 1);

            gRoot.Children.Add(date);
            gRoot.Children.Add(time);

            cControl.Content = gRoot;
        }
    }
}
