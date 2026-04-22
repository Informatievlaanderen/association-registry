namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Erkenningen.RegistreerErkenning;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen;

public record RegistreerErkenningCommand(VCode VCode, TeRegistrerenErkenning Erkenning)
{
    protected virtual bool PrintMembers(System.Text.StringBuilder builder)
    {
        builder.Append($"VCode = {VCode}, ");
        builder.Append($"ErkenningsPeriode = {Erkenning.ErkenningsPeriode}, ");
        builder.Append($"Hernieuwingsdatum = {Erkenning.Hernieuwingsdatum}, ");
        builder.Append($"HernieuwingsUrl = {Erkenning.HernieuwingsUrl}, ");
        builder.Append($"IpdcProductNummer = {Erkenning.IpdcProductNummer}, ");
        return true;
    }
}
