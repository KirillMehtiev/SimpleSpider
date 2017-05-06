using System;
using System.Collections.Generic;
using System.Text;

namespace DLL.Entities
{
    internal class RecordItem : BaseEntity
    {
        public int RecordId { get; set; }
        public string RequestUrl { get; set; }
        public TimeSpan RequestTime { get; set; }
    }
}
