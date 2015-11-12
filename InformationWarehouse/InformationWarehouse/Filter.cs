using System.Collections.Generic;

namespace InformationWarehouse
{
    public class Filter
    {
        public string Target { get; set; }
        public object Maximum { get; set; }
        public object Minimum { get; set; }
        public object Value { get; set; }

        public IEnumerable<Filter> And { get; set; }
        public IEnumerable<Filter> Or { get; set; }
    }
}