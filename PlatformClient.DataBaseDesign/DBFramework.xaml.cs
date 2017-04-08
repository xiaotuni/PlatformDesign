using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;
using PlatformClient.Common;
using PlatformClient.Common.Interfaces;
using PlatformClient.Model.Method;
using PlatformClient.PageDesignTime.Controls;
using PlatformClient.Utility;
using PlatformClient.Utility.ParseXml;
using PlatformClient.Extend.Core;

namespace PlatformClient.DataBaseDesign
{
    /// <summary>
    /// 数据库框架
    /// </summary>
    public partial class DBFramework : UserControl, IPlatformClientChildWindow
    {
        List<MetaDataColumnInfo> _CurrentDataTable = null;
        List<ControlConfig> _ControlTypeItem = new List<ControlConfig>();
        bool _IsFullscreen = false;
        /// <summary>
        /// 表的信息
        /// </summary>
        public XElement DataTable { get { return GetMetaDataInfo.ToXElement(); } }
        /// <summary>
        /// 
        /// </summary>
        public MetaDataInfo GetMetaDataInfo
        {
            get
            {
                if (null == _CurrentDataTable)
                {
                    return null;
                }
                //-->判断是否选择了。如果没有选择的话，就不要这条记录

                var dt = _CurrentDataTable[0];
                MetaDataInfo mdi = new MetaDataInfo();
                //-->克隆出数据。
                mdi.Item = _CurrentDataTable.CloneList<MetaDataColumnInfo>();
                mdi.table_comment = dt.table_comment;
                mdi.table_name = dt.table_name;
                mdi.table_type = dt.table_type;
                int _count = mdi.Item.Count;
                for (int i = 0; i < _count; i++)
                {
                    var data = mdi.Item[i];
                    data.table_comment = string.Empty;
                    data.table_name = string.Empty;
                    data.table_type = string.Empty;
                    //data.IsChoose = false;
                    _CurrentDataTable[i].IsChoose = false;
                }
                return mdi;
            }
        }

        /// <summary>
        /// 表的名称
        /// </summary>
        public string TableName { get { return _CurrentDataTable[0].table_name; } }

        /// <summary>
        /// 
        /// </summary>
        public bool IsFullscreen { get { return _IsFullscreen; } }

        /// <summary>
        /// 
        /// </summary>
        public DBFramework()
        {
            InitializeComponent();

            dbTable.SelectedItemChanged += dbTable_SelectedItemChanged;
            dgTable.LoadingRow += dgTable_LoadingRow;
            dgTable.CurrentCellChanged += dgTable_CurrentCellChanged;

            _ControlTypeItem.AddRange(ParseControlConfig.ControlItem.Where(p => p.IsDisplay).GetTList<ControlConfig>());
            ControlConfig _cc = new ControlConfig();
            _cc.Name = "--请选择--";
            _cc.Description = "默认为(TextBox)控件";
            _ControlTypeItem.Insert(0, _cc);
        }

        void dgTable_CurrentCellChanged(object sender, EventArgs e)
        {
            //-->判断两个地方，第一个是是否选择，第二个是否是隐藏
            DataGrid dg = sender as DataGrid;
            if (null == dg || null == dg.SelectedItem)
            {
                return;
            }
            var cbb = dg.CurrentColumn.GetCellContent(dg.SelectedItem) as CheckBox;
            if (null == cbb)
            {
                return;
            }
            cbb.IsChecked = cbb.IsChecked == true ? false : true;
            var data = dg.SelectedItem as MetaDataColumnInfo;
            if (cbb.Name.Equals("cbbHide"))
            {
                data.IsChoose = true;
            }
            if (cbb.Name.Equals("cbbChoice") && cbb.IsChecked == false)
            {
                data.IsHide = false;
            }
        }

        void dgTable_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            DataGrid dg = sender as DataGrid;
            MetaDataColumnInfo mdci = e.Row.DataContext as MetaDataColumnInfo;
            dg.SelectedItem = mdci;

            ComboBox cbb = dg.Columns[7].GetCellContent(e.Row) as ComboBox;
            cbb.ItemsSource = _ControlTypeItem;
            cbb.SelectionChanged += cbb_SelectionChanged;
            //-->要把当前的数据类型，选项控件。

            //-->如果有外键的话，就默认为ComboBox控件
            if (!mdci.column_key.IsNullOrEmpty() && mdci.column_key.ToLower().Equals(ConstantCollection.FOREIGN_KEY.ToLower()))
            {
                var ci = _ControlTypeItem.Where(p => p.Name.ToLower().Equals(ConstantCollection.CONTROL_COMBOBOX.ToLower())).GetFirst<ControlConfig>();
                cbb.SelectedItem = ci;
            }
            else
            {
                foreach (var ci in _ControlTypeItem)
                {
                    if (!ci.DataType.IsNullOrEmpty() && ci.DataType.ToLower().Contains(mdci.data_type.ToLower()))
                    {
                        cbb.SelectedItem = ci;
                        break;
                    }
                }
            }
            if (-1 == cbb.SelectedIndex)
            {
                cbb.SelectedIndex = 0;
            }
        }

        void cbb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MetaDataColumnInfo mdi = dgTable.SelectedItem as MetaDataColumnInfo;
            if (null == mdi)
            {
                return;
            }
            ComboBox cbb = sender as ComboBox;
            ControlConfig cc = cbb.SelectedItem as ControlConfig;
            if (null == cc)
            {
                return;
            }
            if (!cc.DataType.IsNullOrEmpty())
            {
                //-->判断数据类型是否匹配
                if (cc.DataType.ToLower().Contains(mdi.data_type.ToLower()))
                {
                    mdi.control_type = cc.Name;
                }
                else
                {
                    Wrapper.ShowDialog("数据类型不匹配，请重新选择!");
                }
            }
            else
            {
                mdi.control_type = cc.Name;
            }
        }

        void dbTable_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
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
        }

        /// <summary>
        /// 
        /// </summary>
        public bool CheckSave()
        {
            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            foreach (var child in this.dgTable.ItemsSource)
            {
                ComboBox cbb = this.dgTable.Columns[7].GetCellContent(child) as ComboBox;
                if (null == cbb)
                {
                    continue;
                }
                cbb.SelectionChanged -= cbb_SelectionChanged;
            }

            this.dgTable.CurrentCellChanged -= dgTable_CurrentCellChanged;
            this.dbTable.SelectedItemChanged -= dbTable_SelectedItemChanged;
            this.dgTable.LoadingRow -= dgTable_LoadingRow;
            this.dgTable.ItemsSource = null;
            this._ControlTypeItem.Clear();
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
    }
}