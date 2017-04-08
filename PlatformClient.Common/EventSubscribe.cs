using System;
using PlatformClient.Common.Events;

namespace PlatformClient.Common
{
    /// <summary>
    /// 事件访问类
    /// </summary>
    public class EventSubscribe
    {
        /// <summary>
        /// 发送命令消息事件
        /// </summary>
        public static event EventHandler<SendCommandMessageEventArgs> SendCommandMessage;

        /// <summary>
        /// 发送命令消息方法
        /// </summary>
        /// <param name="sender">发送方源【this】</param>
        /// <param name="e">发送命令消息事件类</param>
        public static void SendCommandMessageMethod(object sender, SendCommandMessageEventArgs e)
        {
            if (null != SendCommandMessage)
            {
                SendCommandMessage(sender, e);
            }
        }
    }
}
