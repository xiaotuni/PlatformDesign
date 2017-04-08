
namespace PlatformClient.Utility.ParseXml
{
    ///// <summary>
    ///// 
    ///// </summary>
    //public class ParsePageTemplate
    //{
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    public static List<PageTemplateConfig> PageTempletItem = new List<PageTemplateConfig>();

    //    static ParsePageTemplate()
    //    {

    //        XElement root = XElement.Load(ConstantCollection.Path_PageTemplet);
    //        if (null == root)
    //        {
    //            return;
    //        }
    //        var templetItem = root.Elements("Templet");
    //        foreach (var v in templetItem)
    //        {
    //            PageTemplateConfig ct = new PageTemplateConfig();
    //            ct.TemplateName = v.GetAttributeValue("Name");
    //            //-->
    //            ct.PageBaseInfo = v.GetElementValue("PageBaseInfo");
    //            ct.MetaDataItem = v.GetElementValue("MetaDataItem");
    //            ct.EventBindItem = v.GetElementValue("EventBindItem");
    //            ct.ControlItem = v.GetElementValue("ControlItem");
    //            ct.EventLinkItem = v.GetElementValue("EventLinkItem");
    //            ct.PageLoadingItem = v.GetElementValue("PageLoadingItem");
    //            ct.XmlNode = v;
    //            PageTempletItem.Add(ct);
    //        }
    //    }

    //    /// <summary>
    //    /// 获取模板
    //    /// </summary>
    //    /// <param name="templetName"></param>
    //    /// <returns></returns>
    //    public static PageTemplateConfig GetPageTempletConfig(string templetName)
    //    {
    //        return
    //            PageTempletItem.Where(p => p.TemplateName.Equals(templetName)).GetFirst<PageTemplateConfig>();
    //    }
    //}
}
