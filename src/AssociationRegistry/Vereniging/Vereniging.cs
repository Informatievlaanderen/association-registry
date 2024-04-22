namespace AssociationRegistry.Vereniging;

using Emails;
using Events;
using Exceptions;
using Framework;
using Grar;
using Grar.Exceptions;
using Grar.Models;
using SocialMedias;
using System.Net;
using TelefoonNummers;
using VerenigingWerdVerwijderd = Events.VerenigingWerdVerwijderd;

public class Vereniging : VerenigingsBase, IHydrate<VerenigingState>
{
    public static Vereniging RegistreerFeitelijkeVereniging(
        VCode vCode,
        VerenigingsNaam naam,
        string? korteNaam,
        string? korteBeschrijving,
        Datum? startDatum,
        Doelgroep doelgroep,
        bool uitgeschrevenUitPubliekeDatastroom,
        Contactgegeven[] toeTeVoegenContactgegevens,
        Locatie[] toeTeVoegenLocaties,
        Vertegenwoordiger[] toeTeVoegenVertegenwoordigers,
        HoofdactiviteitVerenigingsloket[] hoofdactiviteitenVerenigingsloketLijst,
        IClock clock)
    {
        Throw<StartdatumMagNietInToekomstZijn>.If(startDatum?.IsInFutureOf(clock.Today) ?? false);

        var toegevoegdeLocaties = Locaties.Empty.VoegToe(toeTeVoegenLocaties);
        var toegevoegdeContactgegevens = Contactgegevens.Empty.VoegToe(toeTeVoegenContactgegevens);
        var toegevoegdeVertegenwoordigers = Vertegenwoordigers.Empty.VoegToe(toeTeVoegenVertegenwoordigers);

        var vereniging = new Vereniging();

        vereniging.AddEvent(
            new FeitelijkeVerenigingWerdGeregistreerd(
                vCode,
                naam,
                korteNaam ?? string.Empty,
                korteBeschrijving ?? string.Empty,
                startDatum?.Value,
                Registratiedata.Doelgroep.With(doelgroep),
                uitgeschrevenUitPubliekeDatastroom,
                ToEventContactgegevens(toegevoegdeContactgegevens),
                ToLocatieLijst(toegevoegdeLocaties),
                ToVertegenwoordigersLijst(toegevoegdeVertegenwoordigers),
                ToHoofdactiviteitenLijst(HoofdactiviteitenVerenigingsloket.FromArray(hoofdactiviteitenVerenigingsloketLijst).ToArray())));

        return vereniging;
    }

    private static Registratiedata.Contactgegeven[] ToEventContactgegevens(Contactgegeven[] contactgegevens)
        => contactgegevens.Select(Registratiedata.Contactgegeven.With).ToArray();

    private static Registratiedata.HoofdactiviteitVerenigingsloket[] ToHoofdactiviteitenLijst(
        HoofdactiviteitVerenigingsloket[] hoofdactiviteitenVerenigingsloketLijst)
        => hoofdactiviteitenVerenigingsloketLijst.Select(Registratiedata.HoofdactiviteitVerenigingsloket.With).ToArray();

    private static Registratiedata.Vertegenwoordiger[] ToVertegenwoordigersLijst(Vertegenwoordiger[] vertegenwoordigersLijst)
        => vertegenwoordigersLijst.Select(Registratiedata.Vertegenwoordiger.With).ToArray();

    private static Registratiedata.Locatie[] ToLocatieLijst(Locatie[] locatieLijst)
        => locatieLijst.Select(Registratiedata.Locatie.With).ToArray();

    public void WijzigNaam(VerenigingsNaam naam)
    {
        if (naam.Equals(State.Naam))
            return;

        AddEvent(new NaamWerdGewijzigd(VCode, naam));
    }

    public void WijzigKorteNaam(string korteNaam)
    {
        if (korteNaam.Equals(State.KorteNaam))
            return;

        AddEvent(new KorteNaamWerdGewijzigd(VCode, korteNaam));
    }

    public void WijzigKorteBeschrijving(string korteBeschrijving)
    {
        if (korteBeschrijving.Equals(State.KorteBeschrijving))
            return;

        AddEvent(new KorteBeschrijvingWerdGewijzigd(VCode, korteBeschrijving));
    }

    public void WijzigStartdatum(Datum? startDatum, IClock clock)
    {
        if (Datum.Equals(State.Startdatum, startDatum))
            return;

        Throw<StartdatumLigtNaEinddatum>.If(State.Einddatum?.IsInPastOf(startDatum) ?? false);
        Throw<StartdatumMagNietInToekomstZijn>.If(startDatum?.IsInFutureOf(clock.Today) ?? false);

        AddEvent(StartdatumWerdGewijzigd.With(State.VCode, startDatum));
    }

