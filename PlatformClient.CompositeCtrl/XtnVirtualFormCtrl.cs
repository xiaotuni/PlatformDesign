using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Xml.Linq;
using PlatformClient.Common;
using PlatformClient.Common.Interfaces;
using PlatformClient.Model.Method;
using PlatformClient.Utility;
using PlatformClient.Extend.Core;

namespace PlatformClient.CompositeCtrl
{
    /// <summary>
    /// 虚拟控件
    /// </summary>
    public class XtnVirtualFormCtrl : FrameworkElement, ICompositeCtrl
    {
        /// <summary>
        /// 运行时接口
        /// </summary>
        IPageRuntime _IPageRuntime;
        /// <summary>
        /// 控件信息
        /// </summary>
        ControlInfo _ControlInfo;
        /// <summary>
        /// 元数据信息,只有在InitLoad的时候，才会改变这里的东西，其它的时候，此字段是不能进行修改的。
        /// </summary>
        MetaDataInfo _MetaDataInfo;
        CtrlPlatformCommandInfo _InitLoadParameter;
        /// <summary>
        /// 控件集合字典
        /// </summary>
        Dictionary<String, FrameworkElement> _DictControl = new Dictionary<string, FrameworkElement>();
        /// <summary>
        /// 宽
        /// </summary>
        public double TitleWidth { get; set; }

        /// <summary>
        /// 设置运行时接口
        /// </summary>
        /// <param name="iRuntime"></param>
        public void SetPageRuntimeInterface(IPageRuntime iRuntime)
        {
            this._IPageRuntime = iRuntime;
        }

        /// <summary>
        /// 获取属性值
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public object GetProperty(string propertyName)
        {
            try
            {
                return this.GetPropertyValue(propertyName);
            }
            finally
            {
                this._NotifyRuntimePage(ConstantCollection.COMPOSITE_CONTROL_GET_PROPERTY);
            }
        }

        /// <summary>
        /// 设置属性值
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="propertyValue"></param>
        public void SetProperty(string propertyName, object propertyValue)
        {
            this.SetPropertyValue(propertyName, propertyValue);
            this._NotifyRuntimePage(ConstantCollection.COMPOSITE_CONTROL_SET_PROPERTY);
        }

        /// <summary>
        /// 初始化控件的标题
        /// </summary>
        /// <param name="metaData"></param>
        public void InitTitle(CtrlPlatformCommandInfo metaData)
        {
        }

        /// <summary>
        /// 获取初始化值
        /// </summary>
        /// <param name="methodName"></param>
        void GetInitValue(string methodName)
        {
            string SqlSentence = _BuilderSqlSentence();
            CtrlPlatformCommandInfo cmd = new CtrlPlatformCommandInfo();
            cmd.CommandName = ConstantCollection.CommandName_MixedCommand;
            cmd.CompositeControlName = this.Name;
            cmd.ExecSql = new List<CtrlExecSQLCmd>();
            var exec = new CtrlExecSQLCmd(CtrlExecSqlCmdType.Query, SqlSentence, _MetaDataInfo.table_name, this.Name, methodName);
            cmd.ExecSql.Add(exec);
            cmd.TempValue = new XElement("query", new XAttribute(ConstantCollection.METHOD_NAME, methodName));
            _IPageRuntime.SendCommand(cmd);
        }

