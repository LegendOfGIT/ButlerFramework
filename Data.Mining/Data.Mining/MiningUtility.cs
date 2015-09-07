using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CsQuery;

using Data.Web;
using Newtonsoft.Json;

namespace Data.Mining.Web
{
    public class MiningUtility {
        public MiningUtility() {
            this.ContextDictionary = new Dictionary<string, IEnumerable<string>> { };
        }

        public Dictionary<string, IEnumerable<string>> ContextDictionary { get; set; }
        public List<MiningCommand> ContextCommandset { get; set; }
        public Encoding MiningPageEncoding { get; set; }

        public void Mining(IEnumerable<MiningCommand> commandset, IEnumerable<string> contents = null) {
            if (commandset != null)
            {
                contents = contents ?? new[] { string.Empty };
                foreach (var content in contents)
                {
                    foreach (var command in commandset)
                    {
                        var doRepeat = true;
                        while (doRepeat)
                        {
                            var subcommandContents = default(List<string>);

                            //  Anwendung eines Kommandos
                            var querytext = command.Command;
                            var queryattribute = command.AttributID;

                            var wasCommandApplied = default(bool);
                            //  Ausführen des Querys
                            if (!string.IsNullOrEmpty(querytext) || !string.IsNullOrEmpty(queryattribute))
                            {
                                var quertarget = command.Target;
                                var commandtokens = default(string[]);
                                commandtokens = (quertarget ?? string.Empty).Split('.');
                                var isTargetInformationItem = commandtokens != null && commandtokens.Length > 1;

                                //  Ermittlung des Queryergebnis
                                var querycontent = FindContent(
                                    content,
                                    command
                                );

                                //  Bei einem Informationsobjekt als Zieltyp ...
                                commandtokens = (quertarget ?? string.Empty).Split('.');
                                if (isTargetInformationItem)
                                {
                                    //  ... wird bei erster Verwendung/einer Wiederholung der Index des Zielobjektes hochgezählt
                                    var next = commandtokens.First();
                                    var targetbase = commandtokens.First();
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

                                        if (command == firstObjectCommand)
                                        {
                                            doIterate = ContextDictionary.Any(entry => entry.Key.StartsWith(key));
                                        }
                                        else
                                        {
                                            doIterate = ContextDictionary.Any(entry => entry.Key.StartsWith(key));
                                            if (!doIterate)
                                            {
                                                key = previouskey;
                                            }
                                        }
                                    }

                                    quertarget = string.Format("{0}.{1}", key, string.Join(".", commandtokens.Skip(1)));
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
                                        case MiningUtilityConstants.CommandBrowse:
                                            {
                                                var uris = ContextDictionary.ContainsKey(commandobject) ? ContextDictionary[commandobject] ?? default(string[]) : default(string[]);
                                                if (uris != null)
                                                {
                                                    foreach (var uri in uris)
                                                    {
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
                                Mining(command.Subcommands, subcommandContents);
                            }

                            doRepeat =
                                command.IsLoop && wasCommandApplied
                            ;
                        }
                    }
                }
            }
        }
        private IEnumerable<string> FindContent(string context, MiningCommand command)
        {
            var content = default(List<string>);

            var querytext = command == null ? string.Empty : command.Command;

            if (content == null || !content.Any())
            {
                //  Inhalt über JSON suchen
                if ((context ?? string.Empty).StartsWith("{"))
                {
                    dynamic json = JsonConvert.DeserializeObject(context);
                    var jsonvalue = json == null ? null : json.Property(querytext);
                    if(jsonvalue != null && !string.IsNullOrEmpty(jsonvalue.Value.ToString()))
                    {
                        content = new List<string> { jsonvalue.Value.ToString() };
                    }
                }
            }

            if (content == null || !content.Any())
            {
                var queryattribute = command == null ? string.Empty : command.AttributID;
                var quertarget = command == null ? string.Empty : command.Target;

                //  Inhalt über CSS-Query suchen
                if (!string.IsNullOrEmpty(context) || !string.IsNullOrEmpty(queryattribute))
                {
                    var commandtokens = default(string[]);
                    commandtokens = (quertarget ?? string.Empty).Split('.');
                    var isTargetInformationItem = commandtokens != null && commandtokens.Length > 1;
                    var dom = new CQ(context);
                    var query = default(CQ);
                    try
                    {
                        query = string.IsNullOrEmpty(querytext) ? dom : dom[querytext];
                    }
                    catch (Exception) { }

                    if (query != null && query.Any())
                    {
                        content = query.Select(
                            item =>
                                string.IsNullOrEmpty(queryattribute) ?
                                    isTargetInformationItem ?
                                    item.InnerHTML :
                                    item.OuterHTML :
                                item.Attributes[queryattribute] ?? string.Empty
                        ).ToList();
                    }
                }
            }

            //  Query in das Contextdictionary leiten
            if(content == null || !content.Any())
            {
                content = ContextDictionary.ContainsKey(querytext) ? ContextDictionary[querytext].ToList() : content;
            }

            return content;
        }
    }
}
