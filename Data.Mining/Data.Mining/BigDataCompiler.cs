using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Data.Web;

namespace Data.Mining
{
    public class BigDataCompiler
    {
        public Dictionary<string, IEnumerable<string>> Compile(Dictionary<string, IEnumerable<string>> information)
        {
            var bigdata = default(Dictionary<string, IEnumerable<string>>);

            if(information != null)
            { 
                foreach(var info in information)
                {
                    var key = info.Key.RemoveHTML();
                    var values = info.Value.Select(
                        value => 
                            new string(
                                value.RemoveHTML().Where(c => !char.IsControl(c)).ToArray()
                            ).Trim()
                    );

                    foreach(var value in values)
                    {
                        var content = new List<string>();

                        var keyprefix = string.Empty;
                        key.Split('.').Reverse().Skip(1).ToList().ForEach(token => { keyprefix = $"{token}.{keyprefix}"; });
                        key = key.Split('.')?.Last() ?? key;
                    }
                }
            }

            return bigdata;
        }
    }
}
