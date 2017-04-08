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

namespace PGrid
{
    /// <summary>
    /// 
    /// </summary>
    public partial class MainPage : UserControl
    {
        /// <summary>
        /// 
        /// </summary>
        public MainPage()
        {
            InitializeComponent();
        }

        private void ButtonEx_Click(object sender, RoutedEventArgs e)
        {
            pgc.ReadAttribute(sender as UIElement);
        }
    }
}
