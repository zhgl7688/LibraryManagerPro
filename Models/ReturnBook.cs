using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    /// <summary>
    /// 还书信息
    /// </summary>
    [Serializable ]
   public  class ReturnBook
    {
        public int ReturnId { get; set; }
        public int BorrowDetailId { get; set; }
        public int ReturnCount { get; set; }
        public DateTime  ReturnDate { get; set; }//还书时间
        public string  AdminName_R { get; set; }
        //扩展
        public string  BookName { get; set; }
        public string BarCode { get; set; }


    }
}
