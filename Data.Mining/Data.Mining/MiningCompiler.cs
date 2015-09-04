﻿using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Data.Mining
{
    public class MiningCompiler
    {
        public List<MiningCommand> ContextCommandset { get; set; }

        public IEnumerable<MiningCommand> ParseCommandset(IEnumerable<string> lines, int index)
        {
            var commands = default(List<MiningCommand>);

            var line = lines.ToArray()[index];
            var level = line.GetLevel();
            var commandlines = new List<KeyValuePair<int, string>>();
            foreach(var followingline in lines.Skip(index))
            {
                if(followingline.GetLevel() < level)
                {
                    break;
                }

                if(followingline.GetLevel() == level)
                {
                    commandlines.Add(new KeyValuePair<int, string>(lines.ToList().IndexOf(followingline), followingline));
                }
            }
            //    lines
            //        .Skip(index)
            //            .Where(l => l.GetLevel() == level && !string.IsNullOrEmpty(l))
            //                .Select(l => new KeyValuePair<int, string>(lines.ToList().IndexOf(l), l))
            //;

            foreach (var commandline in commandlines)
            {
                var commandtext = commandline.Value;
                var tokens = default(IEnumerable<string>);

                //  Enthält das Kommando ein !, wird dieses Kommando solange wiederholt wie es mindestens ein Ergebnis zurückgibt.
                var isLoop = commandtext.Contains("!");
                commandtext = commandtext.Replace("!", string.Empty);
                //  Ermittlung des Kommandoziels
                tokens = Regex.Split(commandtext, ">>");
                var target =
                    (tokens != null && tokens.Count() > 1 ? (tokens.Skip(1).FirstOrDefault() ?? string.Empty) : string.Empty).Trim()
                ;
                commandtext = (tokens != null ? (tokens.FirstOrDefault() ?? string.Empty) : string.Empty).Trim();
                //  Ermittlung eines Attributes
                tokens = commandtext.Split('@');
                var attribute = tokens != null && tokens.Count() > 1 ? (tokens.Skip(1).FirstOrDefault() ?? string.Empty).Trim() : string.Empty;
                commandtext = string.IsNullOrEmpty(attribute) ? commandtext : tokens.FirstOrDefault() ?? commandtext;

                commands = commands ?? new List<MiningCommand>();
                var command = new MiningCommand
                {
                    AttributID = attribute,
                    IsLoop = isLoop,
                    Command = commandtext,
                    Target = target
                };

                this.ContextCommandset = this.ContextCommandset ?? new List<MiningCommand>();
                var subcommands = default(List<MiningCommand>);
                var followingLine = new KeyValuePair<int, string>(commandline.Key + 1, lines.Skip(commandline.Key + 1).FirstOrDefault() ?? string.Empty);
                if (!string.IsNullOrEmpty(followingLine.Value) && followingLine.Value.GetLevel() == level + 1)
                {
                    subcommands = subcommands ?? new List<MiningCommand>();
                    var set = ParseCommandset(lines, followingLine.Key);
                    subcommands.AddRange(set);
                    this.ContextCommandset.AddRange(set);
                }

                command.Subcommands = subcommands;
                commands.Add(command);
                this.ContextCommandset.Add(command);
            }

            return commands;
        }
    }
}
