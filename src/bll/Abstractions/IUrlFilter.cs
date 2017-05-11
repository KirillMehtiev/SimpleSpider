using System;
using System.Collections.Generic;

namespace BLL.Abstractions
{
    /// <summary>
    /// Helper for filtering URLs
    /// </summary>
    public interface IUrlFilter
    {
        /// <summary>
        /// Remove all URLs which do not refer to current website pages
        /// </summary>
        /// <param name="urls">URLs to be filtered</param>
        /// <param name="baseUri">Base URL for URLs</param>
        /// <returns>Filtered URLs which refer to website's pages</returns>
        ICollection<Uri> RemoveUnnecessary(ICollection<string> urls, Uri baseUri);
    }
}
