namespace AssociationRegistry.Events;

[Obsolete]
public record AfdelingWerdGeregistreerd(
    string VCode,
    string Naam,
    AfdelingWerdGeregistreerd.MoederverenigingsData Moedervereniging,
    string KorteNaam,
    string KorteBeschrijving,
    DateOnly? Startdatum,
    Registratiedata.Doelgroep Doelgroep,
    Registratiedata.Contactgegeven[] Contactgegevens,
    Registratiedata.Locatie[] Locaties,
    Registratiedata.Vertegenwoordiger[] Vertegenwoordigers,
    Registratiedata.HoofdactiviteitVerenigingsloket[] HoofdactiviteitenVerenigingsloket
) : IEvent
{
    // [IgnoreDataMember]
    // public Bron Bron
    //     => Bron.Initiator;

    public record MoederverenigingsData(string KboNummer, string VCode, string Naam);

    protected virtual bool PrintMembers(System.Text.StringBuilder builder)
    {
        builder.Append($"VCode = {VCode}, ");
        builder.Append($"Naam = {Naam}, ");
        builder.Append($"Moedervereniging = {Moedervereniging}, ");
        builder.Append($"KorteNaam = {KorteNaam}, ");
        builder.Append($"KorteBeschrijving = {KorteBeschrijving}, ");
        builder.Append($"Startdatum = {Startdatum}, ");
        builder.Append($"Doelgroep = {Doelgroep}, ");
        builder.Append($"Contactgegevens = {Contactgegevens.Length} items, ");
        builder.Append($"Locaties = {Locaties.Length} items, ");
        builder.Append($"HoofdactiviteitenVerenigingsloket = {HoofdactiviteitenVerenigingsloket.Length} items");
        return true;
    }
}
