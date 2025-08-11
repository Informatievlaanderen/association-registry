namespace AssociationRegistry.Grar.AdresMatch;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Events;

public abstract record AdresMatchResult
{
    public abstract IEvent ToEvent(VCode vCode, int locatieId);
}

public record LocatieVerwijderdResult : AdresMatchResult
{
    public override IEvent ToEvent(VCode vCode, int locatieId)
        => new AdresKonNietOvergenomenWordenUitAdressenregister(
            vCode,
            locatieId,
            string.Empty,
            AdresKonNietOvergenomenWordenUitAdressenregister.RedenLocatieWerdVerwijderd);
}

public record AdresNietGevondenResult(Locatie Locatie) : AdresMatchResult
{
    public override IEvent ToEvent(VCode vCode, int locatieId)
        => new AdresWerdNietGevondenInAdressenregister(
            vCode,
            locatieId,
            Locatie.Adres.Straatnaam,
            Locatie.Adres.Huisnummer,
            Locatie.Adres.Busnummer,
            Locatie.Adres.Postcode,
            Locatie.Adres.Gemeente.Naam);
}

public record AdresNietUniekResult(NietUniekeAdresMatchUitAdressenregister[] Matches) : AdresMatchResult
{
    public override IEvent ToEvent(VCode vCode, int locatieId)
        => new AdresNietUniekInAdressenregister(vCode, locatieId, Matches);
}

public record AdresGevondenResult(
    Registratiedata.AdresId AdresId,
    Registratiedata.AdresUitAdressenregister Adres) : AdresMatchResult
{
    public override IEvent ToEvent(VCode vCode, int locatieId)
        => new AdresWerdOvergenomenUitAdressenregister(vCode, locatieId, AdresId, Adres);
}

public record AdresKonNietOvergenomenResult(string AdresString, string Reden) : AdresMatchResult
{
    public override IEvent ToEvent(VCode vCode, int locatieId)
        => new AdresKonNietOvergenomenWordenUitAdressenregister(vCode, locatieId, AdresString, Reden);
}
