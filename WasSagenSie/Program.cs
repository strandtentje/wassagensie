using System;
using System.Collections.Generic;
using System.Linq;

namespace WasSagenSie
{
    class MainClass
    {
        static string[] ExitStrings = new string[] { "exit", "quit" };
        static ZaagContext context = new ZaagContext();

        public static void Main(string[] args)
        {
            Console.Write("wassagensie> ");
            string input = Console.ReadLine();
            while (!ExitStrings.Contains(input.ToLower().Trim()))
            {
                if (TryReadCommands(input, out CommandList commands) && TryRunSolver(commands, out ResultSet results))
                    results.Print(0, PrintResult);

                Console.Write("wassagensie> ");
                input = Console.ReadLine();
            }
        }

        private static bool TryRunSolver(CommandList commands, out ResultSet results)
        {
            try
            {
                results = Solver.For(commands).Execute(context);
                return true;
            }
            catch (ExecutionException ex)
            {
                results = null;
                WriteError(ex);
                return false;
            }
        }

        public static void PrintResult(int depth, string content, ResultType rt)
        {
            Console.Write(new String(' ', depth));
            switch (rt)
            {
                case ResultType.Heading:
                    Console.BackgroundColor = ConsoleColor.White;
                    Console.ForegroundColor = ConsoleColor.Black;
                    break;
                case ResultType.Dimension:
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case ResultType.Good:
                    Console.BackgroundColor = ConsoleColor.DarkGray;
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;
                case ResultType.Bad:
                    Console.BackgroundColor = ConsoleColor.DarkGray;
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                default:
                    Console.ResetColor();
                    break;
            }
            Console.Write(content);
            Console.ResetColor();
            Console.WriteLine();
        }

        private static bool TryReadCommands(string input, out CommandList commands)
        {
            try
            {
                commands = CommandList.FromUserInput(input);
                return true;
            }
            catch (CommandSyntaxException ex)
            {
                commands = null;
                WriteError(ex);
                return false;
            }
        }

        private static void WriteError(Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Execution was halted due to a " + ex.GetType().Name);
            Console.WriteLine(ex.Message);
            Console.ResetColor();
        }
    }
}
