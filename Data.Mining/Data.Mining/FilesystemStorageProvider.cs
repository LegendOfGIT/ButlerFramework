using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Data.Web;

namespace Data.Warehouse
{
    public class FilesystemStorageProvider : StorageProvider
    {
        public IEnumerable<Dictionary<string, IEnumerable<string>>> DigInformation(Dictionary<string, IEnumerable<string>> information)
        {
            var diggingresult = default(IEnumerable<Dictionary<string, IEnumerable<string>>>);

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
                    Path.Combine("../../", "CrawlingStorage", $"{id}.crawl"),
                    content.ToString()
                );
            }
        }
    }
}
