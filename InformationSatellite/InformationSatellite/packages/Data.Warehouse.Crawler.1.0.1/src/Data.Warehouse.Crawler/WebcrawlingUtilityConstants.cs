using System.Collections.Generic;

namespace Data.Warehouse.Crawler
{
    public class WebcrawlingUtilityConstants
    {
        public const string BaseUri = "base.uri";
        public const string CommandBrowse = "browse()";
        public const string CurrentUri = "current.uri";

        public static List<string> MiningCommands = new List<string>
        {
            CommandBrowse
        };
    }
}
