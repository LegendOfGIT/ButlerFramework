using System;
using System.Collections.Generic;

using Data.Warehouse;

namespace Infosatellite
{
    public class InfostoreProvider : StorageProvider
    {
        public IEnumerable<Dictionary<string, IEnumerable<string>>> DigInformation(string question)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Leitet die aus dem Web gelesenenen Informationen an den Infostore weiter.
        /// </summary>
        /// <param name="information"></param>
        public void StoreInformation(Dictionary<string, IEnumerable<string>> information)
        {
            
        }
    }
}