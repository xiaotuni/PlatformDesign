using System.Collections.Generic;
using PlatformClient.Common.Interfaces;
using PlatformClient.Common.Lib;
using PlatformClient.EventDesign.Core.Activitys;
using PlatformClient.Model.Method;

namespace PlatformClient.EventDesign.Activity
{
    /// <summary>
    /// 
    /// </summary>
    public partial class BeginActivity : XtnBaseActivity, IActivity
    {
        /// <summary>
        /// 是否可以连接
        /// </summary>
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
            //-->如果已经存在一根开始线了，就不能再有了。
            return base.IsStart();
        }
        /// <summary>
        /// 右键菜单
        /// </summary>
        public List<ContextMenuInfo> ContextMenu()
        {
            return _ContextMenus;
        }

        /// <summary>
        /// 判断当前Activity是否合法
        /// </summary>
        /// <returns></returns>
        public bool IsCheck()
        {
            return this.DictArrowFootPoint.Count > 0 ? true : false;
        }
    }
}
