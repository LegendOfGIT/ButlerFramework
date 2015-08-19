using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using Data.Web;

namespace Data.Mining.Web
{
    public class WebMiningUtility {
        public WebMiningUtility() {
            MiningURL =
                string.Empty
            ;

            MiningItem = new WebMiningItem();
        }

        private Dictionary<string, WebminingElement> Miningmap = new Dictionary<string, WebminingElement>();
        private List<WebminingElement> Mininglist = new List<WebminingElement>();
        private int LastLevelindex { get; set; }
        private WebMiningItem MiningItem { get; set; }

        public Encoding MiningPageEncoding { get; set; }
        public string MiningURL { get; set; }
        public string TestHTML { get; set; }

        public WebMiningResponse Mining() {
            var response = new WebMiningResponse();

            if (!string.IsNullOrEmpty(MiningURL) || !string.IsNullOrEmpty(TestHTML)) {
                var content = string.IsNullOrEmpty(TestHTML) ? WebUtility.GetWebsiteContent(MiningURL, MiningPageEncoding) : TestHTML;
                content = WebUtility.RemoveHTMLDecorations(content);
                content = WebUtility.RemoveHTMLComments(content);

                Mininglist.AddRange(GetMiningelements(content));

                //  <meta>
                MiningMeta();
                //  <ul>|<ol>
                MiningLists();
                //  <table>
                MiningTables();
                //  weiter/zurück
                MiningNavigation();
            }

            response.MiningItem = MiningItem;

            return response;
        }
        private List<WebminingElement> GetMiningelements(string content)
        {
            var response = new List<WebminingElement>();

            if (!string.IsNullOrEmpty(content))
            {
                var tags = Regex.Matches(content, HTMLContext.HTMLTagRegex);
                if (tags != null && tags.Count > 0)
                {
                    foreach (Match tag in tags)
                    {
                        if (!tag.Value.Contains("</"))
                        {
                            var grabbingattribute = Regex.Split(tag.Value, " ");
                            var begin = grabbingattribute[0];

                            var tagname = begin.Replace("<", string.Empty).Replace(">", string.Empty);
                            var element = new WebminingElement
                            {
                                Tagname = tagname,
                                HtmlContent = WebUtility.Take(content, tag, "<" + tagname, "</" + tagname).Trim()
                            };
                            element.FormattedContent = WebUtility.RemoveHTML(element.HtmlContent).Trim();
                            element.Content = RemoveFormatters(element.FormattedContent).Trim();

                            // Ermitteln und Ablegen der Tagattribute
                            var currentattribute = string.Empty;
                            foreach (var grabbingattribut in grabbingattribute)
                            {
                                currentattribute += string.IsNullOrEmpty(currentattribute) ? grabbingattribut.Contains('"') ? grabbingattribut : string.Empty : " " + grabbingattribut;
                                if (Regex.Matches(currentattribute, "\"").Count == 2)
                                {
                                    var attributteile = Regex.Split(currentattribute, "\"");
                                    if (attributteile != null && attributteile.Count() == 3)
                                    {
                                        element.Attributes[attributteile[0].Replace("=", string.Empty)] = WebUtility.RemoveHTML(attributteile[1]);
                                    }

                                    currentattribute = string.Empty;
                                }
                            }

                            response.Add(element);
                        }
                    }
                }
            }

            return response;
        }

        //public WebMiningResponse Mining(){
        //    var response = new WebMiningResponse();

        //    if (!string.IsNullOrEmpty(MiningURL) || !string.IsNullOrEmpty(TestHTML)) {
        //        var content = string.IsNullOrEmpty(TestHTML) ? WebUtility.GetWebsiteContent(MiningURL, MiningPageEncoding) : TestHTML;

        //        content = WebUtility.RemoveHTMLDecorations(content);
        //        content = WebUtility.RemoveHTMLComments(content);

        //        // Auslesen aller HTML-Tags
        //        var grabbingtags = Regex.Matches(content, HTMLContext.HTMLTagRegex);
        //        if (grabbingtags != null && grabbingtags.Count > 0) {
        //            var grabbingelements = new List<Match>();

        //            // Aufnahme aller XML-Tags die nicht mit </ geschlossen werden.
        //            foreach (Match grabbingelement in grabbingtags) {
        //                if (
        //                    grabbingelement != null &&
        //                    grabbingelement.Success &&
        //                    !grabbingelement.Value.Contains("</")
        //                ){
        //                    grabbingelements.Add(grabbingelement);
        //                }
        //            }