        /// <summary>
        /// 生成SQL语句
        /// </summary>
        /// <returns></returns>
        string _BuilderSqlSentence()
        {
            //-->发命令获取数据去。
            string fields = _ControlInfo.ColumnName.Replace('|', ',');
            //-->获取主键ID
            var mdciItem = _MetaDataInfo.Item.Where(p =>
                                                    !p.column_key.IsNullOrEmpty() &&
                                                    p.column_key.Equals(ConstantCollection.TABLE_KEY)
                                                    ).GetTList<MetaDataColumnInfo>();
            //-->条件一般是主键、工作流实例ID、用户ID三个。
            List<String> _WhereCondition = new List<string>();
            foreach (var mdci in mdciItem)
            {
                //string aa = string.Format("{0} = '{0}'",v.column_name,
                GlobalVariableInfo gvi = _IPageRuntime.GetGlobalVariablesByName(mdci.column_name);
                if (null == gvi || gvi.VariableValue.IsNullOrEmpty())
                {
                    continue;
                }
                _WhereCondition.Add(string.Format("{0} = '{1}'", mdci.column_name, gvi.VariableValue));
            }
            var wfid = _IPageRuntime.GetGlobalVariablesByName(ConstantCollection.WORK_FLOW_ID);
            if (null != wfid && wfid.VariableValue.IsNullOrEmpty())
            {
                _WhereCondition.Add(string.Format("{0} = '{1}'", ConstantCollection.WORK_FLOW_ID, wfid.VariableValue));
            }
            string sql = string.Format("select {0} from {1} ", fields, _MetaDataInfo.table_name);
            if (0 < _WhereCondition.Count)
            {
                sql += string.Format(" where {0}", string.Join(" and ", _WhereCondition));
            }
            return sql;
        }

        /// <summary>
        /// 初始化子控件
        /// </summary>
        /// <param name="_MetaDataInfo"></param>
        void InitChildControl(MetaDataInfo _MetaDataInfo)
        {
            List<String> _MegItem = new List<string>();
            foreach (var child in _MetaDataInfo.Item)
            {
                var controlName = string.Format("{0}_{1}", this.Name, child.column_name);
                var fe = _IPageRuntime.FindControl(controlName);
                if (null == fe)
                {
                    _MegItem.Add(controlName);
                    continue;
                }
                if (!_DictControl.ContainsKey(controlName))
                {
                    _DictControl.Add(controlName, fe);
                }
                //-->判断是否是复件控件
                if (fe is ICompositeCtrl)
                {
                    var icc = fe as ICompositeCtrl;
                    var _newCtrlMetaData = new MetaDataInfo();
                    _newCtrlMetaData.table_type = _MetaDataInfo.table_type;
                    _newCtrlMetaData.table_name = _MetaDataInfo.table_name;
                    _newCtrlMetaData.table_comment = _MetaDataInfo.table_comment;
                    _newCtrlMetaData.Item = new List<MetaDataColumnInfo>();
                    _newCtrlMetaData.Item.Add(child);

                    ControlInfo ci = new ControlInfo();
                    ci.Name = fe.Name;
                    ci.ColumnName = child.column_name;
                    ci.Comment = child.column_comment;
                    ci.ControlType = ConstantCollection.COMPOSITE_CONTROL;
                    ci.Type = fe.GetType().Name;
                    ci.MetaData = _MetaDataInfo.table_name;

                    CtrlPlatformCommandInfo ctrlCmd = new CtrlPlatformCommandInfo();
                    ctrlCmd.ControlInfo = ci;
                    ctrlCmd.MetaDataInfo = _newCtrlMetaData;
                    icc.InitLoad(ctrlCmd);
                }
            }
            if (0 != _MegItem.Count)
            {
                _IPageRuntime.AlertMessage(string.Format("有【{0}】控件在表单上没有找着\r\n{1}", _MegItem.Count, string.Join("\r\n", _MegItem)));
            }
        }

        /// <summary>
        /// 初始化操作
        /// </summary>
        /// <param name="metaData">参数</param>
        public void InitLoad(CtrlPlatformCommandInfo metaData)
        {
            _InitLoadParameter = metaData;
            _MetaDataInfo = metaData.MetaDataInfo;
            _ControlInfo = metaData.ControlInfo;

            //-->调用子控件的 InitLoad方法
            InitChildControl(_MetaDataInfo);

            GetInitValue(ConstantCollection.COMPOSITE_CONTROL_INIT_LOAD);

            this._NotifyRuntimePage(ConstantCollection.COMPOSITE_CONTROL_INIT_LOAD);
        }

