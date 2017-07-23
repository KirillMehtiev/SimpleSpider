using BLL.Dtos;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace BLL.Abstractions
{
    public interface ISpiderService
    {
        Task<RecordDto> CrawlWebsiteAsync(string startUrl, BlockingCollection<RecordItemDto> bc, CancellationToken cancellationToken);
    }
}
