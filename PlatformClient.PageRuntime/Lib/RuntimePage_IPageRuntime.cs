using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Xml.Linq;
using PlatformClient.Common;
using PlatformClient.Common.Events;
using PlatformClient.Common.Interfaces;
using PlatformClient.Extend.Core;
using PlatformClient.Model.Method;
using PlatformClient.Model.Table;
using PlatformClient.Utility;

namespace PlatformClient.PageRuntime
{
    /// <summary>
    /// 运行时页面
    /// </summary>
    public partial class RuntimePage : IPageRuntime
    {
        /// <summary>
        /// 使用之前必须清空【.Clear()】,且必须和IsBeginInitControl一起使用。
        /// </summary>
        List<CtrlPlatformCommandInfo> CommandItem = new List<CtrlPlatformCommandInfo>();
        /// <summary>
        /// 目录下节点结构
        /// </summary>
        public PageDirectorySub PageInfo { get; set; }

        /// <summary>
        /// 查找控件
        /// </summary>
        /// <param name="controlName"></param>
        /// <returns></returns>
        public FrameworkElement FindControl(string controlName)
        {
            if (controlName.IsNullOrEmpty())
            {
                return null;
            }
            if (null == _DictControl || 0 == _DictControl.Count || !_DictControl.ContainsKey(controlName))
            {
                var fc = Wrapper.FindControl(this.LayoutRoot, controlName);
                if (null != fc)
                {
                    _DictControl.Add(controlName, fc);
                }
                return fc;
            }
            if (_DictControl.ContainsKey(controlName))
            {
                return _DictControl[controlName];
            }
            return null;
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            try
            {
                foreach (var KeyValue in _DictControl)
                {
                    if (!(KeyValue.Value is ICompositeCtrl))
                    {
                        continue;
                    }
                    var icc = KeyValue.Value as ICompositeCtrl;
                    try
                    {
                        icc.Dispose();
                    }
                    catch { }
                }
            }
            catch { }
        }

        /// <summary>
        /// 获取全局变量信息
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public GlobalVariableInfo GetGlobalVariablesByName(string name)
        {
            return null;
        }

        /// <summary>
        /// 复合控件通知运行方法
        /// </summary>
        /// <param name="controlName"></param>
        /// <param name="methodName"></param>
        public void CompositeControlNotifyRuntimePage(string controlName, string methodName)
        {
            if (controlName.IsNullOrEmpty() || methodName.IsNullOrEmpty())
            {
                return;
            }
            //-->查找方法
            var eli = _XmlTemplate.EventLinkItem.Where(p =>
                                p.ControlName.Equals(controlName) &&
                                p.EventName.Equals(methodName)
                                ).GetFirst<EventLinkInfo>();
            ExecuteInvoke(eli, controlName);
        }

        void ExecuteInvoke(EventLinkInfo eli, string controlName)
        {
            if (null == eli || null == eli.Item || 0 == eli.Item.Count)
            {
                return;
            }
            foreach (var child in eli.Item)
            {
                var _cpci = new CtrlPlatformCommandInfo();
                switch (child.EventName.ToLower())
                {
                    case "popupform":
                        _cpci.CompositeControlName = controlName;
                        _cpci.PageGuid = child.PageGuid;
                        PopupForm(_cpci);
                        break;
                    case "closeform":
                        _cpci.CompositeControlName = controlName;
                        _cpci.PageGuid = child.PageGuid;
                        CloseForm(_cpci);
                        break;
                    case "switchform":
                        _cpci.CompositeControlName = controlName;
                        _cpci.PageGuid = child.PageGuid;
                        SwitchForm(_cpci);
                        break;
                    case "refreshform":
                        RefreshForm();
                        break;
                    default:
                        var icc = this.FindControl(child.ControlName);
                        if (null == icc)
                        {
                            break;
                        }
                        var mi = icc.GetType().GetMethod(child.EventName);
                        if (null == mi)
                        {
                            break;
                        }
                        if (mi.GetParameters().Length > 0)
                        {
                            _cpci = GetCtrlPlatformCommandInfo(child, controlName);
                            mi.Invoke(icc, new object[] { _cpci });
                        }
                        else
                        {
                            mi.Invoke(icc, null);
                        }
                        break;
                }
            }
        }

