using System.Collections.Generic;
using PlatformClient.Common.Interfaces;
using PlatformClient.Common.Lib;
using PlatformClient.EventDesign.Core.Activitys;
using PlatformClient.Model.Method;

namespace PlatformClient.EventDesign.Activity
{
    public partial class BeginActivity : BaseActivity, IActivity
    {
        public BeginActivity()
        {
            InitializeComponent();
        }

        public bool IsConnection()
        {
            return false;
        }

        /// <summary>
        /// 是否开始
        /// </summary>
        /// <returns></returns>
        public new bool IsStart()
        {
            ////-->如果已经存在一根开始线了，就不能再有了。
            //if (0 < DictArrowFootPoint.Count)
            //{
            //    return false;
            //}
            //return true;
            return base.IsStart();
        }
        public List<ContextMenuInfo> ContextMenu()
        {
            return _ContextMenus;
        }
    }
}
