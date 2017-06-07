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
    public partial class FrmModifyPwd : Form
    {
        public FrmModifyPwd()
        {
            InitializeComponent();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        //修改密码
        private void btnModify_Click(object sender, EventArgs e)
        {
            if (this.txtOldPwd .Text .Trim ().Length ==0)
            {
                MessageBox.Show("原密码不能为空", "信息提示");
                this.txtOldPwd.Focus();
                return;
            }
            if (this.txtNewPwd .Text.Trim().Length == 0)
            {
                MessageBox.Show("新密码不能为空", "信息提示");
                this.txtNewPwd.Focus();
                return;
            }
            if (this.txtNewPwdConfirm .Text.Trim().Length == 0)
            {
                MessageBox.Show("确认密码不能为空", "信息提示");
                this.txtNewPwdConfirm.Focus();
                return;
            }
            if (! this.txtNewPwdConfirm.Text.Trim().Equals (this.txtNewPwd .Text .Trim ()))
            {
                MessageBox.Show("确认密码与原密码不一致", "信息提示");
                this.txtNewPwdConfirm.SelectAll();
                this.txtNewPwdConfirm.Focus();
                return;
            }
            if (this.txtOldPwd .Text .Equals (Program.CurrentSysAdmin .LoginPwd ))
            {
                Program.CurrentSysAdmin .LoginPwd =this.txtNewPwd .Text .Trim ();
                new BLL.SysAdminManager().ModifyPwd(Program.CurrentSysAdmin);
                MessageBox.Show("密码修改成功！");
                this.Close();
            }
        }
    }
}
