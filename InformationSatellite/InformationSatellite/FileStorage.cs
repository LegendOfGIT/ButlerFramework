using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Infosatellite
{
    public class FileStorage : Storage
    {
        private string location = string.Empty;

        public override void Store(KeyValuePair<string, Dictionary<string, List<string>>> kvpInfos, XDocument xdTemplate)
        {
            XAttribute aGet = null;

            aGet = xdTemplate == null ? null : xdTemplate.Root.Descendants("Settings").Elements("Store").Attributes("location").FirstOrDefault();
            location = aGet == null ? string.Empty : aGet.Value;

            if (Directory.Exists(location)){
                XElement xeRoot = new XElement("Informationen", new XAttribute("id", kvpInfos.Key));

                XDocument xdInfos = new XDocument(xeRoot);

                foreach (KeyValuePair<string, List<string>> kvp in kvpInfos.Value)
                {
                    XElement xeInfo = new XElement("Information", new XAttribute("id", kvp.Key));
                    foreach (string s in kvp.Value)
                        xeInfo.Add(new XElement("Eintrag", new XCData(s)));

                    xeRoot.Add(xeInfo);
                }

                try
                {
                    xdInfos.Save(location + kvpInfos.Key.Replace("/", "_") + ".xml");
                }
                catch (Exception) { }
            }
        }
    }
}