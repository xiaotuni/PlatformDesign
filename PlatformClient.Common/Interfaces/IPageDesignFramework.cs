using System;
using System.Windows;
using PlatformClient.Common.Lib;
using PlatformClient.Model.Method;

namespace PlatformClient.Common.Interfaces
{
    /// <summary>
    /// 设计时框架接口
    /// </summary>
    public interface IPageDesignFramework:IDisposable
    {
        /// <summary>
        /// 更新当前选择的控件
        /// 通知更新属性控件值
        /// </summary>
        /// <param name="ui"></param>
        void UpdateCurrentSelectedCtrl(UIElement ui);
        /// <summary>
        /// 获取当前的xml模板
        /// </summary>
        /// <returns></returns>
        XmlTemplate GetCurrentXmlTemplate();
        /// <summary>
        /// 获取当前的xaml模板
        /// </summary>
        /// <returns></returns>
        XamlTemplate GetCurrentXamlTemplate();
        /// <summary>
        /// 更新当前的xml以及xaml模板
        /// </summary>
        void UpdateCurrentTemplate();
        /// <summary>
        /// 查找控件
        /// </summary>
        /// <param name="controlName">控件名称</param>
        /// <returns></returns>
        UIElement FindControl(string controlName);

        /// <summary>
        /// 获取文件
        /// </summary>
        /// <returns></returns>
        IPageFileTree GetIPageFileTree();

        /// <summary>
        /// 获取控件信息
        /// </summary>
        /// <param name="controlName">控件名称</param>
        /// <returns>返回控件信息</returns>
        ControlInfo GetControlInfoByControlName(string controlName);
        /// <summary>
        /// 获取MetaData信息
        /// </summary>
        /// <param name="tableName">表的名称</param>
        /// <returns>返回一个MetaData对象</returns>
        MetaDataInfo GetMetaDataInfoByTableName(string tableName);
    }
}
