using System;

namespace PlatformClient.PropertyGrid.Events
{
    /// <summary>
    /// 失去焦点事件类
    /// </summary>
    public class EvaluationCtrlLostFocusEventArgs : EventArgs
    {
        /// <summary>
        /// 源
        /// </summary>
        public Object Source { get; set; }
        /// <summary>
        /// 值
        /// </summary>
        public String Value { get; set; }
        /// <summary>
        /// 属性名称
        /// </summary>
        public string PropertyName { get; set; }
        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="source"></param>
        ///// <param name="value"></param>
        //public EvaluationCtrlLostFocusEventArgs(Object source, string value)
        //{
        //    this.Source = source;
        //    this.Value = value;
        //}
        /// <summary>
        /// 
        /// </summary>
        public EvaluationCtrlLostFocusEventArgs() { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="propertyName">属性名称</param>
        /// <param name="value">属性值</param>
        /// <param name="controlName">控件名称</param>
        public EvaluationCtrlLostFocusEventArgs(Object source, string propertyName, string value,string controlName)
        {
            this.Source = source;
            this.PropertyName = propertyName;
            this.Value = value;
            this.ControlName = controlName;
        }

        /// <summary>
        /// 控件名称
        /// </summary>
        public string ControlName { get; set; }
    }
}
