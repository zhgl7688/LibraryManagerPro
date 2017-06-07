using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Models;
using BLL;

namespace LibraryManagerPro
{
    public partial class FrmBorrowBook : Form
    {//初始化逻辑访问对象
        private BorrowManager objBorrowManger = new BorrowManager();
        private BorrowInfo objBorrowInfo = new BorrowInfo();
        private List<Book> bookList = new List<Book>();
        Reader objReader = null;
        public FrmBorrowBook()
        {
            InitializeComponent();
            this.dgvBookList.AutoGenerateColumns = false;
            //禁用借书
            this.txtBarCode.Enabled = false;
            this.btnSave.Enabled = false;
            this.btnDel.Enabled = false;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        //根据借书证号查询借书信息
        private void txtReadingCard_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 13 && this.txtReadingCard.Text.Trim().Length != 0)
            {
                objReader = objBorrowManger.GetReaderBorrowInfo(this.txtReadingCard.Text.Trim());
                if (objReader != null)
                {
                    if (objReader.StatusId == 1)
                    {
                        //显示借书信息
                        this.lblReaderName.Text = objReader.ReaderName;
                        this.lblRoleName.Text = objReader.RoleName;
                        this.lblAllowCounts.Text = objReader.AllowCounts.ToString();
                        this.lblBorrowCount.Text = objReader.BorrowCount.ToString();
                        this.pbReaderImage.Image = objReader.ReaderImage.Length != 0 ? (Image)new Common.SerializeObjectToString().DeserializeObject(objReader.ReaderImage) : null;
                        int remainder = objReader.AllowCounts - objReader.BorrowCount;//剩借总数
                        this.lbl_Remainder.Text = remainder.ToString();
                        if (remainder > 0)
                        {
                            //启用借书
                            this.txtBarCode.Enabled = true;
                            this.btnSave.Enabled = true;
                            this.btnDel.Enabled = true;
                            return;
                        }
                        else
                        {
                            MessageBox.Show("该证借书到到上限！", "提示信息");
                        }
                    }
                    else
                    {
                        MessageBox.Show("该证已挂失，请与管理员联系！", "提示信息");
                    }
                }
                else
                {
                    MessageBox.Show("该证不存在，请核实！", "提示信息");
                }
                //清空借书信息
                this.lblReaderName.Text = "";
                this.lblRoleName.Text = "";
                this.lblAllowCounts.Text = "0";
                this.lblBorrowCount.Text = "0";
                this.pbReaderImage.Image = null;
                this.lbl_Remainder.Text = "0";
                this.txtReadingCard.Text = "";
                this.dgvBookList.DataSource = null;
                this.objReader = null;
                //禁用借书
                this.txtBarCode.Enabled = false;
                this.btnSave.Enabled = false;
                this.btnDel.Enabled = false;
            }
        }
        //添加借阅图书信息
        private void txtBarCode_KeyDown(object sender, KeyEventArgs e)
        {
            //借书总数是否超
            if (e.KeyValue == 13 && this.txtBarCode.Text.Trim().Length != 0)
            {

                Book objBook = null;
                //在本地查询是否有该书
                if (bookList.Count > 0)
                {
                    int BorrowCount = (bookList.Select(b => b.BorrowCount)).Sum();
                    if (Convert.ToInt32(this.lbl_Remainder.Text.Trim()) == 0)
                    {
                        MessageBox.Show("借书超上限了", "信息提示");
                        return;
                    }
                    //LinQ查询booklist集合中是否有该书
                    objBook = bookList.FirstOrDefault<Book>(b => b.BarCode == this.txtBarCode.Text.Trim());
                    if (objBook != null)
                    {
                        objBook.BorrowCount += 1;
                        this.lblBorrowCount.Text = (Convert.ToInt32(this.lblBorrowCount.Text.Trim()) + 1).ToString();
                        this.lbl_Remainder.Text = (Convert.ToInt32(this.lbl_Remainder.Text.Trim()) - 1).ToString();
                        //刷新显示
                        this.dgvBookList.DataSource = null;
                        this.dgvBookList.DataSource = bookList;
                        this.dgvBookList.Refresh();
                        this.txtBarCode.Text = "";
                        return;
                    }
                }
                //从数据库中查询
                objBook = new BookManager().GetBookByBarCode(this.txtBarCode.Text.Trim());
                //添加借书总类
                if (objBook != null)
                {
                    objBook.BorrowCount = 1;
                    objBook.Expire = Convert.ToDateTime(DateTime.Now.AddDays(objReader.AllowDay));
                    //添加到列表中
                    bookList.Add(objBook);
                    this.lblBorrowCount.Text = (Convert.ToInt32(this.lblBorrowCount.Text.Trim()) + 1).ToString();
                    this.lbl_Remainder.Text = (Convert.ToInt32(this.lbl_Remainder.Text.Trim()) - 1).ToString();
                    //刷新显示
                    this.dgvBookList.DataSource = null;
                    this.dgvBookList.DataSource = bookList;
                    this.dgvBookList.Refresh();
                }
                else
                {
                    MessageBox.Show("没有该图书！", "信息提示");
                }
                this.txtBarCode.Text = "";

            }
        }
        //删除借书信息
        private void btnDel_Click(object sender, EventArgs e)
        {
            if (this.dgvBookList.RowCount > 0)
            {
                if (this.dgvBookList.SelectedRows.Count > 0)
                {
                    Book objBook = bookList.FirstOrDefault<Book>(b => b.BarCode == this.dgvBookList.SelectedRows[0].Cells["BarCode"].Value.ToString());
                    if (objBook.BorrowCount > 1)
                    { objBook.BorrowCount -= 1; }
                    else
                    {
                        bookList.Remove(objBook);
                        this.btnDel.Enabled = false;
                    }
                    this.lblBorrowCount.Text = (Convert.ToInt32(this.lblBorrowCount.Text.Trim()) - 1).ToString();
                    this.lbl_Remainder.Text = (Convert.ToInt32(this.lbl_Remainder.Text.Trim()) + 1).ToString();
                    this.dgvBookList.DataSource = null;
                    this.dgvBookList.DataSource = bookList;
                }
                else
                {
                    MessageBox.Show("请选择要删除的图书！", "信息提示");
                }
            }
        }
        //在数据库中添加借书
        private void btnSave_Click(object sender, EventArgs e)
        {
            //数据验证
            if (this.dgvBookList.RowCount > 0)
            {
                string borrowId = DateTime.Now.ToString("yyyyMMddHHmmssms");
                //封装数据(副表Detail和主表info
                objBorrowInfo = new BorrowInfo()
                {
                    BorrowDate = DateTime.Now,
                    AdminName_B = Program.CurrentSysAdmin.AdminName,
                    BorrowId = borrowId,
                    ReaderId = objReader.ReaderId
                };
                List<BorrowDetail> detaiList = new List<BorrowDetail>();
                foreach (Book item in bookList)//封装副表borrowDetail
                {
                    detaiList.Add(new BorrowDetail()
                    {
                        BookId = item.BookId,
                        BorrowCount = item.BorrowCount,
                        BorrowId = borrowId,
                        Expire = item.Expire,
                        NonReturnCount = item.BorrowCount
                    });
                }
                //数据更新
                bool result = objBorrowManger.AddBorrowInfo(objBorrowInfo, detaiList);
                if (result)
                {
                    MessageBox.Show("借书成功！", "信息提示");
                    //清除数据
                    this.dgvBookList.DataSource = null;
                    this.objBorrowInfo = null;
                }
            }
        }
    }
}
