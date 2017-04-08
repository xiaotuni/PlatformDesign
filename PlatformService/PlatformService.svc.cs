using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.ServiceModel.Activation;
using System.Text;
using System.Xml.Linq;
using MySql.Data.MySqlClient;
using PlatformService.Database;
using PlatformService.Lib;
using PlatformService.Model;
using PlatformService.Utility;

namespace PlatformService
{
    /// <summary>
    ///注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码、svc 和配置文件中的类名“PlatformService”。 
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class PlatformService : IPlatformService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="aa"></param>
        /// <returns></returns>
        public string TextValue(string aa)
        {
            List<String> item = new List<string>();
            for (int i = 0; i < 100; i++)
            {
                item.Add(string.Format("{0}\t{1}", i.ToString().PadLeft(9, '0'), aa));
            }
            return string.Join("\r\n", item);
        }
        /// <summary>
        /// 开始执行
        /// </summary>
        /// <param name="pci"></param>
        /// <returns></returns>
        public PlatformCommandInfo DoAction(PlatformCommandInfo pci)
        {
            if (null == pci)
            {
                return null;
            }
            Type t = this.GetType();
            //-->命令名称
            string commandName = string.Format("Process{0}", pci.CommandName);
            //-->获取方法
            MethodInfo method = t.GetMethod(commandName, BindingFlags.NonPublic | BindingFlags.Instance);
            if (null == method)
            {
                return null;
            }
            try
            {
                object result = method.Invoke(this, new object[] { pci });
                return result as PlatformCommandInfo;
            }
            catch (Exception ee)
            {
                pci.ErrorMessage = ee.ToString();
                return pci;
            }
        }

        /// <summary>
        /// 批量处理命令
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        PlatformCommandInfo ProcessBatchCommand(PlatformCommandInfo cmd)
        {
            Dictionary<String, XElement> _DictSql = new Dictionary<string, XElement>();
            foreach (var child in cmd.ExecSql)
            {
                if (_DictSql.ContainsKey(child.Sql.ToLower()))
                {
                    child.Result = _DictSql[child.Sql.ToLower()];
                    continue;
                }
                switch (child.ExecType)
                {
                    case ExecSqlCmdType.ExecuteNonQuery:
                        child.Result = new XElement("Result", DBAccess.ExecuteNonQuery(child.Sql));
                        break;
                    case ExecSqlCmdType.Query:
                        DataTable dt = DBAccess.GetDataTable(child.Sql);
                        child.Result = PlatformHelper.ParseDataTable(dt, child.TableName);
                        break;
                    case ExecSqlCmdType.ExecuteScalar:
                        child.Result = new XElement("Result", DBAccess.ExecuteScalar(child.Sql));
                        break;
                }
                child.Error = DBAccess.ErrorMessage;
                _DictSql.Add(child.Sql, child.Result);
            }
            return cmd;
        }

