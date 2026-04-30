namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Erkenningen.SchorsErkenning;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen;

public record SchorsErkenningCommand(VCode VCode, TeSchorsenErkenning Erkenning)
{
    protected virtual bool PrintMembers(System.Text.StringBuilder builder)
    {
        builder.Append($"VCode = {VCode}, ");
        builder.Append($"ErkenningId = {Erkenning.ErkenningId}, ");
        builder.Append($"RedenSchorsing = {Erkenning.RedenSchorsing}, ");
        return true;
    }
}
