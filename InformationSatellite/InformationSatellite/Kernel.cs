﻿using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Data.Warehouse.Crawler;

namespace Infosatellite
{
    public class Kernel
    {
        public static void Process(string template, Dictionary<string, string> parameters)
        {
            if (!string.IsNullOrEmpty(template))
            {
                var compiler = new WebcrawlerCompiler();
                var commandset = compiler.ParseCommandset(Regex.Split(template, Environment.NewLine));
                
                new WebcrawlingUtility().Crawling(commandset);
            }
        }
    }
}