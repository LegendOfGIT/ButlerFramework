﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using Data.Mining;
using Data.Web;

namespace Data.Warehouse
{
    public class DataWarehouseFilesystemStorageProvider : DataWarehouseProvider
    {
        private string Storagefolder = Path.Combine("../../", "CrawlingStorage");

        public DataWarehouseFilesystemStorageProvider(string storeagefolder = null)
        {
            this.Storagefolder = storeagefolder;
        }

        public IEnumerable<Dictionary<string, IEnumerable<string>>> DigInformation(string question)
        {
            var diggingresult = default(IEnumerable<Dictionary<string, IEnumerable<string>>>);

            var querykey = string.Empty;
            var querys = MiningCompiler.CompileQuerys(question);
            var matches = new List<string>(Directory.GetFiles(Storagefolder));
            if (querys != null)
            {
                querykey = string.Join(".", new[] { querykey, $"{querys.Target} {string.Join(" or", querys?.Expressions)}" });
                querykey = querykey.GetHashCode().ToString();

                var cachefile = Path.Combine(Storagefolder, querykey, ".cache");
                matches = File.Exists(cachefile) ? new List<string>(File.ReadAllLines(cachefile)) : matches;

                if (matches != null && matches.Any())
                {
                    diggingresult = Enumerable.Empty<Dictionary<string, IEnumerable<string>>>();

                    //  Rückübersetzung der Treffer in Trefferdictionary
                    foreach(var match in matches) 
                    {
                        var information = RetrieveInformation(match);
                        //if(IsMatchingInformation(querys, information))
                        //{ 
                        //    diggingresult = diggingresult.Concat(new[] { information });
                        //}
                    }
                }
            }

            return diggingresult;
        }
        private bool IsMatchingInformation(KeyValuePair<string, IEnumerable<string>> query, Dictionary<string, IEnumerable<string>> information)
        {
            var isMatch = default(bool);

            var queryValueset = query.Value;
            var targetValueset = (
                information != null ?
                information.FirstOrDefault(entry => entry.Key.EndsWith($".{query.Key}")) : 
                new KeyValuePair<string, IEnumerable<string>>()
            ).Value;

            isMatch = queryValueset.Any(queryValue => targetValueset.Any(targetValue => Regex.IsMatch(Regex.Replace(targetValue.ToLower(), @"(\(|\))", string.Empty), queryValue.ToLower())));

            return isMatch;
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
            information = information.PrepareInformation();

            var id = information.GetId();
            if(!string.IsNullOrEmpty(id))
            {
                var content = new StringBuilder();

                foreach(var entry in information)
                {
                    content.AppendLine($"{entry.Key} = {string.Join("|", entry.Value)}");
                }

                File.WriteAllText(
                    Path.Combine(Storagefolder, $"{id}.crawl"),
                    content.ToString()
                );
            }
        }
    }
}
