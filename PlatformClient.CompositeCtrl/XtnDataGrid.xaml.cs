using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Xml.Linq;
using PlatformClient.Common;
using PlatformClient.Common.Interfaces;
using PlatformClient.Extend.Core;
using PlatformClient.Model.Method;
using PlatformClient.Utility;

namespace PlatformClient.CompositeCtrl
{
    /// <summary>
    /// DataGrid数据控件
    /// </summary>
    public partial class XtnDataGrid : UserControl, ICompositeCtrl
    {
        IPageRuntime _IPageRuntime;
        string _DataSource;
        /// <summary>
        /// 表的名称
        /// </summary>
        string _TableName;
        /// <summary>
        /// 字段列表
        /// </summary>
        List<String> _FieldItem = new List<string>();
        /// <summary>
        /// 列的集合
        /// </summary>
        List<MetaDataColumnInfo> _Columns = new List<MetaDataColumnInfo>();
        /// <summary>
        /// 是否发送命令
        /// </summary>
        bool IsSendCommand;
        ///// <summary>
        ///// SQL语句
        ///// </summary>
        //string _SqlSentence;
        private CtrlPlatformCommandInfo _InitLoadParameter;
        private string _ConvertColumnDB;
        /// <summary>
        /// default 20
        /// </summary>
        private int _PageSize = 20;
        private CtrlPlatformCommandInfo _ConditionQueryParam;
        private int _PageCount;
        /// <summary>
        /// 标题的高度
        /// </summary>
        public double TitleWidth { get; set; }
        /// <summary>
        /// 数据源设置
        /// </summary>
        [PropertyInfoAttribute("PlatformClient.PageDesignTime.Controls;PlatformClient.PageDesignTime.Controls.XtnSetDataGridItemsSource", "数据源设置")]
        public string DataSource
        {
            get { return _DataSource; }
            set
            {
                _DataSource = value;
                InitDataGridColumnHeader();
            }
        }
        CtrlPlatformCommandInfo ctrlCmd = null;
        /// <summary>
        /// 页面大小 default = 20
        /// </summary>
        [PropertyInfoAttribute("", "页面大小")]
        public int PageSize { get { return _PageSize; } set { _PageSize = value; } }

        /// <summary>
        /// 表数据转换
        /// </summary>
        [PropertyInfoAttribute("PlatformClient.PageDesignTime.Controls;PlatformClient.PageDesignTime.Controls.XtnSetDataGridConvertColumnDB", "表数据转换")]
        public string ConvertColumnDB
        {
            get { return _ConvertColumnDB; }
            set
            {
                _ConvertColumnDB = value;
                InitDataGridColumnHeader();
            }
        }

        /// <summary>
        /// 条件
        /// </summary>
        [PropertyInfoAttribute("", "初条件")]
        public string WhereCondition { get; set; }
        /// <summary>
        /// 解析DataSource信息
        /// </summary>
        /// <param name="value"></param>
        void DecodeXml(string value)
        {
            if (value.IsNullOrEmpty())
            {
                return;
            }
            string _value = value;
            var _data = _value.Split(ConstantCollection.Separator_Sub_section_number);
            _TableName = _data[0];
            if (_data[1].IsNullOrEmpty())
            {
                return;
            }
            var _fields = _data[1].Split(',');
            if (null == _fields || 0 == _fields.Length)
            {
                return;
            }
            _Columns.Clear();
            foreach (var field in _fields)
            {
                var _fi = field.Split(ConstantCollection.Separator_Wavy_line);
                var column = new MetaDataColumnInfo();
                column.data_type = _fi[2];
                column.column_name = _fi[1].ToLower();
                column.column_comment = _fi[0];
                _FieldItem.Add(column.column_name);
                _Columns.Add(column);
            }
            var rn = new MetaDataColumnInfo();
            rn.data_type = "int";
            rn.column_name = "rn";
            rn.column_comment = "序号";
            _FieldItem.Insert(0, rn.column_name);
            _Columns.Insert(0, rn);
        }
        /// <summary>
        /// 初始化DataGrid的列的Header
        /// </summary>
        /// <param name="columns"></param>
        void InitDataGridColumn(List<MetaDataColumnInfo> columns)
        {
            xtn_Data.Columns.Clear();
            if (null == columns || 0 == columns.Count)
            {
                return;
            }
            foreach (var child in columns)
            {
                var test = GetDataGridTextColumn(child);
                if (null == test)
                {
                    continue;
                }
                xtn_Data.Columns.Add(test);
            }
        }

