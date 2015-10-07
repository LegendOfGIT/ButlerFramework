using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Mining
{
    public class MiningCompiler
    {
        public static Dictionary<string, IEnumerable<string>> CompileQuerys(string question)
        {
            var querys = new Dictionary<string, IEnumerable<string>>();

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
            }


            return querys;
        }
    }
}
