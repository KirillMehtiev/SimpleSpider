using BLL.Dtos;
using System.Threading.Tasks;

namespace BLL.Abstractions
{
    public interface ISpiderService
    {
        Task<RecordDto> CrawlWebsiteAsync(string startUrl);
    }
}
