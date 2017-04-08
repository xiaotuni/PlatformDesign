using System;
using System.Xml.Linq;
using PlatformClient.Common.Lib;
using PlatformClient.Model.Method;

namespace PlatformClient.Common.Interfaces
{
    /// <summary>
    /// 复合控件接口
    /// </summary>
    public interface ICompositeCtrl : IDisposable
    {
        /// <summary>
        /// 标题内容的宽度
        /// </summary>
        double TitleWidth { get; set; }
        /// <summary>
        /// 设置运行时接口
        /// </summary>
        /// <param name="iRuntime">运行时接口</param>
        void SetPageRuntimeInterface(IPageRuntime iRuntime);
        /// <summary>
        /// 获取属性
        /// </summary>
        /// <param name="propertyName">属性名称</param>
        /// <returns></returns>
        object GetProperty(string propertyName);
        /// <summary>
        /// 设置属性
        /// </summary>
        /// <param name="propertyName">属性名称</param>
        /// <param name="propertyValue">属性值</param>
        void SetProperty(string propertyName, object propertyValue);
        /// <summary>
        /// 加载数据
        /// </summary>
        /// <param name="cmd">复合控件命令对象类</param>
        void InitLoad(CtrlPlatformCommandInfo cmd);
        /// <summary>
        /// 初始化标题
        /// </summary>
        /// <param name="cmd">复合控件命令对象类</param>
        void InitTitle(CtrlPlatformCommandInfo cmd);
        /// <summary>
        /// 输出参数
        /// </summary>
        /// <returns></returns>
        CtrlPlatformCommandInfo OutputParameter();
        /// <summary>
        /// 输入参数
        /// </summary>
        /// <param name="cmd">复合控件命令对象类</param>
        void InputParameter(CtrlPlatformCommandInfo cmd);

        //void SetTitleWidth(double maxWidth);

        /// <summary>
        /// 执行操作
        /// </summary>
        /// <param name="cmd">复合控件命令对象类</param>
        void DoAction(CtrlPlatformCommandInfo cmd);

        /// <summary>
        /// 清空值操作
        /// </summary>
        void ClearCtrlValue();

        /// <summary>
        /// 刷新操作
        /// </summary>
        void Refresh(CtrlPlatformCommandInfo cmd);
    }
}
