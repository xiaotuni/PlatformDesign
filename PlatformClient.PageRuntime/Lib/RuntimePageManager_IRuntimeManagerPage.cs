using System.Collections.Generic;
using System.Windows.Controls;
using System.Xml.Linq;
using PlatformClient.Common.Interfaces;
using PlatformClient.Model.Method;
using PlatformClient.Model.Table;
using PlatformClient.Utility;
using PlatformClient.Extend.Core;

namespace PlatformClient.PageRuntime
{
    /// <summary>
    /// 运行时管理界面
    /// </summary>
    public partial class RuntimePageManager : IRuntimeManagerPage
    {
        Dictionary<string, TabItem> dictTabItem = new Dictionary<string, TabItem>();
        /// <summary>
        /// 
        /// </summary>
        public object ParentControl { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bizPageGUID"></param>
        public void OpenBizPageInNewTab(string bizPageGUID)
        {
            cmd = new CtrlPlatformCommandInfo();
            cmd.CommandName = "GetDataTable";
            string[] sqlItem = bizPageGUID.Split(',');
            string sql = string.Format("select t.* from PageDirectorySub t where t.PageGuid in ('{0}')", string.Join("','", sqlItem));

            cmd.ExecSql = new List<CtrlExecSQLCmd>();
            var exec = new CtrlExecSQLCmd(CtrlExecSqlCmdType.Query, sql, "PageDirectorySub", this.Name, "OpenBizPageInNewTab");
            cmd.ExecSql.Add(exec);

            cmd.TempValue = new XElement("query", new XAttribute(ConstantCollection.FUNCTION_NAME, "OpenBizPageInNewTab"));
            this.SendToService(cmd, this);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmd"></param>
        public void ProcessOpenBizPageInNewTab(CtrlPlatformCommandInfo cmd)
        {
            //-->开始进行解析信息了
            List<PageDirectorySub> dirItem = Wrapper.ConvertToList<PageDirectorySub>(cmd.ExecSql[0].Result);//, "PageDirectorySub");
            //-->创建多个TabItem页签
            foreach (var child in dirItem)
            {
                OpenNewPage(child);
            }
        }

        void OpenNewPage(PageDirectorySub child)
        {
            if (dictTabItem.ContainsKey(child.PageGuid))
            {
                var ti = dictTabItem[child.PageGuid];
                ti.IsSelected = true;
            }
            else
            {
                RuntimePage rp = new RuntimePage();
                rp.PageInfo = child;
                rp.IRuntimePageManager = this as IRuntimeManagerPage;
                TabItem ti = new TabItem();
                ti.IsSelected = true;
                ti.Content = rp;
                ti.Header = child.PageName;
                dictTabItem.Add(child.PageGuid, ti);
                this.tc_Manager.Items.Add(ti);
                rp.Redraw();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void CloseCurrentBizPage()
        {
            if (null == this.tc_Manager || 0 == this.tc_Manager.Items.Count)
            {
                return;
            }

            object selectedItem = this.tc_Manager.SelectedItem;
            if (null == selectedItem)
            {
                return;
            }
            this.tc_Manager.Items.Remove(selectedItem);
            TabItem ti = selectedItem as TabItem;
            IPageRuntime rp = ti.Content as IPageRuntime;
            rp.Dispose();
            if (dictTabItem.ContainsKey(rp.PageInfo.PageGuid))
            {
                dictTabItem.Remove(rp.PageInfo.PageGuid);
            }
            JudgeIsChildWindow();
        }
        /// <summary>
        /// 判断是不是弹出来的窗体
        /// </summary>
        void JudgeIsChildWindow()
        {
            //-->判断是不是弹出来的窗体。
            if (0 == this.tc_Manager.Items.Count && (this.ParentControl is XtnChildRunPage))
            {
                (this.ParentControl as XtnChildRunPage).Close();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageGUID"></param>
        public void CloseBizPage(string pageGUID)
        {
            if (dictTabItem.ContainsKey(pageGUID))
            {
                var ti = dictTabItem[pageGUID];
                var ipr = ti.Content as IPageRuntime;
                ipr.Dispose();
                this.tc_Manager.Items.Remove(ti);
                dictTabItem.Remove(pageGUID);

                JudgeIsChildWindow();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public void CloseAllBizPage()
        {
            if (null == this.tc_Manager || 0 == this.tc_Manager.Items.Count)
            {
                return;
            }
            foreach (var v in this.tc_Manager.Items)
            {
                TabItem ti = v as TabItem;
                if (null == ti)
                {
                    continue;
                }
                IPageRuntime irp = ti.Content as IPageRuntime;
                if (null == irp)
                {
                    continue;
                }
                if (dictTabItem.ContainsKey(irp.PageInfo.PageGuid))
                {
                    dictTabItem.Remove(irp.PageInfo.PageGuid);
                }
                irp.Dispose();
            }
            this.tc_Manager.Items.Clear();
            dictTabItem.Clear();
            JudgeIsChildWindow();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmd"></param>
        public void CallControlFunction(CtrlPlatformCommandInfo cmd)
        {

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmd"></param>
        public void BatchOpenPage(CtrlPlatformCommandInfo cmd)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        public override void Dispose()
        {

        }

        public void SwitchForm(CtrlPlatformCommandInfo ctrlCmd)
        {
            cmd = new CtrlPlatformCommandInfo();
            cmd.CommandName = ConstantCollection.CommandName_MixedCommand;
            string sql = string.Format("select t.* from PageDirectorySub t where t.PageGuid = '{0}' ", ctrlCmd.PageGuid.Split(',')[0]);

            cmd.ExecSql = new List<CtrlExecSQLCmd>();
            var exec = new CtrlExecSQLCmd(CtrlExecSqlCmdType.Query, sql, "PageDirectorySub", this.Name, "OpenBizPageInNewTab");
            cmd.ExecSql.Add(exec);

            cmd.TempValue = new XElement("query",
                                            new XAttribute(ConstantCollection.FUNCTION_NAME, "SwitchForm"),
                                            new XAttribute("PageGuid", ctrlCmd.PageGuid.Split(',')[1])
                                        );
            this.SendToService(cmd, this);
        }

        public void ProcessSwitchForm(CtrlPlatformCommandInfo cmd)
        {
            //-->开始进行解析信息了
            var pds = Wrapper.ConvertToModel<PageDirectorySub>(cmd.ExecSql[0].Result);
            if (null == pds)
            {
                return;
            }
            var old_page_guid = cmd.TempValue.GetAttributeValue("PageGuid");
            if (dictTabItem.ContainsKey(old_page_guid))
            {
                var ti = dictTabItem[old_page_guid];
                var ipr = ti.Content as IPageRuntime;
                ipr.Dispose();

                RuntimePage rp = new RuntimePage();
                rp.PageInfo = pds;
                rp.IRuntimePageManager = this as IRuntimeManagerPage;
                ti.IsSelected = true;
                ti.Content = rp;
                ti.Header = pds.PageName;
                dictTabItem.Add(pds.PageGuid, ti);
                rp.Redraw();
                dictTabItem.Remove(old_page_guid);
            }
        }

        public void RefreshForm(CtrlPlatformCommandInfo cmd)
        {
            if (dictTabItem.ContainsKey(cmd.PageGuid))
            {
                var ti = dictTabItem[cmd.PageGuid];
                var ipr = ti.Content as IApiRuntimePage;
                ipr.RefreshForm();
            }
        }

        public void RefreshAllForm(CtrlPlatformCommandInfo cmd)
        {
            foreach (var child in dictTabItem)
            {
                var ti = dictTabItem[child.Key];
                var ipr = ti.Content as IApiRuntimePage;
                ipr.RefreshForm();
            }
        }
    }
}
