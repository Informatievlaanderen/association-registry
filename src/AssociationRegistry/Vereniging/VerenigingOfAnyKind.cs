namespace AssociationRegistry.Vereniging;

using DecentraalBeheer.Lidmaatschappen.VoegLidmaatschapToe;
using DecentraalBeheer.Lidmaatschappen.WijzigLidmaatschap;
using Emails;
using EventFactories;
using Events;
using Exceptions;
using Framework;
using GemeentenaamVerrijking;
using Geotags;
using Grar.AdresMatch;
using Grar.Clients;
using Grar.Exceptions;
using Grar.Models;
using SocialMedias;
using System.Diagnostics.Contracts;
using System.Net;
using TelefoonNummers;

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

    public void VoegVertegenwoordigerToe(Vertegenwoordiger vertegenwoordiger)
    {
        var toegevoegdeVertegenwoordiger = State.Vertegenwoordigers.VoegToe(vertegenwoordiger);

        AddEvent(EventFactory.VertegenwoordigerWerdToegevoegd(toegevoegdeVertegenwoordiger));
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

        AddEvent(EventFactory.VertegenwoordigerWerdGewijzigd(gewijzigdeVertegenwoordiger));
    }

    public void VerwijderVertegenwoordiger(int vertegenwoordigerId)
    {
        var vertegenwoordiger = State.Vertegenwoordigers.Verwijder(vertegenwoordigerId);
        AddEvent(EventFactory.VertegenwoordigerWerdVerwijderd(vertegenwoordiger));
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

    public Lidmaatschap VoegLidmaatschapToe(VoegLidmaatschapToeCommand.ToeTeVoegenLidmaatschap lidmaatschap)
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

    public void WijzigLidmaatschap(WijzigLidmaatschapCommand.TeWijzigenLidmaatschap lidmaatschap)
    {
        var toegevoegdLidmaatschap = State.Lidmaatschappen.Wijzig(lidmaatschap);

        if (toegevoegdLidmaatschap is null)
            return;

        AddEvent(EventFactory.LidmaatschapWerdGewijzigd(VCode, toegevoegdLidmaatschap));
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

            var postalInformation = await grarClient.GetPostalInformationDetail(adresDetailResponse.Postcode);

            var verrijkteGemeentenaam = GemeentenaamDecorator.VerrijkGemeentenaam(
                locatie.Adres.Gemeente,
                postalInformation,
                adresDetailResponse.Gemeente);

            var verrijkteLocatie = locatie.VerrijkMet(verrijkteGemeentenaam);
            State.Locaties.ThrowIfCannotAppendOrUpdate(verrijkteLocatie);

            var registratieData =
                EventFactory.AdresUitAdressenregister(adresDetailResponse, verrijkteGemeentenaam);

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
                             EventFactory.AdresId(locatie.AdresId),
                             EventFactory.Adres(locatie.Adres)));

                continue;
            }

            var postalInformation = await grarClient.GetPostalInformationDetail(adresDetailResponse.Postcode);

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
                                                                  EventFactory
                                                                     .AdresUitAdressenregister(
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
            var @event = await AdresMatchService.GetAdresMatchEvent(locatieId, locatie, grarClient, cancellationToken, VCode);

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
            AddEvent(new AdresKonNietOvergenomenWordenUitAdressenregister(
                         VCode,
                         locatieId,
                         locatie.Adres.ToAdresString(),
                         ex.Message
                     ));
        }
        catch(AdressenregisterReturnedNotFoundStatusCode ex)
        {
            AddEvent(EventFactory.AdresWerdNietGevondenInAdressenregister(VCode, locatie));
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

        var postalInformation = await grarClient.GetPostalInformationDetail(adresDetailResponse.Postcode);

        var verrijkteGemeentenaam = GemeentenaamDecorator.VerrijkGemeentenaam(
            postalInformation,
            adresDetailResponse.Gemeente);

        var decoratedLocatie = locatie.MetAdresUitGrar(adresDetailResponse, verrijkteGemeentenaam);
        State.Locaties.ThrowIfCannotAppendOrUpdate(decoratedLocatie);

        var registratieData =
            EventFactory.AdresUitAdressenregister(adresDetailResponse, verrijkteGemeentenaam);

        AddEvent(new AdresWerdOvergenomenUitAdressenregister(VCode, locatie.LocatieId,
                                                             adresDetailResponse.AdresId,
                                                             registratieData));
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
