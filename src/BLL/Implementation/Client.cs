using System.Net.Http;
using System.Threading.Tasks;
using BLL.Abstractions;

namespace BLL.Implementation
{
    public class Client : IClient
    {
        public static readonly HttpClient _client = new HttpClient();

        public async Task<HttpResponseMessage> GetAsync(string url)
        {
            return await _client.GetAsync(url);
        }
    }

}
