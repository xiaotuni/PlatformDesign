using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.ServiceModel.Activation;

namespace PlatformService
{
    /// <summary>
    /// 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码、svc 和配置文件中的类名“Service1”
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class Service1 : IService1
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public string GetData(int value)
        {
            return string.Format("You entered: {0}", value);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="composite"></param>
        /// <returns></returns>
        public CompositeType GetDataUsingDataContract(CompositeType composite)
        {
            if (composite == null)
            {
                throw new ArgumentNullException("composite");
            }
            if (composite.BoolValue)
            {
                composite.StringValue += "Suffix";
            }
            return composite;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="aa"></param>
        /// <returns></returns>
        public string TextData(string aa)
        {
            List<string> item = new List<string>();
            for (int i = 0; i < 10; i++)
            {
                item.Add(aa);
            }
            return string.Join("\r\n", item);
        }
    }
}
