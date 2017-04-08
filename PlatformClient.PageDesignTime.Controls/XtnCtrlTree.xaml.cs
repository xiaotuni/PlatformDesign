using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using PlatformClient.Extend.Core;
using PlatformClient.Model.Method;

namespace PlatformClient.PageDesignTime.Controls
{
    /// <summary>
    /// 控件树
    /// </summary>
    public partial class XtnCtrlTree : UserControl, IDisposable
    {
        /// <summary>
        /// 当前选中控件
        /// </summary>
        public event RoutedPropertyChangedEventHandler<object> SelectedItemChanged;

        List<ControlInfo> _Item;

        private ControlInfo _ControlInfo;
        /// <summary>
        /// 当前选中控件信息
        /// </summary>
        public ControlInfo SelectedControlInfo { get { return _ControlInfo; } }

        /// <summary>
        /// 获取当前选择行对象
        /// </summary>
        public object SelectedItem { get { return this.tv_Controls.SelectedItem; } }

        void _SelectedItemChangedMethod(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (null != SelectedItemChanged)
            {
                SelectedItemChanged(this, e);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public XtnCtrlTree()
        {
            InitializeComponent();
            this.tv_Controls.SelectedItemChanged += tv_Controls_SelectedItemChanged;
        }

        void tv_Controls_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            TreeViewItem tvi = tv_Controls.SelectedItem as TreeViewItem;
            if (tvi != null)
            {
                _ControlInfo = tvi.Tag as ControlInfo;
            }
            _SelectedItemChangedMethod(this, e);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public XtnCtrlTree(List<ControlInfo> item)
            : this()
        {
            this._Item = item;
            this.ShowTree(item);
        }

        /// <summary>
        /// 文本大纲(TreeViewItem.Tag = ControlInfo)
        /// </summary>
        /// <param name="item"></param>
        public void ShowTree(List<ControlInfo> item)
        {
            this.tv_Controls.Items.Clear();
            if (null == item || 0 == item.Count)
            {
                return;
            }
            //-->开始生成树
            TreeViewItem root = new TreeViewItem();
            root.Header = "LayoutRoot(根节点)";
            root.Tag = item;
            root.IsExpanded = true;
            this.tv_Controls.Items.Add(root);

            //-->找出父节点来
            BuildTree(root, "LayoutRoot", item);
        }

        /// <summary>
        /// 递归生成树
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="pNode"></param>
        /// <param name="item"></param>
        void BuildTree(TreeViewItem parent, string pNode, List<ControlInfo> item)
        {
            if (null == parent || pNode.IsNullOrEmpty() || null == item || 0 == item.Count)
            {
                return;
            }
            var childerns = item.Where(p => !p.ParentCtrlName.IsNullOrEmpty() && p.ParentCtrlName.Equals(pNode)).GetTList<ControlInfo>();
            if (null == childerns && 0 == childerns.Count)
            {
                return;
            }
            foreach (var child in childerns)
            {
                TreeViewItem c_tvi = new TreeViewItem();
                c_tvi.Name = string.Format("tvi_{0}", child.Name);
                c_tvi.Header = child.Name;
                c_tvi.Tag = child;
                ToolTipService.SetToolTip(c_tvi, string.Format("{0}", child.Comment));
                parent.Items.Add(c_tvi);
                //--> 查找此控件下所有子控件
                BuildTree(c_tvi, child.Name, item);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="controlName"></param>
        public void SetSelectionControl(string controlName)
        {
            if (controlName.IsNullOrEmpty())
            {
                return;
            }
            TreeViewItem objControl = this.tv_Controls.FindName(string.Format("tvi_{0}", controlName)) as TreeViewItem;
            if (null == objControl)
            {
                return;
            }
            objControl.IsExpanded = true;
            objControl.IsSelected = true;
            _ControlInfo = objControl.Tag as ControlInfo;
            _SelectedItemChangedMethod(this, null);
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            this.tv_Controls.SelectedItemChanged -= tv_Controls_SelectedItemChanged;
            _ControlInfo = null;
            _Item.Clear();
            _Item = null;
            tv_Controls.Items.Clear();
        }
    }
}
