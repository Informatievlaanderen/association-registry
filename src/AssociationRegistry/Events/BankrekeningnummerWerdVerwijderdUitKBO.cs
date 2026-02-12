namespace AssociationRegistry.Events;

using System.Runtime.Serialization;
using Vereniging.Bronnen;

public record BankrekeningnummerWerdVerwijderdUitKBO(int BankrekeningnummerId, string Iban) : IEvent
{
    [IgnoreDataMember]
    public Bron Bron => Bron.KBO;

    protected virtual bool PrintMembers(System.Text.StringBuilder builder)
    {
        builder.Append($"BankrekeningnummerId = {BankrekeningnummerId}, ");
        return true;
    }
}
