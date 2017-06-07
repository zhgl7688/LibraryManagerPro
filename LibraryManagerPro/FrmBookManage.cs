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

namespace LibraryManagerPro
{
    public partial class FrmBookManage : Form
    {
        //获取Bll对象
        private BookManager objBookManager = new BookManager();
        List<Book> bookList = null;
        public FrmBookManage()
        {
            InitializeComponent();
            //初始化图书分类下拉框
            List<Category> categorylist = objBookManager.GetAllCategory();
            var list = categorylist.Select(t => t);
            this.cbo_BookCategory.DataSource =list.ToList ();
            this.cbo_BookCategory.DisplayMember = "CategoryName";
            this.cbo_BookCategory.ValueMember = "CategoryId";
            this.cbo_BookCategory.SelectedIndex = -1;
            categorylist.Insert(0, new Category() { CategoryName = "", CategoryId = -1 });
            this.cboCategory.DataSource = categorylist;
            this.cboCategory.DisplayMember = "CategoryName";
            this.cboCategory.ValueMember = "CategoryId";
            this.cboCategory.SelectedIndex = -1;

            //初始化出版社分类下拉框
            this.cbo_Publisher.DataSource = objBookManager.GetAllPubisher();
            this.cbo_Publisher.DisplayMember = "PublisherName";
            this.cbo_Publisher.ValueMember = "PublisherId";
            this.cbo_Publisher.SelectedIndex = -1;
            this.dgvBookList.AutoGenerateColumns = false;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        //组合查询图书信息
        private void btnQuery_Click(object sender, EventArgs e)
        {
            //禁用选择改变事件
            this.dgvBookList.SelectionChanged -= this.dgvBookList_SelectionChanged;
            //三个条件整合（为null、空为的合为null)
            string barCode = this.txtBarCode.Text != null && this.txtBarCode.Text.Trim().Length != 0 ? this.txtBarCode.Text.Trim() : null;
            string categoryId = null;
            if (this.cboCategory.SelectedValue != null && this.cboCategory.SelectedValue.ToString() != "-1")
            {
                categoryId = this.cboCategory.SelectedValue.ToString();
            }
            string bookName = this.txtBookName.Text != null && this.txtBookName.Text.Trim().Length != 0 ? this.txtBookName.Text.Trim() : null;
            //三个条件必须有一个
            if (barCode == null && categoryId == null && bookName == null)
            {
                MessageBox.Show("请选择查询条件！");
                return;
            }
            //根据条件组合查询
            bookList = objBookManager.GetBookByMore(barCode, categoryId, bookName);
            //查询结果显示列表中
            this.dgvBookList.DataSource = bookList;
            //根据查询结果决定是否开启“删除”“修改保存”按钮
            if (bookList.Count == 0)
            {
                this.btnDel.Enabled = false;
                this.btnSave.Enabled = false;
            }
            else
            {
                this.btnDel.Enabled = true;
                this.btnSave.Enabled = true;
                //开启选择改变事件
                this.dgvBookList.SelectionChanged += this.dgvBookList_SelectionChanged;
                this.dgvBookList_SelectionChanged(null, null);
            }

        }
        //显示单个图书信息
        private void displayBook(Book objBook)
        {
            if (objBook != null)
            {
                this.txt_Author.Text = objBook.Author;
                this.txt_BookName.Text = objBook.BookName;
                this.txt_BookPosition.Text = objBook.BookPosition;
                this.txt_UnitPrice.Text = objBook.UnitPrice.ToString();
                this.lbl_BarCode.Text = objBook.BarCode;
                this.lbl_BookCount.Text = objBook.BookCount.ToString();
                this.lbl_BookId.Text = objBook.BookId.ToString();
                this.dtp_PublishDate.Text = objBook.PublishDate.ToString();
                this.cbo_BookCategory.SelectedValue = objBook.BookCategory;
                this.cbo_Publisher.SelectedValue = objBook.PublisherId;
                this.pbCurrentImage.Image = objBook.BookImage.Length != 0 ? (Image)new Common.SerializeObjectToString().DeserializeObject(objBook.BookImage) : null;

            }
        }

        //删除图书
        private void btnDel_Click(object sender, EventArgs e)
        {
            if (this.dgvBookList.RowCount > 0 && this.dgvBookList.SelectedRows.Count > 0)
            {
                //获取图书条码
                string barCode = this.dgvBookList.SelectedRows[0].Cells["BarCode"].Value.ToString();
                DialogResult result = MessageBox.Show("确认删除" + this.txt_BookName.Text + "图书信息吗？", "确认提示", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.No) return;
                try
                {
                    if (objBookManager.DeleteBook(barCode) == 1)
                    {
                        MessageBox.Show("删除成功！");
                        //刷新表格信息
                        bookList.Remove((from b in bookList where b.BarCode == barCode select b).First());
                        this.dgvBookList.DataSource = null;
                        this.dgvBookList.DataSource = bookList;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }



            }
        }
        //选择图书首页
        private void btnChoseImage_Click(object sender, EventArgs e)
        {
            OpenFileDialog opd = new OpenFileDialog();
            opd.Filter = "png图书|*.png|jpg图片|*.jpg|所有文件|*.*";
            DialogResult result = opd.ShowDialog();
            if (result == DialogResult.OK)
            {
                this.pbCurrentImage.ImageLocation = opd.FileName;
            }
        }
        //更新图书信息
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (this.lbl_BarCode.Text.Trim().Length != 0)
            {
                #region 数据验证
                if (this.txt_BookName.Text.Trim().Length == 0)//图书名不能为空
                {
                    MessageBox.Show("书名不能为空！");
                    this.txt_BookName.Focus();
                    return;
                }
                if (this.txt_Author.Text.Trim().Length == 0)//主编人不能为空
                {
                    MessageBox.Show("主编人不能为空！");
                    this.txt_Author.Focus();
                    return;
                }
                if (this.cbo_BookCategory.Text.Trim().Length == 0)//图书分类不能为空
                {
                    MessageBox.Show("图书分类不能为空！");
                    this.cbo_BookCategory.Focus();
                    return;
                }
                if (this.cbo_Publisher.Text.Trim().Length == 0)//出版社不能为空
                {
                    MessageBox.Show("出版社不能为空！");
                    this.cbo_Publisher.Focus();
                    return;
                }
                if (this.txt_UnitPrice.Text.Trim().Length == 0)//价格不能为空
                {
                    MessageBox.Show("价格不能为空！");
                    this.txt_UnitPrice.Focus();
                    return;
                }
                if (!Common.DataValidate.IsInteger(this.txt_UnitPrice.Text.Trim()))//价格为整数
                {
                    MessageBox.Show("价格不正确！");
                    this.txt_UnitPrice.Focus();
                    return;
                }
                #endregion
                var objBook = (from b in bookList where b.BarCode == this.lbl_BarCode.Text.Trim() select b).First();
                #region 数据封装
                objBook.BookName = this.txt_BookName.Text.Trim();
                objBook.Author = this.txt_Author.Text.Trim();
                objBook.PublisherId = Convert.ToInt32(this.cbo_Publisher.SelectedValue);
                objBook.PublishDate = Convert.ToDateTime(this.dtp_PublishDate.Text);
                objBook.BookCategory = Convert.ToInt32(this.cbo_BookCategory.SelectedValue);
                objBook.UnitPrice = Convert.ToDouble(this.txt_UnitPrice.Text.Trim());
                objBook.BookImage = this.pbCurrentImage.Image != null ? new Common.SerializeObjectToString().SerializeObject(this.pbCurrentImage.Image) : "";
                objBook.BookPosition = this.txt_BookPosition.Text.Trim();
                #endregion
                #region 更新图书
                try
                {
                    int result = objBookManager.ModifyBook(objBook);
                    if (result == 1)
                    {
                        MessageBox.Show("图书修改成功！", "信息提示");
                        //在列表中更新图书
                        this.dgvBookList.Refresh();
                    }
                    else MessageBox.Show("添加图书失败，错误信息：");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("添加图书失败，错误信息：" + ex.Message);
                }

                #endregion

            }
        }
        //显示单个图片信息
        private void dgvBookList_SelectionChanged(object sender, EventArgs e)
        {
            if (this.dgvBookList.RowCount > 0 && this.dgvBookList.SelectedRows.Count > 0)
            {
                //获取图书条码
                string barCode = this.dgvBookList.SelectedRows[0].Cells["BarCode"].Value.ToString();
                Book objBook = (from b in bookList where b.BarCode.Equals(barCode) select b).First();
                displayBook(objBook);
            }

        }
    }
}
