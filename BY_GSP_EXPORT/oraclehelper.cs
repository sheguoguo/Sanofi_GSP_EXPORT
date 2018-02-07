using System;
using System.Data;
using Oracle.DataAccess.Client;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;


namespace Sanofi_GSP_EXPORT
{
    /// <summary>
    /// Desciption: Oracle数据库访问类 (注：针对 odp.net ). 
    /// Author    : yenange
    /// Date      : 2013-09-21
    /// </summary>
    public static class oraclehelper
    {
        #region [ 连接对象 ]
        /// <summary>
        /// 连接对象 字段
        /// </summary>
        private static OracleConnection conn = null;
        /// <summary>
        /// 连接串 字段
        /// </summary>

        private static string connstr = @"Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=172.16.100.102)(PORT=1521)))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=wmrdc)));User Id=wmrdc;Password=wmrdc;";
        //private static string connstr = @"Data Source=wmrdc;User ID=wmrdc;Password=wmrdc;";

        /// <summary>
        /// 取得连接串
        /// </summary>
        /// 
        public static string GetConnectionString
        {
            
            get
            {
                return connstr;
            }
        }

        /// <summary>
        /// 取得连接对象, 没有打开
        /// </summary>
        public static OracleConnection GetOracleConnection
        {
            get
            {
                return new OracleConnection(GetConnectionString);
            }
        }

        /// <summary>
        /// 取得连接对象， 并打开
        /// </summary>
        public static OracleConnection GetOracleConnectionAndOpen
        {
            get
            {
                OracleConnection conn = GetOracleConnection;
                conn.Open();
                return conn;
            }
        }

        /// <summary>
        /// 彻底关闭并释放 OracleConnection 对象，再置为null.
        /// </summary>
        /// <param name="conn">OracleConnection</param>
        public static void CloseOracleConnection(OracleConnection conn)
        {
            if (conn == null)
                return;
            conn.Close();
            conn.Dispose();
            conn = null;
        }
        #endregion

        #region [ ExecuteNonQuery ]
        /// <summary>
        /// 普通SQL语句执行增删改
        /// </summary>
        /// <param name="cmdText">SQL语句</param>
        /// <param name="commandParameters">可变参数</param>
        /// <returns>受影响行数</returns>
        public static int ExecuteNonQuery(string cmdText, params OracleParameter[] commandParameters)
        {
            return ExecuteNonQuery(cmdText, CommandType.Text, commandParameters);
        }
        /// <summary>
        /// 存储过程执行增删改
        /// </summary>
        /// <param name="cmdText">存储过程</param>
        /// <param name="commandParameters">可变参数</param>
        /// <returns>受影响行数</returns>
        public static int ExecuteNonQueryByProc(string cmdText, params OracleParameter[] commandParameters)
        {
            return ExecuteNonQuery(cmdText, CommandType.StoredProcedure, commandParameters);
        }
        /// <summary>
        /// 执行增删改
        /// </summary>
        /// <param name="cmdText">命令字符串</param>
        /// <param name="cmdType">命令类型</param>
        /// <param name="commandParameters">可变参数</param>
        /// <returns>受影响行数</returns>
        public static int ExecuteNonQuery(string cmdText, CommandType cmdType, params OracleParameter[] commandParameters)
        {
            int result = 0;
            OracleConnection conn = null;

            try
            {
                conn = GetOracleConnectionAndOpen;
                OracleCommand command = new OracleCommand();
                PrepareCommand(command, conn, cmdType, cmdText, commandParameters);
                result = command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                result = -1;
            }
            finally
            {
                if (conn != null)
                    CloseOracleConnection(conn);
            }

            return result;
        }
        #endregion

