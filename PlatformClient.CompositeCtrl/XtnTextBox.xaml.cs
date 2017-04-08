using System;
using System.Reflection;
using System.Windows.Controls;
using System.Xml.Linq;
using PlatformClient.Common;
using PlatformClient.Common.Lib;
using PlatformClient.Common.Interfaces;
using PlatformClient.Model.Method;
using PlatformClient.Model;
using PlatformClient.Extend.Core;

namespace PlatformClient.CompositeCtrl
{
    /// <summary>
    /// 
    /// </summary>
    public partial class XtnTextBox : UserControl, ICompositeCtrl
    {
        IPageRuntime _IPageRuntime;
        ControlInfo _ControlInfo;
        MetaDataInfo _MetaDataInfo;

        /// <summary>
        /// 
        /// </summary>
        [PropertyInfoAttribute("", "文件框标题")]
        public object TextContent { get { return this.txtContent.Text; } set { this.txtContent.Text = string.Format("{0}", value); } }

        /// <summary>
        /// 文件框值
        /// </summary>
        //[PropertyInfoAttribute("", "文件框值")]
        public String TextValue { get { return this.txtValue.Text; } set { this.txtValue.Text = value; } }

        /// <summary>
        /// 
        /// </summary>
        public XtnTextBox()
        {
            InitializeComponent();

            this.GotFocus += XtnTextBox_GotFocus;
        }

        void XtnTextBox_GotFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            this.txtValue.Focus();
            this.txtValue.SelectAll();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="iRuntime"></param>
        public void SetPageRuntimeInterface(IPageRuntime iRuntime)
        {
            this._IPageRuntime = iRuntime;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public object GetProperty(string propertyName)
        {
            if (String.IsNullOrEmpty(propertyName))
            {
                return null;
            }
            var pi = this.GetType().GetProperty(propertyName);
            if (null == pi)
            {
                return null;
            }
            return pi.GetValue(this, null);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        public void SetProperty(string propertyName, object value)
        {
            if (String.IsNullOrEmpty(propertyName))
            {
                return;
            }
            var pi = this.GetType().GetProperty(propertyName);
            if (null == pi)
            {
                return;
            }
            pi.SetValue(this, Convert.ChangeType(value, pi.PropertyType, null), null);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="metaData"></param>
        public void InitLoad(CtrlPlatformCommandInfo metaData)
        {

        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public CtrlPlatformCommandInfo OutputParameter()
        {
            return null;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameter"></param>
        public void InputParameter(CtrlPlatformCommandInfo parameter)
        {
        }
        /// <summary>
        /// 
        /// </summary>
        public XElement EvaluationContent { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            this.GotFocus -= XtnTextBox_GotFocus;
        }
        /// <summary>
        /// 
        /// </summary>
        public double TitleWidth { get { return this.txtContent.Width; } set { this.txtContent.Width = value; } }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="metaData"></param>
        public void InitTitle(CtrlPlatformCommandInfo metaData)
        {
            _ControlInfo = metaData.ControlInfo;
            _MetaDataInfo = metaData.MetaDataInfo;
            this.TextContent = _ControlInfo.Comment;
        }
        /// <summary>
        /// 处理返回回来的命令
        /// </summary>
        /// <param name="cmd">复合控件命令对象类</param>
        public void DoAction(CtrlPlatformCommandInfo cmd)
        {
            try
            {
                string mn = cmd.TempValue.GetAttributeValue(ConstantCollection.METHOD_NAME);
                string _MethodName = string.Format("Process{0}", mn);
                var mi = this.GetType().GetMethod(_MethodName, BindingFlags.Instance | BindingFlags.NonPublic);
                if (null == mi)
                {
                    _IPageRuntime.AlertMessage("没有找着【" + _MethodName + "】");
                    return;
                }
                mi.Invoke(this, new object[] { cmd });
            }
            catch (Exception ee)
            {
                _IPageRuntime.AlertMessage(ee);
            }
        }
        /// <summary>
        /// 清空值操作
        /// </summary>
        public void ClearCtrlValue()
        {
            // To do
            this._NotifyRuntimePage(ConstantCollection.COMPOSITE_CONTROL_CLEAR_VALUE);
        }

        void _NotifyRuntimePage(string propertyName)
        {

        }
        /// <summary>
        /// 刷新操作
        /// </summary>
        public void Refresh(CtrlPlatformCommandInfo cmd)
        {
        }
    }
}
