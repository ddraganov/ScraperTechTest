using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using ScraperTechTest.Extensions;
using ScraperTechTest.Model;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

namespace ScraperTechTest.Scrapers.Pure
{
    public class PureScraper : IScraper
    {
        private ILogger<PureScraper> _logger;

        public PureScraper(ILogger<PureScraper> logger)
        {
            _logger = logger;
        }

        public IEnumerable<Dish> Scrape(string menuUrl, IWebDriver driver)
        {
            var dishes = ScrapeMenuPageInfo(menuUrl, driver);
            PupulateDishDescriptions(dishes, driver);

            return dishes;
        }

        private IEnumerable<Dish> ScrapeMenuPageInfo(string menuUrl, IWebDriver driver)
        {
            driver.Navigate().GoToUrl(menuUrl);

            int subMenuItemsCount = GetSubmenuItems(driver).Count();

            var dishes = new List<Dish>();

            // Iterate over menus
            // Start from 2 to skip Wellbeing boxes
            for (int i = 2; i <= subMenuItemsCount; i++)
            {
                GetSubmenuItems(driver).ElementAt(i - 1).Click();

                string menuTitle = GetMenuTitle(driver, i);
                _logger.LogInformation($"Reading {menuTitle} page");
                string menuDescription = GetMenuDescription(driver);

                // Iterate over sections
                var menuSectionsTitleElements = driver.FindElements(By.CssSelector("h4.menu-title"));
                foreach (var sectionTitleElement in menuSectionsTitleElements)
                {
                    string menuSectionTitle = sectionTitleElement.Text;
                    _logger.LogInformation($"Reading {menuSectionTitle} section");
                    dishes.AddRange(GetDishesInSection(sectionTitleElement, driver, menuTitle, menuDescription, menuSectionTitle));
                }
            }

            return dishes;
        }

        private static ReadOnlyCollection<IWebElement> GetSubmenuItems(IWebDriver driver)
        {
            return driver.FindElements(By.CssSelector(@"body > nav > ul:nth-child(2) > li:nth-child(2) > ul > li"));
        }

        private IEnumerable<Dish> GetDishesInSection(IWebElement sectionTitleElement, IWebDriver driver, string menuTitle, string menuDescription, string menuSectionTitle)
        {
            var dishesInSection = new List<Dish>();

            try
            {
                foreach (var dishDisplaying in GetDishDisplayings(sectionTitleElement, driver))
                {
                    dishesInSection.Add(new Dish
                    {
                        MenuTitle = menuTitle,
                        MenuDescription = menuDescription,
                        MenuSectionTitle = menuSectionTitle,
                        DishName = dishDisplaying.GetAttribute("title"),
                        DishPage = dishDisplaying.GetAttribute("href")
                    });
                    _logger.LogInformation($"{dishesInSection.Last().DishName} just collected");
                }
            }
            catch
            {
                _logger.LogError($"Error ocurred while scraping main menu page");
            }

            return dishesInSection;
        }

        private void PupulateDishDescriptions(IEnumerable<Dish> dishes, IWebDriver driver)
        {
            foreach (var dish in dishes)
            {
                driver.Navigate().GoToUrl(dish.DishPage);
                try
                {
                    dish.DishDescription = driver.FindElement(By.CssSelector(".menu-item-details > div:nth-child(3)")).Text;
                    _logger.LogInformation($"Dish description successfully scraped from {dish.DishPage}");
                }
                catch
                {
                    // TODO: still don't know what to do with this. Waiting for response
                    _logger.LogError($"Failed to read {dish.DishPage}");
                }
            }
        }

        private string GetMenuTitle(IWebDriver driver, int submenuIndex)
        {
            return driver.FindElementSafe(By.CssSelector($"body > nav > ul:nth-child(2) > li:nth-child(2) > ul > li:nth-child({submenuIndex}) > a")).Text;
        }

        private string GetMenuDescription(IWebDriver driver)
        {
            return driver.FindElement(By.CssSelector("body > main > section > header > p")).Text;
        }

        private IEnumerable<IWebElement> GetDishDisplayings(IWebElement sectionTitleElement, IWebDriver driver)
        {
            string dishId = sectionTitleElement.FindElement(By.CssSelector("a")).GetAttribute("aria-controls");
            var dishDisplayingsSection = driver.FindElement(By.Id(dishId));

            return dishDisplayingsSection.FindElements(By.CssSelector("a[href]"));
        }
    }
}
