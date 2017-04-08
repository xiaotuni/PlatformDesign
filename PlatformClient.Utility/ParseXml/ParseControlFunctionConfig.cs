using System.Linq;
using System.Xml.Linq;
using PlatformClient.Extend.Core;
using PlatformClient.Model.Method;

namespace PlatformClient.Utility.ParseXml
{
    /// <summary>
    /// 
    /// </summary>
    public class ParseControlFunctionConfig
    {
        private static CtrlFunctionConfig _ControlItem;

        /// <summary>
        /// 
        /// </summary>
        public static CtrlFunctionConfig ControlItem
        {
            get
            {
                if (null == _ControlItem)
                {
                    InitLoadConfig();
                }
                return _ControlItem;
            }
        }

        static ParseControlFunctionConfig()
        {
            InitLoadConfig();
        }

        static void InitLoadConfig()
        {
            try
            {

                XElement root = XElement.Load(ConstantCollection.Path_ControlFunctionConfig);
                if (null == root)
                {
                    return;
                }
                _ControlItem = root.ToModel<CtrlFunctionConfig>();//.DecodeXmlSingle(root);
            }
            catch { }
        }


        /// <summary>
        /// 获取控件的配置信息
        /// </summary>
        /// <param name="type">控件类型</param>
        /// <returns></returns>
        public static CtrlFuctionTypeInfo GetControlFunctionConfig(string type)
        {
            try
            {
                return ControlItem.Item.Where(p => p.Type.ToLower().Equals(type.ToLower())).GetFirst<CtrlFuctionTypeInfo>();
            }
            catch
            {
                return null;
            }
        }
    }
}
