using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using PlatformService.Lib;
using PlatformService.Utility;

namespace PlatformService
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码、svc 和配置文件中的类名“PlatformChat”。
    // 注意: 为了启动 WCF 测试客户端以测试此服务，请在解决方案资源管理器中选择 PlatformChat.svc 或 PlatformChat.svc.cs，然后开始调试。
    /// <summary>
    /// 
    /// </summary>
    public class PlatformChat : IPlatformChat
    {
        static Dictionary<String, IClientChatCallback> _DictChat = new Dictionary<string, IClientChatCallback>();
        static object LockObject = new object();
        private IClientChatCallback _ReplyClient;
        private string _UserName;

        /// <summary>
        /// 获取用户聊天信息
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        ChatInfo GetChatInfo(PlatformCommandInfo cmd)
        {
            return cmd.ParamCollection.ToModel<ChatInfo>();
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="user"></param>
        public void Logon(string user)
        {
            if (user.IsNullOrEmpty())
            {
                return;
            }
            if (null == _ReplyClient)
            {
                _ReplyClient = OperationContext.Current.GetCallbackChannel<IClientChatCallback>();
                OperationContext.Current.Channel.Faulted -= Channel_Faulted;
                OperationContext.Current.Channel.Closed -= Channel_Closed;
                OperationContext.Current.Channel.Faulted += Channel_Faulted;
                OperationContext.Current.Channel.Closed += Channel_Closed;
                var e = new ChatEventArgs(ChatType.Logon, null, user);
                Reply(e);
            }
        }

        void Reply(ChatEventArgs e)
        {
            lock (LockObject)
            {
                switch (e.ChatType)
                {
                    case ChatType.Logon:
                        if (!_DictChat.ContainsKey(e.UserName))
                        {
                            this._UserName = e.UserName;
                            foreach (var child in _DictChat)
                            {
                                child.Value.ReplyLogon(e.UserName);
                            }
                            _DictChat.Add(e.UserName, _ReplyClient);
                            _ReplyClient.CurrentUsers(_DictChat.Keys.ToList<String>());
                        }
                        break;
                    case ChatType.Leave:
                        foreach (var child in _DictChat)
                        {
                            child.Value.ReplyLeave(e.UserName);
                        }
                        if (_DictChat.ContainsKey(e.UserName))
                        {
                            _DictChat.Remove(e.UserName);
                        }
                        try
                        {
                            _ReplyClient = null;
                            OperationContext.Current.Channel.Abort();
                            OperationContext.Current.Channel.Close();
                        }
                        catch { }
                        break;
                    case ChatType.Say:
                        foreach (var child in _DictChat)
                        {
                            child.Value.ReplySay(e.UserName, e.Msg);
                        }
                        break;
                    case ChatType.Whisper:
                        if (_DictChat.ContainsKey(e.UserName))
                        {
                            _DictChat[e.UserName].ReplyWhisper(e.Msg, this._UserName);
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// 离开
        /// </summary>
        /// <param name="user"></param>
        public void Leave(string user)
        {
            var e = new ChatEventArgs(ChatType.Leave, string.Format("{0}离开聊天", user), user);
            Reply(e);
        }

        /// <summary>
        /// 聊天
        /// </summary>
        /// <param name="msg"></param>
        public void Say(string msg)
        {
            var e = new ChatEventArgs(ChatType.Say, msg, this._UserName);
            Reply(e);
        }

        /// <summary>
        /// 私聊
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="toUser"></param>
        public void Whisper(string msg, string toUser)
        {
            var e = new ChatEventArgs(ChatType.Whisper, msg, toUser);
            Reply(e);
        }

        void Channel_Closed(object sender, EventArgs e)
        {
            Leave(this._UserName);
            this._UserName = string.Empty;
        }

        void Channel_Faulted(object sender, EventArgs e)
        {
            Leave(this._UserName);
            this._UserName = string.Empty;
        }

    }

    class ChatEventArgs : EventArgs
    {
        public ChatType ChatType { get; set; }
        public string Msg { get; set; }
        public string UserName { get; set; }

        public ChatEventArgs(ChatType chatType, string msg, string userName)
        {
            this.ChatType = chatType;
            this.Msg = msg;
            this.UserName = userName;
        }
    }
    enum ChatType
    {
        Logon,
        Leave,
        Say,
        Whisper
    }
}
