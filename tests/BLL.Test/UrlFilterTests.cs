using BLL.Abstractions;
using BLL.Implementation;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace BLL.Test
{
    public class UrlFilterTests
    {
        private readonly IUrlFilter urlFilter = new UrlFilter();

        [Fact]
        public void RelativePathsToNewPages()
        {
            var baseUri = new Uri("http://www.example.com");

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
                "/books/5&page=5",
                "/books/5/page/10"
            };

            var actualUrl = urlFilter.RemoveUnnecessary(testUrl, baseUri);

            Assert.Equal(expectedUrl.Count, actualUrl.Count);
            Assert.Collection(actualUrl,
                item => Assert.Equal(expectedUrl[0], item.PathAndQuery),
                item => Assert.Equal(expectedUrl[1], item.PathAndQuery),
                item => Assert.Equal(expectedUrl[2], item.PathAndQuery));
        }

        [Fact]
        public void AbsolutePathsToNewPages()
        {
            var baseUri = new Uri("http://www.example.com");

            var testUrl = new List<string>
            {
                "http://www.example.com/books/5",
                "http://www.example.com/books/5/page/10",
            };

            var expectedUrl = new List<string> {
                "http://www.example.com/books/5",
                "http://www.example.com/books/5/page/10"
            };

            var actualUrl = urlFilter.RemoveUnnecessary(testUrl, baseUri);

            Assert.Equal(expectedUrl.Count, actualUrl.Count);
            Assert.Collection(actualUrl,
                item => Assert.Equal(expectedUrl[0], item.AbsoluteUri),
                item => Assert.Equal(expectedUrl[1], item.AbsoluteUri));
        }

        [Fact]
        public void ComplicatedRelativePathsToNewPages()
        {
            var baseUri = new Uri("http://www.example.com/one/oneMore/");

            var testUrl = new List<string>
            {
                "books",
                "./books/5",
                "../books",
                "../../books"
            };

            var expectedUrl = new List<string> {
                "http://www.example.com/one/oneMore/books",
                "http://www.example.com/one/oneMore/books/5",
                "http://www.example.com/one/books",
                "http://www.example.com/books"
            };

            var actualUrl = urlFilter.RemoveUnnecessary(testUrl, baseUri);

            Assert.Equal(expectedUrl.Count, actualUrl.Count);
            Assert.Collection(actualUrl,
                item => Assert.Equal(expectedUrl[0], item.AbsoluteUri),
                item => Assert.Equal(expectedUrl[1], item.AbsoluteUri),
                item => Assert.Equal(expectedUrl[2], item.AbsoluteUri),
                item => Assert.Equal(expectedUrl[3], item.AbsoluteUri));
        }

        [Fact]
        public void OnlyPathsForCurrentHostname()
        {
            var baseUri = new Uri("http://www.example.com");
            var testUrl = new List<string>
            {
                "/books/5",
                "http://www.other.com",
                "http://www.dev.example.com"
            };

            var expectedUrl = new List<string> {
                "/books/5"
            };

            var actualUrl = urlFilter.RemoveUnnecessary(testUrl, baseUri);

            Assert.Equal(expectedUrl.Count, actualUrl.Count);
            Assert.Collection(actualUrl,
                item => Assert.Equal(expectedUrl[0], item.PathAndQuery));
        }

        [Fact]
        public void OnlyPathsForCurrentSchema()
        {
            var baseUri = new Uri("http://www.example.com");
            var testUrl = new List<string>
            {
                "https://www.example.com",
                "file://www.example.com/temp/install_log.txt",
                "mailto:www.example.com",
                "ftp://www.example.com",
                "javascript:alert('');"
            };

            var actualUrl = urlFilter.RemoveUnnecessary(testUrl, baseUri);

            Assert.Equal(0, actualUrl.Count);
        }
        
    }
}
