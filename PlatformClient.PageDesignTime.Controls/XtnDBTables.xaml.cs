using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using PlatformClient.Extend.Core;
using PlatformClient.Model.Method;
using PlatformClient.Utility;

namespace PlatformClient.PageDesignTime.Controls
{
    /// <summary>
    /// 
    /// </summary>
    public partial class XtnDBTables : BaseControl, IDisposable
    {
        /// <summary>
        /// 属性的值更改时发生
        /// </summary>
        public event RoutedPropertyChangedEventHandler<object> SelectedItemChanged;
        static List<MetaDataColumnInfo> MetaDataColumnItem = new List<MetaDataColumnInfo>();
        Dictionary<string, List<MetaDataColumnInfo>> DictTable = new Dictionary<string, List<MetaDataColumnInfo>>();
        private object _SelectedItem;
        private bool _IsSetSelectedItem;
        private string _TableName;
        /// <summary>
        /// 获取当前选择的
        /// </summary>
        public object SelectedItem { get { return _SelectedItem == null ? this.tvTables.SelectedItem : _SelectedItem; } }

        /// <summary>
        /// 
        /// </summary>
        public XtnDBTables()
        {
            InitializeComponent();
            _TableName = string.Empty;
            this.Loaded += XtnDBTables_Loaded;
            this.tvTables.SelectedItemChanged += tvTables_SelectedItemChanged;
            this.btnRefresh.Click += btnRefresh_Click;
            this.txtInputValue.TextChanged += txtInputValue_TextChanged;
        }

        void XtnDBTables_Loaded(object sender, RoutedEventArgs e)
        {
            //-->选判断表是否已经存在了。
            //-->如果已经全部都存在，就不用发命令了，不存在，就重新发命令获取表的信息
            if (null == XtnDBTables.MetaDataColumnItem || 0 == XtnDBTables.MetaDataColumnItem.Count)
            {
                this.InitLoaded();
            }
            else
            {
                InitLoadTables();
            }
        }

        void txtInputValue_TextChanged(object sender, TextChangedEventArgs e)
        {
            Query();
        }

        void Query()
        {
            string queryStr = this.txtInputValue.Text.Trim();
            this.tvTables.Items.Clear();
            foreach (var key in DictTable)
            {
                if (key.Key.StartsWith(queryStr))
                {
                    TreeViewItem item = new TreeViewItem();
                    item.Header = key.Key;
                    item.Tag = key.Value;
                    ToolTipService.SetToolTip(item, key.Value[0].table_comment);
                    this.tvTables.Items.Add(item);
                }
            }
        }

        void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            this.InitLoaded();
        }

        void _SelectedItemChangedMethod(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (null != SelectedItemChanged)
            {
                SelectedItemChanged(sender, e);
            }
        }

        void tvTables_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            _SelectedItemChangedMethod(this, e);
        }

        void InitLoaded()
        {
            string Sql = string.Format("select t.table_name,tt.table_comment,tt.table_type,tt.create_time,t.column_name,t.is_nullable,t.data_type,");
            Sql += string.Format("t.column_comment,t.column_default,t.character_maximum_length,t.column_key,t.ordinal_position");
            Sql += string.Format(" from information_schema.COLUMNS t");
            Sql += string.Format(" INNER JOIN information_schema.tables tt on t.table_name = tt.table_name and t.TABLE_SCHEMA = tt.TABLE_SCHEMA");
            Sql += string.Format(" where t.TABLE_SCHEMA = 'db_system_platform'");
            Sql += string.Format(" order by t.TABLE_NAME");

            //Sql = Wrapper.ParseSqlSentence(Sql, "information_schema.columns");
            cmd = new CtrlPlatformCommandInfo();
            cmd.CommandName = ConstantCollection.CommandName_MixedCommand;
            cmd.TempValue = Wrapper.SetXmlValue(ConstantCollection.TABLE_SCHEMA, "InitLoaded");

            cmd.ExecSql = new List<CtrlExecSQLCmd>();
            var exec = new CtrlExecSQLCmd(CtrlExecSqlCmdType.Query, Sql, ConstantCollection.TABLE_SCHEMA, this.Name, "InitLoaded");
            cmd.ExecSql.Add(exec);

            this.SendToService(cmd, this);
        }

        /// <summary>
        /// 处理返回结果：初始化表信息
        /// </summary>
        /// <param name="cmd"></param>
        public void ProcessInitLoaded(CtrlPlatformCommandInfo cmd)
        {
            XtnDBTables.MetaDataColumnItem.Clear();
            if (null == cmd.ExecSql || 0 == cmd.ExecSql.Count)
            {
                return;
            }
            XtnDBTables.MetaDataColumnItem = Wrapper.ConvertToList<MetaDataColumnInfo>(cmd.ExecSql[0].Result);//, ConstantCollection.TABLE_SCHEMA);// "information_schema.columns");

            InitLoadTables();
            if (!_IsSetSelectedItem)
            {
                SetSelectedItem(_TableName);
            }
        }

        void InitLoadTables()
        {
            DictTable.Clear();
            var metadata = XtnDBTables.MetaDataColumnItem;
            if (null == metadata || 0 == metadata.Count)
            {
                return;
            }
            //-->获取所有表
            List<String> allTable = (from p in metadata
                                     orderby p.table_name
                                     select p.table_name).Distinct().GetTList<String>();

            tvTables.Items.Clear();
            foreach (var table in allTable)
            {
                List<MetaDataColumnInfo> data = metadata.Where(p => p.table_name.Equals(table)).GetTList<MetaDataColumnInfo>();
                DictTable.Add(table, data);

                TreeViewItem item = new TreeViewItem();
                item.Header = table;
                item.Tag = data;
                ToolTipService.SetToolTip(item, data[0].table_comment);
                this.tvTables.Items.Add(item);
            }
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public override void Dispose()
        {
            this.tvTables.SelectedItemChanged -= tvTables_SelectedItemChanged;
            this.btnRefresh.Click -= btnRefresh_Click;
            this.txtInputValue.TextChanged -= txtInputValue_TextChanged;
            this.Loaded -= XtnDBTables_Loaded;
            _TableName = string.Empty;
        }
        /// <summary>
        /// 设置选中表
        /// </summary>
        /// <param name="tableName">表的名称</param>
        internal void SetSelectedItem(string tableName)
        {
            if (tableName.IsNullOrEmpty())
            {
                return;
            }
            _IsSetSelectedItem = false;
            _TableName = tableName;
            foreach (TreeViewItem item in this.tvTables.Items)
            {
                if (item.Header.ToString().ToLower().Equals(tableName.ToLower()))
                {
                    _SelectedItem = item;
                    item.IsSelected = true;
                    _IsSetSelectedItem = true;
                    break;
                }
            }
        }
    }
}
