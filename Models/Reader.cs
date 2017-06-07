using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    /// <summary>
    /// 读者
    /// </summary>
    [Serializable]
    public class Reader
    {
        public int ReaderId { get; set; }
        public string ReadingCard { get; set; }
        public string ReaderName { get; set; }
        public string Gender { get; set; }
        public string IDCard { get; set; }
        public string ReaderAddress { get; set; }
        public string PostCode { get; set; }
        public string PhoneNumber { get; set; }
        public int RoleId { get; set; }
        public string ReaderImage { get; set; }
        public DateTime RegTime { get; set; }
        public string ReaderPwd { get; set; }
        public int AdminId { get; set; }
        public int StatusId { get; set; }//会员状态
        //扩展
        public string RoleName { get; set; }//会员角色
        public string StatuesDesc { get; set; }//会员状态描述

        public int AllowCounts { get; set; }//可借总数
        public int BorrowCount { get; set; }//已借总数
        public int AllowDay { get; set; }//可借天数

    }
}
