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
using MyVideo;

namespace LibraryManagerPro
{
    public partial class FrmReaderManage : Form
    {
        //初始化逻辑访问对象
        private ReaderManager objReaderManager = new ReaderManager();
        Video objVideo = null;
        Reader objReader = null;
        List<Reader> readerList = null;
        public FrmReaderManage()
        {
            InitializeComponent();
            //初始化会员角色下拉框
            DataTable dt = objReaderManager.GetAllReaderRole();
            this.cboReaderRole.DataSource = dt;
            this.cboReaderRole.DisplayMember = "RoleName";
            this.cboReaderRole.ValueMember = "RoleId";
            //初始化查询的角色下拉框
            this.cboRole.DataSource = dt.Copy();
            this.cboRole.DisplayMember = "RoleName";
            this.cboRole.ValueMember = "RoleId";
            //禁用修改和借阅证挂失按钮
            this.btnEdit.Enabled = false;
            this.btnEnable.Enabled = false;
        }
        //关闭窗口
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        //添加会员读者
        private void btnAdd_Click(object sender, EventArgs e)
        {
            #region 数据验证
            if (this.txtReaderName.Text.Trim().Length == 0)
            {
                MessageBox.Show("读者姓名不能为空", "信息提示");
                this.txtReaderName.Focus();
                return;
            }
            if (this.txtReadingCard.Text.Trim().Length == 0)
            {
                MessageBox.Show("借阅证编号不能为空", "信息提示");
                this.txtReadingCard.Focus();
                return;
            }
            //借阅证不能重复
            if (objReaderManager.ExitReaderCard(this.txtReadingCard.Text.Trim()))
            {
                MessageBox.Show("借阅证编号已存在", "信息提示");
                this.txtReadingCard.Focus();
                return;
            }
            if (this.txtIDCard.Text.Trim().Length == 0)
            {
                MessageBox.Show("身份证不能为空", "信息提示");
                this.txtIDCard.Focus();
                return;
            }
            //身份证不能重复
            if (objReaderManager.ExitIDCard(this.txtIDCard.Text.Trim()))
            {
                MessageBox.Show("身份证已存在", "信息提示");
                this.txtIDCard.Focus();
                return;
            }
            if (!(this.rdoMale.Checked || this.rdoFemale.Checked))
            {
                MessageBox.Show("请选择性别", "信息提示");
                this.rdoMale.Focus();
                return;
            }
            if (this.cboReaderRole.Text.Trim().Length == 0)
            {
                MessageBox.Show("会员角色不能为空", "信息提示");
                this.cboReaderRole.Focus();
                return;
            }
            if (this.txtPhone.Text.Trim().Length == 0)
            {
                MessageBox.Show("会员电话不能为空", "信息提示");
                this.txtPhone.Focus();
                return;
            }
            #endregion
            #region 数据封装
            Reader objReader = new Reader()
            {
                IDCard = this.txtIDCard.Text.Trim(),
                ReaderName = this.txtReaderName.Text.Trim(),
                ReadingCard = this.txtReadingCard.Text.Trim(),
                Gender = this.rdoMale.Checked ? "男" : "女",
                RoleId = Convert.ToInt32(this.cboReaderRole.SelectedValue),
                PostCode = this.txtPostcode.Text == null ? "" : this.txtPostcode.Text.Trim(),
                PhoneNumber = this.txtPhone.Text.Trim(),
                ReaderAddress = this.txtAddress.Text == null ? "" : this.txtAddress.Text.Trim(),
                ReaderImage = this.pbReaderPhoto.Image == null ? "" : new Common.SerializeObjectToString().SerializeObject(this.pbReaderPhoto.Image)
            };
            #endregion
            #region 数据会员添加更新
            try
            {
                int result = objReaderManager.AddReader(objReader);
                if (result == 1)
                {
                    MessageBox.Show("会员添加成功");
                    //清空填写框
                    foreach (Control item in this.tabPage2.Controls)
                    {
                        if (item is TextBox) ((TextBox)item).Text = "";
                        if (item is PictureBox) ((PictureBox)item).Image = null;
                    }
                    this.cboReaderRole.SelectedItem = -1;
                }
                else
                {
                    MessageBox.Show("会员添加失败，请检查数据");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            #endregion
        }
        //根据会员角色查询会员信息
        private void btnQueryReader_Click(object sender, EventArgs e)
        {
            if (this.cboRole.SelectedValue.ToString().Length != 0)
            {
                int readerCount = 0;
                readerList = objReaderManager.GetReaderByRoleId(this.cboRole.SelectedValue.ToString(), out  readerCount);
                this.lvReader.Items.Clear();
                //给ListView绑定数据源
                foreach (Reader reader in readerList)
                {
                    ListViewItem ivItem = new ListViewItem(reader.ReaderId.ToString());
                    this.lvReader.Items.Add(ivItem);//添加首列数据
                    ivItem.SubItems.AddRange(new string[]{
                     reader.ReadingCard ,
                     reader.ReaderName ,
                     reader.Gender ,
                     reader.PhoneNumber ,
                     reader.RegTime .ToShortDateString (),
                     reader.StatuesDesc ,
                     reader.ReaderAddress 
                });
                }
                this.lblReaderCount.Text = readerCount.ToString();
            }
        }
        //根据身份证或借阅证查询会员信息
        private void btnQuery_Click(object sender, EventArgs e)
        {
            //判断根据身份证还是借阅证
            if (rdoIDCard.Checked && this.txt_IDCard.Text.Trim().Length != 0)
            {
                objReader = objReaderManager.GetReaderByIDCard(this.txt_IDCard.Text.Trim());
            }
            else if (this.txt_ReadingCard.Text.Trim().Length != 0)
            {
                objReader = objReaderManager.GetReaderByReadingCard(this.txt_ReadingCard.Text.Trim());
            }
            else
            {
                MessageBox.Show("请输入查询内容！", "查询提示");
                return;
            }
            if (objReader != null)
            {
                this.displayReader(objReader); //显示会员具体信息

            }
            else
            {
                MessageBox.Show("当前读者不存在", "信息提示");
                //清空列表
                clearDisplay();
            }
        }
        private void clearDisplay()
        {
            //禁用修改和借阅证挂失按钮
            this.btnEdit.Enabled = false;
            this.btnEnable.Enabled = false;
            this.lblIDCard.Text = "";
            this.lblReaderName.Text = "";
            this.lblAddress.Text = "";
            this.lblGender.Text = "";
            this.lblPhone.Text = "";
            this.lblPostCode.Text = "";
            this.lblReadingCard.Text = "";
            this.lblRoleName.Text = "";
            this.pbReaderImg.Image = null;
        }
        //显示会员具体信息
        private void displayReader(Reader objReader)
        {
            //开启修改和借阅证挂失按钮
            bool state = objReader.StatusId == 1;
            this.btnEdit.Enabled = state;
            this.btnEnable.Enabled = state;
            this.lblIDCard.Text = objReader.IDCard;
            this.lblReaderName.Text = objReader.ReaderName;
            this.lblAddress.Text = objReader.ReaderAddress;
            this.lblGender.Text = objReader.Gender;
            this.lblPhone.Text = objReader.PhoneNumber;
            this.lblPostCode.Text = objReader.PostCode;
            this.lblReadingCard.Text = objReader.ReadingCard;
            this.lblRoleName.Text = objReader.RoleName;
            this.pbReaderImg.Image = objReader.ReaderImage.Length != 0 ? (Image)new Common.SerializeObjectToString().DeserializeObject(objReader.ReaderImage) : null;
        }
        //列表数据与会员信息同步
        private void lvReader_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.lvReader.SelectedItems.Count > 0)
            {
                int readerId = Convert.ToInt32(this.lvReader.SelectedItems[0].Text.Trim());
                objReader = (readerList.Where(r => r.ReaderId == readerId)).First();
                objReader.RoleName = this.cboRole.Text;
                displayReader(objReader);
            }

        }
        //启动摄像头
        private void btnStartVideo_Click(object sender, EventArgs e)
        {
            try
            {
                //创建摄像头操作类
                objVideo = new Video(this.pbReaderVideo.Handle, this.pbReaderVideo.Left,
                    this.pbReaderVideo.Top, this.pbReaderVideo.Width, (short)this.pbReaderVideo.Height);
                //打开摄像头
                objVideo.OpenVideo();
                //同时禁用和打开相关按钮
                this.btnStartVideo.Enabled = false;
                this.btnCloseVideo.Enabled = true;
                this.btnTake.Enabled = true;

                this.btnCloseVideo.BackColor = Color.Red;
                this.btnTake.BackColor = Color.YellowGreen;
                this.btnTake.ForeColor = Color.White;
            }
            catch (Exception ex)
            {
                MessageBox.Show("摄像头启动失败！" + ex.Message);
            }
        }
        //关闭摄像头
        private void btnCloseVideo_Click(object sender, EventArgs e)
        {
            objVideo.CloseVideo();
            this.btnStartVideo.Enabled = true;
            this.btnCloseVideo.Enabled = false;
            this.btnTake.Enabled = false;

            //this.btnCloseVideo .BackColor .
        }
        //拍照
        private void btnTake_Click(object sender, EventArgs e)
        {
            //  objVideo.
        }
        //修改会员信息
        private void btnEdit_Click(object sender, EventArgs e)
        {
            //显示修改窗体并传递会员信息
            if (objReader == null)
            {
                MessageBox.Show("没有要修改的会员信息？", "信息提示");
                return;
            }
            FrmEditReader objFrm = new FrmEditReader(objReader);
            //返回修改结果并显示
            DialogResult result = objFrm.ShowDialog();
            if (result == DialogResult.OK)
            {
                //修改成功并显示
                displayReader(objReader);
            }
        }
        //借阅证挂失
        private void btnEnable_Click(object sender, EventArgs e)
        {
            if (objReader != null)
            {
                DialogResult result = MessageBox.Show("确认挂失:" + objReader.ReaderName, "挂失提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if (result == DialogResult.Yes)
                {
                    try
                    {
                        objReaderManager.ForbiddenReaderCard(objReader.ReaderId.ToString());
                        MessageBox.Show("挂失成功");
                        //清空信息

                    }
                    catch (Exception)
                    {

                        throw;
                    }


                }
            }
        }


    }
}
