using BLL.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using System.Linq;
using BLL.Implementation;
using Moq;
using System.Threading.Tasks;
using System.Net.Http;

namespace BLL.Test
{
    public class SpiderServiceTests
    {
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
                return urls.Select(url => new Uri(baseUri, url)).ToList();
            });

            ISpiderService spider = new SpiderService(htmlParse.Object, client.Object, urlFilter.Object);

            var result = await spider.CrawlWebsiteAsync(baseUrl);
            var actualPages = result.Items.OrderBy(o => o.RequestUrl).ToList();

            Assert.Equal(expectedPages.Count, actualPages.Count);
            Assert.Collection(actualPages,
                item => Assert.Equal(expectedPages[0], item.RequestUrl),
                item => Assert.Equal(expectedPages[1], item.RequestUrl),
                item => Assert.Equal(expectedPages[2], item.RequestUrl));
        }

        [Fact]
        public async Task ThrowExeptionIfProviderUrlNotValid()
        {
            var invalidUrl = "www.example.com";

            Mock<IClient> client = new Mock<IClient>();
            Mock<IHtmlParser> htmlParse = new Mock<IHtmlParser>();
            Mock<IUrlFilter> urlFilter = new Mock<IUrlFilter>();
            ISpiderService spider = new SpiderService(htmlParse.Object, client.Object, urlFilter.Object);

            var exeption = await Record.ExceptionAsync(() => spider.CrawlWebsiteAsync(invalidUrl));

            Assert.Equal(typeof(UriFormatException), exeption.GetType());
        }

        [Fact]
        public async Task ReturnFilledResult()
        {
            var baseUrl = "http://www.example.com";
            var expectedPages = new List<string> {
                "/",
                "/books",
            };

            Mock<IClient> client = new Mock<IClient>();
            client.Setup(m => m.GetAsync(It.IsAny<string>())).ReturnsAsync((string url) =>
            {
                return new HttpResponseMessage { Content = new StringContent(string.Empty) };
            });

            Mock<IHtmlParser> htmlParse = new Mock<IHtmlParser>();
            htmlParse.Setup(m => m.GetUrlsFromHtmlATag(It.IsAny<string>())).Returns((string page) =>
            {
                return expectedPages;
            });

            Mock<IUrlFilter> urlFilter = new Mock<IUrlFilter>();
            urlFilter.Setup(m => m.RemoveUnnecessary(It.IsAny<List<string>>(), It.IsAny<Uri>()))
                .Returns((ICollection<string> urls, Uri baseUri) =>
                {
                    return urls.Select(url => new Uri(baseUri, url)).ToList();
                });

            ISpiderService spider = new SpiderService(htmlParse.Object, client.Object, urlFilter.Object);

            var result = await spider.CrawlWebsiteAsync(baseUrl);
            var actual = result.Items.OrderBy(x => x.RequestUrl).ToList();

            Assert.Equal(expectedPages.Count, actual.Count);
            Assert.Equal(baseUrl, result.RequestedUrl);
            Assert.Collection(actual,
                item =>
                {
                    Assert.Equal(expectedPages[0], item.RequestUrl);
                    Assert.NotNull(item.RequestTime);
                },
                item =>
                {
                    Assert.Equal(expectedPages[1], item.RequestUrl);
                    Assert.NotNull(item.RequestTime);
                });
        }
    }
}
