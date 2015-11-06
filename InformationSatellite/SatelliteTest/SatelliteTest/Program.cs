using System;
using System.IO;
using System.Text.RegularExpressions;

using Data.Warehouse.Crawler;

namespace SatelliteTest
{
    class Program
    {
        static void Main(string[] args)
        {
            //var template = File.ReadAllText(@"..\..\App_Data\shopping\mytoys.crawl");
            //var template = File.ReadAllText(@"..\..\App_Data\chefkoch.crawl");
            //var template = File.ReadAllText(@"..\..\App_Data\stackoverflow.crawl");
            var template = File.ReadAllText(@"..\..\App_Data\fun\9gag.crawl");

            //ExecuteSatelliteTest(template);

            ExecuteMockTest(template);
        }

        private static void ExecuteSatelliteTest(string template)
        {
            var client = new InformationSatellite.InformationSatelliteClient();
            client.Process(template, null);
        }
        private static void ExecuteMockTest(string template)
        {
            var compiler = new WebcrawlerCompiler();
            var commandset = compiler.ParseCommandset(Regex.Split(template, Environment.NewLine));

            var crawler = new WebcrawlingUtility(new ProviderMock());
            crawler.ContextCommandset = compiler.ContextCommandset;
            crawler.Crawling(commandset);
        }
    }
}
