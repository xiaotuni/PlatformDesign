using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using PlatformClient.Common;
using PlatformClient.Common.Interfaces;
using PlatformClient.PageDesignTime.Lib;
using PlatformClient.Utility;
using PlatformClient.Utility.Events;
using PlatformClient.Extend.Core;

namespace PlatformClient.PageDesignTime
{
    /// <summary>
    /// 页面设计界面
    /// </summary>
    public partial class PageDesign : UserControl, IDisposable
    {
        /// <summary>
        /// 控件位置发生变发
        /// </summary>
        public event EventHandler<ControlModifyPropertyEventArgs> ControlPosition;
        /// <summary>
        /// 删除控件
        /// </summary>
        public event EventHandler<ControlDeleteEventArgs> ControlDelete;
        /// <summary>
        /// 线
        /// </summary>
        List<Line> gridlines = new List<Line>();
        /// <summary>
        /// 当前控件
        /// </summary>
        private FrameworkElement _CurrentDesignCtrl;
        /// <summary>
        /// 是否是鼠标
        /// </summary>
        private bool isMouseCaptured;
        /// <summary>
        /// 垂直位置
        /// </summary>
        private double mouseVerticalPosition;
        /// <summary>
        /// 水平位置
        /// </summary>
        private double mouseHorizontalPosition;
        /// <summary>
        /// 页面框架接口
        /// </summary>
        IPageDesignFramework _IDesigntime;
        double _maxLeft;
        double _maxTop;
        private bool isMoveByDesignRectangle;
        Dictionary<String, FrameworkElement> dictControl = new Dictionary<string, FrameworkElement>();
        DesignRectangle dr;
        TextBox txtHide;
        private Line line_Top;
        private Line line_Left;
        double _PageWidth;
        double _PageHeight;
        /// <summary>
        /// 
        /// </summary>
        public double PageWidth
        {
            get { return _PageWidth; }
            set
            {
                _PageWidth = value;
                cvsPanle.PageWidth = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public double PageHeight
        {
            get { return _PageHeight; }
            set
            {
                _PageHeight = value;
                cvsPanle.PageHeight = value;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public PageDesign()
        {
            InitializeComponent();
            if (null == cvsPanle)
            {
                cvsPanle = new PCCanvas();
                //b_canvas.Child = cvsPanle;
            }
            dr = cvsPanle.dr;
            txtHide = cvsPanle.txtHide;
            line_Top = cvsPanle.line_Top;
            line_Left = cvsPanle.line_Left;
            line_Top.Visibility = Visibility.Collapsed;
            line_Left.Visibility = Visibility.Collapsed;

            dr.MouseMove += dr_MouseMove;
            dr.MouseLeftButtonDown += dr_MouseLeftButtonDown;
            dr.MouseLeftButtonUp += dr_MouseLeftButtonUp;
            dr.MouseLeftUp += dr_MouseLeftUp;
            dr.ResizeChanged += dr_ResizeChanged;
            dr.Visibility = Visibility.Collapsed;

            cvsPanle.MouseLeftButtonUp += cvsPanle_MouseLeftButtonUp;
            this.Loaded += PageDesign_Loaded;
            this.SizeChanged += PageDesign_SizeChanged;

            this.MouseRightButtonDown += PageDesign_MouseRightButtonDown;
        }

        void PageDesign_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        void cvsPanle_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //-->用来处理将当前的画布存放到属性页面上去。
            _IDesigntime.UpdateCurrentSelectedCtrl(this.cvsPanle);
        }

        void PageDesign_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            SetGridLines();

            line_Top.X1 = 0;
            line_Top.Y1 = e.NewSize.Height / 2;
            line_Top.X2 = e.NewSize.Width;
            line_Top.Y2 = e.NewSize.Height / 2;

            line_Left.X1 = e.NewSize.Width / 2;
            line_Left.Y1 = 0;
            line_Left.X2 = e.NewSize.Width / 2;
            line_Left.Y2 = e.NewSize.Height;
        }

        void UpdateLineTopOrLeftPoint(Point p)
        {
            if (!double.IsNaN(p.Y))
            {
                line_Top.Y1 = p.Y;
                line_Top.Y2 = p.Y;
            }
            if (!double.IsNaN(p.X))
            {
                line_Left.X1 = p.X;
                line_Left.X2 = p.X;
            }
        }

        /// <summary>
        /// 画格线的方法
        /// </summary>
        void SetGridLines()
        {
            //return;
            SolidColorBrush brush = new SolidColorBrush();
            brush.Color = Color.FromArgb(255, 160, 160, 160);
            double thickness = 0.3;
            double top = 0;
            double left = 0;

            double width = cvsPanle.ActualWidth;
            double height = cvsPanle.ActualHeight;
            if (width <= 0)
            {
                width = 480d;
            }
            if (height <= 0)
            {
                height = 480d;
            }
            double stepLength = 20;

            double x, y;
            x = left + stepLength;
            y = top;
            foreach (Line l in gridlines)
            {
                cvsPanle.Children.Remove(l);
            }
            gridlines.Clear();
            while (x < width + left)
            {
                Line line = new Line();
                line.X1 = x;
                line.Y1 = y;
                line.X2 = x;
                line.Y2 = y + height;
                line.SetValue(Canvas.ZIndexProperty, 0);

                line.Stroke = brush;
                line.StrokeThickness = thickness;
                line.Stretch = Stretch.Fill;
                cvsPanle.Children.Insert(0, line);
                gridlines.Add(line);
                x += stepLength;
            }

            x = left;
            y = top + stepLength;

            while (y < height + top)
            {
                Line line = new Line();
                line.X1 = x;
                line.Y1 = y;
                line.X2 = x + width;
                line.Y2 = y;
                line.SetValue(Canvas.ZIndexProperty, 0);

                line.Stroke = brush;
                line.Stretch = Stretch.Fill;
                line.StrokeThickness = thickness;
                cvsPanle.Children.Insert(0, line);
                gridlines.Add(line);
                y += stepLength;
            }
        }

        void PageDesign_Loaded(object sender, RoutedEventArgs e)
        {
            SetGridLines();

            this.KeyDown += PageDesign_KeyDown;
            this.KeyUp += PageDesign_KeyUp;
        }

        void PageDesign_KeyUp(object sender, KeyEventArgs e)
        {
            //if (e.Key == Key.Delete)
            //{
            //    DeleteCurrentSelectedControl();
            //    return;
            //}
            //-->修改xaml里的值。
            double newTop = (double)dr.GetValue(Canvas.TopProperty);
            double newLeft = (double)dr.GetValue(Canvas.LeftProperty);
            ControlModifyPropertyEventArgs ce = new ControlModifyPropertyEventArgs();
            ce.DictProperty = new Dictionary<string, object>();
            ce.DictProperty.Add("Canvas.Top", newTop);
            ce.DictProperty.Add("Canvas.Left", newLeft);
            ce.DictProperty.Add("Height", dr.Height);
            ce.DictProperty.Add("Width", dr.Width);
            ce.ControlName = string.Format("{0}", _CurrentDesignCtrl.Name);
            _ControlPositionMethod(this, ce);
        }

        void PageDesign_KeyDown(object sender, KeyEventArgs e)
        {
            if (null == _CurrentDesignCtrl)
            {
                return;
            }
            SetControlFocus(_CurrentDesignCtrl);
            double setp = 1;
            double left = (double)dr.GetValue(Canvas.LeftProperty);
            double top = (double)dr.GetValue(Canvas.TopProperty);
            double newLeft = left;
            double newTop = top;

            switch (e.Key)
            {
                case Key.Left:
                    newLeft = left - setp;
                    break;
                case Key.Right:
                    newLeft = left + setp;
                    break;
                case Key.Up:
                    newTop = top - setp;
                    break;
                case Key.Down:
                    newTop = top + setp;
                    break;
                case Key.Delete:
                    DeleteCurrentSelectedControl();
                    return;
            }
            dr.SetValue(Canvas.TopProperty, newTop);
            dr.SetValue(Canvas.LeftProperty, newLeft);
            this._CurrentDesignCtrl.SetValue(Canvas.TopProperty, newTop);
            this._CurrentDesignCtrl.SetValue(Canvas.LeftProperty, newLeft);
            isMouseCaptured = false;
        }

        /// <summary>
        /// 删除当前选择的控件
        /// </summary>
        void DeleteCurrentSelectedControl()
        {
            if (null == _CurrentDesignCtrl)
            {
                return;
            }
            ControlDeleteEventArgs de = new ControlDeleteEventArgs();
            de.ControlName = this._CurrentDesignCtrl.Name;
            _ControlDeleteMethod(this, de);

            //-->删除字典里的控件
            dictControl.Remove(de.ControlName);

            //-->获取当前的控件所在索引
            int indexs = this.cvsPanle.Children.IndexOf(this._CurrentDesignCtrl);
            this.cvsPanle.Children.Remove(this._CurrentDesignCtrl);
            _CurrentDesignCtrl = null;
            //-->
            if (0 == this.cvsPanle.Children.Count)
            {
                dr.Visibility = System.Windows.Visibility.Collapsed;
                return;
            }
            if (indexs >= this.cvsPanle.Children.Count)
            {
                indexs = this.cvsPanle.Children.Count - 1;
            }
            this._CurrentDesignCtrl = this.cvsPanle.Children[indexs] as FrameworkElement;
            if (this._CurrentDesignCtrl is Line || _CurrentDesignCtrl.Equals(dr))
            {
                dr.Visibility = System.Windows.Visibility.Collapsed;
                return;
            }
            dr.Width = this._CurrentDesignCtrl.Width;
            dr.Height = this._CurrentDesignCtrl.Height;
            dr.SetValue(Canvas.TopProperty, (double)this._CurrentDesignCtrl.GetValue(Canvas.TopProperty));
            dr.SetValue(Canvas.LeftProperty, (double)this._CurrentDesignCtrl.GetValue(Canvas.LeftProperty));
        }

        void dr_KeyDown(object sender, KeyEventArgs e)
        {
            isMouseCaptured = true;
            isMoveByDesignRectangle = false;
        }

        /// <summary>
        /// DesignRectangle控件上的Auchor控件鼠标左键弹起
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void dr_MouseLeftUp(object sender, MouseButtonEventArgs e)
        {
            DesignRectangle item = sender as DesignRectangle;
            if (null != _CurrentDesignCtrl)
            {
                _CurrentDesignCtrl.Width = item.Width;
                _CurrentDesignCtrl.Height = item.Height;
            }
            ControlModifyPropertyEventArgs ce = new ControlModifyPropertyEventArgs();
            ce.DictProperty.Add("Height", dr.Height);
            ce.DictProperty.Add("Width", dr.Width);
            ce.ControlName = string.Format("{0}", _CurrentDesignCtrl.GetPropertyValue("Name"));// Wrapper.GetPropertyValue(_CurrentDesignCtrl, "Name"));
            _ControlPositionMethod(this, ce);
        }

        /// <summary>
        /// DesignRectangle控件大小更变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void dr_ResizeChanged(object sender, ResizeEventArgs e)
        {
            DesignRectangle item = sender as DesignRectangle;
            if (null != _CurrentDesignCtrl)
            {
                _CurrentDesignCtrl.Width = item.Width;
                _CurrentDesignCtrl.Height = item.Height;
                //-->修改top、left的值
                _CurrentDesignCtrl.SetValue(Canvas.LeftProperty, item.GetValue(Canvas.LeftProperty));
                _CurrentDesignCtrl.SetValue(Canvas.TopProperty, item.GetValue(Canvas.TopProperty));
                //-->通知txaml\xml修改
            }
        }

        /// <summary>
        /// DesignRectangle控件鼠标左键弹起
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void dr_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            DesignRectangle item = sender as DesignRectangle;
            item.ReleaseMouseCapture();
            mouseVerticalPosition = -1;
            mouseHorizontalPosition = -1;
            if (null != _CurrentDesignCtrl && !this._CurrentDesignCtrl.Equals(item.Tag as FrameworkElement))
            {
                item.Tag = this._CurrentDesignCtrl;
                SetControlFocus(_CurrentDesignCtrl);
            }
            _IDesigntime.UpdateCurrentSelectedCtrl(this._CurrentDesignCtrl);
            if (isMoveByDesignRectangle)
            {   //-->判断当前选中的控件与
                double newTop = (double)item.GetValue(Canvas.TopProperty);
                double newLeft = (double)item.GetValue(Canvas.LeftProperty);
                ControlModifyPropertyEventArgs ce = new ControlModifyPropertyEventArgs();
                ce.DictProperty.Add("Canvas.Top", newTop);
                ce.DictProperty.Add("Canvas.Left", newLeft);
                ce.ControlName = string.Format("{0}", _CurrentDesignCtrl.Name);
                _ControlPositionMethod(this, ce);
            }
            isMouseCaptured = false;
            e.Handled = true;

            line_Top.Visibility = System.Windows.Visibility.Collapsed;
            line_Left.Visibility = System.Windows.Visibility.Collapsed;
        }

        /// <summary>
        /// DesignRectangle控件鼠标左键按下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void dr_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DesignRectangle item = sender as DesignRectangle;
            mouseVerticalPosition = e.GetPosition(cvsPanle).Y;
            mouseHorizontalPosition = e.GetPosition(cvsPanle).X;
            isMouseCaptured = true;
            isMoveByDesignRectangle = false;
            item.CaptureMouse();
            e.Handled = true;
            line_Top.Visibility = System.Windows.Visibility.Visible;
            line_Left.Visibility = System.Windows.Visibility.Visible;
        }

