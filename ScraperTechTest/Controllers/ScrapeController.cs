using Microsoft.AspNetCore.Mvc;
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

        public ScrapeController(IWebDriver webDriver, PureScraper pureScraper)
        {
            _webDriver = webDriver;
            _pureScraper = pureScraper;
        }

        [HttpPost]
        public IEnumerable<Dish> Post([FromBody]ScrapeRequest scrapeRequest)
        {
            // If we have multiple scrapers logic for picking one should go here.
            // Now we use _pureScraper directly
            var dishes = _pureScraper.Scrape(scrapeRequest.MenuUrl, _webDriver);

            return dishes;
        }
    }
}