        //            // Durchlaufe alle XML-Tags
        //            foreach (Match grabbingelement in grabbingelements) {
        //                if (grabbingelement != null && grabbingelement.Success) {
        //                    var grabbingattribute = Regex.Split(grabbingelement.Value, " ");
        //                    var begin = grabbingattribute[0];
        //                    var grabbingcontent = grabbingelement.Value.Contains("/>") ? grabbingelement.Value : WebUtility.Take(content, grabbingelement, begin, "</" + begin.Replace("<", string.Empty));

        //                    // Festhalten eines HTML-Elementes
        //                    var element = new WebminingElement {
        //                        // Kompletter Inhalt
        //                        HtmlContent = grabbingcontent.Trim(),
        //                        // Inhalt ohne HTML-Tags
        //                        FormattedContent = WebUtility.RemoveHTML(grabbingcontent).Trim(),
        //                        // Name des HTML-Tags
        //                        Tagname = begin.Replace("<", string.Empty).Replace(">", string.Empty)
        //                    };

        //                    // Inhalt ohne HTML-Tags und Steuerzeichen
        //                    element.Content = RemoveFormatters(element.FormattedContent).Trim();

        //                    // Ermittlung des Taglevels und ablegen im Ergebnisdictionary
        //                    var level = GetLevel(content, grabbingelement);
        //                    element.Level = level;
        //                    Miningmap[level] = element;

        //                    // Ermitteln und Ablegen der Tagattribute
        //                    var currentattribute = string.Empty;
        //                    foreach (var grabbingattribut in grabbingattribute) {
        //                        currentattribute += string.IsNullOrEmpty(currentattribute) ? grabbingattribut.Contains('"') ? grabbingattribut : string.Empty : " " + grabbingattribut;
        //                        if (Regex.Matches(currentattribute, "\"").Count == 2) {
        //                            var attributteile = Regex.Split(currentattribute, "\"");
        //                            if (attributteile != null && attributteile.Count() == 3) {
        //                                element.Attributes[attributteile[0].Replace("=", string.Empty)] = WebUtility.RemoveHTML(attributteile[1]);
        //                            }

        //                            currentattribute = string.Empty;
        //                        }
        //                    }
        //                }
        //            }
        //        }

        //        Mininglist = Miningmap.Values.ToList();
        //        foreach (var Element in Miningmap) {
        //            Mining(Element);
        //        }

        //        response.MiningItem = MiningItem;
        //    }

        //    return response;
        //}
        private void Mining(KeyValuePair<string, WebminingElement> entry) {
            MiningItem = MiningItem ?? new WebMiningItem { }; 
            
            //  <meta>
            //MiningMeta(entry);
            //  <table>
            //MiningList(entry);
        }

