using System.ServiceModel;

namespace PlatformService
{
    /// <summary>
    /// 平台回调接口
    /// </summary>
    public interface ICallbackPlatformService
    {
        /// <summary>
        /// 回调函数
        /// </summary>
        /// <param name="cmd"></param>
        [OperationContract(IsOneWay = true)]
        void DoActionReply(PlatformCommandInfo cmd);
    }
}