        void ProcessInitLoad(CtrlPlatformCommandInfo cmd)
        {
            var datatable = cmd.ExecSql[0].Result;
            //-->给控件赋值操作。
            List<MetaDataColumnInfo> columns = DynamicallyGeneratedClass.GetDataTableColumnCollection(datatable);//, _MetaDataInfo.table_name);
            if (null == columns)
            {
                return;
            }
            var firstRow = datatable.Element("rows").Elements("row").GetFirst<XElement>();
            foreach (var column in columns)
            {
                string controlName = string.Format("{0}_{1}", this.Name, column.column_name);
                var icc = _IPageRuntime.FindControl(controlName) as ICompositeCtrl;
                if (null == icc)
                {
                    continue;
                }
                column.column_value = firstRow.GetAttributeValue(column.column_name.ToLower());
                column.table_name = _MetaDataInfo.table_name;
                column.table_type = _MetaDataInfo.table_type;
                column.table_comment = _MetaDataInfo.table_comment;

                CtrlPlatformCommandInfo newCmd = new CtrlPlatformCommandInfo();
                newCmd.ParamCollection = column.ToXElement();
                newCmd.MetaDataInfo = new MetaDataInfo();
                newCmd.MetaDataInfo.Item = new List<MetaDataColumnInfo>();
                newCmd.MetaDataInfo.Item.Add(column);
                newCmd.ControlInfo = _IPageRuntime.GetControlInfoByControlName(controlName);

                icc.InputParameter(newCmd);

            }
            this._NotifyRuntimePage(ConstantCollection.COMPOSITE_CONTROL_INIT_LOAD_COMPLATE);
        }

        /// <summary>
        /// 条件初始化
        /// </summary>
        /// <param name="metaData">参数</param>
        public void InitLoadByCondition(CtrlPlatformCommandInfo metaData)
        {
            InitLoad(metaData);
            this._NotifyRuntimePage(ConstantCollection.COMPOSITE_CONTROL_INIT_LOAD_BY_CONDITION);
        }

        /// <summary>
        /// 输出参数
        /// </summary>
        /// <returns></returns>
        public CtrlPlatformCommandInfo OutputParameter()
        {
            try
            {
                CtrlPlatformCommandInfo cmd = new CtrlPlatformCommandInfo();
                foreach (var mdci in _MetaDataInfo.Item)
                {
                    string controlName = string.Format("{0}_{1}", this.Name, mdci.column_name);
                    var icc = this._IPageRuntime.FindControl(controlName) as ICompositeCtrl;
                    if (null == icc)
                    {
                        continue;
                    }
                    var op = icc.OutputParameter();
                    if (null == op || null == op.MetaDataInfo || null == op.MetaDataInfo.Item || 0 == op.MetaDataInfo.Item.Count)
                    {
                        continue;
                    }
                    mdci.column_value = op.MetaDataInfo.Item[0].column_value;
                }
                cmd.MetaDataInfo = _MetaDataInfo;
                cmd.ControlInfo = this._ControlInfo;

                return cmd;
            }
            finally
            {
                this._NotifyRuntimePage(ConstantCollection.COMPOSITE_CONTROL_OUTPUT_PARAMETER);
            }
        }

