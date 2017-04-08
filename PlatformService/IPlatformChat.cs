using System.Collections.Generic;
using System.ServiceModel;

namespace PlatformService
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码和配置文件中的接口名“IPlatformChat”。
    /// <summary>
    /// 
    /// </summary>
    [ServiceContract(CallbackContract = typeof(IClientChatCallback))]
    public interface IPlatformChat
    {
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="user"></param>
        [OperationContract(IsOneWay = true)]
        void Logon(string user);
        /// <summary>
        /// 离开
        /// </summary>
        /// <param name="user"></param>
        [OperationContract(IsOneWay = true)]
        void Leave(string user);
        /// <summary>
        /// 说话
        /// </summary>
        /// <param name="smg"></param>
        [OperationContract(IsOneWay = true)]
        void Say(string smg);
        /// <summary>
        /// 私聊
        /// </summary>
        /// <param name="smg"></param>
        /// <param name="toUser"></param>
        [OperationContract(IsOneWay = true)]
        void Whisper(string smg, string toUser);
    }

    /// <summary>
    /// 客户端聊天回调接口
    /// </summary>
    public interface IClientChatCallback
    {
        /// <summary>
        /// 回复信息
        /// </summary>
        /// <param name="user"></param>
        [OperationContract(IsOneWay = true)]
        void ReplyLogon(string user);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="users"></param>
        [OperationContract(IsOneWay = true)]
        void CurrentUsers(List<string> users);

        /// <summary>
        /// 离开
        /// </summary>
        /// <param name="user"></param>
        [OperationContract(IsOneWay = true)]
        void ReplyLeave(string user);

        /// <summary>
        /// 说话
        /// </summary>
        /// <param name="user"></param>
        /// <param name="smg"></param>
        [OperationContract(IsOneWay = true)]
        void ReplySay(string user, string smg);

        /// <summary>
        /// 私聊
        /// </summary>
        /// <param name="smg"></param>
        /// <param name="toUser"></param>
        [OperationContract(IsOneWay = true)]
        void ReplyWhisper(string smg, string toUser);
    }
}
