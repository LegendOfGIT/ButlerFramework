using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new InformationWarehouse.InformationWarehouseClient();
            var information = client.DigInformation("Kleidung bis 20 EUR");
        }
    }
}
