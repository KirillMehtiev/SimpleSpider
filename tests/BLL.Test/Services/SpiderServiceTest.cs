using BLL.Abstractions;
using Xunit;

namespace BLL.Test.Services
{
    public class SpiderServiceTest
    {
        private readonly ISpiderService _spiderService;

        public SpiderServiceTest(ISpiderService spiderService)
        {
            _spiderService = spiderService;
        }

        [Fact]
        public void GetCrawlWebsiteInformation()
        {

        }
    }
}