        DataGridTextColumn GetDataGridTextColumn(MetaDataColumnInfo col)
        {
            if (null == col)
            {
                return null;
            }
            DataGridTextColumn text = new DataGridTextColumn();
            text.Header = col.column_comment;
            text.Binding = new Binding(col.column_name);
            switch (col.data_type.ToLower())
            {
                case "datetime":
                    text.Binding.StringFormat = "yyyy-MM-dd HH:mm:ss";//"D";"yyyy-MM-dd HH:mm:ss";
                    break;
                case "double":
                    text.Binding.StringFormat = "F2";
                    break;

            }
            return text;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public XtnDataGrid()
        {
            InitializeComponent();

            this._Columns.Clear();
            this._FieldItem.Clear();
            this._TableName = string.Empty;

            xtn_Data.SelectionChanged += xtn_Data_SelectionChanged;
            btnFirstPage.Click += btnFirstPage_Click;
            btnNextPage.Click += btnNextPage_Click;
            btnPreviousPage.Click += btnPreviousPage_Click;
            btnLastPage.Click += btnLastPage_Click;
        }

        void InitDataGridColumnHeader()
        {
            DecodeXml(this._DataSource);
            InitDataGridColumn(this._Columns);
        }

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
            return this.GetPropertyValue(propertyName);
        }

        /// <summary>
        /// 设置属性值
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="propertyValue"></param>
        public void SetProperty(string propertyName, object propertyValue)
        {
            this.SetPropertyValue(propertyName, propertyValue);
        }

        /// <summary>
        /// where 条件
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        string _GetCommandWhereCondition(CtrlPlatformCommandInfo cmd)
        {
            List<String> condition = new List<string>();
            if (!WhereCondition.IsNullOrEmpty())
            {
                condition.Add(WhereCondition);
            }
            if (null != cmd && null != cmd.MetaDataInfo && null != cmd.MetaDataInfo.Item && 0 < cmd.MetaDataInfo.Item.Count)
            {
                var _where = from p in cmd.MetaDataInfo.Item
                             where !p.column_name.IsNullOrEmpty() &&
                                   !p.table_name.IsNullOrEmpty() &&
                                   p.table_name.Equals(_TableName)
                             select string.Format("{0} {1}", p.column_name, Wrapper.ConvertToDBTypeWhereUsing(p));// p.column_value);
                condition.AddRange(_where);
            }
            if (0 < condition.Count)
            {
                return string.Format(" where {0} ", string.Join(" and ", condition));
            }
            return "";
        }

