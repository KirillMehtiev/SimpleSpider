using System;
using System.Collections.Generic;
using System.Text;

namespace DLL.Entities
{
    internal class Record : BaseEntity
    {
        public int WebSiteId { get; set; }
        public DateTime RecordCreated { get; set; }

        public virtual ICollection<RecordItem> Items { get; set; }
    }
}
