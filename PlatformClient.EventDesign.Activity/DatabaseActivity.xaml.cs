using System.Collections.Generic;
using PlatformClient.Common.Interfaces;
using PlatformClient.Common.Lib;
using PlatformClient.EventDesign.Core.Activitys;
using PlatformClient.Model.Method;

namespace PlatformClient.EventDesign.Activity
{
    public partial class DatabaseActivity : BaseActivity, IActivity
    {
        public DatabaseActivity()
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
                Source = this,
                Header = "参数设置",
                Type = ContextMenuType.ParameterSettings
            });
            return a;
        }
    }
}
