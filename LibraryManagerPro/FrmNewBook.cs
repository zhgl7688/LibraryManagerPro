using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using BLL;
using Models;
using Common;

namespace LibraryManagerPro
{
    public partial class FrmNewBook : Form
    {
        //初始化图书管理业务逻辑类
        private BookManager objBookManager = new BookManager();
        private List<Book> bookList = new List<Book>();
        public FrmNewBook()
        {
            InitializeComponent();
            this.dgvBookList.AutoGenerateColumns = false;
            this.txtAddCount.Enabled = false;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        //根据图书条码获取图书信息
        private void txtBarCode_KeyDown(object sender, KeyEventArgs e)
        {
            if (this.txtBarCode.Text.Trim().Length != 0 && e.KeyValue == 13)
            {
                Book objBook = objBookManager.GetBookByBarCode(this.txtBarCode.Text.Trim());
                if (objBook == null)
                {
                    MessageBox.Show("没有该图书信息", "提示信息");
                    this.txtBarCode.SelectAll();
                    this.txtBarCode.Focus();
                    return;
                }
                //显示图书信息
                displayBook(objBook);
                //开启新增图书数量
                this.txtAddCount.Enabled = true;
                this.txtAddCount.Focus();
                //图书列表显示
                int count = bookList.Where(b => b.BookId == objBook.BookId).Count();
                int counta = (from b in bookList where b.BookId == objBook.BookId select b).Count();
                if (count == 0)
                {
                    this.dgvBookList.DataSource = null;
                    this.bookList.Add(objBook);
                    this.dgvBookList.DataSource = bookList;
                }
            }
        }
        //显示图书信息
        private void displayBook(Book objBook)
        {
            this.pbImage.Image = (Image)new Common.SerializeObjectToString().DeserializeObject(objBook.BookImage);
            this.lblBookId.Text = objBook.BookId.ToString();
            this.lblBookName.Text = objBook.BookName;
            this.lblCategory.Text = objBook.CategoryName;
            this.lblBookCount.Text = objBook.BookCount.ToString();
            this.lblBookPosition.Text = objBook.BookPosition;
            this.pbImage.Image = objBook.BookImage.Length == 0 ? null : (Image)new SerializeObjectToString().DeserializeObject(objBook.BookImage);

        }
        //更新图书总数
        private void btnSave_Click(object sender, EventArgs e)
        {
            //数据验证
            if (this.txtAddCount.Text.Trim().Length == 0)
            {
                MessageBox.Show("请输入新增图书量", "信息提示");
                this.txtAddCount.Focus();
                this.txtAddCount.SelectAll();
                return;
            }
            if (!DataValidate.IsInteger(this.txtAddCount.Text.Trim()))
            {
                MessageBox.Show("新增总数非法，必须为整数", "错误提示");
                return;
            }
            //数据更新
            try
            {
                int addCount = Convert.ToInt32(this.txtAddCount.Text.Trim());
                //更新数据库图书总数
                int result = objBookManager.AddBookCount(this.txtBarCode.Text.Trim(), addCount);
                //更新列表中图书总数
                if (result == 1)
                {
                    var bookListTemp = bookList.Where(b => b.BarCode == this.txtBarCode.Text.Trim()).First();
                    bookListTemp.BookCount += addCount;
                    this.dgvBookList.Refresh();
                    //清空图书信息的显示
                    this.lblBookName.Text = "";
                    this.lblBookCount.Text = "";
                    this.lblBookPosition.Text = "";
                    this.lblCategory.Text = "";
                    this.lblBookId.Text = "";
                    this.pbImage.Image = null;
                    //图书条码文本框获取焦点
                    this.txtBarCode.Clear();
                    this.txtAddCount.Clear();
                    this.txtAddCount.Enabled = false;
                    this.txtBarCode.Focus();

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void txtAddCount_KeyDown(object sender, KeyEventArgs e)
        {
            if (this.txtAddCount.Text.Trim().Length != 0 && e.KeyValue == 13)
            {
                btnSave_Click(null, null);
            }
        }
        //选中列表单元格显示
        private void dgvBookList_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (this.dgvBookList.Rows.Count > 0 && this.dgvBookList.SelectedRows.Count > 0)
            {
                Book objBook = (from b in bookList where b.BarCode == this.dgvBookList.SelectedRows[0].Cells["BarCode"].Value.ToString() select b).First();
                displayBook(objBook);
                this.txtBarCode.Text = objBook.BarCode.ToString();
            }
        }
    }
}
