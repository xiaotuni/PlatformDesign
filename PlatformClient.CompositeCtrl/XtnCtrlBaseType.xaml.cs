using System;
using System.Reflection;
using System.Windows.Controls;
using PlatformClient.Common.Interfaces;
using PlatformClient.Extend.Core;
using PlatformClient.Model.Method;
using PlatformClient.Utility;

namespace PlatformClient.CompositeCtrl
{
    /// <summary>
    /// 类型基本控件
    /// </summary>
    public partial class XtnCtrlBaseType : UserControl, ICompositeCtrl
    {
        /// <summary>
        /// 
        /// </summary>
        protected IPageRuntime _IPageRuntime;
        /// <summary>
        /// 
        /// </summary>
        protected MetaDataInfo _MetaDataInfo;
        /// <summary>
        /// 
        /// </summary>
        protected ControlInfo _ControlInfo;

        /// <summary>
        /// 数据类型
        /// </summary>
        public string DataType { get; set; }
        /// <summary>
        /// 字段长度
        /// </summary>
        public int FieldLength { get; set; }

        /// <summary>
        ///宽度 
        /// </summary>
        public double TitleWidth { get { return this.txtContent.Width; } set { this.txtContent.Width = value; } }

        /// <summary>
        /// 文件框标题
        /// </summary>
        [PropertyInfoAttribute("", "文件框标题")]
        public object TextContent { get { return this.txtContent.Content; } set { this.txtContent.Content = value; } }

        /// <summary>
        /// 是否显示
        /// </summary>
        [PropertyInfoAttribute("", "是否显示")]
        public bool IsDisplay
        {
            get
            {
                return this.Visibility == System.Windows.Visibility.Visible ? true : false;
            }
            set
            {
                this.Visibility = value == true ? this.Visibility = System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            }
        }

        /// <summary>
        /// 是否可用
        /// </summary>
        [PropertyInfoAttribute("", "是否可用")]
        public bool IsUse { get { return this.IsEnabled; } set { this.IsEnabled = value; } }

        /// <summary>
        /// 文件框值
        /// </summary>
        [PropertyInfoAttribute("", "文件框值")]
        public String TextValue
        {
            get { return this.txtValue.Text; }
            set
            {
                //-->判断值是否大于数据库字段长度
                if (value.IsNullOrEmpty())
                {
                    this.txtValue.Text = string.Empty;
                }
                else if (value.Length > FieldLength && 0 < FieldLength)
                {
                    Wrapper.ShowDialog("字符串长度大于数据库字符长度");
                }
                else
                {
                    this.txtValue.Text = value;
                }
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public XtnCtrlBaseType()
        {
            InitializeComponent();
            this.GotFocus += XtnCtrlBaseType_GotFocus;
        }

        void XtnCtrlBaseType_GotFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            if (null != txtValue)
            {
                this.txtValue.Focus();
                this.txtValue.SelectAll();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="iRuntime"></param>
        public virtual void SetPageRuntimeInterface(IPageRuntime iRuntime)
        {
            _IPageRuntime = iRuntime;
        }

        /// <summary>
        /// 获取属性值的值
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public virtual object GetProperty(string propertyName)
        {
            return this.GetPropertyValue(propertyName);
        }
        /// <summary>
        /// 设置属性的值
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="propertyValue"></param>
        public virtual void SetProperty(string propertyName, object propertyValue)
        {
            this.SetPropertyValue(propertyName, propertyValue);
        }
        /// <summary>
        /// 初始化操作
        /// </summary>
        /// <param name="cmd"></param>
        public virtual void InitLoad(CtrlPlatformCommandInfo cmd)
        {
            this._NotifyRuntimePage(ConstantCollection.COMPOSITE_CONTROL_INIT_LOAD);
        }

        /// <summary>
        /// 初始化标题
        /// </summary>
        /// <param name="cmd"></param>
        public virtual void InitTitle(CtrlPlatformCommandInfo cmd)
        {
            _ControlInfo = cmd.ControlInfo;
            _MetaDataInfo = cmd.MetaDataInfo;
            this.TextContent = _ControlInfo.Comment;
            if (null == _MetaDataInfo || null == _MetaDataInfo.Item || 0 == _MetaDataInfo.Item.Count)
            {
                return;
            }
            this.FieldLength = _MetaDataInfo.Item[0].character_maximum_length;
        }

        /// <summary>
        /// 输出参数
        /// </summary>
        /// <returns></returns>
        public virtual CtrlPlatformCommandInfo OutputParameter()
        {
            try
            {
                CtrlPlatformCommandInfo cmd = new CtrlPlatformCommandInfo();
                cmd.ControlInfo = this._ControlInfo;
                cmd.MetaDataInfo = this._MetaDataInfo;
                cmd.MetaDataInfo.Item[0].column_value = this.txtValue.Text;

                return cmd;
            }
            finally
            {
                this._NotifyRuntimePage(ConstantCollection.COMPOSITE_CONTROL_OUTPUT_PARAMETER);
            }
        }
        /// <summary>
        /// 输入参数
        /// </summary>
        /// <param name="cmd"></param>
        public virtual void InputParameter(CtrlPlatformCommandInfo cmd)
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
        /// 释放资源
        /// </summary>
        public virtual void Dispose()
        {
            this.GotFocus -= XtnCtrlBaseType_GotFocus;
        }

        /// <summary>
        /// 通知运行时
        /// </summary>
        /// <param name="methodName"></param>
        protected void _NotifyRuntimePage(string methodName)
        {
            if (null != this._IPageRuntime)
            {
                this._IPageRuntime.CompositeControlNotifyRuntimePage(this.Name, methodName);
            }
        }

        /// <summary>
        /// 清空值操作
        /// </summary>
        public virtual void ClearCtrlValue()
        {
            // To do
            this.txtValue.Text = string.Empty;
            this._NotifyRuntimePage(ConstantCollection.COMPOSITE_CONTROL_CLEAR_VALUE);
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
                var mi = this.GetType().GetMethod(_MethodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.CreateInstance);
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
        /// 刷新操作
        /// </summary>
        public virtual void Refresh(CtrlPlatformCommandInfo cmd)
        {
        }
    }
}
