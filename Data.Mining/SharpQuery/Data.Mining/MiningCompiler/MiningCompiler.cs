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
                    var zutat = default(string);

                    //  Suchfaktor "Zutat"
                    expression = "mit.*";
                    match = Regex.Match(question, expression);
                    zutat = match != null && match.Success ? Regex.Replace(match.Value, "mit ", string.Empty) : zutat;
                    if(!string.IsNullOrEmpty(zutat))
                    {
                        querys["zutat"] = new[] { $".*?{zutat}.*?" };
                    }
                }
            }


            return querys;
        }
    }
}
