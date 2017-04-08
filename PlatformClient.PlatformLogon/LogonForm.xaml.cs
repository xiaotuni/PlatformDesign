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

namespace PlatformClient.PlatformLogon
{
    public partial class LogonForm : UserControl
    {
        public LogonForm()
        {
            InitializeComponent();

            this.btnLogon.Click += btn_Click;
            this.btnLogout.Click += btnLogout_Click;
        }

        void btnLogout_Click(object sender, RoutedEventArgs e)
        {

        }

        void btn_Click(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