        #region [ ExecuteReader ]
        /// <summary>
        /// SQL语句得到 OracleDataReader 对象
        /// </summary>
        /// <param name="cmdText">命令字符串</param>
        /// <param name="commandParameters">可变参数</param>
        /// <returns>OracleDataReader 对象</returns>
        public static OracleDataReader ExecuteReader(string cmdText, params OracleParameter[] commandParameters)
        {
            return ExecuteReader(cmdText, CommandType.Text, commandParameters);
        }
        /// <summary>
        /// 存储过程得到 OracleDataReader 对象
        /// </summary>
        /// <param name="cmdText">命令字符串</param>
        /// <param name="commandParameters">可变参数</param>
        /// <returns>OracleDataReader 对象</returns>
        public static OracleDataReader ExecuteReaderByProc(string cmdText, params OracleParameter[] commandParameters)
        {
            return ExecuteReader(cmdText, CommandType.StoredProcedure, commandParameters);
        }
        /// <summary>
        /// 得到 OracleDataReader 对象
        /// </summary>
        /// <param name="cmdText">命令字符串</param>
        /// <param name="cmdType">命令类型</param>
        /// <param name="commandParameters">可变参数</param>
        /// <returns>OracleDataReader 对象</returns>
        public static OracleDataReader ExecuteReader(string cmdText, CommandType cmdType, params OracleParameter[] commandParameters)
        {
            OracleDataReader result = null;
            OracleConnection conn = null;

            try
            {
                conn = GetOracleConnectionAndOpen;
                OracleCommand command = new OracleCommand();
                PrepareCommand(command, conn, cmdType, cmdText, commandParameters);
                result = command.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch (Exception ex)
            {
                result = null;
            }
            finally
            {
                if (conn != null)
                    CloseOracleConnection(conn);
            }

            return result;
        }
        #endregion

        #region [ ExecuteScalar ]
        /// <summary>
        /// 执行SQL语句, 返回Object
        /// </summary>
        /// <param name="cmdText">命令字符串</param>
        /// <param name="commandParameters">可变参数</param>
        /// <returns> Object </returns>
        public static Object ExecuteScalar(string cmdText, params OracleParameter[] commandParameters)
        {
            return ExecuteScalar(cmdText, CommandType.Text, commandParameters);
        }

        /// <summary>
        /// 执行存储过程, 返回Object
        /// </summary>
        /// <param name="cmdText">命令字符串</param>
        /// <param name="commandParameters">可变参数</param>
        /// <returns> Object </returns>
        public static Object ExecuteScalarByProc(string cmdText, params OracleParameter[] commandParameters)
        {
            return ExecuteScalar(cmdText, CommandType.StoredProcedure, commandParameters);
        }

        /// <summary>
        /// 返回Object
        /// </summary>
        /// <param name="cmdText">命令字符串</param>
        /// <param name="cmdType">命令类型</param>
        /// <param name="commandParameters">可变参数</param>
        /// <returns> Object </returns>
        public static Object ExecuteScalar(string cmdText, CommandType cmdType, params OracleParameter[] commandParameters)
        {
            Object result = null;
            OracleConnection conn = null;

            try
            {
                conn = GetOracleConnectionAndOpen;
                OracleCommand command = new OracleCommand();

                PrepareCommand(command, conn, cmdType, cmdText, commandParameters);
                result = command.ExecuteScalar();
            }
            catch (Exception ex)
            {
                result = null;
            }
            finally
            {
                if (conn != null)
                    CloseOracleConnection(conn);
            }

            return result;
        }
        #endregion

        #region [ ExecuteDataSet ]
        /// <summary>
        /// 执行SQL语句, 返回DataSet
        /// </summary>
        /// <param name="cmdText">命令字符串</param>
        /// <param name="commandParameters">可变参数</param>
        /// <returns> DataSet </returns>
        public static DataSet ExecuteDataSet(string cmdText, params OracleParameter[] commandParameters)
        {
            return ExecuteDataSet(cmdText, CommandType.Text, commandParameters);
        }

        /// <summary>
        /// 执行存储过程, 返回DataSet
        /// </summary>
        /// <param name="cmdText">命令字符串</param>
        /// <param name="commandParameters">可变参数</param>
        /// <returns> DataSet </returns>
        public static DataSet ExecuteDataSetByProc(string cmdText, params OracleParameter[] commandParameters)
        {
            return ExecuteDataSet(cmdText, CommandType.StoredProcedure, commandParameters);
        }

        /// <summary>
        /// 返回DataSet
        /// </summary>
        /// <param name="cmdText">命令字符串</param>
        /// <param name="cmdType">命令类型</param>
        /// <param name="commandParameters">可变参数</param>
        /// <returns> DataSet </returns>
        public static DataSet ExecuteDataSet(string cmdText, CommandType cmdType, params OracleParameter[] commandParameters)
        {
            DataSet result = null;
            OracleConnection conn = null;

            try
            {
                conn = GetOracleConnectionAndOpen;
                OracleCommand command = new OracleCommand();

                PrepareCommand(command, conn, cmdType, cmdText, commandParameters);
                OracleDataAdapter adapter = new OracleDataAdapter();
                adapter.SelectCommand = command;
                result = new DataSet();
                adapter.Fill(result);
            }
            catch (Exception ex)
            {
                result = null;
            }
            finally
            {
                if (conn != null)
                    CloseOracleConnection(conn);
            }

            return result;
        }
        #endregion

        #region [ ExecuteDataTable ]
        /// <summary>
        /// 执行SQL语句, 返回DataTable
        /// </summary>
        /// <param name="cmdText">命令字符串</param>
        /// <param name="commandParameters">可变参数</param>
        /// <returns> DataTable </returns>
        public static DataTable ExecuteDataTable(string cmdText, params OracleParameter[] commandParameters)
        {
            return ExecuteDataTable(cmdText, CommandType.Text, commandParameters);
        }

        /// <summary>
        /// 执行存储过程, 返回DataTable
        /// </summary>
        /// <param name="cmdText">命令字符串</param>
        /// <param name="commandParameters">可变参数</param>
        /// <returns> DataTable </returns>
        public static DataTable ExecuteDataTableByProc(string cmdText, params OracleParameter[] commandParameters)
        {
            return ExecuteDataTable(cmdText, CommandType.StoredProcedure, commandParameters);
        }

        /// <summary>
        /// 返回DataTable
        /// </summary>
        /// <param name="cmdText">命令字符串</param>
        /// <param name="cmdType">命令类型</param>
        /// <param name="commandParameters">可变参数</param>
        /// <returns> DataTable </returns>
        public static DataTable ExecuteDataTable(string cmdText, CommandType cmdType, params OracleParameter[] commandParameters)
        {
            DataTable dtResult = null;
            DataSet ds = ExecuteDataSet(cmdText, cmdType, commandParameters);

            if (ds != null && ds.Tables.Count > 0)
            {
                dtResult = ds.Tables[0];
            }
            return dtResult;
        }
        #endregion

        #region [ PrepareCommand ]
        /// <summary>
        /// Command对象执行前预处理
        /// </summary>
        /// <param name="command"></param>
        /// <param name="connection"></param>
        /// <param name="trans"></param>
        /// <param name="cmdType"></param>
        /// <param name="cmdText"></param>
        /// <param name="commandParameters"></param>
        private static void PrepareCommand(OracleCommand command, OracleConnection connection, CommandType cmdType, string cmdText, OracleParameter[] commandParameters)
        {
            try
            {
                if (connection.State != ConnectionState.Open) connection.Open();

                command.Connection = connection;
                command.CommandText = cmdText;
                command.CommandType = cmdType;

                //if (trans != null) command.Transaction = trans;

                if (commandParameters != null)
                {
                    foreach (OracleParameter parm in commandParameters)
                        command.Parameters.Add(parm);
                }
            }
            catch
            {

            }
        }
        #endregion

        /**  
            * 批量插入数据  
            * @tableName 表名称  
            * @columnRowData 键-值存储的批量数据：键是列名称，值是对应的数据集合  
            * @conStr 连接字符串  
            * @len 每次批处理数据的大小  
            */
        public static int BatchInsert(string tableName, Dictionary<string, object> columnRowData, int len)
        {
            if (string.IsNullOrEmpty(tableName))
            {
                throw new ArgumentException("必须指定批量插入的表名称", "tableName");
            }

            if (columnRowData == null || columnRowData.Count < 1)
            {
                throw new ArgumentException("必须指定批量插入的字段名称", "columnRowData");
            }

            int iResult = 0;
            string[] dbColumns = new string[columnRowData.Keys.Count];
            columnRowData.Keys.CopyTo(dbColumns, 0);
            StringBuilder sbCmdText = new StringBuilder();
            if (columnRowData.Count > 0)
            {
                //准备插入的SQL  
                sbCmdText.AppendFormat("INSERT INTO {0}(", tableName);
                sbCmdText.Append(string.Join(",", dbColumns));
                sbCmdText.Append(") VALUES (");
                sbCmdText.Append(":" + string.Join(",:", dbColumns));
                sbCmdText.Append(")");

                using (OracleConnection conn = new OracleConnection(GetConnectionString))
                {
                    using (OracleCommand cmd = conn.CreateCommand())
                    {
                        //绑定批处理的行数  
                        cmd.ArrayBindCount = len;
                        cmd.BindByName = true;
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = sbCmdText.ToString();
                        cmd.CommandTimeout = 600;//10分钟  

                        //创建参数  
                        OracleParameter oraParam;
                        List<IDbDataParameter> cacher = new List<IDbDataParameter>();
                        OracleDbType dbType = new OracleDbType();
                        foreach (string colName in dbColumns)
                        {
                            dbType = GetOracleDbType(columnRowData[colName]);
                            oraParam = new OracleParameter(colName, dbType);
                            oraParam.Direction = ParameterDirection.Input;
                            oraParam.OracleDbType = dbType;

                            oraParam.Value = columnRowData[colName];
                            cmd.Parameters.Add(oraParam);
                        }
                        //打开连接  
                        conn.Open();

                        /*执行批处理*/
                        //var trans = conn.BeginTransaction();
                        try
                        {
                            
                            
                            iResult = cmd.ExecuteNonQuery();
                            //trans.Commit();
                        }
                        catch (Exception ex)
                        {
                           // trans.Rollback();
                           // throw ex;
                        }
                        finally
                        {
                            if (conn != null) conn.Close();
                        }

                    }
                }
            }
            return iResult;
        }

        /**  
         * 根据数据类型获取OracleDbType  
         */
        private static OracleDbType GetOracleDbType(object value)
        {
            OracleDbType dataType = new OracleDbType();
            if (value is string[])
            {
                dataType = OracleDbType.Varchar2;
            }
            else if (value is DateTime[])
            {
                dataType = OracleDbType.TimeStamp;
            }
            else if (value is int[] || value is short[])
            {
                dataType = OracleDbType.Int32;
            }
            else if (value is long[])
            {
                dataType = OracleDbType.Int64;
            }
            else if (value is decimal[] || value is double[] || value is float[])
            {
                dataType = OracleDbType.Decimal;
            }
            else if (value is Guid[])
            {
                dataType = OracleDbType.Varchar2;
            }
            else if (value is bool[] || value is Boolean[])
            {
                dataType = OracleDbType.Byte;
            }
            else if (value is byte[])
            {
                dataType = OracleDbType.Blob;
            }
            else if (value is char[])
            {
                dataType = OracleDbType.Char;
            }
            return dataType;
        } 


    }//end of class
}//end of namespace
