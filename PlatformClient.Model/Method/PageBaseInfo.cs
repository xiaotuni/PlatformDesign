using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace PlatformClient.Model.Method
{
    /// <summary>
    /// PageName="测试页面" 
    /// PageHeight="600" 
    /// PageWidth="800" 
    /// SerialNum="1" 
    /// BackGroundColor="" 
    /// PageGUID="测试表2_2" 
    /// Charset="GB2312" 
    /// Language="zh-cn"
    /// </summary>
    public class PageBaseInfo
    {
        string _PageName;
        double _PageHeight = 600;
        double _PageWidth = 800;
        int _SerialNum = 1;
        string _BackGroundColor;
        string _PageGUID;
        string _Charset = "GB2312";
        string _Language = "zh-cn";
        /// <summary>
        /// 页面名称
        /// </summary>
        public string PageName { get { return _PageName; } set { _PageName = value; } }
        /// <summary>
        /// 页面GUID
        /// </summary>
        public string PageGUID { get { return _PageGUID; } set { _PageGUID = value; } }
        /// <summary>
        /// 宽
        /// </summary>
        public double PageHeight { get { return _PageHeight; } set { _PageHeight = value; } }
        /// <summary>
        /// 宽度
        /// </summary>
        public double PageWidth { get { return _PageWidth; } set { _PageWidth = value; } }
        /// <summary>
        /// 序号
        /// </summary>
        public int SerialNum { get { return _SerialNum; } set { _SerialNum = value; } }
        /// <summary>
        /// 背景色
        /// </summary>
        public string BackGroundColor { get { return _BackGroundColor; } set { _BackGroundColor = value; } }
        /// <summary>
        /// 字符编码
        /// </summary>
        public string Charset { get { return _Charset; } set { _Charset = value; } }
        /// <summary>
        /// 语言
        /// </summary>
        public string Language { get { return _Language; } set { _Language = value; } }
    }
}
