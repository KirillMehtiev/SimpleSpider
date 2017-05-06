using System;
using System.Collections.Generic;
using System.Text;

namespace DLL.Dtos
{
    public class WebSiteDto
    {
        public int Id { get; set; }
        public string BaseUrl { get; set; }
        public ICollection<RecordDto> Records { get; set; }
    }
}
