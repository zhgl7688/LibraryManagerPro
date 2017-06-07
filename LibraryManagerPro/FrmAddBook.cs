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
using MyVideo;
using Common;

namespace LibraryManagerPro
{
    public partial class FrmAddBook : Form
    {
        //获取Bll对象
        private BookManager objBookManager = new BookManager();
        private Common.CVido objMyvideo = null;
        //添加成功关联窗体
        List<Book> bookList = new List<Book>();
        public FrmAddBook()
        {
            InitializeComponent();
            //初始化图书分类下拉框
            this.cboBookCategory.DataSource = objBookManager.GetAllCategory();
            this.cboBookCategory.DisplayMember = "CategoryName";
            this.cboBookCategory.ValueMember = "CategoryId";
            this.cboBookCategory.SelectedIndex = -1;
            //初始化出版商分类下拉框
            this.cboPublisher.DataSource = objBookManager.GetAllPubisher();
            this.cboPublisher.DisplayMember = "PublisherName";
            this.cboPublisher.ValueMember = "PublisherId";
            this.cboPublisher.SelectedIndex = -1;
            //禁用摄像并没有操作相关类
            this.btnCloseVideo.Enabled = false;
            this.btnTake.Enabled = false;
            //关闭自动关联
            this.dgvBookList.AutoGenerateColumns = false;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #region 图书封面
        //选择图书的封面图片
        private void btnChoseImage_Click(object sender, EventArgs e)
        {
            OpenFileDialog objOpenFileDialog = new OpenFileDialog();
            objOpenFileDialog.Filter = "gif files (*.gif)|*.gif|All files (*.*)|*.*";
            if (objOpenFileDialog.ShowDialog() == DialogResult.OK)
            {
                this.pbCurrentImage.ImageLocation = objOpenFileDialog.FileName;
                //   this.pbCurrentImage.Image = Image.FromFile(objOpenFileDialog.FileName);
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            this.pbCurrentImage.ImageLocation = "";

        }

        private void btnStartVideo_Click(object sender, EventArgs e)
        {
            try
            {
                //创建摄像头操作类
               objMyvideo = new CVido(this.pbImage.Handle, this.pbImage.Left, this.pbImage.Top, this.pbImage.Width, this.pbImage.Height);
               // objMyvideo = new Video(this.pbImage.Handle, this.pbImage.Left, this.pbImage.Top, this.pbImage.Width, (short)this.pbImage.Height);
                //打开摄像头
                objMyvideo.StartVideo();
               // objMyvideo.OpenVideo();
                //同进禁用和打开相关按钮
                this.btnStartVideo.Enabled = false;
                this.btnCloseVideo.Enabled = true;
                this.btnTake.Enabled = true;
                this.btnCloseVideo.BackColor = Color.Red;
                this.btnTake.ForeColor = Color.White;
            }
            catch (Exception ex)
            {
                MessageBox.Show("启动摄像头失败，具体信息：" + ex.Message);
            }
        }

        private void btnCloseVideo_Click(object sender, EventArgs e)
        {
            objMyvideo.StopVideo();
            //同进禁用和打开相关按钮
            this.btnStartVideo.Enabled = true;
            this.btnCloseVideo.Enabled = false;
            this.btnTake.Enabled = false;

            this.btnStartVideo.BackColor = Color.Green;
            this.btnStartVideo.ForeColor = Color.White;
            this.btnCloseVideo.ForeColor = Color.White;
            this.btnCloseVideo.BackColor = Color.Gray;
            this.btnTake.ForeColor = Color.YellowGreen;
            this.btnTake.BackColor = Color.Gray;
        }

        private void btnTake_Click(object sender, EventArgs e)
        {
            this.pbCurrentImage.Image = null;// objMyvideo.Images("1.jpg");
        }
        #endregion
        //添加图书
        private void btnAdd_Click(object sender, EventArgs e)
        {

            #region 数据验证
            if (this.txtBookName.Text.Trim().Length == 0)//图书名不能为空
            {
                MessageBox.Show("书名不能为空！");
                this.txtBookName.Focus();
                return;
            }
            if (this.txtBarCode.Text.Trim().Length == 0)//条码不能为空
            {
                MessageBox.Show("条码不能为空！");
                this.txtBarCode.Focus();
                return;
            }
            if (this.txtAuthor.Text.Trim().Length == 0)//主编人不能为空
            {
                MessageBox.Show("主编人不能为空！");
                this.txtAuthor.Focus();
                return;
            }
            if (this.cboBookCategory.Text.Trim().Length == 0)//图书分类不能为空
            {
                MessageBox.Show("图书分类不能为空！");
                this.cboBookCategory.Focus();
                return;
            }
            if (this.cboPublisher.Text.Trim().Length == 0)//出版社不能为空
            {
                MessageBox.Show("出版社不能为空！");
                this.cboPublisher.Focus();
                return;
            }
            if (this.txtUnitPrice.Text.Trim().Length == 0)//价格不能为空
            {
                MessageBox.Show("价格不能为空！");
                this.txtUnitPrice.Focus();
                return;
            }
            if (!Common.DataValidate.IsInteger(this.txtUnitPrice.Text.Trim()))//价格为整数
            {
                MessageBox.Show("价格不正确！");
                this.txtUnitPrice.Focus();
                return;
            }
            #endregion
            #region 数据封装
            Book objBook = new Book()
            {
                BookName = this.txtBookName.Text.Trim(),
                BarCode = this.txtBarCode.Text.Trim(),
                Author = this.txtAuthor.Text.Trim(),
                PublisherId = Convert.ToInt32(this.cboPublisher.SelectedValue),
                PublishDate = Convert.ToDateTime(this.dtpPublishDate.Text),
                BookCategory = Convert.ToInt32(this.cboBookCategory.SelectedValue),
                UnitPrice = Convert.ToDouble(this.txtUnitPrice.Text.Trim()),
                BookImage = this.pbCurrentImage.Image != null ? new SerializeObjectToString().SerializeObject(this.pbCurrentImage.Image) : "",
                BookCount = this.txtBookCount.Text.Trim().Length != 0 ? Convert.ToInt32(this.txtBookCount.Text.Trim()) : 0,
                Remainder = this.txtBookCount.Text.Trim().Length != 0 ? Convert.ToInt32(this.txtBookCount.Text.Trim()) : 0,
                BookPosition = this.txtBookPosition.Text.Trim(),
                PublisherName = this.cboPublisher.Text
            };
            #endregion
            #region 添加图书
            try
            {
                objBookManager.AddBook(objBook);
                      //在列表中更新图书
                    bookList.Add(objBook);
                    this.dgvBookList.DataSource = null;
                    this.dgvBookList.DataSource = bookList;
                    //清空输入文本框，等用户输入新的内容
                    foreach (Control item in this.gbBook.Controls)
                    {
                        if (item is TextBox) item.Text = "";
                        else if (item is ComboBox )
                        {
                            ((ComboBox)item).SelectedIndex = -1;
                        }
                     
                    }
                    pbCurrentImage.Image = null ;
 
            }
            catch (Exception ex)
            {
                MessageBox.Show("添加图书失败，错误信息：" + ex.Message);
            }

            #endregion
        }

        //判断条形码是否存在
        private void txtBarCode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 13)
            {
                txtBarCode_Leave(null, null);
            }
        }
        //判断条形码是否存在
        private void txtBarCode_Leave(object sender, EventArgs e)
        {
            if (this.txtBarCode.Text.Trim().Length > 0 && objBookManager.GetExitBarCode(this.txtBarCode.Text.Trim()))
            {
                MessageBox.Show("图书条码已存在！", "提示信息");
                this.txtBarCode.Focus();
                this.txtBarCode.SelectAll();
                return;

            }

        }
        //判断收藏数为数值
        private void txtBookCount_TextChanged(object sender, EventArgs e)
        {
            if (!DataValidate.IsInteger(this.txtBookCount.Text.Trim()))
            {
                this.txtBookCount.Text = "";
            }
        }
        //判断图书条码为数值
        private void txtBarCode_TextChanged(object sender, EventArgs e)
        {
            if (!DataValidate.IsInteger(this.txtBarCode.Text.Trim()))
            {
                this.txtBarCode.Text = "";
            }
        }
        //添加行号
        private void dgvBookList_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            new DataGridViewStyle().DgvStyle1(this.dgvBookList);
            DataGridViewStyle.DgvRowPostPaint(this.dgvBookList, e);
        }
    }
}
