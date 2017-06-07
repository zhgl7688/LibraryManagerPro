using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Models;
using System.Data;
using System.Data.SqlClient;
using DBUitily;

namespace DAL
{
    public class ReaderService
    {
        //会员办证
        public int AddReader(Reader objReader)
        {
            //参数封装
            SqlParameter[] param = new SqlParameter[]{
               new SqlParameter ("@ReaderName",objReader.ReaderName  ),
               new SqlParameter ("@ReadingCard",objReader.ReadingCard  ),
               new SqlParameter ("@Gender",objReader.Gender  ),
               new SqlParameter ("@RoleId",objReader.RoleId  ),
               new SqlParameter ("@PostCode",objReader.PostCode  ),
               new SqlParameter ("@ReaderAddress",objReader.ReaderAddress  ),
               new SqlParameter ("@PhoneNumber",objReader.PhoneNumber  ),
               new SqlParameter ("@ReaderImage",objReader.ReaderImage  ),
               new SqlParameter ("@IDCard",objReader.IDCard  ),
              
           };
            //带参数SQL语句
            string sql = @"insert into Readers (IDCard,ReadingCard, ReaderName, Gender,ReaderAddress, PostCode, PhoneNumber, RoleId, ReaderImage) 
           values(@IDCard,@ReadingCard, @ReaderName, @Gender,@ReaderAddress, @PostCode, @PhoneNumber, @RoleId, @ReaderImage)";
            try
            {
                return SQLHelper.Update(sql, param);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        //会员修改(根据会员Id)
        public int ModifyReader(Reader objReader)
        {
            //参数封装
            SqlParameter[] param = new SqlParameter[]{
               new SqlParameter ("@ReaderName",objReader.ReaderName  ),
                new SqlParameter ("@Gender",objReader.Gender  ),
               new SqlParameter ("@RoleId",objReader.RoleId  ),
               new SqlParameter ("@PostCode",objReader.PostCode  ),
               new SqlParameter ("@ReaderAddress",objReader.ReaderAddress  ),
               new SqlParameter ("@PhoneNumber",objReader.PhoneNumber  ),
               new SqlParameter ("@ReaderImage",objReader.ReaderImage  ),
                new SqlParameter ("@ReaderId",objReader.ReaderId   ),
                  new SqlParameter ("@IDCard",objReader.IDCard  ),
              
           };
            //带参数SQL语句
            string sql = @"update Readers set IDCard=@IDCard, ReaderName=@ReaderName, Gender=@Gender,ReaderAddress=@ReaderAddress, PostCode=@PostCode, PhoneNumber=@PhoneNumber, RoleId=@RoleId, ReaderImage=@ReaderImage where ReaderId=@ReaderId";
            try
            {
                return SQLHelper.Update(sql, param);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        //挂失借阅证
        public int ForbiddenReaderCard(string readerId)
        {
            //带参数SQL语句
            string sql = @"update  Readers  set StatusId=0 where ReaderId=@ReaderId";
            //参数封装
            SqlParameter[] param = new SqlParameter[]{
                 new SqlParameter ("@ReaderId",readerId  ),
              
           };
            try
            {
                return SQLHelper.Update(sql, param);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        //会员全部角色查询
        public DataTable GetAllReaderRole()
        {
            string sql = "select RoleId ,RoleName from ReaderRoles";
            return SQLHelper.GetDataSet(sql).Tables[0];
        }
        //根据身份证查询会员信息
        public Reader GetReaderByIDCard(string iDCard)
        {
            //参数封装
            SqlParameter[] param = new SqlParameter[]{
                 new SqlParameter ("@IDCard",iDCard  ),
            };
            return GetReaderBySQL(" and  IDCard=@IDCard", param);
        }
        //根据借阅证编号查询会员信息
        public Reader GetReaderByReadingCard(string ReadingCard)
        {
            //参数封装
            SqlParameter[] param = new SqlParameter[]{
                 new SqlParameter ("@ReadingCard",ReadingCard  ),
            };
            return GetReaderBySQL(" and  ReadingCard=@ReadingCard", param);
        }

        public Reader GetReaderBySQL(string addSQL, SqlParameter[] param)
        {
            string sql = @"select AllowDay, AllowCounts,StatusId, ReaderId, IDCard,  ReadingCard, ReaderName, Gender,ReaderAddress, PostCode, PhoneNumber, Readers.RoleId, ReaderImage,RoleName 
               from Readers inner join ReaderRoles on ReaderRoles.RoleId=Readers.RoleId  ";
            sql += addSQL;
            Reader currentReader = null;
            SqlDataReader objReader = SQLHelper.GetReader(sql, param);
            if (objReader.Read())
            {
                currentReader = new Reader()
                {
                     ReaderId =Convert.ToInt32  (objReader ["ReaderId"]),
                     IDCard =objReader ["IDCard"].ToString (),
                    ReaderName = objReader["ReaderName"].ToString(),
                    ReadingCard = objReader["ReadingCard"].ToString(),
                    Gender = objReader["Gender"].ToString(),
                    ReaderAddress = objReader["ReaderAddress"].ToString(),
                    PostCode = objReader["PostCode"].ToString(),
                    PhoneNumber = objReader["PhoneNumber"].ToString(),
                    RoleId = Convert.ToInt32(objReader["RoleId"]),
                    ReaderImage = objReader["ReaderImage"].ToString(),
                    RoleName = objReader["RoleName"].ToString(),
                     StatusId = Convert.ToInt32(objReader["StatusId"]),
                   AllowCounts =Convert .ToInt32( objReader["AllowCounts"]),
                     AllowDay = Convert.ToInt32(objReader["AllowDay"]),
                
                };
            }
            objReader.Close();
            return currentReader;
        }

        //根据会员角色查询，并返回对应会员数：
        public List<Reader> GetReaderByRoleId(string roleId, out int readerCount)
        {
            string sql = "select PostCode, RoleId, ReaderImage, IDCard, ReaderId, ReadingCard, ReaderName, Gender,ReaderAddress, PhoneNumber,RegTime,statusId from Readers ";
            sql += "where RoleId=@RoleId;select count(*) as ReaderCount from readers where RoleId=@RoleId ";
            //参数封装
            SqlParameter[] param = new SqlParameter[]{
                 new SqlParameter ("@RoleId",roleId  )
            };
            List<Reader> list = new List<Reader>();
            SqlDataReader objReader = SQLHelper.GetReader(sql, param);
            while (objReader.Read())
            {
                list.Add(new Reader()
                {
                    PostCode = objReader["PostCode"].ToString(),
                    RoleId = Convert.ToInt32(objReader["RoleId"]),
                    IDCard =objReader ["IDCard"].ToString (),
                    ReaderId = Convert.ToInt32(objReader["ReaderId"]),
                    ReadingCard = objReader["ReadingCard"].ToString(),
                    ReaderName = objReader["ReaderName"].ToString(),
                    Gender = objReader["Gender"].ToString(),
                    PhoneNumber = objReader["PhoneNumber"].ToString(),
                    RegTime = Convert.ToDateTime(objReader["RegTime"]),
                    StatusId = Convert.ToInt32(objReader["StatusId"]),
                    ReaderAddress = objReader["ReaderAddress"].ToString(),
                    ReaderImage = objReader["ReaderImage"].ToString(),
                });
            }
            readerCount = 0;//第二个返回值 
            if (objReader.NextResult() && objReader.Read())
            {
                readerCount = Convert.ToInt32(objReader["ReaderCount"]);
            }
            objReader.Close();
            return list;
        }
        //查询借阅证是否存在
        public bool ExitReaderCard(string ReadingCard)
        {
            //参数封装
            SqlParameter[] param = new SqlParameter[]{
                 new SqlParameter ("@ReadingCard",ReadingCard  ),
            };
            string sql = "select count(*) from readers where ReadingCard=@ReadingCard";
            return (Convert.ToInt32(SQLHelper.GetSingleResult(sql, param)) == 1);
        }
        //查询身份证是否存在
        public bool ExitIDCard(string IDCard)
        {
            //参数封装
            SqlParameter[] param = new SqlParameter[]{
                 new SqlParameter ("@IDCard",IDCard  ),
            };
            string sql = "select count(*) from readers where IDCard=@IDCard";
            return (Convert.ToInt32(SQLHelper.GetSingleResult(sql, param)) == 1);
        }
        //查询除本身外身份证是否存在
        public bool ExitIDCard(string IDCard, int readerId)
        {
            //参数封装
            SqlParameter[] param = new SqlParameter[]{
                 new SqlParameter ("@IDCard",IDCard  ),
                 new SqlParameter ("@ReaderId",readerId )
            };
            string sql = "select count(*) from readers where IDCard=@IDCard and ReaderId<>@ReaderId";
            return (Convert.ToInt32(SQLHelper.GetSingleResult(sql, param)) == 1);
        }
    }
}
