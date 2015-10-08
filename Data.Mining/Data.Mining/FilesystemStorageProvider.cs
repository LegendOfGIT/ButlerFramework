using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Data.Mining;
using Data.Web;

namespace Data.Warehouse
{
    public class FilesystemStorageProvider : StorageProvider
    {
        private string storagefolder = Path.Combine("../../", "CrawlingStorage");

        public IEnumerable<Dictionary<string, IEnumerable<string>>> DigInformation(string question)
        {
            var diggingresult = default(IEnumerable<Dictionary<string, IEnumerable<string>>>);

            var querykey = string.Empty;
            var querys = MiningCompiler.CompileQuerys(question);
            if(querys.Any())
            {
                foreach (var query in querys)
                {
                    querykey = string.Join(".", new[] { querykey, query.Key + " " + string.Join(" or", query.Value) });
                    querykey = querykey.GetHashCode().ToString();

                    var cachefile = Path.Combine(storagefolder, querykey, ".cache");
                    var matches = File.Exists(cachefile) ? new List<string>(File.ReadAllLines(cachefile)) : new List<string>();
                    //  Kein (gecachtes Ergebnis) >> Suche nach passenden Ergebnissen
                    if (matches == null || !matches.Any())
                    {

                    }

                    matches = new List<string>
                    {
                        @"C:\Temp\ButlerFrameworkGIT\Data.Mining\SharpQuery\ConsoleApplication1\ConsoleApplication1\CrawlingStorage\www.chefkoch.de.639161164875917.crawl",
                        @"C:\Temp\ButlerFrameworkGIT\Data.Mining\SharpQuery\ConsoleApplication1\ConsoleApplication1\CrawlingStorage\www.chefkoch.de.2007591325498506.crawl"
                    };
                    if(matches != null)
                    {
                        diggingresult = Enumerable.Empty<Dictionary<string, IEnumerable<string>>>();

                        //  Rückübersetzung der Treffer in Trefferdictionary
                        matches.ForEach(match =>
                        {
                            diggingresult = diggingresult.Concat(new[] { RetrieveInformation(match) });
                        });
                    }
                }
            }

            return diggingresult;
        }
        private Dictionary<string, IEnumerable<string>> RetrieveInformation(string informationfile)
        {
            var information = default(Dictionary<string, IEnumerable<string>>);

            if(File.Exists(informationfile))
            {
                information = new Dictionary<string, IEnumerable<string>>();
                var lines = File.ReadAllLines(informationfile);
                lines.ToList().ForEach(line => {
                    var tokens = default(IEnumerable<string>);

                    tokens = line.Split('=');
                    var key = (tokens.FirstOrDefault() ?? string.Empty).Trim();
                    var values = (tokens.Skip(1).FirstOrDefault() ?? string.Empty).Trim().Split('|');
                    if(key != null)
                    {
                        information[key] = values;
                    }
                });
            }

            return information;
        }
        public void StoreInformation(Dictionary<string, IEnumerable<string>> information)
        {
            var ids = information != null ? information.FirstOrDefault(entry => entry.Key.ToLower().EndsWith(".id")) : default(KeyValuePair<string, IEnumerable<string>>);
            var id = ids.Value?.FirstOrDefault() ?? string.Empty;
            if(!string.IsNullOrEmpty(id))
            {
                var content = new StringBuilder();

                foreach(var entry in information)
                {
                    content.AppendLine($"{entry.Key} = {string.Join("|", entry.Value.Select(v => new string(v.RemoveHTML().Where(c => !char.IsControl(c)).ToArray()).Trim()))}");
                }

                File.WriteAllText(
                    Path.Combine(storagefolder, $"{id}.crawl"),
                    content.ToString()
                );
            }
        }
    }
}
