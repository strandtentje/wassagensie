using System.Collections.Generic;
using WasSagenSie;

internal class SourceBoard : BoardCommand
{

    public override string ToString()
    {
        return string.Format("Ruw {0}x{1}", Width, Height);
    }
}