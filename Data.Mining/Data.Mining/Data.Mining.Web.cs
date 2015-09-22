using System.Collections.Generic;

namespace Data.Mining.Web
{
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
}
