using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    /// <summary>
    /// 借书明细
    /// </summary>
  [Serializable ]
    public  class BorrowDetail
    {
        public int BorrowDetailId { get; set; }
        public string  BorrowId { get; set; }
        public int BookId { get; set; }
        public int BorrowCount { get; set; }//借阅数
        public int ReturnCount { get; set; }
        public int NonReturnCount { get; set; }
        public DateTime Expire { get; set; }//到期时间

      //扩展
        public string  BookName { get; set; }
        public DateTime  BorrowDate { get; set; }//借书时间
        public string  StatusDesc { get; set; }//借书状态
        public string  BarCode { get; set; }//



    }
}
