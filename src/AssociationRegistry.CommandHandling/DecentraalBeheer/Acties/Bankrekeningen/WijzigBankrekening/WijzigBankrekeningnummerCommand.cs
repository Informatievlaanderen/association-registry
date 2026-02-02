namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Bankrekeningen.WijzigBankrekening;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.DecentraalBeheer.Vereniging.Bankrekeningen;

public record WijzigBankrekeningnummerCommand(VCode VCode, TeWijzigenBankrekeningnummer Bankrekeningnummer)
{
    protected virtual bool PrintMembers(System.Text.StringBuilder builder)
    {
        builder.Append($"VCode = {VCode}, ");
        return true;
    }
}
