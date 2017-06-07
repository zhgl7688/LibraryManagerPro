using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    /// <summary>
    /// 用户实体类
    /// </summary>
    [Serializable]
    public class SysAdmin
    {
        public int AdminId { get; set; }
        public string AdminName { get; set; }
        public string LoginPwd { get; set; }
        public int StatusId { get; set; }
        public int RoleId { get; set; }

    }
}
