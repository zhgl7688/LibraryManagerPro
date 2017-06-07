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
    public partial class FrmReturnBook : Form
    {
        //初始化逻辑访问对象
        private BorrowManager objBorrowManager = new BorrowManager();
        private ReturnBookManager objReturnBookManager = new ReturnBookManager();
        private List<BorrowDetail> detailList = null;
        private List<ReturnBook> returnBookList = new List<ReturnBook>();
        private Reader objReader = null;
        public FrmReturnBook()
        {
            InitializeComponent();
            this.txtBarCode.Enabled = false;
            this.btnConfirmReturn.Enabled = false;
            this.dgvNonReturnList.AutoGenerateColumns = false;
            this.dgvReturnList.AutoGenerateColumns = false;
            //加粗显示相关的借书数据
            this.dgvNonReturnList.Columns["BorrowCount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            this.dgvNonReturnList.Columns["BorrowCount"].DefaultCellStyle.Font = new Font("微软雅黑", 14);
            this.dgvNonReturnList.Columns["ReturnCount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            this.dgvNonReturnList.Columns["ReturnCount"].DefaultCellStyle.Font = new Font("微软雅黑", 14);
            this.dgvNonReturnList.Columns["NonReturnCount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            this.dgvNonReturnList.Columns["NonReturnCount"].DefaultCellStyle.Font = new Font("微软雅黑", 14);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        //显示借书信息
        private void txtReadingCard_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 13 && this.txtReadingCard.Text.Trim().Length != 0)
            {
                //调用借书信息
                objReader = objBorrowManager.GetReaderBorrowInfo(this.txtReadingCard.Text.Trim());
                if (objReader != null)
                { //当已借总数大于零(打开还书开关、并显示借书信息)
                    if (objReader.BorrowCount > 0)
                    {
                        if (objReader.StatusId == 0)
                        {
                            MessageBox.Show("借书证禁用", "禁用提示");
                            return;
                        }
                        //打开还书开关
                        this.btnConfirmReturn.Enabled = true;
                        this.txtBarCode.Enabled = true;
                        //显示借书信息
                        this.lblReaderName.Text = objReader.ReaderName;
                        this.lblRoleName.Text = objReader.RoleName;
                        this.lblAllowCounts.Text = objReader.AllowCounts.ToString();
                        this.lblBorrowCount.Text = objReader.BorrowCount.ToString();
                        this.pbReaderImage.Image = objReader.ReaderImage.Length != 0 ? (Image)new Common.SerializeObjectToString().DeserializeObject(objReader.ReaderImage) : null;
                        int remainder = objReader.AllowCounts - objReader.BorrowCount;//剩借总数
                        this.lbl_Remainder.Text = remainder.ToString();
                        //显示借书列表
                        detailList = objReturnBookManager.QueryBookByReadingCard(this.txtReadingCard.Text.Trim());
                        this.dgvNonReturnList.DataSource = detailList;
                    }
                    else
                    {
                        MessageBox.Show("该证还没有借书，没有书可还", "提示信息");
                        this.txtReadingCard.SelectAll();
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("没有该借书证，请核实", "提示信息");
                    return;
                }
            }
        }
        //确认还书
        private void btnConfirmReturn_Click(object sender, EventArgs e)
        {
            if (returnBookList.Count > 0)
            {
                foreach (ReturnBook item in returnBookList)//补全还书日期和操作员
                {
                    item.ReturnDate = DateTime.Now;
                    item.AdminName_R = Program.CurrentSysAdmin.AdminName;
                }
                //更新数据(借书名细，还书名细）
               bool result= objReturnBookManager.UpReturn( returnBookList);
                if (!result )
                { MessageBox.Show("还书不成功", "信息提示"); return; }
                else
                {
                    MessageBox.Show("还书成功！", "信息提示");
                    //清空数据，等待下一位还书
                    this.btnConfirmReturn.Enabled = false;
                    this.txtBarCode.Enabled = false;
                    this.txtReadingCard.Clear();
                    this.returnBookList.Clear();
                    this.dgvReturnList.DataSource = null;
                    this.dgvNonReturnList.DataSource = null;
                    this.detailList.Clear();
                    this.lblReturnCount.Text = "0";
                    this.lbl_Remainder.Text = "0";
                    this.lblAllowCounts.Text = "0";
                    this.lblBorrowCount.Text = "0";
                    this.lblReaderName.Text = "";
                    this.lblRoleName.Text = "";
                    this.pbReaderImage.Image = null;
                    this.txtReadingCard.Focus();
                }
            }
            else
            {
                MessageBox.Show("没有还书记录", "信息提示");
            }
        }
        //临时还书信息根据图书条码查询图书信息
        private void txtBarCode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 13 && this.txtBarCode.Text.Trim().Length > 0)
            {
                if (this.lblBorrowCount.Text == "0") { MessageBox.Show("书已还清,请点确认", "信息提示"); return; }
                #region 查询借书列表，并更新
                BorrowDetail objDetail = detailList.FirstOrDefault<BorrowDetail>(b => b.BarCode.Equals ( this.txtBarCode.Text.Trim()) && b.NonReturnCount != 0);
                if (objDetail == null) { MessageBox.Show("该图书已还清或不在还书之列，请核实", "信息提示"); this.txtBarCode.SelectAll(); return; }
                if (objDetail.NonReturnCount != 0)//大于1本
                {
                    objDetail.ReturnCount += 1;
                    objDetail.NonReturnCount -= 1;
                }
                #endregion
                #region 在本地还书列表增加
                //查询本地还书列表图书
                ReturnBook objReturnBook = returnBookList.FirstOrDefault<ReturnBook>(b => b.BorrowDetailId.Equals ( objDetail.BorrowDetailId));
                if (objReturnBook != null)
                {
                    objReturnBook.ReturnCount += 1;
                }
                else
                {
                    //还书列表不存在，根据图书条码从借书列表detailList中查询图书信息
                    objReturnBook = new ReturnBook();
                    objReturnBook.BorrowDetailId = objDetail.BorrowDetailId;
                    objReturnBook.BarCode = objDetail.BarCode;
                    objReturnBook.BookName = objDetail.BookName;
                    objReturnBook.ReturnCount = 1;
                    //添加集合BookList,显示图书信息
                    returnBookList.Add(objReturnBook);
                }
                #endregion
                //显示查询图书信息
                this.lblReturnCount.Text = (Convert.ToInt32(this.lblReturnCount.Text.Trim()) + 1).ToString();
                this.lblBorrowCount.Text = (Convert.ToInt32(this.lblBorrowCount.Text.Trim()) - 1).ToString();
                this.lbl_Remainder.Text = (Convert.ToInt32(this.lbl_Remainder.Text.Trim()) + 1).ToString();
                var list = from rb in returnBookList
                           group rb by new { rb.BarCode, rb.BookName } into s
                           select new
                           {
                               BarCode = s.Key.BarCode,
                               BookName = s.Key.BookName,
                               ReturnCount = s.Sum(m => m.ReturnCount)
                           };
                this.dgvReturnList.DataSource = null;
                this.dgvReturnList.DataSource = list.ToList();
                var detaill = detailList.Where(d => d.NonReturnCount != 0);
                this.dgvNonReturnList.DataSource = null;
                this.dgvNonReturnList.DataSource = detaill .ToList ();
                this.txtBarCode.Clear();

            }

        }


    }
}
