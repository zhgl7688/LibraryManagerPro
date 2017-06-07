using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    /// <summary>
    /// 图书信息实体类
    /// </summary>
    [Serializable ]
   public  class Book
    {
        public int BookId { get; set; }
        public string  BookName { get; set; }
        public string  BarCode { get; set; }//条码
        public string  Author { get; set; }//作者
        public int PublisherId { get; set; }//出版社编号外键
        public DateTime  PublishDate { get; set; }//出版时间
        public int BookCategory { get; set; }//图片分类
        public double  UnitPrice { get; set; }
        public string  BookImage { get; set; }//图书封皮（c#程序中 图片序列化字符串）
        public int BookCount { get; set; }//图书总数
        public int Remainder { get; set; }//可借总数
        public string  BookPosition { get; set; }//书架位置
        public DateTime  RegTime { get; set; }//上架时间

        //扩展
        public string  PublisherName { get; set; }//出版社
        public string CategoryName { get; set; }//图书分类
        public DateTime Expire { get; set; }//还书时间
        public int BorrowCount { get; set; }//借阅数量
        public int ReturnCount { get; set; }//还书数量

    }
}
