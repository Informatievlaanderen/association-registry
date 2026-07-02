namespace AssociationRegistry.DecentraalBeheer.Vereniging;

using System.Diagnostics.Contracts;
using Adressen;
using AssociationRegistry.Vereniging.Bronnen;
using Bankrekeningen;
using Bankrekeningen.Exceptions;
using Emails;
using Erkenningen;
using Erkenningen.Exceptions;
using Events;
using Events.Factories;
using Exceptions;
using Framework;
using Geotags;
using Grar.AdresMatch;
using Grar.Models;
using SocialMedias;
using TelefoonNummers;
using Wegwijs;

public class VerenigingOfAnyKind : VerenigingsBase, IHydrate<VerenigingState>
{
    public void Hydrate(VerenigingState obj)
    {
        State = obj;
    }

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

    public void WijzigVertegenwoordiger(
        int vertegenwoordigerId,
        string? rol,
        string? roepnaam,
        Email? email,
        TelefoonNummer? telefoonNummer,
        TelefoonNummer? mobiel,
        SocialMedia? socialMedia,
        bool? isPrimair
    )
    {
        var gewijzigdeVertegenwoordiger = State.Vertegenwoordigers.Wijzig(
            vertegenwoordigerId,
            rol,
            roepnaam,
            email,
            telefoonNummer,
            mobiel,
            socialMedia,
            isPrimair
        );

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
        Throw<MaatschappelijkeZetelIsNietToegestaan>.If(
            toeTeVoegenLocatie.Locatietype == Locatietype.MaatschappelijkeZetelVolgensKbo
        );

        var toegevoegdeLocatie = State.Locaties.VoegToe(toeTeVoegenLocatie);

        AddEvent(EventFactory.LocatieWerdToegevoegd(toegevoegdeLocatie));

        return toegevoegdeLocatie;
    }

    public void WijzigLocatie(
        int locatieId,
        string? naam,
        Locatietype? locatietype,
        bool? isPrimair,
        AdresId? adresId,
        Adres? adres
    )
    {
        Throw<MaatschappelijkeZetelIsNietToegestaan>.If(
            locatietype is not null && locatietype == Locatietype.MaatschappelijkeZetelVolgensKbo
        );

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

        Throw<VerenigingKanGeenLidWordenWaarvanZijAlReedsSubverenigingIs>.If(
            State.Verenigingssubtype.IsSubverenigingVan(lidmaatschap.AndereVereniging)
        );

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

    public async Task HeradresseerLocaties(
        List<LocatieWithAdres> locatiesMetAdressen,
        string idempotenceKey,
        IAddressVerrijkingsService addressVerrijkingsService
    )
    {
        if (State.HandledIdempotenceKeys.Contains(idempotenceKey))
            return;

        foreach (var (locatieId, adresDetailResponse) in locatiesMetAdressen)
        {
            if (!State.Locaties.HasKey(locatieId))
                continue;

            var locatie = State.Locaties[locatieId];

            var verrijktAdres = await addressVerrijkingsService.FromAdresAndGrarResponse(
                adresDetailResponse,
                locatie.Adres,
                CancellationToken.None
            );

            var adres = Adres.Hydrate(
                verrijktAdres.AddressResponse.Straatnaam,
                verrijktAdres.AddressResponse.Huisnummer,
                verrijktAdres.AddressResponse.Busnummer,
                verrijktAdres.AddressResponse.Postcode,
                verrijktAdres.Gemeente.Naam,
                Adres.België
            );

            var verrijkteLocatie = locatie.MetAdresUitGrar(adres);

            State.Locaties.ThrowIfCannotAppendOrUpdate(verrijkteLocatie);

            var verrijktAdresUitAdressenRegister = EventFactory.VerrijktAdresUitAdressenregister(verrijktAdres);

            AddEvent(
                new AdresWerdGewijzigdInAdressenregister(
                    VCode,
                    locatieId,
                    adresDetailResponse.AdresId,
                    verrijktAdresUitAdressenRegister.Adres,
                    idempotenceKey
                )
            );
        }
    }

    public async Task SyncAdresLocaties(
        List<LocatieWithAdres> locatiesMetAdressen,
        string idempotenceKey,
        IAddressVerrijkingsService verrijkingService
    )
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
                    AddEvent(
                        new AdresWerdOntkoppeldVanAdressenregister(
                            VCode,
                            locatieId,
                            EventFactory.AdresId(locatie.AdresId),
                            EventFactory.Adres(locatie.Adres)
                        )
                    );

                continue;
            }