        CtrlPlatformCommandInfo GetCtrlPlatformCommandInfo(ControlActivityInfo child, string controlName)
        {
            CtrlPlatformCommandInfo cmd = new CtrlPlatformCommandInfo();
            if (null == child.Param || 0 == child.Param.Count)
            {
                var ci = GetControlInfoByControlName(controlName);
                var mdi = GetMetaDataInfoByTableName(ci.MetaData);
                if (null == mdi || null == mdi.Item || 0 == mdi.Item.Count || ci.ColumnName.IsNullOrEmpty())
                {
                    return new CtrlPlatformCommandInfo();
                }
                var mdci = mdi.Item.Where(p => p.column_name.Equals(ci.ColumnName)).GetFirst<MetaDataColumnInfo>();

                MetaDataInfo _newCtrlMetaData = new MetaDataInfo();
                _newCtrlMetaData.table_type = mdi.table_type;
                _newCtrlMetaData.table_name = mdi.table_name;
                _newCtrlMetaData.table_comment = mdi.table_comment;
                _newCtrlMetaData.Item = new List<MetaDataColumnInfo>();
                _newCtrlMetaData.Item.Add(mdci);

                cmd.MetaDataInfo = _newCtrlMetaData;
                cmd.ControlInfo = ci;
            }
            else
            {
                cmd.MetaDataInfo = new MetaDataInfo();
                cmd.MetaDataInfo.Item = new List<MetaDataColumnInfo>();
                //-->获取控件信息
                var ci = GetControlInfoByControlName(child.ControlName);
                Dictionary<String, CtrlPlatformCommandInfo> _DictCtrlParam = new Dictionary<string, CtrlPlatformCommandInfo>();
                foreach (var item in child.Param)
                {
                    var _newMdci = new MetaDataColumnInfo();
                    //-->表的名称
                    string tableName = item.OrgIsDataSource ? ci.ControlDataSourceTableName : ci.MetaData;
                    var mdi = GetMetaDataInfoByTableName(tableName);
                    //-->列的名称
                    string columnName = item.OrgFieldName;
                    //if (item.OrgIsDataSource &&
                    //    !item.OrgFieldName.Equals(ci.ColumnName) &&
                    //    ci.ColumnName.Split('|').Length == 1
                    //    )
                    //{
                    //    columnName = ci.ColumnName;
                    //}
                    var mdci = mdi.Item.Where(p => p.column_name.Equals(columnName)).GetFirst<MetaDataColumnInfo>();
                    if (null == mdci)
                    {
                        continue;
                    }
                    //--------------上面有问题----------
                    _newMdci = mdci.Clone<MetaDataColumnInfo>();

                    CtrlPlatformCommandInfo op = null;
                    if (!_DictCtrlParam.ContainsKey(item.TargetCtrlName))
                    {
                        var icc = this.FindControl(item.TargetCtrlName) as ICompositeCtrl;
                        if (null == icc)
                        {
                            continue;
                        }
                        op = icc.OutputParameter();
                        _DictCtrlParam.Add(item.TargetCtrlName, op);
                    }
                    else
                    {
                        op = _DictCtrlParam[item.TargetCtrlName];
                    }
                    if (null == op || null == op.MetaDataInfo || null == op.MetaDataInfo.Item || 0 == op.MetaDataInfo.Item.Count)
                    {
                        continue;
                    }
                    var _mdci = op.MetaDataInfo.Item.Where(p => p.column_name.IsNotEmpty() &&
                                                                p.column_name.ToLower().Equals(item.TargetFieldName.ToLower())
                                                                ).GetFirst<MetaDataColumnInfo>();
                    if (null == _mdci)
                    {
                        continue;
                    }
                    _newMdci.column_value = _mdci.column_value;
                    _newMdci.table_comment = mdi.table_comment;
                    _newMdci.table_name = mdi.table_name;
                    _newMdci.table_type = mdi.table_type;
                    cmd.MetaDataInfo.Item.Add(_newMdci);
                }
                cmd.ControlInfo = ci;
            }
            return cmd;
        }

        /// <summary>
        /// 弹出消息
        /// </summary>
        /// <param name="message"></param>
        public void AlertMessage(string message)
        {
            if (message.IsNotEmpty())
            {
                Wrapper.ShowDialog(message);
            }
        }

        /// <summary>
        /// 弹出异常消息
        /// </summary>
        /// <param name="ee"></param>
        public void AlertMessage(Exception ee)
        {
            Wrapper.ShowDialog(ee.Message);
        }

        /// <summary>
        ///  发送命令
        /// </summary>
        /// <param name="ctrlCmd"></param>
        public void SendCommand(CtrlPlatformCommandInfo ctrlCmd)
        {
            if (ctrlCmd.CompositeControlName.IsNullOrEmpty())
            {
                AlertMessage("复合控件命令中：复合控件名称参数为空，这样导致复合收到命令");
                return;
            }

            if (IsBeginInitControl)
            {
                CommandItem.Add(ctrlCmd);
            }
            else
            {
                ctrlCmd.TempValue.Add(new XAttribute(ConstantCollection.COMPOSITE_CONTROL_NAME, ctrlCmd.CompositeControlName));
                EventSubscribe.SendCommandMessageMethod(this, new SendCommandMessageEventArgs(ctrlCmd, ctrlCmd));
            }
        }

