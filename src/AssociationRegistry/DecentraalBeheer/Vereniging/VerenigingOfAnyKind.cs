namespace AssociationRegistry.DecentraalBeheer.Vereniging;

using Adressen;
using Events;
using Framework;
using GemeentenaamVerrijking;
using AssociationRegistry.Grar.AdresMatch;
using AssociationRegistry.Grar.Exceptions;
using Grar.Models;
using Emails;
using Events.Factories;
using Exceptions;
using Geotags;
using Grar;
using Persoonsgegevens;
using SocialMedias;
using System.Diagnostics.Contracts;
using TelefoonNummers;
using VertegenwoordigerPersoonsgegevens = Persoonsgegevens.VertegenwoordigerPersoonsgegevens;

public class VerenigingOfAnyKind : VerenigingsBase, IHydrate<VerenigingState>
{
    public Contactgegeven VoegContactgegevenToe(Contactgegeven contactgegeven)
    {
        var toegevoegdContactgegeven = State.Contactgegevens.VoegToe(contactgegeven);

        AddEvent(EventFactory.ContactgegevenWerdToegevoegd(toegevoegdContactgegeven));

        return toegevoegdContactgegeven;
    }

    public void WijzigContactgegeven(int contactgegevenId, string? waarde, string? beschrijving, bool? isPrimair)
    {
        var gewijzigdContactgegeven = State.Contactgegevens.Wijzig(contactgegevenId, waarde, beschrijving, isPrimair);

        if (gewijzigdContactgegeven is null)
            return;

        AddEvent(EventFactory.ContactgegevenWerdGewijzigd(gewijzigdContactgegeven));
    }

    public void VerwijderContactgegeven(int contactgegevenId)
    {
        var verwijderdContactgegeven = State.Contactgegevens.Verwijder(contactgegevenId);
        AddEvent(EventFactory.ContactgegevenWerdVerwijderd(verwijderdContactgegeven));
    }

    public async Task WijzigVertegenwoordiger(
        int vertegenwoordigerId,
        string? rol,
        string? roepnaam,
        Email? email,
        TelefoonNummer? telefoonNummer,
        TelefoonNummer? mobiel,
        SocialMedia? socialMedia,
        bool? isPrimair,
        IVertegenwoordigerPersoonsgegevensRepository vertegenwoordigerPersoonsgegevensRepository)
    {
        var refId = Guid.NewGuid();
        var gewijzigdeVertegenwoordiger =
            State.Vertegenwoordigers.Wijzig(vertegenwoordigerId, rol, roepnaam, email, telefoonNummer, mobiel, socialMedia, isPrimair);

        if (gewijzigdeVertegenwoordiger is null)
            return;

        var vertegenwoordigerPersoonsgegevens = VertegenwoordigerPersoonsgegevens.ToVertegenwoordiger(refId, VCode, gewijzigdeVertegenwoordiger);
        await vertegenwoordigerPersoonsgegevensRepository.Save(vertegenwoordigerPersoonsgegevens);

        AddEvent(EventFactory.VertegenwoordigerWerdGewijzigd(gewijzigdeVertegenwoordiger, refId));
    }

    public Locatie VoegLocatieToe(Locatie toeTeVoegenLocatie)
    {
        Throw<MaatschappelijkeZetelIsNietToegestaan>.If(toeTeVoegenLocatie.Locatietype == Locatietype.MaatschappelijkeZetelVolgensKbo);

        var toegevoegdeLocatie = State.Locaties.VoegToe(toeTeVoegenLocatie);

        AddEvent(EventFactory.LocatieWerdToegevoegd(toegevoegdeLocatie));

        return toegevoegdeLocatie;
    }

    public void WijzigLocatie(int locatieId, string? naam, Locatietype? locatietype, bool? isPrimair, AdresId? adresId, Adres? adres)
    {
        Throw<MaatschappelijkeZetelIsNietToegestaan>.If(locatietype is not null &&
                                                        locatietype == Locatietype.MaatschappelijkeZetelVolgensKbo);

        var gewijzigdeLocatie = State.Locaties.Wijzig(locatieId, naam, locatietype, isPrimair, adresId, adres);

        if (gewijzigdeLocatie is null)
            return;

        AddEvent(EventFactory.LocatieWerdGewijzigd(gewijzigdeLocatie));
    }

    public void VerwijderLocatie(int locatieId)
    {
        var locatie = State.Locaties.Verwijder(locatieId);
        AddEvent(EventFactory.LocatieWerdVerwijderd(State.VCode, locatie));
    }

