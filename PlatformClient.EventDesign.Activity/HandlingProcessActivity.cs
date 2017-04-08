using System.Collections.Generic;
using PlatformClient.Common.Interfaces;
using PlatformClient.EventDesign.Core.Activitys;
using PlatformClient.Model.Method;

namespace PlatformClient.EventDesign.Activity
{
    /// <summary>
    /// 
    /// </summary>
    public partial class HandlingProcessActivity : XtnBaseActivity, IActivity
    {
        /// <summary>
        /// 
        /// </summary>
        public HandlingProcessActivity()
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
        /// 
        /// </summary>
        public new bool IsStart()
        {
            //-->如果已经存在一根开始线了，就不能再有了。
            return base.IsStart();
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
