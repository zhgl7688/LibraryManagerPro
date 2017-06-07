using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace LibraryManagerPro
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
             Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
           //显示登录窗体返回登录信息
            DialogResult result = new FrmAdminLogin().ShowDialog();//启动登录窗体
            if (result == DialogResult.OK)//登录成功
            {
                Application.Run(new FrmMain());
            }
            else
            {
                Application.Exit();
            }
         
        }
        //定义一个全局变量（用来保存当前用户信息）
        public static Models.SysAdmin CurrentSysAdmin = null;
    }
}
