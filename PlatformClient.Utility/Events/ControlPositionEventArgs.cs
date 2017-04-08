using System;
using System.Collections.Generic;

namespace PlatformClient.Utility.Events
{
    /// <summary>
    /// 修改控件属性事件类
    /// </summary>
    public class ControlModifyPropertyEventArgs : EventArgs
    {
        ///// <summary>
        ///// 上面距离
        ///// </summary>
        //public double TopProperty { get; set; }
        ///// <summary>
        ///// 左右距离
        ///// </summary>
        //public double LeftProperty { get; set; }
        ///// <summary>
        ///// 高
        ///// </summary>
        //public double Height { get; set; }
        ///// <summary>
        ///// 宽
        ///// </summary>
        //public double Width { get; set; }

        Dictionary<String, Object> _DictProperty = new Dictionary<string, object>();
        /// <summary>
        /// key = 属性名称；Object = 属性的值
        /// </summary>
        public Dictionary<String, Object> DictProperty
        {
            get { return _DictProperty; }
            set { _DictProperty = value; }
        }

        /// <summary>
        /// 控件名称
        /// </summary>
        public string ControlName { get; set; }
    }
}
