using System;
using PlatformClient.Model.Method;
using PlatformClient.PlatformServer;

namespace PlatformClient
{
    /// <summary>
    /// 平台客户端接口
    /// </summary>
    public interface IPlatformClient:IDisposable
    {
        /// <summary>
        /// 发送命令
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="client">this</param>
        void SendToService(CtrlPlatformCommandInfo cmd,IPlatformClient client);
        /// <summary>
        /// 处理收到的命令
        /// </summary>
        /// <param name="cmd"></param>
        void DoAction(CtrlPlatformCommandInfo cmd);
    }
}
