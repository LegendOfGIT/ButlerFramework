using System;
using System.Collections.Generic;
using Data.Warehouse;

namespace SatelliteTest
{
    public class ProviderMock : DataWarehouseProvider
    {
        public IEnumerable<Dictionary<string, IEnumerable<string>>> DigInformation(string question)
        {
            throw new NotImplementedException();
        }

        public void StoreInformation(Dictionary<string, IEnumerable<string>> information)
        {
            information = information.PrepareInformation();
        }
    }
}