        /// <summary>
        /// sql语句
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="totalSql"></param>
        /// <returns></returns>
        string _BuilderSqlSentence(CtrlPlatformCommandInfo cmd, out string totalSql)
        {
            //-->这里是条件
            //string sql = string.Format("select {0} from {1} {2}", string.Join(",", _FieldItem), _TableName, _GetCommandWhereCondition(cmd));
            //select *,
            //(select country_name from country_info c where countryid = id) name2,
            //(select country_name from country_info c where cityid = id) name1
            //from user_info t

            List<String> convertItem = new List<string>();
            if (null != this._InitLoadParameter.ControlInfo.Convert)
            {
                foreach (var conver in this._InitLoadParameter.ControlInfo.Convert)
                {
                    var str = string.Format("(select {0} from {1} where {2} = {3}) {2}_{3} ",
                        conver.DisplayField,
                        conver.RelationTableName,
                        conver.RelationField,
                        conver.ColumnName
                        );
                    convertItem.Add(str);
                }
            }
            //string sql = string.Format("select * {0} from {1} {2}",
            //    convertItem.Count > 0 ? "," + string.Join(",", convertItem) : "",
            //    _TableName,
            //    _GetCommandWhereCondition(cmd));

            string _whereCondition = _GetCommandWhereCondition(cmd);
            int CurrentPageIndex = xtn_CurrentIndex.Text.ConvertTo<int>() - 1;// == null ? 0 : string.Format("{0}", this.xtn_CurrentIndex.SelectedValue).ConvertTo<int>();
            string sql = string.Format("select * from( select (@count := @count +1) rn,t.* {0} from {1} t {2} )_view limit {3},{4} ",
                convertItem.Count > 0 ? "," + string.Join(",", convertItem) : "",
                _TableName,
                _whereCondition,
                PageSize * (CurrentPageIndex),
                PageSize
                );

            totalSql = string.Format("select count(*) times from {0} t {1}",
                _TableName,
                _whereCondition
                );
            return sql;
        }
        /// <summary>
        /// 发送查询命令
        /// </summary>
        /// <param name="cmd">查询语句</param>
        /// <param name="commandName"></param>
        void SendQueryCommand(CtrlPlatformCommandInfo cmd, string commandName)
        {
            ctrlCmd = new CtrlPlatformCommandInfo();
            ctrlCmd.CompositeControlName = this.Name;
            ctrlCmd.CommandName = ConstantCollection.CommandName_MixedCommand;
            ctrlCmd.ExecSql = new List<CtrlExecSQLCmd>();
            var exec = new CtrlExecSQLCmd(CtrlExecSqlCmdType.ExecuteNonQuery, "set @count=0", _TableName, this.Name, commandName);
            ctrlCmd.ExecSql.Add(exec);

            string totalSql = string.Empty;
            string sql = _BuilderSqlSentence(cmd, out totalSql);
            exec = new CtrlExecSQLCmd(CtrlExecSqlCmdType.Query, sql, _TableName, this.Name, commandName);
            ctrlCmd.ExecSql.Add(exec);
            exec = new CtrlExecSQLCmd(CtrlExecSqlCmdType.ExecuteScalar, totalSql, _TableName, this.Name, commandName);
            ctrlCmd.ExecSql.Add(exec);
            exec = new CtrlExecSQLCmd(CtrlExecSqlCmdType.ExecuteNonQuery, "set @count=0", _TableName, this.Name, commandName);
            ctrlCmd.ExecSql.Add(exec);
            ctrlCmd.TempValue = new XElement("Query", new XAttribute(ConstantCollection.METHOD_NAME, commandName));
            this._IPageRuntime.SendCommand(ctrlCmd);
        }

        /// <summary>
        /// 初始化操作
        /// </summary>
        /// <param name="cmd"></param>
        public void InitLoad(CtrlPlatformCommandInfo cmd)
        {
            _InitLoadParameter = cmd;

            if (IsSendCommand)
            {
                return;
            }
            SendQueryCommand(cmd, ConstantCollection.COMPOSITE_CONTROL_INIT_LOAD);
            IsSendCommand = true;
        }

        /// <summary>
        /// 填充数据
        /// </summary>
        /// <param name="cmd"></param>
        void ProcessInitLoad(CtrlPlatformCommandInfo cmd)
        {
            _FillDataGrid(cmd.ExecSql[1]);
            _FillTrunPage(cmd.ExecSql[2]);
        }

        void _FillDataGrid(CtrlExecSQLCmd ctrlExecSQLCmd)
        {
            var _columns = DynamicallyGeneratedClass.GetDataTableColumnCollection(ctrlExecSQLCmd.Result);
            if (null == _columns)
            {
                this.xtn_Data.ItemsSource = null;
                return;
            }
            if (_columns.Count != _Columns.Count && _columns.Count != this.xtn_Data.Columns.Count)
            {
                //-->这里要进行转换
                int i = 0;
                Dictionary<int, MetaDataColumnInfo> _DictData = new Dictionary<int, MetaDataColumnInfo>();
                foreach (var xtnCol in this.xtn_Data.Columns)
                {
                    var dgCol = xtnCol as DataGridTextColumn;
                    var path = dgCol.Binding.Path.Path;
                    var conver = _InitLoadParameter.ControlInfo.Convert.Where(p => p.ColumnName.ToLower().Equals(path.ToLower())).GetFirst<ColumnDBConvertInfo>();
                    i++;
                    if (null == conver)
                    {
                        continue;
                    }
                    dgCol.Visibility = System.Windows.Visibility.Collapsed;
                    MetaDataColumnInfo col = new MetaDataColumnInfo();
                    col.column_name = string.Format("{0}_{1}", conver.RelationField, conver.ColumnName).ToLower();// conver.DisplayField;
                    col.column_comment = dgCol.Header.ToString();
                    col.data_type = "varchar";
                    _DictData.Add(i, col);
                    i++;
                }
                foreach (var dict in _DictData)
                {
                    var test = this.GetDataGridTextColumn(dict.Value);
                    if (null == test)
                    {
                        continue;
                    }
                    this.xtn_Data.Columns.Insert(dict.Key, test);
                }
            }
            var items = DynamicallyGeneratedClass.ToList(ctrlExecSQLCmd.Result, _columns);
            this.xtn_Data.ItemsSource = items;
        }

