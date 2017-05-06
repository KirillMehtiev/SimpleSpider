using System;
using System.Collections.Generic;
using System.Text;

namespace DLL.Entities
{
    internal class WebSite : BaseEntity
    {
        public string BaseUrl { get; set; }

        public virtual ICollection<Record> Records { get; set; }
    }
}
