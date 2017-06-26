using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.ViewModels
{
    public class RecordItemViewModel
    {
        public int Id { get; set; }
        public int RecordId { get; set; }
        public string RequestUrl { get; set; }
        public TimeSpan RequestTime { get; set; }
    }
}
