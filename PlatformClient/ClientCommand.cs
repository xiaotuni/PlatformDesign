using System;
using System.Diagnostics;
using PlatformClient.Extend.Core;
using PlatformClient.Model.Method;
using PlatformClient.PlatformServer;

namespace PlatformClient
{
    /// <summary>
    /// 命令对象
    /// </summary>
    public class ClientCommand : IPlatformClient
    {
        void client_DoActionCompleted(object sender, DoActionCompletedEventArgs e)
        {
            try
            {
                //var cmd = GetCtrlPlatformCommandInfo(e.Result);
                //PrintfLog(cmd, "返回命令信息");
                (e.UserState as IPlatformClient).DoAction(CommandHelper.ToCtrlPlatformCommandInfo(e.Result));
            }
            catch (Exception ee)
            {
                PrintfLog(ee);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmd"></param>
        public void DoAction(CtrlPlatformCommandInfo cmd)
        {

        }
        /// <summary>
        /// 发送命令
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="client"></param>
        public void SendToService(CtrlPlatformCommandInfo cmd, IPlatformClient client)
        {
            try
            {
                PlatformServiceClient psc = new PlatformServiceClient();
                psc.DoActionCompleted += client_DoActionCompleted;
                psc.DoActionAsync(CommandHelper.ToPlatfromCommandInfo(cmd), client);
            }
            catch (Exception ee)
            {
                PrintfLog(ee);
            }
            //  PrintfLog(cmd, "发送命令信息");
        }

        CtrlPlatformCommandInfo GetCtrlPlatformCommandInfo(PlatformCommandInfo cmd)
        {
            var ctrlCmd = new CtrlPlatformCommandInfo();
            ctrlCmd.CommandName = cmd.CommandName;
            ctrlCmd.CommandType = cmd.CommandType.CloneEnum<CtrlCommandDirectionType>();//.CompositeControl;
            ctrlCmd.FromModel = cmd.FromModel.CloneEnum<CtrlCommandDirectionType>();//.ClientUI;
            ctrlCmd.ToModel = cmd.ToModel.CloneEnum<CtrlCommandDirectionType>();
            ctrlCmd.ParamCollection = cmd.ParamCollection;
            //ctrlCmd.MetaDataInfo = cmd.MetaDataInfo;
            //ctrlCmd.ControlInfo = cmd.ControlInfo
            ctrlCmd.PublicValue = cmd.PublicValue;
            ctrlCmd.TempValue = cmd.TempValue;
            ctrlCmd.ExecSql = cmd.ExecSql.CloneList<CtrlExecSQLCmd>();
            ctrlCmd.TableName = cmd.TableName;
            //ctrlCmd.DataTable = cmd.DataTable;
            ctrlCmd.ExecuteNonQueryResult = cmd.ExecuteNonQueryResult;
            ctrlCmd.ErrorMessage = cmd.ErrorMessage;
            //ctrlCmd.CompositeControlName = cmd.CompositeControlName;
            return ctrlCmd;
        }

        PlatformCommandInfo GetPlatfromCommandInfo(CtrlPlatformCommandInfo ctrlCmd)
        {
            var cmd = new PlatformCommandInfo();
            cmd.CommandName = ctrlCmd.CommandName;
            cmd.CommandType = ctrlCmd.CommandType.CloneEnum<CommandDirectionType>();//.CompositeControl;
            cmd.FromModel = ctrlCmd.FromModel.CloneEnum<CommandDirectionType>();//.ClientUI;
            cmd.ToModel = ctrlCmd.ToModel.CloneEnum<CommandDirectionType>();
            cmd.ParamCollection = ctrlCmd.ParamCollection;
            //cmd.MetaDataInfo = ctrlCmd.MetaDataInfo;
            //cmd.ControlInfo = ctrlCmd.ControlInfo
            cmd.PublicValue = ctrlCmd.PublicValue;
            cmd.TempValue = ctrlCmd.TempValue;
            cmd.ExecSql = ctrlCmd.ExecSql.CloneList<ExecSQLCmd>();
            cmd.TableName = ctrlCmd.TableName;
            //cmd.DataTable = ctrlCmd.DataTable;
            cmd.ExecuteNonQueryResult = ctrlCmd.ExecuteNonQueryResult;
            cmd.ErrorMessage = ctrlCmd.ErrorMessage;
            //cmd.CompositeControlName = ctrlCmd.CompositeControlName;
            return cmd;
        }

        /// <summary>
        /// 打印日志到控制台
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="str">内容</param>
        void PrintfLog(CtrlPlatformCommandInfo cmd, string str)
        {
            try
            {
                string log = string.Format("----------------------------------Begin--{0}-{1}-----------------\r\n", cmd.CommandName, str);
                log += string.Format("Command Name is:{0};Command Type is {1}", cmd.CommandName, cmd.CommandType);
                log += string.Format("\r\n {0} ", cmd.ToXElement());
                log += string.Format("\r\n---------------------------------------------------End---{0}-{1}--------------------------------\r\n", cmd.CommandName, str);
                Debug.WriteLine("{0}-->{1}", DateTime.Now.ToString("yyyy-MM-dd- HH:mm:ss"), log);
            }
            catch { }
        }

        void PrintfLog(Exception ee)
        {
            try
            {
                Debug.WriteLine("{0}-->{1}", DateTime.Now.ToString("yyyy-MM-dd- HH:mm:ss"), ee);
            }
            catch { }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class CommandHelper
    {
        /// <summary>
        /// 返回命令信息
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public static CtrlPlatformCommandInfo ToCtrlPlatformCommandInfo(PlatformCommandInfo cmd)
        {
            var ctrlCmd = new CtrlPlatformCommandInfo();
            ctrlCmd.CommandName = cmd.CommandName;
            ctrlCmd.CommandType = cmd.CommandType.CloneEnum<CtrlCommandDirectionType>();//.CompositeControl;
            ctrlCmd.FromModel = cmd.FromModel.CloneEnum<CtrlCommandDirectionType>();//.ClientUI;
            ctrlCmd.ToModel = cmd.ToModel.CloneEnum<CtrlCommandDirectionType>();
            ctrlCmd.ParamCollection = cmd.ParamCollection;
            //ctrlCmd.MetaDataInfo = cmd.MetaDataInfo;
            //ctrlCmd.ControlInfo = cmd.ControlInfo
            ctrlCmd.PublicValue = cmd.PublicValue;
            ctrlCmd.TempValue = cmd.TempValue;
            ctrlCmd.ExecSql = cmd.ExecSql.CloneList<CtrlExecSQLCmd>();
            ctrlCmd.TableName = cmd.TableName;
            //ctrlCmd.DataTable = cmd.DataTable;
            ctrlCmd.ExecuteNonQueryResult = cmd.ExecuteNonQueryResult;
            ctrlCmd.ErrorMessage = cmd.ErrorMessage;
            //ctrlCmd.CompositeControlName = cmd.CompositeControlName;

            PrintfLog(ctrlCmd, "返回命令信息");
            return ctrlCmd;
        }
        /// <summary>
        /// 发送命令信息
        /// </summary>
        /// <param name="ctrlCmd"></param>
        /// <returns></returns>
        public static PlatformCommandInfo ToPlatfromCommandInfo(CtrlPlatformCommandInfo ctrlCmd)
        {
            var cmd = new PlatformCommandInfo();
            cmd.CommandName = ctrlCmd.CommandName;
            cmd.CommandType = ctrlCmd.CommandType.CloneEnum<CommandDirectionType>();//.CompositeControl;
            cmd.FromModel = ctrlCmd.FromModel.CloneEnum<CommandDirectionType>();//.ClientUI;
            cmd.ToModel = ctrlCmd.ToModel.CloneEnum<CommandDirectionType>();
            cmd.ParamCollection = ctrlCmd.ParamCollection;
            //cmd.MetaDataInfo = ctrlCmd.MetaDataInfo;
            //cmd.ControlInfo = ctrlCmd.ControlInfo
            cmd.PublicValue = ctrlCmd.PublicValue;
            cmd.TempValue = ctrlCmd.TempValue;
            cmd.ExecSql = ctrlCmd.ExecSql.CloneList<ExecSQLCmd>();
            cmd.TableName = ctrlCmd.TableName;
            //cmd.DataTable = ctrlCmd.DataTable;
            cmd.ExecuteNonQueryResult = ctrlCmd.ExecuteNonQueryResult;
            cmd.ErrorMessage = ctrlCmd.ErrorMessage;
            //cmd.CompositeControlName = ctrlCmd.CompositeControlName;

            PrintfLog(ctrlCmd, "发送命令信息");
            return cmd;
        }

        /// <summary>
        /// 打印日志到控制台
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="str">内容</param>
        public static void PrintfLog(CtrlPlatformCommandInfo cmd, string str)
        {
            try
            {
                string log = string.Format("----------------------------------Begin--{0}-{1}-----------------\r\n", cmd.CommandName, str);
                log += string.Format("Command Name is:{0};Command Type is {1}", cmd.CommandName, cmd.CommandType);
                log += string.Format("\r\n {0} ", cmd.ToXElement());
                log += string.Format("\r\n---------------------------------------------------End---{0}-{1}--------------------------------\r\n", cmd.CommandName, str);
                Debug.WriteLine("{0}-->{1}", DateTime.Now.ToString("yyyy-MM-dd- HH:mm:ss"), log);
            }
            catch { }
        }
    }
}
