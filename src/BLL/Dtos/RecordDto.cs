using System;
using System.Collections.Generic;
using System.Text;

namespace BLL.Dtos
{
    public class RecordDto
    {
        public int Id { get; set; }
        public int WebSiteId { get; set; }
        public DateTime RecordCreated { get; set; }
        public string RequestedUrl { get; set; }

        public ICollection<RecordItemDto> Items { get; set; }
    }
}
