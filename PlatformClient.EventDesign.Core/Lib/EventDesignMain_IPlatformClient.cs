using System;
using System.Reflection;
using PlatformClient.PlatformServer;

namespace PlatformClient.EventDesign.Core
{
    //public partial class EventDesignMain : IPlatformClient
    //{
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <param name="cmd"></param>
    //    /// <param name="client"></param>
    //    public void SendToService(PlatformCommandInfo cmd, IPlatformClient client)
    //    {
    //        ClientCommand cc = new ClientCommand();
    //        cc.SendToService(cmd, client);
    //    }
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <param name="cmd"></param>
    //    public void DoAction(PlatformCommandInfo cmd)
    //    {
    //        try
    //        {
    //            if (null == cmd || null == cmd.TempValue)
    //            {
    //                return;
    //            }
    //            var att = cmd.TempValue.Attribute("FunctionName");
    //            if (null == att)
    //            {
    //                return;
    //            }
    //            string functionName = att.Value;
    //            string methodName = string.Format("Process{0}", functionName);
    //            Type tt = this.GetType();
    //            MethodInfo method = tt.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.CreateInstance);
    //            if (null == method)
    //            {
    //                return;
    //            }
    //            method.Invoke(this, new object[] { cmd });
    //        }
    //        catch { }
    //    }
    //}
}