        private void MiningMeta(){
            var elements = Mininglist.Where(element => element.Tagname == "meta");
            if (elements != null && elements.Count() > 0) {
                foreach (var element in elements) {
                    if (element.Attributes != null && element.Attributes.ContainsKey("name") && element.Attributes["name"] == "keywords" && element.Attributes.ContainsKey("content")) {
                        MiningItem.Content["Stichwörter"] = GetList(element.Attributes["content"]);
                    }
                }
            }
        }
        private void MiningLists() {
            var elements = Mininglist.Where(element => element.Tagname == "ul" || element.Tagname == "ol");
            if (elements != null && elements.Count() > 0) {
                foreach (var element in elements) {
                    var title =
                        element == null || element.Attributes == null ? default(string) :
                        element.Attributes.Any(attr => attr.Value.ToLower().Contains("result")) ? MiningConstants.Results :
                        default(string)
                    ;

                    if (title != null) {
                        var listitems = Regex.Matches(element.HtmlContent, "<li");
                        if (listitems != null && listitems.Count > 0) {
                            foreach (Match listitem in listitems) {
                                var item = WebUtility.Take(element.HtmlContent, listitem, "<li", "</li>");
                                var subitem = new WebMiningItem { Title = title };
                                subitem.Content["HTMLContent"] = new List<string> { item };
                                subitem.Content["Content"] = new List<string> { RemoveFormatters(WebUtility.RemoveHTML(item)) };

                                var itemelements = GetMiningelements(item);
                                if (itemelements != null && itemelements.Any()) {
                                    var links = itemelements.Where(ie => ie.Tagname == "a" && ie.Attributes != null && ie.Attributes.ContainsKey("href"));
                                    if (links != null && links.Any()) {
                                        var linklist = subitem.Content.ContainsKey("links") ? subitem.Content["links"] ?? new List<string>() : new List<string>();
                                        foreach (var link in links) { linklist.Add(link.Attributes["href"]); }
                                        subitem.Content["links"] = linklist;
                                    }
                                }

                                MiningItem.Subinformation.Add(subitem);
                            }
                        }
                    }
                }
            }
        }
        private void MiningTables() {
            //  <table>
            var elements = Mininglist.Where(element => element.Tagname == "table");
            if (elements != null && elements.Count() > 0) {
                foreach (var element in elements) {
                    var title =
                        element == null || element.Attributes == null ? default(string) :
                        element.Attributes.Any(attr => attr.Value.ToLower().Contains("result")) ? MiningConstants.Results :
                        default(string)
                    ;

                    var tableelements = GetMiningelements(element.HtmlContent);
                    if (title != null && tableelements != null && tableelements.Any()) {
                        var spaltenids = new List<string>();
                        //  SpaltenIDs aus <th>
                        var headercolumns = tableelements.Where(tableelement => tableelement.Tagname == "th"); if (headercolumns != null) { headercolumns.ToList().ForEach(hc => spaltenids.Add(hc.Content)); }

                        //  <tr>
                        var rows = tableelements.Where(tableelement => tableelement.Tagname == "tr");
                        if (rows != null && rows.Any()) {
                            foreach (var row in rows) {
                                var item = new WebMiningItem { Title = title };

                                var rowelements = GetMiningelements(row.HtmlContent);
                                var dataelements = rowelements.Where(rowelement => rowelement.Tagname == "td");
                                var index = 0;
                                //  <td>
                                if (dataelements != null && dataelements.Any()) {
                                    foreach (var dataelement in dataelements) {
                                        if (spaltenids.Count > index) {
                                            var information = (item.Content.ContainsKey(spaltenids[index]) ? item.Content[spaltenids[index]] : null) ?? new List<string>();
                                            information.Add(dataelement.Content);
                                            item.Content[spaltenids[index]] = information;
                                        }

                                        index++;
                                    }
                                }

                                MiningItem.Subinformation.Add(item);
                            }
                        }
                    }
                }
            }
        }
        private void MiningNavigation()
        {
            //  Weiter
            var elements = Mininglist.Where(element => element.Tagname == "a" && element.Attributes != null && element.Attributes.Any(attr => attr.Value.ToLower().Contains("next")));
            if (elements != null && elements.Count() > 0) {
                foreach (var element in elements) {
                    if (element.Attributes.ContainsKey("href")) {
                        var list = MiningItem.Content.ContainsKey(MiningConstants.Nextlink) ? MiningItem.Content[MiningConstants.Nextlink] ?? new List<string>() : new List<string>();
                        list.Add(element.Attributes["href"]);
                        MiningItem.Content[MiningConstants.Nextlink] = list;
                    }
                }
            }
        }
        //private void MiningMeta(KeyValuePair<string, WebminingElement> entry) {
        //    var miningelement = entry.Value;
        //    if (miningelement.Attributes != null && miningelement.Attributes.ContainsKey("name") && miningelement.Attributes["name"] == "keywords" && miningelement.Attributes.ContainsKey("content")) {
        //        MiningItem.Content["Stichwörter"] = GetList(miningelement.Attributes["content"]);
        //    }
        //}
        //private void MiningList(KeyValuePair<string, WebminingElement> entry) {
        //    if (entry.Value != null && (new []{ "ul", "ol" }.Contains(entry.Value.Tagname))) {
        //        var element = entry.Value;
        //        var index = Mininglist.IndexOf(element);
        //        var title = 
        //            element == null || element.Attributes == null ? default(string) :
        //            element.Attributes.Any(attr => attr.Value.ToLower().Contains("result")) ? MiningConstants.Results :
        //            default(string)
        //        ;
        //        var level = entry.Key.Split('.').Length;
        //        var currentlevel = level;
        //        while (index < Mininglist.Count && currentlevel >= level) {
        //            var currentelement = Mininglist[index];
        //            currentlevel = currentelement.Level.Length;
        //            if (currentelement.Tagname == "li" && title != null)
        //            {
        //                var item = new WebMiningItem { Title = title };

        //                item.Content["Content"] = new List<string> { currentelement.Content };
        //                item.Content["HtmlContent"] = new List<string> { currentelement.HtmlContent };

