namespace AssociationRegistry.Vereniging;

using Emails;
using Events;
using Exceptions;
using Framework;
using Grar;
using Grar.Exceptions;
using Grar.Models;
using SocialMedias;
using System.Diagnostics.Contracts;
using System.Net;
using System.Numerics;
using TelefoonNummers;

public class VerenigingOfAnyKind : VerenigingsBase, IHydrate<VerenigingState>
{
    private static void MustNotBeInFuture(Datum datum, DateOnly today)
        => Throw<StartdatumMagNietInToekomstZijn>.If(datum.IsInFutureOf(today));

    private static Registratiedata.Contactgegeven[] ToEventContactgegevens(Contactgegeven[] contactgegevens)
        => contactgegevens.Select(Registratiedata.Contactgegeven.With).ToArray();

    private static Registratiedata.HoofdactiviteitVerenigingsloket[] ToHoofdactiviteitenLijst(
        HoofdactiviteitVerenigingsloket[] hoofdactiviteitenVerenigingsloketLijst)
        => hoofdactiviteitenVerenigingsloketLijst.Select(Registratiedata.HoofdactiviteitVerenigingsloket.With).ToArray();

    private static Registratiedata.Vertegenwoordiger[] ToVertegenwoordigersLijst(Vertegenwoordiger[] vertegenwoordigersLijst)
        => vertegenwoordigersLijst.Select(Registratiedata.Vertegenwoordiger.With).ToArray();

    private static Registratiedata.Locatie[] ToLocatieLijst(Locatie[] locatieLijst)
        => locatieLijst.Select(Registratiedata.Locatie.With).ToArray();

    public void VoegContactgegevenToe(Contactgegeven contactgegeven)
    {
        var toegevoegdContactgegeven = State.Contactgegevens.VoegToe(contactgegeven);

        AddEvent(ContactgegevenWerdToegevoegd.With(toegevoegdContactgegeven));
    }

    public void WijzigContactgegeven(int contactgegevenId, string? waarde, string? beschrijving, bool? isPrimair)
    {
        var gewijzigdContactgegeven = State.Contactgegevens.Wijzig(contactgegevenId, waarde, beschrijving, isPrimair);

        if (gewijzigdContactgegeven is null)
            return;

        AddEvent(ContactgegevenWerdGewijzigd.With(gewijzigdContactgegeven));
    }

    public void VerwijderContactgegeven(int contactgegevenId)
    {
        var verwijderdContactgegeven = State.Contactgegevens.Verwijder(contactgegevenId);
        AddEvent(ContactgegevenWerdVerwijderd.With(verwijderdContactgegeven));
    }

    public void VoegVertegenwoordigerToe(Vertegenwoordiger vertegenwoordiger)
    {
        var toegevoegdeVertegenwoordiger = State.Vertegenwoordigers.VoegToe(vertegenwoordiger);

        AddEvent(VertegenwoordigerWerdToegevoegd.With(toegevoegdeVertegenwoordiger));
    }

    public void WijzigVertegenwoordiger(
        int vertegenwoordigerId,
        string? rol,
        string? roepnaam,
        Email? email,
        TelefoonNummer? telefoonNummer,
        TelefoonNummer? mobiel,
        SocialMedia? socialMedia,
        bool? isPrimair)
    {
        var gewijzigdeVertegenwoordiger =
            State.Vertegenwoordigers.Wijzig(vertegenwoordigerId, rol, roepnaam, email, telefoonNummer, mobiel, socialMedia, isPrimair);

        if (gewijzigdeVertegenwoordiger is null)
            return;

        AddEvent(VertegenwoordigerWerdGewijzigd.With(gewijzigdeVertegenwoordiger));
    }

    public void VerwijderVertegenwoordiger(int vertegenwoordigerId)
    {
        var vertegenwoordiger = State.Vertegenwoordigers.Verwijder(vertegenwoordigerId);
        AddEvent(VertegenwoordigerWerdVerwijderd.With(vertegenwoordiger));
    }

    public void VoegLocatieToe(Locatie toeTeVoegenLocatie)
    {
        Throw<MaatschappelijkeZetelIsNietToegestaan>.If(toeTeVoegenLocatie.Locatietype == Locatietype.MaatschappelijkeZetelVolgensKbo);

        var toegevoegdeLocatie = State.Locaties.VoegToe(toeTeVoegenLocatie);

        AddEvent(LocatieWerdToegevoegd.With(toegevoegdeLocatie));
    }

    public void WijzigLocatie(int locatieId, string? naam, Locatietype? locatietype, bool? isPrimair, AdresId? adresId, Adres? adres)
    {
        Throw<MaatschappelijkeZetelIsNietToegestaan>.If(locatietype is not null &&
                                                        locatietype == Locatietype.MaatschappelijkeZetelVolgensKbo);

        var gewijzigdeLocatie = State.Locaties.Wijzig(locatieId, naam, locatietype, isPrimair, adresId, adres);

        if (gewijzigdeLocatie is null)
            return;

        AddEvent(LocatieWerdGewijzigd.With(gewijzigdeLocatie));
    }

