using System.Collections.Generic;
using PlatformClient.Common.Interfaces;
using PlatformClient.Common.Lib;
using PlatformClient.EventDesign.Core.Activitys;
using PlatformClient.Model.Method;

namespace PlatformClient.EventDesign.Activity
{
    public partial class CallCtrlFunctionActivity : BaseActivity, IActivity
    {
        public CallCtrlFunctionActivity()
        {
            InitializeComponent();
        }
        public bool IsConnection()
        {
            return true;
        }

        public new bool IsStart()
        {
            return base.IsStart();
        }

        public List<ContextMenuInfo> ContextMenu()
        {
            var a = _ContextMenus;
            a.Add(new ContextMenuInfo
            {
                AssemblyQualifiedName = "PlatformClient.Selectors.Core;PlatformClient.Selectors.Core.CallFunctionSelectors",
                Source = this,
                Header = "参数设置",
                Type = ContextMenuType.ParameterSettings
            });
            return a;
        }
    }
}
