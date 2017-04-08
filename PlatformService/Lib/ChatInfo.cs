using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace PlatformService.Lib
{

    /// <summary>
    /// 变量信息类
    /// </summary>
    [DataContract]
    public class VariableInfo
    {
        /// <summary>
        /// 类型
        /// </summary>
        [DataMember]
        public string Type { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        [DataMember]
        public string Description { get; set; }
        /// <summary>
        /// 值
        /// </summary>
        [DataMember]
        public string Value { get; set; }
    }

    /// <summary>
    /// 聊天信息
    /// </summary>
    [DataContract]
    public class ChatInfo
    {
        /// <summary>
        /// 用户列表
        /// </summary>
        [DataMember]
        public List<String> UserItem { get; set; }
        /// <summary>
        /// 用户登录聊天时的名称
        /// </summary>
        [DataMember]
        public string LogonName { get; set; }
        /// <summary>
        /// 和谁进行聊天那人的名称
        /// </summary>
        [DataMember]
        public string ToUser { get; set; }
        /// <summary>
        /// 消息内容
        /// </summary>
        [DataMember]
        public string Msg { get; set; }
        /// <summary>
        /// 发送时间
        /// </summary>
        [DataMember]
        public DateTime SendTime { get; set; }


    }
}