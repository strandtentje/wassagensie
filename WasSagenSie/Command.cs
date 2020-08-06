using System;
using System.Collections.Generic;
using WasSagenSie;

abstract class Command
{
    internal static Command FromInstructionSegment(string instructionSegment)
    {
        if (Enum.TryParse<CommandType>(instructionSegment, out CommandType instructionType))
        {
            switch (instructionType)
            {
                case CommandType.BronPlank:
                    return new SourceBoard();
                case CommandType.DoelPlank:
                    return new GoalBoard();
                case CommandType.RekenUit:
                    return new CalculationRunner();
                case CommandType.Zaagsnede:
                    return new Zaagsnede();
                case CommandType.Opslaan:
                case CommandType.Laden:
                case CommandType.Legen:
                default:
                    throw new CommandSyntaxException(string.Format("Command recognized but not implemented {0}", instructionSegment));
                    break;
            }

        }
        else
        {
            throw new CommandSyntaxException(string.Format("Unknown command {0}", instructionSegment));
        }
    }

    public abstract ResultSet Run(ZaagContext context);

    public abstract void SetArgs(string[] commandArgs);


}