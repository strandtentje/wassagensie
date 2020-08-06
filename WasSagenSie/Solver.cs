using System;
using System.Collections.Generic;

namespace WasSagenSie
{
    internal class Solver
    {
        private List<Command> Commands;

        internal Solver(List<Command> commands)
        {
            this.Commands = commands;
        }

        internal static Solver For(List<Command> commands)
        {
            return new Solver(commands);
        }

        internal ResultSet Execute(ZaagContext context)
        {
            var results = new ResultSet(ResultType.Heading, "Invoerresultaten:");
            foreach (var command in Commands)
            {
                results.Add(command.Run(context));
            }
            return results;
        }
    }
}