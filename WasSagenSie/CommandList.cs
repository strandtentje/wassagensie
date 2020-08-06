using System;
using System.Collections.Generic;

namespace WasSagenSie
{
    internal class CommandList : List<Command>
    {
        public CommandList(IEnumerable<Command> collection) : base(collection)
        {
        }

        internal static CommandList FromUserInput(string input)
        {
            var commandLines = input.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            var executableCommands = new List<Command>();
            foreach (var commandLine in commandLines)
            {
                var segments = commandLine.Split(':');
                if (segments.Length == 1)
                {
                    executableCommands.Add(Command.FromInstructionSegment(segments[0]));
                }
                else if (segments.Length == 2)
                {
                    var newComm = Command.FromInstructionSegment(segments[0]);
                    var commandArgs = segments[1].Split(',');
                    newComm.SetArgs(commandArgs);
                    executableCommands.Add(newComm);
                }
                else
                {
                    throw new CommandSyntaxException("Commands may only consist of a single instruction and a single collection of arguments");
                }
            }
            return new CommandList(executableCommands);
        }
    }
}