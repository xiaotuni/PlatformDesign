using System;
using System.Windows;
using System.Windows.Controls;
using PlatformClient.Common.Events;
using PlatformClient.Common.Interfaces;
using PlatformClient.Common.Lib;
using PlatformClient.PageDesignTime.Controls;
using PlatformClient.PropertyGrid.Events;
using PlatformClient.Utility.Events;
using PlatformClient.Model.Events;
using PlatformClient.Model.Method;

namespace PlatformClient.PageDesignTime
{
    /// <summary>
    /// 页面设计时框架
    /// </summary>
    public partial class PageDesignFramework : UserControl, IPageDesignFramework
    {
        /// <summary>
        /// 
        /// </summary>
        public event EventHandler<EvaluationCtrlLostFocusEventArgs> PropertyLostFocus;
        /// <summary>
        /// 
        /// </summary>
        public PageDesignFramework()
        {
            InitializeComponent();

            pwag.IDesignFramework = this as IPageDesignFramework;
            pcc.IDesignFramework = this;
            this.Loaded += PageDesignFramework_Loaded;
            this.ct_Controls.SelectedItemChanged += ct_Controls_SelectedItemChanged;
        }

        void ct_Controls_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (null == pwag.CurrentPageWorkArea)
            {
                return;
            }
            var ct = sender as XtnCtrlTree;
            if (null == ct)
            {
                return;
            }
            var sci = ct.SelectedControlInfo;
            var controlName = sci.Name;
            //-->相当于点击当前控件。
            //-->1、更新属性控件
            pwag.CurrentPageWorkArea.ControlTreeSelectedEventUpdateCurrentSelectedControl(controlName);

        }

        void pft_UpdatePageName(object sender, OpenPageInfoEventArgs e)
        {
            pwag.UpdateTabItemHeader(e);
        }

        void btnSaveAll_Click(object sender, RoutedEventArgs e)
        {
            //-->全部保存
            pwag.SaveAll();
        }

        void pft_OpenPageInfo(object sender, OpenPageInfoEventArgs e)
        {
            pwag.OpenPageInfo(e);//, this);
        }

        void btnSave_Click(object sender, RoutedEventArgs e)
        {
            //-->保存当前的数据
            pwag.Save();
        }

        void PageDesignFramework_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                this.cb.SelectionChanged += cbItem_SelectionChanged;
                this.pcc.PropertyLostFocus += pcc_PropertyLostFocus;
                this.pwag.ControlPosition += pwag_ControlPosition;
                this.pwag.ControlTimesUpdate += pwag_ControlTimesUpdate;
                this.pwag.OnClear += pwag_OnClear;
                this.pft.OpenPageInfo += pft_OpenPageInfo;
                this.pft.UpdatePageName += pft_UpdatePageName;
                this.btnSave.Click += btnSave_Click;
                this.btnSaveAll.Click += btnSaveAll_Click;

                //pft.InitDirectory(0);
            }
            catch { }
        }

        void pwag_OnClear(object sender, EventArgs e)
        {
            ct_Controls.ShowTree(null);
            pcc.ReadAttribute(null);
        }

        void pwag_ControlTimesUpdate(object sender, ControlTimesUpdateEventArgs e)
        {
            ct_Controls.ShowTree(e.Controls);
        }

        void pwag_ControlPosition(object sender, ControlModifyPropertyEventArgs e)
        {
            //-->告诉属性控件，属性有变。
            pcc.UpdateProperty(e);
        }

        void _PropertyLostFocusMethod(object sender, EvaluationCtrlLostFocusEventArgs e)
        {
            if (null != PropertyLostFocus)
            {
                PropertyLostFocus(sender, e);
            }
        }

        void pcc_PropertyLostFocus(object sender, EvaluationCtrlLostFocusEventArgs e)
        {
            pwag.PropertyLostFoucs(e);
        }

        void cbItem_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            XtnCtrBoxs cb = sender as XtnCtrBoxs;
            if (null == cb)
            {
                return;
            }
            FrameworkElement fe = cb.SelectedItem as FrameworkElement;
            if (null == fe)
            {
                return;
            }

            //-->将得到的控件信息增加到设计页面和属性窗口里去。
            this.pwag.AddChild(fe);
        }

        /// <summary>
        /// 更新当前选中控件
        /// </summary>
        /// <param name="ui"></param>
        public void UpdateCurrentSelectedCtrl(UIElement ui)
        {
            this.pcc.ReadAttribute(ui);
        }

        private void btnPageView_Click(object sender, RoutedEventArgs e)
        {
            this.pwag.PageView();
        }

        private void btnOpenDBFramework_Click(object sender, RoutedEventArgs e)
        {
            this.pwag.OpenDBFramework();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public XmlTemplate GetCurrentXmlTemplate()
        {
            return pwag.GetCurrentXmlTemplate();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public XamlTemplate GetCurrentXamlTemplate()
        {
            return pwag.GetCurrentXamlTemplate();
        }

        /// <summary>
        /// 更新当前模板
        /// </summary>
        public void UpdateCurrentTemplate()
        {
            pwag.UpdateCurrentTemplate();
        }

        /// <summary>
        /// 查找控件
        /// </summary>
        /// <param name="controlName"></param>
        /// <returns></returns>
        public UIElement FindControl(string controlName)
        {
            return pwag.FindControl(controlName);
        }

        /// <summary>
        /// 获取文件树
        /// </summary>
        /// <returns></returns>
        public IPageFileTree GetIPageFileTree()
        {
            return pft;
        }
        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {

            this.cb.SelectionChanged -= cbItem_SelectionChanged;
            this.pcc.PropertyLostFocus -= pcc_PropertyLostFocus;
            this.pwag.ControlPosition -= pwag_ControlPosition;
            this.pwag.ControlTimesUpdate -= pwag_ControlTimesUpdate;
            this.pft.OpenPageInfo -= pft_OpenPageInfo;
            this.pft.UpdatePageName -= pft_UpdatePageName;
            this.btnSave.Click -= btnSave_Click;
            this.btnSaveAll.Click -= btnSaveAll_Click;
            this.Loaded -= PageDesignFramework_Loaded;

            this.pft.Dispose();
            this.pcc.Dispose();
            this.pwag.Dispose();
            this.pft.Dispose();
        }

        /// <summary>
        /// 获取控件信息
        /// </summary>
        /// <param name="controlName">控件名称</param>
        /// <returns>返回控件信息</returns>
        public ControlInfo GetControlInfoByControlName(string controlName)
        {
            return pwag.GetControlInfoByControlName(controlName);
        }

        /// <summary>
        /// 获取MetaData信息
        /// </summary>
        /// <param name="tableName">表的名称</param>
        /// <returns>返回一个MetaData对象</returns>
        public MetaDataInfo GetMetaDataInfoByTableName(string tableName)
        {
            return pwag.GetMetaDataInfoByTableName(tableName);
        }
    }
}
