using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Data.Mining
{
    public class MiningCompiler
    {
        public static Dictionary<string, IEnumerable<string>> CompileQuerys(string question)
        {
            var querys = new Dictionary<string, IEnumerable<string>>();
            var expression = string.Empty;
            var match = default(Match);

            question = question?.ToLower();

            //  Welche Informationsobjekte sollen gefunden werden?
            var scopes = new[]
            {
                "rezept"
            };
            var contextscopes = scopes.Where(scope => question.Contains(scope));
            if(contextscopes.Any())
            {
                querys[MiningConstants.PropertyTag] = contextscopes;

                //  Kontext = Rezept
                if(contextscopes.Any(scope => scope == "rezept"))
                {
                    var parameter = default(string);

                    //  Suchfaktor "Zutat"
                    expression = "mit.*";
                    match = Regex.Match(question, expression);
                    parameter = match != null && match.Success ? Regex.Replace(match.Value, "mit ", string.Empty) : parameter;

                    var zutaten = Regex.Split(parameter, "(und|,)").Where(token => !Regex.IsMatch(token, "(und|,)")).Select(token => token.Trim());
                    if(zutaten != null)
                    {
                        foreach(var zutat in zutaten)
                        {
                            var synonyms = new string[] {
                                zutat,
                                zutat.Substring(0, zutat.Length - 1),
                                zutat.Replace("Ä", "A").Replace("Ö", "O").Replace("Ü", "U").Replace("ä", "a").Replace("ö", "o").Replace("ü", "u")
                            };

                            var query = querys.ContainsKey("zutat") ? querys["zutat"] : new List<string>();
                            query = query.Concat(synonyms.Select(synonym => $".*?{synonym}.*?").Distinct());
                            querys["zutat"] = query;
                        }
                    }
                }
            }


            return querys;
        }
    }
}
