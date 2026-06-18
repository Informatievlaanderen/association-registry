namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Erkenningen.CorrigeerRedenSchorsingErkenning;

using System.Text;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen;

public record CorrigeerRedenSchorsingErkenningCommand(VCode VCode, TeCorrigerenRedenSchorsingErkenning Erkenning)
{
    protected virtual bool PrintMembers(StringBuilder builder)
    {
        builder.Append($"VCode = {VCode}, ");
        builder.Append($"ErkenningId = {Erkenning.ErkenningId}, ");
        builder.Append($"RedenSchorsing = {Erkenning.RedenSchorsing}, ");
        return true;
    }
}
