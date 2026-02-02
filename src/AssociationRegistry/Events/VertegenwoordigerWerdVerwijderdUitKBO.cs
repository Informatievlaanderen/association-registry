namespace AssociationRegistry.Events;

using System.Runtime.Serialization;
using Vereniging.Bronnen;

[Obsolete("These are the upcasted events, only use this in projections and State")]
public record VertegenwoordigerWerdVerwijderdUitKBO(
    int VertegenwoordigerId,
    string Insz,
    string Voornaam,
    string Achternaam
) : IEvent
{
    [IgnoreDataMember]
    public Bron Bron => Bron.KBO;

    protected virtual bool PrintMembers(System.Text.StringBuilder builder)
    {
        builder.Append($"VertegenwoordigerId = {VertegenwoordigerId}, ");
        return true;
    }
}

public record VertegenwoordigerWerdVerwijderdUitKBOZonderPersoonsgegevens(Guid RefId, int VertegenwoordigerId) : IEvent
{
    [IgnoreDataMember]
    public Bron Bron => Bron.KBO;
}
