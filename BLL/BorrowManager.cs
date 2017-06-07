using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Models;
using DAL;

namespace BLL
{
   public  class BorrowManager
    {
       private BorrowService objBorrowService = new BorrowService();
        //根据借书证号查询读者借阅情况
       public Reader GetReaderBorrowInfo(string readingCard)
       {
          Reader objReader= new ReaderManager ().GetReaderByReadingCard (readingCard);
          if (objReader!=null && objReader.StatusId == 1)//借阅证正常
          {
              objReader.BorrowCount = objBorrowService.GetReaderSum(readingCard);//获取借阅证总数
          }
             return objReader;
       }
       //根据图书条码查询图书信息
       public Book GetBookByBarCode(string barCode)
       {
           return new BookManager().GetBookByBarCode(barCode);
       }

       public bool  AddBorrowInfo(BorrowInfo objBorrowInfo,List<BorrowDetail > detailList)
       {
           return objBorrowService.AddBorrowInfo(objBorrowInfo ,detailList ); 
       }
    }
}