        /// <summary>
        /// DesignRectangle控件移动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void dr_MouseMove(object sender, MouseEventArgs e)
        {
            DesignRectangle item = sender as DesignRectangle;
            if (isMouseCaptured)
            {
                isMoveByDesignRectangle = true;
                _maxLeft = cvsPanle.ActualWidth - item.Width;
                _maxTop = cvsPanle.ActualHeight - item.Height;
                var p = e.GetPosition(cvsPanle);
                double deltaV = p.Y - mouseVerticalPosition;
                double deltaH = p.X - mouseHorizontalPosition;
                double newTop = deltaV + (double)item.GetValue(Canvas.TopProperty);
                double newLeft = deltaH + (double)item.GetValue(Canvas.LeftProperty);
                newLeft = newLeft > _maxLeft ? _maxLeft : (newLeft < 0 ? 0 : newLeft);
                newTop = newTop > _maxTop ? _maxTop : (newTop < 0 ? 0 : newTop);
                // Set new position of object.
                item.SetValue(Canvas.TopProperty, newTop);
                item.SetValue(Canvas.LeftProperty, newLeft);

                UpdateLineTopOrLeftPoint(new Point(newLeft + item.Width, newTop));

                if (null != _CurrentDesignCtrl)
                {
                    _CurrentDesignCtrl.SetValue(Canvas.TopProperty, newTop);
                    _CurrentDesignCtrl.SetValue(Canvas.LeftProperty, newLeft);
                    _CurrentDesignCtrl.Width = item.Width;
                    _CurrentDesignCtrl.Height = item.Height;

                    //-->这里非常耗费资源。当鼠标弹起来的时候，更新会更好些。
                    //ControlModifyPropertyEventArgs ce = new ControlModifyPropertyEventArgs();
                    //ce.DictProperty.Add("Canvas.Top", newTop);
                    //ce.DictProperty.Add("Canvas.Left", newLeft);
                    //ce.DictProperty.Add("Height", dr.Height);
                    //ce.DictProperty.Add("Width", dr.Width);
                    //ce.ControlName = string.Format("{0}", Wrapper.GetPropertyValue(_CurrentDesignCtrl, "Name"));
                    //_ControlPositionMethod(this, ce);
                }
                //Update position global variables.
                mouseVerticalPosition = e.GetPosition(cvsPanle).Y;
                mouseHorizontalPosition = e.GetPosition(cvsPanle).X;
            }
        }

