using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Models;
using DAL;

namespace BLL
{
   public  class SysAdminManager
    {
       //创建数据访问对象
       private SysAdminService objSysAdminSerivce = new SysAdminService();
       //根据用户ID和密码获取用户信息
       public SysAdmin AdminLogin(SysAdmin objAdmin)
       {
           return objSysAdminSerivce.Login(objAdmin);
       }
       //修改密码
       public void ModifyPwd(SysAdmin objAdmin)
       {
           objSysAdminSerivce.ModifyPwd(objAdmin);
       }
       
    }
}
