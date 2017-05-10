using BLL.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BLL.Implementation
{
    /// <summary>
    /// Helper for filtering URLs
    /// </summary>
    public class UrlFilter : IUrlFilter
    {
        /// <summary>
        /// Remove all URLs which do not refer to current website pages
        /// </summary>
        /// <param name="urls">URLs to be filtered</param>
        /// <param name="baseUri">Base URL for URLs</param>
        /// <returns>Filtered URLs which refer to website's pages</returns>
        public ICollection<Uri> RemoveUnnecessary(ICollection<string> urls, Uri baseUri)
        {
            var result = new List<Uri>();

            foreach (var url in urls)
            {
                if (string.IsNullOrEmpty(url))
                    continue;

                if (!Uri.IsWellFormedUriString(url, UriKind.RelativeOrAbsolute))
                    continue;

                Uri.TryCreate(baseUri, url, out Uri currentUri);

                if (currentUri == null)
                    continue;

                if (currentUri.PathAndQuery.Contains('#'))
                    continue;

                if (currentUri.Host != baseUri.Host)
                    continue;

                if (currentUri.Scheme != baseUri.Scheme)
                    continue;

                result.Add(currentUri);
            }

            return result;
        }

    }
}
