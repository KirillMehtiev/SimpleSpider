using System;
using System.Collections.Generic;

namespace BLL.Abstractions
{
    public interface IUrlFilter
    {
        ICollection<Uri> RemoveUnnecessary(ICollection<string> urls, Uri baseUri);
    }
}
