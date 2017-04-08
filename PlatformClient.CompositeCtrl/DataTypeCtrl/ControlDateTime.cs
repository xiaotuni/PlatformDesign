using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using PlatformClient.Model.Method;
using PlatformClient.Common.Interfaces;
using PlatformClient.Utility;
using PlatformClient.Common;
using System.Globalization;

namespace PlatformClient.CompositeCtrl.DataTypeCtrl
{
    public enum DateTimeType
    {
        /// <summary>
        /// yyyy-MM-dd HH:mm:ss
        /// </summary>
        Default,
        To_YYYY_MM_DD,
        To_HH_MM_SS,
        To_YYYY,
        To_YYYY_MM,
        To_YYYY_MM_DD_HH,
        To_YYYY_MM_DD_HH_MM,
        To_YYYY_MM_DD_HH_MM_SS,
    }

    public partial class ControlDateTime : ControlBaseType, ICompositeCtrl
    {
        DatePicker dp = new DatePicker();

        [PropertyInfoAttribute("", "日期类型")]
        public DateTimeType Format { get; set; }

        /// <summary>
        /// 文件框值
        /// </summary>
        [PropertyInfoAttribute("", "文件框值")]
        public new String TextValue
        {
            get
            {
                string result = dp.DisplayDate.ToString(ConstantCollection.DATE_FORMAT_YYYY_MM_DD_HH_MM_SS);
                switch (Format)
                {
                    case DateTimeType.To_HH_MM_SS:
                        result = dp.DisplayDate.ToString(ConstantCollection.DATE_FORMAT_HH_MM_SS);
                        break;
                    case DateTimeType.To_YYYY:
                        result = dp.DisplayDate.ToString(ConstantCollection.DATE_FORMAT_YYYY);
                        break;
                    case DateTimeType.To_YYYY_MM:
                        result = dp.DisplayDate.ToString(ConstantCollection.DATE_FORMAT_YYYY_MM);
                        break;
                    case DateTimeType.Default:
                    case DateTimeType.To_YYYY_MM_DD:
                        result = dp.DisplayDate.ToString(ConstantCollection.DATE_FORMAT_YYYY_MM_DD);
                        break;
                    case DateTimeType.To_YYYY_MM_DD_HH: ;
                        result = dp.DisplayDate.ToString(ConstantCollection.DATE_FORMAT_YYYY_MM_DD_HH);
                        break;
                    case DateTimeType.To_YYYY_MM_DD_HH_MM:
                        result = dp.DisplayDate.ToString(ConstantCollection.DATE_FORMAT_YYYY_MM_DD_HH_MM);
                        break;

                    case DateTimeType.To_YYYY_MM_DD_HH_MM_SS:
                        result = dp.DisplayDate.ToString(ConstantCollection.DATE_FORMAT_YYYY_MM_DD_HH_MM_SS);
                        break;
                    default:
                        result = dp.DisplayDate.ToString(ConstantCollection.DATE_FORMAT_YYYY_MM_DD_HH_MM_SS);
                        break;
                }
                return result;
            }
            set
            {
                var dt = value;
                try
                {
                    var ci = CultureInfo.CurrentCulture;
                    switch (Format)
                    {
                        case DateTimeType.To_HH_MM_SS:
                            dp.DisplayDate = DateTime.ParseExact(dt, ConstantCollection.DATE_FORMAT_HH_MM_SS, ci);
                            break;
                        case DateTimeType.To_YYYY:
                            dp.DisplayDate = DateTime.ParseExact(dt, ConstantCollection.DATE_FORMAT_YYYY, ci);
                            break;
                        case DateTimeType.To_YYYY_MM:
                            dp.DisplayDate = DateTime.ParseExact(dt, ConstantCollection.DATE_FORMAT_YYYY_MM, ci);
                            break;
                        case DateTimeType.Default:
                        case DateTimeType.To_YYYY_MM_DD:
                            dp.DisplayDate = DateTime.ParseExact(dt, ConstantCollection.DATE_FORMAT_YYYY_MM_DD, ci);
                            break;
                        case DateTimeType.To_YYYY_MM_DD_HH: ;
                            dp.DisplayDate = DateTime.ParseExact(dt, ConstantCollection.DATE_FORMAT_YYYY_MM_DD_HH, ci);
                            break;
                        case DateTimeType.To_YYYY_MM_DD_HH_MM:
                            dp.DisplayDate = DateTime.ParseExact(dt, ConstantCollection.DATE_FORMAT_YYYY_MM_DD_HH_MM, ci);
                            break;
                        case DateTimeType.To_YYYY_MM_DD_HH_MM_SS:
                            dp.DisplayDate = DateTime.ParseExact(dt, ConstantCollection.DATE_FORMAT_YYYY_MM_DD_HH_MM_SS, ci);
                            break;
                        default:
                            dp.DisplayDate = DateTime.ParseExact(dt, ConstantCollection.DATE_FORMAT_YYYY, ci);
                            break;
                    }
                }
                catch (Exception ee)
                {
                    Wrapper.ShowDialog(ee.Message);
                }
            }
        }

        public ControlDateTime()
        {
            InitializeComponent();

            cControl.Content = dp;
        }
    }
}
