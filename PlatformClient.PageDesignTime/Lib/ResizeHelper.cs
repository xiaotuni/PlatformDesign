using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using PlatformClient.Common.Interfaces;
using PlatformClient.Model.Method;

namespace PlatformClient.PageDesignTime.Lib
{
    /// <summary>
    /// 控件尺寸变化处理类
    /// </summary>
    public class ResizeHelper
    {
        /// <summary>
        /// 触发尺寸改变的部件
        /// </summary>
        Auchor triggerPart;
        /// <summary>
        /// 要尺寸改变的部件
        /// </summary>
        DesignRectangle resizePart;
        /// <summary>
        /// 开始移动
        /// </summary>
        bool startMoving = false;
        /// <summary>
        /// 修改前的位置
        /// </summary>
        Point prevMousePos;
        /// <summary>
        /// 画布
        /// </summary>
        Panel ParentCtr;
        /// <summary>
        /// 改变大小事件
        /// </summary>
        public event EventHandler<ResizeEventArgs> ResizeChanged;
        /// <summary>
        /// 鼠标左键弹起事件
        /// </summary>
        public event MouseButtonEventHandler MouseLeftButtonUp;

        void _ResizeChangedMethod(object sender, ResizeEventArgs e)
        {
            if (null != ResizeChanged)
            {
                ResizeChanged(sender, e);
            }
        }
        void _MouseLeftButtonUpMethod(object sender, MouseButtonEventArgs e)
        {
            if (null != MouseLeftButtonUp)
            {
                MouseLeftButtonUp(sender, e);
            }
        }

        /// <summary>
        /// 邦定要改变部件大小的方法
        /// </summary>
        /// <param name="triggerPart">触发尺寸改变的部件</param>
        /// <param name="resizePart">要尺寸改变的部件</param>
        public void BindResizeFunction(Auchor triggerPart, DesignRectangle resizePart)
        {
            this.triggerPart = triggerPart;
            this.resizePart = resizePart;
            ParentCtr = resizePart.Parent as Panel;

            this.triggerPart.MouseLeftButtonDown += new MouseButtonEventHandler(triggerPart_MouseLeftButtonDown);
            this.triggerPart.MouseLeftButtonUp += new MouseButtonEventHandler(triggerPart_MouseLeftButtonUp);
            this.triggerPart.MouseMove += new MouseEventHandler(triggerPart_MouseMove);
        }

        /// <summary>
        /// 移动事件处理方法
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void triggerPart_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.startMoving)
            {
                ResizeEventArgs args = new ResizeEventArgs();
                //if (null == ParentCtr)
                //{
                //    ParentCtr = resizePart.Parent as Panel;
                //}
                Point curMousePos = e.GetPosition(ParentCtr);//画布的位置
                args.CurPoint = curMousePos;
                args.DragDirection = triggerPart.Location;
                double top = (double)this.resizePart.GetValue(Canvas.TopProperty);
                double left = (double)this.resizePart.GetValue(Canvas.LeftProperty);
                double xValue = curMousePos.X - this.prevMousePos.X;    //-->X轴坐标 水平
                double yValue = curMousePos.Y - this.prevMousePos.Y;    //-->Y轴坐标 垂直
                double Height = this.resizePart.Height;                 //-->高
                double Width = this.resizePart.Width;                   //-->底

                switch (triggerPart.Location)
                {
                    case AuchorLocation.上右:
                        //-->xValue >0; y < 0;
                        //-->控件大小x变大，y变大
                        //-->控件的位置top变小，left不变
                        Width += xValue;
                        Height -= yValue;
                        top += yValue;
                        //top = (double)this.resizePart.GetValue(Canvas.TopProperty) + yValue;
                        //this.resizePart.SetValue(Canvas.TopProperty, top);
                        break;
                    case AuchorLocation.上中:
                        //-->控件x不变，y变大
                        //-->top变小，left不变
                        Height -= yValue;
                        top += yValue;
                        //top = (double)this.resizePart.GetValue(Canvas.TopProperty) + yValue;
                        //this.resizePart.SetValue(Canvas.TopProperty, top);
                        break;
                    case AuchorLocation.上左:
                        //-->控件的坐标x变小、y变小，控件大小是增加 x+width、y+height
                        //--> xValue = 10 - 20 = -10;
                        Width -= xValue;
                        Height -= yValue;
                        top += yValue;
                        left += xValue;
                        //top = (double)this.resizePart.GetValue(Canvas.TopProperty) + yValue;
                        //left = (double)this.resizePart.GetValue(Canvas.LeftProperty) + xValue;
                        //this.resizePart.SetValue(Canvas.TopProperty, top);
                        //this.resizePart.SetValue(Canvas.LeftProperty, left);
                        break;
                    case AuchorLocation.下右:
                        Width += xValue;
                        Height += yValue;
                        break;
                    case AuchorLocation.下中:
                        Height += yValue;
                        break;
                    case AuchorLocation.下左:
                        Width -= xValue;
                        Height += yValue;
                        left += xValue;
                        //left = (double)this.resizePart.GetValue(Canvas.LeftProperty) + xValue;
                        //this.resizePart.SetValue(Canvas.LeftProperty, left);
                        break;
                    case AuchorLocation.右中:
                        //-->X轴发生变化
                        Width += xValue;
                        break;
                    case AuchorLocation.左中:
                        //-->X轴的值变大,宽变大或变小
                        Width -= xValue;
                        left += xValue;
                        //left = (double)this.resizePart.GetValue(Canvas.LeftProperty) + xValue;
                        //this.resizePart.SetValue(Canvas.LeftProperty, left);
                        break;
                }
                //Width = (int)Width;
                //Height = (int)Height;
                if (Width > 16)
                {
                    this.resizePart.Width = Width;
                }
                else
                {
                    this.resizePart.Width = 16;
                }
                if (Height > 16)
                {
                    this.resizePart.Height = Height;
                }
                else
                {
                    this.resizePart.Height = 16;
                }
                //top = (int)top;
                //left = (int)left;
                this.resizePart.SetValue(Canvas.TopProperty, top);
                this.resizePart.SetValue(Canvas.LeftProperty, left);


                this.prevMousePos = curMousePos;
                _ResizeChangedMethod(this, null);
            }
        }

        /// <summary>
        /// 鼠标左键弹起事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void triggerPart_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.startMoving)
            {
                this.triggerPart.ReleaseMouseCapture();
                this.startMoving = false;
            }
            e.Handled = true;
            _MouseLeftButtonUpMethod(this, e);
        }

        /// <summary>
        /// 鼠标左键按下事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void triggerPart_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!this.startMoving)
            {
                if (ParentCtr == null) ParentCtr = resizePart.Parent as Panel;
                this.startMoving = true;
                this.triggerPart.CaptureMouse();
                prevMousePos = e.GetPosition(ParentCtr);
            }
            e.Handled = true;
        }
    }
}
