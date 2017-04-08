using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace PlatformService
{
    /// <summary>
    /// 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码和配置文件中的接口名“IService1”
    /// </summary>
    [ServiceContract]
    public interface IService1
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [OperationContract]
        string GetData(int value);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="composite"></param>
        /// <returns></returns>
        [OperationContract]
        CompositeType GetDataUsingDataContract(CompositeType composite);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="aa"></param>
        /// <returns></returns>
        [OperationContract]
        string TextData(string aa);


        // TODO: 在此添加您的服务操作
    }


    /// <summary>
    /// 使用下面示例中说明的数据约定将复合类型添加到服务操作
    /// </summary>
    [DataContract]
    public class CompositeType
    {
        bool boolValue = true;
        string stringValue = "Hello ";
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public bool BoolValue
        {
            get { return boolValue; }
            set { boolValue = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string StringValue
        {
            get { return stringValue; }
            set { stringValue = value; }
        }
    }
}
