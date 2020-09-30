using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScraperTechTest.Extensions
{
    public static class WebElementExtensions
    {
        public static IWebElement FindElementSafe(this IWebDriver driver, By by)
        {

            if (driver.FindElements(by).Any())
            {
                return driver.FindElement(by);
            }
            else
            { 
                return null; 
            }
        }
    }
}
