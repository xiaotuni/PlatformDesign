using System;
using System.Collections.Generic;
using System.Windows;
using PlatformClient.Model.Method;
using PlatformClient.Model.Table;

namespace PlatformClient.Common.Interfaces
{
    /// <summary>
    /// 运行时接口
    /// </summary>
    public interface IPageRuntime : IDisposable
    {
        /// <summary>
        /// 页面信息
        /// </summary>
        PageDirectorySub PageInfo { get; set; }
        /// <summary>
        /// 查找控件
        /// </summary>
        /// <param name="controlName">控件名称</param>
        /// <returns>返回控件对象</returns>
        FrameworkElement FindControl(string controlName);

        /// <summary>
        /// 获取全局变量对象
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        GlobalVariableInfo GetGlobalVariablesByName(string name);
        /// <summary>
        /// 复合控件通知运行时
        /// </summary>
        /// <param name="controlName">控件名称【this.Name】</param>
        /// <param name="methodName">方法名称【从ConstantCollection里面找】</param>
        void CompositeControlNotifyRuntimePage(string controlName, string methodName);

        /// <summary>
        /// 弹出信息
        /// </summary>
        /// <param name="message">消息内容</param>
        void AlertMessage(string message);
        /// <summary>
        /// 弹出信息
        /// </summary>
        /// <param name="ee">异常信息</param>
        void AlertMessage(Exception ee);
        /// <summary>
        /// 发送命令
        /// </summary>
        /// <param name="cmd"></param>
        void SendCommand(CtrlPlatformCommandInfo cmd);
        
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

        /// <summary>
        /// 事件链信息
        /// </summary>
        /// <param name="controlName">控件名称</param>
        /// <returns></returns>
        EventLinkInfo GetEventLinkInfoByControlName(string controlName);

        /// <summary>
        /// 事件绑定绑定运行时界面
        /// </summary>
        /// <param name="controlName">控件名称</param>
        void EventBindNotifiyRuntimePage(string controlName);

        /// <summary>
        /// 设置属性的值
        /// </summary>
        /// <param name="propertyValue">属性名称</param>
        /// <param name="value">属性值</param>
        void SetProperty(string propertyValue, object value);
        /// <summary>
        /// 获取属性值
        /// </summary>
        /// <param name="propertyName">属性名称</param>
        /// <returns>返回属性值</returns>
        object GetProperty(string propertyName);
    }
}
