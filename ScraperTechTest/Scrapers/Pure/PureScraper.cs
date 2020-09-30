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
            // TODO: inject driver
            ChromeOptions options = new ChromeOptions();
            options.AddArguments("--kiosk");
            using var driver = new ChromeDriver(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), options);

            var dishes = ScrapeMenuPageInfo(menuUrl, driver);
            PupulateDishDescriptions(dishes, driver);

            return dishes;
        }

        private IEnumerable<Dish> ScrapeMenuPageInfo(string menuUrl, IWebDriver driver)
        {
            driver.Navigate().GoToUrl(menuUrl);

            int subMenuItemsCount = driver.FindElements(By.CssSelector(@"body > nav > ul:nth-child(2) > li:nth-child(2) > ul > li")).Count();

            var dishes = new List<Dish>();
            for (int i = 2; i <= subMenuItemsCount; i++)
            {
                //var dish = new Dish();
                string menuTitle = GetMenuTitle(driver, i);
                driver.FindElements(By.CssSelector("body > nav > ul:nth-child(2) > li:nth-child(2) > ul > li")).ElementAt(i - 1).Click();

                string menuDescription = driver.FindElement(By.CssSelector("body > main > section > header > p")).Text;

                var menuSectionsTitleElements = driver.FindElements(By.CssSelector("h4.menu-title"));
                foreach (var sectionTitleElement in menuSectionsTitleElements)
                {
                    string menuSectionTitle = sectionTitleElement.Text;
                    string f = sectionTitleElement.FindElement(By.CssSelector("a")).GetAttribute("aria-controls");

                    var dishDisplayingsSection = driver.FindElement(By.Id(f));
                    var dishDisplayings = dishDisplayingsSection.FindElements(By.CssSelector("a[href]"));
                    foreach (var dishDisplaying in dishDisplayings)
                    {
                        dishes.Add(new Dish
                        {
                            MenuTitle = menuTitle,
                            MenuDescription = menuDescription,
                            MenuSectionTitle = menuSectionTitle,
                            DishName = dishDisplaying.GetAttribute("title"),
                            DishPage = dishDisplaying.GetAttribute("href")
                        });
                    }
                }
            }

            return dishes;
        }

        private void PupulateDishDescriptions(IEnumerable<Dish> dishes, IWebDriver driver)
        {
            foreach (var dish in dishes)
            {
                driver.Navigate().GoToUrl(dish.DishPage);
                try
                {
                    dish.DishDescription = driver.FindElement(By.CssSelector(".menu-item-details > div:nth-child(3) > p")).Text;
                }
                catch
                {
                    Console.WriteLine(dish.DishPage);
                }
            }
        }

        private string GetMenuTitle(IWebDriver driver, int submenuIndex)
        {
            return driver.FindElementSafe(By.CssSelector($"body > nav > ul:nth-child(2) > li:nth-child(2) > ul > li:nth-child({submenuIndex}) > a")).Text;
        }
    }
}