    public void VerwijderLocatie(int locatieId)
    {
        var locatie = State.Locaties.Verwijder(locatieId);
        AddEvent(LocatieWerdVerwijderd.With(State.VCode, locatie));
    }

    public async Task HeradresseerLocaties(List<LocatieWithAdres> locatiesMetAdressen, string idempotenceKey, IGrarClient grarClient)
    {
        if (State.HandledIdempotenceKeys.Contains(idempotenceKey))
            return;

        foreach (var (locatieId, adresDetail) in locatiesMetAdressen)
        {
            if (!State.Locaties.HasKey(locatieId))
                continue;

            var origineleGemeentenaam = State.Locaties[locatieId].Adres!.Gemeente;

            var postalInformation = await grarClient.GetPostalInformation(adresDetail.Postcode);

            var adresDetailUitAdressenregister = AdresDetailUitAdressenregister
                                                .FromResponse(adresDetail)
                                                .DecorateWithPostalInformation(
                                                     origineleGemeentenaam, postalInformation);

            AddEvent(new AdresWerdGewijzigdInAdressenregister(VCode,
                                                              locatieId,
                                                              adresDetailUitAdressenregister,
                                                              idempotenceKey));
        }
    }

    public async Task SyncAdresLocaties(List<LocatieWithAdres> locatiesMetAdressen, string idempotenceKey, IGrarClient grarClient)
    {
        if (State.HandledIdempotenceKeys.Contains(idempotenceKey))
            return;

        foreach (var (locatieId, adresDetail) in locatiesMetAdressen)
        {
            if (!State.Locaties.HasKey(locatieId))
                continue;

            var locatie = State.Locaties[locatieId];

            if (adresDetail is null || !adresDetail.IsActief)
            {
                AddEvent(new AdresWerdOntkoppeldVanAdressenregister(
                             VCode,
                             locatieId,
                             Registratiedata.AdresId.With(locatie.AdresId),
                             Registratiedata.Adres.With(locatie.Adres)));
                continue;
            }

            var origineleGemeentenaam = locatie.Adres!.Gemeente;

            var postalInformation = await grarClient.GetPostalInformation(adresDetail.Postcode);

            var adresDetailUitAdressenregister = AdresDetailUitAdressenregister
                                                .FromResponse(adresDetail)
                                                .DecorateWithPostalInformation(origineleGemeentenaam, postalInformation);

            if (HeeftVerschillenBinnenAdres(locatie, adresDetailUitAdressenregister.Adres))
            {
                AddEvent(new AdresWerdGewijzigdInAdressenregister(VCode,
                                                                  locatieId,
                                                                  adresDetailUitAdressenregister,
                                                                  idempotenceKey));
            }
        }
    }

    public async Task ProbeerAdresTeMatchen(IGrarClient grarClient, int locatieId, CancellationToken cancellationToken)
    {
        var locatie = State.Locaties.SingleOrDefault(s => s.LocatieId == locatieId);

        try
        {
            var @event = await GetAdresMatchEvent(locatieId, locatie, grarClient, cancellationToken);

            if (@event is not AdresWerdOvergenomenUitAdressenregister adresWerdOvergenomen)
            {
                AddEvent(@event);

                return;
            }

            if (!NieuweWaardenIndienWerdOvergenomen(adresWerdOvergenomen, locatie))
                return;

            var stateLocatie = State.Locaties.SingleOrDefault(
                sod =>
                    sod.AdresId is not null &&
                    sod.AdresId == adresWerdOvergenomen.OvergenomenAdresUitAdressenregister.AdresId &&
                    sod.Naam == locatie.Naam);

            if (stateLocatie is not null)
            {
                var verwijderdeLocatieId = !stateLocatie.IsPrimair && locatie.IsPrimair ? stateLocatie.LocatieId : locatieId;
                var behoudenLocatieId = verwijderdeLocatieId == locatieId ? stateLocatie.LocatieId : locatieId;

                AddEvent(new AdresWerdOvergenomenUitAdressenregister(VCode, locatieId,
                                                                     adresWerdOvergenomen.OvergenomenAdresUitAdressenregister));

                AddEvent(new LocatieDuplicaatWerdVerwijderdNaAdresMatch(VCode, verwijderdeLocatieId,
                                                                        behoudenLocatieId,
                                                                        locatie.Naam,
                                                                        adresWerdOvergenomen.OvergenomenAdresUitAdressenregister.AdresId));

                return;
            }

            AddEvent(adresWerdOvergenomen);
        }
        catch (AdressenregisterReturnedNonSuccessStatusCode ex)
        {
            var @event = GetAdresMatchExceptionEvent(locatieId, ex, locatie);

            AddEvent(@event);
        }
    }

