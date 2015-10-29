﻿using System.IO;

namespace SatelliteTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var template = File.ReadAllText(@"..\..\App_Data\chefkoch.crawl");

            var client = new InformationSatellite.InformationSatelliteClient();
            client.Process(template, null);
        }
    }
}
