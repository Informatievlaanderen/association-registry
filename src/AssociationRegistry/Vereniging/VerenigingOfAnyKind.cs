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
    public Contactgegeven VoegContactgegevenToe(Contactgegeven contactgegeven)
    {
        var toegevoegdContactgegeven = State.Contactgegevens.VoegToe(contactgegeven);

        AddEvent(ContactgegevenWerdToegevoegd.With(toegevoegdContactgegeven));

        return toegevoegdContactgegeven;
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

    public Locatie VoegLocatieToe(Locatie toeTeVoegenLocatie)
    {
        Throw<MaatschappelijkeZetelIsNietToegestaan>.If(toeTeVoegenLocatie.Locatietype == Locatietype.MaatschappelijkeZetelVolgensKbo);

        var toegevoegdeLocatie = State.Locaties.VoegToe(toeTeVoegenLocatie);

        AddEvent(LocatieWerdToegevoegd.With(toegevoegdeLocatie));

        return toegevoegdeLocatie;
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

    public Lidmaatschap VoegLidmaatschapToe(Lidmaatschap toeTeVoegenLidmaatschap)
    {
        Throw<LidmaatschapMagNietVerwijzenNaarEigenVereniging>.If(VCode == toeTeVoegenLidmaatschap.AndereVereniging);

        var toegevoegdLidmaatschap = State.Lidmaatschappen.VoegToe(toeTeVoegenLidmaatschap);

        AddEvent(LidmaatschapWerdToegevoegd.With(toegevoegdLidmaatschap));

        return toegevoegdLidmaatschap;
    }

    public async Task HeradresseerLocaties(List<LocatieWithAdres> locatiesMetAdressen, string idempotenceKey, IGrarClient grarClient)
    {
        if (State.HandledIdempotenceKeys.Contains(idempotenceKey))
            return;

        foreach (var (locatieId, adresDetailResponse) in locatiesMetAdressen)
        {
            if (!State.Locaties.HasKey(locatieId))
                continue;

            var locatie = State.Locaties[locatieId];

            var postalInformation = await grarClient.GetPostalInformation(adresDetailResponse.Postcode);

            var verrijkteGemeentenaam = GemeentenaamDecorator.VerrijkGemeentenaam(
                locatie.Adres.Gemeente,
                postalInformation,
                adresDetailResponse.Gemeente);

            var verrijkteLocatie = locatie.VerrijkMet(verrijkteGemeentenaam);
            State.Locaties.ThrowIfCannotAppendOrUpdate(verrijkteLocatie);

            var registratieData =
                Registratiedata.AdresUitAdressenregister.FromVerrijktAddressDetailResponse(adresDetailResponse, verrijkteGemeentenaam);

            AddEvent(new AdresWerdGewijzigdInAdressenregister(VCode,
                                                              locatieId,
                                                              adresDetailResponse.AdresId,
                                                              registratieData,
                                                              idempotenceKey));
        }
    }

    public async Task SyncAdresLocaties(List<LocatieWithAdres> locatiesMetAdressen, string idempotenceKey, IGrarClient grarClient)
    {
        if (State.HandledIdempotenceKeys.Contains(idempotenceKey))
            return;

        foreach (var (locatieId, adresDetailResponse) in locatiesMetAdressen)
        {
            if (!State.Locaties.HasKey(locatieId))
                continue;

            var locatie = State.Locaties[locatieId];

            if (adresDetailResponse is null || !adresDetailResponse.IsActief)
            {
                AddEvent(new AdresWerdOntkoppeldVanAdressenregister(
                             VCode,
                             locatieId,
                             Registratiedata.AdresId.With(locatie.AdresId),
                             Registratiedata.Adres.With(locatie.Adres)));

                continue;
            }

            var postalInformation = await grarClient.GetPostalInformation(adresDetailResponse.Postcode);

            var verrijkteGemeentenaam = GemeentenaamDecorator.VerrijkGemeentenaam(
                locatie.Adres.Gemeente,
                postalInformation,
                adresDetailResponse.Gemeente);

            var verrijkteLocatie = locatie.VerrijkMet(verrijkteGemeentenaam);
            State.Locaties.ThrowIfCannotAppendOrUpdate(verrijkteLocatie);

            if (locatie.Adres != verrijkteLocatie.Adres)
            {
                AddEvent(new AdresWerdGewijzigdInAdressenregister(VCode,
                                                                  locatieId,
                                                                  adresDetailResponse.AdresId,
                                                                  Registratiedata
                                                                     .AdresUitAdressenregister
                                                                     .FromVerrijktAddressDetailResponse(
                                                                          adresDetailResponse, verrijkteGemeentenaam)!,
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
            {
                AddEvent(new AdresHeeftGeenVerschillenMetAdressenregister(VCode,
                                                                          locatieId,
                                                                          adresWerdOvergenomen.AdresId,
                                                                          adresWerdOvergenomen.Adres));

                return;
            }

            var stateLocatie = State.Locaties.SingleOrDefault(
                sod =>
                    sod.LocatieId != locatieId &&
                    sod.AdresId is not null &&
                    sod.AdresId == adresWerdOvergenomen.AdresId &&
                    sod.Naam == locatie.Naam &&
                    sod.Locatietype == locatie.Locatietype);

            if (stateLocatie is not null)
            {
                var verwijderdeLocatieId = !stateLocatie.IsPrimair && locatie.IsPrimair ? stateLocatie.LocatieId : locatieId;
                var behoudenLocatieId = verwijderdeLocatieId == locatieId ? stateLocatie.LocatieId : locatieId;

                AddEvent(adresWerdOvergenomen with
                {
                    VCode = VCode,
                    LocatieId = locatieId
                });

                AddEvent(new LocatieDuplicaatWerdVerwijderdNaAdresMatch(VCode, verwijderdeLocatieId,
                                                                        behoudenLocatieId,
                                                                        locatie.Naam,
                                                                        adresWerdOvergenomen.AdresId));

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

    public async Task NeemAdresDetailOver(
        int locatieId,
        IGrarClient grarClient,
        CancellationToken cancellationToken)
    {
        var locatie = State.Locaties[locatieId];

        var adresDetailResponse = await grarClient.GetAddressById(locatie.AdresId.ToString(), cancellationToken);

        if (!adresDetailResponse.IsActief)
            throw new AdressenregisterReturnedInactiefAdres();

        var postalInformation = await grarClient.GetPostalInformation(adresDetailResponse.Postcode);

        var verrijkteGemeentenaam = GemeentenaamDecorator.VerrijkGemeentenaam(
            postalInformation,
            adresDetailResponse.Gemeente);

        var decoratedLocatie = locatie.MetAdresUitGrar(adresDetailResponse, verrijkteGemeentenaam);
        State.Locaties.ThrowIfCannotAppendOrUpdate(decoratedLocatie);

        var registratieData =
            Registratiedata.AdresUitAdressenregister.FromVerrijktAddressDetailResponse(adresDetailResponse, verrijkteGemeentenaam);

        AddEvent(new AdresWerdOvergenomenUitAdressenregister(VCode, locatie.LocatieId,
                                                             adresDetailResponse.AdresId,
                                                             registratieData));
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
                                                                        AdresKonNietOvergenomenWordenUitAdressenregister
                                                                           .RedenLocatieWerdVerwijderd);
        }

        var adresMatches = await grarClient.GetAddressMatches(
            locatie.Adres.Straatnaam,
            locatie.Adres.Huisnummer,
            locatie.Adres.Busnummer,
            locatie.Adres.Postcode,
            locatie.Adres.Gemeente.Naam,
            cancellationToken);

        var postalInformation = await grarClient.GetPostalInformation(locatie.Adres.Postcode);

        if (adresMatches.HasNoResponse)
            return AdresWerdNietGevondenInAdressenregister.From(VCode, locatie);

        if (!adresMatches.HasSingularResponse)
            return new AdresNietUniekInAdressenregister(VCode, locatieId,
                                                        adresMatches.Select(NietUniekeAdresMatchUitAdressenregister.FromResponse)
                                                                    .ToArray());

        var verrijkteGemeentenaam = GemeentenaamDecorator.VerrijkGemeentenaam(
            locatie.Adres.Gemeente,
            postalInformation,
            adresMatches.SingularResponse.Gemeente);

        var registratieData =
            Registratiedata.AdresUitAdressenregister.FromVerrijktAddressDetailResponse(
                adresMatches.SingularResponse, verrijkteGemeentenaam);

        return new AdresWerdOvergenomenUitAdressenregister(VCode, locatieId,
                                                           adresMatches.SingularResponse.AdresId!,
                                                           registratieData);
    }

    private IEvent GetAdresMatchExceptionEvent(
        int locatieId,
        AdressenregisterReturnedNonSuccessStatusCode ex,
        Locatie locatieVoorTeMatchenAdres)
    {
        IEvent @event = ex.StatusCode switch
        {
            //TODO: is this correct?
            HttpStatusCode.NotFound => AdresWerdNietGevondenInAdressenregister.From(VCode, locatieVoorTeMatchenAdres),
            _ => new AdresKonNietOvergenomenWordenUitAdressenregister(VCode, locatieId, locatieVoorTeMatchenAdres.Adres.ToAdresString(),
                                                                      GetExceptionMessage(ex.StatusCode)),
        };

        return @event;
    }

    private string GetExceptionMessage(HttpStatusCode statusCode)
        => statusCode == HttpStatusCode.BadRequest
            ? GrarClient.BadRequestSuccessStatusCodeMessage
            : GrarClient.OtherNonSuccessStatusCodeMessage;

    [Pure]
    private static bool NieuweWaardenIndienWerdOvergenomen(AdresWerdOvergenomenUitAdressenregister @event, Locatie locatie)
    {
        var adres = Adres.Hydrate(@event.Adres);
        var adresId = AdresId.Hydrate(@event);

        return locatie.Adres != adres ||
               locatie.AdresId != adresId;
    }

    public void Hydrate(VerenigingState obj)
    {
        State = obj;
    }

    public long Version => State.Version;
}
