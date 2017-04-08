using System.Collections.Generic;
using System.Xml.Linq;
using PlatformClient.Common.Lib;
using PlatformClient.Model.Method;
using PlatformClient.Extend.Core;

namespace PlatformClient.Common.Lib
{
    /// <summary>
    /// 
    /// </summary>
    public class XmlTemplate
    {
        PageBaseInfo _BaseInfo = new PageBaseInfo();
        /// <summary>
        /// 
        /// </summary>
        List<MetaDataInfo> _MetaDataItem = new List<MetaDataInfo>();
        /// <summary>
        /// 
        /// </summary>
        List<EventBindInfo> _EventBindItem = new List<EventBindInfo>();
        /// <summary>
        /// 
        /// </summary>
        List<ControlInfo> _ControlItem = new List<ControlInfo>();
        /// <summary>
        /// 
        /// </summary>
        List<EventLinkInfo> _EventLinkItem = new List<EventLinkInfo>();
        /// <summary>
        /// 
        /// </summary>
        List<PageLoadingInfo> _PageLoadingItem = new List<PageLoadingInfo>();

        /// <summary>
        /// 基本信息
        /// </summary>
        public PageBaseInfo BaseInfo { get { return _BaseInfo; } set { _BaseInfo = value; } }

        /// <summary>
        /// 表元素集合
        /// </summary>
        public List<MetaDataInfo> MetaDataItem { get { return _MetaDataItem; } set { _MetaDataItem = value; } }

        /// <summary>
        /// 事件集合
        /// </summary>
        public List<EventBindInfo> EventBindItem { get { return _EventBindItem; } set { _EventBindItem = value; } }
        /// <summary>
        /// 控件集合
        /// </summary>
        public List<ControlInfo> ControlItem { get { return _ControlItem; } set { _ControlItem = value; } }
        /// <summary>
        /// 
        /// </summary>
        public List<EventLinkInfo> EventLinkItem { get { return _EventLinkItem; } set { _EventLinkItem = value; } }

        /// <summary>
        /// 
        /// </summary>
        public List<PageLoadingInfo> PageLoadingItem { get { return _PageLoadingItem; } set { _PageLoadingItem = value; } }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public XElement ParseXml()
        {
            return this.ToXElement();
        }
        /// <summary>
        /// 解析xml生成 XmlTemplate类
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static XmlTemplate DecodeXml(string xml)
        {
            XElement root = XElement.Parse(xml);
            return root.ToModel<XmlTemplate>();
        }
        /// <summary>
        /// 清空所有的集合里的子项
        /// </summary>
        public void Clear()
        {
            this._ControlItem.Clear();
            this._EventBindItem.Clear();
            this._EventLinkItem.Clear();
            this._MetaDataItem.Clear();
            this._PageLoadingItem.Clear();
            this.BaseInfo.SerialNum = 0;
        }
    }
}
