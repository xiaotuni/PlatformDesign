using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.ServiceModel.Activation;

namespace PlatformService
{
    /// <summary>
    ///  注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码和配置文件中的接口名“IPlatformService”
    /// </summary>
    [ServiceContract]
    public interface IPlatformService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="aa"></param>
        /// <returns></returns>
        [OperationContract]
        string TextValue(string aa);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pci"></param>
        /// <returns></returns>
        [OperationContract]
        PlatformCommandInfo DoAction(PlatformCommandInfo pci);
    }
}
