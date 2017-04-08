using System.Collections.Generic;
using System.Xml.Linq;
using PlatformClient.Extend.Core;
using PlatformClient.Model.Method;

namespace PlatformClient.Utility.ParseXml
{
    /// <summary>
    /// 控件配置信息类
    /// </summary>
    public class ControlConfig
    {
        private bool _IsDisplay = true;
        private Dictionary<string, string> _DictProperty;
        /// <summary>
        /// 控件名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// xml节点内容
        /// </summary>
        public XElement XmlNode { get; set; }
        /// <summary>
        /// 控件描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 控件xaml的表显
        /// </summary>
        public string xaml { get; set; }
        /// <summary>
        /// 控件属性集合【Height;高|Width;宽|Content;内容|xx;yy|..;..】
        /// </summary>
        public PropertyCollectionInfo PropertyCollection { get; set; }

        /// <summary>
        /// Key =  属性名称;value = 属性描述
        /// 如{ Height,高}
        /// </summary>
        public Dictionary<string, string> DictProperty
        {
            get
            {
                if (null == _DictProperty)
                {
                    _DictProperty = new Dictionary<string, string>();
                    if (null == PropertyCollection || PropertyCollection.value.IsNullOrEmpty())
                    {
                        return _DictProperty;
                    }
                    string[] pcItems = PropertyCollection.value.Split('|');
                    foreach (var item in pcItems)
                    {
                        string[] k_v = item.Split(';');
                        string key = k_v[0];
                        string value = k_v[1];
                        _DictProperty.Add(key, value);
                    }
                }
                return _DictProperty;
            }
        }

        /// <summary>
        /// 是否是复合控件;1--是;
        /// </summary>
        public bool IsCompositeCtrl { get; set; }

        /// <summary>
        /// 是否显示
        /// </summary>
        public bool IsDisplay { get { return _IsDisplay; } set { _IsDisplay = value; } }
        /// <summary>
        /// 控件的数据类型
        /// </summary>
        public string DataType { get; set; }

        /// <summary>
        /// 复合控件程序集名称
        /// </summary>
        public string AssemblyQualifiedName { get; set; }

        /// <summary>
        /// 事件集合
        /// </summary>
        public List<BindEvent> Events { get; set; }

        /// <summary>
        /// 函数
        /// </summary>
        public List<FunctionInfo> Functions { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Description;
        }
    }

}
