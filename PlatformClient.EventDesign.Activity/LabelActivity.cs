using System.Collections.Generic;
using PlatformClient.Common.Interfaces;
using PlatformClient.EventDesign.Core.Activitys;
using PlatformClient.Model.Method;

namespace PlatformClient.EventDesign.Activity
{
    /// <summary>
    /// 
    /// </summary>
    public partial class LabelActivity : XtnBaseActivity, IActivity
    {
        /// <summary>
        /// 
        /// </summary>
        public LabelActivity()
        {
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
            ////-->如果已经存在一根开始线了，就不能再有了。
            return base.IsStart();
        }
        /// <summary>
        /// 判断当前Activity是否合法
        /// </summary>
        /// <returns></returns>
        public bool IsCheck()
        {
            return DictArrowCapPoint.Count == 1 ? true : false;
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
