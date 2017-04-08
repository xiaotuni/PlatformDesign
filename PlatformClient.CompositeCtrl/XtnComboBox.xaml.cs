using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Controls;
using System.Xml.Linq;
using PlatformClient.Common;
using PlatformClient.Common.Interfaces;
using PlatformClient.Model.Method;
using PlatformClient.Utility;
using PlatformClient.Extend.Core;

namespace PlatformClient.CompositeCtrl
{
    /// <summary>
    /// 下拉框控件
    /// </summary>
    public partial class XtnComboBox : UserControl, ICompositeCtrl
    {
        ControlInfo _ControlInfo;
        MetaDataInfo _MetaDataInfo;
        /// <summary>
        /// 
        /// </summary>
        IPageRuntime _IPageRuntime;
        /// <summary>
        /// 
        /// </summary>
        DataSourceInfo _LinkageInfo;
        List<object> _ItemsSource;
        CtrlPlatformCommandInfo _ConditionByWhere;
        /// <summary>
        /// 是否发送命令
        /// </summary>
        bool IsSendCommand;
        private CtrlPlatformCommandInfo _InitLoadParameter;
        /// <summary>
        ///宽度 
        /// </summary>
        public double TitleWidth { get { return this.txtContent.Width; } set { this.txtContent.Width = value; } }

        /// <summary>
        /// 文件框标题
        /// </summary>
        [PropertyInfoAttribute("", "文件框标题")]
        public object TextContent { get { return this.txtContent.Content; } set { this.txtContent.Content = value; } }

        /// <summary>
        /// 数据源设置
        /// </summary>
        [PropertyInfoAttribute("PlatformClient.PageDesignTime.Controls;PlatformClient.PageDesignTime.Controls.XtnSetComboBoxDataSource", "数据源设置")]
        public string DataSource { get; set; }

        /// <summary>
        /// 条件
        /// </summary>
        [PropertyInfoAttribute("", "初条件")]
        public string WhereCondition { get; set; }

        ///// <summary>
        ///// 联动控件,上一个控件,此方法基本上用不上了。
        ///// </summary>
        //[PropertyInfoAttribute("PlatformClient.PageDesignTime.Controls;PlatformClient.PageDesignTime.Controls.LinkageControl", "联动控件")]
        //public string LinkageControl { get; set; }

