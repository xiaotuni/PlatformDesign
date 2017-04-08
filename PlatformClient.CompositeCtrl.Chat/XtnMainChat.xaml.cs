using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Windows;
using System.Windows.Controls;
using PlatformClient.Extend.Core;
using PlatformClient.Model.Method;
using PlatformClient.PlatformChat;
using PlatformClient.Utility;

namespace PlatformClient.CompositeCtrl.Chat
{
    /// <summary>
    /// 
    /// </summary>
    public partial class XtnMainChat : UserControl, IPlatformChatCallback, IDisposable
    {
        private PlatformChatClient client;
        string LogonName = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public XtnMainChat()
        {
            InitializeComponent();
            this.btnSay.Click += btnSay_Click;
            this.btnWhisper.Click += btnWhisper_Click;
            this.btnConnection.Click += btnConnection_Click;
            this.btnDisConnection.Click += btnDisConnection_Click;
            this.btnClear.Click += btnClear_Click;
            this.Loaded += XtnMainChat_Loaded;
        }

        void XtnMainChat_Loaded(object sender, RoutedEventArgs e)
        {
            ConnectionChatService();
        }

        void btnClear_Click(object sender, RoutedEventArgs e)
        {
            this.lbContent.Items.Clear();
        }

        void btnDisConnection_Click(object sender, RoutedEventArgs e)
        {
            client.CloseAsync();
            client = null;

            this.btnDisConnection.IsEnabled = false;
            this.btnConnection.IsEnabled = true;
        }

        void btnConnection_Click(object sender, RoutedEventArgs e)
        {
            if (null != client && client.State == CommunicationState.Opened)
            {
                LogonName = CtrlPlatformCommandInfo.GetGlobalVariable(ConstantCollection.GlobalVariable_LogonName).Value;
                client.LogonAsync(LogonName);
            }
            this.btnConnection.IsEnabled = false;
            this.btnDisConnection.IsEnabled = true;
        }

        void btnWhisper_Click(object sender, RoutedEventArgs e)
        {
            if (txtMsg.Text.IsNullOrEmpty())
            {
                Wrapper.ShowDialog("消息不能为空");
                return;
            }
            if (null == this.lbUsers.SelectedItem)
            {
                Wrapper.ShowDialog("请选择要私聊的对象");
                return;
            }
            if (null != client && client.State == CommunicationState.Opened)
            {
                client.WhisperAsync(this.txtMsg.Text.Trim(), this.lbUsers.SelectedItem.ToString());
            }
        }

        void btnSay_Click(object sender, RoutedEventArgs e)
        {
            if (txtMsg.Text.IsNullOrEmpty())
            {
                Wrapper.ShowDialog("消息不能为空");
                return;
            }
            if (null != client && client.State == CommunicationState.Opened)
            {
                client.SayAsync(this.txtMsg.Text.Trim());
            }
        }

        /// <summary>
        /// 连接聊天服务器
        /// </summary>
        public void ConnectionChatService()
        {
            if (null == client)
            {
                var end = new EndpointAddress("http://localhost:6667/PlatformChat.svc");
                var binding = new PollingDuplexHttpBinding(PollingDuplexMode.MultipleMessagesPerPoll);
                client = new PlatformChatClient(new InstanceContext(this), binding, end);
            }
        }

        public void ReplyLogon(string user)
        {
            this.lbUsers.Items.Add(user);
        }

        public void CurrentUsers(List<string> users)
        {
            foreach (var user in users)
            {
                this.lbUsers.Items.Add(user);
            }
        }

        public void ReplyLeave(string user)
        {
            this.lbUsers.Items.Remove(user);
        }

        public void ReplySay(string user, string msg)
        {
            this.lbContent.Items.Add(new TextBlock()
            {
                TextWrapping = TextWrapping.Wrap,
                Text = string.Format("{0}\r\n{1}", user, msg)
            });
        }

        public void ReplyWhisper(string smg, string toUser)
        {
            this.lbContent.Items.Add(new TextBlock()
            {
                TextWrapping = TextWrapping.Wrap,
                Text = string.Format("{0} 对你说\r\n{1}", toUser, smg)
            });
        }

        public void Dispose()
        {
            this.btnSay.Click -= btnSay_Click;
            this.btnWhisper.Click -= btnWhisper_Click;
            this.btnConnection.Click -= btnConnection_Click;
            this.btnDisConnection.Click -= btnDisConnection_Click;
            this.btnClear.Click -= btnClear_Click;
            this.Loaded -= XtnMainChat_Loaded;
        }
    }

    internal class ChatHelper<T>
    {
        private static PlatformChatClient client;
        static EndpointAddress end = new EndpointAddress("http://localhost:6667/PlatformChat.svc");
        static PollingDuplexHttpBinding binding = new PollingDuplexHttpBinding(PollingDuplexMode.MultipleMessagesPerPoll);

        static T InstanceContext;

        ChatHelper(T source)
        {
            InstanceContext = source;
            Connection();
        }

        static void Connection()
        {
            client = new PlatformChatClient(new InstanceContext(InstanceContext), binding, end);
        }

        public static void Logon(string user)
        {
            client.LogonAsync(user);
        }


        /// <summary>
        /// 打印日志到控制台
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="str">内容</param>
        public static void PrintfLog(CtrlPlatformCommandInfo cmd, string str)
        {
            try
            {
                string log = string.Format("----------------------------------Begin--{0}-{1}-----------------\r\n", cmd.CommandName, str);
                log += string.Format("Command Name is:{0};Command Type is {1}", cmd.CommandName, cmd.CommandType);
                log += string.Format("\r\n {0} ", cmd.ToXElement());
                log += string.Format("\r\n---------------------------------------------------End---{0}-{1}--------------------------------\r\n", cmd.CommandName, str);
                Debug.WriteLine("{0}-->{1}", DateTime.Now.ToString("yyyy-MM-dd- HH:mm:ss"), log);
            }
            catch { }
        }

    }
}
