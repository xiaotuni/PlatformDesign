using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace PlatformClient.Model.Method
{
    /// <summary>
    /// 控件事件绑定类
    /// </summary>
    public class BindEvent
    {
        //<Event Name="LostFocus" BindFunctionName="LostFocusEvent" AssemblyName="PlatformClient.PageRuntime;PlatformClient.PageRuntime.Lib.EventBinding" />
        /// <summary>
        /// 事件名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 绑定方法名称
        /// </summary>
        public string BindFunctionName { get; set; }
        /// <summary>
        /// 程序集引用
        /// </summary>
        public string AssemblyName { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 事件设计器
        /// </summary>
        public string EventDesigner { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Name;
        }
    }
}
