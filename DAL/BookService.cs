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
    public class BookService
    {
        //获取所有图书分类
        public List<Category> GetAllCategory()
        {
            string sql = "select  CategoryId , CategoryName from Categories";
            List<Category> list = new List<Category>();
            try
            {
                //获取所有图书分类
                SqlDataReader objReader = SQLHelper.GetReader(sql);
                //封装所有图书分类
                while (objReader.Read())
                {
                    list.Add(new Category()
                        {
                            CategoryId = Convert.ToInt32(objReader["CategoryId"]),
                            CategoryName = objReader["CategoryName"].ToString()
                        });
                }
                objReader.Close();
                return list;
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }
        //获取所有出版社分类
        public List<Publisher> GetAllPubisher()
        {
            string sql = "select  PublisherId ,PublisherName from Publishers";
            List<Publisher> list = new List<Publisher>();
            try
            {
                //获取所有图书分类
                SqlDataReader objReader = SQLHelper.GetReader(sql);
                //封装所有图书分类
                while (objReader.Read())
                {
                    list.Add(new Publisher()
                    {
                        PublisherId = Convert.ToInt32(objReader["PublisherId"]),
                        PublisherName = objReader["PublisherName"].ToString()
                    });
                }
                objReader.Close();
                return list;
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }
        //根据图书条码获取个数
        public int GetCountByBarCode(string barCode)
        {
            string sql = "select count(*) from books where BarCode=@BarCode";
            SqlParameter[] param = new SqlParameter[]
            {
                new SqlParameter ("@BarCode",barCode )
            };
            try
            {
                return Convert.ToInt32(SQLHelper.GetSingleResult(sql, param));
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }
        //添加图书
        public int AddBook(Book objBook)
        {
            SqlParameter[] param = new SqlParameter[]
            {
                 new SqlParameter ("@BookName",objBook.BookName  ),
                  new SqlParameter ("@BarCode",objBook.BarCode  ),
                   new SqlParameter ("@Author",objBook.Author  ),
                    new SqlParameter ("@PublisherId",objBook.PublisherId  ),
                     new SqlParameter ("@PublishDate",objBook.PublishDate  ),
                      new SqlParameter ("@BookCategory",objBook.BookCategory  ),
                      new SqlParameter ("@UnitPrice",objBook.UnitPrice  ),
                      new SqlParameter ("@BookImage",objBook.BookImage  ),
                      new SqlParameter ("@BookCount",objBook.BookCount  ),
                     new SqlParameter ("@Remainder",objBook.Remainder  ),
                      new SqlParameter ("@BookPosition",objBook.BookPosition  )
            };
            try
            {
                return SQLHelper.UpdateByProcedure("usp_AddBook", param);

            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }
        //根据图书条码获取图书信息
        public Book GetBookByBarCode(string barCode)
        {
            SqlParameter[] param = new SqlParameter[]{
              new SqlParameter ("@BarCode",barCode )
          };
            Book objBook = null;
            try
            {
                SqlDataReader objReader = SQLHelper.GetReaderByProcedure("usp_GetBookByBarCode", param);
                if (objReader.Read())
                {
                    objBook = new Book()
                    {
                        Author = objReader["Author"].ToString(),
                        BarCode = objReader["BarCode"].ToString(),
                        BookCategory = Convert.ToInt32(objReader["BookCategory"]),
                        BookCount = Convert.ToInt32(objReader["BookCount"]),
                        BookId = Convert.ToInt32(objReader["BookId"]),
                        BookImage = objReader["BookImage"].ToString(),
                        BookName = objReader["BookName"].ToString(),
                        BookPosition = objReader["BookPosition"].ToString(),
                        PublishDate = Convert.ToDateTime(objReader["PublishDate"]),
                        PublisherId = Convert.ToInt32(objReader["PublisherId"]),
                        PublisherName = objReader["PublisherName"].ToString(),
                        RegTime = Convert.ToDateTime(objReader["RegTime"]),
                        Remainder = Convert.ToInt32(objReader["Remainder"]),
                        UnitPrice = Convert.ToDouble(objReader["UnitPrice"]),
                        CategoryName = objReader["CategoryName"].ToString()
                    };
                }
                objReader.Close();
                return objBook;
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }

        }
        //根据图书条码更新图书总数
        public int AddBookCount(string barCode, int bookCount)
        {
            string sql = "update Books set BookCount=BookCount+@BookCount where BarCode=@BarCode";
            SqlParameter[] param = new SqlParameter[]{
                new SqlParameter ("@BarCode",barCode ),
                new SqlParameter ("@BookCount",bookCount )
            };
            return SQLHelper.Update(sql, param);
        }
        //组合图书信息查询（图书分类、条码、名称）
        public List<Book> GetBookByMore( string barCode, string categoryId,string bookName)
        {
            //定义参数集合
            SqlParameter[] param = new SqlParameter[]{
               new SqlParameter("@BarCode", barCode) ,
                new SqlParameter("@BookCategory", categoryId),
                new SqlParameter("@BookName", bookName)
            };
            #region 带参数sql写法
            //string sql = "select CategoryName, BookId,BookName,BarCode,Author,PublisherId,PublishDate,BookCategory,UnitPrice,BookImage,BookCount,Remainder,BookPosition,RegTime from Books ";
            //sql += "inner join Categories on Books.BookCategory=Categories.BooksCategory ";
            //sql += "inner join Publishers on Publishers.PublisherId=Books.PublisherId ";
            //sql += "where 1=1 ";
            //if (barCode != null && barCode.Length > 0)
            //{
            //    sql += "and BarCode=@BarCode ";
            //    paramList.Add(new SqlParameter("@BarCode", barCode));
            //}
            //else 
            //{
            //    if (categoryId !=-1)
            //    {
            //        sql += "and CategoryId=@CategoryId";
            //        paramList.Add(new SqlParameter("@CategoryId", categoryId));
            //    }
            //    if(bookName !=null && bookName .Length >0)
            //    {
            //        sql += "and BookName like '%'+@BookName+'%'";
            //        paramList.Add(new SqlParameter("@BookName", bookName));
            //    }
            //}
            //SqlDataReader objReader = SQLHelper.GetReader(sql, param);
            # endregion
            List<Book> list = new List<Book>();
            SqlDataReader objReader = SQLHelper.GetReaderByProcedure("usp_QeryBookByMore", param);
            while (objReader.Read())
            {
                list.Add(new Book()
                {
                    Author = objReader["Author"].ToString(),
                    BarCode = objReader["BarCode"].ToString(),
                    BookCategory = Convert.ToInt32(objReader["BookCategory"]),
                    BookCount = Convert.ToInt32(objReader["BookCount"]),
                    BookId = Convert.ToInt32(objReader["BookId"]),
                    BookImage = objReader["BookImage"].ToString(),
                    BookName = objReader["BookName"].ToString(),
                    BookPosition = objReader["BookPosition"].ToString(),
                    PublishDate = Convert.ToDateTime(objReader["PublishDate"]),
                    PublisherId = Convert.ToInt32(objReader["PublisherId"]),
                    PublisherName = objReader["PublisherName"].ToString(),
                    RegTime = Convert.ToDateTime(objReader["RegTime"]),
                    Remainder = Convert.ToInt32(objReader["Remainder"]),
                    UnitPrice = Convert.ToDouble(objReader["UnitPrice"]),
                    CategoryName = objReader["CategoryName"].ToString()
                });
            }
            objReader.Close();
            return list;
        }
        //根据图书条码BarCode删除图书
        public int DeleteBook(string barCode)
        {
            string sql = "delete from books where barcode=" + barCode;
            try
            {

            }
            catch (SqlException ex)
            {
                if (ex.Number == 547)
                    throw new Exception("当前图片已经被其他图书引用，不能删除");
                else
                    throw new Exception("数据库访问异常" + ex.Message);
            }
            catch (Exception ex)
            {
                
                throw ex ;
            }
            return SQLHelper.Update(sql);
        }
        //更新修改图书
        public int ModifyBook(Book objBook)
        {
            SqlParameter[] param = new SqlParameter[]
            {
                 new SqlParameter ("@BookName",objBook.BookName  ),
                  new SqlParameter ("@BookId",objBook.BookId   ),
                   new SqlParameter ("@Author",objBook.Author  ),
                    new SqlParameter ("@PublisherId",objBook.PublisherId  ),
                     new SqlParameter ("@PublishDate",objBook.PublishDate  ),
                      new SqlParameter ("@BookCategory",objBook.BookCategory  ),
                      new SqlParameter ("@UnitPrice",objBook.UnitPrice  ),
                      new SqlParameter ("@BookImage",objBook.BookImage  ),
                      new SqlParameter ("@BookCount",objBook.BookCount  ),
                     //new SqlParameter ("@Remainder",objBook.Remainder  ),
                      new SqlParameter ("@BookPosition",objBook.BookPosition  )
            };
            try
            {
                return SQLHelper.UpdateByProcedure("usp_ModifyBook", param);

            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }
    }
}
