using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using System.Linq;

namespace BLL.Test
{
    public class UrlFilterTests
    {
        private readonly IUrlFilter urlFilter = new UrlFilter();

        [Fact]
        public void OnlyPathsToNewPage()
        {
            var testUrl = new List<string>
            {
                "",
                "/books/5",
                "/books/5&page=5",
                "/books/5/page/10",
                "/books/5#description"
            };

            var expectedUrl = new List<string> {
                "/books/5",
                "/books/5/page/10"
            };

            var actualUrl = urlFilter.ByHostnameAndPathToNewPage(testUrl);

            Assert.Equal(expectedUrl.Count, actualUrl.Count);
            Assert.Collection(actualUrl,
                item => Assert.Equal(expectedUrl[0], item),
                item => Assert.Equal(expectedUrl[1], item));
        }

        [Fact]
        public void OnlyPathsForCurrentHostname()
        {
            var testUrl = new List<string>
            {
                "/books/5",
                "http://www.example.com"
            };

            var expectedUrl = new List<string> {
                "/books/5"
            };

            var actualUrl = urlFilter.ByHostnameAndPathToNewPage(testUrl);

            Assert.Equal(expectedUrl.Count, actualUrl.Count);
            Assert.Collection(actualUrl,
                item => Assert.Equal(expectedUrl[0], item));
        }
    }

    internal class UrlFilter : IUrlFilter
    {
        public ICollection<string> ByHostnameAndPathToNewPage(ICollection<string> urls)
        {
            throw new NotImplementedException();
        }
    }

    internal interface IUrlFilter
    {
        ICollection<string> ByHostnameAndPathToNewPage(ICollection<string> urls);
    }
}
