using System.Collections.Generic;
using PlatformClient.Common.Interfaces;
using PlatformClient.Common.Lib;
using PlatformClient.EventDesign.Core.Activitys;
using PlatformClient.Model.Method;

namespace PlatformClient.EventDesign.Activity
{
    public partial class EndActivity : BaseActivity, IActivity
    {
        public EndActivity()
        {
            InitializeComponent();
            this.LabelContent = "结束";
        }
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
    }
}
