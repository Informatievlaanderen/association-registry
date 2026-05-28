namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Erkenningen.WijzigErkenning;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen;

public record WijzigErkenningCommand(VCode VCode, TeWijzigenErkenning Erkenning)
{
    protected virtual bool PrintMembers(System.Text.StringBuilder builder)
    {
        builder.Append($"VCode = {VCode}, ");
        builder.Append($"ErkenningId = {Erkenning.ErkenningId}, ");
        builder.Append($"StartDatum = {Erkenning.StartDatum}, ");
        builder.Append($"EindDatum = {Erkenning.EindDatum}, ");
        builder.Append($"Hernieuwingsdatum = {Erkenning.Hernieuwingsdatum}, ");
        builder.Append($"HernieuwingsUrl = {Erkenning.HernieuwingsUrl}, ");
        builder.Append($"RedenVanWijziging = {Erkenning.RedenVanWijziging}, ");
        return true;
    }
}
