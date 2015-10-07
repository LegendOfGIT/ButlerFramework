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
                    var matches = new List<string>();
                    //  
                    
                }
            }

            return diggingresult;
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
