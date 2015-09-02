using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using Data.Web;

namespace Data.Mining.Web
{
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
