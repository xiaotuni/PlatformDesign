using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;
using PlatformClient.Common;
using PlatformClient.Common.Interfaces;
using PlatformClient.Common.Lib;
using PlatformClient.Model.Method;
using PlatformClient.Utility;
using PlatformClient.Extend.Core;

namespace PlatformClient.PageDesignTime.Controls
{
    /// <summary>
    /// DataGrid数据源设置
    /// </summary>
    public partial class XtnSetDataGridItemsSource : UserControl, IPageDesignTimeUserControl
    {
        XElement _EvaluationContent;
        /// <summary>
        /// 是否全屏
        /// </summary>
        public bool IsFullscreen { get { return false; } }
        /// <summary>
        /// 设计时框架
        /// </summary>
        public IPageDesignFramework IDesignFramework { get; set; }
        /// <summary>
        /// 当前选择的控件名称
        /// </summary>
        public string CurrentSelectedControlName { get; set; }
        /// <summary>
        /// 当前选择表的列集合
        /// </summary>
        List<MetaDataColumnInfo> _CurrentDataTable;
        private string[] _fields;
        /// <summary>
        /// 赋值内容
        /// </summary>
        public XElement EvaluationContent
        {
            get
            {
                return _EvaluationContent;
            }
            set
            {
                _EvaluationContent = value;
            }
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        public XtnSetDataGridItemsSource()
        {
            InitializeComponent();
            tables.SelectedItemChanged += tables_SelectedItemChanged;

            this.Loaded += XtnSetDataGridItemsSource_Loaded;
        }
        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            tables.SelectedItemChanged -= tables_SelectedItemChanged;
            Loaded -= XtnSetDataGridItemsSource_Loaded;
        }

        void XtnSetDataGridItemsSource_Loaded(object sender, RoutedEventArgs e)
        {
            //-->初始化数据
            //this.EvaluationContent.Value
            //"user_info§主键～ID～int,用户名称～UserName～varchar,用户密码～Password～varchar,电子邮件～Email～varchar,地址～Address～varchar,国家ID～CountryID～int,城市ID～CityID～int,QQ、MSN帐号～SNSAccount～varchar,工作流ID～WorkFlowID～varchar,创建时间～Create_Date～datetime,创建者用户～Create_User～varchar"

            if (null == this.EvaluationContent || this.EvaluationContent.Value.IsNullOrEmpty())
            {
                return;
            }
            //-->获取表
            var _value = this.EvaluationContent.Value;
            var _data = _value.Split(ConstantCollection.Separator_Sub_section_number);
            var _TableName = _data[0];
            if (_data[1].IsNullOrEmpty())
            {
                return;
            }
            _fields = _data[1].Split(',');
            tables.SetSelectedItem(_TableName);
        }

        void tables_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            this.dgTable.ItemsSource = null;
            XtnDBTables table = sender as XtnDBTables;
            TreeViewItem obj = table.SelectedItem as TreeViewItem;
            if (null == obj)
            {
                return;
            }
            _CurrentDataTable = obj.Tag as List<MetaDataColumnInfo>;
            this.dgTable.ItemsSource = _CurrentDataTable;

            if (null == _fields || 0 == _fields.Length)
            {
                return;
            }
            foreach (var field in _fields)
            {
                var _fi = field.Split(ConstantCollection.Separator_Wavy_line);
                var column_name = _fi[1].ToLower();
                var data = _CurrentDataTable.Where(p => p.column_name.ToLower().Equals(column_name)).GetFirst<MetaDataColumnInfo>();
                if (null == data)
                {
                    continue;
                }
                data.IsChoose = true;
            }
        }

        void cbbAllChoice_Click(object sender, RoutedEventArgs e)
        {
            var all = sender as CheckBox;
            if (null == all)
            {
                return;
            }
            bool isChecked = all.IsChecked == true ? true : false;
            foreach (var item in _CurrentDataTable)
            {
                item.IsChoose = isChecked;
                if (!isChecked)
                {
                    item.IsHide = false;
                }
            }
        }

        void cbbAllHide_Click(object sender, RoutedEventArgs e)
        {
            var all = sender as CheckBox;
            if (null == all)
            {
                return;
            }
            bool isChecked = all.IsChecked == true ? true : false;
            foreach (var item in _CurrentDataTable)
            {
                item.IsHide = isChecked;
                if (isChecked && !item.IsChoose)
                {
                    item.IsChoose = true;
                }
            }
        }

        /// <summary>
        /// 查检是否保存
        /// </summary>
        public bool CheckSave()
        {
            if (null == _CurrentDataTable)
            {
                Wrapper.ShowDialog("请选择绑定的表。");
                return false;
            }
            //-->给控件属性赋值
            //-->判断是否选择了。如果没有选择的话，就不要这条记录
            var dt = _CurrentDataTable[0];
            //-->xml 控件信息设置 
            var xml = IDesignFramework.GetCurrentXmlTemplate();
            var ci = IDesignFramework.GetControlInfoByControlName(this.CurrentSelectedControlName);
            if (null != ci)
            {
                ci.MetaData = dt.table_name;
                ci.ColumnName = string.Join("|", from p in _CurrentDataTable
                                                 where p.IsChoose == true
                                                 select p.column_name);
                //ci.ControlDataSourceTableName = dt.table_name;
            }
            //-->xaml  控件属性
            var columns = from p in _CurrentDataTable
                          where p.IsChoose == true
                          select string.Format("{0}{1}{2}{1}{4}",
                          p.column_comment, ConstantCollection.Separator_Wavy_line,
                          p.column_name, ConstantCollection.Separator_Wavy_line,
                          p.data_type);
            string fields = string.Join(",", columns);
            string result = string.Format("{0}{1}{2}", dt.table_name, ConstantCollection.Separator_Sub_section_number, fields);
            //foreach (var data in _CurrentDataTable)
            //{
            //    data.IsChoose = false;
            //    data.IsHide = false;
            //}
            AddTableToXml(xml, _CurrentDataTable);
            IDesignFramework.UpdateCurrentTemplate();
            _EvaluationContent = new XElement("DataSource", result);
            return true;
        }
        /// <summary>
        /// 将当前选中的表添加到xml里的metadata里去。
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="_CurrentDataTable"></param>
        void AddTableToXml(XmlTemplate xml, List<MetaDataColumnInfo> _CurrentDataTable)
        {
            if (null == xml.MetaDataItem)
            {
                xml.MetaDataItem = new List<MetaDataInfo>();
            }
            var dt = _CurrentDataTable[0];

            var data = xml.MetaDataItem.Where(p => p.table_name.ToLower().Equals(dt.table_name.ToLower())).GetFirst<MetaDataInfo>();
            if (null != data)
            {
                return;
            }
            data = new MetaDataInfo();
            xml.MetaDataItem.Add(data);
            data.table_comment = dt.table_comment;
            data.table_name = dt.table_name;
            data.table_type = dt.table_type;
            data.Item = _CurrentDataTable.CloneList<MetaDataColumnInfo>();
            int _count = data.Item.Count;
            for (int i = 0; i < _count; i++)
            {
                var child = data.Item[i];
                child.table_comment = string.Empty;
                child.table_name = string.Empty;
                child.table_type = string.Empty;
                _CurrentDataTable[i].IsChoose = false;
            }
        }
    }
}
