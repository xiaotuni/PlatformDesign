using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using PlatformClient.Common;
using PlatformClient.Common.Interfaces;
using PlatformClient.Extend.Core;
using PlatformClient.Model.Method;
using PlatformClient.Model.Table;
using PlatformClient.Utility;

namespace PlatformClient.EventDesign.Core
{
    /// <summary>
    /// 事件树，主要是显示出所有设计出来的事件。
    /// </summary>
    public partial class XtnEventTree : BaseControl
    {
        /// <summary>
        /// EventDesigner_Content 表
        /// </summary>
        string _TableName = "EventDesigner_Content";
        /// <summary>
        /// 
        /// </summary>
        public IPageDesignFramework IDesignFramework { get; set; }
        /// <summary>
        /// 选择事件
        /// </summary>
        public event SelectionChangedEventHandler SelectionChanged;
        /// <summary>
        /// 删除操作
        /// </summary>
        public event EventHandler OnDelete;
        /// <summary>
        /// 修改操作
        /// </summary>
        public event EventHandler OnModify;
        private List<EventDesignerContent> _ItemsSource;
        private bool _IsSetSelectedItem;
        private int _EventID;
        EventDesignerContent _SelectedItem;
        /// <summary>
        /// 
        /// </summary>
        public EventDesignerContent SelectedItem
        {
            get
            {
                if (null == _SelectedItem)
                {
                    var item = this.lbEventContentItem.SelectedItem as ListBoxItem;
                    _SelectedItem = null == item ? null : item.Tag as EventDesignerContent;
                }
                return _SelectedItem;
            }
        }
        /// <summary>
        /// 结构函数
        /// </summary>
        public XtnEventTree()
        {
            InitializeComponent();

            this.Loaded += XtnEventTree_Loaded;
            this.lbEventContentItem.SelectionChanged += lbEventContentItem_SelectionChanged;
            this.btnNew.Click += btnNew_Click;
            this.btnDelete.Click += btnDelete_Click;
            this.btnModify.Click += btnModify_Click;
            this.btnRefresh.Click += btnRefresh_Click;
        }

        void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadingEventDesingerContent();
        }

        void btnModify_Click(object sender, RoutedEventArgs e)
        {
            if (null == SelectedItem)
            {
                Wrapper.ShowDialog("请选择要修改的事件流程。");
                return;
            }
            var edc = SelectedItem;
            XtnCreateEventWorkFlow work = new XtnCreateEventWorkFlow();
            work.SetEventDesignerContent(edc);
            XtnChildWindow modify = new XtnChildWindow(IDesignFramework, work);
            modify.Closed += modify_Closed;
            modify.SetTitle(string.Format("正在修改【{0}】事件流程", edc.EventName));
            modify.Show();
        }

        void modify_Closed(object sender, System.EventArgs e)
        {
            child_Closed(sender, e);
            if (null != OnModify)
            {
                OnModify(this, e);
            }
        }

        void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (null == SelectedItem)
            {
                Wrapper.ShowDialog("请选择要修改的事件流程。");
                return;
            }
            var edc = SelectedItem;
            cmd = new CtrlPlatformCommandInfo();
            cmd.CommandName = ConstantCollection.CommandName_MixedCommand;

            cmd.ExecSql = new List<CtrlExecSQLCmd>();
            var Sql = string.Format("delete from {0} where id = {1}", _TableName, edc.ID);
            var exec = new CtrlExecSQLCmd(CtrlExecSqlCmdType.ExecuteNonQuery, Sql, _TableName, this.Name, "DeleteEvent");
            cmd.ExecSql.Add(exec);

