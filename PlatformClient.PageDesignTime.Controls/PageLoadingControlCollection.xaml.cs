using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;
using PlatformClient.Extend.Core;
using PlatformClient.Common;
using PlatformClient.Common.Interfaces;
using PlatformClient.Model.Method;

namespace PlatformClient.PageDesignTime.Controls
{
    /// <summary>
    /// 
    /// </summary>
    public partial class PageLoadingControlCollection : UserControl, IPageDesignTimeUserControl
    {
        /// <summary>
        /// 当前选中控件名称
        /// </summary>
        public string CurrentSelectedControlName { get; set; }
        /// <summary>
        /// 初始化方法【InitLoad】
        /// </summary>
        string _INIT_FUNCTION = ConstantCollection.COMPOSITE_CONTROL_INIT_LOAD;

        /// <summary>
        /// 
        /// </summary>
        public PageLoadingControlCollection()
        {
            InitializeComponent();
            this.Loaded += PageLoadingControlCollection_Loaded;
            this.btnLeft.Click += btnLeft_Click;
            this.btnLeftAll.Click += btnLeftAll_Click;
            this.btnRight.Click += btnRight_Click;
            this.btnRightAll.Click += btnRightAll_Click;
        }

        /// <summary>
        /// 将当左右所有列添加到右边
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnRightAll_Click(object sender, RoutedEventArgs e)
        {
            var controlItem = this.IDesignFramework.GetCurrentXmlTemplate().ControlItem;
            this.lbLoadingControls.Items.Clear();
            List<PageLoadingInfo> result = new List<PageLoadingInfo>();
            foreach (var v in controlItem)
            {
                PageLoadingInfo pli = new PageLoadingInfo();
                pli.CallFunctionName = _INIT_FUNCTION;// "InitLoad";
                pli.ControlName = v.Name;
                result.Add(pli);
                ListBoxItem lbi = new ListBoxItem();
                lbi.Content = pli;
                this.lbLoadingControls.Items.Add(lbi);
            }
            this.IDesignFramework.GetCurrentXmlTemplate().PageLoadingItem = result;
            this.IDesignFramework.UpdateCurrentTemplate();
            _SetEvaluationContent();
        }

        /// <summary>
        /// 将当前选择的列添加到右边
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnRight_Click(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = this.tvControls.SelectedItem as TreeViewItem;
            if (null == item)
            {
                return;
            }
            string controlName = string.Format("{0}", item.Header);
            var pageLoadItem = this.lbLoadingControls.ItemsSource as List<PageLoadingInfo>;
            if (null == pageLoadItem)
            {
                pageLoadItem = new List<PageLoadingInfo>();
                PageLoadingInfo pli = new PageLoadingInfo();
                pli.ControlName = controlName;
                pli.CallFunctionName = _INIT_FUNCTION;//"InitLoad";
                ListBoxItem lbi = new ListBoxItem();
                lbi.Content = pli;
                this.lbLoadingControls.Items.Add(lbi);
                pageLoadItem.Add(pli);
                this.IDesignFramework.GetCurrentXmlTemplate().PageLoadingItem = pageLoadItem;
                this.IDesignFramework.UpdateCurrentTemplate();
            }
            else
            {
                var isExists = from p in pageLoadItem
                               where p.ControlName.Equals(controlName)
                               select p;
                if (null == isExists || 0 == isExists.Count())
                {
                    PageLoadingInfo pli = new PageLoadingInfo();
                    pli.ControlName = controlName;
                    pli.CallFunctionName = _INIT_FUNCTION;//"InitLoad";
                    pageLoadItem.Add(pli);
                    FillListBoxItem(pageLoadItem);

                    this.IDesignFramework.GetCurrentXmlTemplate().PageLoadingItem = pageLoadItem;
                    this.IDesignFramework.UpdateCurrentTemplate();
                }
            }
            _SetEvaluationContent();
        }

        void _SetEvaluationContent()
        {
            var pageLoadItem = this.IDesignFramework.GetCurrentXmlTemplate().PageLoadingItem;
            XElement returnValue = new XElement("PageLoadingItem");
            returnValue.Add(
                   from p in pageLoadItem
                   select p.ToXElement());
            _EvaluationContent = returnValue;
        }

        /// <summary>
        /// 清空当前的列
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnLeftAll_Click(object sender, RoutedEventArgs e)
        {
            this.lbLoadingControls.Items.Clear();
            this.IDesignFramework.GetCurrentXmlTemplate().PageLoadingItem.Clear();
            this.IDesignFramework.UpdateCurrentTemplate();
            _SetEvaluationContent();
        }

        /// <summary>
        /// 删除当前选择的列
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnLeft_Click(object sender, RoutedEventArgs e)
        {
            var items = this.lbLoadingControls.SelectedItems;
            if (null == items || 0 == items.Count)
            {
                return;
            }
            foreach (var item in items)
            {
                PageLoadingInfo p = item as PageLoadingInfo;
                if (null == p)
                {
                    continue;
                }
                this.IDesignFramework.GetCurrentXmlTemplate().PageLoadingItem.Remove(p);
            }
            this.IDesignFramework.UpdateCurrentTemplate();
            FillListBoxItem(this.IDesignFramework.GetCurrentXmlTemplate().PageLoadingItem);
            _SetEvaluationContent();
        }

        void PageLoadingControlCollection_Loaded(object sender, RoutedEventArgs e)
        {
            //-->开始绑定ControlInfo
            if (null == tvControls)
            {
                tvControls = new XtnCtrlTree();
            }
            tvControls.ShowTree(this.IDesignFramework.GetCurrentXmlTemplate().ControlItem);
            if (null == lbLoadingControls)
            {
                lbLoadingControls = new ListBox();
            }
            var pControls = this.IDesignFramework.GetCurrentXmlTemplate().PageLoadingItem;
            FillListBoxItem(pControls);
            _SetEvaluationContent();
        }

        void FillListBoxItem(List<PageLoadingInfo> items)
        {
            this.lbLoadingControls.ItemsSource = null;
            this.lbLoadingControls.ItemsSource = items;
        }

        XElement _EvaluationContent;

        /// <summary>
        /// 
        /// </summary>
        public XElement EvaluationContent
        {
            get
            {
                if (null == _EvaluationContent)
                {
                    _SetEvaluationContent();
                }
                return _EvaluationContent;
            }
            set { _EvaluationContent = value; }
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            try
            {
                _EvaluationContent = null;
                this.Loaded -= PageLoadingControlCollection_Loaded;
                this.btnLeft.Click -= btnLeft_Click;
                this.btnLeftAll.Click -= btnLeftAll_Click;
                this.btnRight.Click -= btnRight_Click;
                this.btnRightAll.Click -= btnRightAll_Click;
            }
            catch { }
        }

        /// <summary>
        /// 设计时框架接口
        /// </summary>
        public IPageDesignFramework IDesignFramework { get; set; }

        /// <summary>
        /// 查检是否保存
        /// </summary>
        public bool CheckSave()
        {
            return true;
        }
        /// <summary>
        /// 是否全屏
        /// </summary>
        public bool IsFullscreen
        {
            get { return false; }
        }
    }
}
