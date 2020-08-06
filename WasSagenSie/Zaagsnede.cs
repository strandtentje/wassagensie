using System.Collections.Generic;
using WasSagenSie;

internal class Zaagsnede : Command
{
    public short Thickness { get; private set; }

    public override ResultSet Run(ZaagContext context)
    {
        context.Sawcut = Thickness;
        return new ResultSet(ResultType.Dimension, string.Format("Zaagsnede: {0}", Thickness));
    }

    public override void SetArgs(string[] commandArgs)
    {
        if ((commandArgs.Length == 1) && short.TryParse(commandArgs[0], out short thickness))
        {
            this.Thickness = thickness;
        }
        else
        {
            throw new CommandSyntaxException("Zaagsnede dient dikte te specificeren");
        }
    }
}