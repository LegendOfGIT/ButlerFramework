using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Data.Mining
{
    public class FilesystemStorageProvider : StorageProvider
    {
        public void StoreInformation(Dictionary<string, IEnumerable<string>> information)
        {
            var bigdataInformation = new BigDataCompiler().Compile(information);

            var ids = bigdataInformation != null ? bigdataInformation.FirstOrDefault(entry => entry.Key.ToLower().EndsWith(".id")) : default(KeyValuePair<string, IEnumerable<string>>);
            var id = ids.Value?.FirstOrDefault() ?? string.Empty;
            if(!string.IsNullOrEmpty(id))
            {
                var content = new StringBuilder();

                foreach(var entry in information)
                {
                    content.AppendLine($"{entry.Key} = {string.Join(", ", entry.Value)}");
                }

                File.WriteAllText(
                    Path.Combine("../../", "CrawlingStorage", $"{id}.crawl"),
                    content.ToString()
                );
            }
        }
    }
}
