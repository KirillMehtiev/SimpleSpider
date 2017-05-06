using System;
using System.Collections.Generic;
using System.Text;

namespace BLL.Abstractions
{
    public interface ICaller
    {
        string Call(string url);
    }
}
