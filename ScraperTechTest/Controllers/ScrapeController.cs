using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using ScraperTechTest.Model;
using ScraperTechTest.Scrapers;
using ScraperTechTest.Scrapers.Pure;
using System.Collections.Generic;

namespace ScraperTechTest.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ScrapeController : ControllerBase
    {
        private readonly IWebDriver _webDriver;
        private readonly IScraper _pureScraper;
        private readonly ILogger<ScrapeController> _logger;

        public ScrapeController(IWebDriver webDriver, PureScraper pureScraper, ILogger<ScrapeController> logger)
        {
            _webDriver = webDriver;
            _pureScraper = pureScraper;
            _logger = logger;
        }

        [HttpPost]
        public IEnumerable<Dish> Post([FromBody]ScrapeRequest scrapeRequest)
        {
            _logger.LogInformation($"Request for scraping received. Menu URL: {scrapeRequest.MenuUrl}");
            // If we have multiple scrapers logic for picking one should go here.
            // Now we use _pureScraper directly
            var dishes = _pureScraper.Scrape(scrapeRequest.MenuUrl, _webDriver);

            return dishes;
        }
    }
}
