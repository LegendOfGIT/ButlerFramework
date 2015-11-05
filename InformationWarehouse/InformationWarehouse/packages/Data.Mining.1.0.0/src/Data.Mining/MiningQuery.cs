using System;
using System.Collections.Generic;

namespace Data.Mining
{
    public class MiningQuery
    {
        public string Target { get; set; }
        public IEnumerable<string> Expressions { get; set; }

        public MiningQuery Query { get; set; }
    }
}
