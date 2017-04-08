using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Xml.Linq;
using PlatformClient.Common.Interfaces;
using PlatformClient.Utility;
using PlatformClient.Extend.Core;
using PlatformClient.Model.Method;


namespace PlatformClient.PageDesignTime.Controls
{
    /// <summary>
    /// 
    /// </summary>
    public partial class XtnLogonFormDataAssociated : UserControl, IPageDesignTimeUserControl
    {
        private XElement _EvaluationContent;
        private IPageDesignFramework _IDesignFramework;
        private Model.Method.MetaDataColumnInfo metadata_username;
        private Model.Method.MetaDataColumnInfo metadata_password;
        private string _TableName;
        private string _UserField;
        private string _PasswordField;

        /// <summary>
        /// 是否全屏
        /// </summary>
        public bool IsFullscreen { get { return false; } }
        /// <summary>
        /// 赋值内容
        /// </summary>
        public XElement EvaluationContent { get { return _EvaluationContent; } set { _EvaluationContent = value; } }
        /// <summary>
        /// 设计时接口
        /// </summary>
        public IPageDesignFramework IDesignFramework { get { return _IDesignFramework; } set { _IDesignFramework = value; } }
        /// <summary>
        /// 当前选择控件的名称
        /// </summary>
        public string CurrentSelectedControlName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public XtnLogonFormDataAssociated()
        {
            InitializeComponent();
            this.btnPassword.Click += btnPassword_Click;
            this.btnUsername.Click += btnUsername_Click;
            this.Loaded += XtnLogonFormDataAssociated_Loaded;
        }

        void XtnLogonFormDataAssociated_Loaded(object sender, RoutedEventArgs e)
        {
            InitData();
        }
        void InitData()
        {
            if (null == EvaluationContent || EvaluationContent.Value.IsNullOrEmpty())
            {
                return;
            }
            var eValue = this.EvaluationContent.Value;
            var arr = eValue.Split('|');
            _TableName = arr[0];
            _UserField = arr[1];
            _PasswordField = arr[2];
            this.xtnTableField.SetSelectedItem(_TableName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool CheckSave()
        {
            return UpdateXml();
        }
        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
        }

        void btnUsername_Click(object sender, RoutedEventArgs e)
        {
            metadata_username = this.xtnTableField.SelectedMetaData;
            if (null == metadata_username)
            {
                Wrapper.ShowDialog("请选择要关联的用户名字段！");
            }
            _UserField = metadata_username.column_name;
        }

        void btnPassword_Click(object sender, RoutedEventArgs e)
        {
            metadata_password = this.xtnTableField.SelectedMetaData;
            if (null == metadata_password)
            {
                Wrapper.ShowDialog("请选择要关联的密码字段！");
            }
            _PasswordField = metadata_password.column_name;
        }

        bool UpdateXml()
        {
            if (null == metadata_username)
            {
                Wrapper.ShowDialog("请选择要关联的用户名字段！");
                return false;
            }
            if (null == metadata_password)
            {
                Wrapper.ShowDialog("请选择要关联的密码字段！");
                return false;
            }

            var fields = xtnTableField.Fields;
            if (null == fields || 0 == fields.Count)
            {
                Wrapper.ShowDialog("字符参数为空");
                return false;
            }
            var dt = fields[0];
            var mdi = new MetaDataInfo();
            //-->克隆出数据。
            mdi.Item = fields.CloneList<MetaDataColumnInfo>();
            mdi.table_comment = dt.table_comment;
            mdi.table_name = dt.table_name;
            mdi.table_type = dt.table_type;
            foreach (var data in mdi.Item)
            {
                data.table_comment = string.Empty;
                data.table_name = string.Empty;
                data.table_type = string.Empty;
            }
            var xml = IDesignFramework.GetCurrentXmlTemplate();
            var ci = IDesignFramework.GetControlInfoByControlName(this.CurrentSelectedControlName);
            if (null == ci)
            {
                Wrapper.ShowDialog("找不着控件信息。");
                return false;
            }

            ci.MetaData = ci.MetaData.IsNullOrEmpty() ? _TableName : ci.MetaData;
            ci.ColumnName = string.Format("{0}|{1}", _UserField, _PasswordField);

            ci.ControlDataSourceTableName = mdi.table_name;
            //-->判断tableName是否已经增加了
            var old_mdi = IDesignFramework.GetMetaDataInfoByTableName(mdi.table_name);
            if (null == old_mdi)
            {
                xml.MetaDataItem.Add(mdi);
            }
            IDesignFramework.UpdateCurrentTemplate();

            string value = string.Format("{0}|{1}|{2}", this.xtnTableField.TableName, _UserField, _PasswordField);
            _EvaluationContent = new XElement("DataAssociated", value);

            return true;
        }
    }
}
