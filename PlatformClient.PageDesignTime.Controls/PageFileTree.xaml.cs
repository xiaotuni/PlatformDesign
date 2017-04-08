using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;
using PlatformClient.Common;
using PlatformClient.Common.Interfaces;
using PlatformClient.Extend.Core;
using PlatformClient.Model.Events;
using PlatformClient.Model.Method;
using PlatformClient.Model.Table;
using PlatformClient.Utility;

namespace PlatformClient.PageDesignTime.Controls
{
    /// <summary>
    /// 
    /// </summary>
    public partial class PageFileTree : BaseControl, IPageFileTree
    {
        AddPageOrDirectory pfd = new AddPageOrDirectory();
        XtnChildWindow child = null;
        OperatorType _ExecuteFuction;
        /// <summary>
        /// 打开页面。
        /// </summary>
        public event EventHandler<OpenPageInfoEventArgs> OpenPageInfo;
        /// <summary>
        /// 更新页面名称
        /// </summary>
        public event EventHandler<OpenPageInfoEventArgs> UpdatePageName;
        static CtrlPlatformCommandInfo InitDirectoryParam;
        /// <summary>
        /// 当前选择的项
        /// </summary>
        public object SelectedItem { get { return this.tvPageManager.SelectedItem; } }

        /// <summary>
        /// 是否显示工具栏
        /// </summary>
        public Visibility IsDisplayToolbar { get { return this.sp_toolbar.Visibility; } set { this.sp_toolbar.Visibility = value; } }

        void _OpenPageInfoMethod(object sender, OpenPageInfoEventArgs e)
        {
            if (null != OpenPageInfo)
            {
                OpenPageInfo(sender, e);
            }
        }

