using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Media;
using PlatformClient.Extend.Core;
using PlatformClient.Model.Method;
using PlatformClient.PlatformServer;

namespace PlatformClient
{
    /// <summary>
    /// 基本控件
    /// </summary>
    public class BaseControl : UserControl, IPlatformClient
    {
        /// <summary>
        /// 命令对象
        /// </summary>
        protected CtrlPlatformCommandInfo cmd = null;

        /// <summary>
        /// 构造函数
        /// </summary>
        public BaseControl()
        {
            Color c = new Color();
            c.A = 0xFF;
            c.B = 0xF0;
            c.G = 0xF0;
            c.R = 0xF0;
            this.Background = new SolidColorBrush(c);   //"#FFF0F0F0";
        }

        /// <summary>
        /// 发送命令【方法是：public void Process+方法名称,参数里自己定义名称】
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="client"></param>
        public void SendToService(CtrlPlatformCommandInfo cmd, IPlatformClient client)
        {
            ClientCommand cc = new ClientCommand();
            cc.SendToService(cmd, client);
        }

        /// <summary>
        /// 发送命令
        /// </summary>
        /// <param name="cmd"></param>
        public void SendToService(CtrlPlatformCommandInfo cmd)
        {
            this.SendToService(cmd, this);
        }

        /// <summary>
        /// 处理命令,查找
        /// </summary>
        /// <param name="cmd"></param>
        public virtual void DoAction(CtrlPlatformCommandInfo cmd)
        {
            if (null == cmd || null == cmd.TempValue)
            {
                return;
            }
            var att = cmd.TempValue.Attribute("FunctionName");
            if (null == att)
            {
                return;
            }
            string functionName = att.Value;
            string methodName = string.Format("Process{0}", functionName);
            Type tt = this.GetType();
            MethodInfo method = tt.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.CreateInstance);
            if (null == method)
            {
                return;
            }
            method.Invoke(this, new object[] { cmd });
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public virtual void Dispose()
        {


        }
    }
}
