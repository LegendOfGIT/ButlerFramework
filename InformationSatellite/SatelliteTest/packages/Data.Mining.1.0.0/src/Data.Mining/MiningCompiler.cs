using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Data.Mining
{
    public class MiningCompiler
    {
        public static MiningQuery CompileQuerys(string question)
        {
            var scopequery = default(MiningQuery);

            var querys = new Dictionary<string, IEnumerable<string>>();
            var expression = string.Empty;
            var match = default(Match);

            question = question?.ToLower();

            //  Welche Informationsobjekte sollen gefunden werden?
            var scopes = new[]
            {
                "rezept"
            };
            var contextscopes = scopes.Where(scope => question.Contains(scope)).Concat(question.Split(' ').First().GetPluralSynonyms());
            if (contextscopes.Any())
            {
                scopequery = new MiningQuery {
                    Target = MiningConstants.PropertyTag,
                    Expressions = contextscopes
                };

                //  Kontext = Rezept
                if(contextscopes.Any(scope => scope == "rezept"))
                {
                    var parameter = default(string);

                    //  Suchfaktor "Zutat"
                    expression = "mit.*";
                    match = Regex.Match(question, expression);
                    parameter = match != null && match.Success ? Regex.Replace(match.Value, "mit ", string.Empty) : parameter;

                    var zutaten = Regex.Split(parameter, "(und|oder|,)").Where(token => !Regex.IsMatch(token, "(und|oder|,)")).Select(token => token.Trim());
                    if(zutaten != null)
                    {
                        foreach(var zutat in zutaten)
                        {
                            var synonyms = zutat.GetPluralSynonyms();

                            var q = querys.ContainsKey("zutat") ? querys["zutat"] : new List<string>();
                            q = q.Concat(synonyms.Select(synonym => $".*?{synonym}.*?").Distinct());
                            querys["zutat"] = q;
                        }
                    }
                }
            }


            return scopequery;
        }
    }
}