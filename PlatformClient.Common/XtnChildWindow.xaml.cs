using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml.Linq;
using PlatformClient.Common.Interfaces;
using PlatformClient.Extend.Core;
using PlatformClient.Utility;

namespace PlatformClient.Common
{
    /// <summary>
    /// 平台弹出窗体
    /// </summary>
    public partial class XtnChildWindow : ChildWindow, IUserControl
    {
        private bool _IsFullscreen;
        /// <summary>
        /// 当前界面上增加的子控件
        /// </summary>
        public FrameworkElement CurrentAddChildControl { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public XElement EvaluationContent { get; set; }
        /// <summary>
        /// 设计时框架接口
        /// </summary>
        public IPageDesignFramework IDesignFramework { get; set; }
        /// <summary>
        /// 当前选中控件名称
        /// </summary>
        public string CurrentSelectedControlName { get; set; }
        /// <summary>
        /// 构造函数
        /// </summary>
        public XtnChildWindow()
        {
            InitializeComponent();
            this.KeyUp += PropertyGridChildWindow_KeyUp;
            this.Loaded += XtnChildWindow_Loaded;
            this.SizeChanged += XtnChildWindow_SizeChanged;
        }

        void XtnChildWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
           // SetSize();
        }

        void XtnChildWindow_Loaded(object sender, RoutedEventArgs e)
        {
           // SetSize();
        }

        void SetSize()
        {
            if (_IsFullscreen)
            {
                CurrentAddChildControl.Height = BrowserHelper.ClientHeight - 90;
                CurrentAddChildControl.Width = BrowserHelper.ClientWidth - 26;
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="iDesignFramework">设置时接口</param>
        /// <param name="control">控件</param>
        public XtnChildWindow(IPageDesignFramework iDesignFramework, FrameworkElement control)
            : this()
        {
            CurrentAddChildControl = control;
            this.IDesignFramework = iDesignFramework;

            if (!Double.IsNaN(control.Height))
            {
                this.Height = control.Height + 100;
                this.Width = control.Width + 50;
            }
            if (control is IUserControl && null != iDesignFramework)
            {
                (control as IUserControl).IDesignFramework = iDesignFramework;
            }
            if (control is IPlatformClientChildWindow)
            {
                var _ipc = control as IPlatformClientChildWindow;
                this._IsFullscreen = _ipc.IsFullscreen;
                if (_IsFullscreen)
                {
                    this.HorizontalAlignment = HorizontalAlignment.Stretch;
                    this.VerticalAlignment = VerticalAlignment.Stretch;
                }
            }
            this.gControl.Children.Add(control);
        }

        void PropertyGridChildWindow_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.Close();
            }
        }

        void OKButton_Click(object sender, RoutedEventArgs e)
        {
            //-->现在这里进行判断
            var ui = this.gControl.Children[0] as IPlatformClientChildWindow;
            if (null != ui)
            {
                bool ischecked = ui.CheckSave();
                if (!ischecked)
                {
                    return;
                }
            }
            this.DialogResult = true;
        }

        void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
        /// <summary>
        /// 获取控件
        /// </summary>
        public FrameworkElement GetControl
        {
            get
            {
                if (this.gControl.Children.Count > 0)
                {
                    return this.gControl.Children[0] as FrameworkElement;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            try
            {
                CurrentAddChildControl = null;
                EvaluationContent = null;
                IDesignFramework = null;
                if (this.gControl.Children.Count > 0)
                {
                    UIElement ui = this.gControl.Children[0];
                    //if (ui is IPlatformClientChildWindow)
                    //{
                    //    var child = ui as IPlatformClientChildWindow;
                    //    child.CheckSave(this.DialogResult == true ? true : false);
                    //    child.Dispose();
                    //}
                    if (ui is IUserControl && true == this.DialogResult)
                    {
                        this.EvaluationContent = (ui as IUserControl).EvaluationContent;
                    }
                    if (ui is IDisposable)
                    {
                        (ui as IDisposable).Dispose();
                    }
                }
                this.KeyUp -= PropertyGridChildWindow_KeyUp;
                gControl.Children.Clear();
            }
            catch { }
        }

        /// <summary>
        /// 设置标题
        /// </summary>
        /// <param name="title"></param>
        public void SetTitle(string title)
        {
            if (title.IsNullOrEmpty())
            {
                return;
            }
            this.Title = title;
        }

    }
}

