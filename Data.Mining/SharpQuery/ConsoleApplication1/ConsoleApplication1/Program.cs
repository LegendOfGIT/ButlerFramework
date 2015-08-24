using CsQuery;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Data.Web;

namespace ConsoleApplication1
{
    class Program
    {
        static Dictionary<string, IEnumerable<string>> ContextDictionary = new Dictionary<string, IEnumerable<string>>();

        static void Main(string[] args)
        {
            var encoding = Encoding.GetEncoding(1252);

            var index = default(int);
            var crawlingmap = @"C:\Temp\ButlerFrameworkGIT\Data.Mining\SharpQuery\ConsoleApplication1\ConsoleApplication1\gelbeseiten.crawl";
            var lines = default(string[]);
            if (File.Exists(crawlingmap))
            {
                lines = File.ReadAllLines(crawlingmap);
            }

            var sourceUri = string.Empty;
            foreach (var line in lines)
            {
                if(line.StartsWith("Source"))
                {
                    sourceUri = line.Split('=')[1];
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

            var commandset = ParseCommandset(lines, index);
            ContextDictionary[Constants.CurrentUri] = new[]{ sourceUri };
            ApplyCommandset(commandset);

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

        private static IEnumerable<CrawlCommand> ParseCommandset(IEnumerable<string> lines, int index)
        {
            var commands = default(List<CrawlCommand>);

            var line = lines.ToArray()[index];
            var level = line.GetLevel();
            var commandlines = 
                lines
                    .Skip(index)
                        .Where(l => l.GetLevel() == level && !string.IsNullOrEmpty(l))
                            .Select(l => new KeyValuePair<int, string>(lines.ToList().IndexOf(l), l))
            ;

            foreach (var commandline in commandlines)
            {
                commands = commands ?? new List<CrawlCommand>();
                var command = new CrawlCommand
                {
                    IsLoop = commandline.Value.Contains("!"),
                    Command = commandline.Value.Replace("!", string.Empty)
                };

                var subcommands = default(List<CrawlCommand>);
                var followingLines = lines.Skip(commandline.Key + 1).Select(l => new KeyValuePair<int, string>(lines.ToList().IndexOf(l), l));
                foreach (var followingLine in followingLines)
                {
                    if(followingLine.Value.GetLevel() == level + 1)
                    {
                        subcommands = subcommands ?? new List<CrawlCommand>();
                        subcommands.AddRange(ParseCommandset(lines, followingLine.Key));
                    }
                    else
                    {
                        break;
                    }
                }

                command.Subcommands = subcommands;
                commands.Add(command);
            }

            return commands;
        }
        private static void ApplyCommandset(IEnumerable<CrawlCommand> commandset, IEnumerable<string> contents = null)
        {
            if (commandset != null)
            {
                var querydoms = 
                    contents == null ? new[] { new CQ() } :
                    contents.Select(c => new CQ(c))
                ;
                foreach (var dom in querydoms)
                {
                    foreach (var command in commandset)
                    {
                        var doRepeat = true;
                        while (doRepeat)
                        {
                            var subcommandContents = default(List<string>);

                            //  Anwendung eines Kommandos
                            var commandtokens = default(IEnumerable<string>);
                            commandtokens = Regex.Split(command.Command, ">>");
                            var querytext = (commandtokens != null ? (commandtokens.FirstOrDefault() ?? string.Empty) : string.Empty).Trim();

                            var querytarget = 
                                (commandtokens != null && commandtokens.Count() > 1 ? (commandtokens.Skip(1).FirstOrDefault() ?? string.Empty) : string.Empty).Trim()
                            ;
                            if(!string.IsNullOrEmpty(querytarget))
                            {
                                var tokens = querytarget.Split('.');
                                if(tokens.Length > 1)
                                {
                                    var index = default(int);
                                    var key = string.Format(
                                        "{0}[{1}].{2}",
                                        tokens.First(),
                                        index,
                                        string.Join(".", tokens.Skip(1))
                                    );
                                    while (ContextDictionary.ContainsKey(key))
                                    {
                                        index++;
                                        key = string.Format(
                                            "{0}[{1}].{2}",
                                            tokens.First(),
                                            index,
                                            string.Join(".", tokens.Skip(1))
                                        );
                                    }

                                    querytarget = key;
                                }
                            }

                            commandtokens = querytext.Split('@');
                            var queryattribute = commandtokens != null && commandtokens.Count() > 1 ? (commandtokens.Skip(1).FirstOrDefault() ?? string.Empty).Trim() : string.Empty;
                            querytext = string.IsNullOrEmpty(queryattribute) ? querytext : commandtokens.FirstOrDefault() ?? querytext;

                            var wasCommandApplied = default(bool);
                            //  Ausführen des Querys
                            if (!string.IsNullOrEmpty(querytext))
                            {
                                var querycontent = default(IEnumerable<string>);
                                var query = default(CQ);
                                try {
                                    query = dom[querytext];
                                } catch (Exception) { }

                                if (query != null && query.Any())
                                {
                                    querycontent = query.Select(item => string.IsNullOrEmpty(queryattribute) ? item.InnerHTML : item.Attributes[queryattribute] ?? string.Empty);
                                }
                                //  Query in das Contextdictionary leiten
                                else
                                {
                                    querycontent = ContextDictionary.ContainsKey(querytext) ? ContextDictionary[querytext] : querycontent;
                                }

                                //  Die Abfrage ergab mindestens einen Treffer => Inhaltsverarbeitung
                                if (querycontent != null)
                                {
                                    foreach (var c in querycontent)
                                    {
                                        subcommandContents = subcommandContents ?? new List<string>();
                                        subcommandContents.Add(c);
                                    }

                                    if (!string.IsNullOrEmpty(querytarget))
                                    {
                                        ContextDictionary[querytarget] = querycontent;
                                    }
                                }
                                //  Kein Treffer => Sonstiger Befehl
                                else
                                {
                                    if (!string.IsNullOrEmpty(querytarget))
                                    {
                                        ContextDictionary[querytarget] = new[] { string.Empty };
                                    }

                                    commandtokens = querytext.Split(':');
                                    var commandobject = commandtokens.FirstOrDefault() ?? string.Empty;
                                    var commandexpression = commandtokens.Count() > 1 ? commandtokens.Skip(1).FirstOrDefault() ?? string.Empty : string.Empty;

                                    //  Befehle
                                    //  Browse()
                                    switch (commandexpression.ToLower())
                                    {
                                        case Constants.CommandBrowse:
                                            {
                                                var uris = ContextDictionary.ContainsKey(commandobject) ? ContextDictionary[commandobject] ?? default(string[]) : default(string[]);
                                                if(uris != null) { 
                                                    foreach(var uri in uris) {
                                                        if (!string.IsNullOrEmpty(uri))
                                                        {
                                                            subcommandContents = subcommandContents ?? new List<string>();
                                                            subcommandContents.Add(WebUtility.GetWebsiteContent(uri));
                                                            wasCommandApplied = true;
                                                        }
                                                    }
                                                }

                                                break;
                                            }
                                    }
                                }
                            }

                            //  Ausführen der Subkommandos
                            if (command.Subcommands != null)
                            {
                                foreach (var subcommand in command.Subcommands)
                                {
                                    ApplyCommandset(command.Subcommands, subcommandContents);
                                }
                            }

                            doRepeat =
                                command.IsLoop && wasCommandApplied
                            ;
                        }
                    }
                }
            }
        }
    }

    public class CrawlCommand
    {
        public string Command { get; set; }
        public bool IsLoop { get; set; }
        public IEnumerable<CrawlCommand> Subcommands { get; set; }
    }

    public static class Extensions{
        public static int GetLevel(this string line)
        {
            var level = default(int);
            for (int c = 0; c <= line.Length -1; c++) { if (line[c] != ' ') { break; } level++; }
            return level;
        }
    }
}
public class Constants
{
    public const string CommandBrowse = "browse()";
    public const string CurrentUri = "current.uri";
}