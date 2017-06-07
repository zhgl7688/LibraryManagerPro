using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    /// <summary>
    /// 借阅主表信息
    /// </summary>
    [Serializable ]
   public  class BorrowInfo
    {
        public string  BorrowId { get; set; }
        public int ReaderId { get; set; }
        public DateTime  BorrowDate { get; set; }
        public string  AdminName_B { get; set; }

        

    }
}
