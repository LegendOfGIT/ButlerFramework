using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Data.Mining;
using Data.Mining.Web;

namespace ConsoleApplication1
{
    class Program
    {
        static List<MiningCommand> ContextCommandset = new List<MiningCommand>();
        static Dictionary<string, IEnumerable<string>> ContextDictionary = new Dictionary<string, IEnumerable<string>>();

        static void Main(string[] args)
        {
            var encoding = Encoding.GetEncoding(1252);

            var index = default(int);
            var lines = LoadMappingTemplate(@"../../crawling/chefkoch.crawl");

            var baseUri = string.Empty;
            var sourceUri = string.Empty;
            foreach (var line in lines)
            {
                var tokens = line.Split('=');
                var key = tokens != null ? tokens.FirstOrDefault() ?? string.Empty : string.Empty;
                var value = tokens != null ? tokens.Skip(1).FirstOrDefault() ?? string.Empty : string.Empty;
                if (key.StartsWith("BaseUri"))
                {
                    baseUri = value;
                }
                else if (key.StartsWith("Source"))
                {
                    sourceUri = value;
                    break;
                }
                index++;
            }
            for (int i = index + 1; i <= lines.Length; i++)
            {
                if (lines[i] != string.Empty)
                {
                    index = i;
                    break;
                }
            }

            var compiler = new MiningCompiler();
            var commandset = compiler.ParseCommandset(lines, index);
            ContextDictionary[MiningUtilityConstants.BaseUri] = new[] { baseUri };
            ContextDictionary[MiningUtilityConstants.CurrentUri] = new[] { sourceUri };

            var miningutility = new MiningUtility {
                ContextCommandset = compiler.ContextCommandset,
                ContextDictionary = ContextDictionary
            };            
            miningutility.Mining(commandset);

            sourceUri = sourceUri;



            //while (!string.IsNullOrEmpty(sourceUri)) {
            //    var content = WebUtility.GetWebsiteContent(sourceUri, encoding);
            
            //    var result = new Dictionary<string, string>(); 
            //    var query = new CQ(content);

            //    var doctors = query[".teilnehmer"];
            //    foreach (var doctor in doctors)
            //    {
            //        var doctordom = new CQ(doctor.InnerHTML);
                    
            //        //  Name
            //        var name = doctordom["span[itemprop='name']"].Text();
            //        var communication = doctordom[".hidden-xs"];
            //        //  Telefonnummer
            //        var telefonnumber = communication[".nummer"].First().Text();
            //        //  E-Mail
            //        var email = communication[".email span.text"].First().Text();
            //    }

            //    var uri = query[".gs_seite_vor_wrapper a"].Select(wrapper => wrapper["href"]).FirstOrDefault();
            //    sourceUri = uri == null ? null : uri.ToString();
            //}

            //Console.ReadLine();
        }
        private static string[] LoadMappingTemplate(string templatepath)
        {
            var lines = default(List<string>);
            //  Existiert die Vorlage?
            if (File.Exists(templatepath ?? string.Empty))
            {
                //  Wenn noch keine Crawlingkommandos eingelesen wurden, oder noch ein Untermodul zu finden ist.
                while(lines == null || lines.Any(line => line.EndsWith(".crawl")))
                { 
                    lines = new List<string>();
                    //  Einladen des ursprünglich angeforderten Templates
                    var templatelines = File.ReadAllLines(templatepath);
                    if(templatelines != null && templatelines.Any())
                    {
                        //  Durchlaufe alle Zeilen des Templates
                        templatelines.ToList().ForEach(templateline => {
                            //  Endet die aktuelle Zeile mit einem .crawl-Modul?
                            if(templateline.EndsWith(".crawl"))
                            {
                                //  Zusammenstellung des Modulpfades
                                var modulepath = $@"..\..\{templateline.Trim()}";
                                //  Existiert das Modul?
                                if (File.Exists(modulepath))
                                {
                                    var modulelevel = templateline.GetLevel();
                                    //  Lade das Untermodul und durchlaufe alle Modulzeilen
                                    var modulelines = File.ReadAllLines(modulepath);
                                    foreach (var moduleline in modulelines)
                                    {
                                        //  Füge jede Modulzeile auf der Ebene des Modules in dem Ergebnis ein.
                                        lines.Add("".PadLeft(modulelevel) + moduleline);
                                    }
                                }
                            }
                            //  Handelt es sich um eine reguläre Vorlagenzeile ...
                            else
                            {
                                //  ... wird diese dem Ergebnis hinzugefügt.
                                lines.Add(templateline);
                            }
                        });
                    }
                    else
                    {
                        lines.AddRange(templatelines);
                    }

                    templatelines = lines.ToArray();
                }
            }

            return lines == null ? null : lines.ToArray();
        }
    }
}