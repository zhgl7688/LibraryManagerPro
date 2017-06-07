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
    public partial class FrmMain : Form
    {
        public FrmMain()
        {
            InitializeComponent();
            this.lblAdminName.Text += Program.CurrentSysAdmin.AdminName;

        }

        //关闭所有嵌入窗体并显示新窗体
        private void DisplayFrm(Form objFrm)
        {
            //将嵌入窗体中的所有窗体关闭
            foreach (Control item in this.spContainer.Panel2.Controls)
            {
                if (item is Form) ((Form)item).Close();
            }
            //显示传入窗体
            objFrm.TopLevel = false;
            objFrm.FormBorderStyle = FormBorderStyle.None;
            //objFrm.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            objFrm.Parent = this.spContainer.Panel2;
            objFrm.Dock = DockStyle.Fill;
            objFrm.Show();
            this.lblOperationName.Text = objFrm.Text;//显示嵌入窗体名
        }
        #region 显示嵌入窗体

        //添加新书
        private void btnAddBook_Click(object sender, EventArgs e)
        {
            this.DisplayFrm(new FrmAddBook());
        }
        //图书上架
        private void btnBookNew_Click(object sender, EventArgs e)
        {
            this.DisplayFrm(new FrmNewBook());
        }
        //图书维护
        private void btnBookManage_Click(object sender, EventArgs e)
        {
            this.DisplayFrm(new FrmBookManage());
        }
        //会员管理
        private void btnReaderManager_Click(object sender, EventArgs e)
        {
            this.DisplayFrm(new FrmReaderManage());
        }
        //图书出借
        private void btnBorrowBook_Click(object sender, EventArgs e)
        {
            this.DisplayFrm(new FrmBorrowBook());
        }
        //图书归还
        private void btnReturnBook_Click(object sender, EventArgs e)
        {
            this.DisplayFrm(new FrmReturnBook());
        }
        //密码修改
        private void btnModifyPwd_Click(object sender, EventArgs e)
        {
            this.DisplayFrm(new FrmModifyPwd());
        }
        #endregion

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FrmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult result = MessageBox.Show("确认退出吗？","退出提示",MessageBoxButtons.YesNo ,MessageBoxIcon.Warning );
            if (result !=DialogResult.Yes )
            {
                e.Cancel = true;
            }
        }
    }
}