        /// <summary>
        /// 输入参数
        /// </summary>
        /// <param name="cmd"></param>
        public void InputParameter(CtrlPlatformCommandInfo cmd)
        {
            if (null == cmd || null == cmd.MetaDataInfo || null == cmd.MetaDataInfo.Item || 0 == cmd.MetaDataInfo.Item.Count)
            {
                return;
            }
            var mdi = cmd.MetaDataInfo;
            foreach (var mdci in cmd.MetaDataInfo.Item)
            {
                string controlName = string.Format("{0}_{1}", this.Name, mdci.column_name);
                var fe = this._IPageRuntime.FindControl(controlName);

                //-->判断是否是复件控件
                if (fe is ICompositeCtrl)
                {
                    var icc = fe as ICompositeCtrl;
                    var _newMdi = new MetaDataInfo();
                    _newMdi.table_type = mdi.table_type;
                    _newMdi.table_name = mdi.table_name;
                    _newMdi.table_comment = mdi.table_comment;
                    _newMdi.Item = new List<MetaDataColumnInfo>();
                    _newMdi.Item.Add(mdci);

                    ControlInfo ci = new ControlInfo();
                    ci.Name = fe.Name;
                    ci.ColumnName = mdci.column_name;
                    ci.Comment = mdci.column_comment;
                    ci.ControlType = ConstantCollection.COMPOSITE_CONTROL;
                    ci.Type = fe.GetType().Name;
                    ci.MetaData = mdi.table_name;

                    CtrlPlatformCommandInfo ctrlCmd = new CtrlPlatformCommandInfo();
                    ctrlCmd.ControlInfo = ci;
                    ctrlCmd.MetaDataInfo = _newMdi;
                    icc.InputParameter(ctrlCmd);
                }
            }
            this._NotifyRuntimePage(ConstantCollection.COMPOSITE_CONTROL_INPUT_PARAMETER);
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            foreach (var ctr in _DictControl)
            {
                var icc = ctr.Value as ICompositeCtrl;
                if (null == icc)
                {
                    continue;
                }
                icc.Dispose();
            }
        }

        /// <summary>
        /// 处理返回回来的命令
        /// </summary>
        /// <param name="cmd">复合控件命令对象类</param>
        public void DoAction(CtrlPlatformCommandInfo cmd)
        {
            try
            {
                string mn = cmd.TempValue.GetAttributeValue(ConstantCollection.METHOD_NAME);
                string _MethodName = string.Format("Process{0}", mn);
                var mi = this.GetType().GetMethod(_MethodName, BindingFlags.Instance | BindingFlags.NonPublic);
                if (null == mi)
                {
                    _IPageRuntime.AlertMessage("没有找着【" + _MethodName + "】");
                    return;
                }
                mi.Invoke(this, new object[] { cmd });
            }
            catch (Exception ee)
            {
                _IPageRuntime.AlertMessage(ee);
            }
        }

