using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using PlatformClient.Extend.Core;
using PlatformClient.Model.Method;

namespace PlatformClient.Utility.ParseXml
{
    /// <summary>
    /// 
    /// </summary>
    public class ParseControlTemplate
    {
        /// <summary>
        /// 
        /// </summary>
        public static List<ControlTemplateConfig> TempletItem = new List<ControlTemplateConfig>();
        /// <summary>
        /// 
        /// </summary>
        static ParseControlTemplate()
        {
            try
            {
                XElement root = XElement.Load(ConstantCollection.Path_ControlTemplet);
                if (null == root)
                {
                    return;
                }
                var templetItem = root.Elements("Templet");
                foreach (var v in templetItem)
                {
                    ControlTemplateConfig ct = new ControlTemplateConfig();
                    ct.Header = v.GetElementValue("Header");
                    ct.Tail = v.GetElementValue("Tail");
                    ct.XmlNode = v;
                    ct.Name = v.GetAttributeValue("Name");
                    ct.AssemblyCollection = new List<string>();

                    XElement AssemblyQualifiedNames = v.Element("AssemblyQualifiedName");
                    if (null == AssemblyQualifiedNames)
                    {
                        continue;
                    }
                    var assemItem = AssemblyQualifiedNames.Elements("item");
                    if (null == assemItem || 0 == assemItem.Count())
                    {
                        continue;
                    }

                    foreach (var aq in assemItem)
                    {
                        string asse = aq.Value;
                        if (asse.IsNullOrEmpty())
                        {
                            continue;
                        }
                        ct.AssemblyCollection.Add(asse.Trim());
                    }
                    TempletItem.Add(ct);
                }
            }
            catch { }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="templetName"></param>
        /// <returns></returns>
        public static ControlTemplateConfig GetControlTempletConfig(string templetName)
        {
            return TempletItem.Where(p => p.Name.ToUpper().Equals(templetName.ToUpper())).GetFirst<ControlTemplateConfig>();
        }
    }
}
