using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    /// <summary>
    /// 图书分类
    /// </summary>
    [Serializable]
    public class Category
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }

    }
}
