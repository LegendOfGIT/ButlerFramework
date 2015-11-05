using System.Collections.Generic;
using System.Linq;

using Data.Web;

namespace Data.Warehouse
{
    public static class DataWarehouseExtensions
    {
        public static string GetInformationId(this Dictionary<string, IEnumerable<string>> information)
        {
            var id = default(string);

            var idexpressions = new[] { ".id", "id" };
            var ids = 
                information != null ? 
                information?.FirstOrDefault(entry => idexpressions.Any(expression => entry.Key.ToLower().EndsWith(expression))) : 
                default(KeyValuePair<string, IEnumerable<string>>)
            ;
            id = ids?.Value?.FirstOrDefault() ?? string.Empty;

            return id;
        }
        public static Dictionary<string, IEnumerable<string>> PrepareInformation(this Dictionary<string, IEnumerable<string>> information)
        {
            return
                information?.ToDictionary(
                    entry => {
                        var key = entry.Key;

                        var tokens = entry.Key.Split('.');
                        key = tokens?.Length > 1 ? string.Join(".", tokens.Skip(1)) : key;

                        return key;
                    },
                    entry => entry.Value.Select(
                        //  Wert
                        v => 
                            new string(
                                    v
                                    //  HTML-Bestandteile entfernen
                                    .RemoveHTML()
                                    //  Steuerzeichen entfernen
                                    .Where(c => !char.IsControl(c)).ToArray()
                            )
                            //  Überzählige Leerzeichen entfernen
                            .Trim()
                    )
                )
            ;
        }
    }
}
