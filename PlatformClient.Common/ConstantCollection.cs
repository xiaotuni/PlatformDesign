
using System;
namespace PlatformClient.Common
{

    ///// <summary>
    ///// 常量集合
    ///// </summary>
    //public class ConstantCollection
    //{
    //    //(suffix 后缀)

    //    /// <summary>
    //    /// 高 26
    //    /// </summary>
    //    public static readonly double HEIGHT = 26;
    //    /// <summary>
    //    /// 控件配置文件【Config/ControlConfig.xml】
    //    /// </summary>
    //    public static readonly string Path_ControlConfig = string.Format("Config/ControlConfig.xml");
    //    /// <summary>
    //    /// 控件模板配置文件【Config/ControlTemplet.xml】
    //    /// </summary>
    //    public static readonly string Path_ControlTemplet = string.Format("Config/ControlTemplet.xml");
    //    /// <summary>
    //    /// 页面模板配置文件【Config/PageTemplet.xml】
    //    /// </summary>
    //    public static readonly string Path_PageTemplet = string.Format("Config/PageTemplet.xml");
    //    /// <summary>
    //    /// 节点行为配置文件【Config/ActivityConfig.xml】
    //    /// </summary>
    //    public static readonly string Path_ActivityConfig = string.Format("Config/ActivityConfig.xml");
    //    /// <summary>
    //    /// 控件方法配置文件【Config/ControlFunctionConfig.xml】
    //    /// </summary>
    //    public static readonly string Path_ControlFunctionConfig = string.Format("Config/ControlFunctionConfig.xml");
    //    /// <summary>
    //    /// 获取DataTable的时候；SqlSentence里存放查询语句的格式：
    //    /// {   
    //    ///     sql1;tableName1
    //    ///     sql2;tableName2
    //    /// }
    //    /// </summary>
    //    public static readonly string CommandName_GetDataTable = "GetDataTable";
    //    /// <summary>
    //    /// Delete、Update 获取执行结果；SqlSentence里存放查询语句的格式：
    //    /// {   
    //    ///     sql1;
    //    ///     sql2;
    //    ///     ...
    //    ///     sqln;
    //    /// }
    //    /// </summary>
    //    public static readonly string CommandName_ExecuteNonQuery = "ExecuteNonQuery";
    //    /// <summary>
    //    /// 保存页面内容
    //    /// {
    //    ///     xaml;
    //    ///     xml;
    //    /// }SavePageInfo
    //    /// </summary>
    //    public static readonly string CommandName_SavePageInfo = "SavePageInfo";
    //    /// <summary>
    //    /// 保存到 PageDirectorySub 表里去。
    //    /// </summary>
    //    public static readonly string CommandName_SavePageDirectorySub = "SavePageDirectorySub";
    //    /// <summary>
    //    /// 保存到 PageDirectory表
    //    /// </summary>
    //    public static readonly string CommandName_SavePageDirectory = "SavePageDirectory";
    //    /// <summary>
    //    /// 保存到EventDesigner表中去
    //    /// </summary>
    //    public static readonly string CommandName_SaveEventDesigner = "SaveEventDesigner";
    //    /// <summary>
    //    /// 初始化方法【InitLoad】
    //    /// </summary>
    //    public static readonly string INIT_FUNCTION = "InitLoad";
    //    /// <summary>
    //    /// Form 前缀 【值=Form】
    //    /// </summary>
    //    public static readonly string FORM_PREFIX = "Form";
    //    /// <summary>
    //    /// 表的主键【PRI】
    //    /// </summary>
    //    public static readonly string TABLE_KEY = "PRI";

