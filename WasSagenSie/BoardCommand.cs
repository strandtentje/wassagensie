using System.Collections.Generic;
using WasSagenSie;

internal abstract class BoardCommand : Command
{
    public short Width { get; private set; }
    public short Height { get; private set; }
    public short Amount { get; private set; }

    public string ToSizeString()
    {
        return string.Format("{0}x{1}", Width, Height);
    }

    public override void SetArgs(string[] commandArgs)
    {
        if ((commandArgs.Length > 1) && short.TryParse(commandArgs[0], out short width) && short.TryParse(commandArgs[1], out short height))
        {
            if (width > height)
            {
                this.Width = width;
                this.Height = height;
            }
            else
            {
                this.Height = width;
                this.Width = height;
            }
            if ((commandArgs.Length == 3) && short.TryParse(commandArgs[2], out short amount))
            {
                this.Amount = amount;
            }
            else
            {
                this.Amount = 9999;
            }
        }
        else
        {
            throw new CommandSyntaxException("Bronplank verwacht breedte:diepte(:aantal)");
        }
    }

    public override ResultSet Run(ZaagContext context)
    {
        if (this is SourceBoard) context.AddCuttable(Width, Height, Amount);
        else if (this is GoalBoard) context.AddPendingTarget(Width, Height, Amount);

        return new ResultSet(ResultType.Dimension, string.Format("Nieuwe stapel van {0} planken {1}", Amount, ToString()));
    }
}