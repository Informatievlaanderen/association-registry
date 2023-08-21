namespace AssociationRegistry.Events;

using System.Runtime.Serialization;
using Framework;

public record VerenigingMetRechtspersoonlijkheidWerdGeregistreerd(
    string VCode,
    string KboNummer,
    string Rechtsvorm,
    string Naam,
    string KorteNaam,
    DateOnly? Startdatum) : IEvent
{
    [IgnoreDataMember]
    public string Bron
        => AssociationRegistry.Vereniging.Bronnen.Bron.KBO;
}
