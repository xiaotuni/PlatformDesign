using System;
using PlatformClient.Model.Method;

namespace PlatformClient.Common.Events
{
    /// <summary>
    /// 发送命令消息事件类
    /// </summary>
    public class SendCommandMessageEventArgs : EventArgs
    {
        /// <summary>
        /// 
        /// </summary>
        public Object Source { get; set; }
        /// <summary>
        /// 平台命令信息对象
        /// </summary>
        public CtrlPlatformCommandInfo CommandInfo { get; set; }
        /// <summary>
        /// 复合控件命令对象类
        /// </summary>
        public CtrlPlatformCommandInfo CtrlCommandInfo { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public SendCommandMessageEventArgs()
        {
            Source = null;
            this.CommandInfo = null;
            this.CtrlCommandInfo = null;
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="source">源</param>
        /// <param name="cmd">平台命令信息对象</param>
        public SendCommandMessageEventArgs(object source, CtrlPlatformCommandInfo cmd)
            : this()
        {
            this.Source = source;
            this.CommandInfo = cmd;
        }
        ///// <summary>
        ///// 构造函数
        ///// </summary>
        ///// <param name="source">源</param>
        ///// <param name="ctrlCmd">复合控件命令对象类</param>
        //public SendCommandMessageEventArgs(object source, CtrlPlatformCommandInfo ctrlCmd)
        //    : this()
        //{
        //    this.Source = source;
        //    this.CtrlCommandInfo = ctrlCmd;
        //}
    }
}
