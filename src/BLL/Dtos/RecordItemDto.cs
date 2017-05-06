using System;
using System.Collections.Generic;
using System.Text;

namespace DLL.Dtos
{
    public class RecordItemDto
    {
        public int Id { get; set; }
        public int RecordId { get; set; }
        public string RequestUrl { get; set; }
        public TimeSpan RequestTime { get; set; }
    }
}
