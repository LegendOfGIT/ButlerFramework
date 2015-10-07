using System.Collections.Generic;

namespace Data.Warehouse
{
    public interface StorageProvider
    {
        IEnumerable<Dictionary<string, IEnumerable<string>>> DigInformation(string question);
        void StoreInformation(Dictionary<string, IEnumerable<string>> information);
    }
}