    public Lidmaatschap VoegLidmaatschapToe(ToeTeVoegenLidmaatschap lidmaatschap)
    {
        Throw<LidmaatschapMagNietVerwijzenNaarEigenVereniging>.If(VCode == lidmaatschap.AndereVereniging);
        Throw<VerenigingKanGeenLidWordenWaarvanZijAlReedsSubverenigingIs>.If(State.Verenigingssubtype.IsSubverenigingVan(lidmaatschap.AndereVereniging));

        var toegevoegdLidmaatschap = State.Lidmaatschappen.VoegToe(lidmaatschap);

        AddEvent(EventFactory.LidmaatschapWerdToegevoegd(VCode, toegevoegdLidmaatschap));

        return toegevoegdLidmaatschap;
    }

    public void VerwijderLidmaatschap(LidmaatschapId lidmaatschapId)
    {
        var locatie = State.Lidmaatschappen.Verwijder(lidmaatschapId);
        AddEvent(EventFactory.LidmaatschapWerdVerwijderd(State.VCode, locatie));
    }

    public void WijzigLidmaatschap(TeWijzigenLidmaatschap lidmaatschap)
    {
        var toegevoegdLidmaatschap = State.Lidmaatschappen.Wijzig(lidmaatschap);

        if (toegevoegdLidmaatschap is null)
            return;

        AddEvent(EventFactory.LidmaatschapWerdGewijzigd(VCode, toegevoegdLidmaatschap));
    }

    public async Task HeradresseerLocaties(List<LocatieWithAdres> locatiesMetAdressen, string idempotenceKey, IAddressVerrijkingsService addressVerrijkingsService)
    {
        if (State.HandledIdempotenceKeys.Contains(idempotenceKey))
            return;

        foreach (var (locatieId, adresDetailResponse) in locatiesMetAdressen)
        {
            if (!State.Locaties.HasKey(locatieId))
                continue;

            var locatie = State.Locaties[locatieId];

            var verrijktAdres = await addressVerrijkingsService.FromAdresAndGrarResponse(adresDetailResponse, locatie.Adres, CancellationToken.None);

            var adres = Adres.Hydrate(
                straatnaam: verrijktAdres.AddressResponse.Straatnaam,
                huisnummer: verrijktAdres.AddressResponse.Huisnummer,
                busnummer: verrijktAdres.AddressResponse.Busnummer,
                postcode: verrijktAdres.AddressResponse.Postcode,
                gemeente: verrijktAdres.Gemeente.Naam,
                land: Adres.België);

            var verrijkteLocatie = locatie.MetAdresUitGrar(adres);

            State.Locaties.ThrowIfCannotAppendOrUpdate(verrijkteLocatie);

            var verrijktAdresUitAdressenRegister =
                EventFactory.VerrijktAdresUitAdressenregister(verrijktAdres);

            AddEvent(new AdresWerdGewijzigdInAdressenregister(VCode,
                                                              locatieId,
                                                              adresDetailResponse.AdresId,
                                                              verrijktAdresUitAdressenRegister.Adres,
                                                              idempotenceKey));
        }
    }

    public async Task SyncAdresLocaties(List<LocatieWithAdres> locatiesMetAdressen, string idempotenceKey, IAddressVerrijkingsService verrijkingService)
    {
        if (State.HandledIdempotenceKeys.Contains(idempotenceKey))
            return;

        foreach (var (locatieId, adresDetailResponse) in locatiesMetAdressen)
        {
            if (!State.Locaties.HasKey(locatieId))
                continue;

            var locatie = State.Locaties[locatieId];

            if (locatie.AdresBestaatNietOfIsNietActief(adresDetailResponse))
            {
                if (locatie.HeeftAdresId && locatie.AdresIdKomtOvereenMetGrarIndienBestaand(adresDetailResponse))
                {
                    AddEvent(new AdresWerdOntkoppeldVanAdressenregister(
                                 VCode,
                                 locatieId,
                                 EventFactory.AdresId(locatie.AdresId),
                                 EventFactory.Adres(locatie.Adres)));
                }

                continue;
            }

            var verrijktAdres = await verrijkingService.FromAdresAndGrarResponse(adresDetailResponse, locatie.Adres, CancellationToken.None);

            var adres = Adres.Hydrate(
                straatnaam: verrijktAdres.AddressResponse.Straatnaam,
                huisnummer: verrijktAdres.AddressResponse.Huisnummer,
                busnummer: verrijktAdres.AddressResponse.Busnummer,
                postcode: verrijktAdres.AddressResponse.Postcode,
                gemeente: verrijktAdres.Gemeente.Naam,
                land: Adres.België);

            var verrijkteLocatie = locatie.VerrijkMet(adres);
            State.Locaties.ThrowIfCannotAppendOrUpdate(verrijkteLocatie);

            if (locatie.Adres != verrijkteLocatie.Adres)
            {
                AddEvent(new AdresWerdGewijzigdInAdressenregister(VCode,
                                                                  locatieId,
                                                                  adresDetailResponse.AdresId,
                                                                  EventFactory
                                                                     .VerrijktAdresUitAdressenregister(verrijktAdres)!.Adres,
                                                                  idempotenceKey));
            }
        }
    }