        /// <summary>
        /// 通知运行时
        /// </summary>
        /// <param name="methodName"></param>
        void _NotifyRuntimePage(string methodName)
        {
            if (null != this._IPageRuntime)
            {
                this._IPageRuntime.CompositeControlNotifyRuntimePage(this.Name, methodName);
            }
        }
        /// <summary>
        /// 主键信息类
        /// </summary>
        class PrimaryKeyInfo
        {
            internal string Name { get; set; }
            internal string Value { get; set; }
            internal string DeleteSentence { get; set; }
        }
        /// <summary>
        /// 获取主键信息
        /// </summary>
        /// <param name="items"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        PrimaryKeyInfo GetPKeyInfo(List<MetaDataColumnInfo> items, string tableName)
        {
            var pki = new PrimaryKeyInfo();
            //-->获取出主键来
            var _key = items.Where(p => p.column_key.IsNotEmpty() && p.column_key.Equals(ConstantCollection.TABLE_KEY)).GetTList<MetaDataColumnInfo>();
            if (0 < _key.Count)
            {
                //-->获取where条件
                List<string> _whereCondition = new List<string>();
                List<string> _Fields = new List<string>();
                List<String> _Values = new List<string>();
                foreach (var pk in _key)
                {
                    _Fields.Add(pk.column_name);
                    _Values.Add(pk.column_value);
                    if (pk.data_type.ToLower().Equals("int") && pk.column_value == "0")
                    {
                        continue;
                    }
                    _whereCondition.Add(string.Format("{0} {1}", pk.column_name, Wrapper.ConvertToDBTypeWhereUsing(pk)));
                }
                if (_whereCondition.Count > 0)
                {
                    pki.DeleteSentence = string.Format("delete from {0} where {1}", tableName, string.Join(" and ", _whereCondition));
                }
                pki.Name = string.Join("|", _Fields);
                pki.Value = string.Join(ConstantCollection.Separator_Hollow_pentagram.ToString(), _Values);
            }
            return pki;
        }
        /// <summary>
        /// 获取保存SQL语句
        /// </summary>
        /// <param name="deleteSqlSendence"></param>
        /// <returns></returns>
        string GetSaveToDBSqlSentence(ref string deleteSqlSendence)
        {
            deleteSqlSendence = string.Empty;
            var oup = this.OutputParameter();
            //-->在这里生成Sql语句。
            string tableName = oup.MetaDataInfo.table_name;
            //-->找出主键值来。选删除，然后再插入。
            //-->找出PK来。
            var pki = GetPKeyInfo(oup.MetaDataInfo.Item, tableName);
            var _NotSaveField = new List<String>();
            if (pki.Name.IsNotEmpty())
            {
                _NotSaveField.AddRange(pki.Name.Split('|'));
            }
            if (null != pki || pki.DeleteSentence.IsNotEmpty())
            {
                deleteSqlSendence = pki.DeleteSentence;
            }
            List<String> Fields = new List<String>();
            List<String> Values = new List<String>();
            foreach (var mdci in oup.MetaDataInfo.Item)
            {
                if (pki.DeleteSentence.IsNullOrEmpty())   //-->新增的时候，主键值不能添加
                {
                    var isExists = _NotSaveField.Where(p => p.Equals(mdci.column_name));
                    if (0 < isExists.Count())
                    {
                        continue;
                    }
                }
                Fields.Add(mdci.column_name);
                Values.Add(string.Format("{0}", Wrapper.ConvertToDBType(mdci)));
            }
            string insertSql = string.Format("insert into {0}({1})values({2}) ", tableName, string.Join(",", Fields), string.Join(",", Values));
            return insertSql;
        }

        /// <summary>
        /// 保存数据操作
        /// </summary>
        /// <param name="cmd"></param>
        public void SaveToDB(CtrlPlatformCommandInfo cmd)
        {
            string deleteSql = string.Empty;
            string insertSql = GetSaveToDBSqlSentence(ref deleteSql);
            cmd = new CtrlPlatformCommandInfo();
            cmd.CommandName = ConstantCollection.CommandName_MixedCommand;
            cmd.CompositeControlName = this.Name;

            cmd.ExecSql = new List<CtrlExecSQLCmd>();
            var exec = new CtrlExecSQLCmd(CtrlExecSqlCmdType.ExecuteNonQuery, deleteSql, _MetaDataInfo.table_name, this.Name, ConstantCollection.COMPOSITE_CONTROL_SAVE_TO_DB);
            cmd.ExecSql.Add(exec);
            exec = new CtrlExecSQLCmd(CtrlExecSqlCmdType.ExecuteNonQuery, insertSql, _MetaDataInfo.table_name, this.Name, ConstantCollection.COMPOSITE_CONTROL_SAVE_TO_DB);
            cmd.ExecSql.Add(exec);

            cmd.TempValue = new XElement("execute", new XAttribute(ConstantCollection.METHOD_NAME, ConstantCollection.COMPOSITE_CONTROL_SAVE_TO_DB));
            this._IPageRuntime.SendCommand(cmd);
            this._NotifyRuntimePage(ConstantCollection.COMPOSITE_CONTROL_SAVE_TO_DB);
        }
        /// <summary>
        /// 处理操作结果
        /// </summary>
        /// <param name="cmd"></param>
        void ProcessSaveToDB(CtrlPlatformCommandInfo cmd)
        {
            if (cmd.ExecuteNonQueryResult.IsNullOrEmpty())
            {
                this._NotifyRuntimePage(ConstantCollection.COMPOSITE_CONTROL_SAVE_TO_DB_SUCCESS);
            }
            else
            {
                this._IPageRuntime.AlertMessage("保存失败，原因：" + cmd.ExecuteNonQueryResult);
                this._NotifyRuntimePage(ConstantCollection.COMPOSITE_CONTROL_SAVE_TO_DB_FAILURE);
            }
        }

