using System.Net.Http;
using System.Threading.Tasks;

namespace BLL.Abstractions
{
    public interface IClient
    {
        Task<HttpResponseMessage> GetAsync(string url);
    }
}
