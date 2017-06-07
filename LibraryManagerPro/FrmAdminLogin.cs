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
    public partial class FrmAdminLogin : Form
    {
        private SysAdminManager objSysAdminManager = new SysAdminManager();
        public FrmAdminLogin()
        {
            InitializeComponent();
        }
        //关闭窗体
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        //根据用户ID和密码登录事件
        private void btnLogin_Click(object sender, EventArgs e)
        {
            //数据验证
            if (this.txtAdminId.Text.Trim().Length == 0 || this.txtLoginPwd.Text.Trim().Length == 0)
            {
                MessageBox.Show("用户名或密码不能为空", "登录提示");
                this.txtAdminId.Focus();
                return;
            }
            //数据封装
            SysAdmin objAdmin = new SysAdmin()
            {
                AdminId = Convert.ToInt32(this.txtAdminId.Text.Trim()),
                LoginPwd = this.txtLoginPwd.Text.Trim()
            };
            try
            {
                objAdmin = objSysAdminManager.AdminLogin(objAdmin);
                if (objAdmin == null)
                {
                    MessageBox.Show("用户名或密码不正确！", "登录提示");
                    this.txtAdminId.Focus();
                }
                else
                {
                    this.DialogResult = DialogResult.OK;
                    Program.CurrentSysAdmin = objAdmin;
                    this.Close();
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show("与数据库连接失败,具体信息：" + ex.Message);
            }

        }
        //输入必须为正整数
        private void txtAdminId_TextChanged(object sender, EventArgs e)
        {

            if (!Common.DataValidate.IsInteger(this.txtAdminId.Text.Trim()))
            {
                this.txtAdminId.Clear();
            }
        }
        //回车键跳到密码框中
        private void txtAdminId_KeyDown(object sender, KeyEventArgs e)
        {
            if (this.txtAdminId.Text.Trim().Length != 0 && e.KeyValue == 13)
            {
                this.txtLoginPwd.Focus();
            }
        }
        //回车键运行登录事件
        private void txtLoginPwd_KeyDown(object sender, KeyEventArgs e)
        {
            if (this.txtLoginPwd.Text.Trim().Length != 0 && e.KeyValue == 13)
            {
                btnLogin_Click(null, null);
            }
        }
    }
}
