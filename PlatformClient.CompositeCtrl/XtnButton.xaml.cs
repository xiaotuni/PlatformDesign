using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;
using PlatformClient.Common;
using PlatformClient.Common.Interfaces;
using PlatformClient.Model.Method;
using PlatformClient.Extend.Core;

namespace PlatformClient.CompositeCtrl
{
    /// <summary>
    /// Button--复合控件
    /// </summary>
    public partial class XtnButton : Button, ICompositeCtrl
    {
        IPageRuntime _IPageRuntime;
        ControlInfo _ControlInfo;
        MetaDataInfo _MetaDataInfo;
        CtrlPlatformCommandInfo _InitLoadParameter;
        ///// <summary>
        ///// 单击事件
        ///// </summary>
        //public event RoutedEventHandler Click;
        /// <summary>
        /// 是否可用
        /// </summary>
        [PropertyInfoAttribute("", "是否可用")]
        public bool IsUse { get { return this.IsEnabled; } set { this.IsEnabled = value; } }
        /// <summary>
        /// 是否显示
        /// </summary>
        [PropertyInfoAttribute("", "是否显示")]
        public bool IsDisplay { get { return this.Visibility == Visibility.Visible ? true : false; } set { this.Visibility = value == true ? Visibility.Visible : Visibility.Collapsed; } }

        /// <summary>
        /// 显示内容
        /// </summary>
        [PropertyInfoAttribute("", "显示内容")]
        public string Text { get { return string.Format("{0}", this.Content); } set { this.Content = value == null ? "" : value; } }

        /// <summary>
        /// 
        /// </summary>
        public double TitleWidth { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public XtnButton()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        [EventInfoAttribute("SetControlValue", "", "获取控件值")]
        public object SetControlValue(string value)
        {
            return "";
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
            return this.GetPropertyValue(propertyName);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        public void SetProperty(string propertyName, object value)
        {
            this.SetPropertyValue(propertyName, value);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="metaData"></param>
        public void InitLoad(CtrlPlatformCommandInfo metaData)
        {
            _InitLoadParameter = metaData;
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
        /// 
        /// </summary>
        /// <param name="cmd"></param>
        public void InitTitle(CtrlPlatformCommandInfo cmd)
        {
            _ControlInfo = cmd.ControlInfo;
            _MetaDataInfo = cmd.MetaDataInfo;
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
            if (null != this._IPageRuntime)
            {
                _IPageRuntime.CompositeControlNotifyRuntimePage(this.Name, propertyName);
            }
        }

        /// <summary>
        /// 刷新操作
        /// </summary>
        public void Refresh(CtrlPlatformCommandInfo cmd)
        {
            InitLoad(_InitLoadParameter);
        }
    }
}