        void _FillTrunPage(CtrlExecSQLCmd cmd)
        {
            if (null == cmd.Result)
            {
                return;
            }
            int total = cmd.Result.Value.ConvertTo<int>();
            //-->PageCount
            var _p = total % PageSize;
            _PageCount = total / PageSize + (_p > 0 ? 1 : 0);
            this.txtPageCount.Text = _PageCount.ToString();
        }

        /// <summary>
        /// 初始化标题
        /// </summary>
        /// <param name="cmd"></param>
        public void InitTitle(CtrlPlatformCommandInfo cmd)
        {
        }

        /// <summary>
        /// 输出参数
        /// </summary>
        /// <returns></returns>
        public CtrlPlatformCommandInfo OutputParameter()
        {
            var cmd = new CtrlPlatformCommandInfo();
            var data = new MetaDataInfo();
            data.table_name = this._TableName;

            foreach (var child in this._Columns)
            {
                child.column_value = string.Format("{0}", this.xtn_Data.SelectedItem.GetPropertyValue(child.column_name.ToLower()));
            }
            data.Item = this._Columns;
            cmd.MetaDataInfo = data;
            return cmd;
        }
        /// <summary>
        /// 输入参数
        /// </summary>
        /// <param name="cmd"></param>
        public void InputParameter(CtrlPlatformCommandInfo cmd)
        {
        }

        /// <summary>
        /// 执行Action
        /// </summary>
        /// <param name="cmd"></param>
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
        /// 清空值
        /// </summary>
        public void ClearCtrlValue()
        {
            this.xtn_Data.ItemsSource = null;
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            xtn_Data.SelectionChanged -= xtn_Data_SelectionChanged;
            btnFirstPage.Click -= btnFirstPage_Click;
            btnNextPage.Click -= btnNextPage_Click;
            btnPreviousPage.Click -= btnPreviousPage_Click;
            btnLastPage.Click -= btnLastPage_Click;
        }

        void xtn_Data_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (0 > this.xtn_Data.SelectedIndex)
            {
                return;
            }
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
        /// 刷新操作
        /// </summary>
        public void Refresh(CtrlPlatformCommandInfo cmd)
        {
            IsSendCommand = false;
            InitLoad(_InitLoadParameter);
        }

