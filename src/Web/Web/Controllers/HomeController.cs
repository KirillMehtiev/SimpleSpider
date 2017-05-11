using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BLL.Dtos;
using BLL.Abstractions;

namespace Web.Controllers
{
    public class HomeController : Controller
    {
        public readonly ISpiderService spider;

        public HomeController(ISpiderService spider)
        {
            this.spider = spider;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Crawl(string url)
        {
            var result = await spider.CrawlWebsiteAsync(url);

            return View(result);
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
