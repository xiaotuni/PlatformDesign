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
using PlatformClient.Model.Method;
using PlatformClient.Extend.Core;

namespace PlatformClient.PageDesignTime.Controls
{
    /// <summary>
    /// 表和字段
    /// </summary>
    public partial class XtnDBTableAndFields : UserControl
    {
        /// <summary>
        /// 返回选中的字符项
        /// </summary>
        public object SelectedField { get { return this.lboxFields.SelectedItem; } }

        /// <summary>
        /// 字段集合
        /// </summary>
        public List<MetaDataColumnInfo> Fields { get; set; }
        /// <summary>
        /// 表的名称
        /// </summary>
        public string TableName { get; set; }
        /// <summary>
        /// 返回当前选择的字段信息
        /// </summary>
        public MetaDataColumnInfo SelectedMetaData
        {
            get
            {
                ListBoxItem item = this.lboxFields.SelectedItem as ListBoxItem;
                if (null == item)
                {
                    return null;
                }
                return item.Tag as MetaDataColumnInfo;
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public XtnDBTableAndFields()
        {
            InitializeComponent();
            this.tables.SelectedItemChanged += tables_SelectedItemChanged;
        }

        void tables_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            XtnDBTables table = sender as XtnDBTables;
            TreeViewItem obj = table.SelectedItem as TreeViewItem;
            Fields = obj.Tag as List<MetaDataColumnInfo>;
            TableName = Fields[0].table_name;
            this.lboxFields.Items.Clear();
            this.lboxFields.Tag = Fields;
            foreach (var field in Fields)
            {
                ListBoxItem data = new ListBoxItem();
                data.Content = string.Format("{0}【{1}】", field.column_name, field.column_comment);
                data.Tag = field;
                this.lboxFields.Items.Add(data);
            }
        }
        /// <summary>
        /// 设置要选择表
        /// </summary>
        /// <param name="tableName">表的名称</param>
        internal void SetSelectedItem(string tableName)
        {
            if (tableName.IsNullOrEmpty())
            {
                return;
            }
            this.tables.SetSelectedItem(tableName);
        }
    }
}
