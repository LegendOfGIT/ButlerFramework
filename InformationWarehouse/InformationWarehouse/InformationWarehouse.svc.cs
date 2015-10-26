using System;
using System.Collections.Generic;
using System.ServiceModel;
using Data.Warehouse;

namespace InformationWarehouse
{
    [ServiceContract]
    public class InformationWarehouse : StorageProvider
    {
        [OperationContract]
        public IEnumerable<Dictionary<string, IEnumerable<string>>> DigInformation(string question)
        {
            throw new NotImplementedException();
        }
        [OperationContract]
        public void StoreInformation(Dictionary<string, IEnumerable<string>> information)
        {
            throw new NotImplementedException();
        }
    }
}