        //                var subindex = index + 1;
        //                var sublevel = currentlevel;
        //                var subcurrentelement = Mininglist[subindex];
        //                while (subindex < Mininglist.Count && sublevel >= currentlevel && subcurrentelement.Tagname != "li") {
        //                    subcurrentelement = Mininglist[subindex];
        //                    sublevel = subcurrentelement.Level.Split('.').Length;

        //                    //  Link
        //                    if (subcurrentelement.Tagname == "a" && subcurrentelement.Attributes != null && subcurrentelement.Attributes.ContainsKey("href")) {
        //                        var links = item.Content.ContainsKey("Links") ? item.Content["links"] ?? new List<string>() : new List<string>();
        //                        links.Add(subcurrentelement.Attributes["href"]);
        //                        item.Content["links"] = links;
        //                    }

        //                    subindex++;
        //                }

        //                MiningItem.Subinformation.Add(item);
        //            }

        //            index++;
        //        }
        //    }
        //}
        private List<string> GetList(string content)
        {
            List<string> list = new List<string>();

            var separators = new[] { ',', ';' };
            foreach (var separator in separators) { list = content.Split(separator).Where(sep => !string.IsNullOrEmpty(sep.Trim())).ToList(); if (list.Count() > 1) { break; } }

            list = list == null || list.Count == 0 ? new List<string> { content } : list;
            var response = new List<string>(); foreach (var l in list) { response.Add(l.Trim()); }
            
            return response;
        }

