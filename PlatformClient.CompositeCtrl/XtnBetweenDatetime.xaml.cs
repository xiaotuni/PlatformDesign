using System;
using System.Collections.Generic;
using System.Windows.Controls;
using PlatformClient.Common.Interfaces;
using PlatformClient.Extend.Core;
using PlatformClient.Model.Method;

namespace PlatformClient.CompositeCtrl
{
    /// <summary>
    /// 
    /// </summary>
    public partial class XtnBetweenDatetime : UserControl, ICompositeCtrl
    {
        private IPageRuntime _IPageRuntime;
        private ControlInfo _ControlInfo;
        private MetaDataInfo _MetaDataInfo;
        private string _DataSource;

        /// <summary>
        /// 文件框标题
        /// </summary>
        [PropertyInfoAttribute("", "文件框标题")]
        public string TextContent { get { return this.txtContent.Text; } set { this.txtContent.Text = value; } }
        /// <summary>
        /// 条件字段{table:field}
        /// </summary>
        [PropertyInfoAttribute("PlatformClient.PageDesignTime.Controls;PlatformClient.PageDesignTime.Controls.XtnConditionField", "条件字段")]
        public string DataSource
        {
            get { return _DataSource; }
            set
            {
                _DataSource = value;
                var arr = _DataSource.Split('|');
                this.txtContent.Text = arr[2];
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public XtnBetweenDatetime()
        {
            InitializeComponent();
            this.Loaded += XtnBetweenDatetime_Loaded;
        }

        void XtnBetweenDatetime_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            InitDateTime();
        }
        void InitDateTime()
        {
            this.dpBegin.SelectedDate = DateTime.Now.AddMonths(-1);
            this.dpEnd.SelectedDate = DateTime.Now;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="iRuntime"></param>
        public void SetPageRuntimeInterface(IPageRuntime iRuntime)
        {
            this._IPageRuntime = iRuntime;
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
        /// 
        /// </summary>
        /// <param name="cmd"></param>
        public void InitLoad(CtrlPlatformCommandInfo cmd)
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmd"></param>
        public void InitTitle(CtrlPlatformCommandInfo cmd)
        {
            _ControlInfo = cmd.ControlInfo;
            _MetaDataInfo = cmd.MetaDataInfo;
            this.TextContent = _ControlInfo.Comment;
            if (null == _MetaDataInfo || null == _MetaDataInfo.Item || 0 == _MetaDataInfo.Item.Count)
            {
                return;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public CtrlPlatformCommandInfo OutputParameter()
        {
            var _value = string.Format("BETWEEN '{0}' and '{1}'", 
                ((DateTime) dpBegin.SelectedDate).ToString("yyyy-MM-dd"),
                ((DateTime)dpEnd.SelectedDate).ToString("yyyy-MM-dd"));
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
                col.column_name = DataSource.Split(',')[1];
                col.table_name = DataSource.Split(',')[0];
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
        /// <param name="cmd"></param>
        public void InputParameter(CtrlPlatformCommandInfo cmd)
        {
        }
        /// <summary>
        /// 
        /// </summary>
        public double TitleWidth { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmd"></param>
        public void DoAction(CtrlPlatformCommandInfo cmd)
        {
        }
        /// <summary>
        /// 
        /// </summary>
        public void ClearCtrlValue()
        {
            InitDateTime();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmd"></param>
        public void Refresh(CtrlPlatformCommandInfo cmd)
        {
        }
        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            this.Loaded += XtnBetweenDatetime_Loaded;
        }
    }
}
