using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using PlatformClient.Common;
using PlatformClient.Common.Interfaces;
using PlatformClient.EventDesign.Core.Lib;
using PlatformClient.Extend.Core;

namespace PlatformClient.EventDesign.Core
{
    /// <summary>
    /// 
    /// </summary>
    public partial class ArrowLine : UserControl, IArrowLine
    {
        /// <summary>
        /// 删除箭头
        /// </summary>
        public event RoutedEventHandler DeleteLine;
        /// <summary>
        /// 
        /// </summary>
        public event RoutedEventHandler ResetDirection;
        private Point startPoint;
        private Point endPoint;
        ContextMenu _Cmenu = new ContextMenu();
        private string _LineGuid;

        /// <summary>
        /// 
        /// </summary>
        public string LineGuid { get { return _LineGuid; } set { _LineGuid = value; } }
        /// <summary>
        /// 
        /// </summary>
        public string CtrName { get { return this.Name; } set { this.Name = value; } }

        /// <summary>
        /// 箭头尾部所在的控件
        /// </summary>
        public UIElement ArrowFootControl { get; set; }
        /// <summary>
        /// 箭头头部所在的控件
        /// </summary>
        public UIElement ArrowCapControl { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Point StartPoint
        {
            get { return startPoint; }
            set
            {
                startPoint = value;
                Update();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Point EndPoint
        {
            get { return endPoint; }
            set
            {
                endPoint = value;
                Update();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public ArrowLine()
        {
            InitializeComponent();

            MenuItem mi = new MenuItem();
            mi.Click += new RoutedEventHandler(mi_Click);
            mi.Header = "删除";
            mi.Tag = "delete";
            _Cmenu.Items.Add(mi);
            _Cmenu.Visibility = System.Windows.Visibility.Collapsed;

            mi = new MenuItem();
            mi.Click += new RoutedEventHandler(mi_Click);
            mi.Header = "修改方向";
            mi.Tag = "ResetDirection";
            _Cmenu.Items.Add(mi);

            mi = new MenuItem();
            mi.Click += new RoutedEventHandler(mi_Click);
            mi.Header = "说明";
            mi.Tag = "Comment";
            _Cmenu.Items.Add(mi);
            _Cmenu.MouseLeave += new MouseEventHandler(_Cmenu_MouseLeave);

            this.MouseRightButtonDown += new MouseButtonEventHandler(ArrowLine_MouseRightButtonDown);
            this.txtComment.LostFocus += new RoutedEventHandler(txtComment_LostFocus);
            this.LayoutRoot.Children.Add(_Cmenu);
        }

        void _Cmenu_MouseLeave(object sender, MouseEventArgs e)
        {
            _Cmenu.Visibility = System.Windows.Visibility.Collapsed;
        }

        void txtComment_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtComment.Text.IsNullOrEmpty() || txtComment.Text.Equals("请输入说明..."))
            {
                txtComment.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                txtComment.IsEnabled = false;
            }
        }

        void ArrowLine_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            var p = e.GetPosition(sender as UIElement);
            Canvas.SetLeft(_Cmenu, p.X);
            Canvas.SetTop(_Cmenu, p.Y);
            _Cmenu.Visibility = System.Windows.Visibility.Visible;
        }

        void mi_Click(object sender, RoutedEventArgs e)
        {
            MenuItem mi = sender as MenuItem;
            string state = mi.Tag.ToString();
            switch (state)
            {
                case "delete":
                    if (null != DeleteLine)
                    {
                        DeleteLine(this, e);
                    }
                    break;
                case "ResetDirection":
                    if (null != ResetDirection)
                    {
                        ResetDirection(this, e);
                    }

                    break;
                case "Comment":
                    txtComment.IsEnabled = true;
                    txtComment.Visibility = System.Windows.Visibility.Visible;
                    break;
            }
            _Cmenu.Visibility = System.Windows.Visibility.Collapsed;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="startPoint"></param>
        /// <param name="endPoint"></param>
        public ArrowLine(Point startPoint, Point endPoint)
            : this()
        {
            this.startPoint = startPoint;
            this.endPoint = endPoint;
            Update();
        }

        private void Update()
        {
            double angleOfLine = Math.Atan2((endPoint.Y - startPoint.Y), (endPoint.X - startPoint.X)) * 180 / Math.PI;

            Connector.X1 = startPoint.X;
            Connector.Y1 = startPoint.Y;
            Connector.X2 = endPoint.X;
            Connector.Y2 = endPoint.Y;
            Connector.StrokeThickness = 1;
            Connector.Stroke = new SolidColorBrush(Colors.Black);

            Cap.X1 = (startPoint.X + endPoint.X) / 2;
            Cap.Y1 = (startPoint.Y + endPoint.Y) / 2;
            Cap.X2 = (startPoint.X + endPoint.X) / 2;
            Cap.Y2 = (startPoint.Y + endPoint.Y) / 2;
            Cap.StrokeEndLineCap = PenLineCap.Triangle;
            Cap.StrokeThickness = 20;
            Cap.Stroke = new SolidColorBrush(Colors.Black);
            CapRotateTransform.Angle = angleOfLine;
            CapRotateTransform.CenterX = (this.StartPoint.X + this.endPoint.X) / 2;
            CapRotateTransform.CenterY = (this.StartPoint.Y + this.endPoint.Y) / 2;

            Foot.X1 = endPoint.X;
            Foot.Y1 = endPoint.Y;
            Foot.X2 = endPoint.X;
            Foot.Y2 = endPoint.Y;
            Foot.StrokeEndLineCap = PenLineCap.Triangle;
            Foot.StrokeThickness = 20;
            Foot.Stroke = new SolidColorBrush(Colors.Black);
            FootRotateTransform.Angle = angleOfLine;
            FootRotateTransform.CenterX = this.EndPoint.X;
            FootRotateTransform.CenterY = this.EndPoint.Y;

            Canvas.SetLeft(txtComment, (this.endPoint.X + this.startPoint.X) / 2);
            Canvas.SetTop(txtComment, (this.endPoint.Y + this.startPoint.Y) / 2);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            try
            {
                foreach (var v in _Cmenu.Items)
                {
                    MenuItem mi = v as MenuItem;
                    if (null == mi)
                    {
                        continue;
                    }
                    mi.Click -= mi_Click;
                }
                this.MouseRightButtonDown -= ArrowLine_MouseRightButtonDown;
                this.txtComment.LostFocus -= txtComment_LostFocus;
            }
            catch { }
        }

        /// <summary>
        /// 
        /// </summary>
        public string ExportLocation()
        {
            ArrowLineInfo ail = new ArrowLineInfo();
            ail.Type = this.GetType().Name;
            ail.Name = this.Name;
            ail.Guid = this.LineGuid;
            ail.StartX = StartPoint.X;
            ail.StartY = StartPoint.Y;
            ail.EndX = EndPoint.X;
            ail.EndY = EndPoint.Y;
            var aa = ail.ToXElement("ArrowLine");
            return aa.ToString();
        }
    }
}
