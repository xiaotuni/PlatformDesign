using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using PlatformClient.Extend.Core;
using PlatformClient.Model.Method;

namespace PlatformClient.Utility.ParseXml
{
    /// <summary>
    /// 解析控件配置文件类
    /// </summary>
    public class ParseControlConfig
    {
        static List<ControlConfig> _ControlItem = new List<ControlConfig>();
        /// <summary>
        /// 控件集合
        /// </summary>
        public static List<ControlConfig> ControlItem
        {
            get
            {
                if (null == _ControlItem || 0 == _ControlItem.Count)
                {
                    InitLoadConfig();
                }
                return _ControlItem;
            }
        }

        static ParseControlConfig()
        {
            InitLoadConfig();
        }

        static void InitLoadConfig()
        {
            try
            {

                XElement root = XElement.Load(ConstantCollection.Path_ControlConfig);
                if (null == root)
                {
                    return;
                }
                _ControlItem = root.ToModelList<ControlConfig>();
            }
            catch
            {
            }
        }

        /// <summary>
        /// 获取控件的配置信息
        /// </summary>
        /// <param name="controlType">控件类型</param>
        /// <returns></returns>
        public static ControlConfig GetControlConfig(string controlType)
        {
            return ControlItem.Where(p => p.Name.ToLower().Equals(controlType.ToLower())).GetFirst<ControlConfig>();
        }
    }
}
