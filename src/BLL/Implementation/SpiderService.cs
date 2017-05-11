using BLL.Abstractions;
using BLL.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;

namespace BLL.Implementation
{
    public class SpiderService : ISpiderService
    {
        private readonly IHtmlParser htmlParser;
        private readonly IClient client;
        private readonly IUrlFilter urlFilter;

        public SpiderService(IHtmlParser htmlParser, IClient client, IUrlFilter urlFilter)
        {
            this.htmlParser = htmlParser;
            this.client = client;
            this.urlFilter = urlFilter;
        }

        public async Task<RecordDto> CrawlWebsiteAsync(string startUrl)
        {
            if (!Uri.IsWellFormedUriString(startUrl, UriKind.Absolute))
                throw new UriFormatException("Url in not valid");

            var startUri = new Uri(startUrl, UriKind.Absolute);
            var stopwath = new Stopwatch();
            var visitedUri = new List<Uri>();
            var pagesToBeCalled = new Queue<Uri>();
            var result = new RecordDto();

            pagesToBeCalled.Enqueue(startUri);

            result.RequestedUrl = startUrl;
            result.RecordCreated = DateTime.UtcNow;
            result.Items = new List<RecordItemDto>();

            while (pagesToBeCalled.Any())
            {
                var recordItem = new RecordItemDto();
                var currentUri = pagesToBeCalled.Dequeue();
                stopwath.Reset();

                stopwath.Start();
                var requestResult = await client.GetAsync(currentUri.AbsoluteUri);
                var content = await requestResult.Content.ReadAsStringAsync();
                stopwath.Stop();

                visitedUri.Add(currentUri);

                recordItem.RequestUrl = currentUri.AbsolutePath;
                recordItem.RequestTime = stopwath.Elapsed;
                result.Items.Add(recordItem);

                var parsedUrls = htmlParser.GetUrlsFromHtmlATag(content);
                var filteredPaths = urlFilter.RemoveUnnecessary(parsedUrls, currentUri);

                foreach (var parsedUri in filteredPaths)
                {
                    if (!visitedUri.Contains(parsedUri) && !pagesToBeCalled.Contains(parsedUri))
                        pagesToBeCalled.Enqueue(parsedUri);
                }
            }

            return result;
        }
    }
}
