using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using PlatformClient.Extend.Core;
using PlatformClient.Model.Method;
using PlatformClient.Model.Table;
using PlatformClient.Utility;
using PlatformClient.Utility.ParseXml;

namespace PlatformClient.EventDesign.Core
{
    public partial class XtnCreateEventWorkFlow
    {
        private EventDesignerContent _EventDesignerContent;

        /// <summary>
        /// 加载控件事件设置器
        /// </summary>
        void LoadingControlEventDesinger()
        {
            cmd = new CtrlPlatformCommandInfo();
            cmd.CommandName = ConstantCollection.CommandName_MixedCommand;
            string sql = string.Format("select t.* from EventDesigner t where t.PageGuid = '{0}' ", IDesignFramework.GetCurrentXmlTemplate().BaseInfo.PageGUID);
            sql += string.Format(" and t.ControlName = '{0}' ", _GetControlName());
            sql += string.Format(" and t.EventName = '{0}'", this.EventName);

            cmd.ExecSql = new List<CtrlExecSQLCmd>();
            var exec = new CtrlExecSQLCmd(CtrlExecSqlCmdType.Query, sql, "EventDesigner", this.Name, "LoadingControlEventDesinger");
            cmd.ExecSql.Add(exec);

            cmd.TempValue = Wrapper.SetXmlValue("Query", "LoadingControlEventDesinger");
            this.SendToService(cmd, this);
        }

        /// <summary>
        /// 获取控件名称
        /// </summary>
        /// <returns></returns>
        string _GetControlName()
        {
            /************************************************************************
             * 判断当前事件名称【EventName】它是来到于哪里，是Form还是自己所在控件里
             * 
             * 如果来到是Form里的话，此时的_ControlName就是用Wrapper.ParseControlName()得到
             * 
             * 否而的话，就不分析Control控件了
             * 
             ************************************************************************/
            var cc = ParseControlConfig.GetControlConfig(this.CurrentSelectedControl.GetType().Name);
            if (null == cc || null == cc.Functions || 0 == cc.Functions.Count)
            {
                return Wrapper.ParseControlName(EventControlName); ;
            }
            var fi = cc.Functions.Where(p => p.Name.Equals(EventName)).GetFirst<FunctionInfo>();
            if (null == fi)
            {
                return Wrapper.ParseControlName(EventControlName);
            }
            return EventControlName;
        }

        /// <summary>
        /// 处理加载控件事件设置
        /// </summary>
        /// <param name="cmd"></param>
        public void ProcessLoadingControlEventDesinger(CtrlPlatformCommandInfo cmd)
        {
            var edi = Wrapper.ConvertToModel<EventDesignerContent>(cmd.ExecSql[0].Result);//, "EventDesigner");
            if (null == edi)
            {
                return;
            }
            ImportWorkFlow(edi.EventContent);
        }

        /// <summary>
        /// 保存到数据库
        /// </summary>
        void SaveXmlToDB()
        {
            string _NewWorkFlow = this.GetWorkFlowXml();
            if (_OldWorkFlowStr.IsNotEmpty() && _OldWorkFlowStr.Equals(_NewWorkFlow))
            {
                return;
            }
            cmd = new CtrlPlatformCommandInfo();
            cmd.CommandName = ConstantCollection.CommandName_SaveEventDesignerContent;

            if (null == _EventDesignerContent)
            {
                _EventDesignerContent = new EventDesignerContent();
            }
            _EventDesignerContent.EventName = this.txtEventName.Text.Trim();
            _EventDesignerContent.Description = this.txtDescription.Text.Trim();
            _EventDesignerContent.EventContent = XElement.Parse(_NewWorkFlow);

            cmd.ParamCollection = _EventDesignerContent.ToXElement();
            cmd.TempValue = Wrapper.SetXmlValue("Save", "SaveXmlToDB");
            this.SendToService(cmd, this);
        }

        /// <summary>
        /// 返回保存是否成功。
        /// </summary>
        /// <param name="cmd"></param>
        public void ProcessSaveXmlToDB(CtrlPlatformCommandInfo cmd)
        {
            //-->判断处理是否成功
            if (cmd.ExecuteNonQueryResult.IsNullOrEmpty())
            {
                Wrapper.ShowDialog("成功");
            }
            else
            {
                Wrapper.ShowDialog("失败：" + cmd.ExecuteNonQueryResult);
            }
        }

        internal void SetEventDesignerContent(EventDesignerContent edc)
        {
            _EventDesignerContent = edc;
            if (null == edc)
            {
                return;
            }
            this.txtDescription.Text = string.Format("{0}", edc.Description);
            this.txtEventName.Text = string.Format("{0}", edc.EventName);
            ImportWorkFlow(edc.EventContent);
        }
    }
}
