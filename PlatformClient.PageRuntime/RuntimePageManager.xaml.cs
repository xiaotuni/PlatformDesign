using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;
using PlatformClient.Common;
using PlatformClient.Common.Events;
using PlatformClient.Extend.Core;
using PlatformClient.Model.Method;

namespace PlatformClient.PageRuntime
{
    /// <summary>
    /// 
    /// </summary>
    public partial class RuntimePageManager : BaseControl
    {
        /// <summary>
        /// 
        /// </summary>
        public RuntimePageManager()
        {
            InitializeComponent();
            this.Loaded += RuntimePageManager_Loaded;

            EventSubscribe.SendCommandMessage += EventSubscribe_SendCommandMessage;
        }

        void EventSubscribe_SendCommandMessage(object sender, SendCommandMessageEventArgs e)
        {
            RuntimePage rp = sender as RuntimePage;
            if (!rp.IRuntimePageManager.Equals(this))
            {
                return;
            }

            //-->判断
            if (null == e.CommandInfo)
            {
                return;
            }
            e.CommandInfo.TempValue.Add(
                                            new XAttribute(ConstantCollection.TABCONTROL_SELECTED_INDEX, this.tc_Manager.SelectedIndex),
                                            new XAttribute(ConstantCollection.FUNCTION_NAME, "EventSubscribe")
                                        );
            this.SendToService(e.CommandInfo, this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmd"></param>
        public void ProcessEventSubscribe(CtrlPlatformCommandInfo cmd)
        {
            string commandName = string.Format("Process{0}", cmd.CommandName);
            var method = this.GetType().GetMethod(commandName);
            if (null != method)
            {
                method.Invoke(this, new object[] { cmd });
                return;
            }

            //-->获取哪个索引出来
            string index = cmd.TempValue.GetAttributeValue(ConstantCollection.TABCONTROL_SELECTED_INDEX);
            if (index.IsNullOrEmpty())
            {
                return;
            }
            var _index = index.ConvertTo<Int32>();
            //-->找出是哪个TabItem
            if ((_index - 1) > this.tc_Manager.Items.Count)
            {
                //-->当时页面可能已经关闭了。
                return;
            }
            var ti = this.tc_Manager.Items[_index] as TabItem;
            if (null == ti)
            {
                return;
            }
            RuntimePage rp = ti.Content as RuntimePage;
            if (null == rp)
            {
                return;
            }
            rp.DoAction(cmd);
        }

        void RuntimePageManager_Loaded(object sender, RoutedEventArgs e)
        {
            rt_debuger.IRuntimeManager = this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmd"></param>
        public void ProcessOpenPage(CtrlPlatformCommandInfo cmd)
        {
            try
            {
                //--> 当前页面的索引号
                var indexValue = cmd.TempValue.GetAttributeValue("SelectedIndex").ConvertTo<int>();
                if ((indexValue - 1) > this.tc_Manager.Items.Count)
                {
                    return;
                }
                //-->找到当前的索引号
                TabItem selectContent = this.tc_Manager.Items[indexValue] as TabItem;
                if (null == selectContent)
                {
                    return;
                }
                RuntimePage rp = selectContent.Content as RuntimePage;
                if (null == rp)
                {
                    return;
                }
                rp.DoAction(cmd);
            }
            catch { }
        }
    }
}