        /// <summary>
        /// 处理获取数据
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        PlatformCommandInfo ProcessGetDataTable(PlatformCommandInfo cmd)
        {
            foreach (var child in cmd.ExecSql)
            {
                DataTable dt = DBAccess.GetDataTable(child.Sql);
                child.Result = PlatformHelper.ParseDataTable(dt, child.TableName);
                child.Error = DBAccess.ErrorMessage;
            }
            return cmd;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        PlatformCommandInfo ProcessExecuteNonQuery(PlatformCommandInfo cmd)
        {
            //int result = DBAccess.ExecuteNonQuery(cmd.SqlSentence);
            //if (0 != result)
            //{
            //    cmd.ExecuteNonQueryResult = DBAccess.ErrorMessage;
            //}
            //return cmd;

            foreach (var child in cmd.ExecSql)
            {
                //switch (child.ExecType)
                //{
                //    case ExecSqlCmdType.ExecuteNonQuery:
                child.Result = new XElement("Result", DBAccess.ExecuteNonQuery(child.Sql));
                child.Error = DBAccess.ErrorMessage;
                //        break;
                //    case ExecSqlCmdType.Query:
                //        DataTable dt = DBAccess.GetDataTable(child.Sql);
                //        child.Result = PlatformHelper.GetXml(dt, child.Sql, string.IsNullOrEmpty(child.TableName) ? "_tablename" : child.TableName);
                //        child.Error = DBAccess.ErrorMessage;
                //        break;
                //}
            }
            return cmd;
        }

        /// <summary>
        /// 混合命令.
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        PlatformCommandInfo ProcessMixedCommand(PlatformCommandInfo cmd)
        {
            if (null == cmd.ExecSql || 0 == cmd.ExecSql.Count)
            {
                cmd.ExecuteNonQueryResult = "混合命令，参数输入错误";
                return cmd;
            }
            foreach (var child in cmd.ExecSql)
            {
                switch (child.ExecType)
                {
                    case ExecSqlCmdType.ExecuteNonQuery:
                        child.Result = new XElement("Result", DBAccess.ExecuteNonQuery(child.Sql));
                        break;
                    case ExecSqlCmdType.Query:
                        DataTable dt = DBAccess.GetDataTable(child.Sql);
                        child.Result = PlatformHelper.ParseDataTable(dt, child.TableName);
                        break;
                    case ExecSqlCmdType.ExecuteScalar:
                        child.Result = new XElement("Result", DBAccess.ExecuteScalar(child.Sql));
                        break;
                }
                child.Error = DBAccess.ErrorMessage;
            }
            return cmd;
        }

        /// <summary>
        /// 向PageDirectorySub表中更新PageContent的内容
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        PlatformCommandInfo ProcessSavePageInfo(PlatformCommandInfo cmd)
        {
            var pageItems = cmd.ParamCollection.Elements("PageInfo");
            foreach (var v in pageItems)
            {
                string sqlSentence = v.GetAttributeValue("SqlSentence");
                string value = v.Value.Trim();
                byte[] buffer = Encoding.Default.GetBytes(value);
                MySqlParameter param = new MySqlParameter("?PageContent", MySqlDbType.MediumBlob, buffer.Length, ParameterDirection.Input, true, 0, 0, "", DataRowVersion.Default, buffer);
                int result = DBAccess.ExecuteNonQuery(sqlSentence, param);
                v.Add(new XAttribute("Result", result));
                v.Add(new XAttribute("ErrorMessage", string.Format("{0}", DBAccess.ErrorMessage)));
            }
            return cmd;
        }

        /// <summary>
        /// 向PageDirectorySub表中保存记录
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        PlatformCommandInfo ProcessSavePageDirectorySub(PlatformCommandInfo cmd)
        {
            //int result = DBAccess.ExecuteNonQuery(cmd.ExecSql[0].Sql);
            //if (0 == result)
            //{
            //    cmd.ExecuteNonQueryResult = "";
            //    PageDirectorySub pds = cmd.ParamCollection.ToModel<PageDirectorySub>();
            //    string sql = string.Format("select * from PageDirectorySub t where t.PageGuid = '{0}'", pds.PageGuid);
            //    //-->获取结果
            //    //cmd.SqlSentence = new List<string>();
            //    //cmd.SqlSentence.Add(Wrapper.ParseSqlSentence(sql, "PageDirectorySub"));
            //    //cmd = ProcessGetDataTable(cmd);

            //    cmd.ExecSql[0].Result = PlatformHelper.ParseDataTable( DBAccess.GetDataTable(sql
            //}
            //else
            //{
            //    cmd.ExecuteNonQueryResult = DBAccess.ErrorMessage;
            //    //cmd.ErrorMessage = "保存目录节点失败。";
            //}
            //cmd.ErrorMessage = DBAccess.ErrorMessage;
            return cmd;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        PlatformCommandInfo ProcessSaveEventDesigner(PlatformCommandInfo cmd)
        {
            var edi = cmd.ParamCollection.ToModel<EventDesigner>();
            List<String> itemSql = new List<string>();
            string sql = string.Format("delete from EventDesigner where PageGuid = '{0}' ", edi.PageGuid);
            sql += string.Format(" and ControlName = '{0}'", edi.ControlName);
            sql += string.Format(" and EventName = '{0}'", edi.EventName);
            itemSql.Add(sql);
            sql = string.Format("insert into EventDesigner(ContentID,PageGuid,ControlName,EventName)values(");
            sql += string.Format(" {0},", edi.ContentID);
            sql += string.Format(" '{0}',", edi.PageGuid);
            sql += string.Format(" '{0}',", edi.ControlName);
            sql += string.Format(" '{0}')", edi.EventName);
            itemSql.Add(sql);
            int result = DBAccess.ExecuteNonQuery(itemSql);
            if (0 != result)
            {
                cmd.ExecuteNonQueryResult = DBAccess.ErrorMessage;
            }
            return cmd;
        }
        /// <summary>
        /// 保存事件流
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        PlatformCommandInfo ProcessSaveEventDesignerContent(PlatformCommandInfo cmd)
        {
            if (null == cmd.ParamCollection)
            {
                return cmd;
            }
            var edi = cmd.ParamCollection.ToModel<EventDesignerContent>();
            if (null == edi)
            {
                cmd.ErrorMessage = "事件设计参数转换失败.";
                return cmd;
            }
            if (0 < edi.ID)
            {
                string sql = string.Format("insert into EventDesigner_Content(EventName,Description,EventContent)values(");
                sql += string.Format(" '{0}',", edi.EventName);
                sql += string.Format(" '{0}',", edi.Description);
                sql += string.Format("?event_content)");
                string content = string.Format("{0}", edi.EventContent);
                var buffer = Encoding.Default.GetBytes(content);
                MySqlParameter param = new MySqlParameter("?event_content", MySqlDbType.MediumBlob, buffer.Length, ParameterDirection.Input, true, 0, 0, "", DataRowVersion.Default, buffer);
                int result = DBAccess.ExecuteNonQuery(sql, param);
                if (0 != result)
                {
                    cmd.ExecuteNonQueryResult = DBAccess.ErrorMessage;
                }
            }
            else
            {
                string sql = string.Format("delete from EventDesigner_Content where id = '{0}' ", edi.ID);
                DBAccess.ExecuteNonQuery(sql);
                //-->增加记录操作
                sql = string.Format("insert into EventDesigner_Content(id,EventName,Description,EventContent)values(");
                sql += string.Format(" '{0}',", edi.ID);
                sql += string.Format(" '{0}',", edi.EventName);
                sql += string.Format(" '{0}',", edi.Description);
                sql += string.Format("?event_content)");
                string content = string.Format("{0}", edi.EventContent);
                var buffer = Encoding.Default.GetBytes(content);
                MySqlParameter param = new MySqlParameter("?event_content", MySqlDbType.MediumBlob, buffer.Length, ParameterDirection.Input, true, 0, 0, "", DataRowVersion.Default, buffer);
                int result = DBAccess.ExecuteNonQuery(sql, param);
                if (0 != result)
                {
                    cmd.ExecuteNonQueryResult = DBAccess.ErrorMessage;
                }
            }
            return cmd;
        }
    }
}
