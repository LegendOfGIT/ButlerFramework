using System;
using System.Collections.Generic;
using System.Linq;
using Data.Warehouse;

namespace SatelliteTest
{
    public class ProviderMock : StorageProvider
    {
        public IEnumerable<Dictionary<string, IEnumerable<string>>> DigInformation(string question)
        {
            throw new NotImplementedException();
        }

        public void StoreInformation(Dictionary<string, IEnumerable<string>> information)
        {
            information = information.ToDictionary(entry => entry.Key, entry => entry.Value.Select(e => new string(e.Where(c => !char.IsControl(c)).ToArray()).Trim()));
        }
    }
}
