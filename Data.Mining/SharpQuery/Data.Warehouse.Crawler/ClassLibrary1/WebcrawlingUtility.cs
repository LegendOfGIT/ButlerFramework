using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using CsQuery;

using Data.Web;
using Data.Warehouse;
using Newtonsoft.Json;

namespace Data.Warehouse.Crawler
{
    public class WebcrawlingUtility {
        public WebcrawlingUtility() {
            this.ContextDictionary = new Dictionary<string, IEnumerable<string>> { };
        }

        private StorageProvider Storageprovider = new FilesystemStorageProvider();
        public Dictionary<string, IEnumerable<string>> ContextDictionary { get; set; }
        public List<WebcrawlerCommand> ContextCommandset { get; set; }
        public Encoding MiningPageEncoding { get; set; }

        public void Mining(IEnumerable<WebcrawlerCommand> commandset, IEnumerable<string> contents = null) {
            if (commandset != null)
            {
                contents = contents ?? new[] { string.Empty };
                foreach (var content in contents)
                {
                    var storetarget = string.Empty;

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
                                var isTargetInformationItem = commandtokens?.Length > 1;

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
                                    var targetbase = commandtokens.First();
                                    var lastObjectCommand = ContextCommandset.Last(c => c.Target.StartsWith(targetbase + "."));

                                    if(command == lastObjectCommand)
                                    {
                                        storetarget = $"{targetbase}.";
                                    }
                                }

                                var isMiningCommand = WebcrawlingUtilityConstants.MiningCommands.Any(commandid => querytext.ToLower().Contains(commandid.ToLower()));
                                //  Die Abfrage ergab mindestens einen Treffer => Inhaltsverarbeitung
                                if (!isMiningCommand)
                                {
                                    foreach (var c in querycontent)
                                    {
                                        subcommandContents = subcommandContents ?? new List<string>();
                                        subcommandContents.Add(c);
                                    }

                                    //  Inhalt im Ziel sichern
                                    if (!string.IsNullOrEmpty(quertarget) && querycontent.Any())
                                    {
                                        var isFirstStoreCommand = ContextCommandset.First(storecommand => storecommand.Target == quertarget) == command;

                                        //  Erstes Speicherkommando >> Neuanlage einer Informationswiederholung
                                        if(isFirstStoreCommand || !isTargetInformationItem)
                                        { 
                                            var storecontent = ContextDictionary.ContainsKey(quertarget) ? new List<string>(ContextDictionary[quertarget]) : new List<string>();
                                            storecontent.AddRange(querycontent);
                                            ContextDictionary[quertarget] = storecontent;
                                        }
                                        //  Ergänzen der letzten Speicherinformation
                                        else
                                        {
                                            var storeitems = ContextDictionary.ContainsKey(quertarget) ? ContextDictionary[quertarget] : default(IEnumerable<string>);
                                            var lastStoreitem = storeitems != null && storeitems.Any() ? storeitems.Last() : default(string);
                                            if(lastStoreitem != null)
                                            {
                                                lastStoreitem += string.Format(" {0}", string.Join(" ", querycontent));
                                                var storecontent = ContextDictionary[quertarget].ToList();
                                                storecontent[storecontent.Count - 1] = lastStoreitem;
                                                ContextDictionary[quertarget] = storecontent;
                                            }
                                        }
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
                                        case WebcrawlingUtilityConstants.CommandBrowse:
                                        {
                                            var uris = ContextDictionary.ContainsKey(commandobject) ? ContextDictionary[commandobject] ?? default(string[]) : default(string[]);
                                            if (uris != null)
                                            {
                                                foreach (var uri in uris)
                                                {
                                                    var browseuri = uri.Trim();
                                                    if (!string.IsNullOrEmpty(browseuri))
                                                    {
                                                        if (!new[] { "http:", "https:" }.Any(prefix => browseuri.StartsWith(prefix)))
                                                        {
                                                            var baseuri = ContextDictionary.ContainsKey(WebcrawlingUtilityConstants.BaseUri) ? ContextDictionary[WebcrawlingUtilityConstants.BaseUri].FirstOrDefault() ?? string.Empty : string.Empty;
                                                            browseuri = baseuri + browseuri;
                                                        }

                                                        subcommandContents = subcommandContents ?? new List<string>();
                                                        subcommandContents.Add(WebUtility.GetWebsiteContent(browseuri));
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

                    //  Wenn das storetarget gesetzt wurde (letzter Zielverweis auf Informationsobjekt), wird das Informationsobjekt über einen Provider gesichert.
                    if (!string.IsNullOrEmpty(storetarget) && this.Storageprovider != null)
                    {
                        var storedictionary = new Dictionary<string, IEnumerable<string>>();
                        foreach (var entry in this.ContextDictionary.Where(e => e.Key.StartsWith(storetarget)))
                        {
                            storedictionary[entry.Key] = entry.Value;
                        }

                        //  Überarbeiten der ID-Inhalte
                        var keys = storedictionary.Where(item => item.Key.ToLower().EndsWith(".id")).Select(item => item.Key).ToList();
                        if (keys != null)
                        {
                            var baseuri = this.ContextDictionary.ContainsKey(WebcrawlingUtilityConstants.BaseUri) ? this.ContextDictionary[WebcrawlingUtilityConstants.BaseUri].First() : string.Empty;
                            baseuri = Regex.Split(baseuri, @"://")?.Skip(1).FirstOrDefault() ?? string.Empty;
                            if (!string.IsNullOrEmpty(baseuri))
                            {
                                foreach (var key in keys)
                                {
                                    storedictionary[key] = storedictionary[key].Select(dictionarycontent => $"{baseuri}.{dictionarycontent}");
                                }
                            }
                        }

                        this.Storageprovider.StoreInformation(storedictionary);
                        storedictionary.ToList().ForEach(storeitem => { ContextDictionary.Remove(storeitem.Key); });
                    }
                }
            }
        }
        private IEnumerable<string> FindContent(string context, WebcrawlerCommand command)
        {
            var content = new List<string>();

            var querytext = command?.Command;

            //  Inhalt über JSON suchen
            if (!content.Any())
            {
                if ((context ?? string.Empty).StartsWith("{"))
                {
                    dynamic json = JsonConvert.DeserializeObject(context);
                    var jsonvalue = json?.Property(querytext);
                    if(!string.IsNullOrEmpty(jsonvalue?.Value.ToString()))
                    {
                        content = new List<string> { jsonvalue.Value.ToString() };
                    }
                }
            }

            //  Inhalt über CSS-Query suchen
            if (!content.Any())
            {
                var queryattribute = command?.AttributID;
                var quertarget = command?.Target;

                if (!string.IsNullOrEmpty(context) || !string.IsNullOrEmpty(queryattribute))
                {
                    var commandtokens = default(string[]);
                    commandtokens = (quertarget ?? string.Empty).Split('.');
                    var isTargetInformationItem = commandtokens?.Length > 1;
                    var dom = new CQ(context);
                    var query = default(CQ);
                    try
                    {
                        query = string.IsNullOrEmpty(querytext) ? dom : dom[querytext];
                    }
                    catch (Exception) { }

                    if (query != null && query.Any())
                    {
                        try { 
                            content = query.Select(
                                item =>
                                    string.IsNullOrEmpty(queryattribute) ?
                                        isTargetInformationItem ?
                                        item.InnerHTML :
                                        item.OuterHTML :
                                    item.Attributes[queryattribute] ?? string.Empty
                            ).ToList();
                        }
                        catch (Exception) { }
                    }
                }
            }

            //  Inhalt über Regularexpression suchen
            if (!content.Any())
            {
                try
                { 
                    var expression = (@"" + querytext ?? string.Empty);
                    var matches = 
                        Regex.Matches(context, expression).
                        OfType<Match>().
                        Where(match => match.Success && !string.IsNullOrEmpty(match.Value)).
                        Select(match => match.Value)
                    ;                
                    if(matches != null && matches.Any())
                    {
                        content = matches.ToList();
                    }
                }
                catch (Exception) { }
            }

            //  Query in das Contextdictionary leiten
            if (!content.Any())
            {
                var tokens = querytext.Split(':');
                querytext = tokens?.FirstOrDefault() ?? string.Empty;
                content = ContextDictionary.ContainsKey(querytext) ? ContextDictionary[querytext].ToList() : content;
            }

            //  Inhalt mit "" umschlossen >> Statischer Inhalt
            if (!content.Any())
            {
                var quotation = "\"";
                if (querytext.StartsWith(quotation) && querytext.EndsWith(quotation))
                {
                    content = querytext.Replace(quotation, string.Empty).Split(',').Select(token => token.Trim()).ToList();
                }
            }

            return content;
        }
    }
}