        /// <summary>
        /// 获取关联信息
        /// </summary>
        internal DataSourceInfo SourceInfo
        {
            get
            {
                if (null == _LinkageInfo)
                {
                    _LinkageInfo = GetDataSourceInfoInfo(this.DataSource);
                }
                return _LinkageInfo;
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public XtnComboBox()
        {
            InitializeComponent();
            this.cbbDropDownList.SelectionChanged += cbbDropDownList_SelectionChanged;
        }

        void cbbDropDownList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _NotifyRuntimePage(ConstantCollection.COMPOSITE_CONTROL_SELECTION_CHANGED);
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
        /// 
        /// </summary>
        /// <param name="iRuntime"></param>
        public void SetPageRuntimeInterface(IPageRuntime iRuntime)
        {
            _IPageRuntime = iRuntime;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public object GetProperty(string propertyName)
        {
            return this.GetPropertyValue(propertyName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="propertyValue"></param>
        public void SetProperty(string propertyName, object propertyValue)
        {
            this.SetPropertyValue(propertyName, propertyValue);
        }

        /// <summary>
        /// 初始化操作
        /// </summary>
        /// <param name="cmd">复合控件命令对象类</param>
        public void InitLoad(CtrlPlatformCommandInfo cmd)
        {
            _InitLoadParameter = cmd;
            if (IsSendCommand)
            {
                return;
            }
            string sqlSentence = GetSqlSentence(cmd);
            if (sqlSentence.IsNullOrEmpty())
            {
                return;
            }

            var newCmd = new CtrlPlatformCommandInfo();
            newCmd.CompositeControlName = this.Name;
            newCmd.CommandName = ConstantCollection.CommandName_MixedCommand;

            newCmd.ExecSql = new List<CtrlExecSQLCmd>();
            var exec = new CtrlExecSQLCmd(CtrlExecSqlCmdType.Query, sqlSentence, this.SourceInfo.TableName, this.Name, "InitLoadData");
            newCmd.ExecSql.Add(exec);

            newCmd.TempValue = new XElement("Query", new XAttribute(ConstantCollection.METHOD_NAME, "InitLoadData"));

            this._IPageRuntime.SendCommand(newCmd);
            IsSendCommand = true;
        }

        void UpdateComboBoxItemsSource(CtrlPlatformCommandInfo cmd)
        {
            this.cbbDropDownList.ItemsSource = null;
            if (null == _ItemsSource || 0 == _ItemsSource.Count)
            {
                return;
            }
            if (null != cmd && null != cmd.MetaDataInfo && null != cmd.MetaDataInfo.Item && 0 < cmd.MetaDataInfo.Item.Count)
            {

                var pItem = cmd.MetaDataInfo.Item.Where(p =>
                                                        p.table_name.IsNotEmpty() &&
                                                        p.table_name.Equals(this.SourceInfo.TableName)
                                                        ).GetTList<MetaDataColumnInfo>();
                List<Object> _tempSource = new List<object>();
                foreach (var source in _ItemsSource)
                {
                    bool isAdd = true;
                    foreach (var where in pItem)
                    {
                        var pv = source.GetPropertyValue(where.column_name.ToLower());
                        if (null == pv || !pv.ToString().Equals(where.column_value))
                        {
                            isAdd = false;
                            break;
                        }
                    }
                    if (isAdd)
                    {
                        _tempSource.Add(source);
                    }
                }
                if (0 < _tempSource.Count)
                {
                    this.cbbDropDownList.ItemsSource = _tempSource;
                }
            }
            else
            {
                this.cbbDropDownList.ItemsSource = _ItemsSource;
            }
            if (this.cbbDropDownList.Items.Count > 0)
            {
                this.cbbDropDownList.DisplayMemberPath = this.SourceInfo.BindField.ToLower();
                this.cbbDropDownList.SelectedIndex = 0;
            }
        }

        string GetSqlSentence(CtrlPlatformCommandInfo cmd)
        {
            var si = this.SourceInfo;
            if (null == si)
            {
                _IPageRuntime.AlertMessage("数据源设置不正确");
                return null;
            }
            string querySql = string.Format("select * from {0} ", si.TableName);
            List<String> condition = new List<string>();
            if (!WhereCondition.IsNullOrEmpty())
            {
                condition.Add(WhereCondition);
            }
            if (null != cmd && null != cmd.MetaDataInfo && null != cmd.MetaDataInfo.Item && 0 < cmd.MetaDataInfo.Item.Count)
            {
                var pItem = cmd.MetaDataInfo.Item.Where(p =>
                                    !p.table_name.IsNullOrEmpty() &&
                                    p.table_name.Equals(si.TableName)
                                    ).GetTList<MetaDataColumnInfo>();
                if (null != pItem && 0 < pItem.Count)
                {
                    var _where = from p in pItem
                                 select string.Format("{0} = '{1}'", p.column_name, p.column_value);
                    condition.AddRange(_where);
                }
            }
            if (0 < condition.Count)
            {
                querySql += string.Format(" where {0} ", string.Join(" and ", condition));
            }
            return querySql;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmd">复合控件命令对象类</param>
        public void InitTitle(CtrlPlatformCommandInfo cmd)
        {
            _ControlInfo = cmd.ControlInfo;
            _MetaDataInfo = cmd.MetaDataInfo;
            if (!_ControlInfo.Comment.IsNullOrEmpty())
            {
                this.TextContent = _ControlInfo.Comment;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public CtrlPlatformCommandInfo OutputParameter()
        {
            var _value = string.Format("{0}", this.cbbDropDownList.SelectedItem.GetPropertyValue(this.SourceInfo.SaveField.ToLower()));
            if (_value.IsNullOrEmpty())
            {
                return null;
            }
            //-->这个说是用户从控件上添加进来的，所以没有列。
            if (_MetaDataInfo.Item.Count == 0)
            {
                _MetaDataInfo.Item = new List<MetaDataColumnInfo>();
                var col = new MetaDataColumnInfo();
                col.column_value = _value;
                col.column_comment = string.Format("{0}", TextContent);
                col.column_name = SourceInfo.SaveField;
                col.table_name = SourceInfo.TableName;
                _MetaDataInfo.Item.Add(col);
            }
            else
            {
                var mdci = _MetaDataInfo.Item[0];
                mdci.column_value = _value;
            }
            CtrlPlatformCommandInfo cmd = new CtrlPlatformCommandInfo();
            cmd.ParamCollection = _MetaDataInfo.ToXElement();
            cmd.MetaDataInfo = new MetaDataInfo();
            cmd.MetaDataInfo = _MetaDataInfo;
            cmd.ControlInfo = _ControlInfo;

            return cmd;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmd">复合控件命令对象类</param>
        public void InputParameter(CtrlPlatformCommandInfo cmd)
        {
            if (null == cmd || null == cmd.MetaDataInfo || null == cmd.MetaDataInfo.Item || 0 == cmd.MetaDataInfo.Item.Count || null == this.cbbDropDownList.ItemsSource)
            {
                return;
            }
            string _saveField = this.SourceInfo.SaveField.ToLower();
            string column_value = cmd.MetaDataInfo.Item[0].column_value;
            foreach (var child in this.cbbDropDownList.ItemsSource)
            {
                var value = child.GetPropertyValue(_saveField);
                if (string.Format("{0}", value).Equals(column_value))
                {
                    this.cbbDropDownList.SelectedItem = child;
                    return;
                }
            }
        }

        /// <summary>
        /// 联动更改下拉框数据
        /// </summary>
        /// <param name="cmd">复合控件命令对象类</param>
        public void ChangeComboBoxValue(CtrlPlatformCommandInfo cmd)
        {
            if (null == _ItemsSource || 0 == _ItemsSource.Count)
            {
                _ConditionByWhere = cmd;
                InitLoad(new CtrlPlatformCommandInfo());
                return;
            }
            UpdateComboBoxItemsSource(cmd);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            this.cbbDropDownList.ItemsSource = null;
            this.cbbDropDownList.SelectionChanged -= cbbDropDownList_SelectionChanged;
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
        /// 填充数据
        /// </summary>
        /// <param name="cmd"></param>
        void ProcessInitLoadData(CtrlPlatformCommandInfo cmd)
        {
            _ItemsSource = DynamicallyGeneratedClass.ToList(cmd.ExecSql[0].Result);
            UpdateComboBoxItemsSource(cmd);
            if (null != _ConditionByWhere)
            {
                UpdateComboBoxItemsSource(_ConditionByWhere);
            }
        }

        /// <summary>
        /// 获数据源信息
        /// </summary>
        /// <param name="dataSource"></param>
        /// <returns></returns>
        DataSourceInfo GetDataSourceInfoInfo(string dataSource)
        {
            if (dataSource.IsNullOrEmpty())
            {
                return null;
            }
            //Table;{0}|SaveField;{1}|BindField;{2}
            var arr = dataSource.Split('|');
            if (3 != arr.Length)
            {
                return null;
            }
            DataSourceInfo lci = new DataSourceInfo();
            lci.TableName = arr[0];
            lci.SaveField = arr[1];
            lci.BindField = arr[2];

            return lci;
        }

        /// <summary>
        /// 清空值操作
        /// </summary>
        public void ClearCtrlValue()
        {
            cbbDropDownList.SelectedIndex = -1;
            this._NotifyRuntimePage(ConstantCollection.COMPOSITE_CONTROL_CLEAR_VALUE);
        }

        /// <summary>
        /// 刷新操作
        /// </summary>
        public void Refresh(CtrlPlatformCommandInfo cmd)
        {
            InitLoad(_InitLoadParameter);
        }

        /// <summary>
        /// 联动控件信息类
        /// </summary>
        internal class DataSourceInfo
        {
            /// <summary>
            /// 表的名称
            /// </summary>
            public string TableName { get; set; }
            /// <summary>
            /// 绑定字段
            /// </summary>
            public string BindField { get; set; }
            /// <summary>
            /// 保存字段
            /// </summary>
            public string SaveField { get; set; }
        }
    }
}
