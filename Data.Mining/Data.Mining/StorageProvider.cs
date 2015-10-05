using System.Collections.Generic;

namespace Data.Warehouse
{
    public interface StorageProvider
    {
        void StoreInformation(Dictionary<string, IEnumerable<string>> information);
    }
}
