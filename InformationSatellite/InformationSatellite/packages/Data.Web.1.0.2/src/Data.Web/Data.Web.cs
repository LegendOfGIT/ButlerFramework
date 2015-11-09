using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace Data.Web
{
    public static class WebUtility
    {
        public static string GetWebsiteContent(Uri URL, Encoding Encoding = null)
        {
            return URL == null ? string.Empty : GetWebsiteContent(URL.ToString(), Encoding);
        }
        public static string GetWebsiteContent(string URL, Encoding Encoding = null)
        {
            string result = string.Empty;

            try
            {
                var request = WebRequest.Create(URL);
                var response = request.GetResponse();

                var stream = response == null ? null : response.GetResponseStream();
                using (var reader = Encoding == null ? new StreamReader(stream, true) : new StreamReader(stream, Encoding, false))
                {
                    stream.Flush();
                    result = reader.ReadToEnd();
                }
            }
            catch (Exception) { }

            return result;
        }

        public static string RemoveHTMLComments(this string content)
        {
            var commentmatch = Regex.Match(content, @"<!--[\s\S]*?-->");
            while(commentmatch != null && commentmatch.Success){
                content = content.Replace(commentmatch.Value, string.Empty);
                commentmatch = Regex.Match(content, @"<!--[\s\S]*?-->");
            }

            return content;
        }
        public static string RemoveHTMLDecorations(this string content)
        {
            var decorations = new List<string> { "b", "i" };
            foreach (var decoration in decorations)
            {
                content = Regex.Replace(content, "(<" + decoration + ">|</" + decoration + ">)", string.Empty);
            }

            return content;
        }
        public static string RemoveHTML(this string content)
        {
            foreach (var HTM in HTMLContext.HTML)
            {
                content = Regex.Replace(content, HTM.Key, HTM.Value);
            }

            // Unicodes entfernen
            content = System.Net.WebUtility.HtmlDecode(content);

            return content;
        }
        public static string Take(string content, Match hit, string begin, string end)
        {
            var response = new StringBuilder();

            int level = 0;
            int index = 0;

            var matches = new Regex(HTMLContext.HTMLTagRegex).Matches(content, hit.Index);
            if (matches != null && matches.Count > 0)
            {
                foreach (Match match in matches)
                {
                    if (index > 0)
                    {
                        response.Append(content.Substring(index, match.Index - index));
                    }

                    level += match.Value.StartsWith(begin) ? 1 : match.Value.StartsWith(end) ? -1 : 0;

                    index = match.Index;

                    if (level == 0)
                    {
                        response.Append(match.Value);
                        break;
                    }
                }
            }

            return response.ToString();
        }
    }
    public static class HTMLContext
    {
        // Regular-Expression zum ermitteln von XML-Tags
        public const string HTMLTagRegex = @"</?\w+((\s+\w+(\s*=\s*(?:"".*?""|'.*?'|[^'"">\s]+))?)+\s*|\s*)/?>";

        public static Dictionary<string, string> HTML = new Dictionary<string, string>{
            { "<.*?>", string.Empty },
            { "&Auml;", "Ä" }, { "&auml;", "ä" },
            { "&Ouml;", "Ö" }, { "&ouml;", "ö" },
            { "&Uuml;", "Ü" }, { "&uuml;", "ü" },
            { "&szlig;", "ß" }, {"&quot;", "\""}, { "&euro;", "€" }, { "&amp;", "&" }, { "&nbsp;", " " }, {"&gt;", ">"} , {"&lt;", "<"}
        };
    }
}