        /// <summary>
        /// Diese Funktion ermittelt die Ebene eines HTML-Tags
        /// Beispiel:
        /// 1
        /// 1.1
        /// 1.1.1
        /// 1.1.2
        /// 1.2
        /// 1.2.1
        /// </summary>
        /// <param name="content">Gesamtinhalt des HTML-Dokument</param>
        /// <param name="element">HTML-Tag, dessen Ebene ermittelt werden soll</param>
        /// <returns>Gibt die Ebene des HTML-Tag zurück.</returns>
        private string GetLevel(string content, Match element)
        {
            // Integerliste die später im Format x.x.x.x.x zurückgegeben wird.
            var response = new List<int>();

            int level = default(int);
            if (Miningmap != null && Miningmap.Any()) {
                var lastelement = Miningmap.Last();
                var lastlevel = lastelement.Key.Split('.');
                lastlevel.ToList().ForEach(l => response.Add(int.Parse(l)));
                level = response.Count;
            }

            // Ermitteln aller HTML-Tags im Dokument
            var grabbingtags = new Regex(HTMLContext.HTMLTagRegex).Matches(content, LastLevelindex);
            if (grabbingtags != null && grabbingtags.Count > 0)
            {
                // Index des aktuell untersuchten HTML-Tags
                var matchindex = default(int);

                // Durchlaufe alle HTML-Tags
                foreach (Match grabbingelement in grabbingtags)
                {
                    if (Miningmap.Keys.Count == 0 || matchindex > 0)
                    {
                        // Vorheriges HTML-Tag
                        Match prevmatch = matchindex > 0 ? grabbingtags[matchindex - 1] : null;
                        // Nächstes HTML-Tag
                        Match nextmatch = grabbingtags.Count > matchindex ? grabbingtags[matchindex + 1] : null;

                        // Inwiefern wird das Taglevel modifiziert? 0 = Ebene bleibt gleich, 1 = Ebene wird erhöht (verschachtelt), -1 = Ebene wird reduziert.
                        var modifikator = GetModifikator(prevmatch, grabbingelement, nextmatch);

                        // Ggf. ändern der aktuellen Ebene
                        level += modifikator;

                        // Betreten einer neuen, tieferen Ebene. Aus 2.1 wird 2.1.1
                        if (response.Count < level) { response.Add(0); }

                        // Bei Absteigen einer Ebene, werden die Stellen hinter der aktuellen Ebene abgeschnitten. Aus 2.1.1 wird 2.2
                        if (modifikator < 0)
                        {
                            for (int i = level; i < response.Count; i++) { response.RemoveAt(i); }
                        }

                        // Ermittlung des Namen des aktuell untersuchten HTML-Tags.
                        var tagcontent = Regex.Split(element.Value.Replace("</", string.Empty).Replace("<", string.Empty).Replace(">", string.Empty), " ");
                        var tagname = tagcontent != null && tagcontent.Length > 0 ? tagcontent[0] : string.Empty;

                        // Hochzählen des letzten Ebeneneintrags bei einem nicht schließenden HTML-Tag.
                        if (!grabbingelement.Value.Contains("</"))
                        {
                            response[response.Count - 1] = response[response.Count - 1] + 1;
                        }

                        // Das Element, dessen Ebene festgestellt werden soll wurde erreicht. Abbruch.
                        if (grabbingelement.Index == element.Index)
                        {
                            LastLevelindex = prevmatch != null ? prevmatch.Index + prevmatch.Length : 0;
                            break;
                        }
                    }

                    matchindex++;
                }
            }

            return string.Join(".", response);
        }
        private string GetKey(int index) {
            var response = string.Empty;

            for (int i = index; i > 0; i--) {
                var element = Mininglist[i];

                response =
                    element == null ? response :
                    element.Attributes.Any(attr => attr.Value.ToLower().Contains("crumb")) ? MiningConstants.Breadcrumbs :
                    element.Attributes.Any(attr => attr.Value.ToLower().Contains("navigation")) ? MiningConstants.Navigation :
                    element.Attributes.Any(attr => attr.Value.ToLower().Contains("results")) ? MiningConstants.Results :
                    element.Attributes.Any(attr => attr.Value.ToLower().Contains("ergebnisse")) ? MiningConstants.Results :
                    Regex.IsMatch(element.Tagname, "(h|H).") ? element.Content :
                    response
                ;

                if (!string.IsNullOrEmpty(response)) { break; }
            }

            return response;
        }
        private static int GetModifikator(Match prevmatch, Match match, Match nextmatch)
        {
            var response = default(int);

            if (match != null){
                var tagcontent = Regex.Split(match.Value.Replace("</", string.Empty).Replace("<", string.Empty).Replace(">", string.Empty), " ");
                var tagname = tagcontent.Length > 0 ? tagcontent[0] : string.Empty;

                response =
                    // Schließt das aktuelle HTML-Tag und das vorherige HTML-Tag öffnet nicht mit dem gleichen Namen, wird die Ebene verringert. </span></div>
                    match.Value.Contains("</" + tagname) ? (prevmatch != null && !prevmatch.Value.Contains("<" + tagname)) ? -1 :
                    // Schließt das aktuelle HTML-Tag und das vorherige HTML-Tag öffnet mit dem gleichen Namen, bleibt die Ebene gleich. <span></span>
                    0 :
                    // Schließt das aktuelle HTML-Tag mit /> (<meta.../>) ...
                    match.Value.Contains("/>") ?
                    // ... und das vorherige Tag war eine Ebene Tiefer, wird die Ebene verschachtelt.
                    // <div> 1.1
                    // <meta../> 1.1.1
                    prevmatch != null && !(prevmatch.Value.Contains("/>") || prevmatch.Value.Contains("</")) ? 1 :
                    // Ansonsten bleibt die Ebene gleich.
                    0 :
                    // In allen anderen Fällen liegt ein öffnendes HTML-Tag vor
                    // Die Ebene bleibt allerdings gleich, wenn das vorherige HTML-Tag mit /> abschließt.
                    prevmatch != null && prevmatch.Value.Contains("/>") ? 0 :
                    // Auch bleibt die Ebene gleich, wenn das vorherige HTML-Tag eine Ebene schließt.
                    prevmatch != null && prevmatch.Value.Contains("</") ? 0 :
                    // In allen anderen Fällen wird eine neue Ebene betreten.
                    1
                ;
            }

            return response;
        }

        private static string RemoveFormatters(string content)
        {
            var remove = new List<string> { "\t", "\n", "\r" };
            remove.ForEach(rm => { content = content.Replace(rm, " "); });
            return content;
        }    
    }
    public class WebMiningResponse
    {
        public WebMiningResponse() {
        }

        public WebMiningItem MiningItem { get; set; }
    }
    public class WebMiningItem
    {
        public WebMiningItem() {
            Title =
                string.Empty
            ;

            Content = new Dictionary<string, List<string>>();
            Subinformation = new List<WebMiningItem>();
        }

        public string Title { get; set; }
        public WebMiningItem Parent { get; set; }
        public Dictionary<string, List<string>> Content { get; set; }
        public List<WebMiningItem> Subinformation { get; set; }
    }

    class WebminingElement
    {
        public WebminingElement() {
            Attributes = new Dictionary<string, string>();
        }

        public string Level { get; set; }
        public string HtmlContent { get; set; }
        public string FormattedContent { get; set; }
        public string Content { get; set; }
        public string Tagname { get; set; }
        public Dictionary<string, string> Attributes { get; set; }
    }
}
