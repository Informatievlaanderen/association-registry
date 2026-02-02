namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Bankrekeningen.VoegBankrekeningToe;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.DecentraalBeheer.Vereniging.Bankrekeningen;

public record VoegBankrekeningnummerToeCommand(VCode VCode, ToeTevoegenBankrekeningnummer Bankrekeningnummer)
{
    protected virtual bool PrintMembers(System.Text.StringBuilder builder)
    {
        builder.Append($"VCode = {VCode}, ");
        return true;
    }
}
