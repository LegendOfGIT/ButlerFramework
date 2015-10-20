using System.Collections.Generic;
using System.ServiceModel;
using System.Xml.Linq;

using Butler.Shared;

namespace Infosatellite
{
    [ServiceContract]
    public interface IInfosatellite
    {

        [OperationContract]
        void Process(string template, Dictionary<string, string> parameters);
    }

    public class Storage
    {
        public virtual void Store(KeyValuePair<string, Dictionary<string, List<string>>> kvpInfos, XDocument xdTemplate) { }
    }
}
