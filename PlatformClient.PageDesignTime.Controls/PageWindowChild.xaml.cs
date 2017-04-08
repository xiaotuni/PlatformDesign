using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace PlatformClient.PageDesignTime.Controls
{
    public partial class PageWindowChild : ChildWindow
    {
        /// <summary>
        /// 
        /// </summary>
        public PageWindowChild()
        {
            InitializeComponent();
            this.KeyDown += new KeyEventHandler(PageWindowChild_KeyDown);
        }

        public PageWindowChild(UIElement child)
            : this()
        {
            this.gControl.Children.Clear();
            this.gControl.Children.Add(child);
        }

        void PageWindowChild_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.Close();
            }
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        public UIElement GetControl
        {
            get
            {
                if (this.gControl.Children.Count > 0)
                {
                    return this.gControl.Children[0];
                }
                else
                {
                    return null;
                }
            }
        }
    }
}

