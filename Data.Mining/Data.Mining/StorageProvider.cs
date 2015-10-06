using System.Collections.Generic;

namespace Data.Warehouse
{
    public interface StorageProvider
    {
        IEnumerable<Dictionary<string, IEnumerable<string>>> DigInformation(Dictionary<string, IEnumerable<string>> information);
        void StoreInformation(Dictionary<string, IEnumerable<string>> information);
    }
}
