namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Erkenningen.CorrigeerErkenning;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen;

public record CorrigeerErkenningCommand(VCode VCode, TeCorrigerenErkenning Erkenning)
{
    protected virtual bool PrintMembers(System.Text.StringBuilder builder)
    {
        builder.Append($"VCode = {VCode}, ");
        builder.Append($"ErkenningId = {Erkenning.ErkenningId}, ");
        builder.Append($"StartDatum = {Erkenning.StartDatum}, ");
        builder.Append($"EindDatum = {Erkenning.EindDatum}, ");
        builder.Append($"Hernieuwingsdatum = {Erkenning.Hernieuwingsdatum}, ");
        builder.Append($"HernieuwingsUrl = {Erkenning.HernieuwingsUrl}, ");
        return true;
    }
}
