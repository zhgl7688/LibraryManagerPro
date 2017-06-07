using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace DBUitily
{
    public class SQLHelper
    {
        private static string connString = ConfigurationManager.ConnectionStrings["connString"].ToString();
        #region 封装格式化SQL语句执行的各种方法
        //增删改操作
        public static int Update(string sql)
        {
            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(sql, conn);
            try
            {
                conn.Open();
                return cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                //将异常信息写入日志 
                string errorInfo = "调用Update(string sql)方法时发生错误，具体信息：" + ex.Message;
                WriteLog(errorInfo);
                throw ex;
            }
            finally
            {
                conn.Close();
            }

        }

        //获取只读数据集操作
        public static SqlDataReader GetReader(string sql)
        {
            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(sql, conn);
            try
            {
                /*1、当使用连接池以后，执行open()方法的时候，系统会从连接池中
                提取一个现有的连接对象过来，这时候打开的是一个逻辑连接
                 2、如果连接池中的连接对象都被占用了，则会创建一个新连接对象
                 */
                conn.Open();
                return cmd.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch (Exception ex)
            {
                /*
                 1、当使用Close()方法关闭连接的时候，系统会把连接对象放回
                 连接池，这时候关闭的是一个逻辑连接。
                 2、如果是独立创建的对象，则会被GC翻译掉。
                 */
                conn.Close();
                string errorInfo = "调用GetReader(string sql)方法时发生错误，具体信息：" + ex.Message;
                //将异常信息写入日志 
                WriteLog(errorInfo);
                throw new Exception(errorInfo);
            }
        }
        //获取单一结果
        public static object GetSingleResult(string sql)
        {
            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(sql, conn);
            try
            {
                conn.Open();
                return cmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
                //将异常信息写入日志 
                string errorInfo = "调用GetSingleResult(string sql)方法时发生错误，具体信息：" + ex.Message;
                WriteLog(errorInfo);
                throw new Exception(errorInfo);
            }
            finally
            {
                conn.Close();
            }
        }
        //获取DataSet数据集
        public static DataSet GetDataSet(string sql)
        {
            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(sql, conn);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            try
            {
                conn.Open();
                da.Fill(ds);
                return ds;
            }
            catch (Exception ex)
            {
                //将异常信息写入日志 
                string errorInfo = "调用 GetDataSet(string sql)方法时发生错误，具体信息：" + ex.Message;
                WriteLog(errorInfo);
                throw new Exception(errorInfo);
            }
            finally
            {
                conn.Close();
            }
        }
        #endregion
        #region 封装带参数的格式化SQL语句的各种方法
        //增删改操作
        public static int Update(string sql, SqlParameter[] param)
        {
            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(sql, conn);
            try
            {
                conn.Open();
                cmd.Parameters.AddRange(param);
                return cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                //将异常信息写入日志 
                string errorInfo = "调用Update(string sql, SqlParameter[] param)方法时发生错误，具体信息：" + ex.Message;
                WriteLog(errorInfo);
                throw ex;
            }
            finally
            {
                conn.Close();
            }

        }

        //获取只读数据集操作
        public static SqlDataReader GetReader(string sql, SqlParameter[] param)
        {
            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(sql, conn);
            try
            {
                conn.Open();
                cmd.Parameters.AddRange(param);
                return cmd.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch (Exception ex)
            {
                //将异常信息写入日志 
                conn.Close();
                string errorInfo = "调用GetReader(string sql, SqlParameter[] param)方法时发生错误，具体信息：" + ex.Message;
                WriteLog(errorInfo);
                throw new Exception(errorInfo);
            }
        }
        //获取单一结果
        public static object GetSingleResult(string sql, SqlParameter[] param)
        {
            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(sql, conn);
            try
            {
                conn.Open();
                cmd.Parameters.AddRange(param);
                return cmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
                //将异常信息写入日志 
                string errorInfo = "调用GetSingleResult(string sql, SqlParameter[] param)方法时发生错误，具体信息：" + ex.Message;
                WriteLog(errorInfo);
                throw new Exception(errorInfo);
            }
            finally
            {
                conn.Close();
            }
        }

        /// <summary>
        /// 启用事务提交多条带参数的SQL语句
        /// </summary>
        /// <param name="mainSql">主表SQL</param>
        /// <param name="mainParam">主表对应的参数</param>
        /// <param name="detailSql">明细表SQL语句</param>
        /// <param name="detailParam">明细表对应的参数</param>
        /// <returns>返回事务是否成功</returns>
        public static bool UpdateByTran(string mainSql, SqlParameter[] mainParam, string detailSql, List<SqlParameter[]> detailParam)
        {
            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            try
            {
                conn.Open();
                cmd.Transaction = conn.BeginTransaction();//开启事务
                if (mainSql != null && mainSql.Length != 0)
                {
                    cmd.CommandText = mainSql;
                    cmd.Parameters.AddRange(mainParam);
                    cmd.ExecuteNonQuery();
                }
                foreach (SqlParameter[] param in detailParam)
                {
                    cmd.CommandText = detailSql;
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddRange(param);
                    cmd.ExecuteNonQuery();
                }
                cmd.Transaction.Commit();//提交事务
                return true;
            }
            catch (Exception ex)
            {
                if (cmd.Transaction != null)
                {
                    cmd.Transaction.Rollback();//回滚事务
                }

                //将异常信息写入日志 
                string errorInfo = "调用UpdateByTran(string mainSql,  SqlParameter[] mainParam,string detailSql ,List <SqlParameter []>detailParam)方法时发生错误，具体信息：" + ex.Message;
                WriteLog(errorInfo);
                throw ex;
            }
            finally
            {
                if (cmd.Transaction != null)
                {
                    cmd.Transaction = null;//清空事务
                }
                conn.Close();
            }
        }
        #endregion
        #region 封装带参数的存储过程的各种方法
        //增删改操作
        public static int UpdateByProcedure(string spName, SqlParameter[] param)
        {
            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(spName, conn);
            try
            {
                conn.Open();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddRange(param);
                return cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                //将异常信息写入日志 
                string errorInfo = "调用UpdateByProcedure(string spName, SqlParameter[] param)方法时发生错误，具体信息：" + ex.Message;
                WriteLog(errorInfo);
                throw new Exception(errorInfo);
            }
            finally
            {
                conn.Close();
            }

        }

        //获取只读数据集操作
        public static SqlDataReader GetReaderByProcedure(string spName, SqlParameter[] param)
        {
            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(spName, conn);
            try
            {
                conn.Open();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddRange(param);
                return cmd.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch (Exception ex)
            {
                //将异常信息写入日志 
                conn.Close();
                string errorInfo = "调用GetReaderByProcedure(string spName, SqlParameter[] param)方法时发生错误，具体信息：" + ex.Message;
                WriteLog(errorInfo);
                throw new Exception(errorInfo);
            }
        }
        //获取单一结果
        public static object GetSingleResultByProcedure(string spName, SqlParameter[] param)
        {
            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(spName, conn);
            try
            {
                conn.Open();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddRange(param);
                return cmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
                //将异常信息写入日志 
                string errorInfo = "调用GetSingleResult(string sql)方法时发生错误，具体信息：" + ex.Message;
                WriteLog(errorInfo);
                throw new Exception(errorInfo);
            }
            finally
            {
                conn.Close();
            }
        }
        /// <summary>
        /// 启用事务调用带参数的存储过程
        /// </summary>
        /// <param name="spName">存储过程名称</param>
        /// <param name="detailParam">存储参数</param>
        /// <returns></returns>
        public static bool UpdateByTran(string spName, List<SqlParameter[]> Params)
        {
            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            try
            {
                conn.Open();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = spName;
                cmd.Transaction = conn.BeginTransaction();//开启事务
                foreach (SqlParameter[] param in Params)
                {
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddRange(param);
                    cmd.ExecuteNonQuery();
                }
                cmd.Transaction.Commit();//提交事务
                return true;
            }
            catch (Exception ex)
            {
                if (cmd.Transaction != null)
                {
                    cmd.Transaction.Rollback();//回滚事务
                }

                //将异常信息写入日志 
                string errorInfo = "调用UpdateByTran(string mainSql,  SqlParameter[] mainParam,string detailSql ,List <SqlParameter []>detailParam)方法时发生错误，具体信息：" + ex.Message;
                WriteLog(errorInfo);
                throw ex;
            }
            finally
            {
                if (cmd.Transaction != null)
                {
                    cmd.Transaction = null;//清空事务
                }
                conn.Close();
            }
        }

        #endregion
        #region 写入错误日志
        private static void WriteLog(string msg)
        {
            FileStream fs = null;
            if (!File.Exists("error.obj"))
            {
                try
                {
                    fs = new FileStream("error.obj", FileMode.CreateNew);
                    fs.Close();
                }
                catch (Exception ex)
                {

                    throw new Exception("创建日志文件失败" + ex.Message);
                }

            }
            fs = new FileStream("error.obj", FileMode.Append);
            StreamWriter sw = new StreamWriter(fs);
            sw.WriteLine(msg);
            sw.Close();
            fs.Close();
        }
        #endregion

    }
}
