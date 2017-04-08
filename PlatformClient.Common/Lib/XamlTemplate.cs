using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;
using PlatformClient.Extend.Core;

namespace PlatformClient.Common.Lib
{
    /// <summary>
    /// xaml文件的模板
    /// </summary>
    public class XamlTemplate
    {
        //<UserControl mc:Ignorable="d" 
        //  xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" 
        //  xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml" 
        //  xmlns:d="http://schemas.microsoft.com/expression/blend/2008" 
        //  xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006" 
        //  xmlns:sdk="http://schemas.microsoft.com/winfx/2006/xaml/presentation/sdk">
        //  <Canvas x:Name="LayoutRoot" Background="White">
        //    <Button Name="a_864257" Content="Button" Height="63" Width="304" Canvas.Left="166" Canvas.Top="137" />
        //    <TextBox Name="a_787109" Text="TextBox" Height="45" Width="164" Canvas.Left="163" Canvas.Top="73" />
        //  </Canvas>
        //</UserControl>
        List<String> _FixedAssemblyCollection = new List<string>();
        List<String> _AssemblyCollection = new List<string>();
        XamlCanvasTemplate _CanvasTemplate = new XamlCanvasTemplate();

        double _Height = 600;
        double _Width = 800;
        /// <summary>
        /// 高 default value is 600px
        /// </summary>
        public double Height { get { return _Height; } set { _Height = value; } }

        /// <summary>
        /// 宽 default value is 800px
        /// </summary>
        public double Width { get { return _Width; } set { _Width = value; } }
        /// <summary>
        /// 
        /// </summary>
        public List<String> FixedAssemblyCollection
        {
            get
            {
                if (null == _FixedAssemblyCollection || 0 == _FixedAssemblyCollection.Count)
                {
                    _FixedAssemblyCollection.Add(string.Format("{0}=\"{1}\"", "mc:Ignorable", "d"));
                    _FixedAssemblyCollection.Add(string.Format("{0}=\"{1}\"", "xmlns", "http://schemas.microsoft.com/winfx/2006/xaml/presentation"));
                    _FixedAssemblyCollection.Add(string.Format("{0}=\"{1}\"", "xmlns:x", "http://schemas.microsoft.com/winfx/2006/xaml"));
                    _FixedAssemblyCollection.Add(string.Format("{0}=\"{1}\"", "xmlns:d", "http://schemas.microsoft.com/expression/blend/2008"));
                    _FixedAssemblyCollection.Add(string.Format("{0}=\"{1}\"", "xmlns:mc", "http://schemas.openxmlformats.org/markup-compatibility/2006"));
                    _FixedAssemblyCollection.Add(string.Format("{0}=\"{1}\"", "xmlns:sdk", "http://schemas.microsoft.com/winfx/2006/xaml/presentation/sdk"));
                }
                return _FixedAssemblyCollection;
            }
        }

        /// <summary>
        /// 程序集引用
        /// </summary>
        public List<String> AssemblyCollection { get { return _AssemblyCollection; } set { _AssemblyCollection = value; } }
        /// <summary>
        /// 画布
        /// </summary>
        public XamlCanvasTemplate CanvasTemplate { get { return _CanvasTemplate; } set { _CanvasTemplate = value; } }

        /// <summary>
        /// 格式花xml
        /// </summary>
        /// <returns></returns>
        public XElement ParseXml()
        {
            AssemblyCollection.AddRange(FixedAssemblyCollection);
            AssemblyCollection = AssemblyCollection.Distinct().OrderBy(p => p.Length).GetTList<String>();

            string Canvas = CanvasTemplate.ParseXml();
            string aa = string.Format("<UserControl {0} >\r\n{1}\r\n</UserControl>",
                string.Join(" ", AssemblyCollection),
                Canvas);
            XElement result = XElement.Parse(aa);
            result.Add(
                new XAttribute("Height", this.Height),
                new XAttribute("Width", this.Width));
            return result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="xaml"></param>
        /// <returns></returns>
        public static XamlTemplate DecodeXml(string xaml)
        {
            XamlTemplate temp = new XamlTemplate();
            if (xaml.IsNullOrEmpty())
            {
                return temp;
            }
            XElement xe = XElement.Parse(xaml);
            var Attributes = xe.Attributes();
            foreach (var v in Attributes)
            {
                try
                {
                    if (v.Name.LocalName.Equals("Height"))
                    {
                        temp.Height = double.Parse(v.Value);
                        continue;
                    }
                    if (v.Name.LocalName.Equals("Width"))
                    {
                        temp.Width = double.Parse(v.Value);
                        continue;
                    }
                }
                catch { }
                string value = v.ToString();
                if (value.IsNullOrEmpty())
                {
                    continue;
                }
                if (temp.IsExists(value))
                {
                    continue;
                }
                temp.AssemblyCollection.Add(value);
            }

            var canvas = xe.FirstNode as XElement;//.Element("Canvas");
            if (null != canvas)
            {
                temp.CanvasTemplate = XamlCanvasTemplate.DecodeXml(canvas);
            }
            return temp;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_value"></param>
        /// <returns></returns>
        bool IsExists(string _value)
        {
            List<String> item = new List<string>();
            item.AddRange(this.AssemblyCollection);
            item.AddRange(this.FixedAssemblyCollection);

            foreach (var v in item)
            {
                if (v.Equals(_value))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
