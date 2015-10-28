using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;

using Data.Warehouse;

namespace InformationWarehouse
{
    [ServiceContract]
    public class InformationWarehouse : StorageProvider
    {
        private StorageProvider WarehouseProvider = default(StorageProvider);

        [OperationContract]
        public IEnumerable<Dictionary<string, IEnumerable<string>>> DigInformation(string question)
        {
            throw new NotImplementedException();
        }
        [OperationContract]
        public void StoreInformation(Dictionary<string, IEnumerable<string>> information)
        {
            //  Vorbereitung der einzuspeisenden Informationen
            information = information?.ToDictionary(
                entry => {
                    var key = entry.Key;

                    var tokens = entry.Key.Split('.');
                    key = tokens?.Length > 1 ? string.Join(".", tokens.Skip(1)) : key;

                    return key;
                },
                entry => entry.Value
            );

            //this.WarehouseProvider = new FilesystemStorageProvider(@"C:\Temp\Github\ButlerFramework\InformationWarehouse\InformationWarehouse\App_Data\Warehouse");
            this.WarehouseProvider = new MongoWarehouseProvider();
            this.WarehouseProvider.StoreInformation(information);
        }
    }
}
