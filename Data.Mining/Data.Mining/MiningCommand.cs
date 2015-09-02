using System.Collections.Generic;

namespace Data.Mining
{
    public class MiningCommand
    {
        public string AttributID { get; set; }
        public string Command { get; set; }
        public string Target { get; set; }
        public bool IsLoop { get; set; }
        public IEnumerable<MiningCommand> Subcommands { get; set; }
    }
}
