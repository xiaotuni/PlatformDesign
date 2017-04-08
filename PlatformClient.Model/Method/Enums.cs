using System.ComponentModel;

namespace PlatformClient.Model.Method
{
    /// <summary>
    /// 加密类型
    /// </summary>
    public enum EncryptType
    {
        /// <summary>
        /// 明文
        /// </summary>
        [DescriptionAttribute("明文")]
        None,
        /// <summary>
        /// 常归加密
        /// </summary>
        [DescriptionAttribute("常归加密")]
        Encrypt,
        /// <summary>
        /// Md5加密
        /// </summary>
        [DescriptionAttribute("Md5加密")]
        Md5
    }
    /// <summary>
    /// 命令类型
    /// </summary>
    public enum CtrlCommandDirectionType
    {
        /// <summary>
        /// 复合控件
        /// </summary>
        CompositeControl,
        /// <summary>
        /// 数据类型
        /// </summary>
        DB,
        /// <summary>
        /// 消息
        /// </summary>
        Message,
        /// <summary>
        /// 客户UI界面
        /// </summary>
        ClientUI,
    }
    /// <summary>
    /// 执行SQL类型
    /// </summary>
    public enum CtrlExecSqlCmdType
    {
        /// <summary>
        /// 更新,插入,删除
        /// </summary>
        ExecuteNonQuery,
        /// <summary>
        /// 查询
        /// </summary>
        Query,
        /// <summary>
        /// 首行首例的值
        /// </summary>
        ExecuteScalar
    }
    /// <summary>
    /// 右键菜单类型
    /// </summary>
    public enum ContextMenuType
    {
        /// <summary>
        /// 删除
        /// </summary>
        Delete,
        /// <summary>
        /// 修改内容
        /// </summary>
        ModifyContent,
        /// <summary>
        /// 参数设置
        /// </summary>
        ParameterSettings,
    }

    /// <summary>
    /// 锚点位置
    /// </summary>
    public enum AuchorLocation
    {
        /// <summary>
        /// 
        /// </summary>
        上左,
        /// <summary>
        /// 
        /// </summary>
        上中,
        /// <summary>
        /// 
        /// </summary>
        上右,
        /// <summary>
        /// 
        /// </summary>
        右中,
        /// <summary>
        /// 
        /// </summary>
        左中,
        /// <summary>
        /// 
        /// </summary>
        下左,
        /// <summary>
        /// 
        /// </summary>
        下中,
        /// <summary>
        /// 
        /// </summary>
        下右
    }

    /// <summary>
    /// 日期类型
    /// </summary>
    public enum DateTimeType
    {
        /// <summary>
        /// 默认为短日期
        /// </summary>
        [DescriptionAttribute("默认为短日期格式(yyyy-MM-dd)")]
        Default,
        /// <summary>
        /// 短日期
        /// </summary>
        [DescriptionAttribute("短日期格式(yyyy-MM-dd)")]
        To_YYYY_MM_DD,
        /// <summary>
        /// 时间
        /// </summary>
        [DescriptionAttribute("时间格式(HH:mm:ss)")]
        To_HH_MM_SS,
        /// <summary>
        /// 年格式
        /// </summary>
        [DescriptionAttribute("年格式(yyyy)")]
        To_YYYY,
        /// <summary>
        /// 年-月格式
        /// </summary>
        [DescriptionAttribute("年-月格式(yyyy-MM)")]
        To_YYYY_MM,
        /// <summary>
        /// 年-月-日 时格式
        /// </summary>
        [DescriptionAttribute("年-月-日 时格式(yyyy-MM-dd HH)")]
        To_YYYY_MM_DD_HH,
        /// <summary>
        /// 年-月-日 时:分
        /// </summary>
        [DescriptionAttribute("年-月-日 时:分格式(yyyy-MM-dd HH:mm)")]
        To_YYYY_MM_DD_HH_MM,
        /// <summary>
        /// 长日期格式:yyyy-MM-dd HH:mm:ss
        /// </summary>
        [DescriptionAttribute("长日期格式(yyyy-MM-dd HH:mm:ss)")]
        To_YYYY_MM_DD_HH_MM_SS,
    }

    /// <summary>
    /// 操作类型
    /// </summary>
    public enum OperatorType
    {
        /// <summary>
        /// 
        /// </summary>
        None,
        /// <summary>
        /// 添加页面
        /// </summary>
        AddPage,
        /// <summary>
        /// 添加目录
        /// </summary>
        AddDirectory,
        /// <summary>
        /// 删除
        /// </summary>
        Delete,
        /// <summary>
        /// 修改
        /// </summary>
        Modify,
    }
}
