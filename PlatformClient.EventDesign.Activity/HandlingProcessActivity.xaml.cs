using System.Collections.Generic;
using PlatformClient.Common.Interfaces;
using PlatformClient.Common.Lib;
using PlatformClient.EventDesign.Core.Activitys;
using PlatformClient.Model.Method;

namespace PlatformClient.EventDesign.Activity
{
    public partial class HandlingProcessActivity : BaseActivity, IActivity
    {
        public HandlingProcessActivity()
        {
            InitializeComponent();
        }

        public bool IsConnection()
        {
            return true;
        }

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
