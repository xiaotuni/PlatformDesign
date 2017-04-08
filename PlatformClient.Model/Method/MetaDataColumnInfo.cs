using System;
using System.ComponentModel;

namespace PlatformClient.Model.Method
{
    /// <summary>
    /// 元素信息
    /// </summary>
    public class MetaDataColumnInfo : INotifyPropertyChanged
    {
        bool _IsChoose;
        bool _IsHide;

        /// <summary>
        /// 表名称
        /// </summary>
        public string table_name { get; set; }
        /// <summary>
        /// 表的备注
        /// </summary>
        public string table_comment { get; set; }
        /// <summary>
        /// 表的类型
        /// </summary>
        public string table_type { get; set; }

        ///// <summary>
        ///// 列的顺序
        ///// </summary>
        //public int ordinal_position { get; set; }
        /// <summary>
        /// 最大长度
        /// </summary>
        public int character_maximum_length { get; set; }
        /// <summary>
        /// 列的名称
        /// </summary>
        public string column_name { get; set; }
        /// <summary>
        /// 数据类型
        /// </summary>
        public string data_type { get; set; }
        /// <summary>
        /// 是否可以为空
        /// </summary>
        public string is_nullable { get; set; }
        /// <summary>
        /// 列的备注
        /// </summary>
        public string column_comment { get; set; }
        /// <summary>
        /// 主值
        /// </summary>
        public string column_key { get; set; }
        /// <summary>
        /// 列的默认值
        /// </summary>
        public string column_default { get; set; }

        /// <summary>
        /// 此列的值
        /// </summary>
        public string column_value { get; set; }

        /// <summary>
        /// 是否选择
        /// </summary>
        public bool IsChoose
        {
            get { return _IsChoose; }
            set
            {
                _IsChoose = value;
                _PropertyChangedMethod("IsChoose");
            }
        }

        /// <summary>
        /// 是否隐藏
        /// </summary>
        public bool IsHide
        {
            get { return _IsHide; }
            set
            {
                _IsHide = value;
                _PropertyChangedMethod("IsHide");
            }
        }
        /// <summary>
        /// 控件类型
        /// </summary>
        string _control_type = "ControlString";
        /// <summary>
        /// 控件类型 default=""
        /// </summary>
        public string control_type
        {
            get { return _control_type; }
            set
            {
                _control_type = value;
                _PropertyChangedMethod("control_type");
            }
        }

        //string _display_name;
        ///// <summary>
        ///// 显示名称
        ///// </summary>
        //public string display_name
        //{
        //    get { return String.IsNullOrEmpty(_display_name) ? column_comment : _display_name; }
        //    set
        //    {
        //        _display_name = value;
        //        _PropertyChangedMethod("display_name");
        //    }
        //}

        /// <summary>
        /// 
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        void _PropertyChangedMethod(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName) || null == PropertyChanged)
            {
                return;
            }
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0}【{1}】", this.column_comment, this.column_name);
        }
    }
}
