using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    /// <summary>
    /// 出版社
    /// </summary>
    [Serializable]
    public class Publisher
    {
        public int PublisherId { get; set; }
        public string PublisherName { get; set; }

    }
}
