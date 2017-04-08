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
    public partial class DesigningPage : UserControl
    {
        bool isMouseCaptured;
        double mouseVerticalPosition;
        double mouseHorizontalPosition;
        /// <summary>
        /// 
        /// </summary>
        public DesigningPage()
        {
            InitializeComponent();
        }


        private void btnAddControl_Click(object sender, RoutedEventArgs e)
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Handle_MouseDown(object sender, MouseEventArgs e)
        {
            Rectangle item = sender as Rectangle;
            mouseVerticalPosition = e.GetPosition(rootCanvas).Y;
            mouseHorizontalPosition = e.GetPosition(rootCanvas).X;
            isMouseCaptured = true;
            item.CaptureMouse();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Handle_MouseMove(object sender, MouseEventArgs e)
        {
            Rectangle item = sender as Rectangle;
            if (isMouseCaptured)
            {

                double maxTop = rootCanvas.Height - item.Height;
                double maxLeft = rootCanvas.Width - item.Width;


                // Calculate the current position of the object.
                double deltaV = e.GetPosition(rootCanvas).Y - mouseVerticalPosition;
                double deltaH = e.GetPosition(rootCanvas).X - mouseHorizontalPosition;
                double newTop = deltaV + (double)item.GetValue(Canvas.TopProperty);
                double newLeft = deltaH + (double)item.GetValue(Canvas.LeftProperty);


                newTop = newTop > maxTop ? maxTop : (newTop < 0 ? 0 : newTop);
                newLeft = newLeft > maxLeft ? maxLeft : (newLeft < 0 ? 0 : newLeft);

                this.txtTop.Text = newTop.ToString();
                this.txtLeft.Text = newLeft.ToString();



                // Set new position of object.
                item.SetValue(Canvas.TopProperty, newTop);
                item.SetValue(Canvas.LeftProperty, newLeft);

                // Update position global variables.
                mouseVerticalPosition = e.GetPosition(rootCanvas).Y;
                mouseHorizontalPosition = e.GetPosition(rootCanvas).X;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Handle_MouseUp(object sender, MouseEventArgs e)
        {
            Rectangle item = sender as Rectangle;
            isMouseCaptured = false;
            item.ReleaseMouseCapture();
            mouseVerticalPosition = -1;
            mouseHorizontalPosition = -1;
        }

        private void Rectangle_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Rectangle item = sender as Rectangle;
            item.Height = e.NewSize.Height;
            item.Width = e.NewSize.Width;
            item.Cursor = Cursors.SizeNESW;
        }
    }
}
