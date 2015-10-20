using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infosatellite
{
    public class Infosattelite : IInfosatellite
    {
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
