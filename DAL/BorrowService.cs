using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Models;
using DBUitily;
using System.Data;
using System.Data.SqlClient;

namespace DAL
{
    public class BorrowService
    {

        //根据借阅证查询当前读者借书总数
        public int GetReaderSum(string readingCard)
        {

            SqlParameter outBorrowCount = new SqlParameter("@BorrowCount", SqlDbType.Int);
            outBorrowCount.Direction = ParameterDirection.Output;
            SqlParameter[] param = new SqlParameter[]{
                      new SqlParameter("@ReadingCard",readingCard ),
                      outBorrowCount 
                      };
            Convert.ToInt32(SQLHelper.GetSingleResultByProcedure("usp_BorrowCount", param));
            return Convert.ToInt32(outBorrowCount.Value);
        }
        //根据图书条码查询图书信息
        //读者借书
        public bool AddBorrowInfo(BorrowInfo objBorrowInfo ,List<BorrowDetail > detailList)
        {
            //主表实现
            string sqlMain = "insert into BorrowInfo(BorrowId, ReaderId, BorrowDate, AdminName_B)values(@BorrowId, @ReaderId, @BorrowDate, @AdminName_B) ";
            SqlParameter[] param = new SqlParameter[]{
             new SqlParameter ("@BorrowId",objBorrowInfo .BorrowId ),
             new SqlParameter ("@ReaderId",objBorrowInfo .ReaderId ),
             new SqlParameter ("@BorrowDate",objBorrowInfo .BorrowDate ),
             new SqlParameter ("@AdminName_B",objBorrowInfo .AdminName_B )
         };
            //副表实现
            string sqlDetail = "insert into BorrowDetail(BorrowId, BookId, BorrowCount, NonReturnCount, Expire)values(@BorrowId, @BookId, @BorrowCount, @NonReturnCount, @Expire)";
            List<SqlParameter[]> paramList = new List<SqlParameter[]>();
            SqlParameter[] paramt = null; 
            foreach (BorrowDetail item in detailList)
            {
                paramt = new SqlParameter[]{
             new SqlParameter ("@BorrowId",item.BorrowId  ),
             new SqlParameter ("@BookId",item.BookId  ),
             new SqlParameter ("@BorrowCount",item.BorrowCount ),
             new SqlParameter ("@NonReturnCount",item.NonReturnCount ),
             new SqlParameter ("@Expire",item.Expire  )
              };
                paramList.Add(paramt);
            }
            //添加借书
            return SQLHelper.UpdateByTran(sqlMain, param, sqlDetail, paramList);
        }
        //读者还书
    }
}
