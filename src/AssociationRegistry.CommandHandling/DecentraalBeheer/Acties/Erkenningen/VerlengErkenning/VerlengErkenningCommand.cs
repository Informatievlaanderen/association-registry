namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Erkenningen.VerlengErkenning;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen;

public record VerlengErkenningCommand(VCode VCode, TeVerlengenErkenning Erkenning)
{
    protected virtual bool PrintMembers(System.Text.StringBuilder builder)
    {
        builder.Append($"VCode = {VCode}, ");
        builder.Append($"ErkenningId = {Erkenning.ErkenningId}, ");
        builder.Append($"Einddatum = {Erkenning.Einddatum}, ");
        builder.Append($"Hernieuwingsdatum = {Erkenning.Hernieuwingsdatum}, ");
        return true;
    }
}
