using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using PlatformClient.Common;
using PlatformClient.Utility;
using PlatformClient.Utility.ParseXml;
using PlatformClient.Extend.Core;

namespace PlatformClient.PageDesignTime.Controls
{
    /// <summary>
    /// 控件工具箱
    /// </summary>
    public partial class XtnCtrBoxs : UserControl, IDisposable
    {
        /// <summary>
        /// 选中改变事件
        /// </summary>
        public event EventHandler<SelectionChangedEventArgs> SelectionChanged;
        /// <summary>
        /// 控件模板
        /// </summary>
        ControlTemplateConfig TempletConfig;

        /// <summary>
        /// 当前选中项
        /// </summary>
        public object SelectedItem
        {
            get
            {
                if (null == this.lbControls.SelectedItem)
                {
                    return null;
                }
                //-->创建一个控件
                UIElement ui = CreateControl();
                lbControls.SelectedIndex = -1;
                return ui;
            }
        }

        /// <summary>
        /// 创建控件
        /// </summary>
        /// <returns></returns>
        UIElement CreateControl()
        {
            if (null == this.lbControls.SelectedItem)
            {
                return null;
            }
            ControlConfig config = (this.lbControls.SelectedItem as ListBoxItem).Tag as ControlConfig;

            List<String> assItem = new List<string>();
            assItem.AddRange(TempletConfig.AssemblyCollection);
            //-->判断是不是复合控件
            if (config.IsCompositeCtrl && !config.AssemblyQualifiedName.IsNullOrEmpty())
            {
                assItem.Add(config.AssemblyQualifiedName.Trim());
            }
            string ass = string.Join(" ", assItem);
            ass = string.Format(config.xaml, ass);
            ass = ass.Replace("??", config.Name + DateTime.Now.ToString("ffffff"));
            ass = ass.Trim();
            FrameworkElement ui = Wrapper.LoadXaml<FrameworkElement>(ass);
            ui.Tag = config;
            return ui;
        }

        void _SelectionChangedMethod(object sender, SelectionChangedEventArgs e)
        {
            if (null == SelectionChanged)
            {
                return;
            }
            SelectionChanged(sender, e);
        }

        /// <summary>
        /// 结构函数
        /// </summary>
        public XtnCtrBoxs()
        {
            InitializeComponent();

            lbControls.SelectionChanged += lbControls_SelectionChanged;

            InitLoadControl();

            TempletConfig = ParseControlTemplate.GetControlTempletConfig("UserControl");
        }

        void InitLoadControl()
        {
            List<ControlConfig> item = ParseControlConfig.ControlItem.Where(p => p.IsDisplay).GetTList<ControlConfig>();
            if (null == item || 0 == item.Count)
            {
                return;
            }
            this.lbControls.Items.Clear();
            foreach (var v in item)
            {
                ListBoxItem lbi = new ListBoxItem();
                lbi.Tag = v;
                lbi.Content = v.Description;
                ToolTipService.SetToolTip(lbi, string.Format("{0}【{1}】", v.Description, v.Name));
                this.lbControls.Items.Add(lbi);
            }
        }

        void lbControls_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _SelectionChangedMethod(this, e);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            lbControls.SelectionChanged -= lbControls_SelectionChanged;
        }
    }
}
