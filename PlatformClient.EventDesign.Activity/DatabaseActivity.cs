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
    public partial class DatabaseActivity : XtnBaseActivity, IActivity
    {
        /// <summary>
        /// 
        /// </summary>
        public DatabaseActivity()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsConnection()
        {
            return true;
        }
        /// <summary>
        /// 判断当前Activity是否合法
        /// </summary>
        /// <returns></returns>
        public bool IsCheck()
        {
            return DictArrowFootPoint.Count == 1 && DictArrowCapPoint.Count == 1 ? true : false;
        }

        /// <summary>
        /// 
        /// </summary>
        public new bool IsStart()
        {
            return base.IsStart();
        }

        /// <summary>
        /// 
        /// </summary>
        public List<ContextMenuInfo> ContextMenu()
        {
            var a = _ContextMenus;
            a.Add(new ContextMenuInfo
            {
                Source = this,
                Header = "参数设置",
                Type = ContextMenuType.ParameterSettings
            });
            return a;
        }
    }
}
