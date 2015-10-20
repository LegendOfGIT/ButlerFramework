using System.Collections.Generic;
using System.Linq;

namespace Data.Mining
{
    public static class MiningExtensions
    {
        public static IEnumerable<string> GetPluralSynonyms(this string expression)
        {
            var synonyms = new List<string>(new[] { expression });

            if(!string.IsNullOrEmpty(expression))
            {
                synonyms = synonyms.Concat(new[]
                {
                    expression.Substring(0, expression.Length - 1).Trim(),
                    expression.Replace("Ä", "A").Replace("Ö", "O").Replace("Ü", "U").Replace("ä", "a").Replace("ö", "o").Replace("ü", "u").Trim()
                }).ToList();
            }

            return synonyms.Distinct();
        }
    }
}