            var verrijktAdres = await verrijkingService.FromAdresAndGrarResponse(
                adresDetailResponse,
                locatie.Adres,
                CancellationToken.None
            );

            var adres = Adres.Hydrate(
                verrijktAdres.AddressResponse.Straatnaam,
                verrijktAdres.AddressResponse.Huisnummer,
                verrijktAdres.AddressResponse.Busnummer,
                verrijktAdres.AddressResponse.Postcode,
                verrijktAdres.Gemeente.Naam,
                Adres.België
            );

            var verrijkteLocatie = locatie.VerrijkMet(adres);
            // TODO: if locatie is duplicatie verwijder duplicate locatie event?
            State.Locaties.ThrowIfCannotAppendOrUpdate(verrijkteLocatie);

            if (locatie.Adres != verrijkteLocatie.Adres)
                AddEvent(
                    new AdresWerdGewijzigdInAdressenregister(
                        VCode,
                        locatieId,
                        adresDetailResponse.AdresId,
                        EventFactory.VerrijktAdresUitAdressenregister(verrijktAdres)!.Adres,
                        idempotenceKey
                    )
                );
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
            AddEvent(
                new AdresHeeftGeenVerschillenMetAdressenregister(
                    VCode,
                    locatieId,
                    adresWerdOvergenomen.AdresId,
                    adresWerdOvergenomen.Adres
                )
            );