        void _UpdatePageNameMethod(object sender, OpenPageInfoEventArgs e)
        {
            if (null != UpdatePageName)
            {
                UpdatePageName(sender, e);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public PageFileTree()
        {
            InitializeComponent();
            try
            {
                child = new XtnChildWindow(null, pfd);
                this.child.Closed += child_Closed;
                this.Loaded += PageFileTree_Loaded;
                this.btnAdd.Click += btnAdd_Click;
                this.btnModify.Click += btnModify_Click;
                this.btnDelete.Click += btnDelete_Click;
                this.btnAddDirectory.Click += btnAddDirectory_Click;
                this.btnRefresh.Click += btnRefresh_Click;
            }
            catch { }
        }

        void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = this.SelectedItem as TreeViewItem;
            if (null != item && item.Tag is PageDirectory)
            {
                PageDirectory pd = item.Tag as PageDirectory;
                pd.IsSendCommand = false;
                //-->发送命令获取下面所有目录及文件
                //-->获取目录的查询语句;
                //-->获取文件的查询语句
                InitCurrentNodeDirectory(pd.ID, item.Name);
            }
            else
            {
                InitDirectory(0);
            }
        }

        void PageFileTree_Loaded(object sender, RoutedEventArgs e)
        {
            if (null != InitDirectoryParam)
            {
                ProcessInitDirectory(InitDirectoryParam);
            }
            else
            {
                InitDirectory(0);
            }
        }

        /// <summary>
        /// 初始化目录
        /// 获取当前ParentID下所有目录，以及该节点下所有文件。
        /// {
        ///     select * from pageDirectory t where t.ParentID = @parentID;
        ///     select * from pageDirectorySub t where t.PageDirectoryID = @parentID;
        /// }
        /// </summary>
        public void InitDirectory(int parentID)
        {
            try
            {
                var cmd = GetInitPlatformCommandInfo(parentID, "InitDirectory");
                cmd.TempValue = new XElement("Directory", new XAttribute("FunctionName", "InitDirectory"));
                this.SendToService(cmd, this);
            }
            catch { }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmd"></param>
        public void ProcessInitDirectory(CtrlPlatformCommandInfo cmd)
        {
            InitDirectoryParam = cmd;
            this.tvPageManager.Items.Clear();
            this.tviRoot.Items.Clear();
            TreeViewItem root = new TreeViewItem();
            root.Expanded += root_Expanded;
            root.Collapsed += root_Expanded;
            PageDirectory _pd = new PageDirectory();
            _pd.ID = 0;
            _pd.NodeName = "页面管理器";
            root.Tag = _pd;
            root.IsExpanded = true;
            root.Header = _pd.NodeName;
            root.Name = string.Format("tvi_{0}", _pd.ID);

            this.tvPageManager.Items.Add(root);
            _InitDirectory(cmd, root);
        }

        /// <summary>
        /// 获取命令对象
        /// </summary>
        /// <param name="parentID">父节点ID</param>
        /// <param name="methodName">方法名称</param>
        /// <returns></returns>
        CtrlPlatformCommandInfo GetInitPlatformCommandInfo(int parentID, string methodName)
        {
            var cmd = new CtrlPlatformCommandInfo();
            cmd.CommandName = ConstantCollection.CommandName_MixedCommand;

            //-->目录
            cmd.ExecSql = new List<CtrlExecSQLCmd>();
            string sql = string.Format("select * from PageDirectory t where t.parentID = {0} order by t.createDate ", parentID);
            var exec = new CtrlExecSQLCmd(CtrlExecSqlCmdType.Query, sql, "PageDirectory", this.Name, methodName);
            cmd.ExecSql.Add(exec);
            //-->PageFile
            sql = string.Format("select * from PageDirectorySub t where t.PageDirectoryId = {0} ", parentID);
            exec = new CtrlExecSQLCmd(CtrlExecSqlCmdType.Query, sql, "PageDirectorySub", this.Name, methodName);
            cmd.ExecSql.Add(exec);
            return cmd;
        }

        void btnAddDirectory_Click(object sender, RoutedEventArgs e)
        {
            _ExecuteFuction = OperatorType.AddDirectory;
            child.Show();
        }

        void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            _ExecuteFuction = OperatorType.AddPage;
            child.Show();
        }

        void btnModify_Click(object sender, RoutedEventArgs e)
        {
            _ExecuteFuction = OperatorType.Modify;
            TreeViewItem item = this.SelectedItem as TreeViewItem;
            pfd.GetNodeName = string.Format("{0}", item.Header);
            child.Show();
        }

        void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            _ExecuteFuction = OperatorType.Delete; ;
            child.Show();
        }

        void child_Closed(object sender, System.EventArgs e)
        {
            string name = pfd.GetNodeName;
            if (name.IsNullOrEmpty())
            {
                return;
            }
            TreeViewItem item = this.tvPageManager.SelectedItem as TreeViewItem;

            switch (_ExecuteFuction)
            {
                case OperatorType.AddDirectory:
                    string inserSql = string.Format("insert into page_directory(parentID,NodeName,CreateDate)values(");
                    inserSql += string.Format(" {0},", 0);
                    inserSql += string.Format(" {0},", name);
                    inserSql += string.Format(" NOW() )");
                    break;
                case OperatorType.AddPage:
                    AddPage(name);
                    break;
                case OperatorType.Delete:
                    DeleteOperator(name);
                    break;
                case OperatorType.Modify:
                    ModifyOperator(name);
                    break;
            }
        }

