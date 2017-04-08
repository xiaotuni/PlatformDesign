using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml.Linq;
using PlatformClient.Common;
using PlatformClient.Common.Interfaces;
using PlatformClient.Extend.Core;
using PlatformClient.Model;
using PlatformClient.Model.Method;
using PlatformClient.Utility;

namespace PlatformClient.CompositeCtrl
{
    /// <summary>
    /// 
    /// </summary>
    public partial class XtnLogonForm : UserControl, ICompositeCtrl
    {
        IPageRuntime _IPageRuntime;
        private string _Authenticode;
        private string _TableName;
        private string _UsernameField;
        private string _PasswordField;
        private bool _IsAuthenticode;
        /// <summary>
        /// 
        /// </summary>
        [PropertyInfoAttribute("", "密码加密类型")]
        public EncryptType EncryptType { get; set; }
        /// <summary>
        /// 是否要验证码
        /// </summary>
        [PropertyInfoAttribute("", "是否要验证码")]
        public bool IsAuthenticode
        {
            get { return _IsAuthenticode; }
            set
            {
                _IsAuthenticode = value;
                txtAuthCode.Visibility = value == true ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
                gAuthCode.Visibility = value == true ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            }
        }
        /// <summary>
        /// 数据关连
        /// </summary>
        [PropertyInfoAttribute("PlatformClient.PageDesignTime.Controls;PlatformClient.PageDesignTime.Controls.XtnLogonFormDataAssociated", "数据关连")]
        public string DataAssociated { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public XtnLogonForm()
        {
            InitializeComponent();
            this.Loaded += XtnLogonForm_Loaded;
            this.imgAuthenticode.MouseLeftButtonUp += imgAuthenticode_MouseLeftButtonUp;
            this.btnLogon.Click += btnLogon_Click;
            this.btnLogout.Click += btnLogout_Click;
        }

        void imgAuthenticode_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _Authenticode = Wrapper.BuildAuthenticode(imgAuthenticode, 28, 150);
        }

        void XtnLogonForm_Loaded(object sender, RoutedEventArgs e)
        {
            _Authenticode = Wrapper.BuildAuthenticode(imgAuthenticode, 28, 150);
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
        /// <param name="propertyValue"></param>
        public void SetProperty(string propertyName, object propertyValue)
        {
            this.SetPropertyValue(propertyName, propertyValue);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmd"></param>
        public void InitLoad(CtrlPlatformCommandInfo cmd)
        {
            if (DataAssociated.IsNullOrEmpty())
            {
                this._IPageRuntime.AlertMessage("登录控件数据关系没有设置。");
                return;
            }

            var arrary = this.DataAssociated.Split('|');
            _TableName = arrary[0];
            _UsernameField = arrary[1];
            _PasswordField = arrary[2];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmd"></param>
        public void InitTitle(CtrlPlatformCommandInfo cmd)
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
        /// <param name="cmd"></param>
        public void InputParameter(CtrlPlatformCommandInfo cmd)
        {
        }
        /// <summary>
        /// 
        /// </summary>
        public double TitleWidth { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmd"></param>
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
        public void ClearCtrlValue()
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmd"></param>
        public void Refresh(CtrlPlatformCommandInfo cmd)
        {
        }
        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            this.Loaded -= XtnLogonForm_Loaded;
            this.imgAuthenticode.MouseLeftButtonUp -= imgAuthenticode_MouseLeftButtonUp;
            this.btnLogon.Click -= btnLogon_Click;
            this.btnLogout.Click -= btnLogout_Click;
        }

        /// <summary>
        /// 通知运行时
        /// </summary>
        /// <param name="methodName"></param>
        void _NotifyRuntimePage(string methodName)
        {
            if (null != this._IPageRuntime)
            {
                this._IPageRuntime.CompositeControlNotifyRuntimePage(this.Name, methodName);
            }
        }

        /// <summary>
        /// 退出登录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            _NotifyRuntimePage(ConstantCollection.Composite_Control_Logout);
        }
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnLogon_Click(object sender, RoutedEventArgs e)
        {
            _UserLogonOperator();
        }

        /// <summary>
        /// 加密
        /// </summary>
        /// <returns></returns>
        string _Encrypt()
        {
            var password = this.txtPassword.Password;
            switch (this.EncryptType)
            {
                case Model.Method.EncryptType.Encrypt:
                    password = EncryptHelper.Encrypt(password);
                    break;
                case Model.Method.EncryptType.Md5:
                    password = EncryptHelper.Md5(password);
                    break;
            }
            return password;
        }
        /// <summary>
        /// 用户登录操作
        /// </summary>
        void _UserLogonOperator()
        {
            //-->判断验证码
            if (this.IsAuthenticode)
            {
                if (this.txtAuthenticode.Text.IsNullOrEmpty())
                {
                    this._IPageRuntime.AlertMessage("请输入验证码");
                    return;
                }
                if (!this.txtAuthenticode.Text.Trim().ToLower().Equals(_Authenticode.ToLower()))
                {
                    this._IPageRuntime.AlertMessage("验证码输入不正确");
                    return;
                }
            }
            //-->判断用户名密码不能为空
            if (this.txtUserName.Text.IsNullOrEmpty())
            {
                this._IPageRuntime.AlertMessage("用户名不能为空");
                return;
            }
            if (this.txtPassword.Password.IsNullOrEmpty())
            {
                this._IPageRuntime.AlertMessage("密码不能为空");
                return;
            }

            //-->获取加密密码
            var password = this._Encrypt();

            var newCmd = new CtrlPlatformCommandInfo();
            newCmd.CompositeControlName = this.Name;
            newCmd.CommandName = ConstantCollection.CommandName_MixedCommand;

            string sqlSentence = string.Format("select * from {0} where {1} = '{2}' and {3} = '{4}'", _TableName, _UsernameField, this.txtUserName.Text.Trim(), _PasswordField, password);
            newCmd.ExecSql = new List<CtrlExecSQLCmd>();
            var exec = new CtrlExecSQLCmd(CtrlExecSqlCmdType.Query, sqlSentence, _TableName, this.Name, "UserLogon");
            newCmd.ExecSql.Add(exec);

            newCmd.TempValue = new XElement("Query", new XAttribute(ConstantCollection.METHOD_NAME, "Logon"));
            this._IPageRuntime.SendCommand(newCmd);
        }
        /// <summary>
        /// 登录操作
        /// </summary>
        /// <param name="cmd"></param>
        void ProcessLogon(CtrlPlatformCommandInfo cmd)
        {
            var result = cmd.ExecSql[0];
            if (result.Error.IsNotEmpty())
            {
                _IPageRuntime.AlertMessage(result.Error);
                return;
            }
            var model = DynamicallyGeneratedClass.ToFirst(result.Result);
            if (null == model)
            {
                _IPageRuntime.AlertMessage("用户名或密码不正确。");
                return;
            }
            _NotifyRuntimePage(ConstantCollection.Composite_Control_LoginSucess);
        }
    }
}
