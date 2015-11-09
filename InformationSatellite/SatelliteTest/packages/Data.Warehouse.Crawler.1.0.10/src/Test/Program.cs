using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Data.Warehouse.Crawler;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var templatesfiles = new[]
            {
                @"G:\Entwicklung\GitHUB\ButlerFramework\InformationSatellite\SatelliteTest\SatelliteTest\App_Data\shopping\mytoys.crawl",
                //@"..\..\App_Data\chefkoch.crawl",
                //@"..\..\App_Data\stackoverflow.crawl",
                //@"..\..\App_Data\fun\9gag.crawl"
            };
            templatesfiles.ToList().ForEach(templatefile =>
            {
                var template = File.ReadAllText(templatefile);

                var compiler = new WebcrawlerCompiler();
                var commandset = compiler.ParseCommandset(Regex.Split(template, Environment.NewLine));
            });
        }
    }
}
