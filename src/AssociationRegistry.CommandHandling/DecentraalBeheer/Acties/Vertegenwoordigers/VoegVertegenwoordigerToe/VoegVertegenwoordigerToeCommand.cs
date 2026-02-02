namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Vertegenwoordigers.VoegVertegenwoordigerToe;

using System.Text;
using AssociationRegistry.DecentraalBeheer.Vereniging;

public record VoegVertegenwoordigerToeCommand(VCode VCode, Vertegenwoordiger Vertegenwoordiger)
{
    protected virtual bool PrintMembers(StringBuilder builder)
    {
        builder.Append($"VCode = {VCode}, ");
        return true;
    }
}