            return;
        }

        var stateLocatie = State.Locaties.SingleOrDefault(sod =>
            sod.LocatieId != locatieId
            && sod.AdresId is not null
            && sod.AdresId == adresWerdOvergenomen.AdresId
            && sod.Naam == locatie.Naam
            && sod.Locatietype == locatie.Locatietype
        );

        if (stateLocatie is not null)
        {
            var verwijderdeLocatieId =
                !stateLocatie.IsPrimair && locatie.IsPrimair ? stateLocatie.LocatieId : locatieId;

            var behoudenLocatieId = verwijderdeLocatieId == locatieId ? stateLocatie.LocatieId : locatieId;

            AddEvent(adresWerdOvergenomen with { VCode = VCode, LocatieId = locatieId });

            AddEvent(
                new LocatieDuplicaatWerdVerwijderdNaAdresMatch(
                    VCode,
                    verwijderdeLocatieId,
                    behoudenLocatieId,
                    locatie.Naam,
                    adresWerdOvergenomen.AdresId
                )
            );

            return;
        }

        AddEvent(adresWerdOvergenomen);
    }

    // public async Task ProbeerAdresTeMatchen(IGrarClient grarClient, int locatieId, CancellationToken cancellationToken)
    // {
    //     var locatie = State.Locaties.SingleOrDefault(s => s.LocatieId == locatieId);
    //
    //     try
    //     {
    //         var @event = await LegacyAdresMatchWrapperService.GetAdresMatchEvent(locatieId, locatie, grarClient, cancellationToken, VCode);
    //
    //         if (@event is not AdresWerdOvergenomenUitAdressenregister adresWerdOvergenomen)
    //         {
    //             AddEvent(@event);
    //
    //             return;
    //         }
    //
    //         if (!NieuweWaardenIndienWerdOvergenomen(adresWerdOvergenomen, locatie))
    //         {
    //             AddEvent(new AdresHeeftGeenVerschillenMetAdressenregister(VCode,
    //                                                                       locatieId,
    //                                                                       adresWerdOvergenomen.AdresId,
    //                                                                       adresWerdOvergenomen.Adres));
    //
    //             return;
    //         }
    //
    //         var stateLocatie = State.Locaties.SingleOrDefault(
    //             sod =>
    //                 sod.LocatieId != locatieId &&
    //                 sod.AdresId is not null &&
    //                 sod.AdresId == adresWerdOvergenomen.AdresId &&
    //                 sod.Naam == locatie.Naam &&
    //                 sod.Locatietype == locatie.Locatietype);
    //
    //         if (stateLocatie is not null)
    //         {
    //             var verwijderdeLocatieId = !stateLocatie.IsPrimair && locatie.IsPrimair ? stateLocatie.LocatieId : locatieId;
    //             var behoudenLocatieId = verwijderdeLocatieId == locatieId ? stateLocatie.LocatieId : locatieId;
    //
    //             AddEvent(adresWerdOvergenomen with
    //             {
    //                 VCode = VCode,
    //                 LocatieId = locatieId
    //             });
    //
    //             AddEvent(new LocatieDuplicaatWerdVerwijderdNaAdresMatch(VCode, verwijderdeLocatieId,
    //                                                                     behoudenLocatieId,
    //                                                                     locatie.Naam,
    //                                                                     adresWerdOvergenomen.AdresId));
    //
    //             return;
    //         }
    //
    //         AddEvent(adresWerdOvergenomen);
    //     }
    //     catch (AdressenregisterReturnedNonSuccessStatusCode ex)
    //     {
    //         AddEvent(new AdresKonNietOvergenomenWordenUitAdressenregister(
    //                      VCode,
    //                      locatieId,
    //                      locatie.Adres.ToAdresString(),
    //                      ex.Message
    //                  ));
    //     }
    //     catch(AdressenregisterReturnedNotFoundStatusCode ex)
    //     {
    //         AddEvent(EventFactory.AdresWerdNietGevondenInAdressenregister(VCode, locatie));
    //     }
    // }

    public async Task NeemAdresDetailOver(
        int locatieId,
        IAddressVerrijkingsService verrijkingService,
        CancellationToken cancellationToken
    )
    {
        var locatie = State.Locaties[locatieId];

        var verrijktAdres = await verrijkingService.FromActiefAdresId(locatie.AdresId!, cancellationToken);

        var adres = Adres.Hydrate(
            verrijktAdres.AddressResponse.Straatnaam,
            verrijktAdres.AddressResponse.Huisnummer,
            verrijktAdres.AddressResponse.Busnummer,
            verrijktAdres.AddressResponse.Postcode,
            verrijktAdres.Gemeente.Naam,
            Adres.België
        );

        var decoratedLocatie = locatie.MetAdresUitGrar(adres);
        State.Locaties.ThrowIfCannotAppendOrUpdate(decoratedLocatie);

        var registratieData = EventFactory.VerrijktAdresUitAdressenregister(verrijktAdres);

        AddEvent(
            new AdresWerdOvergenomenUitAdressenregister(
                VCode,
                locatie.LocatieId,
                verrijktAdres.AddressResponse.AdresId,
                registratieData.Adres
            )
        );
    }

    public void OntkoppelLocatie(int locatieId)
    {
        if (!State.Locaties.HasKey(locatieId))
            return;

        var locatie = State.Locaties[locatieId];

        if (locatie.AdresId is null)
            return;

        AddEvent(
            new AdresWerdOntkoppeldVanAdressenregister(
                VCode,
                locatieId,
                EventFactory.AdresId(locatie.AdresId),
                EventFactory.Adres(locatie.Adres)
            )
        );
    }

    public void AanvaardDubbeleVereniging(VCode dubbeleVereniging)
    {
        Throw<InvalidOperationVerenigingKanGeenDubbelWordenVanZichzelf>.If(dubbeleVereniging.Equals(VCode));
        AddEvent(EventFactory.VerenigingAanvaarddeDubbeleVereniging(VCode, dubbeleVereniging));
    }

    public void AanvaardCorrectieDubbeleVereniging(VCode dubbeleVereniging)
    {
        if (!State.CorresponderendeVCodes.Contains(dubbeleVereniging))
            throw new ApplicationException(
                $"Vereniging kon correctie dubbele vereniging ({dubbeleVereniging}) niet aanvaarden omdat dubbele vereniging "
                    + $"niet voorkomt in de corresponderende VCodes: {string.Join(separator: ',', State.CorresponderendeVCodes)}."
            );

        AddEvent(EventFactory.VerenigingAanvaarddeCorrectieDubbeleVereniging(VCode, dubbeleVereniging));
    }

    [Pure]
    private static bool NieuweWaardenIndienWerdOvergenomen(
        AdresWerdOvergenomenUitAdressenregister @event,
        Locatie locatie
    )
    {
        var adres = Adres.Hydrate(@event.Adres);
        var adresId = AdresId.Hydrate(@event);

        return locatie.Adres != adres || locatie.AdresId != adresId;
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

    public void WijzigBankrekeningnummer(TeWijzigenBankrekeningnummer teWijzigenBankrekeningnummer, string intiator)
    {
        var vorigeTitularis = State
            .Bankrekeningnummers.GetById(teWijzigenBankrekeningnummer.BankrekeningnummerId)
            .Titularis;

        var gewijzigdBankrekeningnummer = State.Bankrekeningnummers.Wijzig(teWijzigenBankrekeningnummer);

        if (gewijzigdBankrekeningnummer is null)
            return;

        AddEvent(
            new BankrekeningnummerWerdGewijzigd(
                gewijzigdBankrekeningnummer.BankrekeningnummerId,
                gewijzigdBankrekeningnummer.Doel,
                gewijzigdBankrekeningnummer.Titularis.Value
            )
        );
    }

    public void Valideer(int bankrekeningnummerId, string initiator)
    {
        var bankrekeningnummer = State.Bankrekeningnummers.SingleOrDefault(x =>
            x.BankrekeningnummerId == bankrekeningnummerId
        );

        Throw<BankrekeningnummerIsNietGekend>.If(bankrekeningnummer == null, bankrekeningnummerId.ToString());
        Throw<BankrekeningnummerValidatieIsAlReedsToegevoegd>.If(
            bankrekeningnummer!.BevestigdDoor.Contains(initiator),
            initiator
        );

        AddEvent(
            new AanwezigheidBankrekeningnummerValidatieDocumentWerdBevestigd(
                bankrekeningnummer.BankrekeningnummerId,
                initiator
            )
        );
    }

    public void MaakValidatieOngedaan(int bankrekeningnummerId, string initiator)
    {
        var bankrekeningnummer = State.Bankrekeningnummers.SingleOrDefault(x =>
            x.BankrekeningnummerId == bankrekeningnummerId
        );

        Throw<BankrekeningnummerIsNietGekend>.If(bankrekeningnummer == null, bankrekeningnummerId.ToString());

        Throw<ValidatieBankrekeningnummerIsNietGekend>.If(
            !bankrekeningnummer!.BevestigdDoor.Contains(initiator),
            initiator
        );

        AddEvent(
            new AanwezigheidBankrekeningnummerValidatieDocumentWerdOngedaanGemaakt(
                bankrekeningnummer.BankrekeningnummerId,
                initiator
            )
        );
    }

    public int VoegBankrekeningToe(ToeTevoegenBankrekeningnummer bankrekeningnummer)
    {
        var toegevoegdBankrekeningnummer = State.Bankrekeningnummers.VoegToe(bankrekeningnummer);

        AddEvent(
            new BankrekeningnummerWerdToegevoegd(
                toegevoegdBankrekeningnummer.BankrekeningnummerId,
                toegevoegdBankrekeningnummer.Iban.Value,
                toegevoegdBankrekeningnummer.Doel,
                toegevoegdBankrekeningnummer.Titularis.Value
            )
        );

        return toegevoegdBankrekeningnummer.BankrekeningnummerId;
    }

    public void VerwijderBankrekeningnummer(int bankrekeningnummerId)
    {
        var bankrekeningnummer = State.Bankrekeningnummers.SingleOrDefault(x =>
            x.BankrekeningnummerId == bankrekeningnummerId
        );

        Throw<BankrekeningnummerIsNietGekend>.If(bankrekeningnummer == null, bankrekeningnummerId.ToString());

        Throw<ActieIsNietToegestaanVoorKboBankrekeningnummer>.If(bankrekeningnummer!.Bron == Bron.KBO);

        AddEvent(
            new BankrekeningnummerWerdVerwijderd(bankrekeningnummer.BankrekeningnummerId, bankrekeningnummer.Iban.Value)
        );
    }

    public int RegistreerErkenning(
        TeRegistrerenErkenning erkenning,
        IpdcProduct ipdcProduct,
        GegevensInitiator initiator
    )
    {
        var toegevoegdeErkenning = Erkenning.Create(State.Erkenningen.NextId, erkenning, ipdcProduct, initiator);

        Throw<ErkenningCombinatieBestaatAl>.If(
            !State.Erkenningen.KanErkenningToevoegenMetCombinatie(toegevoegdeErkenning)
        );

        AddEvent(EventFactory.ErkenningWerdGeregistreerd(toegevoegdeErkenning));

        if (toegevoegdeErkenning.Status == ErkenningStatus.Actief && !State.IsErkend)
            AddEvent(EventFactory.VerenigingWerdErkend());

        return toegevoegdeErkenning.ErkenningId;
    }

    public async Task SchorsErkenning(
        TeSchorsenErkenning teSchorsenErkenning,
        string initiator,
        IOrganisatieBevoegdheidService organisatieBevoegdheidService
    )
    {
        var huidigeErkenning = State.Erkenningen.GetById(teSchorsenErkenning.ErkenningId);

        var isBevoegd = await ValideerBevoegdheidEnVoegErkenningOpvolgersToeAlsBeheerder(
            initiator,
            organisatieBevoegdheidService,
            huidigeErkenning
        );

        Throw<GiIsNietBevoegd>.If(!isBevoegd);
        Throw<ErkenningIsAlReedsGeschorst>.If(huidigeErkenning.Status == ErkenningStatus.Geschorst);
        Throw<ErkenningRedenSchorsingIsVerplicht>.If(string.IsNullOrEmpty(teSchorsenErkenning.RedenSchorsing));
        Throw<VerlopenErkenningKanNietGeschorstWorden>.If(huidigeErkenning.Status == ErkenningStatus.Verlopen);

        AddEvent(EventFactory.ErkenningWerdGeschorst(teSchorsenErkenning));

        if (
            State.IsErkend
            && !State.Erkenningen.Any(e =>
                e.ErkenningId != teSchorsenErkenning.ErkenningId && e.Status == ErkenningStatus.Actief
            )
        )
            AddEvent(EventFactory.VerenigingWerdNietLangerErkend());
    }

    public async Task HefSchorsingErkenningOp(
        int erkenningId,
        string initiator,
        IOrganisatieBevoegdheidService organisatieBevoegdheidService
    )
    {
        var huidigeErkenning = State.Erkenningen.GetById(erkenningId);

        var isBevoegd = await ValideerBevoegdheidEnVoegErkenningOpvolgersToeAlsBeheerder(
            initiator,
            organisatieBevoegdheidService,
            huidigeErkenning
        );

        Throw<GiIsNietBevoegd>.If(!isBevoegd);
        Throw<ErkenningIsNietGeschorst>.If(huidigeErkenning.Status != ErkenningStatus.Geschorst);

        var today = DateOnly.FromDateTime(DateTime.Today);

        var status = ErkenningStatus.Bepaal(huidigeErkenning.ErkenningsPeriode, today);

        AddEvent(EventFactory.HefSchorsingErkenningOp(huidigeErkenning.ErkenningId, status));

        var heeftAndereActieveErkenningen = State.Erkenningen.Any(e =>
            e.ErkenningId != erkenningId && e.Status == ErkenningStatus.Actief
        );

        if (status == ErkenningStatus.Actief && !State.IsErkend && !heeftAndereActieveErkenningen)
            AddEvent(EventFactory.VerenigingWerdErkend());

        if (status != ErkenningStatus.Actief && State.IsErkend && !heeftAndereActieveErkenningen)
            AddEvent(EventFactory.VerenigingWerdNietLangerErkend());
    }

    public async Task CorrigeerRedenSchorsingErkenning(
        TeCorrigerenRedenSchorsingErkenning teCorrigerenRedenSchorsingErkenning,
        string initiator,
        IOrganisatieBevoegdheidService organisatieBevoegdheidService
    )
    {
        Throw<ErkenningRedenSchorsingIsVerplicht>.If(
            string.IsNullOrEmpty(teCorrigerenRedenSchorsingErkenning.RedenSchorsing)
        );

        var huidigeErkenning = State.Erkenningen.GetById(teCorrigerenRedenSchorsingErkenning.ErkenningId);

        var isBevoegd = await ValideerBevoegdheidEnVoegErkenningOpvolgersToeAlsBeheerder(
            initiator,
            organisatieBevoegdheidService,
            huidigeErkenning
        );

        Throw<GiIsNietBevoegd>.If(!isBevoegd);
        Throw<ErkenningIsNietGeschorst>.If(huidigeErkenning.Status != ErkenningStatus.Geschorst);

        var heeftWijzigingen = huidigeErkenning.RedenSchorsing != teCorrigerenRedenSchorsingErkenning.RedenSchorsing;

        if (!heeftWijzigingen)
            return;

        AddEvent(EventFactory.CorrigeerRedenSchorsingErkenning(teCorrigerenRedenSchorsingErkenning));
    }

    public async Task WijzigErkenning(
        TeWijzigenErkenning teWijzigenErkenning,
        string initiator,
        IOrganisatieBevoegdheidService organisatieBevoegdheidService
    )
    {
        var huidigeErkenning = State.Erkenningen.GetById(teWijzigenErkenning.ErkenningId);

        var isBevoegd = await ValideerBevoegdheidEnVoegErkenningOpvolgersToeAlsBeheerder(
            initiator,
            organisatieBevoegdheidService,
            huidigeErkenning
        );

        Throw<GiIsNietBevoegd>.If(!isBevoegd);
        Throw<RedenVanWijzigingIsVerplicht>.If(teWijzigenErkenning.HeeftGeenGeldigeRedenVanWijziging);
        Throw<MinstensEenTeWijzigenVeldMoetIngevuldZijn>.If(teWijzigenErkenning.HeeftGeenTeWijzigenWaarde);

        var erkenningWijziging = ErkenningWijziging.Create(teWijzigenErkenning, huidigeErkenning);

        var heeftWijzigingen = erkenningWijziging.HeeftWijzigingen(huidigeErkenning);

        if (!heeftWijzigingen)
            return;

        var gewijzigdeErkenning = huidigeErkenning.CreateFromErkenningWijziging(erkenningWijziging);

        Throw<ErkenningCombinatieBestaatAl>.If(
            !State.Erkenningen.KanErkenningWijzigenMetCombinatie(gewijzigdeErkenning)
        );

        AddEvent(EventFactory.ErkenningWerdGewijzigd(gewijzigdeErkenning, teWijzigenErkenning.RedenVanWijziging));

        var heeftActieveErkenning = State
            .Erkenningen.Without(gewijzigdeErkenning)
            .Append(gewijzigdeErkenning)
            .Any(e => e.Status == ErkenningStatus.Actief);

        if (heeftActieveErkenning && !State.IsErkend)
            AddEvent(EventFactory.VerenigingWerdErkend());

        if (!heeftActieveErkenning && State.IsErkend)
            AddEvent(EventFactory.VerenigingWerdNietLangerErkend());
    }

    public async Task VerwijderErkenning(
        int erkenningId,
        string initiator,
        IOrganisatieBevoegdheidService organisatieBevoegdheidService
    )
    {
        var huidigeErkenning = State.Erkenningen.GetById(erkenningId);

        var isBevoegd = await ValideerBevoegdheidEnVoegErkenningOpvolgersToeAlsBeheerder(
            initiator,
            organisatieBevoegdheidService,
            huidigeErkenning
        );

        Throw<GiIsNietBevoegd>.If(!isBevoegd);
        Throw<ErkenningIsGeschorst>.If(huidigeErkenning.Status == ErkenningStatus.Geschorst);

        AddEvent(EventFactory.ErkenningWerdVerwijderd(erkenningId));

        if (
            State.IsErkend
            && huidigeErkenning.Status == ErkenningStatus.Actief
            && !State.Erkenningen.Any(e => e.ErkenningId != erkenningId && e.Status == ErkenningStatus.Actief)
        )
            AddEvent(EventFactory.VerenigingWerdNietLangerErkend());
    }

    public void ActiveerErkenning(int erkenningId)
    {
        var erkenning = State.Erkenningen.GetById(erkenningId);
        var today = DateOnly.FromDateTime(DateTime.Today);

        if (!erkenning.KanGeactiveerdWordenOp(today))
            throw new ErkenningKanNietGeactiveerdWorden(
                erkenningId,
                erkenning.ErkenningsPeriode.Startdatum,
                erkenning.ErkenningsPeriode.Einddatum,
                erkenning.Status
            );

        AddEvent(EventFactory.ErkenningWerdGeactiveerd(erkenningId));

        if (
            !State.IsErkend
            && !State.Erkenningen.Any(e => e.ErkenningId != erkenningId && e.Status == ErkenningStatus.Actief)
        )
            AddEvent(EventFactory.VerenigingWerdErkend());
    }

    public void VerloopErkenning(int erkenningId)
    {
        var erkenning = State.Erkenningen.GetById(erkenningId);
        var today = DateOnly.FromDateTime(DateTime.Today);

        if (!erkenning.KanVerlopenWordenOp(today))
            throw new ErkenningKanNietVerlopenWorden(
                erkenningId,
                erkenning.ErkenningsPeriode.Startdatum,
                erkenning.ErkenningsPeriode.Einddatum,
                erkenning.Status
            );

        AddEvent(EventFactory.ErkenningWerdVerlopen(erkenningId));

        if (
            State.IsErkend
            && !State.Erkenningen.Any(e => e.ErkenningId != erkenningId && e.Status == ErkenningStatus.Actief)
        )
            AddEvent(EventFactory.VerenigingWerdNietLangerErkend());
    }

    private async Task<bool> ValideerBevoegdheidEnVoegErkenningOpvolgersToeAlsBeheerder(
        string initiator,
        IOrganisatieBevoegdheidService organisatieBevoegdheidService,
        Erkenning huidigeErkenning
    )
    {
        var isBevoegdeOrganisatie =
            huidigeErkenning.GeregistreerdDoor.OvoCode == initiator || huidigeErkenning.Beheerders.Contains(initiator);

        if (isBevoegdeOrganisatie)
            return true;

        var opvolgers = await organisatieBevoegdheidService.GetOpvolgers(huidigeErkenning.GeregistreerdDoor.OvoCode);
        var isOpvolger = opvolgers.Contains(initiator);

        if (isOpvolger)
            AddEvent(
                EventFactory.ErkenningOpvolgersWerdenToegevoegdAlsBeheerder(huidigeErkenning.ErkenningId, opvolgers)
            );

        return isOpvolger;
    }
}