    private async Task<IEvent> GetAdresMatchEvent(
        int locatieId,
        Locatie locatie,
        IGrarClient grarClient,
        CancellationToken cancellationToken)
    {
        if (locatie is null)
        {
            return new AdresKonNietOvergenomenWordenUitAdressenregister(VCode,
                                                                        locatieId,
                                                                        string.Empty,
                                                                        "Locatie kon niet gevonden worden. Mogelijks is deze verwijderd.");
        }

        var adresMatch = await grarClient.GetAddressMatches(
            locatie.Adres.Straatnaam,
            locatie.Adres.Huisnummer,
            locatie.Adres.Busnummer,
            locatie.Adres.Postcode,
            locatie.Adres.Gemeente,
            cancellationToken);

        var postalInformation = await grarClient.GetPostalInformation(locatie.Adres.Postcode);

        return adresMatch?.Count switch
        {
            0 => new AdresWerdNietGevondenInAdressenregister(VCode, locatieId,
                                                             locatie.Adres.Straatnaam,
                                                             locatie.Adres.Huisnummer,
                                                             locatie.Adres.Busnummer,
                                                             locatie.Adres.Postcode,
                                                             locatie.Adres.Gemeente),
            1 => new AdresWerdOvergenomenUitAdressenregister(VCode, locatieId,
                                                             AdresMatchUitAdressenregister
                                                                .FromResponse(adresMatch.Single())
                                                                .DecorateWithPostalInformation(
                                                                     locatie.Adres.Gemeente, postalInformation)),
            _ => adresMatch.Count(c => c.Score == 100).Equals(1)
                ? new AdresWerdOvergenomenUitAdressenregister(VCode, locatieId,
                                                              AdresMatchUitAdressenregister
                                                                 .FromResponse(adresMatch.Single(s => s.Score == 100))
                                                                 .DecorateWithPostalInformation(
                                                                      locatie.Adres.Gemeente, postalInformation))
                : new AdresNietUniekInAdressenregister(VCode, locatieId,
                                                       adresMatch.Select(
                                                                      match => NietUniekeAdresMatchUitAdressenregister.FromResponse(
                                                                          match))
                                                                 .ToArray()),
        };
    }

    private IEvent GetAdresMatchExceptionEvent(
        int locatieId,
        AdressenregisterReturnedNonSuccessStatusCode ex,
        Locatie locatieVoorTeMatchenAdres)
    {
        IEvent @event = ex.StatusCode switch
        {
            HttpStatusCode.NotFound => new AdresWerdNietGevondenInAdressenregister(VCode, locatieId,
                                                                                   locatieVoorTeMatchenAdres.Adres.Straatnaam,
                                                                                   locatieVoorTeMatchenAdres.Adres.Huisnummer,
                                                                                   locatieVoorTeMatchenAdres.Adres.Busnummer,
                                                                                   locatieVoorTeMatchenAdres.Adres.Postcode,
                                                                                   locatieVoorTeMatchenAdres.Adres.Gemeente),
            _ => new AdresKonNietOvergenomenWordenUitAdressenregister(VCode, locatieId, locatieVoorTeMatchenAdres.Adres.ToAdresString(),
                                                                      ex.Message),
        };

        return @event;
    }

    [Pure]
    private static bool NieuweWaardenIndienWerdOvergenomen(AdresWerdOvergenomenUitAdressenregister @event, Locatie locatie)
    {
        return HeeftVerschillenBinnenAdres(locatie, @event.OvergenomenAdresUitAdressenregister.Adres) ||
               HeeftVerschillenBinnenAdresId(locatie, @event.OvergenomenAdresUitAdressenregister.AdresId);
    }

    [Pure]
    private static bool HeeftVerschillenBinnenAdres(Locatie locatie, Registratiedata.Adres adresUitAdressenregister)
        => HeeftVerschillenBinnenAdres(locatie.Adres,
                                       adresUitAdressenregister.Straatnaam,
                                       adresUitAdressenregister.Huisnummer,
                                       adresUitAdressenregister.Busnummer,
                                       adresUitAdressenregister.Postcode,
                                       adresUitAdressenregister.Gemeente,
                                       adresUitAdressenregister.Land);

    [Pure]
    private static bool HeeftVerschillenBinnenAdres(
        Adres adres,
        string straatnaam,
        string huisnummer,
        string busnummer,
        string postcode,
        string gemeente,
        string land)
        => (
            straatnaam != adres.Straatnaam ||
            huisnummer != adres.Huisnummer ||
            busnummer != adres.Busnummer ||
            postcode != adres.Postcode ||
            gemeente != adres.Gemeente ||
            land != adres.Land
            );

    [Pure]
    private static bool HeeftVerschillenBinnenAdresId(Locatie locatie, Registratiedata.AdresId? adresIdUitAdressenregister)
        => (
            adresIdUitAdressenregister?.Broncode != locatie.AdresId?.Adresbron.Code ||
            adresIdUitAdressenregister?.Bronwaarde != locatie.AdresId?.Bronwaarde
            );

    public void Hydrate(VerenigingState obj)
    {
        State = obj;
    }

    public long Version => State.Version;
}
