using OpenQA.Selenium;
using ScraperTechTest.Model;
using System.Collections.Generic;

namespace ScraperTechTest.Scrapers
{
    public interface IScraper
    {
        IEnumerable<Dish> Scrape(string menuUrl, IWebDriver driver);
    }
}
