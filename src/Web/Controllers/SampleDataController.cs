using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks.Dataflow;
using BLL.Dtos;
using BLL.Abstractions;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace Web.Controllers
{
    [Route("api/[controller]")]
    public class SampleDataController : Controller
    {
        private readonly ISpiderService spiderService;

        public SampleDataController(ISpiderService spiderService)
        {
            this.spiderService = spiderService;
        }

        private static string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        [HttpGet("[action]")]
        public IEnumerable<WeatherForecast> WeatherForecasts()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                DateFormatted = DateTime.Now.AddDays(index).ToString("d"),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            });
        }

        public class WeatherForecast
        {
            public string DateFormatted { get; set; }
            public int TemperatureC { get; set; }
            public string Summary { get; set; }

            public int TemperatureF
            {
                get
                {
                    return 32 + (int)(TemperatureC / 0.5556);
                }
            }
        }

        [HttpGet("[action]")]
        public async Task Sse(string url)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var response = HttpContext.Response;
            response.ContentType = "text/event-stream";

            var blockingCollection = new BlockingCollection<RecordItemDto>(100);

            var task = spiderService.CrawlWebsiteAsync(url, blockingCollection);

            foreach (var item in blockingCollection.GetConsumingEnumerable())
            {
                await response
                    .WriteAsync($"data: requested {item.RequestUrl} at {item.RequestTime.Milliseconds} ms\r\r");

                response.Body.Flush();

                //await Task.Delay(1000);
            }

            await task;
            stopwatch.Stop();

            await response
                    .WriteAsync($"data: Total time spent {stopwatch.ElapsedMilliseconds} ms\r\r");
        }
    }
}
