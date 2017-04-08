using System;
using System.ComponentModel;

namespace PlatformClient.Model.Method
{
    /// <summary>
    /// 
    /// </summary>
    public class BaseAttribute : Attribute, INotifyPropertyChanged
    {
        private object _DefaultValue;
        /// <summary>
        /// 属性名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        ///  反射用的程序集名称
        /// </summary>
        public string AssemblyQualifiedName { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 控件 UIElement
        /// </summary>
        public object Source { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public object DefaultValue
        {
            get { return _DefaultValue; }
            set
            {
                _DefaultValue = value;
                _PropertyChangedMethod("DefaultValue");
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        void _PropertyChangedMethod(string propertyName)
        {
            if (null != PropertyChanged)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Description;
        }
    }
}
