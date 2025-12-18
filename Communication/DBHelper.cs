using System;
using System.Data;
using System.Data.SqlClient;

namespace VTP_Induction
{

    public class DBHelper
    {
        public static ResultClass.resultStruct insertLoginLog(string dbType, string connStr)
        {
            ResultClass.resultStruct result = default(ResultClass.resultStruct);
            try
            {
                if (dbType == "SQLSERVER")
                {
                    SqlConnection sqlConnection = new SqlConnection(connStr);
                    sqlConnection.Open();
                    SqlCommand sqlCommand = new SqlCommand("insert into dbo.WCS_WORK_LOG(LOG_TYPE,LOG_TEXT,CREATE_TIME) VALUES(3,N'[用户名]：管理员，[登录名]：admin 打开WCS-PLC接口程序','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "')", sqlConnection);
                    sqlCommand.ExecuteNonQuery();
                    if (sqlConnection.State.Equals(ConnectionState.Open))
                    {
                        sqlConnection.Close();
                        sqlConnection.Dispose();
                    }
                }
                else if (!(dbType == "Oracle"))
                {
                }
                result.result = 1;
            }
            catch (Exception ex)
            {
                result.result = 0;
                result.ExceptionMsg = ex.Message;
            }
            return result;
        }

        public static ResultClass.resultStruct loadAdminUser(string dbType, string connStr)
        {
            ResultClass.resultStruct result = default(ResultClass.resultStruct);
            try
            {
                if (dbType == "SQLSERVER")
                {
                    SqlConnection sqlConnection = new SqlConnection(connStr);
                    sqlConnection.Open();
                    SqlCommand selectCommand = new SqlCommand("SELECT LOGIN_NAME,PASSWORD FROM dbo.SYS_USER WHERE LOGIN_NAME = 'admin'", sqlConnection);
                    SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(selectCommand);
                    result.table = new DataTable();
                    sqlDataAdapter.Fill(result.table);
                    if (sqlConnection.State.Equals(ConnectionState.Open))
                    {
                        sqlConnection.Close();
                        sqlConnection.Dispose();
                    }
                }
                else if (!(dbType == "Oracle"))
                {
                }
                result.result = 1;
            }
            catch (Exception ex)
            {
                result.result = 0;
                result.ExceptionMsg = ex.Message;
            }
            return result;
        }

        public static ResultClass.resultStruct insertLogoutLog(string dbType, string connStr)
        {
            ResultClass.resultStruct result = default(ResultClass.resultStruct);
            try
            {
                if (dbType == "SQLSERVER")
                {
                    SqlConnection sqlConnection = new SqlConnection(connStr);
                    sqlConnection.Open();
                    SqlCommand sqlCommand = new SqlCommand("insert into dbo.WCS_WORK_LOG(LOG_TYPE,LOG_TEXT,CREATE_TIME) VALUES(3,N'[用户名]：管理员，[登录名]：admin 关闭WCS-PLC接口程序','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "')", sqlConnection);
                    sqlCommand.ExecuteNonQuery();
                    if (sqlConnection.State.Equals(ConnectionState.Open))
                    {
                        sqlConnection.Close();
                        sqlConnection.Dispose();
                    }
                }
                else if (!(dbType == "Oracle"))
                {
                }
                result.result = 1;
            }
            catch (Exception ex)
            {
                result.result = 0;
                result.ExceptionMsg = ex.Message;
            }
            return result;
        }

        public static ResultClass.resultStruct loadDvcTable(string dbType, string connStr, string dvcTp)
        {
            ResultClass.resultStruct result = default(ResultClass.resultStruct);
            try
            {
                string Connect = "Data Source=DESKTOP-IVJB6QI\\KTEAM;Initial Catalog=WCS_Viettel;Integrated Security= True";
                if (!(dbType == "Oracle") && dbType == "SQLSERVER")
                {
                    SqlConnection sqlConnection = new SqlConnection(connStr);
                    sqlConnection.Open();
                    SqlCommand sqlCommand = new SqlCommand("SELECT DVC_IP,PORT,DVC_NO,PARENT_PLCID,DVC_TP,COMMENTS FROM dbo.WCS_DVC WHERE (DVC_TP = 'PLC' OR DVC_TP = 'BCR' OR DVC_TP = 'PRINTER') AND DVC_ST = 1 ORDER BY DVC_TP,DVC_NO ASC", sqlConnection);
                    SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
                    result.table = new DataTable();
                    sqlDataAdapter.Fill(result.table);
                    sqlDataAdapter.Dispose();
                    sqlCommand.Dispose();
                    if (sqlConnection.State.Equals(ConnectionState.Open))
                    {
                        sqlConnection.Close();
                        sqlConnection.Dispose();
                    }
                }
                result.result = 1;
            }
            catch (Exception ex)
            {
                result.result = 0;
                result.ExceptionMsg = ex.Message;
            }
            return result;
        }
    }
}
