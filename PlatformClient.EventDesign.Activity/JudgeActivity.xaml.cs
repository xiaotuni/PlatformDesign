using System.Collections.Generic;
using System.Windows;
using PlatformClient.Common.Interfaces;
using PlatformClient.Common.Lib;
using PlatformClient.EventDesign.Core.Activitys;
using PlatformClient.Model.Method;

namespace PlatformClient.EventDesign.Activity
{
    public partial class JudgeActivity : BaseActivity, IActivity
    {
        public JudgeActivity()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(JudgeActivity_Loaded);
        }

        void JudgeActivity_Loaded(object sender, RoutedEventArgs e)
        {
            Point p1 = new Point(this.Width / 2, 0);
            Point p2 = new Point(this.Width, this.Height / 2);
            Point p3 = new Point(this.Width / 2, this.Height);
            Point p4 = new Point(0, this.Height / 2);
            pJudge.Points.Add(p1);
            pJudge.Points.Add(p2);
            pJudge.Points.Add(p3);
            pJudge.Points.Add(p4);
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
        public List<ContextMenuInfo> ContextMenu()
        {
            return _ContextMenus;
        }
    }
}