        /// <summary>
        /// 修改操作
        /// </summary>
        /// <param name="name"></param>
        void ModifyOperator(string name)
        {
            TreeViewItem item = this.tvPageManager.SelectedItem as TreeViewItem;
            var cmd = new CtrlPlatformCommandInfo();
            cmd.CommandName = ConstantCollection.CommandName_MixedCommand;
            cmd.ExecSql = new List<CtrlExecSQLCmd>();
            Dictionary<String, Object> dict = new Dictionary<string, Object>();
            if (item.Tag is PageDirectory)
            {
                PageDirectory pd = item.Tag as PageDirectory;
                pd.NodeName = name;
                //-->修改当前选中目录
                string sql = string.Format("update PageDirectory  set NodeName = '{0}' where ID = {1}", pd.NodeName, pd.ID);
                var exec = new CtrlExecSQLCmd(CtrlExecSqlCmdType.ExecuteNonQuery, sql, "PageDirectory", this.Name, "ModifyOperator");
                cmd.ExecSql.Add(exec);

                cmd.TempValue = Wrapper.SetXmlValue("PageDirectory", "ModifyOperator",
                    new KeyValuePair<String, Object>[]
                    {
                        new KeyValuePair<String, Object>("IsDirectory",true),
                        new KeyValuePair<String, Object>("ControlName",item.Name),
                        new KeyValuePair<String, Object>("ID",pd.ID),
                        new KeyValuePair<String, Object>("NewValue",name)
                    });
            }
            else if (item.Tag is PageDirectorySub)
            {
                //-->修改当前选中的页面
                PageDirectorySub pds = item.Tag as PageDirectorySub;
                pds.PageName = name;
                string sql = string.Format("update PageDirectorySub set PageName = '{0}' where id = {1}", pds.PageName, pds.ID);

                var exec = new CtrlExecSQLCmd(CtrlExecSqlCmdType.ExecuteNonQuery, sql, "PageDirectorySub", this.Name, "ModifyOperator");
                cmd.ExecSql.Add(exec);

                dict.Add("IsDirectory", false);
                dict.Add("ID", pds.ID);
                cmd.TempValue = Wrapper.SetXmlValue("PageDirectorySub", "ModifyOperator",
                    new KeyValuePair<String, Object>[]
                    {
                        new KeyValuePair<String, Object>("IsDirectory",false),
                        new KeyValuePair<String, Object>("ControlName",item.Name),
                        new KeyValuePair<String, Object>("ID",pds.ID),
                        new KeyValuePair<String, Object>("NewValue",name)
                    });
            }
            this.SendToService(cmd, this);
        }

        /// <summary>
        /// 返回命令之修改
        /// </summary>
        /// <param name="cmd"></param>
        public void ProcessModifyOperator(CtrlPlatformCommandInfo cmd)
        {
            if (!cmd.ExecuteNonQueryResult.IsNullOrEmpty())
            {
                Wrapper.ShowDialog(cmd.ExecuteNonQueryResult.ToString());
                return;
            }
            string controlName = cmd.TempValue.GetAttributeValue("ControlName");
            string newValue = cmd.TempValue.GetAttributeValue("NewValue");
            TreeViewItem item = this.tvPageManager.FindName(controlName) as TreeViewItem;
            if (null == item)
            {
                return;
            }
            item.Header = newValue;

            //-->如果是页面的名称修改了，通知PageWorkAre更新Header
            if (item.Tag is PageDirectorySub)
            {
                _UpdatePageNameMethod(this, new OpenPageInfoEventArgs(item.Tag as PageDirectorySub));
            }
        }

