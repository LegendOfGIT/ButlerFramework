using System.IO;

namespace SatelliteTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var template = File.ReadAllText(@"..\..\App_Data\gelbeseiten.crawl");

            var client = new InformationSatellite.InfosatelliteClient();
            client.Process(template, null);
        }
    }
}
