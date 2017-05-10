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
        // Throw exeption if not valid url

        [Fact]
        public async Task CanFindAllPageBaseOnGivenUrl()
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
            urlFilter.Setup(m => m.RemoveUnnecessary(It.IsAny<List<string>>(), It.IsAny<Uri>()))
                .Returns((ICollection<string> urls, Uri baseUri) =>
            {
                return new List<Uri>
                { new Uri("http://www.example.com"),
                    new Uri("http://www.example.com/books"),
                    new Uri("http://www.example.com/books/5")
                };
            });

            ISpider spider = new Spider(htmlParse.Object, client.Object, urlFilter.Object);

            var result = await spider.CrawlWebsite(baseUrl);
            var actualPages = result.Items.OrderBy(o => o.RequestUrl).ToList();

            Assert.Equal(expectedPages.Count, actualPages.Count);
            Assert.Collection(actualPages,
                item => Assert.Equal(expectedPages[0], item.RequestUrl),
                item => Assert.Equal(expectedPages[1], item.RequestUrl),
                item => Assert.Equal(expectedPages[2], item.RequestUrl));
        }

        [Fact]
        public void ThrowExeptionIfProviderUrlNotValid()
        {
            var invalidUrl = "www.example.com";

            Mock<IClient> client = new Mock<IClient>();
            Mock<IHtmlParser> htmlParse = new Mock<IHtmlParser>();
            Mock<IUrlFilter> urlFilter = new Mock<IUrlFilter>();
            ISpider spider = new Spider(htmlParse.Object, client.Object, urlFilter.Object);

            var exeption = Record.ExceptionAsync(() => spider.CrawlWebsite(invalidUrl));

            Assert.Equal(typeof(UriFormatException), exeption.GetType());
        }
    }

    internal interface ISpider
    {
        Task<RecordDto> CrawlWebsite(string startUrl);
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

        public async Task<RecordDto> CrawlWebsite(string startUrl)
        {
            if (!Uri.IsWellFormedUriString(startUrl, UriKind.Absolute))
                throw new UriFormatException("Url in not valid");

            var startUri = new Uri(startUrl, UriKind.Absolute);

            var result = new RecordDto() { Items = new List<RecordItemDto>() };
            var visitedUri = new List<Uri>();
            var pagesToBeCalled = new Queue<Uri>();

            pagesToBeCalled.Enqueue(startUri);

            while (pagesToBeCalled.Any())
            {
                var currentUri = pagesToBeCalled.Dequeue();

                var requestResult = await client.GetAsync(currentUri.AbsoluteUri);
                var content = await requestResult.Content.ReadAsStringAsync();

                visitedUri.Add(currentUri);

                var parsedUrls = htmlParser.GetUrlsFromHtmlATag(content);
                var filteredPaths = urlFilter.RemoveUnnecessary(parsedUrls, currentUri);

                foreach (var parsedUri in filteredPaths)
                {
                    if (!visitedUri.Contains(parsedUri) && !pagesToBeCalled.Contains(parsedUri))
                        pagesToBeCalled.Enqueue(parsedUri);
                }
            }

            result = FactoryMethod(visitedUri);

            return result;
        }

        private RecordDto FactoryMethod(List<Uri> pagesUrls)
        {
            var result = new RecordDto();

            result.RecordCreated = DateTime.UtcNow;
            result.Items = new List<RecordItemDto>();

            foreach (var url in pagesUrls)
            {
                var recordItemDto = new RecordItemDto
                {
                    RequestUrl = url.AbsolutePath
                };

                result.Items.Add(recordItemDto);
            }

            return result;
        }

    }
}
