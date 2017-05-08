using System.Collections.Generic;
using BLL.Abstractions;
using System.Text.RegularExpressions;

namespace BLL.Implementation
{
    public class HtmlParser : IHtmlParser
    {
        private const string URL = "URL";

        public ICollection<string> GetUrlsFromHtmlATag(string page)
        {
            var links = GetAllHtmlATags(page);
            var urls = new List<string>();

            foreach (Match link in links)
            {
                var url = link.Groups[URL].Value;
                urls.Add(url);
            }

            return urls;
        }

        private MatchCollection GetAllHtmlATags(string page)
        {
            // regex description: http://stackoverflow.com/questions/8066248/regex-to-get-href-and-src-from-html-content
            var regex = $@"<(?<Tag_Name>(a))\b[^>]*?\b(?<URL_Type>(?(1)href))\s*=\s*(?:""(?<{URL}>(?:\\""|[^""])*)""|'(?<{URL}>(?:\\'|[^'])*)')";

            var matches = Regex.Matches(page, regex, RegexOptions.IgnoreCase | RegexOptions.Singleline);

            return matches;
        }
    }
}