        /// <summary>
        /// 增加子控件
        /// </summary>
        /// <param name="fe"></param>
        internal void AddChild(FrameworkElement fe)
        {
            AddChild(fe, true);
        }

        /// <summary>
        /// 增加子控件
        /// </summary>
        /// <param name="fe">要增加的控件</param>
        /// <param name="isUpdate">是否更新</param>
        internal void AddChild(FrameworkElement fe, bool isUpdate)
        {
            this.cvsPanle.Children.Add(fe);

            if (!dictControl.ContainsKey(fe.Name))
            {
                dictControl.Add(fe.Name, fe);
            }
            UpdateSelectedControl(fe, isUpdate);
        }
        /// <summary>
        /// 更新当前选中控件
        /// </summary>
        /// <param name="controlName"></param>
        internal void UpdateSelectedControl(string controlName)
        {
            var _control = this.FindControl(controlName);
            UpdateSelectedControl(_control, true);
        }

        void UpdateSelectedControl(FrameworkElement fe, bool isUpdate)
        {
            if (null == fe)
            {
                return;
            }

            this._CurrentDesignCtrl = fe;
            dr.Tag = fe;

            fe.Width = fe.Width == double.NaN ? 120 : fe.Width;
            fe.Height = fe.Height == double.NaN ? 30 : fe.Height;
            fe.MouseEnter += fe_MouseEnter;

            SetControlFocus(fe);

            dr.Visibility = System.Windows.Visibility.Visible;
            dr.Width = fe.Width;
            dr.Height = fe.Height;
            double top = (double)fe.GetValue(Canvas.TopProperty);
            double left = (double)fe.GetValue(Canvas.LeftProperty);
            dr.SetValue(Canvas.TopProperty, top);
            dr.SetValue(Canvas.LeftProperty, left);
            dr.SetValue(Canvas.ZIndexProperty, 100);

            if (isUpdate)
            {
                _IDesigntime.UpdateCurrentSelectedCtrl(fe);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void fe_MouseEnter(object sender, MouseEventArgs e)
        {
            this.txtHide.Focus();
            FrameworkElement fe = sender as FrameworkElement;
            if (null == fe)
            {
                return;
            }
            dr.Visibility = System.Windows.Visibility.Visible;
            dr.Width = fe.Width;
            dr.Height = fe.Height;
            double top = (double)fe.GetValue(Canvas.TopProperty);
            double left = (double)fe.GetValue(Canvas.LeftProperty);
            dr.SetValue(Canvas.TopProperty, top);
            dr.SetValue(Canvas.LeftProperty, left);
            dr.SetValue(Canvas.ZIndexProperty, 100);
            dr.Focus();
            this._CurrentDesignCtrl = fe;
            UpdateLineTopOrLeftPoint(new Point(left + fe.Width, top));
        }

        /// <summary>
        /// 设置控件焦点
        /// </summary>
        /// <param name="ui"></param>
        void SetControlFocus(UIElement ui)
        {
            if (null == txtHide)
            {
                txtHide = new TextBox();
            }
            txtHide.Focus();
            txtHide.Width = 1;
        }

        /// <summary>
        /// 设计
        /// </summary>
        /// <param name="iPage"></param>
        internal void SetDesigntimeInterface(IPageDesignFramework iPage)
        {
            _IDesigntime = iPage;
            cvsPanle.IPageDesignFramework = iPage;
        }

        /// <summary>
        /// 控件位置改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _ControlPositionMethod(object sender, ControlModifyPropertyEventArgs e)
        {
            if (null != ControlPosition)
            {
                ControlPosition(sender, e);
            }
        }

        /// <summary>
        /// 控件删除操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _ControlDeleteMethod(object sender, ControlDeleteEventArgs e)
        {
            if (null != ControlDelete)
            {
                ControlDelete(sender, e);
            }
        }

        /// <summary>
        /// 更新DesignRectangle控件大小
        /// </summary>
        /// <param name="ctrl"></param>
        internal void UpdateDesignRectangle(FrameworkElement ctrl)
        {
            if (null == ctrl || null == dr.Tag)
            {
                return;
            }
            if (dr.Tag.Equals(ctrl))
            {
                dr.Height = ctrl.Height;
                dr.Width = ctrl.Width;
                dr.SetValue(Canvas.LeftProperty, (double)ctrl.GetValue(Canvas.LeftProperty));
                dr.SetValue(Canvas.TopProperty, (double)ctrl.GetValue(Canvas.TopProperty));
            }
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            dr.MouseMove -= dr_MouseMove;
            dr.MouseLeftButtonDown -= dr_MouseLeftButtonDown;
            dr.MouseLeftButtonUp -= dr_MouseLeftButtonUp;
            dr.MouseLeftUp -= dr_MouseLeftUp;
            dr.ResizeChanged -= dr_ResizeChanged;
            Loaded -= PageDesign_Loaded;
            SizeChanged -= PageDesign_SizeChanged;
            KeyDown -= PageDesign_KeyDown;
            KeyUp -= PageDesign_KeyUp;
            this._IDesigntime = null;
            this._maxLeft = 0;
            this._maxTop = 0;

            foreach (var v in this.cvsPanle.Children)
            {
                FrameworkElement fe = v as FrameworkElement;
                if (null == fe)
                {
                    continue;
                }
                fe.MouseEnter -= fe_MouseEnter;
            }
            this.cvsPanle.Children.Clear();
        }

        /// <summary>
        /// 删除子控件
        /// </summary>
        /// <param name="_ControlName"></param>
        internal void DeleteChilren(string _ControlName)
        {
            UIElement ui = this.cvsPanle.FindName(_ControlName) as UIElement;
            if (null == ui)
            {
                return;
            }
            this.cvsPanle.Children.Remove(ui);
        }
        /// <summary>
        /// 在切换tableControl控件的时候，更新属性框信息
        /// </summary>
        internal void UpdatePropertyGrid()
        {
            this._IDesigntime.UpdateCurrentSelectedCtrl(this._CurrentDesignCtrl);
        }
        /// <summary>
        /// 查找控件
        /// </summary>
        /// <param name="controlName"></param>
        internal FrameworkElement FindControl(string controlName)
        {
            if (dictControl.ContainsKey(controlName))
            {
                return dictControl[controlName];
            }
            else
            {
                var ui = this.cvsPanle.FindName(controlName) as FrameworkElement;
                if (null == ui)
                {
                    return null;
                }
                dictControl.Add(controlName, ui);
                return ui;
            }
        }
    }
}
