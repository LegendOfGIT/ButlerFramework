using System.Collections.Generic;
using System.Linq;

using Data.Web;

namespace Data.Warehouse
{
    public static class DataWarehouseExtensions
    {
        /// <summary>
        /// Bildet einen Hashcode für das Wiedererkennen der Information in dem Data-Warehouse
        /// </summary>
        /// <param name="information">Informationssammlung</param>
        /// <returns>Gibt einen Hashcode zurück, der die Inhalte der Informationssammlung in kurzer Form erkennbar macht.</returns>
        public static string GetHashcode(this Dictionary<string, IEnumerable<string>> information)
        {
            var hashcode = default(string);

            hashcode = string.Join(
                string.Empty,
                information?.Select(entry =>
                {
                    return string.Format(
                        "{0}.{1}",
                        //  Hashcode des Dictionary-Key
                        entry.Key.GetHashCode().ToString(),
                        //  Hashcode aller Einträge der Liste unter dem Dictionaryschlüssel
                        string.Join(
                            string.Empty,
                            entry.Value?.Select(token => 
                                token.GetHashCode().ToString()
                            )
                        ).GetHashCode().ToString()
                    );
                })
            //  Hashcode aus allen zuvor ermittelten Hashcodes
            ).GetHashCode().ToString();

            return hashcode;
        }
        /// <summary>
        /// Ermittelt die ID-Information aus der Informationssammlung
        /// </summary>
        /// <param name="information">Informationssammlung</param>
        /// <returns>ID-Information</returns>
        public static string GetId(this Dictionary<string, IEnumerable<string>> information)
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
        /// <summary>
        /// Bereitet die Informationen für die Einspeisung in das Data-Warehouse vor
        /// </summary>
        /// <param name="information">Informationssammlung</param>
        /// <returns>Gibt die vorbereitete Form der Informationssammlung zurück</returns>
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
