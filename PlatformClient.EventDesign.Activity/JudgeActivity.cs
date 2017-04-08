using System.Collections.Generic;
using System.Windows;
using PlatformClient.Common.Interfaces;
using PlatformClient.Common.Lib;
using PlatformClient.EventDesign.Core.Activitys;
using PlatformClient.Model.Method;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows.Media;

namespace PlatformClient.EventDesign.Activity
{
    /// <summary>
    /// 
    /// </summary>
    public partial class JudgeActivity : XtnBaseActivity, IActivity
    {
        Polygon pJudge = new Polygon();
        /// <summary>
        /// 
        /// </summary>
        public JudgeActivity()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(JudgeActivity_Loaded);
        }

        void JudgeActivity_Loaded(object sender, RoutedEventArgs e)
        {
            //<Polygon Canvas.ZIndex="0" Fill="#FF98DE6C" Name="pJudge" />
            Point p1 = new Point(this.Width / 2, 0);
            Point p2 = new Point(this.Width, this.Height / 2);
            Point p3 = new Point(this.Width / 2, this.Height);
            Point p4 = new Point(0, this.Height / 2);

            Canvas.SetZIndex(pJudge, 0);
            pJudge.Fill = new SolidColorBrush(Color.FromArgb(0xFF, 0x98, 0xDE, 0x6C));
            pJudge.Points.Add(p1);
            pJudge.Points.Add(p2);
            pJudge.Points.Add(p3);
            pJudge.Points.Add(p4);
            this.Children.Add(pJudge);
        }

        /// <summary>
        /// 是否可以连接上
        /// </summary>
        /// <returns></returns>
        public bool IsConnection()
        {
            return true;
        }
        /// <summary>
        /// 是否开始
        /// </summary>
        /// <returns></returns>
        public new bool IsStart()
        {
            //-->如果已经存在二根开始线了，就不能再有了。
            if (2 <= DictArrowFootPoint.Count)
            {
                return false;
            }
            return true;
        }
        /// <summary>
        /// 判断当前Activity是否合法
        /// </summary>
        /// <returns></returns>
        public bool IsCheck()
        {
            //-->最少要有根箭头指向此控件.
            return DictArrowCapPoint.Count > 0 && DictArrowFootPoint.Count == 2 ? true : false;
        }
        /// <summary>
        /// 
        /// </summary>
        public List<ContextMenuInfo> ContextMenu()
        {
            return _ContextMenus;
        }
    }
}
