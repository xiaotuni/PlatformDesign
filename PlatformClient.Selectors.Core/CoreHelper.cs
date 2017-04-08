using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using PlatformClient.Common;
using PlatformClient.Common.Lib;
using PlatformClient.Model.Method;
using PlatformClient.Utility;
using PlatformClient.Utility.ParseXml;
using PlatformClient.Extend.Core;

namespace PlatformClient.Selectors.Core
{
    internal class CoreHelper
    {

        internal static List<MetaDataColumnInfo> GetControlParameter(XmlTemplate xml, string MetaData, string ColumnName)
        {
            if (MetaData.IsNullOrEmpty() || null == xml)
            {
                return null;
            }
            //-->查找 metadata 里 row里的信息
            var mdis = GetMetaDataInfoByTableName(xml, MetaData);
            if (null == mdis)
            {
                return null;
            }
            if (ColumnName.IsNullOrEmpty())
            {
                return mdis.Item;
            }
            var items = ColumnName.Split('|');
            if (1 < items.Length)
            {
                var datas = new List<MetaDataColumnInfo>();
                foreach (var field in items)
                {
                    foreach (var mdci in mdis.Item)
                    {
                        if (mdci.column_name.Equals(field))
                        {
                            datas.Add(mdci);
                            break;
                        }
                    }
                }
                return datas;
            }
            else
            {
                return mdis.Item.Where(p => p.column_name.Equals(ColumnName)).GetTList<MetaDataColumnInfo>();
            }
        }

        static MetaDataInfo GetMetaDataInfoByTableName(XmlTemplate xml, string MetaData)
        {
            if (MetaData.IsNullOrEmpty() || null == xml || null == xml.MetaDataItem || 0 == xml.MetaDataItem.Count())
            {
                return null;
            }
            return xml.MetaDataItem.Where(p => p.table_name.Equals(MetaData)).GetFirst<MetaDataInfo>();
        }

        /// <summary>
        /// 判断当前事件名称【EventName】它是来到于哪里，是Form还是自己所在控件里,
        /// 如果来到是Form里的话，此时的_ControlName就是用Wrapper.ParseControlName()得到,
        /// 否则的话，就不分析Control控件了
        /// </summary>
        /// <param name="ctrl"></param>
        /// <param name="eventName"></param>
        /// <returns></returns>
        internal static string GetControlName(FrameworkElement ctrl, string eventName)
        {
            var cc = ParseControlConfig.GetControlConfig(ctrl.GetType().Name);
            if (null == cc || null == cc.Functions || 0 == cc.Functions.Count)
            {
                return Wrapper.ParseControlName(ctrl.Name); ;
            }
            var fi = cc.Functions.Where(p => p.Name.Equals(eventName)).GetFirst<FunctionInfo>();
            if (null == fi)
            {
                return Wrapper.ParseControlName(ctrl.Name);
            }
            return ctrl.Name;
        }
    }
}
