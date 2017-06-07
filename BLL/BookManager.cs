using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DAL;
using Models;

namespace BLL
{
  public   class BookManager
    {
      private BookService objBookService=new BookService ();
        //获取所有图书分类
      public List<Category> GetAllCategory()
      {
          return objBookService.GetAllCategory();
      }
          //获取所有出版社分类
      public List<Publisher> GetAllPubisher()
      {
          return objBookService.GetAllPubisher ();
      }
       //判断图书条码是否存在
      public bool GetExitBarCode(string barCode)
      {
          return (objBookService.GetCountByBarCode(barCode )==1);
      }
         //添加图书
      public int AddBook(Book objBook)
      {
          return objBookService.AddBook(objBook);
      }
       //根据图书条码获取图书信息
      public Book GetBookByBarCode(string barCode)
      {
          return objBookService.GetBookByBarCode(barCode);
      }
       //根据图书条码更新图书总数
      public int AddBookCount(string barCode, int bookCount)
      {
          return objBookService.AddBookCount(barCode, bookCount);
      }
       //组合图书信息查询（图书分类、条码、名称）
      public List<Book> GetBookByMore(string barCode,string  categoryId,  string bookName)
      {
             return objBookService.GetBookByMore(barCode, categoryId, bookName);
      }
       //根据图书条码BarCode删除图书
      public int DeleteBook(string barCode)
      {
          return objBookService.DeleteBook(barCode);
      }
       //更新修改图书
      public int ModifyBook(Book objBook)
      {
          return objBookService.ModifyBook(objBook);
      }
    }
}
