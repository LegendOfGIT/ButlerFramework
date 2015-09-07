using System.Collections.Generic;

namespace Data.Mining
{
    interface StorageProvider
    {
        void StoreInformation(Dictionary<string, IEnumerable<string>> information);
    }
}
