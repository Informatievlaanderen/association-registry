namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Erkenningen.RegistreerErkenning;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen;

public record RegistreerErkenningCommand(VCode VCode, TeRegistrerenErkenning erkenning)
{
    protected virtual bool PrintMembers(System.Text.StringBuilder builder)
    {
        builder.Append($"VCode = {VCode}, ");
        builder.Append($"StartDatum = {erkenning.Startdatum}, ");
        builder.Append($"Einddatum = {erkenning.Einddatum}, ");
        builder.Append($"Hernieuwingsdatum = {erkenning.Hernieuwingsdatum}, ");
        builder.Append($"HernieuwingsUrl = {erkenning.HernieuwingsUrl}, ");
        builder.Append($"IpdcProductNummer = {erkenning.IpdcProductNummer}, ");
        return true;
    }
}
