using System.Collections.Generic;
using System.Threading.Tasks;

using Butler.Shared;

namespace Infosatellite
{
    // HINWEIS: Mit dem Befehl "Umbenennen" im Menü "Umgestalten" können Sie den Klassennamen "Service1" sowohl im Code als auch in der SVC- und der Konfigurationsdatei ändern.
    public class Infosattelite : IInfosatellite
    {
        public void Process(string template, Dictionary<string, string> parameters)
        {
            new Task(
                () => {
                    new Kernel {
                    }.Process(
                        template, 
                        parameters
                    );
                }
            ).Start();
        }
    }
}