    //    /// <summary>
    //    ///  时
    //    /// </summary>
    //    public static readonly string DATE_FORMAT_HH = "HH";
    //    /// <summary>
    //    ///  时：分
    //    /// </summary>
    //    public static readonly string DATE_FORMAT_HH_MM = "HH:mm";
    //    /// <summary>
    //    ///  时：分：秒
    //    /// </summary>
    //    public static readonly string DATE_FORMAT_HH_MM_SS = "HH:mm:ss";
    //    /// <summary>
    //    ///  年
    //    /// </summary>
    //    public static readonly string DATE_FORMAT_YYYY = "YYYY";
    //    /// <summary>
    //    ///  年-月
    //    /// </summary>
    //    public static readonly string DATE_FORMAT_YYYY_MM = "yyyy-MM";
    //    /// <summary>
    //    ///  年-月-日
    //    /// </summary>
    //    public static readonly string DATE_FORMAT_YYYY_MM_DD = "yyyy-MM-dd";
    //    /// <summary>
    //    ///  年-月-日 时
    //    /// </summary>
    //    public static readonly string DATE_FORMAT_YYYY_MM_DD_HH = "yyyy-MM-dd HH";
    //    /// <summary>
    //    ///  年-月-日 时：分
    //    /// </summary>
    //    public static readonly string DATE_FORMAT_YYYY_MM_DD_HH_MM = "yyyy-MM-dd HH:mm";
    //    /// <summary>
    //    /// 长日期格式
    //    /// </summary>
    //    public static readonly string DATE_FORMAT_YYYY_MM_DD_HH_MM_SS = "yyyy-MM-dd HH:mm:ss";
    //    /// <summary>
    //    /// 选择事件【SelectionChanged】事件
    //    /// </summary>
    //    public static readonly string SELECTION_CHANGED = "SelectionChanged";
    //    /// <summary>
    //    /// 外键【mul】
    //    /// </summary>
    //    public static readonly string FOREIGN_KEY = "MUL";
    //    /// <summary>
    //    /// ComboBox控件 【ControlComboBox】
    //    /// </summary>
    //    public static readonly string CONTROL_COMBOBOX = "ControlComboBox";
    //    /// <summary>
    //    /// 复合控件的名称【CompositeControlName】
    //    /// </summary>
    //    public static readonly string COMPOSITE_CONTROL_NAME = "CompositeControlName";
    //    /// <summary>
    //    /// 方法名称【MethodName】
    //    /// </summary>
    //    public static readonly string METHOD_NAME = "MethodName";
    //    /// <summary>
    //    /// TabControl.SelectedIndex索引值【Index】
    //    /// </summary>
    //    public static readonly string TABCONTROL_SELECTED_INDEX = "Index";
    //    /// <summary>
    //    /// 方法名称【FunctionName】
    //    /// </summary>
    //    public static readonly string FUNCTION_NAME = "FunctionName";
    //    /// <summary>
    //    /// 工作流ID【WorkFlowID】
    //    /// </summary>
    //    public static readonly string WORK_FLOW_ID = "WorkFlowID";
    //    /// <summary>
    //    /// 复合控件类型【CompositeCtrl】
    //    /// </summary>
    //    public static readonly string COMPOSITE_CONTROL = "CompositeCtrl";
    //    /// <summary>
    //    /// 复合控件刷新【Refresh】
    //    /// </summary>
    //    public static readonly string COMPOSITE_CONTROL_REFRESH = "Refresh";
    //    /// <summary>
    //    /// 初始化完成【InitLoadComplate】
    //    /// </summary>
    //    public static readonly string COMPOSITE_CONTROL_INIT_LOAD_COMPLATE = "InitLoadComplate";
    //    /// <summary>
    //    /// 初始化【InitLoad】
    //    /// </summary>
    //    public static readonly string COMPOSITE_CONTROL_INIT_LOAD = "InitLoad";
    //    /// <summary>
    //    /// 设置属性【SetProperty】
    //    /// </summary>
    //    public static readonly string COMPOSITE_CONTROL_SET_PROPERTY = "SetProperty";
    //    /// <summary>
    //    /// 获取属性【GetProperty】
    //    /// </summary>
    //    public static readonly string COMPOSITE_CONTROL_GET_PROPERTY = "GetProperty";
    //    /// <summary>
    //    /// 输出参数【OutputParameter】
    //    /// </summary>
    //    public static readonly string COMPOSITE_CONTROL_OUTPUT_PARAMETER = "OutputParameter";
    //    /// <summary>
    //    /// 输入参数【InputParameter】
    //    /// </summary>
    //    public static readonly string COMPOSITE_CONTROL_INPUT_PARAMETER = "InputParameter";
    //    /// <summary>
    //    /// 刷新完成【RefreshComplate】
    //    /// </summary>
    //    public static readonly string COMPOSITE_CONTROL_REFRESH_COMPLATE = "RefreshComplate";
    //    /// <summary>
    //    /// 获取当前的时候【yyyyMMddHHmmss】
    //    /// </summary>
    //    public static readonly string GET_CURRENT_DATETIME = string.Format("{0}", DateTime.Now.ToString("yyyyMMddHHmmss"));
    //    /// <summary>
    //    /// 动态程序集名称【DynamicallyAssemblyName】
    //    /// </summary>
    //    public static readonly string DYNAMICALLY_ASSEMBLY_NAME = "DynamicallyAssemblyName";
    //    /// <summary>
    //    /// 动态程序集的前缀名称【LHB】
    //    /// </summary>
    //    public static readonly string DYNAMICALLY_ASSEMBLY_NAME_PREFIX = "LHB";
    //}
}