        /// <summary>
        /// 删除操作
        /// </summary>
        /// <param name="name"></param>
        void DeleteOperator(string name)
        {
            TreeViewItem item = this.tvPageManager.SelectedItem as TreeViewItem;
            var cmd = new CtrlPlatformCommandInfo();
            cmd.CommandName = ConstantCollection.CommandName_MixedCommand;
            cmd.ExecSql = new List<CtrlExecSQLCmd>();

            if (item.Tag is PageDirectory)
            {
                //-->判断是否还有子项
                //-->有子项不让其删除
                if (0 < item.Items.Count)
                {
                    Wrapper.ShowDialog("此目录下还有子项,请先删除子项");
                    return;
                }
                PageDirectory pd = item.Tag as PageDirectory;
                //-->删除当前选中目录
                string sql = string.Format("delete from PageDirectory where ID = {0}", pd.ID);
                //cmd.SqlSentence.Add(sql);

                var exec = new CtrlExecSQLCmd(CtrlExecSqlCmdType.ExecuteNonQuery, sql, "PageDirectory", this.Name, "DeleteOperator");
                cmd.ExecSql.Add(exec);

                cmd.TempValue = Wrapper.SetXmlValue("PageDirectory", "DeleteOperator",
                    new KeyValuePair<String, Object>[]
                    {
                        new KeyValuePair<String, Object>("ControlName",item.Name)
                    });
            }
            else if (item.Tag is PageDirectorySub)
            {
                //-->删除当前选中的页面
                PageDirectorySub pds = item.Tag as PageDirectorySub;
                string sql = string.Format("delete from PageDirectorySub where ID= '{0}'", pds.ID);

                var exec = new CtrlExecSQLCmd(CtrlExecSqlCmdType.ExecuteNonQuery, sql, "PageDirectorySub", this.Name, "DeleteOperator");
                cmd.ExecSql.Add(exec);

                cmd.TempValue = Wrapper.SetXmlValue("PageDirectorySub", "DeleteOperator",
                    new KeyValuePair<String, Object>[]
                    {
                        new KeyValuePair<String, Object>("ControlName",item.Name)
                    });
            }
            this.SendToService(cmd, this);
        }

        /// <summary>
        /// 处理删除是否成功
        /// </summary>
        /// <param name="cmd"></param>
        public void ProcessDeleteOperator(CtrlPlatformCommandInfo cmd)
        {
            if (!cmd.ExecuteNonQueryResult.IsNullOrEmpty())
            {
                Wrapper.ShowDialog(cmd.ExecuteNonQueryResult.ToString());
                return;
            }
            string controlName = cmd.TempValue.GetAttributeValue("ControlName");
            TreeViewItem item = this.tvPageManager.FindName(controlName) as TreeViewItem;
            //-->删除操作
            (item.Parent as TreeViewItem).Items.Remove(item);
        }

