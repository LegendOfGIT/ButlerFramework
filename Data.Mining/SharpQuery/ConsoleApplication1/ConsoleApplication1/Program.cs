using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Data.Warehouse;
using Data.Warehouse.Crawler;

namespace ConsoleApplication1
{
    class Program
    {
        static List<WebcrawlerCommand> ContextCommandset = new List<WebcrawlerCommand>();
        static Dictionary<string, IEnumerable<string>> ContextDictionary = new Dictionary<string, IEnumerable<string>>();

        static void Main(string[] args)
        {
            //CrawlInformation(@"../../crawling/chefkoch.crawl");

            var provider = new FilesystemStorageProvider();
            var diggingResult = provider.DigInformation("Rezepte mit Eier");
        }

        private static void CrawlInformation(string templatefile)
        {
            var encoding = Encoding.GetEncoding(1252);

            var index = default(int);
            var lines = LoadMappingTemplate(templatefile);

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

            var compiler = new WebcrawlerCompiler();
            var commandset = compiler.ParseCommandset(lines, index);
            ContextDictionary[WebcrawlingUtilityConstants.BaseUri] = new[] { baseUri };
            ContextDictionary[WebcrawlingUtilityConstants.CurrentUri] = new[] { sourceUri };

            var miningutility = new WebcrawlingUtility
            {
                ContextCommandset = compiler.ContextCommandset,
                ContextDictionary = ContextDictionary
            };
            miningutility.Mining(commandset);

            sourceUri = sourceUri;
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