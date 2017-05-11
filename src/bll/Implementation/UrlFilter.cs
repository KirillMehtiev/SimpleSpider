using BLL.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BLL.Implementation
{
    public class UrlFilter : IUrlFilter
    {
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
