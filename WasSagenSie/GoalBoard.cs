using System.Collections.Generic;
using WasSagenSie;

internal class GoalBoard : BoardCommand
{
    public override string ToString()
    {
        return string.Format("Doel {0}x{1}", Width, Height);
    }
}