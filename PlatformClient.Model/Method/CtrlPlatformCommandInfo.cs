using System;
using System.Collections.Generic;
using System.Xml.Linq;
using PlatformClient.Extend.Core;

namespace PlatformClient.Model.Method
{
    /// <summary>
    /// 复合控件命令对象类
    /// </summary>
    public class CtrlPlatformCommandInfo
    {
        /// <summary>
        /// 命令名称
        /// </summary>
        public string CommandName { get; set; }
        /// <summary>
        /// 命令类型
        /// </summary>
        public CtrlCommandDirectionType CommandType { get; set; }

        /// <summary>
        /// 来到于哪个模块
        /// </summary>
        public CtrlCommandDirectionType FromModel { get; set; }

        /// <summary>
        /// 到哪个模块
        /// </summary>
        public CtrlCommandDirectionType ToModel { get; set; }
        /// <summary>
        /// 参数集合【MetaDataInfo.ToElement();】
        /// </summary>
        public XElement ParamCollection { get; set; }

        /// <summary>
        /// 元信息
        /// </summary>
        public MetaDataInfo MetaDataInfo { get; set; }
        /// <summary>
        /// 控件信息
        /// </summary>
        public ControlInfo ControlInfo { get; set; }

        /// <summary>
        /// 临时存放的值
        /// </summary>
        public XElement TempValue { get; set; }
        ///// <summary>
        ///// SQL语句
        ///// </summary>
        //public List<string> SqlSentence { get; set; }
        /// <summary>
        /// 执行SQL语句集合[insert,update,select,delete and so on ...]
        /// </summary>
        public List<CtrlExecSQLCmd> ExecSql { get; set; }
        /// <summary>
        /// 表的名称
        /// </summary>
        public string TableName { get; set; }
        ///// <summary>
        ///// 返回数据
        ///// </summary>
        //public XElement DataTable { get; set; }
        /// <summary>
        /// 返回执行的结果
        /// </summary>
        public string ExecuteNonQueryResult { get; set; }
        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrorMessage { get; set; }
        /// <summary>
        /// 复合控件名称
        /// </summary>
        public string CompositeControlName { get; set; }
        /// <summary>
        /// 页面GUID
        /// </summary>
        public string PageGuid { get; set; }

        /// <summary>
        /// 公共值
        /// </summary>
        public XElement PublicValue
        {
            get
            {
                _PublicValue = GlobalVariables.Values.ToXElement();
                return _PublicValue;
            }
            set
            {
                _PublicValue = value;
                GlobalVariables.Clear();
                var pvs = _PublicValue.ToModelList<VariableInfo>();
                if (null != pvs)
                {
                    foreach (var v in pvs)
                    {
                        AddGlobalVariable(v.Name, v);
                    }
                }
            }
        }
        /// <summary>
        /// 全局变量
        /// </summary>
        static Dictionary<string, VariableInfo> GlobalVariables = new Dictionary<string, VariableInfo>();
        private XElement _PublicValue;
        /// <summary>
        /// 获取变量
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static VariableInfo GetGlobalVariable(string key)
        {
            if (null == GlobalVariables || String.IsNullOrEmpty(key) || 0 == GlobalVariables.Count)
            {
                return GlobalVariables[key];
            }
            return null;
        }
        /// <summary>
        /// 添加变量
        /// </summary>
        /// <param name="key"></param>
        /// <param name="vi"></param>
        public static void AddGlobalVariable(string key, VariableInfo vi)
        {
            if (String.IsNullOrEmpty(key) || null == vi)
            {
                return;
            }
            if (GlobalVariables.ContainsKey(key))
            {
                GlobalVariables[key] = vi;
            }
            GlobalVariables.Add(key, vi);
        }
        /// <summary>
        /// 更新变量
        /// </summary>
        /// <param name="key"></param>
        /// <param name="vi"></param>
        public static void UpdateGlobalVariable(string key, VariableInfo vi)
        {
            if (String.IsNullOrEmpty(key))
            {
                return;
            }

            if (GlobalVariables.ContainsKey(key))
            {
                GlobalVariables[key] = vi;
            }
        }
        /// <summary>
        /// 删除变量
        /// </summary>
        /// <param name="key"></param>
        public static void RemoveGlobalVariable(string key)
        {
            if (GlobalVariables.ContainsKey(key))
            {
                GlobalVariables.Remove(key);
            }
        }
    }
}