        /// <summary>
        /// 增加页面
        /// </summary>
        /// <param name="name"></param>
        void AddPage(string name)
        {
            TreeViewItem item = this.tvPageManager.SelectedItem as TreeViewItem;
            if (null == item)
            {
                Wrapper.ShowDialog("请选择目录");
                return;
            }
            PageDirectory pd = item.Tag as PageDirectory;
            if (null == pd)
            {
                Wrapper.ShowDialog("请选择目录");
                return;
            }
            var cmd = new CtrlPlatformCommandInfo();
            cmd.CommandName = ConstantCollection.CommandName_MixedCommand;
            cmd.ExecSql = new List<CtrlExecSQLCmd>();

            PageDirectorySub sds = new PageDirectorySub();
            sds.PageDirectoryId = pd.ID;
            sds.PageName = name;
            sds.PageGuid = Wrapper.GuidValue;

            string sql = string.Format("insert into PageDirectorySub(PageDirectoryId,PageGuid,PageName,CreateDate)values(");
            sql += string.Format("{0},", sds.PageDirectoryId);
            sql += string.Format("'{0}',", sds.PageGuid);
            sql += string.Format("'{0}',", sds.PageName);
            sql += string.Format("NOW())");
            var exec = new CtrlExecSQLCmd(CtrlExecSqlCmdType.ExecuteNonQuery, sql, "PageDirectorySub", this.Name, "AddPage");
            cmd.ExecSql.Add(exec);
            sql = string.Format("select * from PageDirectorySub t where t.PageGuid = '{0}'", sds.PageGuid);
            exec = new CtrlExecSQLCmd(CtrlExecSqlCmdType.Query, sql, "PageDirectorySub", this.Name, "AddPage");
            cmd.ExecSql.Add(exec);
            cmd.ParamCollection = sds.ToXElement();
            cmd.TempValue = Wrapper.SetXmlValue("AddPage", "AddPage",
                new KeyValuePair<String, Object>[] { 
                  new  KeyValuePair<String,Object>("ParentID",pd.ID),
                  new KeyValuePair<String,Object>("ControlName",item.Name)
                });
            this.SendToService(cmd, this);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmd"></param>
        public void ProcessAddPage(CtrlPlatformCommandInfo cmd)
        {
            //-->判断是否有成功
            if (!cmd.ExecuteNonQueryResult.IsNullOrEmpty())
            {
                Wrapper.ShowDialog("失败:" + cmd.ErrorMessage);
                return;
            }
            string controlName = cmd.TempValue.GetAttributeValue("ControlName");
            TreeViewItem item = tvPageManager.FindName(controlName) as TreeViewItem;
            if (null == item)
            {
                return;
            }
            //-->成功
            PageDirectorySub pds = Wrapper.ConvertToModel<PageDirectorySub>(cmd.ExecSql[1].Result);
            if (null == pds)
            {
                return;
            }
            TreeViewItem tvi = new TreeViewItem();
            tvi.Name = string.Format("tvi_{0}_{1}", pds.PageDirectoryId, pds.ID);
            tvi.Header = pds.PageName;
            if (!pds.PageContent.IsNullOrEmpty())
            {
                string[] strBuffer = pds.PageContent.Split(' ');
                int count = strBuffer.Length;
                byte[] buffer = new byte[count];
                for (int i = 0; i < count; i++)
                {
                    buffer[i] = byte.Parse(strBuffer[i]);
                }
                string content = Encoding.UTF8.GetString(buffer, 0, count);
                pds.PageContent = content;
            }
            tvi.Tag = pds;
            tvi.Expanded += tvi_Expanded;
            tvi.Collapsed += tvi_Expanded;
            item.Items.Add(tvi);
            var e = new OpenPageInfoEventArgs();
            e.PageDirectorySub = pds;
            _OpenPageInfoMethod(this, e);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmd"></param>
        public void ProcessSavePageDirectorySub(CtrlPlatformCommandInfo cmd)
        {
            if (cmd.ExecuteNonQueryResult.ConvertTo<int>() > 0)
            {
                OpenPageInfoEventArgs e = new OpenPageInfoEventArgs();
                List<PageDirectorySub> item = Wrapper.ConvertToList<PageDirectorySub>(cmd.ExecSql[0].Result);//, "PageDirectorySub");
                e.PageDirectorySub = item[0];
                //--------->通知打开页面操作。
                _OpenPageInfoMethod(this, e);
            }
            else
            {
                MessageBox.Show(cmd.ErrorMessage);
            }
        }

        void _InitDirectory(CtrlPlatformCommandInfo cmd, TreeViewItem parentItem)
        {
            if (null == cmd || null == cmd.ExecSql || 0 == cmd.ExecSql.Count)
            {
                return;
            }
            //-->table
            //获取目录
            PropertyInitDirectoryByDirectory(cmd.ExecSql[0], parentItem);

            //获取文件
            PropertyInitDirectoryByPageFile(cmd.ExecSql[1], parentItem);
        }

        /// <summary>
        /// 初始化目录
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="parentItem"></param>
        void PropertyInitDirectoryByDirectory(CtrlExecSQLCmd cmd, TreeViewItem parentItem)
        {
            List<PageDirectory> dirItem = Wrapper.ConvertToList<PageDirectory>(cmd.Result);
            if (null == dirItem || 0 == dirItem.Count)
            {
                return;
            }
            foreach (PageDirectory pd in dirItem)
            {
                TreeViewItem tvi = new TreeViewItem();
                tvi.Name = string.Format("tvi_{0}", pd.ID);
                tvi.Header = pd.NodeName;
                tvi.Tag = pd;
                tvi.Expanded += tvi_Expanded;
                tvi.Collapsed += tvi_Expanded;

                //-->加一个子节点
                TreeViewItem chidren = new TreeViewItem();
                chidren.Header = "正在加载...";
                chidren.Name = string.Format("tvi_{0}_loading", pd.ID);
                tvi.Items.Add(chidren);

                parentItem.Items.Add(tvi);
            }
        }
        /// <summary>
        /// 初始化Page文件
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="item"></param>
        void PropertyInitDirectoryByPageFile(CtrlExecSQLCmd cmd, TreeViewItem item)
        {
            if (null == item)
            {
                return;
            }
            //-->加载文件
            //-->判断文件在哪个目录下
            List<PageDirectorySub> dirItem = Wrapper.ConvertToList<PageDirectorySub>(cmd.Result);
            if (null == dirItem || 0 == dirItem.Count)
            {
                return;
            }
            foreach (var sub in dirItem)
            {
                TreeViewItem tvi = new TreeViewItem();
                tvi.Name = string.Format("tvi_{0}_{1}", sub.PageDirectoryId, sub.ID);
                tvi.Header = sub.PageName;
                sub.PageContent = sub.PageContent;
                tvi.Tag = sub;
                tvi.Expanded += tvi_Expanded;
                tvi.Collapsed += tvi_Expanded;
                item.Items.Add(tvi);
            }
        }

        void tvi_Expanded(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = sender as TreeViewItem;
            if (item.Tag is PageDirectory)
            {
                PageDirectory pd = item.Tag as PageDirectory;
                if (!pd.IsSendCommand)
                {
                    //-->发送命令获取下面所有目录及文件
                    //-->获取目录的查询语句;
                    //-->获取文件的查询语句
                    InitCurrentNodeDirectory(pd.ID, item.Name);
                }
            }
            else if (item.Tag is PageDirectorySub)
            {
                //-->打开文件
                OpenPageInfoEventArgs oe = new OpenPageInfoEventArgs();
                oe.PageDirectorySub = item.Tag as PageDirectorySub;
                _OpenPageInfoMethod(this, oe);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parentID"></param>
        /// <param name="parentControlName"></param>
        void InitCurrentNodeDirectory(int parentID, string parentControlName)
        {
            var cmd = GetInitPlatformCommandInfo(parentID, "InitCurrentNodeDirectory");
            cmd.TempValue = new XElement("Directory",
                new XAttribute("FunctionName", "InitCurrentNodeDirectory"),
                new XAttribute("ParentControlID", parentControlName));
            this.SendToService(cmd, this);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmd"></param>
        public void ProcessInitCurrentNodeDirectory(CtrlPlatformCommandInfo cmd)
        {
            //-->找着父节点的控件
            string controlName = cmd.TempValue.GetAttributeValue("ParentControlID");
            TreeViewItem item = tvPageManager.FindName(controlName) as TreeViewItem;
            if (null == item)
            {
                return;
            }
            item.Items.Clear();
            PageDirectory pd = item.Tag as PageDirectory;
            pd.IsSendCommand = true;
            _InitDirectory(cmd, item);
        }


        void root_Expanded(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = sender as TreeViewItem;
            item.IsExpanded = true;
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Dispose()
        {
            if (pfd is IDisposable)
            {
                (pfd as IDisposable).Dispose();
            }

            this.btnAdd.Click -= btnAdd_Click;
            this.btnModify.Click -= btnModify_Click;
            this.btnDelete.Click -= btnDelete_Click;
            this.btnAddDirectory.Click -= btnAddDirectory_Click;
            this.btnRefresh.Click -= btnRefresh_Click;
            this.Loaded -= PageFileTree_Loaded;
            this.child.Closed -= child_Closed;

            DisploseChild(this.tvPageManager.Items[0] as TreeViewItem);
        }

        void DisploseChild(TreeViewItem tvi)
        {
            if (null == tvi || 0 == tvi.Items.Count)
            {
                tvi.Expanded -= tvi_Expanded;
                tvi.Collapsed -= tvi_Expanded;
            }
            foreach (var item in tvi.Items)
            {
                DisploseChild(item as TreeViewItem);
            }
        }
    }
}
