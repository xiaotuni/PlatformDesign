using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using PlatformClient.Extend.Core;

namespace PlatformClient.Common.Lib
{
    /// <summary>
    /// 
    /// </summary>
    public class XamlCanvasTemplate
    {
        string _Name = "LayoutRoot";
        string _Background = "White";

        List<String> _CanvasPropertyItem = new List<string>();

        /// <summary>
        /// 
        /// </summary>
        public List<String> CanvasPropertyItem { get { return _CanvasPropertyItem; } }

        /// <summary>
        /// 
        /// </summary>
        public Dictionary<String, String> CanvasPropertyDict = new Dictionary<string, string>();

        /// <summary>
        /// 
        /// </summary>
        public string Background { get { return _Background; } set { _Background = value; } }

        /// <summary>
        /// 
        /// </summary>
        public string Name { get { return _Name; } set { _Name = value; } }

        /// <summary>
        /// 子控件集合
        /// {
        ///     key = 控件名称 value = 控件的xaml
        /// }
        /// </summary>
        public Dictionary<String, String> Children = new Dictionary<string, String>();

        /// <summary>
        /// 
        /// </summary>
        public string ParseXml()
        {
            string a = string.Format("<Canvas Name=\"{0}\" Background=\"{1}\">\r\n{2}\r\n</Canvas>",
                this.Name, this.Background, string.Join("\r\n", Children.Values));
            return a;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="canvas"></param>
        /// <returns></returns>
        public static XamlCanvasTemplate DecodeXml(XElement canvas)
        {
            if (null == canvas)
            {
                return new XamlCanvasTemplate();
            }
            XamlCanvasTemplate temp = canvas.ToModel<XamlCanvasTemplate>();
            var rootAtt = canvas.Attributes();
            foreach (var ra in rootAtt)
            {
                temp.CanvasPropertyItem.Add(ra.ToString());

                temp.CanvasPropertyDict.Add(ra.Name.LocalName, ra.Value);
            }
            var childerns = canvas.Elements();
            if (null != childerns && 0 < childerns.Count())
            {
                foreach (var child in childerns)
                {
                    List<string> item = new List<string>();
                    var attItem = child.Attributes();
                    foreach (var a in attItem)
                    {
                        item.Add(a.ToString());
                    }
                    string _result = string.Format("{0} {1} />", child.ToString().Split(' ')[0],
                        string.Join(" ", item));
                    string key = child.GetAttributeValue("Name");
                    temp.Children.Add(key, _result);
                }
            }
            return temp;
        }
    }
}