        /// <summary>
        /// 加载初始化控件
        /// </summary>
        void LoadInitControl()
        {
            if (null == _XmlTemplate || null == _XmlTemplate.PageLoadingItem || 0 == _XmlTemplate.PageLoadingItem.Count)
            {
                return;
            }

            IsBeginInitControl = true;
            CommandItem.Clear();

            foreach (var pl in _XmlTemplate.PageLoadingItem)
            {
                if (pl.CallFunctionName.IsNullOrEmpty() || pl.ControlName.IsNullOrEmpty())
                {
                    continue;
                }
                FrameworkElement control = this.FindControl(pl.ControlName);
                if (null == control)
                {
                    continue;
                }
                var mi = control.GetType().GetMethod(pl.CallFunctionName);
                if (null == mi)
                {
                    continue;
                }
                if (pl.CallFunctionName.Equals(ConstantCollection.COMPOSITE_CONTROL_INIT_LOAD))
                {
                    object param = control.Tag;
                    if (!(param is CtrlPlatformCommandInfo))
                    {
                        //-->开始查找信息
                        var ci = GetControlInfoByControlName(pl.ControlName);
                        //-->查找 metadata 里 row里的信息
                        var mdi = GetMetaDataInfoByTableName(ci.MetaData);
                        if (null == mdi || null == mdi.Item || 0 == mdi.Item.Count)
                        {
                            continue;
                        }
                        //--> metaData Column
                        var mdci = mdi.Item.Where(p => p.column_name.Equals(ci.ColumnName)).GetFirst<MetaDataColumnInfo>();
                        if (null == mdci)
                        {
                            continue;
                        }
                        var _newMdi = new MetaDataInfo();
                        _newMdi.table_type = mdi.table_type;
                        _newMdi.table_name = mdi.table_name;
                        _newMdi.table_comment = mdi.table_comment;
                        _newMdi.Item = new List<MetaDataColumnInfo>();
                        _newMdi.Item.Add(mdci);

                        CtrlPlatformCommandInfo ctrlCmd = new CtrlPlatformCommandInfo();
                        ctrlCmd.MetaDataInfo = _newMdi;
                        ctrlCmd.ControlInfo = ci;
                        param = ctrlCmd;
                    }
                    mi.Invoke(control, new object[] { param });
                }//End init function;
            }//End foreach;

            BatchSendCommand();
        }

        /// <summary>
        /// 指发送命令
        /// </summary>
        void BatchSendCommand()
        {
            if (null == CommandItem || 0 == CommandItem.Count)
            {
                this.IsBeginInitControl = false;
                return;
            }
            var cmd = new CtrlPlatformCommandInfo();
            cmd.CommandName = ConstantCollection.BATCH_COMMAND;
            cmd.CommandType = CtrlCommandDirectionType.ClientUI;
            cmd.FromModel = CtrlCommandDirectionType.ClientUI;
            cmd.ExecSql = new List<CtrlExecSQLCmd>();
            Dictionary<string, XElement> dictCmd = new Dictionary<string, XElement>();
            //-->开发处理发命令。
            foreach (var item in CommandItem)
            {
                foreach (var exec in item.ExecSql)
                {
                    string methodName = item.TempValue.GetAttributeValue(ConstantCollection.METHOD_NAME);
                    if (dictCmd.ContainsKey(exec.Sql))
                    {
                        dictCmd[exec.Sql].Add(new XElement("item", new XAttribute(ConstantCollection.COMPOSITE_CONTROL_NAME, item.CompositeControlName),
                                                                  new XAttribute(ConstantCollection.METHOD_NAME, methodName),
                                                                  new XAttribute(ConstantCollection.COMMAND_NAME, item.CommandName),
                                                                  new XAttribute(ConstantCollection.TABLE_NAME, exec.TableName)
                                                                  ));
                    }
                    else
                    {
                        XElement xe = new XElement("sql", new XAttribute("Name", exec.Sql));
                        xe.Add(new XElement("item", new XAttribute(ConstantCollection.COMPOSITE_CONTROL_NAME, item.CompositeControlName),
                                                    new XAttribute(ConstantCollection.METHOD_NAME, methodName),
                                                    new XAttribute(ConstantCollection.COMMAND_NAME, item.CommandName),
                                                    new XAttribute(ConstantCollection.TABLE_NAME, exec.TableName)
                                                    ));
                        dictCmd.Add(exec.Sql, xe);
                    }
                }
                cmd.ExecSql.AddRange(item.ExecSql);
            }
            if (0 == dictCmd.Count)
            {
                return;
            }
            //cmd.ParamCollection = new XElement("Batch", dictCmd.Values);
            cmd.TempValue = new XElement("query", new XAttribute(ConstantCollection.METHOD_NAME, "BatchSendCommand"));
            EventSubscribe.SendCommandMessageMethod(this, new SendCommandMessageEventArgs(CommandItem, cmd));

            CommandItem.Clear();
            IsBeginInitControl = false;
        }

