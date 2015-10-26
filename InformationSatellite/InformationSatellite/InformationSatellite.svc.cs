using System.Collections.Generic;
using System.Threading.Tasks;

using InformationSatellite.DataWarehouse;

namespace InformationSatellite
{
    public class InformationSatellite : IInformationSatellite
    {
        public static InformationWarehouseClient WarehouseClient = new InformationWarehouseClient();

        public void Process(string template, Dictionary<string, string> parameters)
        {               
            new Task(
                () => {
                    Kernel.Process(
                        template, 
                        parameters
                    );
                }
            ).Start();
        }
    }
}
