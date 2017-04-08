using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;
using PlatformClient.Extend.Core;
using PlatformClient.Common;
using PlatformClient.Common.Interfaces;
using PlatformClient.Model.Method;
using PlatformClient.Utility;

namespace PlatformClient.PageDesignTime.Controls
{
    /// <summary>
    /// DataGrid 控件列数据转换设计器
    /// </summary>
    public partial class XtnSetDataGridConvertColumnDB : UserControl, IPageDesignTimeUserControl
    {
        private XElement _EvaluationContent;
        private IPageDesignFramework _IDesignFramework;
        private ControlInfo _CurrentControlInfo;
        private Common.Lib.XmlTemplate _XmlTemplate;
        private MetaDataInfo _CurrentMetaDataInfo;
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
        public XtnSetDataGridConvertColumnDB()
        {
            InitializeComponent();
            this.xtnDataGridHeader.SelectionChanged += xtnDataGridHeader_SelectionChanged;
            this.xtnTable.SelectedItemChanged += xtnTable_SelectedItemChanged;
            this.btnDisplayField.Click += btnDisplayField_Click;
            this.btnRelationField.Click += btnRelationField_Click;
            this.Loaded += XtnSetDataGridConvertColumnDB_Loaded;
        }
        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            this.xtnDataGridHeader.SelectionChanged -= xtnDataGridHeader_SelectionChanged;
            this.xtnTable.SelectedItemChanged -= xtnTable_SelectedItemChanged;
            this.btnDisplayField.Click -= btnDisplayField_Click;
            this.btnRelationField.Click -= btnRelationField_Click;
            this.Loaded -= XtnSetDataGridConvertColumnDB_Loaded;
        }

        void XtnSetDataGridConvertColumnDB_Loaded(object sender, RoutedEventArgs e)
        {
            _CurrentControlInfo = this.IDesignFramework.GetControlInfoByControlName(this.CurrentSelectedControlName);
            if (null == _CurrentControlInfo.Convert)
            {
                _CurrentControlInfo.Convert = new List<ColumnDBConvertInfo>();
            }
            _XmlTemplate = this.IDesignFramework.GetCurrentXmlTemplate();
            _CurrentMetaDataInfo = this.IDesignFramework.GetMetaDataInfoByTableName(_CurrentControlInfo.MetaData);
            if (null == _CurrentMetaDataInfo)
            {
                Wrapper.ShowDialog("请先设置数据源。");
                return;
            }
            this.xtnDataGridHeader.ItemsSource = _CurrentMetaDataInfo.Item;
        }

        void xtnTable_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (null == _CurrentMetaDataInfo)
            {
                Wrapper.ShowDialog("请先设置数据源。");
                return;
            }
            var table = sender as XtnDBTables;
            var item = table.SelectedItem as TreeViewItem;
            var fields = item.Tag as List<MetaDataColumnInfo>;
            this.xtnTableField.Items.Clear();
            this.xtnTableField.Tag = fields;
            foreach (var field in fields)
            {
                ListBoxItem data = new ListBoxItem();
                data.Content = string.Format("{0}【{1}】", field.column_name, field.column_comment);
                data.Tag = field;
                this.xtnTableField.Items.Add(data);
            }
        }

        void xtnDataGridHeader_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.txtDisplayField.Text = string.Empty;
            this.txtRelationField.Text = string.Empty;
        }

        void btnRelationField_Click(object sender, RoutedEventArgs e)
        {
            if (null == _CurrentMetaDataInfo)
            {
                Wrapper.ShowDialog("请先设置数据源。");
                return;
            }
            var item = this.xtnTableField.SelectedItem as ListBoxItem;
            if (null == item)
            {
                Wrapper.ShowDialog("请选择关联字段.");
                return;
            }
            var field = item.Tag as MetaDataColumnInfo;
            this.txtRelationField.Text = item.Content.ToString();
            //-->获取当前选择的列
            var col = this.xtnDataGridHeader.SelectedItem as MetaDataColumnInfo;
            //-->更新xml里的信息
            var convert = _CurrentControlInfo.Convert.Where(p => p.ColumnName.Equals(col.column_name)).GetFirst<ColumnDBConvertInfo>();
            if (null == convert)
            {
                convert = new ColumnDBConvertInfo();
                _CurrentControlInfo.Convert.Add(convert);
            }
            convert.ColumnName = col.column_name;
            convert.RelationField = field.column_name;
            convert.RelationTableName = field.table_name;
            this.IDesignFramework.UpdateCurrentTemplate();
        }

        void btnDisplayField_Click(object sender, RoutedEventArgs e)
        {
            if (null == _CurrentMetaDataInfo)
            {
                Wrapper.ShowDialog("请先设置数据源。");
                return;
            }
            var item = this.xtnTableField.SelectedItem as ListBoxItem;
            if (null == item)
            {
                Wrapper.ShowDialog("请选择显示字段.");
                return;
            }
            var field = item.Tag as MetaDataColumnInfo;
            this.txtDisplayField.Text = item.Content.ToString();

            var col = this.xtnDataGridHeader.SelectedItem as MetaDataColumnInfo;
            //-->更新xml里的信息
            var convert = _CurrentControlInfo.Convert.Where(p => p.ColumnName.Equals(col.column_name)).GetFirst<ColumnDBConvertInfo>();
            if (null == convert)
            {
                convert = new ColumnDBConvertInfo();
                _CurrentControlInfo.Convert.Add(convert);
            }
            convert.ColumnName = col.column_name;
            convert.DisplayField = field.column_name;
            convert.RelationTableName = field.table_name;
            this.IDesignFramework.UpdateCurrentTemplate();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool CheckSave()
        {
            return true;
        }
    }
}