        void ProcessBatchSendCommand(CtrlPlatformCommandInfo cmd)
        {
            var _DictCtrlCmd = new Dictionary<string, CtrlPlatformCommandInfo>();
            foreach (var exec in cmd.ExecSql)
            {
                string key = exec.CtrlName;
                if (_DictCtrlCmd.ContainsKey(key))
                {
                    _DictCtrlCmd[key].ExecSql.Add(exec);
                }
                else
                {
                    var ctrlCmd = new CtrlPlatformCommandInfo();
                    ctrlCmd.CompositeControlName = exec.CtrlName;
                    ctrlCmd.TempValue = new XElement("ctrl", new XAttribute(ConstantCollection.METHOD_NAME, exec.MethodName));
                    ctrlCmd.ExecSql = new List<CtrlExecSQLCmd>();
                    ctrlCmd.ExecSql.Add(exec);
                    _DictCtrlCmd.Add(exec.CtrlName, ctrlCmd);
                }
            }

            //--> DoAction
            foreach (var child in _DictCtrlCmd)
            {
                var icc = this.FindControl(child.Key) as ICompositeCtrl;
                if (null == icc)
                {
                    continue;
                }
                icc.DoAction(child.Value);
            }
        }

        /// <summary>
        /// 获取控件信息
        /// </summary>
        /// <param name="controlName">控件名称</param>
        /// <returns>返回控件信息</returns>
        public ControlInfo GetControlInfoByControlName(string controlName)
        {
            if (controlName.IsNullOrEmpty() || null == _XmlTemplate || null == _XmlTemplate.MetaDataItem || 0 == _XmlTemplate.MetaDataItem.Count)
            {
                return null;
            }
            return _XmlTemplate.ControlItem.Where(p => p.Name.Equals(controlName)).GetFirst<ControlInfo>();
        }

        /// <summary>
        /// 获取MetaData信息
        /// </summary>
        /// <param name="tableName">表的名称</param>
        /// <returns>返回一个MetaData对象</returns>
        public MetaDataInfo GetMetaDataInfoByTableName(string tableName)
        {
            if (tableName.IsNullOrEmpty() || null == _XmlTemplate || null == _XmlTemplate.MetaDataItem || 0 == _XmlTemplate.MetaDataItem.Count)
            {
                return null;
            }
            return _XmlTemplate.MetaDataItem.Where(p => p.table_name.Equals(tableName)).GetFirst<MetaDataInfo>();
        }

        /// <summary>
        /// 事件链信息
        /// </summary>
        /// <param name="controlName">控件名称</param>
        /// <returns></returns>
        public EventLinkInfo GetEventLinkInfoByControlName(string controlName)
        {
            if (controlName.IsNullOrEmpty() || null == _XmlTemplate || null == _XmlTemplate.EventLinkItem || 0 == _XmlTemplate.EventLinkItem.Count)
            {
                return null;
            }
            return _XmlTemplate.EventLinkItem.Where(p => p.ControlName.Equals(controlName)).GetFirst<EventLinkInfo>();
        }

        /// <summary>
        /// 事件绑定绑定运行时界面
        /// </summary>
        /// <param name="controlName">控件名称</param>
        public void EventBindNotifiyRuntimePage(string controlName)
        {
            var eli = GetEventLinkInfoByControlName(controlName);
            //-->如果以后这里要发送多个命令的时候，可以考虑，将命令截取出来然后统一一起发送到服务器端去。
            ExecuteInvoke(eli, controlName);
        }

        /// <summary>
        /// 设置属性的值
        /// </summary>
        /// <param name="propertyValue">属性名称</param>
        /// <param name="value">属性值</param>
        public void SetProperty(string propertyValue, object value)
        {
            this.SetPropertyValue(propertyValue, value);
        }

        /// <summary>
        /// 获取属性值
        /// </summary>
        /// <param name="propertyName">属性名称</param>
        /// <returns>返回属性值</returns>
        public object GetProperty(string propertyName)
        {
            return GetProperty(propertyName);
        }
    }
}
