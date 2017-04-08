using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;
using PlatformClient.Common.Events;
using PlatformClient.Common.Lib;
using PlatformClient.Model.Method;
using PlatformClient.Utility;
using PlatformClient.Model.Events;

namespace PlatformClient.PageDesignTime.Controls
{
    /// <summary>
    /// 
    /// </summary>
    public partial class PageFileTreeManager : UserControl, IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        public event RoutedPropertyChangedEventHandler<object> SelectedItemChanged;
        /// <summary>
        /// 打开页面。
        /// </summary>
        public event EventHandler<OpenPageInfoEventArgs> OpenPageInfo;
        /// <summary>
        /// 选择控件属性事件
        /// </summary>
        public event SelectionChangedEventHandler SelectionControlProperty;

         XmlTemplate _XmlTemplate_CurrentPage;
         XamlTemplate _XamlTemplate;
         UserControl _PageInfo;
         ControlInfo _ControlInfo;
         List<PropertyInfoAttribute> _SelectedControlPropertyItem;
         PropertyInfoAttribute _SelectedControlProperty;

        /// <summary>
        /// 页面控件
        /// </summary>
        public UserControl PageControl { get { return _PageInfo; } }
        /// <summary>
        /// 当前选中控件的属性
        /// </summary>
        public PropertyInfoAttribute SelectedControlProperty { get { return _SelectedControlProperty; } }

        /// <summary>
        /// 当前选中控件所有属性
        /// </summary>
        public List<PropertyInfoAttribute> SelectedControlPropertyItem { get { return _SelectedControlPropertyItem; } }
        /// <summary>
        /// 当前选中的页面信息
        /// </summary>
        public XmlTemplate SelectedPageInfo { get { return _XmlTemplate_CurrentPage; } }
        /// <summary>
        /// 当前选中的控件信息
        /// </summary>
        public ControlInfo SelectedControlInfo { get { return _ControlInfo; } }

        void _SelectedItemChangedMethod(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (null != SelectedItemChanged)
            {
                SelectedItemChanged(sender, e);
            }
        }

        void _SelectionPropertyMethod(object sender, SelectionChangedEventArgs e)
        {
            if (null != SelectionControlProperty)
            {
                SelectionControlProperty(this, e);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public PageFileTreeManager()
        {
            InitializeComponent();

            this.pft.OpenPageInfo += pft_OpenPageInfo;
            this.ct.SelectedItemChanged += ct_SelectedItemChanged;
            this.lboxProperty.SelectionChanged += lboxProperty_SelectionChanged;

            //this.pft.InitDirectory(0);
            this.pft.IsDisplayToolbar = System.Windows.Visibility.Collapsed;
        }

        void lboxProperty_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _SelectedControlProperty = lboxProperty.SelectedItem as PropertyInfoAttribute;

            _SelectionPropertyMethod(this, e);
        }

        void ct_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            this.lboxProperty.ItemsSource = null;
            _ControlInfo = ct.SelectedControlInfo;
            if (null == _ControlInfo)
            {
                _SelectedItemChangedMethod(this, e);
                return;
            }
            //-->判断是否是Form控件
            UIElement ui = this._PageInfo.FindName(_ControlInfo.Name) as UIElement;
            if (null == ui)
            {
                _SelectedItemChangedMethod(this, e);
                return;
            }
            _SelectedControlPropertyItem = Wrapper.ReadControlAttribute<PropertyInfoAttribute>(ui);
            _SelectedControlPropertyItem.InsertRange(0, Wrapper.CommonPropertyItem(ui));
            //得到所有控件的属性了。
            this.lboxProperty.ItemsSource = _SelectedControlPropertyItem;
            this.lboxProperty.SelectedIndex = 0;

            _SelectedItemChangedMethod(this, e);
        }

        void pft_OpenPageInfo(object sender, OpenPageInfoEventArgs e)
        {
            //-->获取控件
            XElement _xe_page = XElement.Parse(e.PageDirectorySub.PageContent.Trim());
            if (null == _xe_page)
            {
                return;
            }
            XElement _xe_xaml = _xe_page.Element("xaml");
            XElement _xe_xml = _xe_page.Element("xml");

            string xaml = _xe_xaml != null && _xe_xaml.FirstNode != null ? _xe_xaml.FirstNode.ToString() : "";
            string xml = _xe_xml != null && _xe_xml.FirstNode != null ? _xe_xml.FirstNode.ToString() : "";
            //-->xml操作
            _XmlTemplate_CurrentPage = XmlTemplate.DecodeXml(xml);
            //-->获取xaml
            _XamlTemplate = XamlTemplate.DecodeXml(xaml);
            _PageInfo = Wrapper.CreateUIElement<UserControl>(xaml);
            LoadControls();

            if (null != OpenPageInfo)
            {
                OpenPageInfo(this, e);
            }
        }

        void LoadControls()
        {
            if (null == _XmlTemplate_CurrentPage)
            {
                return;
            }
            ct.ShowTree(_XmlTemplate_CurrentPage.ControlItem);
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            this.pft.OpenPageInfo -= pft_OpenPageInfo;
            this.ct.SelectedItemChanged -= ct_SelectedItemChanged;
            this.lboxProperty.SelectionChanged -= lboxProperty_SelectionChanged;

            _XmlTemplate_CurrentPage = null;
            _XamlTemplate = null;
            _PageInfo = null;
            _ControlInfo = null;
            ct.Dispose();
        }
    }
}
