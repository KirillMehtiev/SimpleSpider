using DLL.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace BLL.Abstractions
{
    public interface ISpiderService
    {
       RecordDto CrawlWebsite(string startUrl);
    }
}