            cmd.TempValue = Wrapper.SetXmlValue("Delete", "DeleteEvent");
            this.SendToService(cmd, this);
            this.IsEnabled = false;
        }
        /// <summary>
        /// 处理删除事件
        /// </summary>
        /// <param name="cmd"></param>
        public void ProcessDeleteEvent(CtrlPlatformCommandInfo cmd)
        {
            this.IsEnabled = true;
            if (cmd.ExecuteNonQueryResult.IsNullOrEmpty())
            {
                this.lbEventContentItem.Items.Remove(this.lbEventContentItem.SelectedItem);
                ResetSort();
                if (null != OnDelete)
                {
                    OnDelete(this, null);
                }
                Wrapper.ShowDialog("删除成功。");
            }
            else
            {
                Wrapper.ShowDialog(cmd.ExecuteNonQueryResult);
            }
        }
        void ResetSort()
        {
            int index = 0;
            foreach (var child in this.lbEventContentItem.Items)
            {
                var item = child as ListBoxItem;
                var edc = item.Tag as EventDesignerContent;
                item.Content = string.Format("{0}、{1}", ++index, edc.EventName);
            }
        }

        void btnNew_Click(object sender, RoutedEventArgs e)
        {
            XtnCreateEventWorkFlow work = new XtnCreateEventWorkFlow();
            XtnChildWindow child = new XtnChildWindow(IDesignFramework, work);
            child.Closed += child_Closed;
            child.SetTitle(string.Format("创建新的事件处理流程"));
            child.Show();
        }

        void child_Closed(object sender, System.EventArgs e)
        {
            XtnChildWindow pcc = sender as XtnChildWindow;
            if (null != pcc)
            {
                pcc.Closed -= child_Closed;
                pcc.Dispose();
            }
        }

        void lbEventContentItem_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = this.lbEventContentItem.SelectedItem as ListBoxItem;
            if (null == item)
            {
                return;
            }
            _SelectedItem = item.Tag as EventDesignerContent;
            if (null != SelectionChanged)
            {
                SelectionChanged(this, e);
            }
        }

        void XtnEventTree_Loaded(object sender, RoutedEventArgs e)
        {
            LoadingEventDesingerContent();
        }

        void LoadingEventDesingerContent()
        {
            cmd = new CtrlPlatformCommandInfo();
            cmd.CommandName = ConstantCollection.CommandName_MixedCommand;
            string sql = string.Format("select t.* from {0} t order by EventName", _TableName);

            cmd.ExecSql = new List<CtrlExecSQLCmd>();
            var exec = new CtrlExecSQLCmd(CtrlExecSqlCmdType.Query, sql, _TableName, this.Name, "LoadingEventDesingerContent");
            cmd.ExecSql.Add(exec);

            cmd.TempValue = Wrapper.SetXmlValue("Query", "LoadingEventDesingerContent");
            this.SendToService(cmd, this);
        }

        /// <summary>
        /// 加载事件
        /// </summary>
        /// <param name="cmd"></param>
        public void ProcessLoadingEventDesingerContent(CtrlPlatformCommandInfo cmd)
        {
            _ItemsSource = Wrapper.ConvertToList<EventDesignerContent>(cmd.ExecSql[0].Result);//, "EventContent");
            if (null == _ItemsSource)
            {
                return;
            }
            int index = 1;
            this.lbEventContentItem.Items.Clear();
            foreach (var edc in _ItemsSource)
            {
                ListBoxItem item = new ListBoxItem();
                item.Content = string.Format("{0}、{1}", index++, edc.EventName);
                if (edc.Description.IsNotEmpty())
                {
                    ToolTipService.SetToolTip(item, edc.Description);
                }
                item.Tag = edc;
                this.lbEventContentItem.Items.Add(item);
            }
            if (!_IsSetSelectedItem)
            {
                SetSelectedItem(_EventID);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public override void Dispose()
        {
            this.Loaded -= XtnEventTree_Loaded;
            this.lbEventContentItem.SelectionChanged -= lbEventContentItem_SelectionChanged;
            this.btnNew.Click -= btnNew_Click;
            this.btnDelete.Click -= btnDelete_Click;
            this.btnModify.Click -= btnModify_Click;
            this.btnRefresh.Click -= btnRefresh_Click;
            base.Dispose();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventID">事件ID</param>
        internal void SetSelectedItem(int eventID)
        {
            _IsSetSelectedItem = false;
            _EventID = eventID;
            if (0 == this.lbEventContentItem.Items.Count)
            {
                return;
            }
            foreach (var child in this.lbEventContentItem.Items)
            {
                var item = child as ListBoxItem;
                if (null == item)
                {
                    continue;
                }
                var edc = item.Tag as EventDesignerContent;
                if (null == edc)
                {
                    continue;
                }
                if (edc.ID.Equals(eventID))
                {
                    _SelectedItem = edc;
                    _IsSetSelectedItem = true;
                    item.IsSelected = true;
                    this.lbEventContentItem.SelectedItem = item;
                }
            }
        }
    }
}
