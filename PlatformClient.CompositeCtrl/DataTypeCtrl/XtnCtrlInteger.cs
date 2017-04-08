using System;
using System.Windows;
using System.Windows.Controls;
using PlatformClient.Common;
using PlatformClient.Common.Interfaces;
using PlatformClient.Model.Method;
using PlatformClient.Utility;
using PlatformClient.Extend.Core;

namespace PlatformClient.CompositeCtrl.DataTypeCtrl
{
    /// <summary>
    /// 
    /// </summary>
    public partial class XtnCtrlInteger : XtnCtrlBaseType, ICompositeCtrl
    {
        NumericUpDown nud = new NumericUpDown();

        /// <summary>
        /// 文件框值
        /// </summary>
        [PropertyInfoAttribute("", "文件框值")]
        public new String TextValue
        {
            get { return Convert.ToInt64(nud.Value).ToString(); }
            set
            {
                try
                {
                    if (value.IsNullOrEmpty())
                    {
                        return;
                    }
                    var result = Convert.ToInt64(value);
                    nud.Value = result;
                }
                catch (Exception ee)
                {
                    Wrapper.ShowDialog(ee.Message);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public XtnCtrlInteger()
        {
            if (null == nud)
            {
                nud = new NumericUpDown();
            }
            nud.Maximum = Int64.MaxValue;
            this.cControl.Content = nud;

        }

        /// <summary>
        /// 
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();
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
            this._NotifyRuntimePage("InputParameter");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override CtrlPlatformCommandInfo OutputParameter()
        {
            var opt = base.OutputParameter();
            opt.MetaDataInfo.Item[0].column_value = Convert.ToInt64(nud.Value).ToString();
            return opt;
        }
        /// <summary>
        /// 清空资源
        /// </summary>
        public override void ClearCtrlValue()
        {
            base.ClearCtrlValue();
            nud.Value = 0;
        }
    }
}
