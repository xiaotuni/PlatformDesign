using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using MySql.Data.MySqlClient;
using PlatformService.Utility;

namespace PlatformService.Database
{
    /// <summary>
    /// 
    /// </summary>
    internal class MySQLHelper : IDatabase
    {
        string _ConnectionString;
        string ConnectionString
        {
            get
            {
                if (String.IsNullOrEmpty(_ConnectionString))
                {
                    _ConnectionString = ConfigurationManager.ConnectionStrings["MySqlConnection"].ConnectionString;
                    _ConnectionString = Wrapper.Decrypt(_ConnectionString);
                    DBAccess.printf(_ConnectionString);
                }
                return _ConnectionString;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        MySqlConnection Connection()
        {
            this.ErrorMessage = string.Empty;
            try
            {
                MySqlConnection cn = new MySqlConnection(ConnectionString);
                cn.Open();
                return cn;
            }
            catch (Exception ee)
            {
                printf(ee);
                return null;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        MySqlCommand Command()
        {
            this.ErrorMessage = string.Empty;
            try
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = this.Connection();
                return cmd;
            }
            catch (Exception ee)
            {
                printf(ee);
                return null;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="executeSql"></param>
        /// <returns></returns>
        public int ExecuteNonQuery(string executeSql)
        {
            this.ErrorMessage = string.Empty;
            try
            {
                using (MySqlCommand cmd = Command())
                {
                    cmd.CommandText = executeSql;
                    var result = cmd.ExecuteNonQuery();
                    cmd.Connection.Close();
                    //cmd.Connection.Dispose();
                    //cmd.Dispose();
                    return result;
                }
            }
            catch (Exception ee)
            {
                printf(ee);
                return -1;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="executeSql"></param>
        /// <param name="oParams"></param>
        /// <returns></returns>
        public int ExecuteNonQuery(string executeSql, params DbParameter[] oParams)
        {
            this.ErrorMessage = string.Empty;
            using (MySqlCommand cmd = Command())
            {
                try
                {
                    cmd.CommandText = executeSql;
                    cmd.Parameters.AddRange(oParams);
                    int result = cmd.ExecuteNonQuery();
                    cmd.Connection.Close();
                    //cmd.Connection.Dispose();
                    //cmd.Dispose();
                    return result >= 0 ? 0 : -1; ;
                }
                catch (Exception ee)
                {
                    printf(ee);
                    return -1;
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sqlItem"></param>
        /// <returns></returns>
        public int ExecuteNonQuery(List<string> sqlItem)
        {
            this.ErrorMessage = string.Empty;
            using (MySqlCommand cmd = Command())
            {
                if (null == cmd || null == cmd.Connection)
                {
                    this.ErrorMessage = "创建Connection对象时，知道";
                    return -1;
                }
                MySqlTransaction tr = cmd.Connection.BeginTransaction();
                bool isComplete = true;
                foreach (var sql in sqlItem)
                {
                    try
                    {
                        cmd.CommandText = sql;
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ee)
                    {
                        printf(ee);
                        isComplete = false;
                        break;
                    }
                }
                int result = 0;
                if (isComplete)
                {
                    tr.Commit();
                }
                else
                {
                    tr.Rollback();
                    result = -1;
                }
                cmd.Connection.Close();
                //cmd.Connection.Dispose();
                //cmd.Dispose();

                return result;
            }

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="executeSql"></param>
        /// <returns></returns>
        public object ExecuteScalar(string executeSql)
        {
            this.ErrorMessage = string.Empty;
            using (MySqlCommand cmd = Command())
            {
                try
                {
                    if (null == cmd)
                    {
                        DBAccess.printf("ExecuteScalar:创建MySqlCommand失败 ");
                        return null;
                    }

                    cmd.CommandText = executeSql;
                    object value = cmd.ExecuteScalar();
                    cmd.Connection.Close();
                    //cmd.Connection.Dispose();
                    //cmd.Dispose();
                    return value;
                }
                catch (Exception ee)
                {
                    printf(ee);
                    return null;
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="executeSql"></param>
        /// <returns></returns>
        public DataSet GetDataSet(string executeSql)
        {
            this.ErrorMessage = string.Empty;
            return GetDataSet(executeSql, "DataTable");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="executeSql"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public DataSet GetDataSet(string executeSql, string name)
        {
            this.ErrorMessage = string.Empty;
            using (MySqlConnection cn = Connection())
            {
                try
                {
                    MySqlDataAdapter odda = new MySqlDataAdapter(executeSql, cn);
                    DataSet ds = new DataSet();
                    if (cn == null)
                    {
                        return null;
                    }
                    else
                    {
                        odda.Fill(ds, name);
                        cn.Close();
                        //cn.Dispose();
                        return ds;
                    }
                }
                catch (Exception ee)
                {
                    printf(ee);
                    return null;
                }
            }
        }

        private void printf(Exception ee)
        {
            this.ErrorMessage = string.Format("{0}", ee.ToString());
            DBAccess.printf(ee);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="procedureName"></param>
        /// <returns></returns>
        public DataSet ExecuteProcedure(string procedureName)
        {
            return null;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="executeSql"></param>
        /// <returns></returns>
        public DataTable GetDataTable(string executeSql)
        {
            this.ErrorMessage = string.Empty;
            try
            {
                DataSet ds = this.GetDataSet(executeSql);
                return null != ds ? ds.Tables[0] : null;
            }
            catch (Exception ee)
            {
                printf(ee);
                return null;
            }
        }

        public string ErrorMessage { get; set; }
    }
}
