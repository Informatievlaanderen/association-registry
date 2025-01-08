namespace AssociationRegistry.Events;


using System.Runtime.Serialization;
using Vereniging.Bronnen;

public record VerenigingMetRechtspersoonlijkheidWerdGeregistreerd(
    string VCode,
    string KboNummer,
    string Rechtsvorm,
    string Naam,
    string KorteNaam,
    DateOnly? Startdatum) : IEvent
{
    [IgnoreDataMember]
    public Bron Bron
        => Bron.KBO;
}
