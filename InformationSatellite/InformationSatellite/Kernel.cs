using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Data.Warehouse.Crawler;

namespace InformationSatellite
{
    public class Kernel
    {
        public static void Process(string template, Dictionary<string, string> parameters)
        {
            if (!string.IsNullOrEmpty(template))
            {
                var compiler = new WebcrawlerCompiler();
                var commandset = compiler.ParseCommandset(Regex.Split(template, Environment.NewLine));                

                var crawler = new WebcrawlingUtility(new InfostoreProvider());
                crawler.ContextCommandset = compiler.ContextCommandset;
                crawler.Crawling(commandset);
            }
        }
    }
}