    public void Stop(Datum einddatum, IClock clock)
    {
        if (einddatum == State.Einddatum) return;

        Throw<EinddatumMagNietInToekomstZijn>.If(einddatum.IsInFutureOf(clock.Today));
        Throw<EinddatumLigtVoorStartdatum>.If(einddatum.IsInPastOf(State.Startdatum));

        if (State.IsGestopt)
            AddEvent(EinddatumWerdGewijzigd.With(einddatum));
        else
            AddEvent(VerenigingWerdGestopt.With(einddatum));
    }

    public void Verwijder(string reden)
    {
        Throw<VerenigingKanNietVerwijderdWorden>.If(State.IsVerwijderd);
        AddEvent(VerenigingWerdVerwijderd.With(reden));
    }

    public void WijzigDoelgroep(Doelgroep doelgroep)
    {
        if (Doelgroep.Equals(State.Doelgroep, doelgroep)) return;

        AddEvent(DoelgroepWerdGewijzigd.With(doelgroep));
    }

    public void WijzigHoofdactiviteitenVerenigingsloket(HoofdactiviteitVerenigingsloket[] hoofdactiviteitenVerenigingsloket)
    {
        if (HoofdactiviteitenVerenigingsloket.Equals(hoofdactiviteitenVerenigingsloket, State.HoofdactiviteitenVerenigingsloket))
            return;

        var hoofdactiviteiten = HoofdactiviteitenVerenigingsloket.FromArray(hoofdactiviteitenVerenigingsloket);
        AddEvent(HoofdactiviteitenVerenigingsloketWerdenGewijzigd.With(hoofdactiviteiten.ToArray()));
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

    public void SchrijfUitUitPubliekeDatastroom()
    {
        if (State.IsUitgeschrevenUitPubliekeDatastroom) return;
        AddEvent(new VerenigingWerdUitgeschrevenUitPubliekeDatastroom());
    }

    public void SchrijfInInPubliekeDatastroom()
    {
        if (!State.IsUitgeschrevenUitPubliekeDatastroom) return;
        AddEvent(new VerenigingWerdIngeschrevenInPubliekeDatastroom());
    }

    public void Hydrate(VerenigingState obj)
    {
        Throw<ActieIsNietToegestaanVoorVerenigingstype>.If(obj.Verenigingstype != Verenigingstype.FeitelijkeVereniging);
        State = obj;
    }

    public async Task ProbeerAdresTeMatchen(IGrarClient grarClient, int locatieId)
    {
        var locatie = State.Locaties.SingleOrDefault(s => s.LocatieId == locatieId);

        try
        {
            var @event = await GetAdresMatchEvent(locatieId, locatie, grarClient);

            if (@event is AdresWerdOvergenomenUitAdressenregister adresWerdOvergenomenUitAdressenregister)
            {
                if (!NieuweWaardenIndienWerdOvergenomen(adresWerdOvergenomenUitAdressenregister, locatie)) return;
            }

            AddEvent(@event);
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
        IGrarClient grarClient)
    {
        if (locatie is null)
        {
            return new AdresKonNietOvergenomenWordenUitAdressenregister(VCode,
                                                                        locatieId,
                                                                        string.Empty,
                                                                        "Locatie kon niet gevonden worden. Mogelijks is deze verwijderd.");
        }

        var adresMatch = await grarClient.GetAddress(
            locatie.Adres.Straatnaam,
            locatie.Adres.Huisnummer,
            locatie.Adres.Busnummer,
            locatie.Adres.Postcode,
            locatie.Adres.Gemeente);

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
                                                                 .ToArray())
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
                                                                      ex.Message)
        };

        return @event;
    }

    private static bool NieuweWaardenIndienWerdOvergenomen(AdresWerdOvergenomenUitAdressenregister @event, Locatie locatie)
    {
        return HeeftVerschillenBinnenAdres(@event.OvergenomenAdresUitAdressenregister.Adres) ||
               HeeftVerschillenBinnenAdresId(@event.OvergenomenAdresUitAdressenregister.AdresId);

        bool HeeftVerschillenBinnenAdres(Registratiedata.Adres adresUitAdressenregister)
            => (
                adresUitAdressenregister.Straatnaam != locatie.Adres.Straatnaam ||
                adresUitAdressenregister.Huisnummer != locatie.Adres.Huisnummer ||
                adresUitAdressenregister.Busnummer != locatie.Adres.Busnummer ||
                adresUitAdressenregister.Postcode != locatie.Adres.Postcode ||
                adresUitAdressenregister.Gemeente != locatie.Adres.Gemeente ||
                adresUitAdressenregister.Land != locatie.Adres.Land
                );

        bool HeeftVerschillenBinnenAdresId(Registratiedata.AdresId? adresIdUitAdressenregister)
            => (
                adresIdUitAdressenregister?.Broncode != locatie.AdresId?.Adresbron.Code ||
                adresIdUitAdressenregister?.Bronwaarde != locatie.AdresId?.Bronwaarde
                );
    }

    public long Version => State.Version;
}
