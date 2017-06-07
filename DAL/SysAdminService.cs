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
   public  class SysAdminService
    {
       /// <summary>
       /// 根据登录Id和登录密码获取登录信息
       /// </summary>
       /// <param name="objAdmin"></param>
       /// <returns></returns>
       public SysAdmin Login(SysAdmin objAdmin)
       {
           //定义sql语句和封装参数
           string sql = "select AdminName,StatusId,RoleId from SysAdmins where AdminId=@AdminId and LoginPwd=@LoginPwd";
           SqlParameter[] param = new SqlParameter[]
           {
               new SqlParameter ("@AdminId",objAdmin .AdminId ),
               new SqlParameter ("@LoginPwd",objAdmin .LoginPwd )
           };
           //执行查询
           SqlDataReader objReader = SQLHelper.GetReader(sql, param);
           if (objReader .Read ())
           {
               objAdmin.AdminName = objReader["AdminName"].ToString();
               objAdmin.StatusId = Convert.ToInt32(objReader["StatusId"]);
               objAdmin.RoleId = Convert.ToInt32(objReader["RoleId"]);
           }
           else
           {
               objAdmin = null;
           }
           objReader .Close ();
           return objAdmin;
       }
       /// <summary>
       /// 修改密码
       /// </summary>
       /// <param name="objAdmin"></param>
       public void  ModifyPwd(SysAdmin objAdmin)
       {
           //定义sql语句和封装参数
           string sql = "update SysAdmins set LoginPwd=@LoginPwd  where AdminId=@AdminId ";
           SqlParameter[] param = new SqlParameter[]
           {
               new SqlParameter ("@AdminId",objAdmin .AdminId ),
               new SqlParameter ("@LoginPwd",objAdmin .LoginPwd )
           };
           //执行查询
           SQLHelper.Update(sql, param);
           return  ;
       }
    }
}
