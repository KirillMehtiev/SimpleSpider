using System;
using System.Collections.Generic;
using System.Text;

namespace DLL.Dtos
{
    public class RecordDto
    {
        public int Id { get; set; }
        public int WebSiteId { get; set; }
        public DateTime RecordCreated { get; set; }

        public ICollection<RecordItemDto> Items { get; set; }
    }
}