    public AdresMatchRequest CreateAdresMatchRequest(int locatieId)
    {
        var locatie = State.Locaties.SingleOrDefault(s => s.LocatieId == locatieId);
        return new AdresMatchRequest(locatie);
    }

    public void ProcessAdresMatchResult(AdresMatchResult result, int locatieId)
    {
        var locatie = State.Locaties.SingleOrDefault(s => s.LocatieId == locatieId);
        var @event = result.ToEvent(VCode, locatieId);

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

    public async Task NeemAdresDetailOver(
        int locatieId,
        IAddressVerrijkingsService verrijkingService,
        CancellationToken cancellationToken)
    {
        var locatie = State.Locaties[locatieId];

        var verrijktAdres = await verrijkingService.FromActiefAdresId(locatie.AdresId!, cancellationToken);

        var adres = Adres.Hydrate(
            straatnaam: verrijktAdres.AddressResponse.Straatnaam,
            huisnummer: verrijktAdres.AddressResponse.Huisnummer,
            busnummer: verrijktAdres.AddressResponse.Busnummer,
            postcode: verrijktAdres.AddressResponse.Postcode,
            gemeente: verrijktAdres.Gemeente.Naam,
            land: Adres.België);

        var decoratedLocatie = locatie.MetAdresUitGrar(adres);
        State.Locaties.ThrowIfCannotAppendOrUpdate(decoratedLocatie);

        var registratieData =
            EventFactory.VerrijktAdresUitAdressenregister(verrijktAdres);

        AddEvent(new AdresWerdOvergenomenUitAdressenregister(VCode, locatie.LocatieId,
                                                             verrijktAdres.AddressResponse.AdresId,
                                                             registratieData.Adres));
    }

    public void OntkoppelLocatie(int locatieId)
    {
        if (!State.Locaties.HasKey(locatieId))
            return;

        var locatie = State.Locaties[locatieId];

        if (locatie.AdresId is null)
            return;

        AddEvent(new AdresWerdOntkoppeldVanAdressenregister(
                     VCode,
                     locatieId,
                     EventFactory.AdresId(locatie.AdresId),
                     EventFactory.Adres(locatie.Adres)));
    }


    public void AanvaardDubbeleVereniging(VCode dubbeleVereniging)
    {
        Throw<InvalidOperationVerenigingKanGeenDubbelWordenVanZichzelf>.If(dubbeleVereniging.Equals(VCode));
        AddEvent(EventFactory.VerenigingAanvaarddeDubbeleVereniging(VCode, dubbeleVereniging));
    }

    public void AanvaardCorrectieDubbeleVereniging(VCode dubbeleVereniging)
    {
        if (!State.CorresponderendeVCodes.Contains(dubbeleVereniging))
            throw new ApplicationException($"Vereniging kon correctie dubbele vereniging ({dubbeleVereniging}) niet aanvaarden omdat dubbele vereniging " +
                                           $"niet voorkomt in de corresponderende VCodes: {string.Join(',', State.CorresponderendeVCodes)}.");

        AddEvent(EventFactory.VerenigingAanvaarddeCorrectieDubbeleVereniging(VCode, dubbeleVereniging));
    }

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

    public async Task InitialiseerGeotags(IGeotagsService service)
    {
        if (State.GeotagsGeinitialiseerd)
            return;

        await HerberekenGeotags(service);
    }

    public async Task HerberekenGeotags(IGeotagsService geotagsService)
    {
        var geotags = await geotagsService.CalculateGeotags(State.Locaties, State.Werkingsgebieden);

        if (geotags.Equals(State.Geotags))
            return;

        AddEvent(EventFactory.GeotagsWerdenBepaald(VCode, geotags));
    }
}
