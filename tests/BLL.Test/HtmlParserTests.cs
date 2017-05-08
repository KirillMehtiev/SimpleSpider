using System.Collections.Generic;
using Xunit;
using System.Linq;
using BLL.Implementation;

namespace BLL.Test
{
    public class HtmlParserTests
    {
        private readonly HtmlParser htmlParser;

        public HtmlParserTests()
        {
            htmlParser = new HtmlParser();
        }

        [Fact]
        public void FindsSingleUrl()
        {
            var url = "http://www.example.com";
            var page = $"<div><a href='{url}'>Link</a></div>";

            var urls = htmlParser.GetUrlsFromHtmlATag(page);

            Assert.Single(urls);
            Assert.Equal(url, urls.FirstOrDefault());
        }

        [Fact]
        public void FindsExternalUrlToPages()
        {
            var externalUrls = new List<string> { "https://www.example.com", "http://example.com" };
            var page = "My favorite web sites include:</p>" +
                          $"<a href='{externalUrls[0]}'>Link text</a></p>" +
                          $"<A HREF=\"{externalUrls[1]}\">" +
                          "example</A></p>";

            var urls = htmlParser.GetUrlsFromHtmlATag(page);

            Assert.Equal(externalUrls.Count, urls.Count);
            Assert.Collection(urls,
                url => Assert.Equal(externalUrls[0], url),
                url => Assert.Equal(externalUrls[1], url));

        }

        [Fact]
        public void FindesInternalUrlToPages()
        {
            var internalUrls = new List<string> { "/", "/books", "/books/1", "/books/1#description" };
            var page =
                "<div>" +
                    $"<a href='{internalUrls[0]}'>Home</a>" +
                    "<div>" +
                        $"<a href='{internalUrls[1]}'></a>" +
                        $"<a href='{internalUrls[2]}'></a>" +
                        $"<a href='{internalUrls[3]}'></a>" +
                    "</div>" +
                "</div>";

            var urls = htmlParser.GetUrlsFromHtmlATag(page);

            Assert.Equal(internalUrls.Count, urls.Count);
            Assert.Collection(urls,
                url => Assert.Equal(internalUrls[0], url),
                url => Assert.Equal(internalUrls[1], url),
                url => Assert.Equal(internalUrls[2], url),
                url => Assert.Equal(internalUrls[3], url));
        }

        [Fact]
        public void DoesNotFoundUrlsIfNoUrls()
        {
            var page = "<b>No urls</b>";

            var urls = htmlParser.GetUrlsFromHtmlATag(page);

            Assert.Equal(0, urls.Count);
        }

        [Fact]
        public void HandleUnvalideHtmlATags()
        {
            var page = "<a>Unclosed a tag" +
                       "<a href=''>Empty href attribute</a>" +
                       "<a>Without href attribute</a>" +
                       "<a href></a>";

            var ex = Record.Exception(() => htmlParser.GetUrlsFromHtmlATag(page));

            Assert.Null(ex);
        }
    }
}
