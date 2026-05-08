namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Erkenningen.HefSchorsingErkenningOp;

using System.Text;
using AssociationRegistry.DecentraalBeheer.Vereniging;

public record HefSchorsingErkenningOpCommand(VCode VCode, int ErkenningId)
{
    protected virtual bool PrintMembers(StringBuilder builder)
    {
        builder.Append($"VCode = {VCode}, ");
        builder.Append($"ErkenningId = {ErkenningId}, ");
        return true;
    }
}
