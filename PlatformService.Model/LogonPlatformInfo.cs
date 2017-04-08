using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlatformService.Model
{
    /// <summary>
    /// 平台登录信息类
    /// </summary>
    public class LogonPlatformInfo
    {
        /// <summary>
        /// 登录名
        /// </summary>
        public string LogonName { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }
        
    }
}
