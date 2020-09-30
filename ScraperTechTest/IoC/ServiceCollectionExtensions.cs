using Microsoft.Extensions.DependencyInjection;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.IO;
using System.Reflection;

namespace ScraperTechTest.IoC
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddChromeWebDriver(this IServiceCollection services)
        {
            ChromeOptions options = new ChromeOptions();
            options.AddArguments("--kiosk");
            var driver = new ChromeDriver(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), options);
            services.AddScoped<IWebDriver>(x => driver);

            return services;
        }
    }
}
