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
    public partial class EndActivity : XtnBaseActivity, IActivity
    {
        /// <summary>
        /// 
        /// </summary>
        public EndActivity()
        {
            InitializeComponent();
            this.LabelContent = "结束";
        }
        /// <summary>
        /// 
        /// </summary>
        public List<ContextMenuInfo> ContextMenu()
        {
            return _ContextMenus;
        }
        /// <summary>
        /// 
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
            return false;
        }
        /// <summary>
        /// 判断当前Activity是否合法
        /// </summary>
        /// <returns></returns>
        public bool IsCheck()
        {
            return DictArrowCapPoint.Count > 0 ? true : false;
        }
    }
}
