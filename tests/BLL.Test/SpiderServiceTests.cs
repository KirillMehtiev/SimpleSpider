using BLL.Abstractions;
using DLL.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using System.Linq;
using BLL.Implementation;
using Moq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Diagnostics;

namespace BLL.Test
{
    public class SpiderServiceTests
    {
        // Return valid result
        // Can extract base url for all pages

        [Fact]
        public void CanFindAllPageBaseOnGivenUrl()
        {
            var baseUrl = "http://www.example.com";
            var expectedPages = new List<string> {
                "/",
                "/books",
                "/books/5"
            };

            Mock<IClient> client = new Mock<IClient>();
            client.Setup(m => m.GetAsync(It.IsAny<string>())).ReturnsAsync((string url) =>
            {
                if (url == baseUrl)
                    return new HttpResponseMessage { Content = new StringContent(expectedPages[0]) };
                else if (url == $"{baseUrl}{expectedPages[0]}")
                    return new HttpResponseMessage { Content = new StringContent(expectedPages[1]) };
                else if (url == $"{baseUrl}{expectedPages[1]}")
                    return new HttpResponseMessage { Content = new StringContent(expectedPages[2]) };

                return new HttpResponseMessage { Content = new StringContent(string.Empty) };
            });

            Mock<IHtmlParser> htmlParse = new Mock<IHtmlParser>();
            htmlParse.Setup(m => m.GetUrlsFromHtmlATag(It.IsAny<string>())).Returns((string page) =>
            {
                return new List<string> { page };
            });

            Mock<IUrlFilter> urlFilter = new Mock<IUrlFilter>();

            ISpider spider = new Spider(htmlParse.Object, client.Object, urlFilter.Object);

            var result = spider.CrawlWebsite(baseUrl);
            var actualPages = result.Items.OrderBy(o => o.RequestUrl).ToList();

            Assert.Equal(expectedPages.Count, actualPages.Count);
            Assert.Collection(actualPages,
                item => Assert.Equal(expectedPages[0], item.RequestUrl),
                item => Assert.Equal(expectedPages[1], item.RequestUrl),
                item => Assert.Equal(expectedPages[2], item.RequestUrl));
        }
    }

    internal interface ISpider
    {
        RecordDto CrawlWebsite(string startUrl);
    }

    internal class Spider : ISpider
    {
        private readonly IHtmlParser htmlParser;
        private readonly IClient client;
        private readonly IUrlFilter urlFilter;

        public Spider(IHtmlParser htmlParser, IClient client, IUrlFilter urlFilter)
        {
            this.htmlParser = htmlParser;
            this.client = client;
            this.urlFilter = urlFilter;
        }
        
        public RecordDto CrawlWebsite(string startUrl)
        {
            var result = new RecordDto();
            var calledPaths = new List<string>();
            var pagesQueue = new Queue<string>();

            var url = new Uri(startUrl);
            var baseUrl = $"{url.Scheme}://{url.Host}";
            var absolutePath = url.AbsolutePath;

            pagesQueue.Enqueue(absolutePath);

            while (pagesQueue.Any())
            {
                var currentAbsolutePath = pagesQueue.Dequeue();

                var requestResult = client.GetAsync($"{baseUrl}{currentAbsolutePath}").Result.Content.ReadAsStringAsync();

                calledPaths.Add(currentAbsolutePath);

                var parsedUrls = htmlParser.GetUrlsFromHtmlATag(requestResult.Result);
                var filteredPaths = urlFilter.ByHostnameAndPathToNewPage(parsedUrls);

                foreach (var parsedUrl in filteredPaths)
                {
                    if (!calledPaths.Contains(parsedUrl) && parsedUrl != string.Empty)
                        pagesQueue.Enqueue(parsedUrl);
                }
            }

            result = FactoryMethod(calledPaths);

            return result;
        }

        private RecordDto FactoryMethod(List<string> pagesUrls)
        {
            var result = new RecordDto();

            result.RecordCreated = DateTime.UtcNow;
            result.Items = new List<RecordItemDto>();

            foreach (var url in pagesUrls)
            {
                var recordItemDto = new RecordItemDto
                {
                    RequestUrl = url
                };

                result.Items.Add(recordItemDto);
            }

            return result;
        }
        
    }
}
