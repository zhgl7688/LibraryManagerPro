using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Models;
using DAL;

namespace BLL
{
    //还书管理 
   public  class ReturnBookManager
    {
        private ReturnBookService objReturnBookService = new ReturnBookService();
         //根据借阅卡号查询借书明细
        public List<BorrowDetail> QueryBookByReadingCard(string readingCard)
        {
            return objReturnBookService.QueryBookByReadingCard(readingCard);
        }

      //还书更新
        public bool UpReturn( List<ReturnBook> returnBookList)
        {
           return objReturnBookService.UpReturn(  returnBookList);
        }
    }
}