        /// <summary>
        /// 删除操作
        /// </summary>
        /// <param name="cmd"></param>
        public void Delete(CtrlPlatformCommandInfo cmd)
        {
            var op = this.OutputParameter();
            if (null == op)
            {
                return;
            }

            List<String> _condition = new List<String>();
            string insertSql = string.Empty;
            //-->获取出主键来
            var columns = op.MetaDataInfo.Item.Where(p => p.column_key.IsNotEmpty() && p.column_key.Equals(ConstantCollection.TABLE_KEY)).GetTList<MetaDataColumnInfo>();
            if (null != columns || 0 < columns.Count)
            {
                foreach (var mdci in columns)
                {
                    _condition.Add(string.Format("{0} {1}", mdci.column_name, Wrapper.ConvertToDBTypeWhereUsing(mdci)));
                }
            }
            else
            {
                foreach (var mdci in op.MetaDataInfo.Item)
                {
                    _condition.Add(string.Format("{0} {1}", mdci.column_name, Wrapper.ConvertToDBTypeWhereUsing(mdci)));
                }
            }
            insertSql = string.Format("delete from {0} where {1} ", _TableName, string.Join(" and ", _condition));

            cmd = new CtrlPlatformCommandInfo();
            cmd.CommandName = ConstantCollection.CommandName_MixedCommand;
            cmd.CompositeControlName = this.Name;
            cmd.ExecSql = new List<CtrlExecSQLCmd>();
            var exec = new CtrlExecSQLCmd(CtrlExecSqlCmdType.ExecuteNonQuery, insertSql, _TableName, this.Name, ConstantCollection.COMPOSITE_CONTROL_DELETE);
            cmd.ExecSql.Add(exec);

            cmd.TempValue = new XElement("Execute", new XAttribute(ConstantCollection.METHOD_NAME, ConstantCollection.COMPOSITE_CONTROL_DELETE));
            this._IPageRuntime.SendCommand(cmd);
            this._NotifyRuntimePage(ConstantCollection.COMPOSITE_CONTROL_DELETE);
        }

        /// <summary>
        /// 处理删除操作
        /// </summary>
        /// <param name="cmd"></param>
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
        /// 条件查询
        /// </summary>
        /// <param name="cmd"></param>
        public void InitLoadByCondition(CtrlPlatformCommandInfo cmd)
        {
            //-->条件查询
            if (null == cmd)
            {
                return;
            }
            _ConditionQueryParam = cmd;
            this.xtn_CurrentIndex.Text = "1";
            SendQueryCommand(cmd, ConstantCollection.COMPOSITE_CONTROL_INIT_LOAD_BY_CONDITION);
        }
        void ProcessInitLoadByCondition(CtrlPlatformCommandInfo cmd)
        {
            ProcessInitLoad(cmd);
        }

        //-->翻页操作
        /// <summary>
        /// 最后一页
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnLastPage_Click(object sender, RoutedEventArgs e)
        {
            var current = this.xtn_CurrentIndex.Text.ConvertTo<int>();
            if (_PageCount == current)
            {
                return;
            }
            this.xtn_CurrentIndex.Text = string.Format("{0}", _PageCount);
            SendQueryCommand(_ConditionQueryParam != null ? _ConditionQueryParam : _InitLoadParameter, ConstantCollection.COMPOSITE_CONTROL_INIT_LOAD_BY_CONDITION);
        }
        /// <summary>
        /// 上一页
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnPreviousPage_Click(object sender, RoutedEventArgs e)
        {
            var current = this.xtn_CurrentIndex.Text.ConvertTo<int>();
            if (current > 1)
            {
                this.xtn_CurrentIndex.Text = string.Format("{0}", --current);
                SendQueryCommand(_ConditionQueryParam != null ? _ConditionQueryParam : _InitLoadParameter, ConstantCollection.COMPOSITE_CONTROL_INIT_LOAD_BY_CONDITION);
            }
        }
        /// <summary>
        /// 下一页
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnNextPage_Click(object sender, RoutedEventArgs e)
        {
            var current = this.xtn_CurrentIndex.Text.ConvertTo<int>();
            if (current < _PageCount)
            {
                this.xtn_CurrentIndex.Text = string.Format("{0}", ++current);
                SendQueryCommand(_ConditionQueryParam != null ? _ConditionQueryParam : _InitLoadParameter, ConstantCollection.COMPOSITE_CONTROL_INIT_LOAD_BY_CONDITION);
            }
        }

        /// <summary>
        /// 第一页
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnFirstPage_Click(object sender, RoutedEventArgs e)
        {
            var current = this.xtn_CurrentIndex.Text.ConvertTo<int>();
            if (1 == current)
            {
                return;
            }
            this.xtn_CurrentIndex.Text = "1";
            SendQueryCommand(_ConditionQueryParam != null ? _ConditionQueryParam : _InitLoadParameter, ConstantCollection.COMPOSITE_CONTROL_INIT_LOAD_BY_CONDITION);
        }
    }
}
//internal class DataFormatters : IValueConverter
//{
//    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
//    {
//    }
//    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
//    {
//    }
//}