using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using ScraperTechTest.Extensions;
using ScraperTechTest.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ScraperTechTest.Scrapers.Pure
{
    public class PureScraper
    {
        public IEnumerable<Dish> Scrape(string menuUrl)
        {
            List<Dish> dishes = new List<Dish>();

            // TODO: inject driver
            ChromeOptions options = new ChromeOptions();
            options.AddArguments("--kiosk");
            using var driver = new ChromeDriver(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), options);
       
            driver.Navigate().GoToUrl(menuUrl);

          
            int subMenuItemsCount = driver.FindElementsByCssSelector(@"body > nav > ul:nth-child(2) > li:nth-child(2) > ul > li").Count();

            for (int i = 2; i <= subMenuItemsCount; i++)
            {
                string menuTitle = GetMenuTitle(driver, i);
                driver.FindElementsByCssSelector("body > nav > ul:nth-child(2) > li:nth-child(2) > ul > li").ElementAt(i-1).Click();

                string menuDescription = driver.FindElementByCssSelector("body > main > section > header > p").Text;
            }


            return dishes;
        }

        private static string GetMenuTitle(IWebDriver driver, int submenuIndex)
        {
            return driver.FindElementSafe(By.CssSelector($"body > nav > ul:nth-child(2) > li:nth-child(2) > ul > li:nth-child({submenuIndex}) > a")).Text;
        }
    }
}
