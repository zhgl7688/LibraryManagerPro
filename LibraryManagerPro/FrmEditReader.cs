using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LibraryManagerPro
{
    public partial class FrmEditReader : Form
    {
        private Models.Reader objReader;

        public FrmEditReader()
        {
            InitializeComponent();
        }
        //接受主窗体传递的读者信息
        public FrmEditReader(Models.Reader objReader)
        {
            InitializeComponent();
            this.objReader = objReader;

            //初始化会员角色下拉框
            this.cboReaderRole.DataSource = new BLL.ReaderManager ().GetAllReaderRole();;
            this.cboReaderRole.DisplayMember = "RoleName";
            this.cboReaderRole.ValueMember = "RoleId"; 
            this.displayReader();
        }
        private void displayReader()
        {
            this.txtIDCard.Text = objReader.IDCard;
            this.txtReaderName.Text = objReader.ReaderName;
            this.txtAddress.Text = objReader.ReaderAddress;
            if (objReader.Gender=="男")
                this.rdoMale .Checked =true ;
            else 
                this.rdoFemale .Checked =true ;
            this.txtPhone.Text = objReader.PhoneNumber;
            this.txtPostcode .Text = objReader.PostCode;
            this.txtReadingCard.Text = objReader.ReadingCard;
            this.cboReaderRole .SelectedValue  = objReader.RoleId ;
            this.pbReaderPhoto .Image = objReader.ReaderImage.Length != 0 ? (Image)new Common.SerializeObjectToString().DeserializeObject(objReader.ReaderImage) : null;
    
        }
        private void btnStartVideo_Click(object sender, EventArgs e)
        {

        }
        //修改读者信息
        private void btnSave_Click(object sender, EventArgs e)
        {
            #region 数据验证
            if (this.txtReaderName.Text.Trim().Length == 0)
            {
                MessageBox.Show("读者姓名不能为空", "信息提示");
                this.txtReaderName.Focus();
                return;
            }
           
            if (this.txtIDCard.Text.Trim().Length == 0)
            {
                MessageBox.Show("身份证不能为空", "信息提示");
                this.txtIDCard.Focus();
                return;
            }
            //身份证不能重复
            if (new BLL.ReaderManager().ExitIDCard(this.txtIDCard.Text.Trim(),objReader.ReaderId ))
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
            objReader.RoleName = this.cboReaderRole.Text ;
            objReader.IDCard = this.txtIDCard.Text.Trim();
            objReader.ReaderName = this.txtReaderName.Text.Trim();
            objReader.ReadingCard = this.txtReadingCard.Text.Trim();
            objReader.Gender = this.rdoMale.Checked ? "男" : "女";
            objReader.RoleId = Convert.ToInt32(this.cboReaderRole.SelectedValue);
            objReader.PostCode = this.txtPostcode.Text == null ? "" : this.txtPostcode.Text.Trim();
            objReader.PhoneNumber = this.txtPhone.Text.Trim();
            objReader.ReaderAddress = this.txtAddress.Text == null ? "" : this.txtAddress.Text.Trim();
            objReader.ReaderImage = this.pbReaderPhoto.Image == null ? "" : new Common.SerializeObjectToString().SerializeObject(this.pbReaderPhoto.Image);
            #endregion
            #region 数据会员更新
            try
            {
                int result = new BLL.ReaderManager().ModifyReader(objReader);
                if (result == 1)
                {
                    MessageBox.Show("会员修改成功");
                    this.DialogResult = DialogResult.OK;
                }
                else
                {
                    MessageBox.Show("会员修改失败，请检查数据");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            #endregion
        }
    }
}
