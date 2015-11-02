using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;

using Data.Warehouse;

namespace InformationWarehouse
{
    [ServiceContract]
    public class InformationWarehouse : DataWarehouseProvider
    {
        private DataWarehouseProvider WarehouseProvider = default(DataWarehouseProvider);

        [OperationContract]
        public IEnumerable<Dictionary<string, IEnumerable<object>>> DigInformation(string question)
        {
            throw new NotImplementedException();
        }
        [OperationContract]
        public void StoreInformation(Dictionary<string, IEnumerable<object>> information)
        {
            //  Vorbereitung der einzuspeisenden Informationen
            information = information?.PrepareInformation();

            //this.WarehouseProvider = new FilesystemStorageProvider(@"C:\Temp\Github\ButlerFramework\InformationWarehouse\InformationWarehouse\App_Data\Warehouse");
            this.WarehouseProvider = new MongoWarehouseProvider();
            this.WarehouseProvider.StoreInformation(information);
        }
    }
}
