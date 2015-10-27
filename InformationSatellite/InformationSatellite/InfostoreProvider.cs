using System;
using System.Collections.Generic;
using System.Linq;

using Data.Warehouse;

namespace InformationSatellite
{
    public class InfostoreProvider : StorageProvider
    {
        public IEnumerable<Dictionary<string, IEnumerable<string>>> DigInformation(string question)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Leitet die aus dem Web gelesenen Informationen an den Infostore weiter.
        /// </summary>
        /// <param name="information"></param>
        public void StoreInformation(Dictionary<string, IEnumerable<string>> information)
        {
            try
            {
                InformationSatellite.WarehouseClient.StoreInformation(information.ToDictionary(
                    entry => entry.Key, 
                    entry => entry.Value.ToArray()
                ));
            }
            catch (Exception) { }
        }
    }
}