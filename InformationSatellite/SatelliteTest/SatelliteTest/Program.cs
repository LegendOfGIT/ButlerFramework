using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Data.Warehouse.Crawler;

namespace SatelliteTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var template = File.ReadAllText(@"..\..\App_Data\shopping\mytoys.crawl");

            ExecuteSatelliteTest(template);

            //ExecuteMockTest(template);
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

        public static void DecimalTest()
        {
            var numbers = new[]{
                "12,45",
                "150,55",
                "2.400,12",
                "3.400",

                "12.45",
                "150.55",
                "2,400.12",
                "3,400"
            };

            numbers.ToList().ForEach(number => {
                var @decimal = default(decimal);

                var matches = default(MatchCollection);
                var n = number;
                var separators = @"(,|\.)";
                matches = Regex.Matches(number, separators);
                if (matches != null && matches.Count <= 1)
                {
                }

                decimal.TryParse(
                    n,
                    System.Globalization.NumberStyles.Number,
                    System.Globalization.CultureInfo.GetCultureInfo("de-DE"),
                    out @decimal
                );

                Console.WriteLine(number + " >> " + @decimal);
            });
        }
    }
}
