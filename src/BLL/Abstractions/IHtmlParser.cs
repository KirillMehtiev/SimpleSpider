﻿using System;
using System.Collections.Generic;
using System.Text;

namespace BLL.Abstractions
{
    public interface IHtmlParser
    {
        ICollection<string> GetUrlsFromHtmlATag(string page);
    }
}
