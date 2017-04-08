using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using PlatformClient.Common.Lib;
using PlatformClient.Model.Method;

namespace PlatformClient.Selectors.Core
{
    /// <summary>
    /// 
    /// </summary>
    public partial class ParameterSelected : UserControl, IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        public event RoutedEventHandler DeleteClick;
        /// <summary>
        /// 
        /// </summary>
        public event RoutedEventHandler ModifyClick;
        /// <summary>
        /// 
        /// </summary>
        Brush _Background;
        bool _OrgIsDataSource = false;
        /// <summary>
        /// 字段名称
        /// </summary>
        public string FieldName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ControlInfo OrgCtrlInfo { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string TargetCtrlName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public MetaDataColumnInfo TargetMetaDataColumn { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public MetaDataColumnInfo OrgMetaDataColumn { get; set; }
        /// <summary>
        /// 初始化值
        /// </summary>
        public ParamInfo InitLoadValue { get; set; }

        /// <summary>
        /// OrgFieldType是否是来到源数据[ControlDataSourceTableName 所对应表里的字段]里的。
        /// </summary>
        public bool OrgIsDataSource
        {
            get { return _OrgIsDataSource; }
            set { _OrgIsDataSource = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public ParameterSelected()
        {
            InitializeComponent();

            this.btnDelete.Click += btnDelete_Click;
            this.btnModify.Click += btnModify_Click;
            _Background = this.LayoutRoot.Background;
        }

        void btnModify_Click(object sender, RoutedEventArgs e)
        {
            if (null != ModifyClick)
            {
                ModifyClick(this, e);
            }
        }

        void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (null != DeleteClick)
            {
                DeleteClick(this, e);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Text { get { return this.txtParamValue.Text; } set { this.txtParamValue.Text = value; } }

        /// <summary>
        /// 
        /// </summary>
        public ParamInfo ParamForamt
        {
            get
            {
                if (null == OrgMetaDataColumn ||
                    null == TargetMetaDataColumn)
                {
                    return InitLoadValue;
                }
                ParamInfo pi = new ParamInfo();
                pi.OrgFieldName = OrgMetaDataColumn.column_name;
                pi.OrgFieldType = OrgMetaDataColumn.data_type;
                pi.OrgIsDataSource = this.OrgIsDataSource;
                pi.TargetCtrlName = this.TargetCtrlName;
                pi.TargetFieldName = this.TargetMetaDataColumn.column_name;
                pi.TargetFieldType = this.TargetMetaDataColumn.data_type;
                return pi;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal void BackgroundReset()
        {
            try
            {
                this.LayoutRoot.Background = _Background;
            }
            catch { }
        }

        /// <summary>
        /// 
        /// </summary>
        internal void BackgroundSet()
        {
            this.LayoutRoot.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0xD3, 0xF5, 0xD3));
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            this.txtParamName.Text = string.Empty;
            this.Text = string.Empty;
            this.FieldName = string.Empty;
            this.TargetMetaDataColumn = null;
            this.OrgCtrlInfo = null;
            this.TargetCtrlName = null;
            this.OrgMetaDataColumn = null;
            this.InitLoadValue = null;
            _Background = null;
        }

    }
}
