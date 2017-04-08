using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace PlatformClient.Utility.ParseXml
{
    /// <summary>
    /// 
    /// </summary>
    public class ControlTemplateConfig
    {
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Header { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Tail { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<String> AssemblyCollection { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public XElement XmlNode { get; set; }
    }
}
