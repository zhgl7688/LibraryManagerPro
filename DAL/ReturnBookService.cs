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
    public class ReturnBookService
    {
        //根据借阅卡号查询借书明细
        public List<BorrowDetail> QueryBookByReadingCard(string readingCard)
        {
            SqlDataReader objReader = SQLHelper.GetReaderByProcedure("usp_QueryBookByReadingCard", new SqlParameter[] { new SqlParameter("@ReadingCard", readingCard) });
            List<BorrowDetail> list = new List<BorrowDetail>();
            while (objReader.Read())
            {
                list.Add(new BorrowDetail()
                {
                    BorrowDetailId = Convert.ToInt32(objReader["BorrowDetailId"]),
                    BookName = objReader["BookName"].ToString(),
                    BorrowCount = Convert.ToInt32(objReader["BorrowCount"]),
                    BorrowDate = Convert.ToDateTime(objReader["BorrowDate"]),
                    Expire = Convert.ToDateTime(objReader["Expire"]),
                    ReturnCount = Convert.ToInt32(objReader["ReturnCount"]),
                    StatusDesc = objReader["StatusDesc"].ToString(),
                    BarCode = objReader["BarCode"].ToString(),
                    NonReturnCount = Convert.ToInt32(objReader["NonReturnCount"])
                });
            }
            objReader.Close();
            return list;
        }

        //更新借书明细
        public bool  UpReturn( List<ReturnBook> returnBookList)
        {
            List<SqlParameter[]> paramArray = new List<SqlParameter[]>();
              foreach (ReturnBook item in returnBookList)
            {
               paramArray.Add ( new SqlParameter[]{
                     new SqlParameter ("@BorrowDetailId",item .BorrowDetailId ),
                      new SqlParameter ("@ReturnCount",item .ReturnCount  ),
                       new SqlParameter ("@ReturnDate",item .ReturnDate  ),
                        new SqlParameter ("@AdminName_R",item .AdminName_R  )
                 });
              } 
            return SQLHelper.UpdateByTran ("usp_ReturnBook",paramArray );

        }
    }
}
