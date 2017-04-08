using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;
using PlatformClient.Common;
using PlatformClient.Common.Interfaces;
using PlatformClient.Model.Method;
using PlatformClient.Utility;
using PlatformClient.Extend.Core;

namespace PlatformClient.PageDesignTime.Controls
{
    /// <summary>
    /// 设置数据源
    /// </summary>
    public partial class XtnSetComboBoxDataSource : UserControl, IPageDesignTimeUserControl
    {
        /// <summary>
        /// 当前选中控件名称
        /// </summary>
        public string CurrentSelectedControlName { get; set; }
        /// <summary>
        /// 绑定显示的列
        /// </summary>
        MetaDataColumnInfo _BindColumn;
        /// <summary>
        /// 保存的到数据库里的列
        /// </summary>
        MetaDataColumnInfo _SaveColumn;
        /// <summary>
        /// 赋值内容
        /// </summary>
        XElement _EvaluationContent;
        /// <summary>
        /// 表的名称
        /// </summary>
        string _TableName;
        /// <summary>
        /// 要保存的字段
        /// </summary>
        string _SaveField;
        /// <summary>
        /// 绑定字段
        /// </summary>
        string _BindField;
        /// <summary>
        /// 设计时接口
        /// </summary>
        public IPageDesignFramework IDesignFramework { get; set; }

        /// <summary>
        /// 返回赋值信息
        /// </summary>
        public XElement EvaluationContent { get { return _EvaluationContent; } set { _EvaluationContent = value; } }
        /// <summary>
        /// 构造函数
        /// </summary>
        public XtnSetComboBoxDataSource()
        {
            InitializeComponent();

            this.Loaded += SetDataSource_Loaded;
            this.tables.SelectedItemChanged += tables_SelectedItemChanged;
            this.btnBindField.Click += btnBindField_Click;
            this.btnSaveField.Click += btnSaveField_Click;
            //this.btnOK.Click += btnOK_Click;
        }

        void SetDataSource_Loaded(object sender, RoutedEventArgs e)
        {
            InitLoadData();
        }

        void InitLoadData()
        {
            if (null == _EvaluationContent)
            {
                return;
            }
            var aa = _EvaluationContent.Value;//.GetAttributeValue("Name");
            if (aa.IsNullOrEmpty())
            {
                return;
            }
            var arr = aa.Split('|');
            _TableName = arr[0];
            _SaveField = arr[1];
            _BindField = arr[2];

            tables.SetSelectedItem(_TableName);

            if (0 < lboxFields.Items.Count)
            {
                var select = lboxFields.Items.Where(p => ((p as ListBoxItem).Tag as MetaDataColumnInfo).column_name.ToLower().Equals(_SaveField.ToLower())).GetFirst<ListBoxItem>();
                lboxFields.SelectedItem = select;
                btnSaveField_Click(null, null);
                select = lboxFields.Items.Where(p => ((p as ListBoxItem).Tag as MetaDataColumnInfo).column_name.ToLower().Equals(_BindField.ToLower())).GetFirst<ListBoxItem>();
                lboxFields.SelectedItem = select;
                btnBindField_Click(null, null);
            }
        }

        bool UpdateMetaDataInfo()
        {
            var fields = this.lboxFields.Tag as List<MetaDataColumnInfo>;
            if (null == fields || 0 == fields.Count)
            {
                Wrapper.ShowDialog("字符参数为空");
                return false;
            }
            var dt = fields[0];
            MetaDataInfo mdi = new MetaDataInfo();
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
            ci.ColumnName = ci.ColumnName.IsNullOrEmpty() ? _SaveField : ci.ColumnName;

            ci.ControlDataSourceTableName = mdi.table_name;
            //-->判断tableName是否已经增加了
            MetaDataInfo old_mdi = IDesignFramework.GetMetaDataInfoByTableName(mdi.table_name);
            if (null == old_mdi)
            {
                xml.MetaDataItem.Add(mdi);
            }
            else
            {
                //-->判断列是否已经存在。
                foreach (var column in mdi.Item)
                {
                    var col = old_mdi.Item.Where(p => p.column_name.Equals(column.column_name)).GetFirst<MetaDataColumnInfo>();
                    if (null != col)
                    {
                        old_mdi.Item.Remove(col);
                    }
                    old_mdi.Item.Add(column);
                }
            }
            IDesignFramework.UpdateCurrentTemplate();
            return true;
        }

        void tables_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            XtnDBTables table = sender as XtnDBTables;
            TreeViewItem obj = table.SelectedItem as TreeViewItem;
            var fields = obj.Tag as List<MetaDataColumnInfo>;
            this.lboxFields.Items.Clear();
            this.lboxFields.Tag = fields;
            foreach (var field in fields)
            {
                ListBoxItem data = new ListBoxItem();
                data.Content = string.Format("{0}【{1}】", field.column_name, field.column_comment);
                data.Tag = field;
                this.lboxFields.Items.Add(data);
            }
        }

        void btnSaveField_Click(object sender, RoutedEventArgs e)
        {
            var item = this.lboxFields.SelectedItem as ListBoxItem;
            if (null == item)
            {
                Wrapper.ShowDialog("请选择要保存的字段.");
                return;
            }
            _SaveColumn = item.Tag as MetaDataColumnInfo;
            this.txtSaveField.Text = item.Content.ToString();
        }

        void btnBindField_Click(object sender, RoutedEventArgs e)
        {
            var item = this.lboxFields.SelectedItem as ListBoxItem;
            if (null == item)
            {
                Wrapper.ShowDialog("请选择要绑定的字段.");
                return;
            }
            _BindColumn = item.Tag as MetaDataColumnInfo;
            this.txtBindField.Text = item.Content.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            this.tables.SelectedItemChanged -= tables_SelectedItemChanged;
            this.btnBindField.Click -= btnBindField_Click;
            this.btnSaveField.Click -= btnSaveField_Click;
            //this.btnOK.Click -= btnOK_Click;
        }

        /// <summary>
        /// 查检是否保存
        /// </summary>
        public bool CheckSave()
        {
            //string value = string.Format("Table|SaveField|BindField",
            string value = string.Format("{0}|{1}|{2}",
                _SaveColumn != null ? _SaveColumn.table_name : "",
                _SaveColumn != null ? _SaveColumn.column_name : "",
                _BindColumn != null ? _BindColumn.column_name : "");

            //_EvaluationContent = new XElement("DataSource", new XAttribute("Name", value));
            _EvaluationContent = new XElement("DataSource", value);
            return UpdateMetaDataInfo();
        }

        /// <summary>
        /// 是否全屏
        /// </summary>
        public bool IsFullscreen
        {
            get { return false; }
        }
    }
}