        /// <summary>
        /// 删除操作
        /// </summary>
        /// <param name="cmd"></param>
        public void Delete(CtrlPlatformCommandInfo cmd)
        {
            //-->在这里生成Sql语句。
            string tableName = _MetaDataInfo.table_name;
            var op = this.OutputParameter();
            //-->找出PK来。
            var pkItem = op.MetaDataInfo.Item.Where(p =>
                                                    !p.column_key.IsNullOrEmpty() &&
                                                    p.column_key.Equals(ConstantCollection.TABLE_KEY)
                                                    ).GetTList<MetaDataColumnInfo>();
            List<String> whereItem = new List<string>();
            if (pkItem.Count > 0)
            {
                foreach (var pk in pkItem)
                {
                    string where = string.Format(" {0} {1} ", pk.column_name, Wrapper.ConvertToDBTypeWhereUsing(pk));
                    whereItem.Add(where);
                }
            }
            else
            {
                foreach (var pk in op.MetaDataInfo.Item)
                {
                    string where = string.Format(" {0} {1} ", pk.column_name, Wrapper.ConvertToDBTypeWhereUsing(pk));
                    whereItem.Add(where);
                }
            }
            if (whereItem.Count == 0)
            {
                return;
            }
            string deleteSql = string.Format("delete from {0} where {1}", tableName, string.Join(" and ", whereItem));

            cmd = new CtrlPlatformCommandInfo();
            cmd.CommandName = ConstantCollection.CommandName_MixedCommand;
            cmd.CompositeControlName = this.Name;

            cmd.ExecSql = new List<CtrlExecSQLCmd>();
            var exec = new CtrlExecSQLCmd(CtrlExecSqlCmdType.ExecuteNonQuery, deleteSql, tableName, this.Name, ConstantCollection.COMPOSITE_CONTROL_DELETE);
            cmd.ExecSql.Add(exec);
            cmd.TempValue = new XElement("execute", new XAttribute(ConstantCollection.METHOD_NAME, ConstantCollection.COMPOSITE_CONTROL_DELETE));

            this._IPageRuntime.SendCommand(cmd);
            this._NotifyRuntimePage(ConstantCollection.COMPOSITE_CONTROL_DELETE);
        }

        void ProcessDelete(CtrlPlatformCommandInfo cmd)
        {
            if (cmd.ExecuteNonQueryResult.IsNullOrEmpty())
            {
                this._NotifyRuntimePage(ConstantCollection.COMPOSITE_CONTROL_DELETE_SUCCESS);
            }
            else
            {
                this._IPageRuntime.AlertMessage("删除失败，原因：" + cmd.ExecuteNonQueryResult);
                this._NotifyRuntimePage(ConstantCollection.COMPOSITE_CONTROL_DELETE_FAILURE);
            }
        }

        /// <summary>
        /// 清空值操作
        /// </summary>
        public void ClearCtrlValue()
        {
            foreach (var ctr in _DictControl)
            {
                var icc = ctr.Value as ICompositeCtrl;
                if (null == icc)
                {
                    continue;
                }
                icc.ClearCtrlValue();
            }
            this._NotifyRuntimePage(ConstantCollection.COMPOSITE_CONTROL_CLEAR_VALUE);
        }

        /// <summary>
        /// 刷新操作
        /// </summary>
        /// <param name="cmd"></param>
        public void Refresh(CtrlPlatformCommandInfo cmd)
        {
            InitLoad(_InitLoadParameter);
        }

        void ProcessRefresh(CtrlPlatformCommandInfo cmd)
        {
            this._NotifyRuntimePage(ConstantCollection.COMPOSITE_CONTROL_REFRESH_COMPLATE);
        }
    }
}
