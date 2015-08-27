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
        static List<CrawlCommand> ContextCommandset = new List<CrawlCommand>();
        static Dictionary<string, IEnumerable<string>> ContextDictionary = new Dictionary<string, IEnumerable<string>>();

        static void Main(string[] args)
        {
            var encoding = Encoding.GetEncoding(1252);

            var index = default(int);
            var crawlingmap = @"G:\Entwicklung\GitHUB\ButlerFramework\Data.Mining\SharpQuery\ConsoleApplication1\ConsoleApplication1\gelbeseiten.crawl";
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
                var commandtext = commandline.Value;
                var tokens = default(IEnumerable<string>);

                //  Enthält das Kommando ein !, wird dieses Kommando solange wiederholt wie es mindestens ein Ergebnis zurückgibt.
                var isLoop = commandtext.Contains("!");
                commandtext = commandtext.Replace("!", string.Empty);
                //  Ermittlung des Kommandoziels
                tokens = Regex.Split(commandtext, ">>");
                var target =
                    (tokens != null && tokens.Count() > 1 ? (tokens.Skip(1).FirstOrDefault() ?? string.Empty) : string.Empty).Trim()
                ;
                commandtext = (tokens != null ? (tokens.FirstOrDefault() ?? string.Empty) : string.Empty).Trim();
                //  Ermittlung eines Attributes
                tokens = commandtext.Split('@');
                var attribute = tokens != null && tokens.Count() > 1 ? (tokens.Skip(1).FirstOrDefault() ?? string.Empty).Trim() : string.Empty;
                commandtext = string.IsNullOrEmpty(attribute) ? commandtext : tokens.FirstOrDefault() ?? commandtext;

                commands = commands ?? new List<CrawlCommand>();
                var command = new CrawlCommand
                {                  
                    AttributID = attribute,  
                    IsLoop = isLoop,
                    Command = commandtext,
                    Target = target
                };

                var subcommands = default(List<CrawlCommand>);
                var followingLines = lines.Skip(commandline.Key + 1).Select(l => new KeyValuePair<int, string>(lines.ToList().IndexOf(l), l));
                foreach (var followingLine in followingLines)
                {
                    if(followingLine.Value.GetLevel() == level + 1)
                    {
                        subcommands = subcommands ?? new List<CrawlCommand>();
                        var set = ParseCommandset(lines, followingLine.Key);
                        subcommands.AddRange(set);
                        ContextCommandset.AddRange(set);
                    }
                    else
                    {
                        break;
                    }
                }

                command.Subcommands = subcommands;
                commands.Add(command);
                ContextCommandset.Add(command);
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
                            var querytext = command.Command;

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
                                    querycontent = query.Select(item => string.IsNullOrEmpty(command.AttributID) ? item.InnerHTML : item.Attributes[command.AttributID] ?? string.Empty);
                                }
                                //  Query in das Contextdictionary leiten
                                else
                                {
                                    querycontent = ContextDictionary.ContainsKey(querytext) ? ContextDictionary[querytext] : querycontent;
                                }

                                var commandtokens = default(string[]);
                                var quertarget = command.Target;

                                //  Bei einem Informationsobjekt als Zieltyp ...
                                commandtokens = (quertarget ?? string.Empty).Split('.');
                                if(commandtokens != null && commandtokens.Length > 1)
                                {
                                    //  ... wird bei erster Verwendung/einer Wiederholung der Index des Zielobjektes hochgezählt
                                    var next = commandtokens.First();
                                    var targetbase = string.Join(".", commandtokens.Skip(1).Select(s => { var sel = next; next = s; return sel; }));
                                    var firstObjectCommand = ContextCommandset.First(c => c.Target.StartsWith(targetbase + "."));

                                    var index = default(int);
                                    var previouskey = default(string);
                                    var key = default(string);

                                    var doIterate = true;
                                    key = string.Format("{0}[{1}]", targetbase, index);
                                    while (doIterate)
                                    {
                                        previouskey = key;

                                        index++;
                                        key = string.Format("{0}[{1}]", targetbase, index);

                                        if(command == firstObjectCommand)
                                        {
                                            doIterate = ContextDictionary.Any(entry => entry.Key.StartsWith(key));
                                        }
                                        else
                                        {
                                            doIterate = ContextDictionary.Any(entry => entry.Key.StartsWith(key));
                                            if(!doIterate)
                                            {
                                                key = previouskey;
                                            }
                                        }
                                    }

                                    quertarget = string.Format("{0}.{1}", key, commandtokens.Last());
                                    
                                }

                                //  Die Abfrage ergab mindestens einen Treffer => Inhaltsverarbeitung
                                if (querycontent != null)
                                {
                                    foreach (var c in querycontent)
                                    {
                                        subcommandContents = subcommandContents ?? new List<string>();
                                        subcommandContents.Add(c);
                                    }

                                    if (!string.IsNullOrEmpty(quertarget))
                                    {
                                        ContextDictionary[quertarget] = querycontent;
                                    }
                                }
                                //  Kein Treffer => Sonstiger Befehl
                                else
                                {
                                    if (!string.IsNullOrEmpty(quertarget))
                                    {
                                        ContextDictionary[quertarget] = new[] { string.Empty };
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
        public string AttributID { get; set; }
        public string Command { get; set; }
        public string Target { get; set; }
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