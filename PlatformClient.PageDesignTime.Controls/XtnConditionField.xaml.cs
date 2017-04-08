using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;
using PlatformClient.Common.Interfaces;
using PlatformClient.Extend.Core;
using PlatformClient.Model.Method;
using PlatformClient.Utility;

namespace PlatformClient.PageDesignTime.Controls
{
    /// <summary>
    /// 
    /// </summary>
    public partial class XtnConditionField : UserControl, IPageDesignTimeUserControl
    {
        /// <summary>
        /// 
        /// </summary>
        public XElement EvaluationContent { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public IPageDesignFramework IDesignFramework { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string CurrentSelectedControlName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool IsFullscreen { get { return false; } }
        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            this.Loaded -= XtnConditionField_Loaded;
            this.tv_MetaData.SelectedItemChanged -= tv_MetaData_SelectedItemChanged;
        }
        /// <summary>
        /// 
        /// </summary>
        public XtnConditionField()
        {
            InitializeComponent();
            this.Loaded += XtnConditionField_Loaded;
            this.tv_MetaData.SelectedItemChanged += tv_MetaData_SelectedItemChanged;
        }

        void tv_MetaData_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var tv = sender as TreeView;
            var item = tv.SelectedItem as TreeViewItem;
            var column = item.Tag as MetaDataColumnInfo;
            if (null == column)
            {
                Wrapper.ShowDialog("请选择一张表的字段。");
                return;
            }

            var ci = IDesignFramework.GetControlInfoByControlName(CurrentSelectedControlName);
            ci.ColumnName = column.column_name;
            ci.Comment = column.column_comment;
            ci.MetaData = ((tv.GetParentItem(tv.SelectedItem) as TreeViewItem).Tag as MetaDataInfo).table_name;

            EvaluationContent = new XElement("DataSource", string.Format("{0}|{1}|{2}", ci.MetaData, ci.ColumnName, ci.Comment));
            IDesignFramework.UpdateCurrentTemplate();
        }

        void XtnConditionField_Loaded(object sender, RoutedEventArgs e)
        {
            ShowTree(this.IDesignFramework.GetCurrentXmlTemplate().MetaDataItem);
        }
        /// <summary>
        /// 文本大纲(TreeViewItem.Tag = ControlInfo)
        /// </summary>
        /// <param name="item"></param>
        void ShowTree(List<MetaDataInfo> item)
        {
            this.tv_MetaData.Items.Clear();
            if (null == item || 0 == item.Count)
            {
                return;
            }
            //-->开始生成树
            TreeViewItem root = new TreeViewItem();
            root.Header = "LayoutRoot(根节点)";
            root.Tag = item;
            root.IsExpanded = true;
            this.tv_MetaData.Items.Add(root);

            //-->找出父节点来
            BuildTree(root, "LayoutRoot", item);
        }

        private void BuildTree(TreeViewItem parent, string pNode, List<MetaDataInfo> item)
        {
            if (null == parent || pNode.IsNullOrEmpty() || null == item || 0 == item.Count)
            {
                return;
            }

            foreach (var child in item)
            {
                TreeViewItem c_tvi = new TreeViewItem();
                c_tvi.Name = string.Format("tvi_{0}", child.table_name);
                c_tvi.Header = string.Format("{0}", child.table_comment);
                c_tvi.Tag = child;
                //--> 查找此控件下所有子控件
                BuildColumnTree(c_tvi, child.table_name, child);
                parent.Items.Add(c_tvi);
            }
        }

        void BuildColumnTree(TreeViewItem parent, string tableName, MetaDataInfo columns)
        {
            foreach (var child in columns.Item)
            {
                TreeViewItem c_tvi = new TreeViewItem();
                c_tvi.Name = string.Format("tvi_{0}_{1}", tableName, child.column_name);
                c_tvi.Header = string.Format("{0}", child.column_comment);
                c_tvi.Tag = child;
                parent.Items.Add(c_tvi);
            }
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
