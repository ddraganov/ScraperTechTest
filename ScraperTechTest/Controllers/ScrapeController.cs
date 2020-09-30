using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ScraperTechTest.Model;
using ScraperTechTest.Scrapers.Pure;

namespace ScraperTechTest.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ScrapeController : ControllerBase
    {
        [HttpPost]
        public IEnumerable<Dish> Post([FromBody]ScrapeRequest scrapeRequest)
        {
            PureScraper x = new PureScraper();
            x.Scrape(scrapeRequest.MenuUrl);

            return new List<Dish>() { new Dish { DishName = "Test Dish" } };
        }
    }
}
